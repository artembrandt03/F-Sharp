# MiniCafe: how I'd approach it in 30 minutes

The solution is one commented file: `Solution/MiniCafe/Program.fs`.

## The plan

| Minutes | Do this | Say something like |
|---|---|---|
| 0 to 3 | Ask questions, sketch the flow | "Bad rows are expected, so they're data. A missing file stops the run." |
| 3 to 7 | Write the types | "An order has all these fields, so it's a record. A discount is one of three things, so it's a union." |
| 7 to 11 | Write `Pricing.total` | "This is pure, so it's easy to test." |
| 11 to 18 | Parse one row | "Each field gives an option, and I match all three together." |
| 18 to 25 | Report | "`groupBy` and `sumBy`, with a shared helper for items and customers." |
| 25 to 28 | `main` and exit codes | "Exceptions are only caught here, at the edge." |
| 28 to 30 | Run it, mention improvements | |

**The flow:**

```text
file lines -> numbered -> drop header and blanks -> ParsedRow[] -> report
```

**If you're running out of time,** cut in this order: the top customer, exit code details, tie-breaking. Keep the types, pricing and invalid-row handling, because they show the most. Tell the interviewer what you're leaving out.

## Decisions to be ready to explain

| Decision | Why |
|---|---|
| `Discount` is a union | Exactly one kind applies, and only `Coupon` has an amount |
| `Order` is a record | Plain immutable data with named fields |
| `decimal` for money | `float` can't store values like 0.1 exactly |
| `ParsedRow` instead of `option` | We need the line number and reason |
| No exception for a bad row | Bad rows are expected, and the run must continue |
| `Map` for the menu | Simple lookup, and `tryFind` returns an option |
| Array, not `seq` | The file is small and the report passes over it several times |
| `totalsBy` takes a function | Item and customer totals differ only in their key |
| One file | Modules give enough structure at this size |
| No class or interface | There's no changing state and only one output, so they'd add nothing |

## Likely follow-ups

**How would you test it?**
Call `Pricing.total` and `Parse.row` directly with plain values. Neither needs a file or the console.

**How would you add a "Loyalty" discount?**
Add a union case. The compiler then warns in `Pricing.total` until it's handled. Add its text to `tryDiscount`, which the compiler can't check because it matches on strings.

**What if the file had a billion rows?**
Switch to `File.ReadLines` and keep running totals in a `Map` with `Seq.fold`, so the rows themselves aren't all stored. Invalid rows would need a cap, or to be written out as they're found.

**Why not read the menu from a file?**
The brief fixed it, so that would be extra scope. Because `Parse.row` already takes the menu as a parameter, changing this later is easy.
