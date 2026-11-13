// =====================================================================
//  HARD 3 - Palindromes                            (lessons 7, 11, 13)
//  Type: BUGGED + INCOMPLETE
// =====================================================================
//
//  A palindrome reads the same forwards and backwards: "level", "noon".
//
//  Part A - `isPalindrome word`: true if the word is a palindrome,
//           IGNORING upper/lower case ("Anna" counts).
//           The empty string "" and single letters count as palindromes.
//           It's written, but it has 3 bugs (two of them crash).
//
//  Part B - `palindromeReport words`: go through the words and return
//           a summary string in this exact format:
//             palindromeReport [| "Anna"; "Bob"; "cat"; "noon" |]
//               = "3 of 4: Anna, Bob, noon"
//             palindromeReport [| "cat"; "dog" |]
//               = "0 of 2: none"
//             palindromeReport [||]
//               = "0 of 0: none"
//           Keep the words in their ORIGINAL spelling and order,
//           separated by ", ".
//
//  New here:
//    * `word.[i]` gets the letter at position i, just like `argv.[i]`.
//    * `word.ToLower()` returns a lower-case copy of the string.
//    * Strings can be joined with `+`:  "ab" + "cd" = "abcd"
//
//  Run:  dotnet run
//        dotnet run Anna racecar hello Level
// =====================================================================

let isPalindrome (word: string) =
    let mutable result = true
    for i in 0..word.Length / 2 do
        if word.[i] <> word.[word.Length - i] then
            result <- false
    result

let palindromeReport (words: string[]) =
    // TODO
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
    check "level" true (isPalindrome "level")
    check "noon (even length)" true (isPalindrome "noon")
    check "hello" false (isPalindrome "hello")
    check "almost (abca)" false (isPalindrome "abca")
    check "mixed case" true (isPalindrome "RaceCar")
    check "single letter" true (isPalindrome "x")
    check "empty string" true (isPalindrome "")
    check "report mixed" "3 of 4: Anna, Bob, noon" (palindromeReport [| "Anna"; "Bob"; "cat"; "noon" |])
    check "report none" "0 of 2: none" (palindromeReport [| "cat"; "dog" |])
    check "report empty" "0 of 0: none" (palindromeReport [||])
    check "report one" "1 of 1: Level" (palindromeReport [| "Level" |])

    printfn "--- Your run ---"
    printfn "%s" (palindromeReport argv)
    0
