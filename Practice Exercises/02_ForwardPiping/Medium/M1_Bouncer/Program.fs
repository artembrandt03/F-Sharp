// =====================================================================
//  MEDIUM 1 - The Bouncer                      (lesson 15 + lesson 11)
//  Type: FILL IN THE BLANKS - about 10 lines of your own code.
// =====================================================================
//
//  You're the bouncer at a club. Names come in from the command line.
//
//  Part A - `isBanned name`: true if the name is "Eve" or "Mallory",
//           IGNORING upper/lower case ("eve", "MALLORY" are banned too).
//
//  Part B - `isNotBanned name`: the opposite of `isBanned`.
//           ONE line: call `isBanned` and pipe the result into `not`.
//
//  Part C - `admitted names`: a pipeline that
//             1. drops blank names (`isValid` is already written for you)
//             2. drops banned names
//           and keeps everyone else in order.
//
//  Part D - `announcement name`: what the bouncer says at the door.
//             * names longer than 6 letters -> "VIP: Christopher"
//             * everyone else               -> "Guest: Bob"
//           (Use `if ... then ... else` as an expression, lesson 11.)
//
//  `main` is already written: it pipes the real arguments through
//  `admitted`, then announces each person with `Array.iter`.
//
//  Hints:
//    * `name.ToLower()` gives a lower-case copy: "EvE".ToLower() = "eve"
//    * `||` means "or":  a = 1 || a = 2
//
//  Run:  dotnet run
//        dotnet run Alice eve Christopher "" MALLORY Bob
// =====================================================================

open System

let isValid (name: string) =
    String.IsNullOrWhiteSpace name |> not

let isBanned (name: string) =
    // TODO (Part A): replace `false`
    false

let isNotBanned (name: string) =
    // TODO (Part B): replace `true`
    true

let admitted (names: string[]) =
    // TODO (Part C): replace `names` with a pipeline
    names

let announcement (name: string) =
    // TODO (Part D): replace ""
    ""


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

let announce name =
    printfn "%s" (announcement name)

[<EntryPoint>]
let main argv =
    printfn "--- Tests ---"
    check "Eve is banned" true (isBanned "Eve")
    check "eve is banned" true (isBanned "eve")
    check "MALLORY is banned" true (isBanned "MALLORY")
    check "Alice is not banned" false (isBanned "Alice")
    check "Evelyn is not banned" false (isBanned "Evelyn")
    check "isNotBanned Bob" true (isNotBanned "Bob")
    check "isNotBanned eve" false (isNotBanned "eve")
    check "admitted"
        [| "Alice"; "Christopher"; "Bob" |]
        (admitted [| "Alice"; "eve"; "Christopher"; ""; "MALLORY"; "Bob" |])
    check "admitted nobody" [||] (admitted [| " "; "Eve" |])
    check "VIP" "VIP: Christopher" (announcement "Christopher")
    check "guest" "Guest: Bob" (announcement "Bob")
    check "exactly 6 is a guest" "Guest: Robert" (announcement "Robert")
    check "7 is a VIP" "VIP: Charles" (announcement "Charles")

    printfn "--- Your run ---"
    argv
    |> admitted
    |> Array.iter announce
    printfn "Doors closed."
    0
