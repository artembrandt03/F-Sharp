// =====================================================================
//  EASY 1 - Running Log                                    (lesson 6)
//  Type: BUGGED - the code compiles, but it has 4 bugs.
// =====================================================================
//
//  A running log is stored like the StudentScores file: one row per
//  runner, TAB-separated, with a header row at the top.
//
//      Name    Run1   Run2   Run3   Run4      <- header
//      Alice   5.5    3.0    8.5    5.0       <- km per run
//      Bob     10.0   12.0
//
//  The functions:
//    runnerName row   -> "Alice"
//    distances row    -> [| 5.5; 3.0; 8.5; 5.0 |]   (everything after the name)
//    longestRun row   -> 8.5
//    averageRun row   -> 5.5
//    runnerCount rows -> how many RUNNERS (the header isn't a runner!)
//    summary row      -> "Alice: longest 8.5 km, average 5.5 km"
//
//  There are 4 bugs spread across these functions. Run it, read the
//  FAIL lines, and fix them. Each fix is small: one word or one
//  number. Don't change the tests.
//
//  Reminder from lesson 6:
//    Array.skip n   drops the first n items
//    Array.map f    applies f to every item, e.g. Array.map float
//    Array.min / Array.max / Array.average / Array.length
//    %0.1f          prints a float with 1 decimal place
//
//  Run:  dotnet run
// =====================================================================

// ----------------------- grading -----------------------
// Result: 10 / 10 tests PASS. All 4 bugs found and fixed.
//
//   [x] distances    Array.skip 2   -> Array.skip 1
//   [x] longestRun   Array.min      -> Array.max
//   [x] runnerCount  Array.length   -> (Array.length) - 1
//   [x] summary      %0.2f          -> %0.1f
//
// Your version is left in place below, with notes on each line.

// ----------------------- your solution -----------------------
let runnerName (row: string) =
    let elements = row.Split('\t')
    elements.[0]                      // (unchanged: this one had no bug)

let distances (row: string) =
    row.Split('\t')
    |> Array.skip 1                   // FIX 1: GOOD. Only the NAME comes before
    |> Array.map float                //   the numbers here. StudentScores skipped 2
                                      //   because it had Name AND Id. Always count
                                      //   the columns of YOUR data before copying a skip.
    // SNEAKY: with `skip 2`, "averageRun Alice" still PASSED, because
    //   [3.0; 8.5; 5.0] happens to average 5.5 too, and "longestRun Bob"
    //   passed because Bob's max is his last run. A passing test doesn't
    //   prove the code is right; the failing ones pointed at the real bug.

let longestRun (row: string) =
    distances row |> Array.max        // FIX 2: GOOD. "longest" = max.

let averageRun (row: string) =
    distances row |> Array.average    // (unchanged: this one had no bug)

let runnerCount (rows: string[]) =
    (rows |> Array.length) - 1        // FIX 3: GOOD. The header row isn't a runner,
                                      //   same as `studentCount` in lesson 6.
    // GOOD: the brackets MATTER here. `-` binds tighter than `|>`, so
    //   `rows |> Array.length - 1` would mean `rows |> (Array.length - 1)`,
    //   i.e. "subtract 1 from a function", which doesn't compile.
    //   You copied the lesson's brackets, and they're exactly right.

let summary (row: string) =
    sprintf "%s: longest %0.1f km, average %0.1f km" (runnerName row) (longestRun row) (averageRun row)
    // FIX 4: GOOD. %0.2f gave "8.50"; %0.1f gives "8.5". The number after
    //   the dot is how many decimal places to print.

// OPTIONAL (not a bug): `summary` calls longestRun and averageRun, and each
//   of those calls `distances`, so the row gets split and parsed twice.
//   For tiny data that's fine. If it mattered, you could parse once:
//
// let summary (row: string) =
//     let runs = distances row
//     sprintf "%s: longest %0.1f km, average %0.1f km"
//         (runnerName row) (Array.max runs) (Array.average runs)


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

let printLine (line: string) =
    printfn "%s" line

[<EntryPoint>]
let main argv =
    let header = "Name\tRun1\tRun2\tRun3\tRun4"
    let alice = "Alice\t5.5\t3.0\t8.5\t5.0"
    let bob = "Bob\t10.0\t12.0"
    let log = [| header; alice; bob |]

    printfn "--- Tests ---"
    check "runnerName" "Alice" (runnerName alice)
    check "distances Alice" [| 5.5; 3.0; 8.5; 5.0 |] (distances alice)
    check "distances Bob" [| 10.0; 12.0 |] (distances bob)
    check "longestRun Alice" 8.5 (longestRun alice)
    check "longestRun Bob" 12.0 (longestRun bob)
    check "averageRun Alice" 5.5 (averageRun alice)
    check "runnerCount" 2 (runnerCount log)
    check "runnerCount only header" 0 (runnerCount [| header |])
    check "summary Alice" "Alice: longest 8.5 km, average 5.5 km" (summary alice)
    check "summary Bob" "Bob: longest 12.0 km, average 11.0 km" (summary bob)

    printfn "--- Report ---"
    printfn "Runners: %i" (runnerCount log)
    log
    |> Array.skip 1
    |> Array.map summary
    |> Array.iter printLine
    0
