// =====================================================================
//  EASY 2 - Even, Odd or Zero                         (lessons 3, 4)
//  Type: INCOMPLETE - fill in the TODO.
// =====================================================================
//
//  Write `describe`, which takes an int and returns:
//
//    * "zero"  if the number is 0
//    * "even"  if the number is divisible by 2 (negatives count too!)
//    * "odd"   otherwise
//
//  Examples:
//    describe 0   = "zero"
//    describe 4   = "even"
//    describe 7   = "odd"
//    describe -3  = "odd"
//
//  New here:
//    * `%` is the remainder operator:  7 % 2 = 1,  8 % 2 = 0
//      Careful: -3 % 2 = -1 (not 1!), so think about which check is safe.
//    * `elif` lets you chain conditions:
//          if a then x
//          elif b then y
//          else z
//
//  Run:  dotnet run
// =====================================================================

// ----------------------- grading -----------------------
// Result: 5 / 5 tests PASS - correct, nothing to fix.
//
// Your version is left in place below, with notes on each line.

// ----------------------- your solution -----------------------
let describe (n: int) =
    if n = 0 then        // GOOD: zero is checked FIRST. 0 % 2 = 0, so if the
        "zero"           //   "even" check came first, describe 0 would be "even".
    else if n % 2 = 0 then  // GOOD: testing `= 0`, not `= 1`. -3 % 2 is -1,
        "even"              //   so `n % 2 = 1` would call -3 even. `= 0` works
    else                    //   for negatives too, and `else` catches all odds.
        "odd"
    // GOOD: if/else is used as an EXPRESSION. Every branch returns a string,
    //   with no `mutable` and no `printfn` inside the function.

// STYLE (optional): `else if` works, but F# usually writes `elif`.
//   They do the same thing. With `else if`, the second `if` is technically
//   nested inside the first `else`; `elif` keeps a long chain flat:
//
// let describe (n: int) =
//     if n = 0 then "zero"
//     elif n % 2 = 0 then "even"
//     else "odd"

// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

[<EntryPoint>]
let main argv =
    printfn "--- Tests ---"
    check "zero" "zero" (describe 0)
    check "positive even" "even" (describe 4)
    check "positive odd" "odd" (describe 7)
    check "negative even" "even" (describe -10)
    check "negative odd" "odd" (describe -3)

    printfn "--- From -3 to 3 ---"
    for i in -3..3 do
        printfn "%d is %s" i (describe i)
    0
