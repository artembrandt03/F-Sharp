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

// ----------------------- grading -----------------------
// Result: 5 / 5 tests PASS - correct, nothing to fix.
//   `dotnet run Ann Christopher Bob Maria` -> "2 of your 4 names have 5+ letters"
//
// Your version is left in place below, with notes on each line.

// ----------------------- your solution -----------------------
let countLongNames (names: string[]) (minLength: int) =
    //1: create a mutable counter starting at 0
    let mutable counter = 0           // GOOD: `let mutable` is needed because the
                                      //   value changes as the loop runs.
    //2: loop over every name
    for name in names do              // GOOD: `for ... in` instead of indexes
                                      //   (0..names.Length-1), so no off-by-one risk.
    //3: if the name is long enough, add 1 to the counter
        if name.Length >= minLength then  // GOOD: `>=`, not `>`. "AT LEAST that
                                          //   long" includes the exact length, which
                                          //   the "exact length counts" test checks.
            counter <- counter + 1    // GOOD: `<-` updates the mutable. An `if` with
                                      //   no `else` is fine because it returns nothing.
    //4: return the counter (replace the 0 below)
    counter                           // GOOD: the last line is the return value,
                                      //   back at the function's indentation (after
                                      //   the loop, not inside it).

// STYLE (optional): comment `//3` sits at the same column as `for`, so it
//   looks like it's outside the loop. The compiler ignores comment
//   indentation, but indenting it with the code it describes is easier to
//   read:
//
//     for name in names do
//         // 3: if the name is long enough, add 1 to the counter
//         if name.Length >= minLength then
//
// PREVIEW (lessons 15 + 17, don't worry about it yet): later you'll be able
//   to write the same thing with no mutable and no loop:
//
// let countLongNames (names: string[]) (minLength: int) =
//     names
//     |> Array.filter (fun name -> name.Length >= minLength)
//     |> Array.length


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
