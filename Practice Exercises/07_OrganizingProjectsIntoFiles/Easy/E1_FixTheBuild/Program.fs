open Weather

// =====================================================================
//  EASY 1 - Fix the Build                                  (lesson 10)
//  Type: BUGGED - the code itself is fine, but the PROJECT is wired up
//        wrong, so it doesn't compile. 3 bugs.
// =====================================================================
//
//  This project is split into 3 files, like StudentScores in lesson 10:
//
//      Temperature.fs   namespace Weather, module Temperature
//                         (toFahrenheit, describe)
//      Report.fs        module Report: builds a line of text, using Temperature
//      Program.fs       this file: tests + main, using Report
//
//  None of the FUNCTIONS have bugs. The 3 bugs are all about how the
//  files fit together, spread across:
//      E1_FixTheBuild.fsproj,  Report.fs,  Program.fs (this file)
//
//  Read the compiler errors from the TOP; fixing one often makes others
//  disappear.
//
//  Expected once it builds:
//      Report.line 20.0  = "20.0C = 68.0F (mild)"
//
//  Stuck? Hints at the VERY BOTTOM of this file.
//
//  Run:  dotnet run
// =====================================================================

// ----------------------- grading -----------------------
// Result: builds, 3 / 3 tests PASS. All 3 wiring bugs fixed.
//
//   [x] BUG 1  .fsproj   Temperature.fs now listed BEFORE Report.fs
//   [x] BUG 2  Report.fs `namespace Weather` added as its first line
//   [x] BUG 3  Program.fs `open Weather` added (line 1 of this file)
//
// WHY EACH ONE MATTERED:
//   1. F# compiles files top to bottom in the .fsproj's order, and a file
//      can only see files ABOVE it. Report uses Temperature, so
//      Temperature must come first. (Unlike C#, order is part of the
//      program in F#. It also means no two files can depend on each other.)
//   2. Every file except the LAST must start with a namespace or module
//      declaration. That was the FS0222 error. And because Report is now
//      in the SAME namespace as Temperature, it can say
//      `Temperature.toFahrenheit` without any `open`.
//   3. Program.fs isn't in the Weather namespace, so it has to `open` it
//      to use `Report.line` by its short name. Without the open, you'd
//      have to write the full name: `Weather.Report.line`.
//
// STYLE (optional): you put `open Weather` at the very top, above the
//   header comment. That works fine. Most F# code puts the `open` lines
//   right after any file header comment and before the code, the way
//   lesson 10's Program.fs starts with its opens.
//
// NOTE: there are matching one-line notes in Report.fs and the .fsproj.

// ----------------------- tests (don't edit below, except imports) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

[<EntryPoint>]
let main argv =
    printfn "--- Tests ---"
    check "freezing" "0.0C = 32.0F (cold)" (Report.line 0.0)
    check "room" "20.0C = 68.0F (mild)" (Report.line 20.0)
    check "summer" "30.0C = 86.0F (hot)" (Report.line 30.0)
    0


// =====================================================================
// =====================================================================
//
//               HINTS - LAST RESORT. TRY IT YOURSELF FIRST!
//
// =====================================================================
// =====================================================================
//
//  BUG 1 - E1_FixTheBuild.fsproj
//    F# compiles files TOP TO BOTTOM in the order the .fsproj lists them,
//    and a file can only use what's in files ABOVE it. Report uses
//    Temperature... so which has to be listed first?
//
//  BUG 2 - Report.fs
//    Compare its first line with Temperature.fs. Every file except the
//    last one needs to say which namespace (or module) it belongs to,
//    and Report should live in the same namespace as Temperature.
//
//  BUG 3 - Program.fs (this file)
//    Report lives inside the Weather namespace now. Look at how
//    lesson 10's Program.fs gets access to StudentScores' modules, and
//    add the same kind of line near the top of this file.
// =====================================================================
