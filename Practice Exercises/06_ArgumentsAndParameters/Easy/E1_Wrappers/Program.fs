// =====================================================================
//  EASY 1 - Wrappers                                       (lesson 23)
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

let wrap (left : string) (right : string) (s : string) =
    left + s + right

let parens = wrap "(" ")"
let brackets = wrap "]" "["

let prefixWith (s : string) (prefix : string) =
    prefix + s

let addTag = prefixWith "[TAG] "

let firstWord s =
    let words = s.Split(' ')
    words.[0]


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
//    lesson 23's namePart. Tell F# what type `s` is.
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
