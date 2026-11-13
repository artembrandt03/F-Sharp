// =====================================================================
//  MEDIUM 3 - Longest Name                         (lessons 7, 11, 13)
//  Type: INCOMPLETE - write the whole function.
// =====================================================================
//
//  `longestName names` returns the longest name in the array.
//
//    * If several names share the longest length, return the FIRST one.
//    * If the array is empty, return "(none)".
//
//  Examples:
//    longestName [| "Ann"; "Christopher"; "Bob" |]  = "Christopher"
//    longestName [| "Tom"; "Amy"; "Joe" |]          = "Tom"    (tie -> first)
//    longestName [||]                               = "(none)"
//
//  Then `main` uses it on the command-line arguments:
//    dotnet run Ann Christopher Bob
//      -> The longest name is Christopher (11 letters)
//    dotnet run
//      -> Nobody to compare!
//
//  Hints:
//    * Keep the "best so far" in a `let mutable`.
//    * Think about what the best-so-far should start as, and why
//      `>` vs `>=` matters for the tie rule.
//
//  Run:  dotnet run
//        dotnet run Ann Christopher Bob
// =====================================================================

let longestName (names: string[]) =
    // TODO: replace "" with your solution
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
    check "longest in middle" "Christopher" (longestName [| "Ann"; "Christopher"; "Bob" |])
    check "longest at end" "Elizabeth" (longestName [| "Al"; "Maria"; "Elizabeth" |])
    check "tie keeps first" "Tom" (longestName [| "Tom"; "Amy"; "Joe" |])
    check "single name" "Zoe" (longestName [| "Zoe" |])
    check "empty array" "(none)" (longestName [||])

    printfn "--- Your run ---"
    // TODO: if argv is empty, print "Nobody to compare!"
    //       otherwise print "The longest name is X (N letters)"
    0
