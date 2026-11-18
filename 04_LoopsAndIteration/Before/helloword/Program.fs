open System

// [<EntryPoint>]
// let main argv =
//     // for i in 0..argv.Length-1 do
//     //     let person = argv.[i]
//     //     printfn "Hello %s from my F# loop!" person
//     for person in argv do
//         printfn "Hello %s from my better F# loop!" person
//     printfn "Nice to meet ya!"
//     0

let sayHello person = 
    printfn "Hello %s from my F# program!" person

[<EntryPoint>]
let main argv = 
    Array.iter sayHello argv
    printfn "Nice to meet ya!"
    0