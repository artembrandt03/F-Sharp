// VENDING MACHINE: an object-oriented exercise.
//
// A vending machine suits a class because it REMEMBERS things between actions:
// the money inserted, the stock left, and whether it's in service. The result of
// pressing a button depends on what happened before.

open System

// ---------------------------------------------------------------------------
// PLAIN DATA: objects don't replace records and unions.
// ---------------------------------------------------------------------------

// A product never changes and has no behaviour, so a RECORD is the right tool.
// decimal, because money must be exact.
type Product =
    {
        Code : string
        Name : string
        Price : decimal
    }

// Methods return these unions instead of printing or throwing.
// WHY: "sold out" and "not enough money" are normal events, not errors, and the
// caller decides what to show. It also makes the class easy to test.
type CoinResult =
    | CoinAccepted of balance : decimal
    | CoinRejected of reason : string

type PurchaseResult =
    | Dispensed of product : Product * change : decimal
    | UnknownCode of code : string
    | SoldOut of product : Product
    | NeedMoreMoney of product : Product * shortBy : decimal
    | OutOfService

// ---------------------------------------------------------------------------
// INTERFACE: what the machine needs from whoever records its sales.
// ---------------------------------------------------------------------------

// The machine only needs "something that can record a sale". It doesn't care
// whether that's memory, a file or a database. A test can pass in a fake.
type ISalesRecorder =
    abstract Record : Product -> unit

// `SalesLedger()` has an empty constructor: each ledger starts with no sales.
type SalesLedger() =
    // Private and mutable: only this class can change the sales history.
    // Outsiders can read summaries, but can't edit or delete a sale.
    let mutable sales : Product list = []

    // Read-only properties: there's a getter and no setter.
    member this.Count = sales.Length
    member this.Total = sales |> List.sumBy (fun product -> product.Price)

    // This is how a class fulfils an interface in F#.
    interface ISalesRecorder with
        member this.Record product = sales <- product :: sales

// ---------------------------------------------------------------------------
// THE MAIN CLASS
// ---------------------------------------------------------------------------

// The brackets are the PRIMARY CONSTRUCTOR. Its parameters are available
// throughout the class.
//
// `recorder` is passed in rather than created here. This is DEPENDENCY
// INJECTION: the machine depends on the interface, not on SalesLedger.
type VendingMachine(slots : (Product * int) list, recorder : ISalesRecorder) =

    // `do` runs once, when the object is created. Bad setup is a programming
    // mistake, so it throws. This guarantees that every VendingMachine that
    // exists is valid, and the methods don't need to re-check it.
    do
        let hasDuplicateCodes =
            slots
            |> List.countBy (fun (product, _) -> product.Code)
            |> List.exists (fun (_, count) -> count > 1)

        if slots.IsEmpty then
            raise (ArgumentException "A machine needs at least one product")

        if hasDuplicateCodes then
            raise (ArgumentException "Product codes must be unique")

        if slots |> List.exists (fun (product, _) -> product.Price <= 0m) then
            raise (ArgumentException "Prices must be positive")

        if slots |> List.exists (fun (_, quantity) -> quantity < 0) then
            raise (ArgumentException "Stock cannot be negative")

    // ENCAPSULATION: `let` fields are private. Nobody outside can set the
    // balance directly, so money only changes through the rules below.
    let acceptedCoins = set [ 0.25m; 0.50m; 1.00m; 2.00m ]

    // Never changes, so it's a plain `let`.
    let catalog =
        slots
        |> List.map (fun (product, _) -> product.Code, product)
        |> Map.ofList

    // These DO change, so they're `let mutable`. The stock Map itself stays
    // immutable: each sale replaces it with a new version.
    let mutable stock =
        slots
        |> List.map (fun (product, quantity) -> product.Code, quantity)
        |> Map.ofList

    let mutable balance = 0m

    // Read-only property: anyone can see the balance, nobody can assign it.
    member this.Balance = balance

    // An auto-property. Outsiders may freely switch this, and no validation is
    // needed, so `member val` saves writing a private field.
    member val InService = true with get, set

    // An unknown code simply has no stock.
    member this.StockOf(code : string) : int =
        stock |> Map.tryFind code |> Option.defaultValue 0

    member this.InsertCoin(amount : decimal) : CoinResult =
        if not this.InService then
            CoinRejected "machine is out of service"
        elif not (acceptedCoins.Contains amount) then
            CoinRejected "coin not accepted"
        else
            balance <- balance + amount
            CoinAccepted balance

    member this.Select(code : string) : PurchaseResult =
        if not this.InService then
            OutOfService
        else
            match Map.tryFind code catalog with
            | None -> UnknownCode code
            | Some product ->
                let remaining = this.StockOf code

                if remaining = 0 then
                    SoldOut product
                elif balance < product.Price then
                    NeedMoreMoney(product, product.Price - balance)
                else
                    // Every state change for a sale happens together, here,
                    // so outsiders can never see a half-finished sale.
                    let change = balance - product.Price
                    stock <- stock |> Map.add code (remaining - 1)
                    balance <- 0m
                    recorder.Record product
                    Dispensed(product, change)

    // Deliberately works when out of service: a broken machine must still be
    // able to give customers their money back.
    member this.Cancel() : decimal =
        let refund = balance
        balance <- 0m
        refund

// ---------------------------------------------------------------------------
// DEMO
// ---------------------------------------------------------------------------

let describeCoin (result : CoinResult) =
    match result with
    | CoinAccepted balance -> sprintf "Coin accepted, balance %.2f" balance
    | CoinRejected reason -> sprintf "Coin rejected: %s" reason

let describePurchase (result : PurchaseResult) =
    match result with
    | Dispensed(product, change) -> sprintf "Dispensed %s, change %.2f" product.Name change
    | UnknownCode code -> sprintf "Unknown code %s" code
    | SoldOut product -> sprintf "%s is sold out" product.Name
    | NeedMoreMoney(product, shortBy) -> sprintf "%s needs %.2f more" product.Name shortBy
    | OutOfService -> "Machine is out of service"

[<EntryPoint>]
let main _ =
    let water = { Code = "A1"; Name = "Water"; Price = 1.25m }
    let chips = { Code = "B1"; Name = "Chips"; Price = 1.75m }
    let gum = { Code = "C1"; Name = "Gum"; Price = 0.75m }

    let ledger = SalesLedger()
    let machine = VendingMachine([ water, 2; chips, 1; gum, 0 ], ledger)

    let insert amount = machine.InsertCoin amount |> describeCoin |> printfn "%s"
    let select code = machine.Select code |> describePurchase |> printfn "%s"

    insert 1.00m
    insert 0.10m
    select "A1"
    insert 0.50m
    select "A1"

    insert 2.00m
    select "C1"
    select "Z9"
    select "B1"

    insert 1.00m
    printfn "Refund %.2f" (machine.Cancel())

    machine.InService <- false
    insert 1.00m
    select "A1"

    printfn "Sales: %i, total %.2f" ledger.Count ledger.Total
    printfn "Stock: A1=%i, B1=%i" (machine.StockOf "A1") (machine.StockOf "B1")

    try
        VendingMachine([ water, 1; water, 1 ], ledger) |> ignore
    with :? ArgumentException as error ->
        printfn "Invalid machine: %s" error.Message

    0
