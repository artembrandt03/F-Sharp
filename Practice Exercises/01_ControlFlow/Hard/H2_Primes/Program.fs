// =====================================================================
//  HARD 2 - Primes                                    (lessons 11, 13)
//  Type: BUGGED + INCOMPLETE
// =====================================================================
//
//  A prime is a whole number GREATER THAN 1 whose only divisors are
//  1 and itself: 2, 3, 5, 7, 11, 13, ...
//
//  Part A - `isPrime n`: true if n is prime, false otherwise.
//           It's written, but it has 2 bugs.
//
//  Part B - `countPrimesUpTo n`: how many primes are between 1 and n
//           (INCLUDING n).
//             countPrimesUpTo 10 = 4      (2, 3, 5, 7)
//
//  Part C - `largestGap n`: look at all the primes up to n in order and
//           return the biggest difference between two NEIGHBOURING
//           primes. Return 0 if there are fewer than 2 primes.
//             primes up to 30: 2 3 5 7 11 13 17 19 23 29
//             gaps:             1 2 2 4  2  4  2  4  6
//             largestGap 30 = 6
//
//  Hints:
//    * Part A: think about which divisor the loop should START at.
//    * Part C: you need two mutables - the previous prime you saw and
//      the biggest gap so far. What should "previous prime" start as
//      so that the first prime doesn't create a fake gap?
//
//  Run:  dotnet run
// =====================================================================

let isPrime (n: int) =
    let mutable prime = true
    for divisor in 1..n-1 do
        if n % divisor = 0 then
            prime <- false
    prime

let countPrimesUpTo (n: int) =
    // TODO
    0

let largestGap (n: int) =
    // TODO
    0


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

[<EntryPoint>]
let main argv =
    printfn "--- Tests ---"
    check "isPrime 2" true (isPrime 2)
    check "isPrime 13" true (isPrime 13)
    check "isPrime 15" false (isPrime 15)
    check "isPrime 1" false (isPrime 1)
    check "isPrime 0" false (isPrime 0)
    check "isPrime -7" false (isPrime -7)
    check "count up to 10" 4 (countPrimesUpTo 10)
    check "count up to 100" 25 (countPrimesUpTo 100)
    check "count up to 1" 0 (countPrimesUpTo 1)
    check "gap up to 30" 6 (largestGap 30)
    check "gap up to 10" 2 (largestGap 10)
    check "gap up to 100" 8 (largestGap 100)
    check "gap with one prime" 0 (largestGap 2)

    printfn "--- Primes up to 50 ---"
    for i in 1..50 do
        if isPrime i then
            printf "%d " i
    printfn ""
    0
