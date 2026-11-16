// =====================================================================
//  MEDIUM 1 - Sales Report                                 (lesson 17)
//  Type: WRITE THE FUNCTIONS - 6 small functions, ~15 lines in total.
// =====================================================================
//
//  Same idea as StudentScores, but for sales. Samples/Sales.txt is
//  TAB-separated with a header row:
//
//      Product   Region  Q1     Q2     Q3     Q4
//      Widget    North   120.5  98.0   143.5  110.0
//      Gadget    South   50.0   75.5   60.0   90.5
//      ...
//
//  Write these 6 functions (each one has a TODO placeholder below):
//
//    1. productLine row  -> "Widget (North)"
//                           (the product, then its region in brackets)
//
//    2. quarterSales row -> [| 120.5; 98.0; 143.5; 110.0 |]
//                           (the numbers only: skip product AND region)
//
//    3. yearTotal row    -> 472.0     (all four quarters added up)
//
//    4. bestQuarter row  -> 143.5     (the highest quarter)
//
//    5. summaryLine row  -> "Widget (North): total 472.0, best 143.5, average 118.0"
//                           (all numbers with 1 decimal place)
//
//    6. report rows      -> one summaryLine per PRODUCT, so skip the header.
//                           Returns an array of strings. Use a pipeline.
//
//  Rules:
//    * Reuse your own functions! e.g. yearTotal should call quarterSales,
//      and summaryLine should call productLine, yearTotal, etc.
//    * Use lesson 17's collection functions (Array.skip, Array.map,
//      Array.max, Array.average...), not loops.
//
//  New here:
//    * `Array.sum` adds up every item:  Array.sum [| 1.0; 2.5 |] = 3.5
//    * `: float[]` after a function's parameters says what it RETURNS.
//      It's only there so the placeholders compile. You can keep it.
//
//  `main` is already written: like lesson 17, it checks the file
//  argument, then prints your report.
//
//  Run:  dotnet run                        -> tests only
//        dotnet run Samples/Sales.txt      -> tests + the real report
// =====================================================================

open System.IO

let productLine (row: string) : string =
    // TODO 1
    ""

let quarterSales (row: string) : float[] =
    // TODO 2
    [||]

let yearTotal (row: string) : float =
    // TODO 3
    0.0

let bestQuarter (row: string) : float =
    // TODO 4
    0.0

let summaryLine (row: string) : string =
    // TODO 5
    ""

let report (rows: string[]) : string[] =
    // TODO 6
    [||]


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

let printLine (line: string) =
    printfn "%s" line

let runTests () =
    let header = "Product\tRegion\tQ1\tQ2\tQ3\tQ4"
    let widget = "Widget\tNorth\t120.5\t98.0\t143.5\t110.0"
    let gadget = "Gadget\tSouth\t50.0\t75.5\t60.0\t90.5"

    printfn "--- Tests ---"
    check "productLine" "Widget (North)" (productLine widget)
    check "quarterSales" [| 120.5; 98.0; 143.5; 110.0 |] (quarterSales widget)
    check "yearTotal Widget" 472.0 (yearTotal widget)
    check "yearTotal Gadget" 276.0 (yearTotal gadget)
    check "bestQuarter Widget" 143.5 (bestQuarter widget)
    check "bestQuarter Gadget" 90.5 (bestQuarter gadget)
    check "summaryLine Widget"
        "Widget (North): total 472.0, best 143.5, average 118.0"
        (summaryLine widget)
    check "summaryLine Gadget"
        "Gadget (South): total 276.0, best 90.5, average 69.0"
        (summaryLine gadget)
    check "report skips header"
        [| "Widget (North): total 472.0, best 143.5, average 118.0"
           "Gadget (South): total 276.0, best 90.5, average 69.0" |]
        (report [| header; widget; gadget |])
    check "report only header" [||] (report [| header |])

[<EntryPoint>]
let main argv =
    runTests ()

    if argv.Length = 1 then
        let filePath = argv.[0]
        if File.Exists filePath then
            printfn "--- Report: %s ---" filePath
            File.ReadAllLines filePath
            |> report
            |> Array.iter printLine
            0
        else
            printfn "File not found: %s" filePath
            2
    else
        printfn "(Pass a file to see the real report: dotnet run Samples/Sales.txt)"
        1
