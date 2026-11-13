// =====================================================================
//  MEDIUM 2 - Sum of Evens                            (lessons 11, 13)
//  Type: BUGGED + INCOMPLETE - fix the bugs AND finish the TODO.
// =====================================================================
//
//  `sumOfEvens a b` adds up every EVEN number between a and b,
//  INCLUDING both ends. The arguments can come in either order.
//
//  Examples:
//    sumOfEvens 1 10  = 2 + 4 + 6 + 8 + 10 = 30
//    sumOfEvens 10 1  = 30         (same range, just reversed)
//    sumOfEvens 3 3   = 0          (3 is odd)
//    sumOfEvens 4 4   = 4
//    sumOfEvens -4 2  = -4 + -2 + 0 + 2 = -4
//
//  What's wrong right now:
//    * 2 bugs inside the loop.
//    * The TODO: `lo` and `hi` are never swapped, so `sumOfEvens 10 1`
//      loops over nothing. Make `lo` the smaller of a and b and `hi`
//      the larger, using `if ... then ... else` as an expression.
//
//  Hint: remember what -3 % 2 is (see Easy 2).
//
//  Run:  dotnet run
// =====================================================================

let sumOfEvens (a: int) (b: int) =
    // TODO: lo should be the smaller of a and b, hi the larger
    let lo = a
    let hi = b

    let mutable total = 0
    for i in lo..hi-1 do
        if i % 2 = 1 then
            total <- total + i
    total


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

[<EntryPoint>]
let main argv =
    printfn "--- Tests ---"
    check "1 to 10" 30 (sumOfEvens 1 10)
    check "10 to 1 (reversed)" 30 (sumOfEvens 10 1)
    check "single odd" 0 (sumOfEvens 3 3)
    check "single even" 4 (sumOfEvens 4 4)
    check "negatives" -4 (sumOfEvens -4 2)
    check "bigger range" 2550 (sumOfEvens 1 100)
    0
