open System
open System.IO
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Text
open Dotnet
open ProjectSystem

[<EntryPoint>]
let main argv =
    let checker = FSharpChecker.Create(keepAssemblyContents = true)
    let line = 24
    let column = 9

// Krzysztof Cieslak: try to do
    let isCompileFile (s:string) =
          s.EndsWith(".fs") || s.EndsWith (".fsi")

    let normalizeOptions (opts : FSharpProjectOptions) =
        { opts with
            SourceFiles = opts.SourceFiles |> Array.map (Path.GetFullPath)
            OtherOptions = opts.OtherOptions |> Array.map (fun n -> if isCompileFile(n) then Path.GetFullPath n else n)
        }

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
        projectOptions |> normalizeOptions

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
            //results.GetAllUsesOfAllSymbolsInFile() |> Async.RunSynchronously |> Some
            results.GetToolTipText
                (line,
                 column,
                 "    let projectName = \"/Users/davethomas/github/Project-C/Project-C.fsproj\"",
                 ["projectName"],
                 FSharpTokenTag.Identifier)
            |> Async.RunSynchronously
            |> Some

    printfn "%A" test
    0 // return an integer exit code
