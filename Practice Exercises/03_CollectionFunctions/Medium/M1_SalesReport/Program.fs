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

// ----------------------- grading -----------------------
// Result: DIDN'T COMPILE (0 / 10). The plan was right (good reuse of your
//   own functions), but 4 of the 6 functions had problems. With the fixes
//   below: 10 / 10.
//
//   [ ] 1 productLine   no sprintf, and the function ends on a `let`
//   [ ] 2 quarterSales  pipeline not indented under `let quarters =`
//   [x] 3 yearTotal     correct
//   [x] 4 bestQuarter   correct
//   [ ] 5 summaryLine   `|` instead of `|>`, and wrong output format
//   [ ] 6 report        never maps rows to summary lines
//
// THE BIG ONE (functions 1, 2 and 6): a function returns its LAST
//   EXPRESSION. A `let` only names something for the lines AFTER it, so a
//   function can't end on a `let` (same error as M2_SumOfEvens in
//   01_ControlFlow). After your `let`s, the last line must be the value
//   you want to give back.

// ----------------------- your attempt (1) -----------------------
// let productLine (row: string) : string =
//     let elements = row.Split('\t')
//     let product = elements.[0]
//     let region = "(%s)" elements.[1]
//
//  * `"(%s)" elements.[1]`: a format string does nothing by itself. It
//    needs `sprintf` in front: `sprintf "(%s)" elements.[1]`. Without it,
//    F# tries to call the string "(%s)" like a function.
//  * The function ENDS on `let region = ...`, so it never returns anything.
//    You built the pieces and then didn't put them together.

// ----------------------- fixed (1) -----------------------
let productLine (row: string) : string =
    let elements = row.Split('\t')
    let product = elements.[0]
    let region = elements.[1]
    sprintf "%s (%s)" product region      // last line = the result

// ----------------------- your attempt (2) -----------------------
// let quarterSales (row: string) : float[] =
//     let elements = row.Split('\t')
//     let quarters =
//     elements
//     |> Array.skip 2
//     |> Array.map float
//
//  * The pipeline is at the SAME indentation as `let quarters =`, so F#
//    thinks `let quarters =` has nothing after it ("offside" error).
//    Whatever belongs to a `let` must be indented further than the `let`.
//  * Even indented, the function would end on the `let`. You'd need a
//    last line `quarters`. Simpler: drop the `let` and let the pipeline
//    BE the result. `Array.skip 2` was right (product AND region).

// ----------------------- fixed (2) -----------------------
let quarterSales (row: string) : float[] =
    row.Split('\t')
    |> Array.skip 2                       // skip product + region
    |> Array.map float                    // the pipeline IS the result

// ----------------------- your solution (3 + 4) -----------------------
let yearTotal (row: string) : float =
    quarterSales row |> Array.sum         // GOOD: reuses quarterSales

let bestQuarter (row: string) : float =
    quarterSales row |> Array.max         // GOOD: reuses quarterSales

// ----------------------- your attempt (5) -----------------------
// let summaryLine (row: string) : string =
//     let product = productLine row
//     let total = yearTotal row
//     let best = bestQuarter row
//     let average = quarterSales row | Array.average
//     sprintf "product %s\ttotal %f\tbest %f\taverage %f" product total best average
//
//  GOOD: one `let` per piece, then sprintf as the last line. That's the
//    right shape, and reusing your functions is exactly what was asked.
//  * `|` is a typo for `|>`. A single `|` means something else in F#
//    (pattern matching, later), so it's a syntax error here.
//  * The format must match the spec EXACTLY:
//      expected: "Widget (North): total 472.0, best 143.5, average 118.0"
//      yours:    "product Widget (North)\ttotal 472.000000\tbest ..."
//    - no "product " at the start, and ": " after the name
//    - ", " between parts, not \t (tab)
//    - `%f` prints 6 decimal places; `%0.1f` prints 1

// ----------------------- fixed (5) -----------------------
let summaryLine (row: string) : string =
    let product = productLine row
    let total = yearTotal row
    let best = bestQuarter row
    let average = quarterSales row |> Array.average
    sprintf "%s: total %0.1f, best %0.1f, average %0.1f" product total best average

// ----------------------- your attempt (6) -----------------------
// let report (rows: string[]) : string[] =
//     rows
//     |> Array.skip 1
//     sprintf "%s" rows
//
//  * Skipping the header was right, but nothing turns each row into a
//    summary line. That's `Array.map summaryLine`: "apply summaryLine to
//    every row" (like `Array.map float` applies float to every item).
//  * `sprintf "%s" rows` starts a NEW line that ignores the pipeline above
//    it, uses the ORIGINAL `rows`, and `%s` expects ONE string, not an
//    array. The pipeline's result should be the function's last
//    expression, with no extra line after it.

// ----------------------- fixed (6) -----------------------
let report (rows: string[]) : string[] =
    rows
    |> Array.skip 1                       // drop the header
    |> Array.map summaryLine              // one summary string per product



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
