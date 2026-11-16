// =====================================================================
//  MEDIUM 1 - Movie Ratings                     (lesson 19 + lesson 17)
//  Type: WRITE THE FUNCTIONS - 5 functions, ~25 lines in total.
// =====================================================================
//
//  Samples/Movies.txt has one movie per row, TAB-separated, with a
//  header row. After the title and year comes ANY number of ratings:
//
//      Title        Year   Ratings...
//      Inception    2010   9.0   8.0   9.5   7.5
//      Casablanca   1942   9.0   9.5   8.5
//
//  The `Movie` record is already defined below. Write:
//
//    1. Movie.fromString row  -> a Movie record:
//         Title         "Inception"
//         Year          2010          (an int)
//         RatingCount   4             (how many ratings)
//         AverageRating 8.5
//         BestRating    9.5
//
//    2. Movie.describe movie  ->
//         "Inception (2010): 8.5 average from 4 ratings, best 9.5"
//
//    3. Movie.isClassic movie -> true if it came out BEFORE 1980
//
//    4. bestFirst movies      -> the movies sorted by AverageRating,
//                                HIGHEST first
//
//    5. report rows           -> skip the header, turn each row into a
//                                Movie, sort best first, and describe
//                                each one. Returns an array of strings.
//                                One pipeline, reusing 1, 2 and 4.
//
//  Hints:
//    * Structure it like `Student.fromString` in lesson 19: split, pull
//      out the pieces with `let`, then build the record LAST.
//    * Parse the ratings ONCE into a `let ratings = ...` and use it for
//      the count, the average and the best.
//    * `int "2010"` turns a string into an int (lesson 7 exercises).
//
//  New here:
//    * `Array.sortByDescending (fun m -> m.Something)` is like
//      Array.sortBy, but biggest first.
//
//  The placeholders return an "empty" Movie or "" so the file compiles.
//  Replace them.
//
//  Run:  dotnet run                         -> tests only
//        dotnet run Samples/Movies.txt      -> tests + the real report
// =====================================================================

open System.IO

type Movie =
    {
        Title : string
        Year : int
        RatingCount : int
        AverageRating : float
        BestRating : float
    }

// ----------------------- worked solution -----------------------
// You asked for the full solution, so this is a WALKTHROUGH, not a grade.
// Your partial start on `fromString` is kept (commented out) with notes.
//
// ----------------------- your start (1) -----------------------
//     let fromString (row : string) : Movie =
//         let elements = row.Split('\t')
//         {
//             Title = elements.[0]
//             Year = elements.[1]
//             RatingCount = (elements.Length - 2)
//             AverageRating
//         }
//
//  GOOD: split first, then build the record last. That's the right shape.
//  GOOD: `elements.Length - 2` for the count is CORRECT: every column
//    except Title and Year is a rating. (Below I count the parsed ratings
//    array instead, so all three numbers come from the same place.)
//  * `Year = elements.[1]`: elements are strings and Year is an int, so it
//    needs `int elements.[1]`. (Compile error: "expected int but here has
//    type string".)
//  * `AverageRating` with no `= value`: every field needs a value, and
//    BestRating was missing too. A record must have ALL its fields.

module Movie =

    // 1. Row -> Movie record -------------------------------------------
    let fromString (row : string) : Movie =
        let elements = row.Split('\t')
        //   "Inception\t2010\t9.0\t8.0\t9.5\t7.5"
        //   -> [| "Inception"; "2010"; "9.0"; "8.0"; "9.5"; "7.5" |]

        // Parse the ratings ONCE and reuse them for count, average and best.
        // Skip 2 = skip Title and Year; everything left is a rating,
        // however many there are.
        let ratings =
            elements
            |> Array.skip 2
            |> Array.map float          // [| 9.0; 8.0; 9.5; 7.5 |]

        // Build the record LAST, so it's the function's result.
        // Every field must be given a value, in any order.
        {
            Title = elements.[0]
            Year = int elements.[1]                     // string -> int
            RatingCount = ratings |> Array.length       // 4
            AverageRating = ratings |> Array.average    // 8.5
            BestRating = ratings |> Array.max           // 9.5
        }

    // 2. Movie -> sentence ------------------------------------------------
    let describe (movie : Movie) : string =
        // The %-slots are filled IN ORDER by the values after the string:
        //   %s Title, %i Year, %0.1f Average, %i Count, %0.1f Best
        // %i is for ints, %0.1f for floats with 1 decimal place.
        sprintf "%s (%i): %0.1f average from %i ratings, best %0.1f"
            movie.Title movie.Year movie.AverageRating movie.RatingCount movie.BestRating

    // 3. Movie -> true/false ---------------------------------------------
    let isClassic (movie : Movie) : bool =
        // A comparison already IS a bool, so no if/else is needed.
        // `<` (not `<=`): "BEFORE 1980" means 1980 itself is not a classic.
        movie.Year < 1980

// 4. Sort, highest rating first ----------------------------------------
let bestFirst (movies : Movie[]) : Movie[] =
    movies
    |> Array.sortByDescending (fun movie -> movie.AverageRating)
    //   `fun movie -> movie.AverageRating` = "given a movie, give back its
    //   rating". sortByDescending sorts by those answers, biggest first.

// 5. The whole report: one pipeline reusing 1, 2 and 4 ----------------
let report (rows : string[]) : string[] =
    rows
    |> Array.skip 1                 // drop the header row
    |> Array.map Movie.fromString   // string[]  -> Movie[]
    |> bestFirst                    // Movie[]   -> Movie[] (sorted)
    |> Array.map Movie.describe     // Movie[]   -> string[]
    // Sort BEFORE describing: once they're strings, the ratings are just
    // text, and sorting text would put "9.0..." after "8.5..." by letters,
    // not by number. Records keep the real numbers until the last step.


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

let runTests () =
    let inception =
        { Title = "Inception"; Year = 2010; RatingCount = 4; AverageRating = 8.5; BestRating = 9.5 }
    let casablanca =
        { Title = "Casablanca"; Year = 1942; RatingCount = 3; AverageRating = 9.0; BestRating = 9.5 }
    let cats =
        { Title = "Cats"; Year = 2019; RatingCount = 2; AverageRating = 2.5; BestRating = 3.0 }

    printfn "--- Tests ---"
    check "fromString Inception" inception (Movie.fromString "Inception\t2010\t9.0\t8.0\t9.5\t7.5")
    check "fromString Casablanca" casablanca (Movie.fromString "Casablanca\t1942\t9.0\t9.5\t8.5")
    check "fromString Cats" cats (Movie.fromString "Cats\t2019\t2.0\t3.0")
    check "describe Inception"
        "Inception (2010): 8.5 average from 4 ratings, best 9.5"
        (Movie.describe inception)
    check "describe Cats"
        "Cats (2019): 2.5 average from 2 ratings, best 3.0"
        (Movie.describe cats)
    check "isClassic Casablanca" true (Movie.isClassic casablanca)
    check "isClassic Inception" false (Movie.isClassic inception)
    check "isClassic 1980 is not" false (Movie.isClassic { cats with Year = 1980 })
    check "bestFirst" [| casablanca; inception; cats |] (bestFirst [| cats; inception; casablanca |])
    check "report"
        [| "Casablanca (1942): 9.0 average from 3 ratings, best 9.5"
           "Inception (2010): 8.5 average from 4 ratings, best 9.5" |]
        (report [| "Title\tYear\tRatings..."
                   "Inception\t2010\t9.0\t8.0\t9.5\t7.5"
                   "Casablanca\t1942\t9.0\t9.5\t8.5" |])
    check "report only header" [||] (report [| "Title\tYear\tRatings..." |])

[<EntryPoint>]
let main argv =
    runTests ()

    if argv.Length = 1 then
        let filePath = argv.[0]
        if File.Exists filePath then
            printfn "--- Report: %s ---" filePath
            File.ReadAllLines filePath
            |> report
            |> Array.iter (fun line -> printfn "%s" line)
            0
        else
            printfn "File not found: %s" filePath
            2
    else
        printfn "(Pass a file to see the real report: dotnet run Samples/Movies.txt)"
        1
