open System

[<EntryPoint>]
let main argv =

    // let total =
    //     [| for i in 1..1000 -> i * i |]
    //     |> Array.sum

    let total = 
        Array.init 1000 (fun i ->
            let x = i + 1
            x * x)
        |> Array.sum

    printfn "The total is: %i" total 

    // let numbers = Array.init 5 (fun i -> pown 2 i)

    // let initializeZeros = Array.zeroCreate<int> 10
    // initializeZeros.[0] <- 42 //arrays are always mutable

    0

