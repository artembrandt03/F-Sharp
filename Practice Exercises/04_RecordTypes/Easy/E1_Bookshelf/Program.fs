// =====================================================================
//  EASY 1 - Bookshelf                                      (lesson 19)
//  Type: BUGGED - the code compiles, but it has 4 bugs.
// =====================================================================
//
//  Each book is one TAB-separated row:
//
//      Title         Author          Year   Pages
//      Dune          Frank Herbert   1965   412
//
//  `Book.fromString` turns a row into a `Book` record, `Book.describe`
//  turns a Book into a sentence, and `oldestFirst` sorts books by the
//  year they came out.
//
//    Book.fromString "Dune\tFrank Herbert\t1965\t412"
//      = { Title = "Dune"; Author = "Frank Herbert"; Year = 1965; Pages = 412 }
//
//    Book.describe dune  = "Dune by Frank Herbert (1965), 412 pages"
//
//  There are 4 bugs. Each fix is small: an index, a field name, or a
//  swap. Don't change the tests.
//
//  Reminders from lesson 19:
//    * Records are built with  { Field1 = value; Field2 = value; ... }
//    * Fields are read with a dot:  book.Title
//    * `module Book` holds the functions that work on Book, so they're
//      called as `Book.fromString`, like `Student.fromString`.
//    * `=` compares two records FIELD BY FIELD, so a test can check a
//      whole record at once.
//    * `Array.sortBy (fun book -> book.Something)` sorts by that field.
//
//  Run:  dotnet run
// =====================================================================

type Book =
    {
        Title : string
        Author : string
        Year : int
        Pages : int
    }

module Book =

    let fromString (row : string) =
        let elements = row.Split('\t')
        {
            Title = elements.[1]
            Author = elements.[0]
            Year = int elements.[2]
            Pages = int elements.[2]
        }

    let describe (book : Book) =
        sprintf "%s by %s (%i), %i pages" book.Title book.Author book.Pages book.Pages

let oldestFirst (books : Book[]) =
    books
    |> Array.sortBy (fun book -> book.Pages)


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

[<EntryPoint>]
let main argv =
    let dune = { Title = "Dune"; Author = "Frank Herbert"; Year = 1965; Pages = 412 }
    let neuromancer = { Title = "Neuromancer"; Author = "William Gibson"; Year = 1984; Pages = 271 }
    let frankenstein = { Title = "Frankenstein"; Author = "Mary Shelley"; Year = 1818; Pages = 280 }

    printfn "--- Tests ---"
    check "fromString Dune" dune (Book.fromString "Dune\tFrank Herbert\t1965\t412")
    check "fromString Neuromancer" neuromancer (Book.fromString "Neuromancer\tWilliam Gibson\t1984\t271")
    check "describe Dune" "Dune by Frank Herbert (1965), 412 pages" (Book.describe dune)
    check "describe Frankenstein" "Frankenstein by Mary Shelley (1818), 280 pages" (Book.describe frankenstein)
    check "oldestFirst"
        [| frankenstein; dune; neuromancer |]
        (oldestFirst [| dune; neuromancer; frankenstein |])

    printfn "--- My shelf, oldest first ---"
    [| "Dune\tFrank Herbert\t1965\t412"
       "Neuromancer\tWilliam Gibson\t1984\t271"
       "Frankenstein\tMary Shelley\t1818\t280" |]
    |> Array.map Book.fromString
    |> oldestFirst
    |> Array.map Book.describe
    |> Array.iter (fun line -> printfn "%s" line)
    0
