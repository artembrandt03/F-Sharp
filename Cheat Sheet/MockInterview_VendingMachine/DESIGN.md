# Vending Machine: how I'd approach it in 30 minutes

The solution is one commented file: `Solution/VendingMachine/Program.fs`.

## Choosing the right tool

Start by asking one question of each thing in the brief: **does it change over time, and does it have rules protecting that change?**

| Thing | Tool | Why |
|---|---|---|
| Product | Record | Plain data that never changes |
| Coin and purchase results | Unions | Each result is exactly one of several outcomes |
| Vending machine | Class | Hidden state changes over time, protected by rules |
| Sales ledger | Class | Keeps a private, growing history |
| Sales recorder | Interface | The machine needs the ability, not one specific class |

## The plan

| Minutes | Do this | Say something like |
|---|---|---|
| 0 to 3 | Clarify, list the state | "The machine must remember its balance, stock and service status." |
| 3 to 7 | Record and unions | "Sold out is a normal result, so I'll return it instead of throwing." |
| 7 to 11 | Interface and `SalesLedger` | "The machine shouldn't know how sales are stored." |
| 11 to 15 | Constructor and validation | "An invalid machine should never exist." |
| 15 to 18 | `InsertCoin`, `Cancel`, properties | "The balance is readable from outside, but only rules can change it." |
| 18 to 25 | `Select` | "I'll check everything first, then make all the changes together." |
| 25 to 30 | Run the demo, discuss improvements | |

If you're short of time, leave out some constructor checks, but say what's missing. Keep `Select` correct, because it's the heart of the exercise.

## OOP concepts, and where they appear

| Concept | Where | What to say |
|---|---|---|
| Class and object | `VendingMachine`, `machine` | The class is the blueprint, the object is one actual machine |
| Primary constructor | `VendingMachine(slots, recorder)` | Its parameters are available throughout the class |
| Constructor validation | `do` block | Objects can't start in a broken state |
| Encapsulation | `let mutable balance` | State is private, and changes only through methods |
| Read-only property | `Balance`, `Count`, `Total` | Callers can look but not change |
| Auto-property | `member val InService` | Simple read/write state with no rules |
| Method | `InsertCoin`, `Select`, `Cancel` | Behaviour that uses the object's state |
| Interface | `ISalesRecorder` | A contract, separate from any implementation |
| Polymorphism | Any recorder can be passed in | The same call can run different implementations |
| Dependency injection | `recorder` constructor parameter | The dependency is given to the object, so it's easy to replace |

There's no **inheritance**, on purpose. Nothing here is a specialised kind of something else. "Prefer composition over inheritance" applies: the machine *has* a recorder, and it doesn't need to *be* one.

## Decisions to be ready to explain

**Why return unions instead of throwing exceptions?**
A sold-out product is an expected event. The compiler also makes the caller handle every outcome.

**Why does the constructor throw, then?**
Invalid setup is a programmer or configuration mistake, and there's no sensible machine to create.

**Why doesn't the machine print anything?**
Then it would be tied to the console. Returning results keeps it reusable and easy to test.

**Why is `Balance` read-only but `InService` read/write?**
Changing money has rules, so it must go through methods. `InService` has none, and an operator can simply switch it.

**Why is the stock a `Map` inside a mutable field?**
Only one field changes, and each version of the `Map` is immutable. That's the same pattern as `ColorHistory` in lesson 22.

## Likely follow-ups

**How would you test it?**
Pass in a fake recorder, then call the methods and check the returned values:

```fsharp
let recorded = ResizeArray<Product>()
let fakeRecorder = { new ISalesRecorder with member _.Record product = recorded.Add product }

let water = { Code = "A1"; Name = "Water"; Price = 1.00m }
let machine = VendingMachine([ water, 1 ], fakeRecorder)

machine.InsertCoin 1.00m |> ignore
machine.Select "A1"       // Dispensed(water, 0.00m)
recorded.Count            // 1
machine.StockOf "A1"      // 0
```

**How would you save sales to a database?**
Write a new class that implements `ISalesRecorder`. `VendingMachine` doesn't change.

**What if recording the sale fails?**
Stock and balance have already been updated at that point, so this design would lose track of the sale. In a real system, I'd record it first or use a transaction, and decide carefully what the customer receives if it fails.

**Could two people use it at once?**
Not safely, because `Select` reads and then updates state. It's fine for one physical machine with one customer. A shared version would need a lock or an agent (`MailboxProcessor`).

**Could this be written without a class?**
Yes. A pure function could take the state and an action, and return the new state and result. That's easier to test, but the caller has to carry the state around. A class fits well when you want one object to own that state.
