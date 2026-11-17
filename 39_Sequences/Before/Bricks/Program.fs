open System
open Bricks

[<EntryPoint>]
let main argv =

    let bricks =
        // changed to seq for comparisons, then changed back to array
        [|
            do printfn "Creating elements" //would have to 'yield' (claude code -> explain yield later in the final cheat sheet)
                                           // but 'do' works as well
            // So IF ARRAY - Message printed before "All the bricks:"
            // IF SEQ - AFTER (since created on demand)
            (3, 2, ConsoleColor.Yellow)
            (4, 2, ConsoleColor.Green)
            (2, 1, ConsoleColor.Magenta)
            (1, 1, ConsoleColor.Blue)
            (2, 2, ConsoleColor.Red)
            (4, 2, ConsoleColor.Blue)
            (4, 2, ConsoleColor.Magenta)
            (2, 2, ConsoleColor.Magenta)
            (2, 2, ConsoleColor.Red)
            (4, 2, ConsoleColor.Blue)
            (3, 2, ConsoleColor.Magenta)
            (4, 2, ConsoleColor.Green)
            (3, 2, ConsoleColor.Red)
            (4, 1, ConsoleColor.Blue)
            (4, 2, ConsoleColor.Yellow)
            (4, 2, ConsoleColor.Yellow)
            (1, 1, ConsoleColor.Blue)
            (1, 1, ConsoleColor.Green)
            (2, 1, ConsoleColor.Yellow)
            (4, 1, ConsoleColor.Magenta)
        |]
        |> Array.map (fun (sc, sr, cc) -> { StudColumns = sc; StudRows = sr; Color = cc })
        // |> Seq.map ...
        // |> Seq.cache -> will be computed only first time it's requested

    printfn "All the bricks:"
    bricks //In the debugger, bricks is now "bricks [IEnumerable]" which is just a term for "Sequence" in .NET
           // So a series of items of the same type which can be iterated over, filtered, etc.
           // Bricks -> would not be generated (in memory?) unless we *view* them
           // SO elems aren't computed until we ask for them
    |> Array.iter (Brick.printConsole)
    printfn "\n"

    // To convert between SEQ and ARRAY:
    // Array.ofSeq
    // Seq.toArray

    // All arrays are sequences, but not all sequences are arrays


    printfn "Count of the bricks:"
    let count = bricks |> Array.length
    printfn "Count: %i\n" count

    printfn "Stud counts:"
    bricks
    |> Array.map (fun b -> b.StudColumns * b.StudRows)
    |> Array.iter (fun c -> printf "%i; " c)
    printfn "\n"

    printfn "Red bricks (Array.filter):"
    bricks
    |> Array.filter (fun b -> b.Color = ConsoleColor.Red)
    |> Array.iter Brick.printConsole
    printfn "\n"

    printfn "Grouped by color (Array.groupBy):"
    let groupedByColor =
        bricks
        |> Array.groupBy (fun b -> b.Color)
 
    groupedByColor
    |> Array.iter (fun (color, bricks) ->
        printfn "%s:" (color.ToString())
        bricks
        |> Array.iter Brick.printConsole
        printfn ""
    )
    printfn ""

    printfn "Grouped by studcount (Array.groupBy):"
    let groupedByStudCount =
        bricks
        |> Array.groupBy (fun b -> b.StudRows * b.StudColumns)
 
    groupedByStudCount
    |> Array.sortByDescending fst
    |> Array.iter (fun (studCount, bricks) ->
        printfn "%i stud:" studCount
        bricks
        |> Array.iter Brick.printConsole
        printfn "")
    printfn ""

    0 

// WHEN TO USE SEQ/ ARRAY QUICK GUIDE:
// Collection is short-lived e.g only exists to be summarized or piped forwards -> SEQ
// Long collection but only some elements need to be accessed -> SEQ
// Need to itterate over the collection several times -> ARRAY or SEQ with Seq.cache
// When needed to access elements in a collection by index -> ARRAY
// When needed to revisit/access same element in a collectionb multiple times -> ARRAY or SEQ.Item

// But in the end... test with a volume of data