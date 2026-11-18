// =====================================================================
//  EASY 1 - Thermostat                                     (lesson 8)
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
//  Reminders from lesson 8:
//    * `Some x` = "there IS a value, and it's x".  `None` = "no value".
//    * Option.defaultValue d  turns  Some x -> x  and  None -> d
//    * Array.choose f  keeps only the Some results of f, unwrapped:
//          Array.choose Reading.tryFromString [| "1.0"; "N/A" |] = [| 1.0 |]
//    * `(Reading.fromStringOr 20.0)` in brackets = the function with its
//      FIRST argument already filled in, so Array.map can give it the rest.
//
//  Run:  dotnet run
// =====================================================================

// ----------------------- grading -----------------------
// Result: 8 / 8 tests PASS, but Bug 2 is only HALF fixed. The tests
//   happened to miss it (my fault as the test writer: they only ever
//   used a default of 20.0).
//
//   [x] Bug 1  tryFromString  Some 0.0 -> None                 solved on your own
//   [~] Bug 2  fromStringOr   0.0 -> 20.0, but should be `d`   half: see below
//   [x] Bug 3  averageKnown   Array.map -> Array.choose        solved with hint
//   [x] Bug 4  countMissing   <> None -> = None                solved with hint
//
// Using the hints for 3 and 4 is fine; that's what they're for. Those
//   two are the core of lesson 8 (choose vs map, and None meaning
//   "missing"), so they're worth re-reading in a day or two.

// ----------------------- your solution -----------------------
module Reading =

    let tryFromString (s : string) =
        if s = "N/A" then
            None                      // FIX 1: GOOD, on your own. "No reading"
        else                          //   isn't "0 degrees". Some 0.0 would claim
            Some (float s)            //   the sensor read 0, and the average
                                      //   would quietly use it. None says "unknown".

    // ---- your attempt (Bug 2) ----
    // let fromStringOr (d : float) (s : string) =
    //     s
    //     |> tryFromString
    //     |> Option.defaultValue 20.0
    //
    //  You replaced one hard-coded number (0.0) with another (20.0). It
    //  passes because every test happens to want 20.0, but `d` is still
    //  never used:
    //      Reading.fromStringOr 5.0 "N/A"   -> 20.0   (should be 5.0!)
    //
    //  The whole point of the `d` parameter is that the CALLER chooses the
    //  default. `fromStringOr 20.0` in main and `fromStringOr 50.0` in
    //  lesson 8 should both work, so pass `d` through.
    //
    //  TIP: if you write a parameter and never use it, something's wrong.
    //    Once Ionide is working (.NET 10), unused parameters show up faded
    //    in the editor.

    // ---- fixed (Bug 2) ----
    let fromStringOr (d : float) (s : string) =
        s
        |> tryFromString              // float option
        |> Option.defaultValue d      // Some x -> x, None -> whatever the caller passed

let averageKnown (readings : string[]) =
    readings
    |> Array.choose Reading.tryFromString   // FIX 3: GOOD. choose keeps only
    |> Array.average                        //   the Somes, unwrapped:
    //   [| "21.0"; "N/A"; "23.0"; "N/A"; "22.0" |] -> [| 21.0; 23.0; 22.0 |] -> 22.0
    //   The old Array.map (fromStringOr 0.0) kept all 5 with zeros -> 13.2.
    //   RULE OF THUMB: map = same number of items out; choose = maybe fewer.
    // NOTE: if EVERY reading were "N/A", choose would give an empty array
    //   and Array.average would crash. That's not tested here, but it's a
    //   classic missing-data edge case to keep in mind.

let countMissing (readings : string[]) =
    readings
    |> Array.filter (fun s -> Reading.tryFromString s = None)   // FIX 4: GOOD.
    |> Array.length
    //   `= None` reads as "is missing". Same as
    //   (fun s -> Option.isNone (Reading.tryFromString s)), which you'll
    //   meet in the medium exercise.


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
    check "fromStringOr other default" 5.0 (Reading.fromStringOr 5.0 "N/A")
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
