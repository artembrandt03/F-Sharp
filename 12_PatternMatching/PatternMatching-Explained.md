# Pattern Matching, Explained

A walkthrough of lesson 12's code: the small demo in `Before/PatternMatching`, and the real change in `After/StudentScores`.

---

## The one idea behind everything

```fsharp
match value with
| pattern1 -> result1
| pattern2 -> result2
| _        -> result3
```

A **pattern** is a picture of what the value might look like, and it can have named blanks.
F# checks the patterns **from top to bottom**. At the **first one that fits**, it:

1. fills in the named blanks with the matching pieces of the value, and
2. returns that branch's result.

You already used this in lesson 11: `| Scored score -> Some score` means "does it look like `Scored <something>`? If so, call the something `score`".
Lesson 12 shows that `match` works on **any** value, not just DUs: numbers, options, arrays and tuples.

The rules, in plain words:

- **Top to bottom, first fit wins.** Later branches are never even looked at.
- **A name in a pattern always fits and captures that piece.** `x` means "anything goes here, and call it `x`".
- **`_` fits anything, without giving it a name.** That's why it goes last: it catches whatever's left.
- **Every branch returns the same type**, because, like `if`, a `match` produces one value.
- **The compiler warns if some value wouldn't fit any branch.**

---

## Part 1: the demo (`Before/PatternMatching/Program.fs`)

### `Green.bottles`: matching on exact values

```fsharp
let bottles n =
    match n with
    | 1 -> "One green bottle"
    | 2 -> "Two green bottles"
    | _ -> sprintf "%i green bottles" n
```

- `| 1 ->` means "is `n` exactly 1?" It's the same as `if n = 1`.
- `| _ ->` means "anything else". It has to be last. Put it first, and every number would land there, because the first branch that fits wins.
- Without `_`, the compiler warns: what about 3? `n` is an `int`, so it could be any number.

This is just a neater `if / elif / else`. For `i in 1..10`, it prints "One green bottle", "Two green bottles", then "3 green bottles" … "10 green bottles".

### `Option.describe`: an option is just a DU

```fsharp
let describe x =
    match x with
    | Some v -> sprintf "The value was %O" v
    | None -> sprintf "There was no value"
```

`option` is a DU that's built into F#. In effect it's defined like this:

```fsharp
type Option<'T> =
    | Some of 'T
    | None
```

So matching on it works exactly like your own DUs (`Delivery`, `Payment`):

- `Some v` means "is it a `Some` box? Call what's inside `v`".
- `None` is the empty case, so there's nothing to name.

`Option.defaultValue` from lesson 8 does this match for you behind the scenes.
`%O` prints any kind of value, which is why `describe` works for `Some 99` and `Some "abc"` alike.

### `Array.describe`: matching on the shape of an array

```fsharp
let describe a =
    match a with
    | [||]     -> "An empty array"
    | [|x|]    -> sprintf "An array with one element: %O" x
    | [|x; y|] -> sprintf "An array with two elements: %O, %O" x y
    | _        -> sprintf "An array with %i elements." a.Length
```

This is the new part. An array pattern checks **how many items there are** and names each one, all in one step:

| Input | Which branch fits | Names it fills |
|---|---|---|
| `[\|\|]` | `[\|\|]`: exactly 0 items | (none) |
| `[\|"hello"\|]` | `[\|x\|]`: exactly 1 item | `x = "hello"` |
| `[\|"hello"; "world"\|]` | `[\|x; y\|]`: exactly 2 items | `x = "hello"`, `y = "world"` |
| `[\|"world"; "hello"\|]` | `[\|x; y\|]` | `x = "world"`, `y = "hello"` |
| `[\|"the"; "cat"; "sat"\|]` | none of the first three fits, so `_` | (none) |

Two things to notice:

- **The names follow position, not value.** That's why the video runs it with "hello", "world" and then reversed. `x` is always the first item.
- `[|x|]` doesn't mean "an array containing something called x". It means "an array of exactly one item, whatever that item is, and call it `x`".

Without this, you'd write `if a.Length = 0 ... elif a.Length = 1 then let x = a.[0] ...`.
The pattern checks the length and pulls out the items at the same time, and you can't get the index wrong.

### `Tuple.describe2`: pulling a tuple apart

```fsharp
let describe2 t =
    let a, b = t
    ...
```

A **tuple** bundles a few values together without names: `("hello", "world")`.
Writing `let a, b = t` is pattern matching too, just inside a `let` instead of a `match`.
The pattern `a, b` means "a pair. Call the first part `a` and the second `b`".
You don't need a `match` here because a pair always has exactly two parts, so only one shape is possible.

---

## Part 2: the real change in StudentScores (`After/StudentScores/Student.fs`)

### The problem with lesson 11's version

```fsharp
let namePart i (s : string) =
    let elements = s.Split(',')
    elements.[i].Trim()
// ...
let given = namePart 1 name
let sur = namePart 0 name
```

This had two problems:

1. **It split the name twice**, once for each call. That's the `// TODO: Inefficient` comment in the old code.
2. **It crashed on names without a comma.** `"Madonna".Split(',')` is `[| "Madonna" |]`, so `elements.[1]` throws an `IndexOutOfRangeException`.

### The new version: split once, then match on the shape

```fsharp
let nameParts (s : string) =
    let elements = s.Split(',')
    match elements with
    | [|surname; givenName|] ->
        {| Surname = surname.Trim()
           GivenName = givenName.Trim() |}
    | [|surname|] ->
        {| Surname = surname.Trim()
           GivenName = "(None)" |}
    | _ ->
        raise (System.FormatException(sprintf "Invalid student name format: \"%s\"" s))
```

This is `Array.describe` from the demo, used for something real. Here's how it handles different inputs:

| `s` | `elements` | Branch | Result |
|---|---|---|---|
| `"Eason, Kit"` | `[\|"Eason"; " Kit"\|]` | 2 items | Surname "Eason", GivenName "Kit" |
| `"Madonna"` | `[\|"Madonna"\|]` | 1 item | Surname "Madonna", GivenName "(None)" |
| `"A, B, C"` | 3 items | `_` | error: "Invalid student name format" |

Each case is handled explicitly. There's no crash on one-part names, and anything strange gets a clear error message instead of a confusing index error.

There are two new pieces of syntax here.

**1. `{| ... |}`: an anonymous record.**
The function needs to return two things, a surname and a given name. A normal record would need a `type` declaration first.
An anonymous record is a record you create on the spot, without declaring a type. The bars inside the braces are the difference:

```fsharp
{  Surname = ...  }     // normal record: needs `type X = { ... }` somewhere
{| Surname = ... |}     // anonymous record: no type declaration needed
```

You read its fields the same way: `name.Surname`.
It's handy for "this function returns a couple of named values". It's not something you'd use as a main type like `Student`.

**2. `raise (System.FormatException(...))`: stop with an error.**
It's similar to `failwith`, but it raises a specific *kind* of error, here "the format is wrong".
Every branch of a `match` must return the same type, but `raise` never returns: it stops the program. So F# accepts it in any branch.
Lesson 13 covers this properly.

### How `fromString` uses it

```fsharp
let name = elements.[0] |> nameParts     // split and match ONCE
...
Surname = name.Surname
GivenName = name.GivenName
```

One call, one split, and the result has both parts with names.

### The exercise version (`Ex1After`): a tuple instead

The lesson's exercise did the same thing, but returned a **tuple** instead of an anonymous record:

```fsharp
| [|surname; givenName|] -> surname.Trim(), givenName.Trim()
| [|surname|]            -> surname.Trim(), "(None)"
...
let sur, given = name |> nameParts      // pull the tuple apart, like describe2
```

Both work. The difference:

- **Tuple:** shorter to write, but the parts have no names. You have to remember that the first is the surname, and it's easy to swap them by mistake.
- **Anonymous record:** a bit longer, but `name.Surname` can't be mixed up.

---

## Part 3: what didn't change, but is the same idea

`TestResult.tryEffectiveScore` from lesson 11 is still there unchanged, and now you can see it's the same thing:

```fsharp
match testResult with
| Absent -> Some 0.0           // exact-case pattern, like `| 1 ->` in bottles
| Excused
| Voided -> None               // two patterns sharing one branch
| Scored score -> Some score   // a case with a name in it, like `Some v`
```

So there aren't separate topics for "DU matching", "option matching" and "array matching".
It's **one feature**, pattern matching, applied to different kinds of values.

---

## Cheat sheet

| Pattern | Fits when… | Gives you |
|---|---|---|
| `1`, `"A"` | the value equals exactly that | nothing |
| `x` (any name) | always | the whole value, named `x` |
| `_` | always | nothing (you don't care) |
| `Some v` / `None` | it's that option case | `v` = the value inside |
| `Scored s` / `Absent` | it's that DU case | `s` = the data inside |
| `[\|\|]`, `[\|x\|]`, `[\|x; y\|]` | the array has exactly 0, 1 or 2 items | each item, named |
| `a, b` | (a pair: always fits) | both parts, named |

When you read a `match`, ask yourself for each line: **"Does the value look like this? If so, what do the names get filled with?"**
Then go top to bottom and stop at the first yes.
