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

module Movie =

    let fromString (row : string) : Movie =
        // TODO 1
        { Title = ""; Year = 0; RatingCount = 0; AverageRating = 0.0; BestRating = 0.0 }

    let describe (movie : Movie) : string =
        // TODO 2
        ""

    let isClassic (movie : Movie) : bool =
        // TODO 3
        false

let bestFirst (movies : Movie[]) : Movie[] =
    // TODO 4
    movies

let report (rows : string[]) : string[] =
    // TODO 5
    [||]


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
