// =====================================================================
//  MEDIUM 2 - Sum of Evens                            (lessons 3, 4)
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

// ----------------------- grading -----------------------
// Result: DIDN'T COMPILE (0 / 6). 1 of 3 problems fixed, and the
//   tuple idea is a good one.
//
//   [x] BUG  `i % 2 = 1` -> `i % 2 = 0`     fixed (works for negatives too)
//   [ ] BUG  `lo..hi-1` skips hi            still there
//   [ ] TODO make lo the smaller, hi the larger: right idea, wrong shape
//
// ----------------------- your attempt -----------------------
// let sumOfEvens (a: int) (b: int) =
//     if a > b then let (lo, hi) = (b,a)
//     elif a < b then let (lo, hi) = (a,b)
//     else let (lo, hi) = (a,a)
//
//     let mutable total = 0
//     for i in lo..hi-1 do
//         if i % 2 = 0 then
//             total <- total + i
//     total
//
// What went wrong:
//
//  1. The compile error: "'let' cannot be the final code element in a
//     block". A `let` only NAMES a value for the code that comes after
//     it, inside the same block. Here each `let (lo, hi) = ...` is the
//     last thing in its branch, so nothing after it uses the name, and
//     the branch has no result. F# refuses that.
//
//  2. Even if it compiled, `lo` and `hi` would only exist INSIDE their
//     branch. A name made inside `if ... then` disappears when the branch
//     ends, so the `for` loop below couldn't see them.
//
//     It's the same lesson 3 pattern as `let person = if ... then ... else ...`:
//     put the `let` OUTSIDE, and let the `if` produce the value.
//
//         WRONG:  if cond then let x = 1 else let x = 2
//         RIGHT:  let x = if cond then 1 else 2
//
//  3. The `a = b` branch isn't needed. When a = b, (a, b) and (b, a) are
//     the same pair, so plain `else` already covers it.
//
//  4. `lo..hi-1`: `a..b` in F# INCLUDES b (same as M1's bug 2), so the
//     `-1` skips hi. sumOfEvens 4 4 became the empty range 4..3 = 0.

// ----------------------- fixed version -----------------------
let sumOfEvens (a: int) (b: int) =
    // Your tuple idea, with the `let` moved OUTSIDE the if. The if/else now
    // PRODUCES a pair, and `let (lo, hi) =` takes it apart into two names
    // that the rest of the function can use.
    let (lo, hi) =
        if a > b then (b, a)     // reversed: swap them
        else (a, b)              // already in order, or equal

    // (The version the TODO had in mind, without tuples, is two lets:
    //    let lo = if a < b then a else b
    //    let hi = if a < b then b else a      )

    let mutable total = 0
    for i in lo..hi do           // no -1: `lo..hi` already includes hi
        if i % 2 = 0 then        // your fix: `= 0` is right for -4, -2 and 0
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
