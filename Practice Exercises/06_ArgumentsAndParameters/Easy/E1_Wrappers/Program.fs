// =====================================================================
//  EASY 1 - Wrappers                                       (lesson 9)
//  Type: BUGGED - it doesn't even compile yet. 3 bugs.
// =====================================================================
//
//  `wrap left right s` puts text on both sides of s:
//      wrap "<" ">" "hi"  = "<hi>"
//
//  Because of currying, giving `wrap` only its FIRST TWO arguments
//  makes a new function that's still waiting for `s`, like
//  `singleQuote = quote '\''` in the lesson:
//      let parens = wrap "(" ")"      ->  parens "x" = "(x)"
//
//  Expected:
//      parens   "x"        = "(x)"
//      brackets "x"        = "[x]"
//      addTag   "hello"    = "[TAG] hello"
//      firstWord "Hello big world" = "Hello"
//      [| "a"; "b" |] |> Array.map (prefixWith "> ")  = [| "> a"; "> b" |]
//
//  The 3 bugs:
//    * 1 stops it compiling (read the error, it's a lesson-23 one).
//    * 2 are logic bugs, both about the ORDER of arguments / parameters.
//
//  Stuck? Hints at the VERY BOTTOM of the file.
//
//  Run:  dotnet run
// =====================================================================

// ----------------------- grading -----------------------
// Result: 6 / 6 tests PASS. All 3 bugs fixed.
//
//   [x] firstWord   added `(s : string)`              (compile error)
//   [x] brackets    wrap "]" "[" -> wrap "[" "]"       (argument order)
//   [x] prefixWith  swapped to (prefix) (s)            (parameter order)
//
// Your version is left in place below, with notes.

// ----------------------- your solution -----------------------
let wrap (left : string) (right : string) (s : string) =
    left + s + right

let parens = wrap "(" ")"      // partial application: wrap with 2 of its 3 args,
let brackets = wrap "[" "]"    //   so `brackets` is a function still waiting for s.
                               // FIX 2: GOOD. left comes first, then right.

let prefixWith (prefix : string) (s : string) =   // FIX 3: GOOD, and the right
    prefix + s                                    //   way to fix it. You swapped
                                                  //   the PARAMETERS, not the
let addTag = prefixWith "[TAG] "                  //   callers.
    // WHY IT MATTERS: partial application always fills parameters from the
    //   LEFT. Before, `prefixWith "[TAG] "` filled `s`, so "[TAG] " was
    //   treated as the text and "hello" as the prefix -> "hello[TAG] ".
    //   With the "setting" (prefix) first and the data (s) last, the same
    //   line means what it says, and so does
    //   `Array.map (prefixWith "> ")`, where Array.map supplies each s.
    //   RULE OF THUMB: settings first, data last.

let firstWord (s : string) =   // FIX 1: GOOD. Without `: string`, F# doesn't
    let words = s.Split(' ')   //   know what `s` is yet, so it can't look up
    words.[0]                  //   `.Split` ("indeterminate type"). Same fix as
                               //   namePart in lesson 9. Whenever you call a
                               //   method after a dot (`s.Split`, `s.Trim`)
                               //   on a parameter, annotate that parameter.


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

[<EntryPoint>]
let main argv =
    printfn "--- Tests ---"
    check "wrap" "<hi>" (wrap "<" ">" "hi")
    check "parens" "(x)" (parens "x")
    check "brackets" "[x]" (brackets "x")
    check "addTag" "[TAG] hello" (addTag "hello")
    check "prefixWith in a pipeline" [| "> a"; "> b" |] ([| "a"; "b" |] |> Array.map (prefixWith "> "))
    check "firstWord" "Hello" (firstWord "Hello big world")
    0


// =====================================================================
// =====================================================================
//
//               HINTS - LAST RESORT. TRY IT YOURSELF FIRST!
//
// =====================================================================
// =====================================================================
//
//  COMPILE ERROR - firstWord
//    "Lookup on object of indeterminate type": F# can't tell what `s` is,
//    so it doesn't know `s.Split` exists. This is the exact error from
//    lesson 9's namePart. Tell F# what type `s` is.
//
//  LOGIC BUG - brackets
//    Which argument of `wrap` goes on the LEFT, and which on the right?
//
//  LOGIC BUG - prefixWith
//    `addTag = prefixWith "[TAG] "` fills in the FIRST parameter. Right
//    now the first parameter is `s` (the text), so "[TAG] " is being
//    treated as the text. The thing you want to fix IN ADVANCE (the
//    prefix) should come first, and the data (s) LAST. That's also what
//    makes it work with Array.map and |>. Swap the parameters, not the
//    callers.
// =====================================================================
