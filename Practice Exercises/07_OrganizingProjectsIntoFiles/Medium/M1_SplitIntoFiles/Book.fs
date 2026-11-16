namespace SplitIntoFilesExercise

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