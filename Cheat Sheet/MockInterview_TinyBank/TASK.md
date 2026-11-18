# Mock Interview: TinyBank

**Time:** 60 to 90 minutes
**Language:** F# console application (.NET 9)
**Allowed:** the .NET docs and your compiler. Try it without this repo's solution.

> Don't open `DESIGN.md` or `Solution/` until you've made a real attempt.

## The brief

> We run a small bank. At the end of each day we receive a file of transactions. Build a command-line program that applies them to our accounts and prints an end-of-day report.
>
> The file comes from another system and isn't always clean. One bad row must not stop the whole run.
>
> We care more about clear modelling and sensible decisions than about clever code. Talk through your choices as you go.

## How it runs

```
TinyBank <accounts file> <transactions file> [output file]
```

With an output file, write the report there and print a short confirmation. Without one, print the report to the console.

Sample files are in `Samples/`. To run a solution from a project two folders below this one:

```powershell
dotnet run -- ../../Samples/Accounts.txt ../../Samples/Transactions.txt
```

## Input files

Both files are tab-separated and start with a header row. Blank lines are ignored.

### Accounts

| Column | Meaning |
|---|---|
| Id | Unique account id |
| Owner | Owner's name |
| Kind | `Checking` or `Savings` |
| Setting | Checking: overdraft limit. Savings: monthly interest rate, in percent |
| OpeningBalance | Balance at the start of the day |

### Transactions

| Column | Meaning |
|---|---|
| TxId | Transaction id |
| Type | `Deposit`, `Withdrawal` or `Transfer` |
| Account | The account it applies to (for a transfer, the source) |
| Amount | A positive amount |
| Target | Transfers only: the destination account |

## Business rules

1. Process transactions in file order. Each one sees the balances left by the ones before it.
2. A deposit adds to the account.
3. A withdrawal or transfer may spend up to the account's **available funds**:
   - Checking: balance + overdraft limit
   - Savings: balance only
4. A transfer moves money between two different existing accounts. It happens completely or not at all.
5. Amounts must be exact. Money must not pick up rounding errors.

## Two kinds of bad data

**Invalid row:** the row itself is malformed. Skip it, record its line number in the file and the reason, and continue. Examples: unknown type, wrong column count, an amount that isn't a positive number, a transfer with no target.

**Rejected transaction:** the row is well formed, but the bank refuses it. Leave balances unchanged, record the reason, and continue. Examples: unknown account, insufficient funds, a transfer to the same account.

## The report

Print these sections, in this order:

1. **Summary:** counts of applied, rejected and invalid.
2. **Balances:** grouped by account kind (kinds alphabetical). Within a kind, highest balance first, with ties broken by id.
3. **Rejected transactions:** in file order, with reasons.
4. **Invalid rows:** in file order, with line numbers and reasons.
5. **Recent activity:** the last 5 well-formed transactions, newest first, marked applied or rejected.
6. **Savings forecast:** for each savings account, sorted by id, the months until its closing balance reaches **10,000.00**. Each month, add interest rounded to 2 decimal places. Stop looking after 600 months. Show `already reached`, `N months`, or `not within 600 months`.

## Errors and exit codes

| Situation | Exit code |
|---|---|
| Success, even if some rows were invalid or rejected | 0 |
| Wrong number of arguments (print usage) | 1 |
| An input or output file can't be read or written | 2 |
| The accounts file is invalid, including duplicate ids | 3 |

A bad accounts file stops the run, because everything depends on it. Print a clear message, not a stack trace.

## Non-functional requirements

- The transactions file could be very large. Don't load it all into memory just to read it.
- The banking rules must be testable without files or a console.
- Adding a new report destination shouldn't require changing the report logic.

## Expected output for the sample files

```text
TINYBANK REPORT

== Summary ==
Applied:  5
Rejected: 4
Invalid:  5

== Balances ==
Checking
  A101   Noah Patel       425.50
  A100   Maya Chen       -150.00
Savings
  S201   Lena Ortiz      9500.00
  S200   Maya Chen       5400.00
  S202   Theo Novak       300.00

== Rejected transactions ==
T002 A101: Withdrawal 100.00 -> insufficient funds (available 80.00, requested 100.00)
T005 S200: Withdrawal 6000.00 -> insufficient funds (available 5400.00, requested 6000.00)
T006 X999: Deposit 50.00 -> unknown account X999
T011 S201: Transfer 100.00 to S201 -> cannot transfer to the same account

== Invalid rows ==
Line 8: unknown transaction type 'Bonus'
Line 9: 'abc' is not a valid amount
Line 10: Transfer needs a target account
Line 11: amount must be positive
Line 16: expected 4 or 5 columns but found 2

== Recent activity (newest first) ==
APPLIED  T013 S201: Transfer 300.00 to A101
APPLIED  T012 A101: Deposit 45.50
REJECTED T011 S201: Transfer 100.00 to S201
REJECTED T006 X999: Deposit 50.00
REJECTED T005 S200: Withdrawal 6000.00

== Savings forecast (target 10000.00) ==
S200   124 months
S201   21 months
S202   not within 600 months
```

## What the interviewer may ask afterwards

Think about these once you're done. Answers are in `DESIGN.md`.

1. Why did you choose those types for accounts and transactions?
2. Which parts are pure, and which have side effects?
3. How would you test the withdrawal rule?
4. What happens to memory with a billion rows?
5. What changes if we add a new account kind? A new report destination?
6. What if the same transaction appears twice?
7. Could two threads use your code at the same time?
