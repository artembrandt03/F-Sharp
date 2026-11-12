// For more information see https://aka.ms/fsharp-console-apps

open System

[<EntryPoint>] //attribute - start executing here
// printfn "Hello from F#"
let main argv = // string [] -> int
    printfn "Hello Word from F#"
    printfn "The args are: %A" argv
    0 // return an integer exit