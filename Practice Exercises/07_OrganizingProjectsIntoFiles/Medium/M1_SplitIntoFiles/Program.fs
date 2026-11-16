// =====================================================================
//  MEDIUM 1 - Split Into Files                             (lesson 25)
//  Type: REORGANIZE - no new logic. Move code into files, like lesson 25.
// =====================================================================
//
//  Right now EVERYTHING is in this one file, and it works: the logic
//  tests already pass. Your job is to split it up the way StudentScores
//  was split in lesson 25:
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

// ======================= PARSE  ->  move to Parse.fs ===================
module Parse =

    let intOr (d : int) (s : string) =
        if s = "?" then d else int s

// ======================= BOOK  ->  move to Book.fs =====================
type Book =
    {
        Title : string
        Author : string
        Pages : int
    }

module Book =

    let fromString (row : string) =
        let elements = row.Split('\t')
        {
            Title = elements.[0]
            Author = elements.[1]
            Pages = Parse.intOr 0 elements.[2]
        }

    let describe (book : Book) =
        sprintf "%s by %s, %i pages" book.Title book.Author book.Pages

// ======================= SHELF  ->  move to Shelf.fs ===================
module Shelf =

    let totalPages (books : Book[]) =
        books
        |> Array.map (fun book -> book.Pages)
        |> Array.sum

    let describeAll (rows : string[]) =
        rows
        |> Array.map Book.fromString
        |> Array.map Book.describe

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
//     Use:   VS Code's Explorer (New File), lesson 25's Float.fs as a model
//     Steps: Create Parse.fs, Book.fs and Shelf.fs in THIS folder (next
//            to Program.fs). Each one starts with the line
//            `namespace Bookshop`, then the code you cut from here.
//            Book.fs gets BOTH `type Book` and `module Book`.
//
//  2. The .fsproj
//     Use:   <Compile Include="..." />,  the order rule from lesson 25
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
