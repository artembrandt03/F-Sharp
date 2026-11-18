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