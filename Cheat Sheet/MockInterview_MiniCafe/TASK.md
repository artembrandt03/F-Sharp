# Mock Interview: MiniCafe

**Time:** 30 minutes
**Language:** F# console application (.NET 9), one file is fine

> Don't open `DESIGN.md` or `Solution/` until you've tried it.

## The brief

> Our café exports the day's orders to a text file. Build a command-line program that reads the file and prints a sales report. The file isn't always clean, and one bad row must not stop the run.

```
MiniCafe <orders file>
```

Create your project in this folder and run it against the sample:

```powershell
dotnet new console -lang F# -n MiniCafe -f net9.0 -o Attempt\MiniCafe
```

```powershell
cd Attempt\MiniCafe
```

```powershell
dotnet run -- ../../Samples/Orders.txt
```

## Menu

Put this directly in your code:

| Item | Price |
|---|---|
| Latte | 4.50 |
| Espresso | 3.00 |
| Muffin | 3.25 |

## Input

`Samples/Orders.txt` is tab-separated, with a header row. Blank lines are ignored.

| Column | Meaning |
|---|---|
| OrderId | Order id |
| Customer | Customer name |
| Item | A menu item |
| Quantity | A whole number above 0 |
| Discount | `None`, `Student`, or `Coupon:<amount>`, such as `Coupon:1.50` |

## Rules

Order total = price × quantity, then the discount:

- `None`: no change
- `Student`: 10% off
- `Coupon`: subtract the amount, which must be above 0

A total can't go below 0.00. Round it to 2 decimal places, with halves rounded up. Money must be exact.

A row is **invalid** if it has the wrong number of columns, an unknown item, a bad quantity or a bad discount. Skip it and record its line number in the file and the reason.

## Report

Print:

1. Valid order count, invalid row count and total revenue
2. Revenue by item, highest first, with ties sorted by name
3. The top customer by spending, with ties sorted by name, or `(none)` if there are no valid orders
4. Invalid rows, in file order

## Exit codes

| Situation | Code |
|---|---|
| Success, even with invalid rows | 0 |
| Wrong number of arguments (print usage) | 1 |
| File can't be read | 2 |

## Expected output

```text
MINICAFE REPORT
Valid orders: 6
Invalid rows: 4
Total revenue: 29.98

Revenue by item:
  Latte         13.05
  Muffin         9.43
  Espresso       7.50

Top customer: Maya (19.55)

Invalid rows:
  Line 6: unknown item 'Tea'
  Line 7: invalid quantity '0'
  Line 9: invalid discount 'Coupon:abc'
  Line 12: expected 5 columns but found 3
```
