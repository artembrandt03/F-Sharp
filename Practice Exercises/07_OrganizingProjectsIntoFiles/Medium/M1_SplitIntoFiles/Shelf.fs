namespace SplitIntoFilesExercise

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