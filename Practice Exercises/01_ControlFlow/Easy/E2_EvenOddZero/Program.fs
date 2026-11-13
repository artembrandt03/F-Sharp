// =====================================================================
//  EASY 2 - Even, Odd or Zero                         (lessons 11, 13)
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

let describe (n: int) =
    // TODO: replace "" with an if / elif / else expression
    ""


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
