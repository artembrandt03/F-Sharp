namespace SplitIntoFilesExercise

// ======================= PARSE  ->  move to Parse.fs ===================
module Parse =

    let intOr (d : int) (s : string) =
        if s = "?" then d else int s