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

// ----------------------- grading -----------------------
// Result: 5 / 5 tests PASS. All 4 bugs found and fixed.
//
//   [x] fromString   Title/Author indexes swapped (1 and 0)  -> 0 and 1
//   [x] fromString   Pages = elements.[2]                    -> elements.[3]
//   [x] describe     (%i) printed book.Pages                 -> book.Year
//   [x] oldestFirst  sortBy book.Pages                       -> book.Year
//
// THE LESSON IN THESE BUGS: all 4 compiled fine. The compiler checks
//   TYPES, not MEANING. Title and Author are both strings, and Year and
//   Pages are both ints, so mixing them up within the same type is
//   invisible to F#. Only tests (or careful reading) catch it.
//   Field NAMES help: `Pages = elements.[2]` reads wrong once you know
//   column 2 is the year.
//
// Your version is left in place below, with notes.

// ----------------------- your solution -----------------------
module Book =

    let fromString (row : string) =
        let elements = row.Split('\t')
        {
            Title = elements.[0]          // FIX 1: GOOD. Columns: 0 Title,
            Author = elements.[1]         //   1 Author, 2 Year, 3 Pages.
            Year = int elements.[2]       //   `int` turns "1965" into 1965
            Pages = int elements.[3]      // FIX 2: GOOD. Pages is the LAST column.
        }
        // GOOD: the record is the last expression, so it's what
        //   fromString returns. Same shape as Student.fromString.

    let describe (book : Book) =
        sprintf "%s by %s (%i), %i pages" book.Title book.Author book.Year book.Pages
        // FIX 3: GOOD. Each %-slot is filled in order by the values after
        //   the format string, so the 3rd value lands in "(%i)".
        //   Reading them in order against the expected text catches this.

let oldestFirst (books : Book[]) =
    books
    |> Array.sortBy (fun book -> book.Year)
    // FIX 4: GOOD. `fun book -> book.Year` is a small throwaway function:
    //   "given a book, give back its Year". Array.sortBy calls it on every
    //   book and sorts by the answers, smallest first.


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
