open System
open System.IO
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Text
open Dotnet
open ProjectSystem

[<EntryPoint>]
let main argv =
    let checker = FSharpChecker.Create(keepAssemblyContents = true)
    let line = 20
    let column = 10

    let projectName = "/Users/davethomas/github/Project-C/Project-C.fsproj"
    let fileName = __SOURCE_FILE__
    let currentSourceText = File.ReadAllText fileName
    
    let projOptions =
        let controller = ProjectController checker
        
        let projectResponse  =
            controller.LoadProject projectName ignore FSIRefs.TFM.NetCore (fun _ _ _ -> () )
            |> Async.RunSynchronously
        //more info than we need
        let projInfo = controller.GetProject projectName 
        let projectOptions = 
            controller.ProjectOptions
            |> Seq.find (fun (name, po) -> name.Contains "Project-C")
            |> snd
        projectOptions

    printfn "%A" projOptions

    let parseResults, checkFileAnswer = 
        checker.ParseAndCheckFileInProject
            (fileName, 0, SourceText.ofString currentSourceText, projOptions)
        |> Async.RunSynchronously

    printfn "%A" parseResults.Errors

    let test =
        match checkFileAnswer with
        | FSharpCheckFileAnswer.Aborted -> None
        | FSharpCheckFileAnswer.Succeeded results ->
            results.GetToolTipText
                (line,
                 column,
                 "",
                 [],
                 FSharpTokenTag.Identifier)
            |> Async.RunSynchronously
            |> Some

    printfn "%A" parseResults.Errors

    printfn "Hello World from F#!"
    0 // return an integer exit code
