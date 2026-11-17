open System
 
[<EntryPoint>]
let main argv =

    let total =
        seq { for i in 1..1000 -> i * i }
        |> Seq.sum
 
    printfn "The total is: %i" total 

    0

// Side note about inefficiency (to include in the final cheat sheet):
// REMINDER:    
// Infefficcient code leads to
// - Unnecessary calculations
// - Unnecessary memory management
// - Unnecessary disk/network usage

// Side note (to include in the final cheat sheet):
// Array:                        Sequence:
// - Populated on creation       - Elements computed on demand
// - Elems occupy continious     - Elements don't occupy much memory
//  stretch of memory that's
// allocated on creation

// A note I wanted to include about tradeoffs:
// If we repeatedly use & access elems - it might be cheaper
// to compute them and store them once with an array, instead of a sequence
