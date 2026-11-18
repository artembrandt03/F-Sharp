// =====================================================================
//  EASY 1 - Guest List                                     (lesson 5)
//  Type: FILL IN THE BLANKS - about 4 lines of your own code.
// =====================================================================
//
//  Build a guest list from a list of names:
//
//    * drop names that are blank: "", "   "
//    * drop "Eve" (she's not invited)
//    * keep everyone else, in the same order
//
//  Example:
//    guestList [| "Alice"; ""; "Eve"; "Bob" |]  =  [| "Alice"; "Bob" |]
//
//  Fill in the 3 TODOs:
//
//    1. `isValid`   - true if the name is NOT blank.
//                     Use `String.IsNullOrWhiteSpace` and `|> not`,
//                     on ONE line, like the lesson.
//
//    2. `isAllowed` - true if the name is not "Eve".
//
//    3. `guestList` - a PIPELINE: start with `names`, then pipe it
//                     through `Array.filter isValid`, then through
//                     `Array.filter isAllowed`. One step per line:
//
//                         names
//                         |> ...
//                         |> ...
//
//  Reminder: `x |> f` means `f x`. The value on the left of `|>` goes
//  in as the LAST argument of the function on the right, so don't
//  also write `names` at the end of each line (that was the
//  `argv |> Array.iter sayHello argv` mistake).
//
//  Run:  dotnet run
//        dotnet run Alice Eve Bob     -> greets your guests
// =====================================================================

open System

// ----------------------- grading -----------------------
// Result: 8 / 8 tests PASS - correct, nothing to fix.
//   `dotnet run Alice Eve "" Bob` -> greets only Alice and Bob.
//
// Your version is left in place below, with notes on each line.

// ----------------------- your solution -----------------------
let isValid (name: string) =
    String.IsNullOrWhiteSpace name |> not   // GOOD: one line, as asked.
    // WHY IT WORKS: calling a function (`String.IsNullOrWhiteSpace name`)
    //   binds tighter than `|>`, so F# reads this as
    //       (String.IsNullOrWhiteSpace name) |> not
    //   i.e. "get true/false first, then flip it". It's the same as
    //   `not (String.IsNullOrWhiteSpace name)`, but reads left to right.

let isAllowed (name: string) =
    name <> "Eve"                           // GOOD: `<>` means "not equal".
    // NOTE: this is an exact match, so "eve" or "EVE" would get in. That's
    //   fine here (the tests only ask for "Eve"). M1_Bouncer makes you
    //   handle any capitalization.

let guestList (names: string[]) =
    names                                   // GOOD: start with the data...
    |> Array.filter isValid                 // ...each step gets the previous
    |> Array.filter isAllowed               //   step's result as its LAST argument.
    // GOOD: no extra `names` at the end of each line (the
    //   `argv |> Array.iter sayHello argv` trap). `isValid` and `isAllowed`
    //   are passed by NAME, without calling them, and Array.filter calls
    //   them once per name.
    // GOOD: the function's last expression is the pipeline, so its result
    //   (the filtered array) is what `guestList` returns.
    //
    // For comparison, the same thing WITHOUT pipes reads inside-out:
    //     Array.filter isAllowed (Array.filter isValid names)
    // The pipeline reads in the order things happen, which is why lesson 5
    //   prefers it.


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

let sayHello name =
    printfn "Hello %s, welcome to the party!" name

[<EntryPoint>]
let main argv =
    printfn "--- Tests ---"
    check "isValid normal name" true (isValid "Alice")
    check "isValid empty" false (isValid "")
    check "isValid spaces" false (isValid "   ")
    check "isAllowed Bob" true (isAllowed "Bob")
    check "isAllowed Eve" false (isAllowed "Eve")
    check "guest list" [| "Alice"; "Bob" |] (guestList [| "Alice"; ""; "Eve"; "Bob" |])
    check "keeps order" [| "Zoe"; "Al"; "Mo" |] (guestList [| "Zoe"; " "; "Al"; "Mo" |])
    check "nobody left" [||] (guestList [| "Eve"; "" |])

    printfn "--- Your run ---"
    argv
    |> guestList
    |> Array.iter sayHello
    0
