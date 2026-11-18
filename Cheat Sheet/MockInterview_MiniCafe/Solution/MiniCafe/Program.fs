// MINICAFE: one file on purpose. In a 30-minute interview, splitting this into
// several files costs time and adds nothing. Modules still keep it organised,
// and top-to-bottom order means each part only uses what's above it.

open System
open System.IO
open System.Globalization

// ---------------------------------------------------------------------------
// TYPES: write these first, and talk through them.
// ---------------------------------------------------------------------------

// A UNION, because an order has exactly ONE kind of discount, and only a coupon
// carries an amount. NoDiscount is clearer here than an option, because "no
// discount" is a real, named business case.
type Discount =
    | NoDiscount
    | Student
    | Coupon of amountOff : decimal

// A RECORD, because an order has ALL of these at once.
// decimal, not float, because money must be exact.
type Order =
    {
        OrderId : string
        Customer : string
        Item : string
        Quantity : int
        UnitPrice : decimal
        Discount : Discount
    }

// Why not option? None couldn't say which line failed or why.
// Why not an exception? A bad row is expected, and the run must continue.
type ParsedRow =
    | Valid of Order
    | Invalid of lineNumber : int * reason : string

// ---------------------------------------------------------------------------
// PRICING: a pure function, so it's trivial to test.
// ---------------------------------------------------------------------------

module Pricing =

    let total (order : Order) : decimal =
        let subtotal = order.UnitPrice * decimal order.Quantity

        // Pattern matching puts every discount rule in one place. If a new
        // discount is added, the compiler warns here until it's handled.
        let discounted =
            match order.Discount with
            | NoDiscount -> subtotal
            | Student -> subtotal * 0.90m
            | Coupon amountOff -> subtotal - amountOff

        // max stops a large coupon producing a negative total.
        // AwayFromZero is normal "round half up"; .NET's default would turn
        // 2.925 into 2.92.
        Math.Round(max 0m discounted, 2, MidpointRounding.AwayFromZero)

// ---------------------------------------------------------------------------
// PARSING: text in, domain values out. Each helper returns an option because
// bad text is a normal case. TryParse doesn't throw, unlike `int` or `decimal`.
// InvariantCulture means "1.50" reads the same on every machine.
// ---------------------------------------------------------------------------

module Parse =

    let private tryAmount (text : string) : decimal option =
        match Decimal.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture) with
        | true, value when value > 0m -> Some value
        | _ -> None

    let private tryQuantity (text : string) : int option =
        match Int32.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture) with
        | true, value when value > 0 -> Some value
        | _ -> None

    // The array pattern splits "Coupon:1.50" and checks its shape in one step.
    let private tryDiscount (text : string) : Discount option =
        match text.Split(':') with
        | [| "None" |] -> Some NoDiscount
        | [| "Student" |] -> Some Student
        // Option.map wraps the amount in Coupon only if it parsed.
        | [| "Coupon"; amount |] -> tryAmount amount |> Option.map Coupon
        | _ -> None

    // Menu first, data last, so `Parse.row menu` can be partially applied.
    let row (menu : Map<string, decimal>) (lineNumber : int) (text : string) : ParsedRow =
        match text.Split('\t') |> Array.map (fun column -> column.Trim()) with
        | [| orderId; customer; item; quantity; discount |] ->
            // Matching on a tuple validates all three fields in one readable
            // table. The last line can only run when everything succeeded.
            match Map.tryFind item menu, tryQuantity quantity, tryDiscount discount with
            | None, _, _ -> Invalid(lineNumber, sprintf "unknown item '%s'" item)
            | _, None, _ -> Invalid(lineNumber, sprintf "invalid quantity '%s'" quantity)
            | _, _, None -> Invalid(lineNumber, sprintf "invalid discount '%s'" discount)
            | Some price, Some count, Some parsedDiscount ->
                Valid
                    {
                        OrderId = orderId
                        Customer = customer
                        Item = item
                        Quantity = count
                        UnitPrice = price
                        Discount = parsedDiscount
                    }
        | columns ->
            Invalid(lineNumber, sprintf "expected 5 columns but found %i" columns.Length)

// ---------------------------------------------------------------------------
// REPORT
// ---------------------------------------------------------------------------

module Report =

    // Both "by item" and "by customer" need the same steps, so the only thing
    // that differs, the grouping key, is passed in as a function.
    let private totalsBy (keyOf : Order -> string) (orders : Order[]) =
        orders
        |> Array.groupBy keyOf
        |> Array.map (fun (key, group) -> key, group |> Array.sumBy Pricing.total)
        // Sorting by a tuple: highest total first, then name to break ties.
        |> Array.sortBy (fun (key, total) -> -total, key)

    let print (rows : ParsedRow[]) =
        // choose = filter + map: keep one case and unwrap it.
        let orders =
            rows
            |> Array.choose (fun row ->
                match row with
                | Valid order -> Some order
                | Invalid _ -> None)

        let invalidRows =
            rows
            |> Array.choose (fun row ->
                match row with
                | Valid _ -> None
                | Invalid(lineNumber, reason) -> Some(lineNumber, reason))

        printfn "MINICAFE REPORT"
        printfn "Valid orders: %i" orders.Length
        printfn "Invalid rows: %i" invalidRows.Length
        printfn "Total revenue: %.2f" (orders |> Array.sumBy Pricing.total)

        printfn ""
        printfn "Revenue by item:"

        orders
        |> totalsBy (fun order -> order.Item)
        |> Array.iter (fun (item, total) -> printfn "  %-10s %8.2f" item total)

        printfn ""

        // tryHead gives an option, so a file with no valid orders can't crash.
        match orders |> totalsBy (fun order -> order.Customer) |> Array.tryHead with
        | Some(customer, total) -> printfn "Top customer: %s (%.2f)" customer total
        | None -> printfn "Top customer: (none)"

        printfn ""
        printfn "Invalid rows:"

        invalidRows
        |> Array.iter (fun (lineNumber, reason) -> printfn "  Line %i: %s" lineNumber reason)

// ---------------------------------------------------------------------------
// ENTRY POINT: the only part that touches files and exit codes.
// ---------------------------------------------------------------------------

// A Map suits a small, fixed lookup table, and Map.tryFind returns an option
// instead of throwing on an unknown item.
let private menu =
    Map.ofList [ "Latte", 4.50m; "Espresso", 3.00m; "Muffin", 3.25m ]

let private run (path : string) =
    // ReadAllLines: the file is small, and we need several passes over the
    // results for the report, so an array fits better than a lazy sequence.
    File.ReadAllLines path
    // Number lines BEFORE skipping, so they match the file in an editor.
    |> Array.mapi (fun index text -> index + 1, text)
    // Line 1 is the header. A filter is safer than Array.skip 1, which throws
    // if the file is completely empty.
    |> Array.filter (fun (lineNumber, text) ->
        lineNumber > 1 && not (String.IsNullOrWhiteSpace text))
    |> Array.map (fun (lineNumber, text) -> Parse.row menu lineNumber text)
    |> Report.print

[<EntryPoint>]
let main argv =
    match argv with
    | [| path |] ->
        // Exceptions are for what we can't continue from, and they're handled
        // only here. try...with is an expression, so it returns the exit code.
        try
            run path
            0
        with
        | :? IOException as error ->
            printfn "File problem: %s" error.Message
            2
        | :? UnauthorizedAccessException as error ->
            printfn "Access denied: %s" error.Message
            2
    | _ ->
        printfn "Usage: MiniCafe <orders file>"
        1
