// =====================================================================
//  EASY 1 - Fix the Greeting                          (lessons 7, 11)
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

let greeting (args: string[]) =
    let name =
        if args.Length >= 0 then
            args.[0]
        else
            "stranger"
    sprintf "Hello %s" name


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
