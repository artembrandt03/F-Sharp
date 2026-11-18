namespace GenericPoints
 
type Point<'T> = 
    {
        X : 'T
        Y : 'T
    }
 
module Point =
 
    let inline moveBy (dx : 'T) (dy : 'T) (p : Point<'T>) =
        {
            X = p.X + dx
            Y = p.Y + dy
        }

// Claude Code: for future final cheat sheet, explain function inlining, what it does and how it works        

// [Claude Code explaining] FUNCTION INLINING (`let inline`)
//
// WHAT IT IS
//   Normally a function is compiled ONCE into one piece of code, and every
//   call jumps to that piece. `inline` says: "don't make one shared copy.
//   At every place I'm called, paste my body right there instead."
//   So `pInt1 |> Point.moveBy 3 4` is compiled as if you'd written
//       { X = pInt1.X + 3; Y = pInt1.Y + 4 }
//   right there, with the types of THAT call (ints).
//
// WHY moveBy NEEDS IT (the real reason it's in this lesson)
//   The body uses `+`. In .NET, int + int, float + float and string + string
//   are DIFFERENT operations under the hood. A normal (non-inline) generic
//   function is compiled once, so the compiler must pick ONE `+` for every
//   possible 'T. It can't, so it gives up and locks 'T to int:
//       warning FS0064: ... 'T has been constrained to be type 'int'
//   and then `moveBy 3.0 4.0` fails ("expected int"). I tested this.
//   With `inline`, there is no single shared copy: each call site gets its
//   own pasted body, so the compiler picks int's `+` for the int call,
//   float's `+` for the float call, and so on. It only needs 'T to
//   "support +" (a so-called member constraint). That's also why
//   scaleBy "factor" fails: string has a `+` but no `*`.
//
// WHEN TO USE IT
//   * Good: small generic MATH helpers that use +, -, *, / on 'T
//     (exactly moveBy / scaleBy). This is the main everyday reason.
//   * Bad: big functions, or functions called from many places. Each call
//     site gets a full copy, so the program grows, and you can't set a
//     breakpoint inside it as easily. Don't add `inline` "for speed"
//     everywhere; .NET already optimises normal calls well.
//
// INTERVIEW ONE-LINER
//   "inline copies the function body into each call site at compile time.
//    In F# it's mainly used so a generic function can use operators like +
//    on any type that supports them, which a normal generic can't do."
//
// More in Cheat Sheet/FSharp-Cheat-Sheet.md, section "Generics".
