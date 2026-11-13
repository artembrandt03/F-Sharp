// =====================================================================
//  HARD 1 - Command-Line Calculator                (lessons 7, 11, 13)
//  Type: BUGGED + INCOMPLETE
// =====================================================================
//
//  `calculate args` takes the command-line arguments in the form
//      [| operation; x; y |]
//  and returns the result as a string.
//
//  Operations (whole numbers only, `div` drops the remainder):
//    add 3 5  -> "8"
//    sub 9 4  -> "5"
//    mul 6 7  -> "42"
//    div 7 2  -> "3"
//
//  Errors (return these exact strings):
//    * not exactly 3 arguments  -> "Usage: <add|sub|mul|div> <x> <y>"
//    * dividing by zero         -> "Error: division by zero"
//    * unknown operation, e.g. pow
//                               -> "Error: unknown operation 'pow'"
//
//  What's wrong right now:
//    * 2 bugs in the code that's already written.
//    * `div` and the unknown-operation error are TODOs.
//
//  New here: `int "42"` turns the string "42" into the number 42.
//  (You can assume x and y are always valid numbers.)
//
//  Run:  dotnet run
//        dotnet run add 3 5
//        dotnet run div 10 0
// =====================================================================

let calculate (args: string[]) =
    if args.Length > 3 then
        "Usage: <add|sub|mul|div> <x> <y>"
    else
        let operation = args.[0]
        let x = int args.[1]
        let y = int args.[2]

        if operation = "add" then
            string (x + y)
        elif operation = "sub" then
            string (y - x)
        elif operation = "mul" then
            string (x * y)
        // TODO: handle "div" (don't forget division by zero!)
        else
            // TODO: return the unknown-operation error instead
            ""


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

[<EntryPoint>]
let main argv =
    let usage = "Usage: <add|sub|mul|div> <x> <y>"

    printfn "--- Tests ---"
    check "add" "8" (calculate [| "add"; "3"; "5" |])
    check "sub" "5" (calculate [| "sub"; "9"; "4" |])
    check "mul" "42" (calculate [| "mul"; "6"; "7" |])
    check "div" "3" (calculate [| "div"; "7"; "2" |])
    check "div by zero" "Error: division by zero" (calculate [| "div"; "7"; "0" |])
    check "negative numbers" "-2" (calculate [| "add"; "-5"; "3" |])
    check "unknown op" "Error: unknown operation 'pow'" (calculate [| "pow"; "2"; "3" |])
    check "too many args" usage (calculate [| "add"; "1"; "2"; "3" |])
    check "too few args" usage (calculate [| "add"; "1" |])
    check "no args" usage (calculate [||])

    printfn "--- Your run ---"
    printfn "%s" (calculate argv)
    0
