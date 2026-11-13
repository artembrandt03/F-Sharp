// =====================================================================
//  MEDIUM 1 - FizzBuzz                                (lessons 11, 13)
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

let fizzBuzz (n: int) =
    if n % 3 = 0 then "Fizz"
    elif n % 5 = 0 then "Buzz"
    elif n % 15 = 0 then "FizzBuzz"
    else string n

let countFizzBuzzes (upTo: int) =
    let mutable count = 0
    for i in 1..upTo-1 do
        if fizzBuzz i = "FizzBuzz" then
            count <- count + 1
    count


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
