// =====================================================================
//  EASY 3 - Count the Long Names                      (lessons 11, 13)
//  Type: INCOMPLETE - fill in the TODOs.
// =====================================================================
//
//  Write `countLongNames`, which takes an array of names and a minimum
//  length, and returns HOW MANY names are at least that long.
//
//  Examples:
//    countLongNames [| "Ann"; "Christopher"; "Bob"; "Maria" |] 5  = 2
//    countLongNames [| "Al"; "Jo" |] 3                            = 0
//    countLongNames [||] 1                                        = 0
//
//  Rules:
//    * Use a `for ... in ... do` loop (lesson 13).
//    * Use a `let mutable` counter and update it with `<-`.
//
//  Hints:
//    * A string's length is `name.Length`, same as `argv.Length`.
//    * An `if` WITHOUT an `else` is fine when it just does something
//      (like updating a mutable) instead of producing a value.
//
//  Run:  dotnet run
//        dotnet run Ann Christopher Bob Maria   -> also counts YOUR names
// =====================================================================

let countLongNames (names: string[]) (minLength: int) =
    // TODO 1: create a mutable counter starting at 0
    // TODO 2: loop over every name
    // TODO 3: if the name is long enough, add 1 to the counter
    // TODO 4: return the counter (replace the 0 below)
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
    check "mixed names" 2 (countLongNames [| "Ann"; "Christopher"; "Bob"; "Maria" |] 5)
    check "exact length counts" 3 (countLongNames [| "Anna"; "Liam"; "Noah"; "Ed" |] 4)
    check "none long enough" 0 (countLongNames [| "Al"; "Jo" |] 3)
    check "all long enough" 2 (countLongNames [| "Alexander"; "Elizabeth" |] 1)
    check "empty array" 0 (countLongNames [||] 1)

    printfn "--- Your run ---"
    printfn "%d of your %d names have 5+ letters" (countLongNames argv 5) argv.Length
    0
