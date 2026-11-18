namespace StudentScores
 
module SchoolCodes =
 
    open System.IO
    open System.Collections.Generic

    let load (filePath : string) =
        // let pairs = 
        //     File.ReadAllLines filePath
        //     |> Seq.skip 1
        //     |> Seq.map (fun row -> 
        //             let elements = row.Split('\t')
        //             let id = elements.[0] |> int 
        //             let name = elements.[1]   
        //             KeyValuePair.Create(id, name))
        // new Dictionary<int, string>(pairs)
        File.ReadAllLines filePath
        |> Seq.skip 1
        |> Seq.map (fun row -> 
                let elements = row.Split('\t')
                // [Claude Code explaining] FIX: this line used to be
                //     let id = elements.[0] |> int
                //   That made the keys ints, so the whole thing was a
                //   Map<int, string>. But Student.fromString looks the school up
                //   with `elements.[2]`, which is a STRING ("1", "2"...). A Map is
                //   strict about its key type: you can't look up a string key in
                //   a map of int keys. Hence the error in Summary.fs line 25:
                //     "Expecting a Map<string,string> but given a Map<int,string>"
                //   It's reported in Summary.fs because that's where the two meet
                //   (`Student.fromString schoolCodes`), but the cause is here.
                //   Dropping `|> int` keeps the keys as strings, like the lecturer's
                //   After version. (The other way, converting the code to int in
                //   Student.fs, would also work, but strings are simpler: the
                //   codes come out of the files as text on both sides.)
                let id = elements.[0]
                let name = elements.[1]
                id, name)
        |> Map.ofSeq
        // [Claude Code explaining] The key here had to change too. It was `0`
        //   (an int), and every key in one map must have the same type. "*" is
        //   the code the lecturer uses for external students.
        |> Map.add "*" "(External)"
        // Or better still:
        //|> readOnlyDict
