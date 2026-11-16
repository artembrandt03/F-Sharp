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

// ----------------------- grading -----------------------
// Result: 9 / 13 tests PASS. Parts B, C and D are correct. Part A
//   (`isBanned`) has a logic bug, and the 4 failing tests all come from it.
//   With the fix below: 13 / 13.
//
//   [ ] Part A  isBanned      `<>` should be `=`        (fixed below)
//   [x] Part B  isNotBanned   correct, after fixing `name isBanned` -> `isBanned name`
//   [x] Part C  admitted      correct
//   [x] Part D  announcement  correct
//
// ----------------------- your attempt (Part A) -----------------------
// let isBanned (name: string) =
//     if name.ToLower() <> "eve" || name.ToLower() <> "mallory" then
//         true
//     else
//         false
//     // was just
//     // name.ToLower() <> "eve" || name.ToLower() <> "mallory"
//     // before, added if else to attempt to fix the fails but it didn't help
//
// What went wrong:
//
//  1. `<>` asks the OPPOSITE question. `isBanned` should ask "is it eve OR
//     is it mallory?", which is `=` ... `||` ... `=`. Asking "is it NOT eve OR
//     NOT mallory?" is true for EVERY name, because no name can be both
//     at once:
//
//       name      <> "eve"   <> "mallory"   || (either true?)
//       "Alice"   true       true           true   <- banned?!
//       "eve"     false      true           true
//       "mallory" true       false          true
//
//     That's why the Eve/Mallory tests passed (by luck: they're always true)
//     and every normal name failed, and why `admitted` let nobody in.
//
//  2. Why the if/else didn't help: `if cond then true else false` always
//     gives the same answer as `cond` alone. It just repeats the condition,
//     so the bug was still inside `cond`. Your original one-line version
//     was the better shape: a condition already IS a true/false value.
//
//  TIP: when a bool function is wrong, check it by hand on one name that
//     should be true and one that should be false, like the table above.

// ----------------------- fixed version (Part A) -----------------------
let isBanned (name: string) =
    let lower = name.ToLower()                // lower-case it ONCE, then reuse it
    lower = "eve" || lower = "mallory"        // "is it eve OR is it mallory?"
    //   "Alice" -> false || false = false    (not banned)
    //   "EVE"   -> true  || false = true     (banned)
    //   "Evelyn"-> false || false = false    (= is an exact match, so not banned)

// ----------------------- your solution (Parts B-D) -----------------------
let isNotBanned (name: string) =
    isBanned name |> not          // GOOD: function first, then the argument.
                                  //   Same shape as `isValid` above.

let admitted (names: string[]) =
    names                         // GOOD: a clean pipeline, reusing your two
    |> Array.filter isValid       //   bool functions by name. It was right all
    |> Array.filter isNotBanned   //   along; it only looked broken because
                                  //   isNotBanned got the wrong answers from isBanned.

let announcement (name: string) =
    if name.Length > 6 then           // GOOD: `>` not `>=`, so exactly 6 letters
        sprintf "VIP: %s" name        //   ("Robert") stays a Guest, as the spec says.
    else                              // GOOD: if/else as an expression; both
        sprintf "Guest: %s" name      //   branches return a string.
    // GOOD: `sprintf` from E1 of 01_ControlFlow. `"VIP: " + name` would work too.


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
