Some diffs:

F# Dictionary           F# Map                  .NET Dictionary

dict                    Map.ofSeq               new Dictionary<_,_>(...)

Elements                Elements                Elements mutable
immutable               immutable but
                        easy to copy with 
                        new or removed elems

(Notes for claude later: in the final cheat sheet - cover interfaces as well and how they're used in c#)
I already saw some interfaces ->
    *IEnumerable*           *IDictionary*
    Trinagle                Key         Value
    Rectangle               3           Trinagle
    Octagon                 4           Rectangle
    ...                     ...
---

## [Claude Code explaining] Interfaces (and how C# uses them)

**Corrections to the table above:**
- `dict` gives you a **read-only** dictionary. Calling `.Add` on it throws `NotSupportedException` (I tested it). It isn't "copy with changes" like Map; it simply can't change.
- A Map really is immutable. `Map.add` doesn't change the old map: it returns a **new** map with the extra entry, and the old one stays as it was.
- Looking up a missing key with `d.[key]` throws `KeyNotFoundException`, for both kinds. `Map.tryFind` returns an option instead, which is safer.

### What an interface is, like you're 5

An interface is a **job description**. It lists what you must be able to do, but not how you do it.

> "To be a **Driver**, you must be able to `Start()`, `Steer()` and `Brake()`."

A taxi driver, a bus driver and a racing driver all do those things differently. But anyone who needs a driver just asks for "a Driver" and doesn't care which kind they get.

In code, the interface is the job description, and the classes that **implement** it are the workers.
By convention, interface names start with **I**: `IEnumerable`, `IDictionary`, `IShape`.

### The two you've already met

| Interface | The job description | Who does that job |
|---|---|---|
| `IEnumerable<T>` (F# calls it `seq<T>`) | "You can hand me your items one by one" | arrays, lists, sets, maps, seq { } ... |
| `IDictionary<K,V>` | "You can look up a value by its key" | `Dictionary`, the result of `dict` |

That's why `Seq.sum` works on an array, a list **and** a set. It only asks for the job `IEnumerable`, and all of those can do it.
It's also why lesson 19's `Student.fromString` takes an `IDictionary<int, string>`: it works with **any** dictionary-like thing, whether it's a `new Dictionary<_,_>` or a `dict` result.

### WHY use interfaces

1. **Swap things easily.** Code written against `IShape` works for every current shape *and* every future one, without changes.
2. **Testing.** You can pass a fake `IDatabase` in tests instead of the real one.
3. **Less coupling.** Your function asks for the smallest job it needs (for example "anything I can loop over"), not one specific class.

The downside is that they add an extra layer to read. Don't create an interface that only one class will ever implement "just in case".

### C# (where you'll see them most)

```csharp
public interface IShape          // the job description
{
    double Area();
    string Name { get; }
}

public class Circle : IShape     // ": IShape" = "I can do that job"
{
    private readonly double _r;
    public Circle(double r) { _r = r; }
    public double Area() => Math.PI * _r * _r;
    public string Name => "Circle";
}

// Works with ANY shape, now and in the future:
static string Describe(IShape s) => $"{s.Name}: {s.Area():0.00}";

List<IShape> shapes = new() { new Circle(1), new Square(2) };
```

In C#, interfaces are everywhere. In ASP.NET, for example, you ask for an `ILogger` or `IRepository` in a constructor (this is called **dependency injection**), and the framework hands you a real one.

### The same thing in F# (tested, it runs)

```fsharp
type IShape =
    abstract Area : unit -> float      // "abstract" = no body, just the promise
    abstract Name : string

type Circle(radius : float) =
    interface IShape with              // "I do the IShape job, like this:"
        member this.Area() = Math.PI * radius * radius
        member this.Name = "Circle"

let describe (s : IShape) = sprintf "%s: %0.2f" s.Name (s.Area())

let c = Circle(2.0)
(c :> IShape).Area()     // F# quirk: to call it directly on a Circle you
                         // must first "upcast" with :>  (C# doesn't need this)

// Object expression: implement an interface on the spot, no class needed
let tri = { new IShape with
              member _.Area() = 0.5
              member _.Name = "Triangle" }
```

**Interface vs F# discriminated union**, a classic interview question:
- **DU:** a fixed set of cases. Adding a new *function* is easy, and the compiler checks you've handled every case. Adding a new *case* means updating every `match`.
- **Interface:** an open set of implementations. Adding a new *class* is easy and nothing else changes. Adding a new *method* means updating every class.

Pick a DU when the set of options is closed and known (TestResult, Payment). Pick an interface when others will add new kinds later (plugins, shapes, data stores).

More in `Cheat Sheet/FSharp-Cheat-Sheet.md`, section "Interfaces".
