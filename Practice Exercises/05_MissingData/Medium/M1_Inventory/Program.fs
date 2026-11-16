// =====================================================================
//  MEDIUM 1 - Inventory                         (lesson 21 + lesson 19)
//  Type: WRITE THE FUNCTIONS - 8 small functions, ~25 lines in total.
// =====================================================================
//
//  Samples/Inventory.txt lists products, TAB-separated, with a header.
//  Some data is MISSING:
//
//      Product   Price   Stock
//      Widget    4.50    10
//      Gizmo     N/A     3        <- price unknown ("N/A")
//      Doodad    12.00   ?        <- stock unknown ("?")
//      Thing             7        <- price unknown (blank "")
//
//  NEW IDEA: a record field can itself be an option. A missing price
//  isn't 0, it's UNKNOWN, so the record stores `Price : float option`:
//
//      { Name = "Widget"; Price = Some 4.5; Stock = 10 }
//      { Name = "Gizmo";  Price = None;     Stock = 3 }
//
//  Write these (each one has a TODO placeholder below):
//
//  module Parse
//    1. tryFloat s     -> None if s is "N/A" OR blank "", else Some (the number)
//                           tryFloat "4.50" = Some 4.5    tryFloat "" = None
//    2. tryInt s       -> None if s is "?", else Some (the number)
//    3. intOr d s      -> the int, or the default d if it's missing
//                           intOr 0 "?" = 0    intOr 0 "7" = 7
//
//  module Item
//    4. fromString row -> an Item. Missing stock counts as 0.
//    5. priceText item -> "$4.50"  or  "price unknown"
//
//  top level
//    6. knownPrices items   -> the prices we DO know: [| 4.5; 12.0 |]
//    7. countUnpriced items -> how many items have no price
//    8. report rows         -> skip the header, one line per item:
//                                "Widget: $4.50, stock 10"
//                                "Gizmo: price unknown, stock 3"
//
//  New here:
//    * `Option.map f` runs f on the value INSIDE a Some, and leaves None
//      alone:
//          Some 4.5 |> Option.map (fun p -> p * 2.0)  = Some 9.0
//          None     |> Option.map (fun p -> p * 2.0)  = None
//    * `Option.isNone x` is true when x is None. (`Option.isSome` is the
//      opposite.)
//    * `%0.2f` prints 2 decimal places: 4.5 -> "4.50"
//
//  STUCK? Each function has a hint at the VERY BOTTOM of the file:
//    first which functions to use, then step-by-step instructions.
//    Try on your own first!
//
//  Run:  dotnet run                           -> tests only
//        dotnet run Samples/Inventory.txt     -> tests + the real report
// =====================================================================

open System.IO

type Item =
    {
        Name : string
        Price : float option
        Stock : int
    }

module Parse =

    let tryFloat (s : string) : float option =
        // TODO 1
        None

    let tryInt (s : string) : int option =
        // TODO 2
        None

    let intOr (d : int) (s : string) : int =
        // TODO 3
        0

module Item =

    let fromString (row : string) : Item =
        // TODO 4
        { Name = ""; Price = None; Stock = 0 }

    let priceText (item : Item) : string =
        // TODO 5
        ""

let knownPrices (items : Item[]) : float[] =
    // TODO 6
    [||]

let countUnpriced (items : Item[]) : int =
    // TODO 7
    0

let report (rows : string[]) : string[] =
    // TODO 8
    [||]


// ----------------------- tests (don't edit) -----------------------
let check (testName: string) expected actual =
    if actual = expected then
        printfn "PASS  %s" testName
    else
        printfn "FAIL  %s\n        expected: %A\n        actual:   %A" testName expected actual

let runTests () =
    let widget = { Name = "Widget"; Price = Some 4.5; Stock = 10 }
    let gizmo = { Name = "Gizmo"; Price = None; Stock = 3 }
    let doodad = { Name = "Doodad"; Price = Some 12.0; Stock = 0 }
    let thing = { Name = "Thing"; Price = None; Stock = 7 }

    printfn "--- Tests ---"
    check "tryFloat number" (Some 4.5) (Parse.tryFloat "4.50")
    check "tryFloat N/A" None (Parse.tryFloat "N/A")
    check "tryFloat blank" None (Parse.tryFloat "")
    check "tryInt number" (Some 7) (Parse.tryInt "7")
    check "tryInt ?" None (Parse.tryInt "?")
    check "intOr number" 7 (Parse.intOr 0 "7")
    check "intOr ? uses default" 0 (Parse.intOr 0 "?")
    check "intOr other default" 99 (Parse.intOr 99 "?")
    check "fromString Widget" widget (Item.fromString "Widget\t4.50\t10")
    check "fromString Gizmo (N/A price)" gizmo (Item.fromString "Gizmo\tN/A\t3")
    check "fromString Doodad (? stock)" doodad (Item.fromString "Doodad\t12.00\t?")
    check "fromString Thing (blank price)" thing (Item.fromString "Thing\t\t7")
    check "priceText known" "$4.50" (Item.priceText widget)
    check "priceText unknown" "price unknown" (Item.priceText gizmo)
    check "knownPrices" [| 4.5; 12.0 |] (knownPrices [| widget; gizmo; doodad; thing |])
    check "knownPrices none" [||] (knownPrices [| gizmo |])
    check "countUnpriced" 2 (countUnpriced [| widget; gizmo; doodad; thing |])
    check "report"
        [| "Widget: $4.50, stock 10"
           "Gizmo: price unknown, stock 3" |]
        (report [| "Product\tPrice\tStock"; "Widget\t4.50\t10"; "Gizmo\tN/A\t3" |])

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
        printfn "(Pass a file to see the real report: dotnet run Samples/Inventory.txt)"
        1


// =====================================================================
// =====================================================================
//
//               HINTS - LAST RESORT. TRY IT YOURSELF FIRST!
//
//   Each function has two levels. Read only "Use:" first, and only read
//   "Steps:" if you're still stuck.
//
// =====================================================================
// =====================================================================
//
//  1. Parse.tryFloat
//     Use:   if / else,  ||,  Some,  None,  float
//     Steps: Exactly like Float.tryFromString in lesson 21, but the
//            condition checks TWO missing markers: s is "N/A" OR s is "".
//            Missing -> None. Otherwise -> Some (float s).
//
//  2. Parse.tryInt
//     Use:   if / else,  Some,  None,  int
//     Steps: Same shape as tryFloat. The missing marker is "?", and the
//            conversion is `int` instead of `float`.
//
//  3. Parse.intOr
//     Use:   |>,  tryInt,  Option.defaultValue
//     Steps: Exactly like Float.fromStringOr in lesson 21: start with s,
//            pipe it into tryInt (now you have an int option), then pipe
//            that into Option.defaultValue d.
//
//  4. Item.fromString
//     Use:   Split('\t'),  Parse.tryFloat,  Parse.intOr,  a record { ... }
//     Steps: Split the row. Then build the record LAST:
//              Name  = column 0, as is
//              Price = column 1 run through Parse.tryFloat
//                      (it's ALREADY an option, so that matches the field)
//              Stock = column 2 run through Parse.intOr with default 0
//
//  5. Item.priceText
//     Use:   item.Price,  |>,  Option.map,  sprintf "$%0.2f",
//            Option.defaultValue
//     Steps: Start with item.Price (a float option). Pipe it into
//            Option.map with a function that turns a price into text:
//            (fun price -> sprintf "$%0.2f" price). Now you have a
//            STRING option: Some "$4.50" or None. Pipe that into
//            Option.defaultValue "price unknown".
//
//  6. knownPrices
//     Use:   Array.choose,  (fun item -> item.Price)
//     Steps: Array.choose wants a function that returns an option, and
//            item.Price already IS one! So pipe items into Array.choose
//            with a function that just returns each item's Price. The
//            Nones are dropped and the Somes are unwrapped.
//
//  7. countUnpriced
//     Use:   Array.filter,  Option.isNone,  Array.length
//     Steps: Pipe items into Array.filter with
//            (fun item -> Option.isNone item.Price), then into Array.length.
//            (A second way: subtract the length of knownPrices from the
//            number of items.)
//
//  8. report
//     Use:   Array.skip,  Array.map,  Item.fromString,  Item.priceText,
//            sprintf "%s: %s, stock %i"
//     Steps: Pipe rows into Array.skip 1 (the header), then
//            Array.map Item.fromString (now you have Items). Then
//            Array.map with a function that takes ONE item and builds its
//            line with sprintf, using item.Name, (Item.priceText item),
//            and item.Stock. It can be a `fun` in place, or a small
//            `let line (item : Item) = ...` written above report.
// =====================================================================
