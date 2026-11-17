open System
open System.IO
open StudentScores

[<EntryPoint>]
let main argv =
    if argv.Length = 1 then
        let filePath = argv.[0]
        if File.Exists filePath then
            printfn "Processing %s" filePath
            try
                Summary.summarize filePath
                0
            with
            | :? FormatException as e ->
                printf "Error: %s" e.Message
                printf "The file is poopy"
                1
            | :? IOException as e ->
                printf "Error: %s" e.Message
                printf "File is open elsewhere"
                1
            | _ as e ->
                printfn "Sum unexpected buffonery! %s" e.Message
                3            
        else
            printfn "File not found: %s" filePath
            4
    else
        printfn "Please specify a file."
        5
