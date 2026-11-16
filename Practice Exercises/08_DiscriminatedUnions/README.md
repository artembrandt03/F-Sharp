# 08 - Discriminated Unions practice

Based on lesson **27**: `type X = | A | B of float`, building values with case names, `match ... with` (including taking the data out of a case), stacking cases, `_`, the "incomplete pattern matches" warning, and DU + `option` + `Array.choose` (like `tryEffectiveScore`).

| Level  | Exercise        | Type                                              |
|--------|-----------------|---------------------------------------------------|
| Easy   | E1_Deliveries   | Bugged: 4 bugs. **Read its CONCEPTS section first.** |
| Medium | M1_Payments     | Add 1 case + write 5 functions (~35 lines)        |

Each `Program.fs` starts with a **CONCEPTS** section explaining what you'll use, and ends with **HINTS - LAST RESORT**.

```bash
cd "Practice Exercises/08_DiscriminatedUnions/Medium/M1_Payments"
dotnet run Samples/Payments.txt
```
