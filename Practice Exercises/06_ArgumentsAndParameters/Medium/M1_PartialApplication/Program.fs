// =====================================================================
//  MEDIUM 1 - Partial Application                          (lesson 23)
//  Type: WRITE THE FUNCTIONS - short ones, ~12 lines in total.
// =====================================================================
//
//  Lesson 23's two big ideas:
//    * Give a function only SOME of its arguments, and you get back a
//      new function waiting for the rest:  singleQuote = quote '\''
//    * So put the "settings" parameters FIRST and the data LAST. Then
//      `data |> f setting` and `Array.map (f setting)` just work.
//
//  Write these. The ones marked (PARTIAL) must be ONE line that gives
//  another function some of its arguments: no new parameters, no `fun`.
//
//    1. between low high x   -> true if low <= x <= high
//    2. isTeen               -> (PARTIAL) between, for ages 13 to 19
//
//    3. namePart i s         -> like lesson 23: split "Eason, Kit" on ','
//                               and return part i, trimmed
//                                 namePart 0 "Eason, Kit" = "Eason"
//                                 namePart 1 "Eason, Kit" = "Kit"
//    4. surname              -> (PARTIAL) namePart for part 0
//    5. givenName            -> (PARTIAL) namePart for part 1
//
//    6. applyDiscount percent price  -> the price after the discount
//                                        applyDiscount 25.0 20.0 = 15.0
//    7. discountAll percent prices   -> every price discounted. Use
//                                       Array.map with applyDiscount
//                                       given ONLY its percent.
//
//  The placeholders return dummy values so it compiles. The (PARTIAL)
//  ones are written as `fun _ -> ...` placeholders: replace the whole
//  right-hand side.
//
//  Stuck? Hints at the VERY BOTTOM of the file ("Use:" first, then
//  "Steps:").
//
//  Run:  dotnet run
// =====================================================================

let between (low : int) (high : int) (x : int) : bool =
    // TODO 1
    false

let isTeen : int -> bool =
    // TODO 2 (PARTIAL)
    fun _ -> false

let namePart (i : int) (s : string) : string =
    // TODO 3
    ""

let surname : string -> string =
    // TODO 4 (PARTIAL)
    fun _ -> ""

let givenName : string -> string =
    // TODO 5 (PARTIAL)
    fun _ -> ""

let applyDiscount (percent : float) (price : float) : float =
    // TODO 6
    0.0

let discountAll (percent : float) (prices : float[]) : float[] =
    // TODO 7
    [||]


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

[<EntryPoint>]
let main argv =
    printfn "--- Tests ---"
    check "between inside" true (between 1 10 5)
    check "between edges" true (between 1 10 10)
    check "between outside" false (between 1 10 11)
    check "isTeen 15" true (isTeen 15)
    check "isTeen 12" false (isTeen 12)
    check "teens from a list" [| 13; 19 |] ([| 12; 13; 20; 19 |] |> Array.filter isTeen)
    check "namePart 0" "Eason" (namePart 0 "Eason, Kit")
    check "namePart 1" "Kit" (namePart 1 "Eason, Kit")
    check "surname" "Shelley" (surname "Shelley, Mary")
    check "givenName" "Mary" (givenName "Shelley, Mary")
    check "applyDiscount" 15.0 (applyDiscount 25.0 20.0)
    check "applyDiscount piped" 4.0 (8.0 |> applyDiscount 50.0)
    check "discountAll" [| 15.0; 6.0 |] (discountAll 25.0 [| 20.0; 8.0 |])
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
//  1. between
//     Use:   >=,  <=,  &&
//     Steps: Two comparisons joined with "and": x is at least low AND x
//            is at most high. A comparison already is a bool, so no if.
//
//  2. isTeen (PARTIAL)
//     Use:   between, with 2 of its 3 arguments
//     Steps: Replace `fun _ -> false` with `between` followed by the
//            lowest and highest teen age. Leave x off; isTeen IS the
//            function still waiting for x.
//
//  3. namePart
//     Use:   s.Split(','),  .[i],  .Trim()
//     Steps: Split s on the comma, take element i, and Trim() it (the
//            given name comes with a leading space: " Kit"). Same as
//            lesson 23.
//
//  4 + 5. surname / givenName (PARTIAL)
//     Use:   namePart, with 1 of its 2 arguments
//     Steps: `namePart 0` and `namePart 1`. That's the whole right-hand
//            side. This is why `i` is namePart's FIRST parameter.
//
//  6. applyDiscount
//     Use:   *,  -,  /
//     Steps: The price keeps (100 - percent) percent of itself:
//            price * (100.0 - percent) / 100.0
//            Note that percent comes FIRST: it's the "setting".
//
//  7. discountAll
//     Use:   |>,  Array.map,  (applyDiscount percent)
//     Steps: Pipe prices into Array.map, and give Array.map applyDiscount
//            with ONLY the percent filled in (in brackets). Array.map
//            supplies each price as the missing last argument.
// =====================================================================
