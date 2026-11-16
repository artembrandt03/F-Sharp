// =====================================================================
//  MEDIUM 1 - Payments                                     (lesson 27)
//  Type: WRITE THE CODE - add 1 case, write 5 functions (~35 lines).
//        Do the Easy one first: its CONCEPTS section explains DUs.
// =====================================================================
//
// ---------------------------------------------------------------------
//  CONCEPTS: quick recap + what's new here
// ---------------------------------------------------------------------
//
//  RECAP (see E1_Deliveries for the long version):
//    * A DU value is exactly ONE of its cases:  Cash 12.5  or  Unpaid
//    * Build one with the case name:            Card 20.0
//    * Look inside with match, one branch per case:
//          match payment with
//          | Cash amount -> ...amount...     (amount = the float inside)
//          | Unpaid -> ...
//    * Stack cases to share a branch; use `_` for data you don't need.
//
//  NEW 1: ADDING A CASE TO A DU
//    A DU's cases are a fixed list, but YOU can extend the list. Adding
//        | Refund of float
//    to the type is one line. After that, every `match` that doesn't
//    handle Refund gets an "Incomplete pattern matches" WARNING. That's
//    the great thing about DUs: the compiler gives you a to-do list of
//    every place in your code that needs to know about the new case.
//    (Lesson 27's exercise did exactly this when it added Voided.)
//
//  NEW 2: THE SAME DATA, DIFFERENT MEANINGS
//    Cash, Card and Refund all carry a float, but they MEAN different
//    things, and match lets you treat each one differently:
//        | Cash amount   -> amount             all of it is ours
//        | Card amount   -> amount - 0.5       the bank keeps a 0.50 fee
//        | Refund amount -> -amount            money going OUT
//    A record with an "Amount" field and a "Kind" string couldn't make the
//    compiler check that you handled every kind. A DU can.
//
//  NEW 3: A CASE THAT CARRIES A STRING
//    Voucher of string: the data is the voucher CODE, not money. Every
//    voucher is worth 10.0, whatever its code, so in amountTaken the
//    code doesn't matter:  | Voucher _ -> 10.0
//
// ---------------------------------------------------------------------
//  THE EXERCISE
// ---------------------------------------------------------------------
//
//  A shop's till log, TAB-separated with a header (Samples/Payments.txt):
//
//      Method    Detail
//      Cash      12.50
//      Card      20.00
//      Voucher   SAVE10
//      Unpaid
//      Refund    5.00
//
//  0. ADD the case `Refund of float` to `type Payment` below.
//
//  1. fromString row   -> a Payment. Same shape as TestResult.fromString:
//                         an if/elif chain on the Method column.
//                           "Cash\t12.50"    -> Cash 12.5
//                           "Voucher\tSAVE10"-> Voucher "SAVE10"
//                           "Unpaid\t"       -> Unpaid
//                           "Refund\t5.00"   -> Refund 5.0
//
//  2. amountTaken p    -> the money this payment adds to the till:
//                           Cash x    -> x
//                           Card x    -> x minus the 0.50 card fee
//                           Voucher _ -> 10.0
//                           Unpaid    -> 0.0
//                           Refund x  -> minus x (money OUT)
//
//  3. describe p       -> "Cash 12.50"
//                         "Card 20.00 (fee 0.50)"
//                         "Voucher SAVE10"
//                         "Unpaid"
//                         "Refund 5.00"
//
//  4. tryCash p        -> Some x for Cash x, None for every other case
//                         (like tryEffectiveScore: Some for one, None for the rest)
//
//  5. dayTotal rows    -> skip the header, turn rows into Payments, then
//                         add up amountTaken of each. One pipeline.
//     cashInDrawer rows-> the same, but only the CASH amounts, using
//                         tryCash + Array.choose.
//
//  The placeholders return dummy values so it compiles. Tests that use
//  Refund only pass once you've added the case AND handled it.
//
//  Stuck? Hints at the VERY BOTTOM of the file ("Use:" first, then
//  "Steps:").
//
//  Run:  dotnet run                         -> tests only
//        dotnet run Samples/Payments.txt    -> tests + the day's summary
// =====================================================================

open System.IO

type Payment =
    | Cash of float
    | Card of float
    | Voucher of string
    | Unpaid
    // TODO 0: add the Refund case here

module Payment =

    let fromString (row : string) : Payment =
        // TODO 1
        Unpaid

    let amountTaken (payment : Payment) : float =
        // TODO 2
        0.0

    let describe (payment : Payment) : string =
        // TODO 3
        ""

    let tryCash (payment : Payment) : float option =
        // TODO 4
        None

let dayTotal (rows : string[]) : float =
    // TODO 5a
    0.0

let cashInDrawer (rows : string[]) : float =
    // TODO 5b
    0.0


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

let runTests () =
    let rows =
        [| "Method\tDetail"; "Cash\t12.50"; "Card\t20.00"; "Voucher\tSAVE10"
           "Unpaid\t"; "Refund\t5.00"; "Cash\t7.50" |]

    printfn "--- Tests ---"
    check "fromString Cash" (Cash 12.5) (Payment.fromString "Cash\t12.50")
    check "fromString Card" (Card 20.0) (Payment.fromString "Card\t20.00")
    check "fromString Voucher" (Voucher "SAVE10") (Payment.fromString "Voucher\tSAVE10")
    check "fromString Unpaid" Unpaid (Payment.fromString "Unpaid\t")
    check "amountTaken Cash" 12.5 (Payment.amountTaken (Cash 12.5))
    check "amountTaken Card (fee)" 19.5 (Payment.amountTaken (Card 20.0))
    check "amountTaken Voucher" 10.0 (Payment.amountTaken (Voucher "ANY"))
    check "amountTaken Unpaid" 0.0 (Payment.amountTaken Unpaid)
    check "describe Cash" "Cash 12.50" (Payment.describe (Cash 12.5))
    check "describe Card" "Card 20.00 (fee 0.50)" (Payment.describe (Card 20.0))
    check "describe Voucher" "Voucher SAVE10" (Payment.describe (Voucher "SAVE10"))
    check "describe Unpaid" "Unpaid" (Payment.describe Unpaid)
    check "tryCash Cash" (Some 12.5) (Payment.tryCash (Cash 12.5))
    check "tryCash Card" None (Payment.tryCash (Card 20.0))
    // These need the Refund case (TODO 0):
    check "Refund amountTaken" -5.0 (Payment.fromString "Refund\t5.00" |> Payment.amountTaken)
    check "Refund describe" "Refund 5.00" (Payment.fromString "Refund\t5.00" |> Payment.describe)
    check "dayTotal" 44.5 (dayTotal rows)
    check "cashInDrawer" 20.0 (cashInDrawer rows)

[<EntryPoint>]
let main argv =
    runTests ()

    if argv.Length = 1 then
        let filePath = argv.[0]
        if File.Exists filePath then
            let rows = File.ReadAllLines filePath
            printfn "--- %s ---" filePath
            rows
            |> Array.skip 1
            |> Array.map Payment.fromString
            |> Array.iter (fun p -> printfn "  %s" (Payment.describe p))
            printfn "Day total:      %0.2f" (dayTotal rows)
            printfn "Cash in drawer: %0.2f" (cashInDrawer rows)
            0
        else
            printfn "File not found: %s" filePath
            2
    else
        printfn "(Pass a file to see the day's summary: dotnet run Samples/Payments.txt)"
        1


// =====================================================================
// =====================================================================
//
//               HINTS - LAST RESORT. TRY IT YOURSELF FIRST!
//
//   Read "Use:" first. Only read "Steps:" if you're still stuck.
//
// =====================================================================
// =====================================================================
//
//  0. Adding Refund
//     Use:   one more `| Name of type` line in the type
//     Steps: Copy the `| Cash of float` line and rename it. Then run
//            `dotnet run` and look at the warnings: once your functions
//            have matches, each one that forgets Refund will be listed.
//
//  1. fromString
//     Use:   Split('\t'),  if / elif / else,  float,  the case names
//     Steps: Split the row; method = elements.[0], detail = elements.[1].
//            Then an if/elif chain like TestResult.fromString:
//              if method = "Cash" then Cash (float detail)
//              elif ... "Card"    -> Card (float detail)
//              elif ... "Voucher" -> Voucher detail   (keep it a string)
//              elif ... "Refund"  -> Refund (float detail)
//              else Unpaid
//            The brackets in `Cash (float detail)` matter: without them,
//            F# reads `Cash float detail` as giving Cash TWO things.
//
//  2. amountTaken
//     Use:   match payment with,  one branch per case (5 of them)
//     Steps: Write `match payment with`, then one `| Case x -> ...` per
//            line, following the table in the exercise. For Voucher use
//            `Voucher _` because the code doesn't change the value. For
//            Refund, `-amount` (or 0.0 - amount) makes it negative. Every
//            branch must give a FLOAT, so Unpaid is 0.0, not 0.
//
//  3. describe
//     Use:   match,  sprintf "%0.2f",  sprintf "%s"
//     Steps: Same match shape as amountTaken, but each branch builds text:
//              | Cash amount -> sprintf "Cash %0.2f" amount
//            Card's text includes the fixed "(fee 0.50)" part. Voucher's
//            data is a string, so use %s with it. Unpaid is plain "Unpaid".
//
//  4. tryCash
//     Use:   match,  Some,  None,  stacked cases / `_`
//     Steps: Two outcomes: `| Cash amount -> Some amount`, and everything
//            else -> None. Stack the other four cases above one `-> None`:
//              | Card _
//              | Voucher _
//              | Unpaid
//              | Refund _ -> None
//            (Exactly the shape of lesson 27's `| Excused | Voided -> None`.)
//
//  5. dayTotal / cashInDrawer
//     Use:   Array.skip 1,  Array.map Payment.fromString,
//            Array.map Payment.amountTaken / Array.choose Payment.tryCash,
//            Array.sum
//     Steps: dayTotal: rows |> skip the header |> map to Payments |> map
//            to amounts |> sum.
//            cashInDrawer: the same, but replace the amountTaken step with
//            Array.choose Payment.tryCash, which drops non-cash payments
//            and unwraps the Somes. Then sum.
//            (With the test rows: 12.5 + 19.5 + 10 + 0 - 5 + 7.5 = 44.5,
//             and the cash only is 12.5 + 7.5 = 20.0.)
// =====================================================================
