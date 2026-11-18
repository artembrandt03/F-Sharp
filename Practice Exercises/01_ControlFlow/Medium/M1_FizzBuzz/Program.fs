// =====================================================================
//  MEDIUM 1 - FizzBuzz                                (lessons 3, 4)
//  Type: BUGGED - the code compiles, but it's wrong.
// =====================================================================
//
//  Part A - `fizzBuzz n` returns:
//    * "FizzBuzz"  if n is divisible by both 3 and 5
//    * "Fizz"      if n is divisible by 3 only
//    * "Buzz"      if n is divisible by 5 only
//    * the number itself as a string otherwise, e.g. "7"
//
//  Part B - `countFizzBuzzes upTo` counts how many numbers from 1 to
//  upTo (INCLUDING upTo) give "FizzBuzz".
//    countFizzBuzzes 15 = 1      (just 15)
//    countFizzBuzzes 45 = 3      (15, 30, 45)
//
//  There are 2 bugs, one in each function. Find and fix them.
//
//  New here: `string 7` turns a number into the string "7".
//
//  Run:  dotnet run
// =====================================================================

// ----------------------- grading -----------------------
// Result: 8 / 8 tests PASS - both bugs found and fixed.
//
// The original code, for reference:
//
//   if n % 3 = 0 then "Fizz"              <- BUG 1: 15 is divisible by 3, so it
//   elif n % 5 = 0 then "Buzz"               stops here as "Fizz". The
//   elif n % 15 = 0 then "FizzBuzz"          "FizzBuzz" branch could never run.
//   else string n
//
//   for i in 1..upTo-1 do                 <- BUG 2: `1..upTo-1` stops one early,
//                                            so upTo itself is never checked
//                                            (count up to 15 gave 0).
//
// Your version is left in place below, with notes.

// ----------------------- your solution -----------------------
let fizzBuzz (n: int) =
    if n % 3 = 0 && n % 5 = 0 then "FizzBuzz"  // FIX 1: GOOD. The most specific
    elif n % 3 = 0 then "Fizz"                 //   case goes FIRST. In an if/elif
    elif n % 5 = 0 then "Buzz"                 //   chain the first true branch
    else string n                              //   wins, and the rest are skipped.
    // GOOD: you also removed the old `n % 15 = 0` line instead of leaving it
    //   as dead code. `&&` ("and") is new, and it's a clear way to say
    //   "divisible by both". `n % 15 = 0` would work the same.

let countFizzBuzzes (upTo: int) =
    let mutable count = 0
    for i in 1..upTo do               // FIX 2: GOOD. In F# `a..b` already
                                      //   INCLUDES b, so no `-1` is needed.
                                      //   (`0..argv.Length-1` in lesson 4 needed -1
                                      //   only because indexes stop at Length-1.)
        if fizzBuzz i = "FizzBuzz" then
            count <- count + 1
    count

// STYLE (optional): `fizzBuzz i = "FizzBuzz"` works, but it depends on the
//   exact text. A typo like "FizzBuz" would quietly give 0. Checking the
//   number directly doesn't depend on any strings:
//
//         if i % 15 = 0 then
//             count <- count + 1
//
//   Reusing `fizzBuzz` like you did is also a fine choice, since the rule
//   stays in one place. Either is OK.


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

[<EntryPoint>]
let main argv =
    printfn "--- Tests ---"
    check "plain number" "7" (fizzBuzz 7)
    check "fizz" "Fizz" (fizzBuzz 9)
    check "buzz" "Buzz" (fizzBuzz 10)
    check "fizzbuzz 15" "FizzBuzz" (fizzBuzz 15)
    check "fizzbuzz 30" "FizzBuzz" (fizzBuzz 30)
    check "count up to 14" 0 (countFizzBuzzes 14)
    check "count up to 15" 1 (countFizzBuzzes 15)
    check "count up to 45" 3 (countFizzBuzzes 45)

    printfn "--- 1 to 15 ---"
    for i in 1..15 do
        printfn "%s" (fizzBuzz i)
    0
