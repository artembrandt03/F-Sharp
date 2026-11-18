# TinyBank: how I'd design it

This is the thinking behind `Solution/TinyBank`. In an interview, this reasoning counts as much as the code, so practise saying it out loud.

Read the code in this order, because each file only depends on the ones before it:

`Domain` → `Parse` → `AccountDirectory` → `Ledger` → `Forecast` → `Bank` → `Reporting` → `Program`

## Step 1: Clarify before coding (about 5 minutes)

Ask, or state an assumption if the interviewer wants you to decide:

| Question | Assumption here |
|---|---|
| Can an amount be zero or negative? | No, that's an invalid row |
| Is a failed transaction fatal? | No, record it and continue |
| Is a bad accounts file fatal? | Yes, everything depends on it |
| Do transactions run in file order? | Yes, order changes the results |
| Can an overdraft apply to savings? | No |
| Are transaction ids checked for duplicates? | Not required, so I'd mention it as a follow-up |
| Is everything one currency? | Yes |

## Step 2: Find the things that matter

Go through the brief and sort what you find:

- **"Has all of these" → record:** Account, Transaction.
- **"Is exactly one of these" → discriminated union:** account kind, transaction action, row parsed or invalid, applied or rejected, rejection reason.
- **"May not exist" → option:** an account lookup, a parsed number, a forecast result.
- **"Changes over time" → keep it small and in one place:** balances and history.
- **"Could be swapped" → interface:** the report destination.

The most important modelling insight is that the brief has **three levels of failure**, and each gets its own mechanism:

| Failure | Example | Mechanism | Why |
|---|---|---|---|
| Malformed row | `abc` as an amount | `Invalid` value | Expected, and we must report the line and reason |
| Refused by the rules | Insufficient funds | `Rejected` value | A normal business result |
| Can't continue | Missing file, duplicate account | Exception | Nothing useful can happen, so stop at the top |

## Step 3: Draw the flow

```text
Accounts.txt ──> Parse.account ──> read-only directory ──┐
                                                         v
Transactions.txt ──> lazy lines ──> Parse.transaction ──> Bank.Process ──> Ledger.apply
                                                             │              (pure rules)
                                                             v
                                                       BankSnapshot
                                                             │
                                                             v
                                         Report.print ──> IReportWriter
                                                           ├─ console
                                                           └─ file
```

The shape is **functional core, imperative shell**:

- **Core (pure):** `Parse.transaction`, `Ledger`, `Forecast`.
- **Shell (side effects):** file reading, `Bank`'s mutable fields, writers, `Program`.

The rules are the part most worth testing, and they need no files or console.

## Step 4: Build in this order

| Time | Do this | Why now |
|---|---|---|
| 10 min | `Domain.fs` | Agree on the model before writing logic |
| 15 min | `Ledger.apply` | The core rules are the highest value |
| 10 min | `Parse.transaction` | Connects real input to the model |
| 10 min | Accounts loading and `Bank` | Now it can run end to end |
| 15 min | The report | Visible results |
| 5 min | Exit codes and exception handling | Finish the outer edge |
| 5 to 15 min | Forecast, file output, cleanup | Valuable, but cut these first if you're short of time |

If you run out of time, a smaller program that works and has clean rules beats a half-finished complete one. Say what you'd do next.

## Key decisions

| Decision | Chosen | Alternative | Reasoning |
|---|---|---|---|
| Money type | `decimal` | `float` | Exact base-10 values, no binary rounding errors |
| Account kind | Union | Record with optional fields | Invalid combinations can't be represented |
| Row result | `Parsed<'T>` | `option` | We need the line number and reason. In production I'd consider `Result` |
| Account lookup | `readOnlyDict` | `Map` | Never changes and is read constantly |
| Balances | `Map` | Mutable `Dictionary` | Each change gives a new version, and a refused transfer has nothing to undo |
| Accounts file | `ReadAllLines` (array) | `ReadLines` | Small, and we want to validate all of it first |
| Transactions file | `ReadLines` (lazy seq) | `ReadAllLines` | May be huge and needs only one pass |
| History | `list` with `::` | Array | Fast at the front, and newest-first is what we need |
| Forecast | Infinite `Seq.unfold` | `while` loop | Each value depends on the last, and laziness stops at the answer |
| Stateful part | One class, `Bank` | Mutable values everywhere | State is hidden and has one way in |
| Output | `IReportWriter` | `printfn` in the report | Destinations can be swapped or faked in tests |
| Catching exceptions | Only in `Program` | `try` around lower-level calls | One clear place for messages and exit codes |

There's deliberately **no catch-all** handler. An unexpected exception means a bug, so it should fail loudly rather than be reported as a data problem.

## Where each course topic appears

| Lesson | In TinyBank |
|---|---|
| 02 Entry point, `argv` | `Program.main` |
| 03 `if` as an expression | `Ledger`, `Parse.account` |
| 05 Pipes | Everywhere, for example the `Program.run` pipeline |
| 06, 15 Collection functions | `map`, `filter`, `choose`, `groupBy`, `countBy`, `sortBy`, `iter` |
| 07 Records | `Account`, `Transaction`, `BankSnapshot` |
| 08 Option | `tryDecimal`, `tryFind`, `monthsToReach` |
| 09 Partial application | `monthsToTarget` in `Reporting` |
| 10 Namespaces, modules, file order | The whole project and `.fsproj` |
| 11 Discriminated unions | `AccountKind`, `TransactionAction`, `Outcome` and more |
| 12 Pattern matching | Array, tuple, union, option and list patterns |
| 13 Exceptions | `raise` in `Parse.account`, `try...with` in `Program` |
| 14 Generics | `Parsed<'T>` |
| 16 Arrays | Accounts loading, `AccountBalances` |
| 17 Sequences and laziness | `File.ReadLines` pipeline |
| 18 `Seq.unfold`, infinite sequences | `Forecast.monthlyBalances` |
| 19 Dictionaries | `readOnlyDict`, `IReadOnlyDictionary` |
| 20 Maps | `Balances` |
| Interfaces | `IReportWriter`, `IReadOnlyDictionary` |
| 21 Classes | `Bank`, the writers |
| 22 Lists | `::`, `List.truncate`, `List.rev`, `[]` patterns |

**Deliberately not used:**

- **`inline`:** everything is `decimal`, so there's no generic math to support.
- **`Seq.cache`:** the transactions are read in a single pass.
- **Mutable `Dictionary` and `for` loops:** a `Map` and pipelines are clearer here.

Saying why you *didn't* use a feature shows better judgment than forcing every feature in.

## Follow-up questions

**Why those types?**
Records hold data that's always present together. Unions model "exactly one of these", so a deposit can't have a target and a savings account can't have an overdraft. The compiler then checks that every `match` covers every case.

**What is pure, and what has side effects?**
`Ledger`, `Forecast` and `Parse.transaction` are pure. Reading files, printing, saving and `Bank`'s mutable fields are side effects, and they're kept at the edges.

**How would you test the withdrawal rule?**
Call it directly with plain values. No files or mocks are needed:

```fsharp
let account = { Id = "A"; Owner = "Test"; Kind = Savings 0m; OpeningBalance = 10m }
let directory = AccountDirectory.ofAccounts [| account |]
let balances = Ledger.opening [ account ]
let withdrawal = { TxId = "T1"; AccountId = "A"; Action = Withdrawal 20m }

let after, outcome = Ledger.apply directory withdrawal balances
// after = balances
// outcome = Rejected(withdrawal, InsufficientFunds(10m, 20m))
```

The report can be tested with a fake writer:

```fsharp
let lines = ResizeArray<string>()
let fakeWriter = { new IReportWriter with member _.WriteLine line = lines.Add line }
```

**What happens with a billion rows?**
Reading is streamed, and `recentActivity` is capped, so those stay small. But the rejected and invalid lists grow with bad data. I'd write them to a file as they occur, or keep counts plus a capped sample. Balances grow with the number of accounts, not transactions.

**What changes for a new account kind?**
Add a union case. The compiler then warns at each `match` that needs a decision, such as `availableFunds`, the report and the parser's string mapping (that last one needs adding by hand, because it matches on text).

**What changes for a new report destination?**
Write one new class that implements `IReportWriter`, and add a way to choose it in `Program`. `Report.print` doesn't change.

**What if a transaction appears twice?**
Right now it would be applied twice. I'd keep a `Set` of seen ids and add a `DuplicateTransaction` rejection reason. For a very large file, that set needs thought, perhaps using a database constraint.

**Is it thread-safe?**
`Ledger` is, because it's pure. `Bank` isn't: two threads could read the same balances and one update would be lost. Order also matters here, so the simplest safe design is one writer. If concurrency were required, I'd put a lock or an agent (`MailboxProcessor`) around `Bank`, or process each account separately only if cross-account transfers were handled carefully.

**What would you add for production?**
Automated tests, the `Result` type for parsing, duplicate detection, a real database transaction around each day's run, and structured logging.
