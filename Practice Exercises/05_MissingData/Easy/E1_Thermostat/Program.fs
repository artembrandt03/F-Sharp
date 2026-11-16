// =====================================================================
//  EASY 1 - Thermostat                                     (lesson 21)
//  Type: BUGGED - the code compiles, but it has 4 bugs.
// =====================================================================
//
//  A thermostat logs one temperature per hour. When the sensor glitches,
//  it writes "N/A" instead of a number:
//
//      [| "21.0"; "N/A"; "23.0"; "N/A"; "22.0" |]
//
//  The functions:
//
//    Reading.tryFromString "21.5"   = Some 21.5
//    Reading.tryFromString "N/A"    = None
//
//    Reading.fromStringOr 20.0 "N/A"  = 20.0   (use the default if missing)
//    Reading.fromStringOr 20.0 "18.0" = 18.0
//
//    averageKnown readings   -> average of the REAL readings only
//                               (missing ones are skipped, not counted as 0)
//    countMissing readings   -> how many readings are "N/A"
//
//  There are 4 bugs, one per function. Don't change the tests.
//  If you're stuck, there are hints at the VERY BOTTOM of the file.
//
//  Reminders from lesson 21:
//    * `Some x` = "there IS a value, and it's x".  `None` = "no value".
//    * Option.defaultValue d  turns  Some x -> x  and  None -> d
//    * Array.choose f  keeps only the Some results of f, unwrapped:
//          Array.choose Reading.tryFromString [| "1.0"; "N/A" |] = [| 1.0 |]
//    * `(Reading.fromStringOr 20.0)` in brackets = the function with its
//      FIRST argument already filled in, so Array.map can give it the rest.
//
//  Run:  dotnet run
// =====================================================================

module Reading =

    let tryFromString (s : string) =
        if s = "N/A" then
            Some 0.0
        else
            Some (float s)

    let fromStringOr (d : float) (s : string) =
        s
        |> tryFromString
        |> Option.defaultValue 0.0

let averageKnown (readings : string[]) =
    readings
    |> Array.map (Reading.fromStringOr 0.0)
    |> Array.average

let countMissing (readings : string[]) =
    readings
    |> Array.filter (fun s -> Reading.tryFromString s <> None)
    |> Array.length


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

[<EntryPoint>]
let main argv =
    let log = [| "21.0"; "N/A"; "23.0"; "N/A"; "22.0" |]

    printfn "--- Tests ---"
    check "tryFromString number" (Some 21.5) (Reading.tryFromString "21.5")
    check "tryFromString N/A" None (Reading.tryFromString "N/A")
    check "fromStringOr number" 18.0 (Reading.fromStringOr 20.0 "18.0")
    check "fromStringOr N/A uses default" 20.0 (Reading.fromStringOr 20.0 "N/A")
    check "averageKnown skips missing" 22.0 (averageKnown log)
    check "averageKnown none missing" 21.0 (averageKnown [| "20.0"; "22.0" |])
    check "countMissing" 2 (countMissing log)
    check "countMissing none" 0 (countMissing [| "20.0" |])

    printfn "--- Hourly log (missing hours shown as 20.0) ---"
    log
    |> Array.map (Reading.fromStringOr 20.0)
    |> Array.iter (fun t -> printfn "%0.1f" t)
    0


// =====================================================================
// =====================================================================
//
//               HINTS - LAST RESORT. TRY IT YOURSELF FIRST!
//
// =====================================================================
// =====================================================================
//
//  BUG 1 - tryFromString
//    Look at what it returns for "N/A". "There's no reading" isn't the
//    same as "the reading is 0 degrees". Which of the two option values
//    means "nothing here"?
//
//  BUG 2 - fromStringOr
//    It takes a default `d`... but does it ever USE `d`?
//
//  BUG 3 - averageKnown
//    It fills missing readings with 0.0 and then averages, so every N/A
//    drags the average down. The task says SKIP them. Which lesson-21
//    function keeps only the Some values and throws away the Nones? Use
//    it with Reading.tryFromString instead of the Array.map line.
//
//  BUG 4 - countMissing
//    `<> None` means "is NOT missing". Which comparison means "IS missing"?
// =====================================================================
