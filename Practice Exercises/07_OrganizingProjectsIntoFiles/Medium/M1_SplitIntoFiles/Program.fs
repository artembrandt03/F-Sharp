// =====================================================================
//  MEDIUM 1 - Split Into Files                             (lesson 10)
//  Type: REORGANIZE - no new logic. Move code into files, like lesson 10.
// =====================================================================
//
//  Right now EVERYTHING is in this one file, and it works: the logic
//  tests already pass. Your job is to split it up the way StudentScores
//  was split in lesson 10:
//
//      Parse.fs     module Parse     (the part marked PARSE below)
//      Book.fs      type Book + module Book
//      Shelf.fs     module Shelf
//      Program.fs   only the tests + main stay here
//
//  Requirements:
//    * Every new file starts with  namespace Bookshop
//    * Add the new files to M1_SplitIntoFiles.fsproj in an order that
//      compiles (Program.fs must stay LAST: it has the [<EntryPoint>]).
//    * Program.fs gets whatever line it needs to see Bookshop's modules.
//
//  The "structure" tests at the bottom check that the files exist and
//  that this file no longer contains the moved code. You're done when
//  every test (logic + structure) says PASS.
//
//  IMPORTANT: run it from THIS folder (the structure tests look for the
//  files next to where you run it):
//      cd "Practice Exercises/07_OrganizingProjectsIntoFiles/Medium/M1_SplitIntoFiles"
//      dotnet run
//
//  Stuck? Hints at the VERY BOTTOM of the file.
// =====================================================================

open System.IO
open SplitIntoFilesExercise

// ----------------------- grading -----------------------
// Result: 10 / 10 PASS (4 logic + 6 structure). The split works.
//
//   [x] Parse.fs, Book.fs, Shelf.fs created, each starting with a namespace
//   [x] type Book AND module Book kept together in Book.fs
//   [x] .fsproj order Parse -> Book -> Shelf -> Program (dependency order)
//   [x] Program.fs opens the namespace and keeps only tests + main
//   [~] namespace name: the spec said `Bookshop`, you used
//       `SplitIntoFilesExercise`
//
// GOOD: the .fsproj order is exactly right. Book uses Parse (intOr), Shelf
//   uses Book, and Program uses all three. Each file sits below
//   everything it depends on.
// GOOD: `type Book` and `module Book` share a file and a name, like
//   Student.fs in lesson 10. The type and the functions for it travel
//   together.
//
// ON THE NAMESPACE: it compiles and works, so it's not a bug, just not
//   what was asked. A namespace names what the code IS ("Bookshop": books,
//   shelves), not which exercise it came from. Lesson 10 uses
//   `StudentScores`, the name of the domain. If you want to match the spec,
//   it's a 4-line change: the first line of Parse.fs, Book.fs and Shelf.fs,
//   plus the `open` above. Nothing else refers to the namespace's name.
//
// TIDY-UP (optional): Parse.fs, Book.fs and Shelf.fs still have the
//   "// ===== X -> move to X.fs =====" banner lines that came with the cut.
//   They were instructions for the move; now they're just noise, so you
//   can delete them.

// ======================= everything below STAYS in Program.fs ==========
// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

let programText () =
    if File.Exists "Program.fs" then File.ReadAllText "Program.fs" else ""

[<EntryPoint>]
let main argv =
    let rows = [| "Dune\tFrank Herbert\t412"; "Emma\tJane Austen\t?" |]

    printfn "--- Logic tests ---"
    check "intOr" 0 (Parse.intOr 0 "?")
    check "fromString" { Title = "Dune"; Author = "Frank Herbert"; Pages = 412 } (Book.fromString rows.[0])
    check "describeAll"
        [| "Dune by Frank Herbert, 412 pages"; "Emma by Jane Austen, 0 pages" |]
        (Shelf.describeAll rows)
    check "totalPages" 412 (rows |> Array.map Book.fromString |> Shelf.totalPages)

    printfn "--- Structure tests ---"
    check "Parse.fs exists" true (File.Exists "Parse.fs")
    check "Book.fs exists" true (File.Exists "Book.fs")
    check "Shelf.fs exists" true (File.Exists "Shelf.fs")
    check "Program.fs no longer has module Parse" false (programText().Contains("\nmodule " + "Parse ="))
    check "Program.fs no longer has type Book" false (programText().Contains("\ntype " + "Book ="))
    check "Program.fs no longer has module Shelf" false (programText().Contains("\nmodule " + "Shelf ="))
    0


// =====================================================================
// =====================================================================
//
//               HINTS - LAST RESORT. TRY IT YOURSELF FIRST!
//
//   Read "Use:" first. Only read "Steps:" if you're still stuck.
//
// =====================================================================
// =====================================================================
//
//  1. Creating the files
//     Use:   VS Code's Explorer (New File), lesson 10's Float.fs as a model
//     Steps: Create Parse.fs, Book.fs and Shelf.fs in THIS folder (next
//            to Program.fs). Each one starts with the line
//            `namespace Bookshop`, then the code you cut from here.
//            Book.fs gets BOTH `type Book` and `module Book`.
//
//  2. The .fsproj
//     Use:   <Compile Include="..." />,  the order rule from lesson 10
//     Steps: Add one Compile line per new file inside the <ItemGroup>.
//            A file can only use files listed ABOVE it, so ask: who uses
//            whom? Book uses Parse. Shelf uses Book. Program uses all.
//            That gives the order: Parse, Book, Shelf, Program.
//
//  3. Program.fs
//     Use:   open
//     Steps: After moving the code out, Program.fs can't see Parse, Book
//            or Shelf, because they're inside the Bookshop namespace.
//            Add `open Bookshop` near the top, next to `open System.IO`.
//
//  4. If it still won't build
//     Read the FIRST error only. "The namespace or module 'X' is not
//     defined" usually means a wrong .fsproj order or a missing
//     namespace/open line. Indentation also matters: `module Book =`
//     sits at the left edge, and its functions are indented under it.
// =====================================================================
