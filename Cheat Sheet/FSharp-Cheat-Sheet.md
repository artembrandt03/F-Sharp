# F# Cheat Sheet: the whole course, explained slowly

This covers every topic from **01 Introduction** to **22 F# Lists**, in course order. Section numbers match the folder numbers.

The early sections are quick reminders. From **06 Collection functions** on, every concept is explained slowly:
**What it is → Why we use it → Example → Good / bad use cases → Interview answer.**

The examples come from the course projects (StudentScores, Bricks, ConsolePrompt, ColorHistory), so you can open the real code next to this.

To practise putting it all together, try a mock interview: [MockInterview_MiniCafe/TASK.md](MockInterview_MiniCafe/TASK.md) (30 minutes) or [MockInterview_TinyBank/TASK.md](MockInterview_TinyBank/TASK.md) (60 to 90 minutes).

---

## Contents

- [Part 0: Ideas that run through everything](#part-0-ideas-that-run-through-everything)
- [02 Your first program](#02-your-first-program)
- [03 If / elif / else](#03-if--elif--else)
- [04 Loops and iteration](#04-loops-and-iteration)
- [05 Forward piping](#05-forward-piping-)
- [06 Collection functions, part 1](#06-collection-functions-part-1)
- [07 Record types](#07-record-types)
- [08 Missing data (Option)](#08-missing-data-option)
- [09 Arguments, parameters, currying](#09-arguments-parameters-currying)
- [10 Organizing projects into files](#10-organizing-projects-into-files)
- [11 Discriminated unions](#11-discriminated-unions)
- [12 Pattern matching](#12-pattern-matching)
- [13 Handling exceptions](#13-handling-exceptions)
- [14 Generics (and `inline`)](#14-generics-and-inline)
- [15 Collection functions, part 2 (groupBy, tuples)](#15-collection-functions-part-2)
- [16 Creating and populating arrays](#16-creating-and-populating-arrays)
- [17 Sequences (laziness, `yield`)](#17-sequences)
- [18 Creating sequences (init, infinite, unfold)](#18-creating-sequences)
- [19 Dictionaries](#19-dictionaries)
- [20 Maps](#20-maps)
- [Interfaces (and C#)](#interfaces-and-c)
- [21 Object orientation and classes](#21-object-orientation-and-classes)
- [22 F# lists](#22-f-lists)
- [The big comparison tables](#the-big-comparison-tables)
- [Interview questions with answers](#interview-questions-with-answers)
- [Glossary](#glossary)

---

## Part 0: Ideas that run through everything

These five ideas explain *why* F# code looks the way it does. Interviewers love them, because they show you understand the language rather than just its syntax.

### 1. Everything is an expression (it gives back a value)

In C#, many things are **statements**: they *do* something but don't *give back* anything (`if`, loops, blocks).
In F#, almost everything is an **expression**: it produces a value.

```fsharp
let label = if score > 50.0 then "pass" else "fail"     // the if GIVES BACK a string
```

So the last line of a function **is** its return value. There's no `return` keyword.

```fsharp
let square x =
    let result = x * x
    result                 // last line = what the function returns
```

When something has nothing useful to give back (like `printfn`), it returns **`unit`**, written `()`. It means "nothing, done". It's like `void` in C#, except it's a real value.

### 2. Immutability by default

`let x = 5` means x **is** 5 forever. You can't change it later.

```fsharp
let x = 5
x <- 6            // ERROR: x is not mutable
let mutable y = 5
y <- 6            // OK: you asked for mutability explicitly
```

**Why?** Values that can't change can't be changed *by surprise*, by another function or another thread.
Less can go wrong, so the code is easier to reason about.
Instead of changing data, F# code makes **new** data: a new record, a new list, a new map.

### 3. Functions are values

A function is a value like an int or a string. You can store it in a `let`, pass it to another function, or return it.
That's why `Array.map Student.fromString` works: you're handing the function itself to `Array.map`.

A function that takes or returns a function is called a **higher-order function**.
`Array.map`, `Array.filter` and `Seq.iter` are all higher-order functions.

### 4. Type inference: F# works out the types for you

```fsharp
let add a b = a + b       // F# figures out: int -> int -> int
```

You add type annotations (`(s : string)`) only when F# can't work a type out. The most common case is calling a .NET **method** on a parameter, like `s.Split(',')`: F# can't know what `s` is from a dot alone.
That's the "lookup on object of indeterminate type" error (FS0072).

### 5. Order matters: top to bottom, file by file

F# reads code **top to bottom**, and files in the order listed in the `.fsproj`. You can only use something that was defined **above** it.
This feels annoying at first, but it means there are no circular dependencies, and you always know where to look.

---

## 02 Your first program

```fsharp
[<EntryPoint>]                 // "start the program here"
let main argv =                // argv = the command-line arguments, a string[]
    printfn "Hello %s" argv.[0]
    0                          // exit code: 0 = success, anything else = error
```

- `dotnet run abc 123` makes `argv = [| "abc"; "123" |]`.
- `[<EntryPoint>]` must be on the **last function of the last file**.
- **printf formats:** `%s` string, `%i` int, `%f` float (6 decimals), `%0.1f` float with 1 decimal, `%b` bool, `%A` anything (pretty-printed), `%O` anything (via `.ToString()`).
- `printfn` adds a new line. `printf` doesn't. `sprintf` gives you the string instead of printing it.

## 03 If / elif / else

```fsharp
let describe n =
    if n < 0 then "negative"
    elif n = 0 then "zero"          // = is comparison in F# (not ==)
    else "positive"
```

- It's an **expression**, so both branches must return the **same type**.
- An `if` without `else` is only allowed when the branch returns `unit` (for example a `printfn`).
- Comparison: `=` equal, `<>` not equal, `&&` and, `||` or, `not` for negation.

## 04 Loops and iteration

```fsharp
for i in 1..10 do printfn "%i" i              // range, inclusive at both ends
for name in argv do printfn "%s" name         // loop over a collection
let mutable total = 0
for x in [|1; 2; 3|] do total <- total + x    // <- assigns to a mutable
```

- `1..10` includes 10. `0..2..10` counts in steps of 2.
- Loops return `unit`. They exist only for side effects.
- **In real F# you rarely write loops.** Collection functions (`Array.sum`, `Array.map`) do the same job in less code, and nothing needs to be mutable. Loops plus `mutable` are the C# way. Pipelines are the F# way.

## 05 Forward piping `|>`

```fsharp
x |> f          // is exactly the same as   f x
```

**What it is:** `|>` takes the value on its left and passes it as the **last** argument of the function on its right.

**Why we use it:** it lets you write steps **in the order they happen**, top to bottom, instead of inside out.

```fsharp
// Without pipes: read from the INSIDE out. Hard to follow.
Array.iter printfn (Array.filter (fun n -> n.Length > 3) names)

// With pipes: read top to bottom, like a recipe.
names
|> Array.filter (fun n -> n.Length > 3)
|> Array.iter (printfn "%s")
```

**Why the data goes last:** because `|>` fills the *last* argument, F# library functions always take the **data last** (`Array.map f data`, `Map.add key value map`). Design your own functions the same way.

**Lambda (anonymous function):** `fun x -> x * 2` is a function with no name, written on the spot.

`<|` is the backward pipe: `f <| x` = `f x`. You'll see it in `raise <| ArgumentException(...)`, where it just saves brackets.

---

## 06 Collection functions, part 1

### What they are

A **collection** is a bunch of values of the same type: an array `[| ... |]`, a list `[ ... ]` or a sequence `seq { ... }`.
**Collection functions** are ready-made tools that do a common job over a whole collection, so you don't have to write a loop.
They live in modules named after the collection: `Array.xxx`, `List.xxx`, `Seq.xxx`. The names are the same in all three.

### Why use them instead of loops

1. **Less code, fewer bugs.** No index variables, no off-by-one mistakes, no `mutable` total.
2. **The name says the intent.** `Array.filter` immediately tells the reader "we're keeping some items". A loop has to be read to the end to find out what it does.
3. **They chain.** Each function returns a new collection, so you can pipe it into the next one.
4. **They don't change the original.** `Array.map` makes a **new** array. The old one stays as it was.

### The core set, as pictures

Imagine a conveyor belt of items. Each function does one job to the belt.

| Function | Job, in plain words | Before → after |
|---|---|---|
| `Array.map f` | **Transform** every item with f | `[1;2;3]` → `[2;4;6]` (f = double) |
| `Array.filter f` | **Keep** only items where f says true | `[1;2;3;4]` → `[2;4]` (f = isEven) |
| `Array.iter f` | **Do** f to each item (printing). Returns unit | prints 1, 2, 3 |
| `Array.skip n` | Drop the first n items | `skip 1 [h;a;b]` → `[a;b]` (drop a header row!) |
| `Array.length` | How many items | `[a;b;c]` → `3` |
| `Array.sum` / `average` | Add up / average | `[1.0;2.0;3.0]` → `6.0` / `2.0` |
| `Array.min` / `max` | Smallest / largest | `[3;1;2]` → `1` / `3` |
| `Array.sortBy f` | Sort by whatever f picks out | sort students by `fun s -> s.MeanScore` |
| `Array.sortByDescending f` | Same, biggest first | top scorers first |

**`map` vs `iter`:** `map` makes a new collection (each item becomes something). `iter` just *does* something with each item and gives back nothing (unit).
If you want a result, use `map`. If you want to print, use `iter`.

### The StudentScores pipeline (lesson 6's main example), step by step

```fsharp
let summarize filePath =
    let rows = File.ReadAllLines filePath          // string[]: one string per line
    let studentCount = (rows |> Array.length) - 1  // -1 for the header line
    printfn "Student count %i" studentCount
    rows
    |> Array.skip 1                                // throw away the header
    |> Array.map Student.fromString                // string[] -> Student[]
    |> Array.sortByDescending (fun s -> s.MeanScore)
    |> Array.iter Student.printSummary             // print each one
```

Read it as: *take the rows → skip the header → turn each row into a student → sort best first → print each*.
**Trick for understanding any pipeline:** write the TYPE next to each line, as in the comments above. The types tell the story.

### String splitting

```fsharp
let elements = "Kit\t90\t85".Split('\t')   // [| "Kit"; "90"; "85" |]
elements.[0]                               // "Kit"   (index starts at 0)
elements.[1] |> float                      // 90.0    (convert a string to a number)
```

### Good / bad use

- ✅ Almost any "do something to every item" job.
- ❌ Calling `Array.min` / `average` on an **empty** array throws an exception. Check the length first, or filter carefully.
- ❌ A very long pipeline doing ten unrelated things. Split it into named functions.

**Interview answer:** *"F# favours collection functions like map, filter and fold over loops. They're declarative: you say **what** you want, not **how** to loop. They don't mutate the input, and they compose with the pipe operator."*

---

## 07 Record types

### What it is

A **record** is a small bundle of **named** values, like a row in a table or a form with labelled fields.

```fsharp
type Student =
    {
        Surname : string
        GivenName : string
        MeanScore : float
    }

let kit = { Surname = "Eason"; GivenName = "Kit"; MeanScore = 85.0 }
kit.Surname          // "Eason"
```

You don't name the type when creating one. F# works out it's a `Student` from the field names.

### Why we use it

- **Names instead of positions.** `kit.MeanScore` is much clearer than `item.[2]` or the second part of a tuple.
- **Immutable.** Once made, it never changes, which makes it safe to pass around.
- **Free equality.** Two records with the same field values **are equal** (`=` compares contents, not memory location). This is called **structural equality**. A C# class doesn't do this by default.
- **Free printing.** `printfn "%A" kit` prints every field.

### "Changing" a record: copy and update

You can't change a record, so you make a copy with some fields different:

```fsharp
let better = { kit with MeanScore = 95.0 }   // kit is unchanged; better is new
```

### Module with the same name as the type

The course puts the type and its functions side by side:

```fsharp
type Student = { ... }

module Student =
    let fromString (s : string) = ...      // build a Student from a line of text
    let printSummary (student : Student) = ...
```

Then you call `Student.fromString`. The type holds the **data**, and the module holds the **functions** that work on it. That's the F# style: data and behaviour are kept separate. (Classes, in section 21, glue them together.)

### Anonymous records `{| |}`

```fsharp
let name = {| Surname = "Eason"; GivenName = "Kit" |}   // no type declaration needed
name.Surname
```

Use one for a quick, local "this function returns two named things" (lesson 12's `nameParts`). Don't use one as a main type you pass around everywhere.

### Good / bad use

- ✅ Plain data: a student, a brick, a payment, a point, a config.
- ❌ Something with changing internal state and behaviour, like a prompt that counts tries. That's a class.
- ❌ A thing that can be one of several *different shapes*. That's a discriminated union.

**Interview answer:** *"A record is an immutable data type with named fields and structural equality. You update one with copy-and-update `{ r with X = ... }`. It's F#'s default way to model data."*

---

## 08 Missing data (Option)

### The problem

Sometimes there is **no value**: a test score of "N/A", a lookup that finds nothing, an empty list's first item.
In C#, people use `null`. The trouble is that **any** reference might be null, the compiler doesn't warn you, and you get a `NullReferenceException` at runtime.
Its inventor called null his "billion-dollar mistake".

### What Option is

`option` is a box that is **either** full or empty, and the type *says so*:

```fsharp
Some 42.0     // a full box with 42.0 inside      : float option
None          // an empty box                     : float option
```

The type `float option` is **different** from `float`. You **can't** accidentally use a `float option` as a number, because the compiler stops you.
You're forced to deal with the empty case. Forgetting is impossible, which is the whole point.

### Making one (lesson 8's `Float.tryFromString`)

```fsharp
let tryFromString s =
    if s = "N/A" then None
    else Some (float s)
```

Naming convention: a function that *might* not give a value starts with **`try`** and returns an option.

### Using one

| Tool | What it does | Example |
|---|---|---|
| `Option.defaultValue d` | Open the box. If it's empty, use d | `None \|> Option.defaultValue 0.0` → `0.0` |
| `Option.map f` | If full, transform what's inside. If empty, stay empty | `Some 2 \|> Option.map (fun x -> x * 10)` → `Some 20` |
| `Array.choose f` | f returns options. **Keep the Somes (unwrapped), drop the Nones** | see below |
| `match` | Handle both cases yourself | see section 12 |

```fsharp
[| "90"; "N/A"; "70" |]
|> Array.choose Float.tryFromString      // [| 90.0; 70.0 |]  (the N/A is gone)
```

**`choose` = `map` + `filter` in one step.** `map` would give `[| Some 90.0; None; Some 70.0 |]`. `choose` throws away the Nones and unwraps the rest.

### Passing a function with its first argument filled in

```fsharp
let fromStringOr d s = s |> tryFromString |> Option.defaultValue d
Array.map (Float.fromStringOr 50.0) data     // every "N/A" becomes 50.0
```

`Float.fromStringOr 50.0` is a new function still waiting for `s`. That's partial application (section 9).

### Good / bad use

- ✅ "This might legitimately not exist": a lookup, parsing, an optional field.
- ❌ Real **errors** (a corrupt file, the network is down). That's what exceptions are for (section 13).
- ❌ Wrapping everything in options "just in case". Then every caller has to unwrap for nothing.

**Interview answer:** *"Option makes the absence of a value explicit in the type: `Some x` or `None`. The compiler forces you to handle None, so whole classes of null-reference bugs disappear."*

---

## 09 Arguments, parameters, currying

### Parameters vs arguments

- **Parameter** = the name in the function definition: `let add a b`, where a and b are parameters.
- **Argument** = the actual value you pass in: `add 1 2`, where 1 and 2 are arguments.

F# separates arguments with **spaces**, not commas: `add 1 2`, not `add(1, 2)`.
(A missing space joins two names into one parameter. That was the 19_Dictionaries bug.)

### Currying: what it is

In F#, **every function really takes exactly one argument**. A "two-argument" function is a function that takes the first argument and **returns a new function** waiting for the second.

```fsharp
let add a b = a + b      // type:  int -> int -> int
                         // read:  give me an int, and I give you back (a function that wants an int and gives an int)
```

The arrows tell the story: each `->` is "give me this, and I give you back what's on the right".

### Partial application: why currying is useful

Because of currying, you can supply **only some** of the arguments and get a **specialised function** back:

```fsharp
let add a b = a + b
let addTen = add 10          // addTen is a function: int -> int
addTen 5                     // 15

let quote (mark : char) (s : string) = sprintf "%c%s%c" mark s mark
let singleQuote = quote '\''          // fill in the SETTING, wait for the DATA
let doubleQuote = quote '"'
singleQuote "hi"                      // 'hi'
```

### The design rule this gives you: settings first, data last

Put the argument that's **least likely to change** (a setting or config) **first**, and the **data** **last**. Then:

1. You can pre-fill the setting to make handy specialised functions, and
2. it works with `|>`, which fills in the last argument:

```fsharp
data |> Array.map (Float.fromStringOr 50.0)     // setting filled in, data piped in
names |> Array.map singleQuote
```

### Tupled arguments (the other style)

```fsharp
let addT (a, b) = a + b      // ONE argument, which is a tuple
addT (1, 2)                  // can't partially apply this
```

.NET methods (C# style) always take tupled arguments, like `s.Split(',')` and `Math.Max(1, 2)`. That's why those use brackets and commas.

**Interview answer:** *"Currying means a multi-argument function is a chain of one-argument functions. Partial application is supplying only the first few arguments to get a specialised function back. That's why F# APIs put the data last: it works with the pipe operator."*

---

## 10 Organizing projects into files

```fsharp
// Float.fs
namespace StudentScores        // a NAMESPACE groups related code under one name

module Float =                 // a MODULE holds functions (and values)
    let tryFromString s = ...
```

```xml
<!-- StudentScores.fsproj: ORDER MATTERS -->
<Compile Include="Float.fs" />        <!-- used by Student.fs, so it comes first -->
<Compile Include="Student.fs" />
<Compile Include="Summary.fs" />
<Compile Include="Program.fs" />      <!-- always last: it has [<EntryPoint>] -->
```

```fsharp
// Program.fs
open StudentScores             // "let me use the names inside without the prefix"
```

| Thing | What it's for | Analogy |
|---|---|---|
| **namespace** | A name for a group of related types and modules | the building |
| **module** | Holds functions and values (and can hold types) | a room in it |
| **open** | Bring names into scope, so you write `Float.x` instead of `StudentScores.Float.x` | walking into the building |

**The rule:** a file can only use files **above** it in the `.fsproj`.
**Why F# insists:** there are no circular dependencies (A needs B and B needs A), so the project always has a clear, one-way structure.
The common errors when it's wrong are "The namespace or module 'X' is not defined" and "'X' is not defined".

---

## 11 Discriminated unions

### What it is, like you're 5

A **discriminated union (DU)** is a type that says *"a value of this type is **exactly one** of these options"*.
Each option is called a **case**, and each case can carry its **own** kind of data, or none at all.

Think of a test result. It can be:
- **Absent** (the student didn't show up), **or**
- **Excused**, **or**
- **Voided** (cheating), **or**
- **Scored**, *with a number*.

```fsharp
type TestResult =
    | Absent                 // a case with no data
    | Excused
    | Voided
    | Scored of float        // a case that CARRIES a float
```

```fsharp
let r1 = Scored 87.5         // create a value by using the case name like a function
let r2 = Absent
```

It's "**this OR that OR that**". A record is "**this AND that AND that**" (all fields at once).

### Why we use it

- **It models reality exactly.** A score of "A" isn't a number. Without a DU, you'd fake it with `-1.0` or a string, and someone would eventually add `-1.0` into an average.
- **Illegal states can't exist.** A `Scored` *always* has a number, and an `Absent` *never* has one. You literally can't build a wrong combination.
- **The compiler checks every case is handled.** If you add `| Voided` later, every `match` that forgets it gets a warning (FS0025, "Incomplete pattern matches"). The compiler shows you every place that needs updating.

### Using one: `match`

```fsharp
let tryEffectiveScore (testResult : TestResult) =
    match testResult with
    | Absent -> Some 0.0                 // absent counts as zero
    | Excused
    | Voided -> None                     // two cases sharing one result ("stacked")
    | Scored score -> Some score         // NAME the data inside: score
```

The key syntax: for a case with data, you **name** the data in the pattern (`Scored score`). The name exists only in that branch.
Use `_` if you don't need the data: `| Scored _ -> "had a score"`.

### Building one from text (lesson 11's `fromString`)

```fsharp
let fromString s =
    if s = "A" then Absent
    elif s = "E" then Excused
    elif s = "V" then Voided
    else Scored (float s)
```

### `option` is just a DU

```fsharp
type Option<'T> =
    | Some of 'T
    | None
```

Everything from lesson 8 is a DU underneath. The course proves it in lesson 13/14 by building its own copy, `Optional<'T> = Something of 'T | Nothing`.

### Good / bad use

- ✅ A **fixed, known** set of possibilities: payment methods, order states (Pending / Shipped / Delivered), shapes, test results, commands.
- ❌ A set that **outsiders keep extending** (plugins). Every new case forces every `match` to be updated. An interface fits better there (see "Interfaces").
- ❌ Using one with a single case and no reason. A record is clearer.

**Interview answer:** *"A DU is a 'one of' type. Each case can carry different data. With pattern matching, the compiler checks you've handled every case. It's how F# makes illegal states unrepresentable."*

---

## 12 Pattern matching

(There's a longer walkthrough in `12_PatternMatching/PatternMatching-Explained.md`.)

### What it is

`match` compares a value against a list of **shapes** (patterns), **top to bottom**, and runs the **first one that fits**. While it checks, it can **pull pieces out** and give them names.

```fsharp
match value with
| pattern1 -> result1
| pattern2 -> result2
| _        -> fallback          // _ = "anything else"
```

### The kinds of pattern you've seen

| Pattern | Fits when | Example |
|---|---|---|
| A constant: `1`, `"A"` | the value equals it | `\| 1 -> "One green bottle"` |
| A name: `x` | always, and captures the value | `\| n -> sprintf "%i bottles" n` |
| `_` | always, captures nothing | the last "else" branch |
| A DU case | it's that case | `\| Scored s -> ...` |
| `Some v` / `None` | it's that option case | |
| An array of a fixed length: `[\|a; b\|]` | exactly 2 items | lesson 12's `nameParts` |
| A list: `head :: tail` / `[]` | non-empty / empty | lesson 22 |
| A tuple: `(a, b)` | always (it's a pair) | `let a, b = pair` |
| A type test: `:? FormatException as e` | it's that type | exception handling |
| Stacked: `\| A \| B -> ...` | either | `\| Excused \| Voided -> None` |

### The real example (lesson 12)

```fsharp
let nameParts (s : string) =
    match s.Split(',') with
    | [| surname; givenName |] -> {| Surname = surname.Trim(); GivenName = givenName.Trim() |}
    | [| surname |]            -> {| Surname = surname.Trim(); GivenName = "(None)" |}
    | _ -> raise (System.FormatException(sprintf "Invalid student name format: \"%s\"" s))
```

In one step, this checks *how many* parts the name has **and** names each part. There's no `elements.[1]` that could crash.

### Why it beats if/elif

- It **checks the shape and extracts the data at the same time**.
- The **compiler warns about missing cases** (if/elif never does).
- It reads like a table of "situation → answer".

### Rules

1. Top to bottom, the first match wins, so `_` goes **last**. Put it first, and it would swallow everything.
2. Every branch returns the **same type**.
3. A missing case gives warning FS0025, and the program **crashes** if that value ever turns up. Don't ignore that warning.

**Interview answer:** *"Pattern matching tests a value against shapes in order, and binds parts of it to names. Combined with DUs, the compiler checks it's exhaustive. It replaces long if/else chains, type checks and casts with one construct."*

---

## 13 Handling exceptions

### What an exception is

An **exception** is the program shouting *"something went wrong, and I can't continue normally"*. When one is **raised** (or "thrown"), normal execution **stops immediately**.
The exception flies up through the functions that called each other, until something **catches** it. If nothing does, the program crashes with a stack trace.

### Raising one

```fsharp
raise (System.FormatException("Invalid student name format"))   // a specific KIND of error
failwith "Something broke"                                     // a general Exception with a message
raise <| ArgumentException("Null or empty", "message")         // same as raise (...), with <| instead of brackets
```

`raise` and `failwith` **never return**. That's why F# allows them in any branch, even when the other branches return a string.

### Catching: `try ... with` (lesson 13's Program.fs)

```fsharp
try
    Summary.summarize filePath
    0                                       // success exit code
with
| :? FormatException as e ->                // ":?" = "is it this TYPE of exception?"
    printfn "Error: %s" e.Message
    printfn "The file was not in the expected format."
    1
| :? IOException as e ->
    printfn "Error: %s" e.Message
    printfn "If the file is open in another program, please close it."
    2
| _ as e ->                                 // anything else
    printfn "Unexpected error: %s" e.Message
    3
```

- The `with` part is **pattern matching on the exception**. Same rules: top to bottom, first match wins, so the **most specific types go first** and the catch-all goes last.
- `:? FormatException as e` means "if it's a FormatException, call it e". `e.Message` is the text it was raised with.
- `try ... with` is an **expression**: both the try part and every handler give back the same type (here an int exit code).
- Related: `try ... finally` runs the `finally` block **whether or not** an exception happened. It's used for cleanup, like closing a file.

### Why handle exceptions at all

To turn a scary crash and stack trace into a **helpful message** and a clean **exit code**, and to react differently to different problems (bad format vs. file locked).

### Exceptions vs Option: the big question

| Use **Option** when… | Use an **exception** when… |
|---|---|
| "Not found" or "no value" is **normal and expected** | Something is **actually wrong** or unexpected |
| A lookup with no match, "N/A" in data | A corrupt file, the disk is full, a network failure |
| The caller should *always* think about it | Most callers can't do anything useful, so let it bubble up to one place |

### Good / bad use

- ✅ Catch at the **top level** (in `main`), where you can show a message. Catch **specific** types.
- ❌ Using exceptions for normal control flow ("try to parse, catch if it fails, for every row"). That's slow and hides the logic. Use `try...` functions and options.
- ❌ An empty catch-all that hides errors (`| _ -> ()`). The bug is still there; you just can't see it any more.

**Interview answer:** *"Exceptions are for exceptional failures. F# catches them with `try...with`, pattern-matching on the type with `:?`, most specific first. For expected absence, I'd use Option, because it's visible in the type and the compiler makes callers handle it."*

---

## 14 Generics (and `inline`)

### What generics are, like you're 5

Imagine a **box** that can hold *anything*: toys, books, socks. The box works the same way no matter what's inside.
A **generic** type or function is code written **once** that works for **any type**, with a placeholder standing in for "whatever type you use".

In F#, the placeholder is written with an apostrophe: **`'T`** (say "tee"; `'a`, `'b` and `'Key` are also common).

```fsharp
type Point<'T> = { X : 'T; Y : 'T }           // a point of ANY type, as long as X and Y match

let pFloat = { X = 1.0; Y = 2.0 }             // Point<float>
let pInt   = { X = 1;   Y = 2   }             // Point<int>
```

### You've used generics all along

- `float option`, `string option` → `Option<'T>`
- `int[]`, `Student[]` → `'T[]`
- `Array.map` has type `('T -> 'U) -> 'T[] -> 'U[]`: "give me a function from some type T to some type U, and an array of T, and I'll give you an array of U".
- `Dictionary<int, string>` → `Dictionary<'Key, 'Value>`

### Why we use them

1. **Write once, use for every type.** Without generics, you'd need `PointInt`, `PointFloat`, `PointSingle`... with the same code copied into each.
2. **Still fully type-safe.** A `Point<int>` can't accidentally get a string in it. You get flexibility **without** losing compiler checks. (The alternative, using `obj` for "anything", loses all checking.)
3. **Often free.** F#'s inference makes your functions generic automatically when they can be:

```fsharp
let first (a, b) = a          // F# infers: 'a * 'b -> 'a   (works for any pair)
```

### A generic DU (lessons 13 and 14)

```fsharp
type Optional<'T> =
    | Something of 'T
    | Nothing

let defaultValue (d : 'T) (optional : Optional<'T>) =
    match optional with
    | Something v -> v
    | Nothing -> d
```

This is a homemade `option`, which shows that the built-in one is nothing magic.

### `inline`: what it is and why `moveBy` needs it

```fsharp
let inline moveBy (dx : 'T) (dy : 'T) (p : Point<'T>) =
    { X = p.X + dx; Y = p.Y + dy }
```

**What `inline` does:** a normal function is compiled **once**, and every call jumps to it.
An `inline` function isn't called at all. The compiler **pastes its body into every place it's used**, with the real types from that place.
So `pInt |> moveBy 3 4` effectively becomes `{ X = pInt.X + 3; Y = pInt.Y + 4 }`.

**Why it's needed here:** under the hood, `int + int`, `float + float` and `string + string` are **different operations**.
A normal generic function is compiled once, so it would have to choose one `+` for every possible `'T`. It can't, so F# gives up and **locks `'T` to `int`**:

```
warning FS0064: This construct causes code to be less generic than indicated by the
type annotations. The type variable 'T has been constrained to be type 'int'.
```

Then `moveBy 3.0 4.0` fails with "expected int" (tested).
With `inline`, each call site gets its own copy, so each copy uses the right `+`. The only requirement is that `'T` **supports `+`**. This is called a **static member constraint**.

That's why:
- `moveBy "c" "d"` works: strings support `+` (concatenation), giving `{ X = "ac"; Y = "bd" }`.
- `scaleBy "factor"` **fails**: "The type 'string' does not support the operator '*'".

**When to use `inline`:**
- ✅ Small generic **math helpers** that use `+ - * /` on `'T`. This is the main real reason.
- ❌ Large functions or ones used in many places: every call site gets a full copy, so the program grows, and debugging is harder.
- ❌ "To make code faster" in general. .NET already optimises normal calls; measure first.

**Interview answers:**
- *"Generics let you write a type or function once with a type parameter like 'T, and use it with any type, while keeping full type safety. F# infers generic types automatically where it can."*
- *"`inline` substitutes the function body at each call site. In F#, it's what lets a generic function use operators like `+`, because the operator is resolved separately for each concrete type."*

---

## 15 Collection functions, part 2

### Tuples, briefly (they show up everywhere here)

A **tuple** bundles a few values **without names**: `(3, 2, ConsoleColor.Yellow)`.

```fsharp
let pair = ("Eason", 85.0)
fst pair            // "Eason"   (first, for pairs only)
snd pair            // 85.0      (second)
let name, score = pair            // unpack by pattern
```

Tuple vs record: a tuple is quick, with no type to declare, but `item1`/`item2` mean nothing to the reader. Use one for short-lived, local pairs, and a record when it travels around the code.

### `Array.groupBy`: the main new function

**What it does:** sorts items into **buckets** by a key you choose. It returns an array of `(key, itemsWithThatKey)` pairs.

Picture: you have a pile of Lego bricks. `groupBy color` puts them into one tub per colour, and labels each tub.

```fsharp
bricks
|> Array.groupBy (fun b -> b.Color)
// [| (Yellow, [|brick1; brick15|]);
//    (Green,  [|brick2; brick12|]);
//    ... |]
```

Using the result, take each `(key, items)` pair apart with a pattern in the lambda:

```fsharp
groupedByColor
|> Array.iter (fun (color, bricksOfThatColor) ->      // unpack the tuple right in the parameter
    printfn "%s:" (color.ToString())
    bricksOfThatColor |> Array.iter Brick.printConsole)
```

Sorting the groups by their key:

```fsharp
|> Array.sortBy fst                 // fst picks the key out of each (key, items)
|> Array.sortByDescending fst       // biggest key first
```

StudentScores version: group the students by surname, sort the groups alphabetically, and print each group.

```fsharp
|> Array.groupBy (fun s -> s.Surname)
|> Array.sortBy fst
|> Array.iter (fun (surname, students) -> printGroupSummary surname students)
```

**Sorting by several things at once:** sort by a tuple.

```fsharp
|> Array.sortBy (fun s -> s.GivenName, s.Id)    // by given name, then by Id when they're equal
```

### Other useful ones

| Function | Job |
|---|---|
| `Array.countBy f` | Like groupBy, but gives counts: `[\|(Red, 3); (Blue, 4)\|]` |
| `Array.distinct` | Remove duplicates |
| `Array.truncate n` | At most the first n items (won't crash if there are fewer) |
| `Array.find f` / `tryFind f` | The first match (`find` crashes if there isn't one; `tryFind` returns an option) |
| `Array.exists f` / `forall f` | Is any item / is every item true for f? |
| `Array.fold f start` | Combine all items into one value, step by step (sum is a special case) |
| `String.replicate n s` | Repeat a string n times (used to draw the brick studs) |

**Interview answer:** *"groupBy partitions a collection by a key function, returning (key, group) pairs. Combine it with sortBy fst to order the groups, and destructure the tuple in the lambda."*

---

## 16 Creating and populating arrays

### What an array is

A **fixed-length row of boxes**, side by side in memory, numbered from 0.

```fsharp
let numbers = [| 1; 2; 4; 8; 16 |]       // literal
numbers.[2]                              // 4: jump straight to box 2, instantly
numbers.[1] <- 99                        // arrays are MUTABLE: you can change a box
```

That last line is unusual for F#: **array elements can be changed in place** (the array itself still has a fixed length).
Lists and maps can't do this.

### Ways to create one

```fsharp
[| 1; 2; 3 |]                                    // 1. write them out

[| for i in 1..1000 -> i * i |]                  // 2. array expression (comprehension)

[| for i in 0..9 do                              //    with logic inside
       let x = i * i
       if x % 2 = 0 then x |]                    //    implicit yield (see section 17)

Array.init 1000 (fun i -> (i + 1) * (i + 1))     // 3. Array.init n f: f gets the INDEX 0..n-1

Array.zeroCreate<int> 10                         // 4. ten zeros (default values), to fill later
Array.create 5 "x"                               // 5. five copies of "x"
```

**`Array.init` gotcha:** the function receives the **index**, which starts at **0**. To get 1..1000, use `i + 1` (as in Ex2After).

### Good / bad use

- ✅ You need **fast access by index**, or you'll loop over the data **many times**.
- ✅ Performance-critical number crunching (it's compact in memory and cache-friendly).
- ❌ Huge data you only pass through once. It all sits in memory at once; a sequence would stream it.
- ❌ Growing and shrinking. The length is fixed, so "adding" means copying the whole array.
- ⚠️ Mutability: if you share an array and someone changes a box, everyone sees the change. Prefer not mutating.

---

## 17 Sequences

### What a sequence is, like you're 5

An **array** is a **box of cookies already baked**: they're all there and take up space, and you can grab cookie #7 straight away.
A **sequence** is a **recipe card**: no cookies exist yet. Each time someone asks for the next cookie, *one* is baked and handed over.

```fsharp
let squares = seq { for i in 1..1000 -> i * i }    // nothing is computed here!
squares |> Seq.sum                                 // NOW they're made, one at a time, and summed
```

That's called **lazy evaluation**: compute on demand, not upfront.
In .NET, a sequence is `IEnumerable<T>`. F#'s `seq<'T>` is just a nickname for it. That's what you saw in the debugger ("bricks [IEnumerable]").

### Proof it's lazy (tested)

```fsharp
let s = seq { printfn "computing 1"; yield 1; printfn "computing 2"; yield 2 }
printfn "seq created"           // printed FIRST: nothing has been computed yet
s |> Seq.head                   // prints "computing 1" only, and stops: it only needed one!
s |> Seq.iter ignore            // prints "computing 1", "computing 2" AGAIN
```

This is also the Bricks demo: with `[| do printfn "Creating elements" ... |]`, the message appears **before** "All the bricks:". As a `seq { }`, it appears **after**, at the moment the bricks are first used.

### Why sequences are great (the benefits)

1. **Memory:** only the current element needs to exist. You can sum a billion numbers without storing a billion numbers.
2. **No wasted work:** if you only need the first 10, only 10 get computed (`Seq.truncate 10`).
3. **Infinite collections are possible** (section 18): "every date from today on" can't be an array.
4. **Streaming files:** `File.ReadLines` (lazy, line by line) vs `File.ReadAllLines` (loads the whole file into an array).

### The big catch: re-evaluation

A sequence is a recipe, so **every time you loop over it, it's cooked again from scratch**.
Loop twice, and every element is computed twice, and a file is **read twice**.
That's why lesson 18's Summary does:

```fsharp
let rows =
    File.ReadLines filePath
    |> Seq.cache                // remember elements the FIRST time; later passes reuse them
let studentCount = (rows |> Seq.length) - 1    // pass 1 (reads the file, caches)
rows |> Seq.skip 1 |> ...                      // pass 2 (from the cache; no second file read)
```

### Arrays vs sequences: your notes, refined

| | Array | Sequence |
|---|---|---|
| When elements exist | All created **upfront** | Created **on demand** (lazy) |
| Memory | Every element, **at once**, in one block | Almost nothing: one element at a time |
| `x.[i]` access | **Instant** | Must walk from the start (`Seq.item i`) |
| Looping twice | Cheap | **Recomputes everything** (unless `Seq.cache`) |
| Infinite? | Impossible | Fine |
| Mutable elements? | Yes | No |

**Quick guide (from your notes, confirmed):**
- Short-lived, only exists to be piped and summarised → **seq**
- Long, but you only need some elements → **seq**
- Looping over it several times → **array** (or `Seq.cache`)
- Need access by index → **array**
- In the end: **measure with realistic data volumes**.

"All arrays are sequences, but not all sequences are arrays": arrays (and lists, sets, maps) implement `IEnumerable`, so every `Seq.xxx` function works on them.
Convert with `Array.ofSeq` / `Seq.toArray` / `List.ofSeq` / `Seq.ofList`.

**Inefficiency**, as your note says, means unnecessary calculations, unnecessary memory, and unnecessary disk or network work. Laziness avoids the first two. Arrays and `Seq.cache` avoid repeating the third.

### `yield`, `yield!` and `do`: what they mean

Inside `[| |]`, `[ ]` or `seq { }`, you're writing a little **factory** that produces elements. Every line is one of:

| Keyword | Meaning | Example |
|---|---|---|
| `yield x` | **Put x into** the collection | `yield 42` |
| `yield! xs` ("yield bang") | **Put all of xs's elements** into the collection | `yield! [1; 2]` |
| `do expr` | **Just run it**, put nothing in (a side effect) | `do printfn "hi"` |
| `->` | Shorthand for `do yield` in a `for` | `for i in 1..3 -> i * i` |

**Implicit yield:** most of the time you can leave `yield` out, and F# adds it for you:

```fsharp
[ for i in 0..9 do if i % 2 = 0 then i ]           // same as `then yield i`
```

**Why the Bricks code needs `do`** (tested): without it, F# reads the newline-separated lines as a plain list of elements, with `printfn` as the first element (type unit) and then the tuples.
All elements must have the same type, so it fails with:

```
All elements of an array must be implicitly convertible to the type of the first element,
which here is 'unit'.
```

The two fixes:
1. `do printfn "..."`: "this line is a side effect, not an element" ✅ (cleanest)
2. Write `yield` before **every** tuple. Once there's an explicit yield, F# treats `printfn` as a side effect.

**Gotcha (tested):** once you use explicit `yield`, a plain value with no `yield` is **silently thrown away** (only a warning, FS3221):

```fsharp
[ yield 0; yield! [1; 2]; 3 ]     // gives [0; 1; 2]. The 3 is GONE!
```

So it's either all implicit, or explicit everywhere.

**Interview answer:** *"A sequence is a lazily evaluated IEnumerable. Elements are computed on demand, so it uses little memory and can be infinite, but it's re-evaluated on every enumeration unless you cache it. I'd pick an array for repeated or indexed access, and a seq for streaming or one-pass pipelines."*

---

## 18 Creating sequences

### `Seq.init`: a fixed number of elements, from the index

```fsharp
Seq.init 1000 (fun i -> (i + 1) * (i + 1))      // like Array.init, but lazy
```

### `Seq.initInfinite`: a never-ending sequence

```fsharp
let squares = Seq.initInfinite (fun i -> (i + 1) * (i + 1))   // 1, 4, 9, 16, ... forever
squares |> Seq.truncate 1000 |> Seq.sum                       // only take what you need!

let from (startDate : DateTime) =
    Seq.initInfinite (fun i -> startDate.AddDays(float i))     // every day, starting at startDate

from DateTime.Now
|> Seq.filter (fun d -> d.Month = 1 && d.Day = 1)    // only New Year's Days
|> Seq.truncate 10                                   // the next 10 of them
```

This is only possible because the sequence is lazy. `Seq.filter` and `Seq.truncate` just stack up more recipe steps, and elements are made only when `Seq.iter` pulls them.

⚠️ **Never** call `Seq.length`, `Seq.sum` or `Seq.sort` on an infinite sequence without truncating it first. It runs forever.

### `Seq.truncate` vs `Seq.take`

- `Seq.truncate 10`: *at most* 10. Fine if there are fewer.
- `Seq.take 10`: *exactly* 10. **Throws an exception** if there are fewer.

### `Seq.unfold`: each element is built from the previous **state**

`unfold` is the hardest one, so slowly:

**The idea:** you have a **state** (whatever you need to remember). From the current state, you produce
**(1) the element to hand out** and **(2) the next state**. Repeat.

```fsharp
Seq.unfold (fun state ->
    // ... work out the element and the next state ...
    Some (element, nextState))      // or None to STOP the sequence
  initialState
```

Returning `Some (element, nextState)` means "here's an element, and here's what to remember for next time". Returning `None` means "the sequence ends here". If you never return `None`, the sequence is infinite.

**Drunkard's walk (lesson 18):** the state is the current position. Each step moves randomly by -1, 0 or +1.

```fsharp
let walk =
    { X = 0; Y = 0 }                            // initial state
    |> Seq.unfold (fun position ->
        let position' = { X = position.X + step(); Y = position.Y + step() }
        Some (position', position'))            // hand out the new position AND remember it
```

**Pell numbers:** each number depends on the **previous two**, so the state is a record holding them: `{ N; Pn2; Pn1 }`.

```fsharp
Some (pn, { N = n'; Pn2 = state.Pn1; Pn1 = pn })    // element = pn; new state shifts the "previous two" along
```

`initInfinite` is for when each element depends only on its **position** (i). Use `unfold` when each element depends on **what came before**.

`'` in a name (`position'`, `n'`) is just part of the name, read "position prime". It's a convention for "the next / new version of".

**Interview answer:** *"Seq.unfold generates a sequence from a state: a function returns Some(element, nextState) to continue or None to stop. It's useful when each element depends on previous ones, like Fibonacci or a random walk."*

---

## 19 Dictionaries

### What a dictionary is

A **lookup table**: you give it a **key**, and it gives you the matching **value**, almost instantly, however big it is.
Like a real dictionary: word → definition. Here, school code → school name.

```
Key  →  Value
 1   →  "Northern High School"
 2   →  "Eastern High School"
```

### Why not just an array of pairs?

To find code 2 in an array, you'd check item by item, which gets slower as it grows.
A dictionary uses a **hash table** internally: it turns the key into a number that points almost directly to the right slot. Lookup takes about the same time for 10 entries or 10 million.

### Three ways to make one (lesson 19)

```fsharp
open System.Collections.Generic

// 1. .NET Dictionary: MUTABLE (you can add and remove later)
let pairs = rows |> Seq.map (fun row -> KeyValuePair.Create(id, name))
let d1 = new Dictionary<int, string>(pairs)      // or Dictionary<_, _>, letting F# infer the types
d1.Add(5, "New School")                          // allowed
d1.[5] <- "Renamed School"                       // allowed

// 2. dict: F#'s shortcut, READ-ONLY
let d2 = rows |> Seq.map (fun row -> id, name) |> dict     // from (key, value) tuples
// d2.Add(...) -> throws NotSupportedException (tested)

// 3. readOnlyDict: read-only, and its TYPE says so (IReadOnlyDictionary has no Add at all)
let d3 = pairsAsTuples |> readOnlyDict
```

### Looking things up

```fsharp
schoolCodes.[2]                    // "Eastern High School"
schoolCodes.[99]                   // throws KeyNotFoundException if the key isn't there (tested)
match schoolCodes.TryGetValue 99 with     // the safe way
| true, name -> name
| false, _  -> "(Unknown)"
```

### `IDictionary` in a function's parameter

```fsharp
let fromString (schoolCodes : IDictionary<int, string>) (s : string) = ...
```

`IDictionary` is an **interface** (see "Interfaces"): "anything that can look up by key". It means this function accepts **either** a `Dictionary` or a `dict` result.

### The 19_Dictionaries bug you hit (`id`)

After renaming a variable to `code`, `KeyValuePair.Create(id, name)` still compiled, because **`id` is a built-in F# function** ("give back whatever you got").
So F# used that function as the key instead of complaining. Watch for names that shadow built-ins (`id`, `fst`, `snd`, `max`, `min`).

### Good / bad use

- ✅ Lots of lookups by a key: codes → names, IDs → records, caching computed results.
- ✅ A .NET `Dictionary` when you really need to add and remove often (a cache, counters), especially in performance-sensitive code.
- ❌ Data you only ever loop over. Use an array or list.
- ❌ Sharing a mutable `Dictionary` between threads without care.

---

## 20 Maps

### What a Map is

F#'s own **immutable** dictionary. Key → value lookup, like a dictionary, but **it can never change**.
"Adding" gives you a **new** map with the extra entry, and the old map is untouched.

```fsharp
let schoolCodes =
    rows
    |> Seq.map (fun row -> code, name)       // (key, value) tuples
    |> Map.ofSeq
    |> Map.add "*" "(External)"              // returns a NEW map that includes "*"
```

```fsharp
let m  = Map.ofList [ "x", 1 ]
let m2 = m |> Map.add "y" 2
// m  = map [("x", 1)]              <- unchanged!   (tested)
// m2 = map [("x", 1); ("y", 2)]
```

**"Isn't copying the whole map slow?"** No. A Map is a balanced tree, and the new map **shares** almost all of its structure with the old one. Only a small path is copied. This is called **structural sharing**.

### Safe lookup with `Map.tryFind`: Option again

```fsharp
let schoolName =
    schoolCodes
    |> Map.tryFind schoolCode                // Some "Eastern High School"  or  None
    |> Option.defaultValue "(Unknown)"
```

Compare `dictionary.[key]`, which **crashes** on a missing key. `tryFind` makes "missing" a normal, handled case. That's the F# way.

### Common Map functions

| Function | Job |
|---|---|
| `Map.ofSeq` / `ofList` / `ofArray` | Build from (key, value) pairs |
| `Map.add k v m` | New map with k → v (replaces it if k already exists) |
| `Map.remove k m` | New map without k |
| `Map.tryFind k m` | `Some v` / `None` |
| `Map.find k m` / `m.[k]` | v, or throws if it's missing |
| `Map.containsKey k m` | true / false |
| `Map.toSeq` / `toList` | Back to pairs (sorted by key!) |

A Map keeps its keys **sorted**. `Map.toList` gives the pairs in key order.

### The 20_FSharpMaps bug you hit

All keys in one map must be the **same type**. The map was built with `int` keys (`|> int`), but looked up with a `string` (`elements.[2]`):
*"Expecting a Map<string,string> but given a Map<int,string>"*. The fix was to keep the keys as strings on both sides.

### Dictionary vs Map: your table, corrected

| | .NET `Dictionary` | F# `dict` | F# `Map` |
|---|---|---|---|
| Created with | `new Dictionary<_,_>(pairs)` | `pairs \|> dict` | `pairs \|> Map.ofSeq` |
| Can change? | **Yes**, mutable | **No**, read-only (`Add` throws) | **No**, but `Map.add` gives a new copy cheaply |
| Lookup speed | Fastest (hash table) | Fastest (hash table) | Fast (tree, a bit slower) |
| Missing key | `d.[k]` throws; use `TryGetValue` | same | `Map.tryFind` → Option ✨ |
| Ordered? | No | No | Yes, by key |
| F#-friendly? | Meh | OK | **Yes** (pipes, Option) |

**Rule of thumb:** in F# code, start with **Map**. Switch to `dict`/`readOnlyDict` for big, read-only lookup tables where speed matters, and to `Dictionary` only when you really need in-place changes.

**Interview answer:** *"A Map is an immutable, sorted key-value collection implemented as a balanced tree. Adding returns a new map that shares structure with the old one. A .NET Dictionary is a mutable hash table: faster for lookups, but mutable. In F#, I'd default to Map, and use tryFind for safe lookups."*

---

## Interfaces (and C#)

(Also in `20_FSharpMaps/Before/StudentScores/maps.md`, as you asked.)

### What an interface is, like you're 5

An interface is a **job description**. It lists **what** you must be able to do, but not **how**.

> To be a **Driver**, you must be able to `Start()`, `Steer()` and `Brake()`.

A taxi driver, a bus driver and a racing driver all do those things differently. But whoever needs "a Driver" doesn't care which kind they get.
**Classes** that promise to do the job are said to **implement** the interface. By convention, interface names start with **I**.

### You've already used two

| Interface | The job | Who can do it |
|---|---|---|
| `IEnumerable<'T>` (= `seq<'T>`) | "Hand me your items one by one" | arrays, lists, sets, maps, `seq { }` |
| `IDictionary<'K,'V>` | "Look up a value by its key" | `Dictionary`, the result of `dict` |

That's why `Seq.sum` works on an array, a list **and** a set (tested: all give the same answer). It only asks for the job, not a specific class.

### Why use interfaces

1. **Swappable parts.** Code written against `IShape` works for every shape, including ones written next year.
2. **Testing.** Pass a fake `IEmailSender` in tests so you don't send real emails.
3. **Loose coupling.** A function asks for the *smallest job it needs* ("anything I can loop over"), not a specific class. Lesson 19's `fromString` takes `IDictionary` for this reason.

**Downside:** an extra layer to read and jump through. Don't make an interface that only one class will ever implement.

### In C# (where you'll see them most)

```csharp
public interface IShape
{
    double Area();
    string Name { get; }
}

public class Circle : IShape            // ": IShape" means "I do this job"
{
    private readonly double _r;
    public Circle(double r) { _r = r; }
    public double Area() => Math.PI * _r * _r;
    public string Name => "Circle";
}

static string Describe(IShape s) => $"{s.Name}: {s.Area():0.00}";   // works for ANY shape
```

In C# web apps (ASP.NET), constructors ask for interfaces like `ILogger<T>` or `IUserRepository`, and the framework passes in a real one. This is **dependency injection**, and it's built entirely on interfaces.

### In F# (tested: it runs)

```fsharp
type IShape =
    abstract Area : unit -> float     // abstract = a promise with no body
    abstract Name : string

type Circle(radius : float) =
    interface IShape with
        member this.Area() = Math.PI * radius * radius
        member this.Name = "Circle"

let describe (s : IShape) = sprintf "%s: %0.2f" s.Name (s.Area())
describe (Circle(1.0))                  // "Circle: 3.14"

let c = Circle(2.0)
(c :> IShape).Area()                    // F# quirk: upcast with :> to call it directly on a Circle

let tri = { new IShape with             // object expression: implement it on the spot, no class
              member _.Area() = 0.5
              member _.Name = "Triangle" }
```

- `:>` is an **upcast**: "treat this Circle as an IShape". It always succeeds.
- `:?>` is a **downcast**: "I think this IShape is really a Circle". It can fail at runtime.
- `:?` is a **type test**: "is it a Circle?", as used in `| :? FormatException as e`.

### Interface vs discriminated union: a classic interview question

| | DU | Interface |
|---|---|---|
| The set of kinds is | **closed**: all cases listed in one place | **open**: anyone can add a class |
| Adding a new **kind** | Update every `match` (the compiler finds them) | Just write a new class |
| Adding a new **operation** | Just write a new function with a `match` | Add it to the interface **and every class** |
| Best for | Known options: TestResult, Payment, order status | Plugins, shapes, storage back-ends, test fakes |

---

## 21 Object orientation and classes

### What a class is

A **class** is a blueprint for **objects**: things that bundle **data** (state) together with **behaviour** (methods) that act on it, and that can **change over time**.
A record is "just data". A class is "data plus the actions that change it".

### The course's `ConsolePrompt`, line by line

```fsharp
type ConsolePrompt(message : string, maxTries : int) =      // (1) primary constructor

    do                                                      // (2) runs when an object is created
        if String.IsNullOrWhiteSpace(message) then
            raise <| ArgumentException("Null or empty", "message")

    let trimmedMessage = message.Trim()                     // (3) private value
    let mutable tryCount = 0                                // (4) private, CHANGEABLE state

    let mutable foreground = ConsoleColor.White
    let mutable background = ConsoleColor.Black

    member this.ColorScheme                                 // (5) a property with custom get and set
        with get() = foreground, background
        and set(fg, bg) =
            if fg = bg then
                raise <| ArgumentException("Foreground and background must be different")
            foreground <- fg
            background <- bg

    member this.GetValue() =                                // (6) a method
        tryCount <- tryCount + 1
        printf "%s: " trimmedMessage
        let input = Console.ReadLine()
        if String.IsNullOrWhiteSpace(input) && tryCount < maxTries then
            if this.BeepOnError then Console.Beep()
            this.GetValue()                                 //     calls itself again: retry
        else
            input

    member val BeepOnError = true with get, set             // (7) an auto-property
```

1. **Primary constructor:** the brackets after the type name are the constructor's parameters. `ConsolePrompt("Your name", 3)` creates an object. (In F#, `new` is optional.) The parameters are visible everywhere inside the class.
2. **`do` block:** code that runs **once, at creation**. It's used here for **validation**: refuse to create a broken object. "Fail fast" means it's better to crash at creation than to behave strangely later.
3. **`let` inside a class = private.** Outsiders can't see `trimmedMessage`. This is **encapsulation**: hiding the inner details.
4. **`let mutable`:** state that changes during the object's life. `tryCount` remembers how many tries there have been. That **remembering between calls** is exactly what a record can't do, and it's why this is a class.
5. **Property with `get`/`set`:** looks like a field from outside (`prompt.ColorScheme <- (Red, Black)`), but runs code. Here, `set` **validates** (it refuses identical colours) before changing the private state.
6. **`member this.Method()`:** a public method. `this` is the name you give to "the current object", so you can use its other members (`this.BeepOnError`). Any name works; `this` and `_` are common.
7. **`member val X = v with get, set`:** an **auto-property**, a property with a hidden storage field, created for you. Set it with `<-`: `namePrompt.BeepOnError <- false`.

Using it:

```fsharp
let namePrompt = ConsolePrompt("Please enter your name", 3)
namePrompt.BeepOnError <- false
let name = namePrompt.GetValue()
```

### The OO vocabulary, in one line each

- **Object / instance:** one actual thing made from the class blueprint (`namePrompt`).
- **Encapsulation:** hide the internal state, and expose only safe operations (private `let`, public `member`).
- **Constructor:** the code that creates and validates an object.
- **Method:** a function that belongs to an object.
- **Property:** a value-like member, possibly with get and set logic.
- **Inheritance:** a class extends another and gets its members. F# supports it, but prefers interfaces and composition.
- **Polymorphism:** one interface, many implementations (`IShape` → Circle, Square).

### Why (and when) to use classes in F#

- ✅ **Encapsulated, changing state**: a prompt counting tries, a colour history, a connection, a game object.
- ✅ **Talking to .NET/C# code**, which is built from classes and interfaces.
- ✅ **Implementing interfaces.**
- ❌ **Plain data:** use a record. It's immutable, with free equality and printing.
- ❌ **Fixed alternatives:** use a DU.
- ❌ **Stateless helpers:** use a module with functions. A class with no state is just ceremony.

**F# style:** a "functional core" (records, DUs, pure functions) with classes at the edges, where state or .NET interop is needed.

**Interview answer:** *"F# is functional-first but fully object-oriented. I'd use records and DUs for data, and classes when I need encapsulated mutable state or .NET interop. A class has a primary constructor, private let bindings, members, and properties with get/set."*

---

## 22 F# lists

### What an F# list is

An **immutable, singly linked list**. Picture a **chain** or a train:
each element is a carriage that holds a value **and** a link to the next carriage. The last one links to "empty" (`[]`).

```
"sometimes" → "the" → "cat" → "sat" → []
   head      └───────── tail ──────────┘
```

```fsharp
let numbers = [ 1; 2; 4; 8; 16 ]                 // square brackets, no bars
let more    = [ for i in 0..4 -> pown 2 i ]      // list expression
let yetMore = List.init 5 (fun i -> pown 2 i)
yetMore.[2]                                      // works, BUT walks the chain: slow for big lists
// yetMore.[1] <- 99                             // ERROR: lists are immutable
```

### The cons operator `::`: adding to the front

```fsharp
let strings  = [ "the"; "cat"; "sat" ]
let strings2 = "sometimes" :: strings      // ["sometimes"; "the"; "cat"; "sat"]
// strings is UNCHANGED: ["the"; "cat"; "sat"]   (tested)
```

**Why it's instant:** the new list is one new carriage pointing at the **existing** chain. Nothing is copied, and both lists share the same carriages.
It's safe to share them, because nobody can change a carriage (immutability).
This is **structural sharing**, the same idea as Map.

Adding to the **end** (`list @ [x]`) is slow: the whole chain has to be copied. So F# code builds lists **at the front**.

### Pattern matching on lists: head and tail

```fsharp
match strings2 with
| head :: tail -> printfn "head: %s, tail: %A" head tail   // non-empty: first item plus the rest
| [] -> printfn "Empty list"                                // empty
```

`head :: tail` as a pattern means "a list with at least one item: call the first one `head` and the rest `tail`". There are exactly two shapes, empty and non-empty, so the compiler can check you've handled both.

### `ColorHistory` (lesson 22): a class that uses a list inside

```fsharp
type ColorHistory(initialColors : seq<Color>, maxLength : int) =
    let mutable colors = initialColors |> Seq.truncate maxLength |> List.ofSeq

    member this.Add(color : Color) =
        colors <- color :: colors                // new colour at the front: the latest
                  |> List.distinct               // re-adding a colour "moves" it to the front
                  |> List.truncate maxLength     // the oldest fall off the end

    member this.TryLatest() =
        match colors with
        | head :: _ -> Some head                 // latest = head
        | [] -> None                             // empty history: Option, not a crash

    member this.RemoveLatest() =
        match colors with
        | _ :: tail -> colors <- tail            // drop the head
        | [] -> ()                               // nothing to remove, which is fine
```

This shows the mix nicely: the **class** gives encapsulated state (`mutable colors`), while each *version* of the list is immutable. "Changing" the history means replacing the field with a new list.

### Good / bad use

- ✅ Small to medium collections, built up **at the front**, processed head-first or recursively.
- ✅ Idiomatic F# data: immutable and easy to pattern-match.
- ❌ Access by index (`list.[5000]` walks 5000 links). Use an array.
- ❌ Appending to the end in a loop. Build it backwards and reverse it, or use an array.
- ❌ Huge numeric data. Arrays are much more memory- and cache-efficient.

**Interview answer:** *"An F# list is an immutable singly linked list. Prepending with `::` is O(1) and shares structure, but indexing and appending are O(n). You pattern-match on it with `head :: tail` and `[]`. It's the default functional collection; I'd switch to arrays for indexed access or performance."*

---

## The big comparison tables

### Which collection?

| | Array `[\| \|]` | List `[ ]` | Seq `seq { }` |
|---|---|---|---|
| Mutable elements? | Yes | No | No |
| Evaluated | Eagerly (all upfront) | Eagerly | **Lazily** (on demand) |
| Index access `.[i]` | **Instant** | Walks the chain (slow) | Walks from the start (slow) |
| Add to front | Copies everything | **Instant** (`::`) | n/a |
| Memory | Compact block | One node per item | Almost none |
| Infinite? | No | No | **Yes** |
| Pattern matching | Fixed lengths `[\|a;b\|]` | **`head :: tail`** | No |
| Pick it when | Speed, indexing, repeated loops | Idiomatic F#, build at the front, recursion | Big, streaming, one-pass or infinite data |

### Which "shape of data"?

| You need… | Use | Why |
|---|---|---|
| A bundle of named fields (A **and** B **and** C) | **Record** | Immutable, equality, simple |
| One of several cases (A **or** B **or** C) | **Discriminated union** | Compiler-checked matching |
| "Maybe a value" | **Option** | No nulls, forced handling |
| Two or three values, briefly | **Tuple** | Quick, no declaration |
| A quick named result inside a function | **Anonymous record** | Names without a type |
| Changing state plus behaviour, or .NET interop | **Class** | Encapsulation |
| "Anything that can do job X" | **Interface** | Open set of implementations |
| Lookup by key | **Map** (or `dict` / `Dictionary`) | Fast lookup |

### Which way to "fail"?

| Situation | Use |
|---|---|
| A value may legitimately be missing | `Option` (`tryFind`, `tryFromString`) |
| A bad input you can't recover from locally | `raise` a specific exception (`FormatException`, `ArgumentException`) |
| A quick "this should never happen" | `failwith "message"` |
| Showing the user a friendly message | `try ... with` at the top (in `main`), most specific type first |

---

## Interview questions with answers

**Q: What does "functional-first" mean for F#?**
Immutability, expressions and functions-as-values are the default. You *can* write mutable OO code, but you opt into it (`mutable`, classes).

**Q: Why immutability?**
Data can't change behind your back, so code is easier to reason about, test, and run in parallel. You create new values instead of changing old ones, and structural sharing keeps that cheap.

**Q: What's the difference between `map`, `iter`, `filter` and `choose`?**
`map` transforms each item into a new collection. `iter` runs a side effect and returns unit. `filter` keeps the items matching a condition. `choose` maps to options and keeps only the `Some` values, unwrapped.

**Q: Record vs class?**
A record is immutable data with structural equality, and it's the default. A class is for encapsulated, changing state or .NET interop.

**Q: Record vs discriminated union?**
A record is "A and B and C" (all fields at once). A DU is "A or B or C" (exactly one case, each with its own data).

**Q: How does F# avoid null reference exceptions?**
F# types can't be null by default, and missing values are modelled with `Option`, which the compiler forces you to handle.

**Q: What is currying / partial application?**
Multi-argument functions are chains of one-argument functions. Supplying some of the arguments returns a specialised function. That's why F# puts the data parameter last: it works with `|>`.

**Q: What is pattern matching, and why is it better than if/else?**
It tests a value against shapes in order, and binds parts of it to names. With DUs, the compiler checks it's exhaustive, and it extracts data and branches in one step.

**Q: Array vs list vs sequence?**
Array: mutable, fast indexing. List: immutable linked list, fast prepend, pattern-matchable. Seq: lazy, can be infinite, re-evaluated on every pass unless cached.

**Q: What is lazy evaluation? A pro and a con?**
Values are computed only when needed. Pro: less memory and work, infinite sequences possible. Con: work is repeated on each enumeration, and side effects happen at surprising times.

**Q: What is `Seq.cache` for?**
To evaluate a sequence once and reuse the results, for example so a file isn't read twice.

**Q: Map vs Dictionary?**
Map: immutable, sorted, a tree, with safe `tryFind`. Dictionary: mutable hash table, faster lookups. Default to Map in F#.

**Q: What are generics?**
Types and functions with type parameters (`'T`), written once and type-safe for every type. F# infers them automatically.

**Q: What does `inline` do?**
It pastes the function's body into each call site at compile time. It lets generic code use operators like `+` per concrete type (static member constraints). Use it only for small functions.

**Q: Exceptions or Option?**
Option for expected absence. Exceptions for truly exceptional failures, caught at a boundary with `try...with`, most specific type first.

**Q: What's an interface?**
A contract listing members without implementations. Classes implement it, and code depends on the contract instead of a concrete class, which gives swappability, testability and loose coupling. Example: `IEnumerable<T>`, which is why `Seq` functions work on every collection.

**Q: DU or interface for a set of shapes?**
If the set is fixed and you'll add operations often, use a DU. If others will add new kinds often, use an interface.

**Q: Why does file order matter in F#?**
Something can only be used after it's defined. That prevents circular dependencies and keeps the structure one-way.

---

## Glossary

| Term | Meaning |
|---|---|
| **argv** | The string array of command-line arguments passed to `main` |
| **binding** | A `let` that gives a name to a value or function |
| **cons** `::` | Put an element at the front of a list. Also a pattern for head and tail |
| **currying** | Multi-argument functions are chains of one-argument functions |
| **eager** | Computed immediately (arrays, lists) |
| **encapsulation** | Hiding an object's internal state behind its members |
| **expression** | Code that produces a value (almost everything in F#) |
| **generic / `'T`** | A type placeholder: code that works for any type |
| **higher-order function** | A function that takes or returns a function (`Array.map`) |
| **immutable** | Can't change after creation |
| **inline** | The function body is pasted into each call site |
| **lambda** | An anonymous function: `fun x -> x + 1` |
| **lazy** | Computed only when needed (sequences) |
| **module** | A named group of functions and values |
| **namespace** | A named group of modules and types |
| **partial application** | Supplying only some arguments to get a new function |
| **pattern** | A shape used in `match` to test and pull apart values |
| **pipe** `\|>` | Pass the left value as the last argument of the right function |
| **side effect** | Anything beyond returning a value: printing, writing a file, mutating |
| **structural equality** | Equal if the contents are equal (records, tuples, lists, DUs) |
| **structural sharing** | A new immutable collection reuses unchanged parts of the old one |
| **type inference** | The compiler works out the types for you |
| **unit** `()` | "No meaningful value", like `void`, but a real value |
| **upcast** `:>` / **type test** `:?` | Treat as a parent type or interface / check a value's type |
| **yield / yield! / do** | Inside `[ ]` / `seq { }`: produce one element / produce all of another collection / just run code |
