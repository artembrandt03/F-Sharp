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
//      type Delivery =
//          | Pending                  waiting in the warehouse
//          | Shipped of string        on its way; carries the TRACKING CODE
//          | Delivered of int         arrived; carries how many DAYS it took
//          | Lost
//
//  Each row of the log is "Status<TAB>Detail":
//      "Shipped\tTRK42"  ->  Shipped "TRK42"
//      "Delivered\t3"    ->  Delivered 3
//      "Pending\t"       ->  Pending          (detail is empty)
//
//  Expected:
//      describe Pending          = "Waiting to ship"
//      describe (Shipped "TRK42")= "On its way (tracking TRK42)"
//      describe (Delivered 3)    = "Delivered in 3 days"
//      describe Lost             = "Lost in transit"
//
//      tryDays (Delivered 3) = Some 3
//      tryDays anything else = None   (no "days" to report yet / ever)
//
//  4 bugs: one in fromString, two in describe, one in tryDays.
//  (One of them shows up as a compiler WARNING before it crashes.)
//  averageDays has no bug, but it gives the wrong answer until tryDays is fixed.
//
//  Stuck? Hints at the VERY BOTTOM of the file.
//
//  Run:  dotnet run
// =====================================================================

type Delivery =
    | Pending
    | Shipped of string
    | Delivered of int
    | Lost

module Delivery =

    let fromString (row : string) =
        let elements = row.Split('\t')
        let status = elements.[0]
        let detail = elements.[1]
        if status = "Pending" then
            Pending
        elif status = "Shipped" then
            Shipped detail
        elif status = "Delivered" then
            Delivered (int detail)
        else
            Pending

    let describe (delivery : Delivery) =
        match delivery with
        | Pending -> "Waiting to ship"
        | Shipped code -> sprintf "On its way (tracking %s)" "code"
        | Delivered days -> sprintf "Delivered in %i days" days

    let tryDays (delivery : Delivery) =
        match delivery with
        | Delivered days -> Some days
        | Pending
        | Shipped _
        | Lost -> Some 0

let averageDays (deliveries : Delivery[]) =
    deliveries
    |> Array.choose Delivery.tryDays
    |> Array.map float
    |> Array.average


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
