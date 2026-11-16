// =====================================================================
//  EASY 1 - Deliveries                                     (lesson 27)
//  Type: BUGGED - 4 bugs. Read the CONCEPTS part first!
// =====================================================================
//
// ---------------------------------------------------------------------
//  CONCEPTS: what you need to know for this exercise
// ---------------------------------------------------------------------
//
//  1. WHAT IS A DISCRIMINATED UNION (DU)?
//
//     A RECORD is "this AND this AND this": a Student has a Name AND an
//     Id AND a MeanScore, all at once.
//
//     A DU is "this OR this OR this": a value is EXACTLY ONE of a fixed
//     list of possibilities, called CASES. In lesson 27, a test result
//     is Absent OR Excused OR Voided OR Scored, never two at once.
//
//         type TestResult =
//             | Absent             <- a case with no data
//             | Excused
//             | Voided
//             | Scored of float    <- a case that CARRIES a value
//
//     Think of each case as a labelled box. Some boxes are empty
//     (Absent). Some have something inside (Scored holds a float).
//
//  2. MAKING A VALUE
//
//     Use the case name like a function:
//         let r1 = Absent            // an empty box
//         let r2 = Scored 87.5       // a "Scored" box with 87.5 inside
//     Both r1 and r2 have the SAME type: TestResult.
//
//  3. MATCH: "WHICH CASE IS IT?"
//
//     You can't just read the number out of a TestResult, because it might
//     be Absent and have no number! So you ask which case it is with `match`,
//     and give one branch per case:
//
//         match result with
//         | Absent -> "absent"
//         | Excused -> "excused"
//         | Voided -> "voided"
//         | Scored score -> sprintf "scored %0.1f" score
//
//     Read `| Scored score ->` as: "if it's the Scored box, take what's
//     inside and call it `score`". You choose the name `score`; it only
//     exists inside that branch. It's a NAME for the value, so use it
//     without quotes.
//
//     Like if/else, `match` is an EXPRESSION: every branch must return the
//     same type, and the whole match becomes that value.
//
//  4. SEVERAL CASES, ONE ANSWER
//
//     Stack cases with no `->` between them to share a branch:
//         | Excused
//         | Voided -> None
//     For a case WITH data, use `_` ("whatever's inside, I don't care"):
//         | Shipped _ -> ...
//
//  5. THE COMPILER CHECKS YOU HANDLED EVERY CASE
//
//     If a match forgets a case, F# gives a WARNING:
//         "Incomplete pattern matches on this expression. For example, the
//          value 'Lost' may indicate a case not covered by the pattern(s)."
//     It still builds! But if that case turns up at runtime, the program
//     CRASHES with a MatchFailureException. Treat that warning like an error.
//     (`dotnet run` prints warnings in yellow at the top, so read them.)
//
//  6. DU + OPTION, like tryEffectiveScore
//
//     A match can return an option: Some for cases that have a useful
//     number, None for the others. Then Array.choose keeps only the Somes.
//
// ---------------------------------------------------------------------
//  THE EXERCISE
// ---------------------------------------------------------------------
//
//  A parcel is in one of 4 states:
//
//      // ----------------------- grading -----------------------
// Result: 12 / 12 tests PASS, and NO compiler warning. All 4 bugs fixed.
//
//   [x] fromString  `else Pending` -> `else Lost`                (concept 2)
//   [x] describe    "code" (text) -> code (the value)            (concept 3)
//   [x] describe    added the missing `| Lost ->` branch         (concept 5)
//   [x] tryDays     Some 0 -> None for the no-days cases         (concept 6)
//
// Since you said DUs didn't click at first: these four fixes cover the
//   whole lesson. You built a value with a case name, took data out of
//   a case with match, made a match handle EVERY case, and turned a DU
//   into an option for Array.choose. That's the core of lesson 27.
//
// Your version is left in place below, with notes.

// ----------------------- your solution -----------------------
type Delivery =
    | Pending                 // 4 cases: a Delivery is exactly ONE of these.
    | Shipped of string       //   Two carry data (a tracking code, a number
    | Delivered of int        //   of days); two are empty labels.
    | Lost

module Delivery =

    let fromString (row : string) =
        let elements = row.Split('\t')
        let status = elements.[0]
        let detail = elements.[1]
        if status = "Pending" then
            Pending                       // case name alone = an empty box
        elif status = "Shipped" then
            Shipped detail                // case name + value = a box with
        elif status = "Delivered" then    //   something inside
            Delivered (int detail)        // brackets: Delivered gets ONE thing,
        else                              //   the result of `int detail`
            Lost                          // FIX 1: GOOD.
        // THINK ABOUT (not a bug here): `else` catches EVERY other status,
        //   so a typo like "Delivred" would quietly become Lost. Writing
        //   `elif status = "Lost" then Lost` and handling "unknown" some
        //   other way is safer. Lesson 31 (exceptions) gives you a tool
        //   for that. For now, matching the lesson's TestResult.fromString
        //   style is fine.

    let describe (delivery : Delivery) =
        match delivery with
        | Pending -> "Waiting to ship"
        | Shipped code -> sprintf "On its way (tracking %s)" code
                                          // FIX 2: GOOD. `code` is the NAME
                                          //   for what was inside the box;
                                          //   "code" in quotes is just the
                                          //   4 letters c-o-d-e.
        | Delivered days -> sprintf "Delivered in %i days" days
        | Lost -> "Lost in transit"       // FIX 3: GOOD. 4 cases, 4 branches,
                                          //   so the "incomplete pattern
                                          //   matches" warning is gone and Lost
                                          //   no longer crashes the program.

    let tryDays (delivery : Delivery) =
        match delivery with
        | Delivered days -> Some days     // the ONE case that has a number
        | Pending                         // stacked cases share one answer.
        | Shipped _                       //   `_` = "whatever code is inside,
        | Lost -> None                    //   I don't need it".
                                          // FIX 4: GOOD. "No days yet" isn't
                                          //   "0 days": None, not Some 0.

let averageDays (deliveries : Delivery[]) =
    deliveries
    |> Array.choose Delivery.tryDays   // [Delivered 3; Shipped..; Delivered 5; Pending; Lost]
    |> Array.map float                 //   -> [| 3; 5 |] -> [| 3.0; 5.0 |]
    |> Array.average                   //   -> 4.0  (with Some 0 it was 8/5 = 1.6)


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

[<EntryPoint>]
let main argv =
    let log = [| Delivered 3; Shipped "TRK42"; Delivered 5; Pending; Lost |]

    printfn "--- Tests ---"
    check "fromString Pending" Pending (Delivery.fromString "Pending\t")
    check "fromString Shipped" (Shipped "TRK42") (Delivery.fromString "Shipped\tTRK42")
    check "fromString Delivered" (Delivered 3) (Delivery.fromString "Delivered\t3")
    check "fromString Lost" Lost (Delivery.fromString "Lost\t")
    check "tryDays Delivered" (Some 3) (Delivery.tryDays (Delivered 3))
    check "tryDays Pending" None (Delivery.tryDays Pending)
    check "tryDays Shipped" None (Delivery.tryDays (Shipped "X"))
    check "averageDays" 4.0 (averageDays log)
    check "describe Pending" "Waiting to ship" (Delivery.describe Pending)
    check "describe Shipped" "On its way (tracking TRK42)" (Delivery.describe (Shipped "TRK42"))
    check "describe Delivered" "Delivered in 3 days" (Delivery.describe (Delivered 3))
    check "describe Lost" "Lost in transit" (Delivery.describe Lost)
    0


// =====================================================================
// =====================================================================
//
//               HINTS - LAST RESORT. TRY IT YOURSELF FIRST!
//
// =====================================================================
// =====================================================================
//
//  fromString
//    Follow each status through the if/elif chain. Which status never
//    gets its own branch, so it falls into `else`? What does `else`
//    return, and is that right for it? (Concept 2: making a value.)
//
//  describe (bug A)
//    `Shipped code ->` takes the tracking code out of the box and names
//    it `code`. Look at what's passed to sprintf: is it the VALUE named
//    code, or the TEXT "code"? (Concept 3: names have no quotes.)
//
//  describe (bug B)
//    Read the warning `dotnet run` prints at the top (concept 5). Count
//    the cases in `type Delivery`, then count the branches in describe.
//    Add the missing branch. It has no data, so no name is needed.
//
//  tryDays
//    Only a Delivered parcel has a number of days. For the other three,
//    there's NO number, and "no value" isn't 0 (same idea as lesson 21:
//    a missing reading isn't 0 degrees). Which option value means "no
//    value"? Once fixed, averageDays skips them via Array.choose, exactly
//    like tryEffectiveScore + Array.choose in lesson 27.
// =====================================================================
