// PROGRAM: the outer shell. It connects files, the bank and the report, and it
// is the one place that turns failures into messages and exit codes.

open System
open System.IO
open TinyBank

// Named values instead of magic numbers scattered through the code.
let private historySize = 5
let private forecastSettings = { TargetBalance = 10000m; MaxMonths = 600 }

let private run (accountsPath : string) (transactionsPath : string) (writer : IReportWriter) =
    let directory = AccountDirectory.load accountsPath
    let bank = Bank(directory, historySize)

    // WHY File.ReadLines AND A SEQUENCE: the transactions file could be huge.
    // Each line is read, processed and released, one at a time. ReadAllLines
    // would need the entire file in memory first.
    //
    // WHY NO Seq.cache: we pass over the data exactly once. Caching would keep
    // every row in memory and throw away the benefit of streaming.
    File.ReadLines transactionsPath
    |> Seq.mapi (fun index row -> index + 1, row)
    |> Seq.skip 1 // the header row
    |> Seq.filter (fun (_, row) -> not (String.IsNullOrWhiteSpace row))
    |> Seq.map (fun (lineNumber, row) -> Parse.transaction lineNumber row)
    // Nothing has been read yet. Seq.iter is what finally pulls rows through.
    |> Seq.iter bank.Process

    bank.Snapshot() |> Report.print forecastSettings writer

let private runToConsole accountsPath transactionsPath =
    run accountsPath transactionsPath (ConsoleReportWriter())

let private runToFile accountsPath transactionsPath outputPath =
    let writer = FileReportWriter(outputPath)
    run accountsPath transactionsPath writer
    writer.Save()
    printfn "Report written to %s" outputPath

// try...with is an expression, so every path gives back an exit code.
// Most specific exceptions come first, and only this outer layer catches them:
// lower layers stay simple and problems can't be quietly swallowed.
let private guarded (action : unit -> unit) : int =
    try
        action ()
        0
    with
    | :? FormatException as error ->
        printfn "Invalid accounts file: %s" error.Message
        3
    | :? IOException as error ->
        printfn "File problem: %s" error.Message
        2
    | :? UnauthorizedAccessException as error ->
        printfn "Access denied: %s" error.Message
        2

[<EntryPoint>]
let main argv =
    // An array pattern validates the argument count and names the values.
    match argv with
    | [| accounts; transactions |] ->
        guarded (fun () -> runToConsole accounts transactions)
    | [| accounts; transactions; output |] ->
        guarded (fun () -> runToFile accounts transactions output)
    | _ ->
        printfn "Usage: TinyBank <accounts file> <transactions file> [output file]"
        1
