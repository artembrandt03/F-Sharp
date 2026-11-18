// BANK: the single stateful object in the program.
//
// The pattern is "functional core, imperative shell". Ledger decides what should
// happen, purely. This class just remembers the latest result between calls.
// It's the same shape as ColorHistory in lesson 22: a class holding immutable
// values in `let mutable` fields.

namespace TinyBank

open System
open System.Collections.Generic

// WHY A CLASS AND NOT A RECORD: this has state that changes across calls, and
// that state should be hidden so outsiders can only change it through Process.
type Bank(directory : IReadOnlyDictionary<AccountId, Account>, historySize : int) =

    // `do` runs during construction. Failing here means a Bank in a nonsense
    // configuration can never exist.
    do
        if historySize < 1 then
            raise (ArgumentException("Must be at least 1", "historySize"))

    // `let` inside a class is private: this is encapsulation.
    let mutable balances = Ledger.opening directory.Values
    let mutable appliedCount = 0

    // WHY LISTS: new items go on the front with `::`, which is instant and
    // copies nothing. The newest item being first suits "recent activity"
    // perfectly. The two full histories are reversed once, in Snapshot.
    let mutable recentActivity : Outcome list = []
    let mutable rejectedTransactions : (Transaction * RejectionReason) list = []
    let mutable invalidRows : (int * string) list = []

    member this.Process(row : Parsed<Transaction>) : unit =
        match row with
        | Invalid(lineNumber, reason) ->
            invalidRows <- (lineNumber, reason) :: invalidRows
        | Valid transaction ->
            let updatedBalances, outcome = Ledger.apply directory transaction balances
            balances <- updatedBalances
            recentActivity <- outcome :: recentActivity |> List.truncate historySize

            match outcome with
            | Applied _ -> appliedCount <- appliedCount + 1
            | Rejected(rejected, reason) ->
                rejectedTransactions <- (rejected, reason) :: rejectedTransactions

    // Returns immutable data, so callers can read everything but change nothing.
    member this.Snapshot() : BankSnapshot =
        {
            AccountBalances =
                directory.Values
                |> Seq.map (fun account -> account, Map.find account.Id balances)
                |> Array.ofSeq
            AppliedCount = appliedCount
            RejectedTransactions = List.rev rejectedTransactions
            InvalidRows = List.rev invalidRows
            RecentActivity = recentActivity
        }
