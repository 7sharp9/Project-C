module App
open Browser.Types
open Browser.Dom
open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.JS

type options =
    abstract debug: bool with get, set

type connection = 
    abstract secure: bool with get, set
    abstract reconnect: bool with get, set

type identity =
    abstract username: string with get, set
    abstract password: string with get, set

type IClientOptions =
    abstract options: options with get, set
    abstract connection: connection with get, set
    abstract identity: identity with get, set
    abstract channels: ResizeArray<string> with get, set

type IClient =
    abstract connect: unit -> Promise<string * int>

    [<Emit("$0.on('message', $1)")>]
    abstract onMessage: ( string -> obj -> string -> bool -> unit) -> unit

    [<Emit("$0.on('connected', $1)")>]
    abstract onConnected:  ( string -> float -> unit) -> unit

    abstract client: IClientOptions -> IClient

[<ImportDefault("tmi.js")>]
let tmi: IClient = jsNative

// Mutable variable to count the number of times we clicked the button
let mutable count = 0

// Get a reference to our button and cast the Element to an HTMLButtonElement
let myButton = document.querySelector(".my-button") :?> Browser.Types.HTMLButtonElement

// Register our listener
myButton.onclick <- fun _ ->
    //debugger()
    let options = 
        jsOptions(fun (o: IClientOptions) ->
            o.channels <- Env.channels |> ResizeArray
            o.connection <- jsOptions(fun (c:connection) -> c.reconnect <- true; c.secure <- true)
            o.identity <- jsOptions(fun (i: identity) -> i.password <- Env.auth; i.username <- Env.username)
            o.options <- jsOptions(fun (o: options) -> o.debug <- true))

    let client = tmi.client options
    client.connect() |> ignore
    client.onConnected(fun host port -> console.info("onConnected", host, port))
    client.onMessage(fun host tags message self -> console.log(message))
    count <- count + 1
