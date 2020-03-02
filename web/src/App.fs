module App

open Browser.Dom
open Fable.Core
open Fable.Core.JsInterop

type identity =  {username: string; password:string }
type opts = { identity: identity; channels: string list }

type MessageArgs = 
    | MessageArgs of channel: string * userstate: string * message: string * self: bool

type Client(opts: opts) =
    // abstract getChannels: unit -> ResizeArray<string>
    // abstract getOptions: unit -> Options
    // abstract getUsername: unit -> string
    // abstract isMod: channel: string * username: string -> bool
    // abstract readyState: unit -> ClientBaseReadyStateReturn
    //abstract on: ``event``: obj option * listener: obj option -> Client
    [<Emit("this.on('message', %1)")>]
    member this.onMessage: MessageArgs -> unit
    // abstract addListener: ``event``: obj option * listener: obj option -> Client
    // abstract removeListener: ``event``: obj option * listener: obj option -> Client
    // abstract removeAllListeners: ?``event``: Events -> Client
    // abstract setMaxListeners: n: float -> Client
    // abstract emits: events: Array<Events> * values: ResizeArray<ResizeArray<obj option>> -> unit
    // abstract emit: (obj option -> bool) with get, set
    // abstract once: ``event``: obj option * listener: obj option -> Client
    // abstract listenerCount: ``event``: Events -> float


let client = Client({identity = {username = ""; password = ""}; channels = [""]})

printfn "%A" client
// Mutable variable to count the number of times we clicked the button
let mutable count = 0

// Get a reference to our button and cast the Element to an HTMLButtonElement
let myButton = document.querySelector(".my-button") :?> Browser.Types.HTMLButtonElement

// Register our listener
myButton.onclick <- fun _ ->
    count <- count + 1
    myButton.innerText <- sprintf "You clicked: %i time(s)" count
