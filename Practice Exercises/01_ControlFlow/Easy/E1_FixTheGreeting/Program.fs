// =====================================================================
//  EASY 1 - Fix the Greeting                          (lessons 2, 3)
//  Type: BUGGED - the code compiles, but it's wrong.
// =====================================================================
//
//  `greeting` receives the command-line arguments and builds a greeting:
//
//    * no arguments          ->  "Hello stranger!"
//    * one or more arguments ->  greet ONLY the first one: "Hello Artem!"
//
//  Examples:
//    greeting [| "Artem" |]        = "Hello Artem!"
//    greeting [| "Ann"; "Bob" |]   = "Hello Ann!"
//    greeting [||]                 = "Hello stranger!"
//
//  Your job: run it, read the FAIL / crash output, and fix `greeting`.
//  There are 2 bugs. Don't change the tests in `main`.
//
//  New here: `sprintf` works exactly like `printfn`, but instead of
//  printing the text it RETURNS it as a string.
//
//  Run:  dotnet run          -> runs the tests
//        dotnet run Artem    -> runs the tests + greets you
// =====================================================================


// ----------------------- your attempt -----------------------
// let greeting (args: string[]) =
//     let name =
//         if args.Length-1 >= 0 then
//             for person in args do
//                 sprintf "Hello %s!" person
//         else
//             "stranger"
//     sprintf "Hello %s!" name
//
// What went wrong:
//
//  1. `args.Length-1 >= 0` is actually CORRECT: it means the same as
//     `args.Length > 0` ("there is at least one argument"). This fixed
//     the original `>= 0` bug, which was always true, even for an empty
//     array, so `args.[0]` crashed. `args.Length > 0` just says it more
//     directly.
//
//  2. The `for` loop can't be the value of `name`.
//     `if ... then ... else` is an EXPRESSION: both branches must give a
//     value of the SAME type. The `else` branch gives a string
//     ("stranger"), but a `for` loop gives `unit` (F#'s "nothing"). A loop
//     *does* things, it doesn't *produce* a value. That's the compile
//     error: "expected string but here has type unit".
//     (And the `sprintf` inside the loop builds a string and throws it
//     away, because nothing uses it.)
//
//  3. The task says greet ONLY THE FIRST name, so no loop is needed.
//     The first element is `args.[0]`.
//
//  4. `sprintf "Hello %s!"` appeared twice (inside the loop AND at the
//     end), so even if it compiled you'd get "Hello Hello Artem!!".
//     `name` should be just the name, and the greeting is built once at
//     the end.

// ----------------------- fixed version -----------------------
let greeting (args: string[]) =
    // `name` is ONLY the name (a string), so both branches return a string.
    let name =
        if args.Length > 0 then   // at least one argument? (`>= 0` was always true)
            args.[0]              // take just the first one, no loop
        else
            "stranger"            // empty array: never touch args.[0]
    // Build the greeting once, here. The original bug: the "!" was missing.
    sprintf "Hello %s!" name


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

[<EntryPoint>]
let main argv =
    printfn "--- Tests ---"
    check "one name" "Hello Artem!" (greeting [| "Artem" |])
    check "two names" "Hello Ann!" (greeting [| "Ann"; "Bob" |])
    check "no names" "Hello stranger!" (greeting [||])

    printfn "--- Your run ---"
    printfn "%s" (greeting argv)
    0
