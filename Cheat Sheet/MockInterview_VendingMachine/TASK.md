# Mock Interview: Vending Machine

**Time:** 30 minutes
**Focus:** classes, encapsulation, properties and interfaces
**Language:** F# console application (.NET 9), one file is fine

> Don't open `DESIGN.md` or `Solution/` until you've tried it.

## The brief

> Model a vending machine. Customers insert coins, choose a product, and receive the product and their change. The machine tracks its own money and stock, and reports each sale to a separate sales recorder.

Create your project in this folder:

```powershell
dotnet new console -lang F# -n VendingMachine -f net9.0 -o Attempt\VendingMachine
```

```powershell
cd Attempt\VendingMachine
```

```powershell
dotnet run
```

## What to build

### `Product`

Has a `Code`, a `Name` and a `Price`. Money must be exact.

### `CoinResult`

- `CoinAccepted` with the new balance
- `CoinRejected` with a reason

### `PurchaseResult`

- `Dispensed` with the product and the change
- `UnknownCode` with the code
- `SoldOut` with the product
- `NeedMoreMoney` with the product and how much is missing
- `OutOfService`

### `ISalesRecorder`

An interface with one member, `Record`, which takes a `Product` and returns nothing.

### `SalesLedger`

A class with a no-argument constructor that implements `ISalesRecorder`.

| Member | Behaviour |
|---|---|
| `Count` | Read-only: number of recorded sales |
| `Total` | Read-only: total value of recorded sales |

Code outside the class must not be able to change the recorded sales directly.

### `VendingMachine`

Its constructor takes:

1. A list of `(Product, quantity)` pairs
2. An `ISalesRecorder`

Creating a machine must throw `ArgumentException` if the list is empty, a product code is repeated, a price isn't positive, or a quantity is negative. Use the message `Product codes must be unique` for repeated codes.

| Member | Behaviour |
|---|---|
| `Balance` | Read-only: money currently inserted |
| `InService` | Read/write, starts as `true` |
| `StockOf(code)` | Quantity left, or 0 for an unknown code |
| `InsertCoin(amount)` | Returns a `CoinResult` |
| `Select(code)` | Returns a `PurchaseResult` |
| `Cancel()` | Returns the balance and resets it to 0 |

## Rules

**Inserting a coin**
- Out of service: reject with `machine is out of service`
- Only 0.25, 0.50, 1.00 and 2.00 are accepted. Otherwise reject with `coin not accepted`
- If accepted, add it to the balance

**Selecting a product.** Check in this order:
1. Out of service
2. Unknown code
3. Sold out
4. Not enough money
5. Otherwise, the sale succeeds: reduce stock by 1, record the sale, return the change, and reset the balance to 0

A failed selection changes nothing.

**Cancelling** works even when the machine is out of service, so customers can always get their money back.

## Demo code

Paste this at the bottom of `Program.fs`, below your types:

```fsharp
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
```

## Expected output

```text
Coin accepted, balance 1.00
Coin rejected: coin not accepted
Water needs 0.25 more
Coin accepted, balance 1.50
Dispensed Water, change 0.25
Coin accepted, balance 2.00
Gum is sold out
Unknown code Z9
Dispensed Chips, change 0.25
Coin accepted, balance 1.00
Refund 1.00
Coin rejected: machine is out of service
Machine is out of service
Sales: 2, total 3.00
Stock: A1=1, B1=0
Invalid machine: Product codes must be unique
```
