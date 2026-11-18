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

// [Claude Code explaining] Your notes above are right. Here they are
//   sharpened up, plus what this file actually does:
//
// WHAT HAPPENS IN THIS FILE
//   seq { for i in 1..1000 -> i * i } doesn't compute anything when it's
//   created. It's a RECIPE ("how to make the squares"). Seq.sum then asks
//   for the squares one at a time: make 1, add it, forget it, make 4, add
//   it, forget it... So only ONE square exists in memory at any moment.
//   The array version [| for ... |] first builds all 1000 squares in
//   memory, and only then sums them.
//
// ARRAY vs SEQUENCE (refined)
//   Array  = a row of boxes, all filled at creation, side by side in memory.
//            Fast to read by index (arr.[500] goes straight there), cheap to
//            loop over again and again. Costs memory for every element, upfront.
//   Seq    = a recipe that makes elements ON DEMAND (called LAZY). Costs
//            almost no memory, can even be INFINITE (Seq.initInfinite),
//            and if you only need the first 10 you only make 10.
//            The catch: it is RE-RUN every time you loop over it. Loop twice,
//            and every element is computed twice (and a file is read twice).
//            Seq.cache fixes that by remembering the elements the first time.
//
// YOUR TRADEOFF NOTE, said as an interview answer:
//   "Use a sequence when the data is big, possibly infinite, or only passes
//    through a pipeline once. Use an array when you'll read it several times
//    or by index, because a sequence recomputes on every pass."
//
// INEFFICIENCY = doing work you didn't need: computing values nobody uses,
//   holding big collections in memory longer than needed, or reading a
//   file/network twice. Laziness avoids the first two; caching/arrays
//   avoid the third. The lecturer's last rule matters most: MEASURE with
//   realistic data volumes before optimising.
//
// More in Cheat Sheet/FSharp-Cheat-Sheet.md, section "Arrays vs Sequences".
