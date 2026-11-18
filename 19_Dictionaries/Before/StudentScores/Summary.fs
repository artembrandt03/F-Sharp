namespace StudentScores 

module Summary =
 
    open System.IO

    let printGroupSummary (surname : string) (students : Student[]) =
        printfn "%s" (surname.ToUpperInvariant())
        students  
        |> Array.sortBy (fun student ->
            student.GivenName, student.Id)
        |> Array.iter (fun student ->
            printfn "\t%20s\t%s\t%0.1f\t%0.1f\t%0.1f"
                student.GivenName student.Id student.MeanScore student.MinScore student.MaxScore)
 
    // [Claude Code explaining] FIX: this line used to read
    //     let summarize schoolCodesFilePathfilePath =
    //   with NO SPACE between the two parameter names. F# saw ONE long
    //   parameter called `schoolCodesFilePathfilePath`, so summarize took
    //   1 argument instead of 2. That single typo caused ALL 3 build errors:
    //     * Summary.fs line 18: "'filePath' is not defined"
    //         (no parameter has that name, only the long glued one)
    //     * Summary.fs line 22: "'schoolCodesFilePath' is not defined"
    //         (same reason; the compiler even suggested the glued name)
    //     * Program.fs line 19: "This value is not a function and cannot be
    //       applied. It has type 'unit'"
    //         (Program passes 2 arguments. After the first one, summarize
    //          has already run and returned unit (nothing), so F# tried to
    //          give the second argument to unit, which isn't a function.)
    //   The fix is just the space. Parameters are separated by spaces,
    //   like `nameParts (s : string)` or `fromString schoolCodes s`.
    //   TIP: when several errors appear at once, look for the one root cause
    //   near the first error. Here, the "not defined" names both pointed
    //   back to this line.
    let summarize schoolCodesFilePath filePath =
        let rows = 
            File.ReadLines filePath
            |> Seq.cache
        let studentCount = (rows |> Seq.length) - 1
        printfn "Student count %i" studentCount
        let schoolCodes = SchoolCodes.load schoolCodesFilePath
        rows
        |> Seq.skip 1
        |> Seq.map (Student.fromString schoolCodes)
        |> Seq.sortByDescending (fun student -> student.MeanScore)
        |> Seq.iter Student.printSummary

    // let summarize filePath =
    //     let rows = File.ReadAllLines filePath
    //     let studentCount = (rows |> Array.length) - 1
    //     printfn "Student count %i" studentCount
    //     rows
    //     |> Array.skip 1
    //     |> Array.map Student.fromString
    //     |> Array.groupBy (fun s -> s.Surname)
    //     |> Array.sortBy fst
    //     |> Array.iter (fun (surname, students) -> 
    //         printGroupSummary surname students)    
     