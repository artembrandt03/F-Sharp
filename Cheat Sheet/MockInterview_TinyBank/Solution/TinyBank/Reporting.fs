// REPORTING: turns a snapshot into text. No banking decisions are made here.

namespace TinyBank

open System.IO

// WHY AN INTERFACE: the report only needs "something I can write a line to".
// Console output, a file, or a fake that collects lines in a test all fit, and
// the report code doesn't change. This is the open-ended case where an interface
// beats a union: new destinations can be added without touching existing code.
type IReportWriter =
    abstract WriteLine : string -> unit

type ConsoleReportWriter() =
    interface IReportWriter with
        member this.WriteLine line = printfn "%s" line

// Lines are collected first and written together by Save, so a failure while
// building the report can't leave a half-written file behind.
type FileReportWriter(path : string) =
    let mutable lines : string list = []

    interface IReportWriter with
        member this.WriteLine line = lines <- line :: lines

    // Lines were added to the front, so they're reversed before writing.
    member this.Save() = File.WriteAllLines(path, List.rev lines)

// Grouped so the report's inputs are named instead of being two loose numbers.
type ForecastSettings = { TargetBalance : decimal; MaxMonths : int }

module Report =

    let private kindName (kind : AccountKind) =
        match kind with
        | Checking _ -> "Checking"
        | Savings _ -> "Savings"

    let private describeAction (action : TransactionAction) =
        match action with
        | Deposit amount -> sprintf "Deposit %.2f" amount
        | Withdrawal amount -> sprintf "Withdrawal %.2f" amount
        | Transfer(target, amount) -> sprintf "Transfer %.2f to %s" amount target

    let private describeReason (reason : RejectionReason) =
        match reason with
        | UnknownAccount id -> sprintf "unknown account %s" id
        | SameAccountTransfer -> "cannot transfer to the same account"
        | InsufficientFunds(available, requested) ->
            sprintf "insufficient funds (available %.2f, requested %.2f)" available requested

    let private describeTransaction (transaction : Transaction) =
        sprintf "%s %s: %s"
            transaction.TxId
            transaction.AccountId
            (describeAction transaction.Action)

    let private describeOutcome (outcome : Outcome) =
        match outcome with
        | Applied transaction -> sprintf "APPLIED  %s" (describeTransaction transaction)
        | Rejected(transaction, _) -> sprintf "REJECTED %s" (describeTransaction transaction)

    // The parameter is the interface, so any writer can be passed in.
    let print (settings : ForecastSettings) (writer : IReportWriter) (snapshot : BankSnapshot) =
        let write (line : string) = writer.WriteLine line

        let section (title : string) =
            write ""
            write (sprintf "== %s ==" title)

        // Prints "(none)" for empty sections so the output shape stays the same.
        let writeAllOrNone (lines : string list) =
            match lines with
            | [] -> write "(none)"
            | _ -> lines |> List.iter write

        write "TINYBANK REPORT"

        section "Summary"
        write (sprintf "Applied:  %i" snapshot.AppliedCount)
        write (sprintf "Rejected: %i" snapshot.RejectedTransactions.Length)
        write (sprintf "Invalid:  %i" snapshot.InvalidRows.Length)

        section "Balances"

        snapshot.AccountBalances
        |> Array.groupBy (fun (account, _) -> kindName account.Kind)
        |> Array.sortBy fst
        |> Array.iter (fun (kind, accounts) ->
            write kind

            accounts
            // Sorting by a tuple gives a tie-break: highest balance first, then
            // id, so the output order is always the same.
            |> Array.sortBy (fun (account, balance) -> -balance, account.Id)
            |> Array.iter (fun (account, balance) ->
                write (sprintf "  %-6s %-12s %10.2f" account.Id account.Owner balance)))

        section "Rejected transactions"

        snapshot.RejectedTransactions
        |> List.map (fun (transaction, reason) ->
            sprintf "%s -> %s" (describeTransaction transaction) (describeReason reason))
        |> writeAllOrNone

        section "Invalid rows"

        snapshot.InvalidRows
        |> List.map (fun (lineNumber, reason) -> sprintf "Line %i: %s" lineNumber reason)
        |> writeAllOrNone

        section "Recent activity (newest first)"
        snapshot.RecentActivity |> List.map describeOutcome |> writeAllOrNone

        section (sprintf "Savings forecast (target %.2f)" settings.TargetBalance)

        // Partial application: the settings are filled in once, leaving a
        // function that only needs a rate and a balance.
        let monthsToTarget = Forecast.monthsToReach settings.MaxMonths settings.TargetBalance

        snapshot.AccountBalances
        // choose = filter + map: keep only savings accounts and unwrap the rate.
        |> Array.choose (fun (account, balance) ->
            match account.Kind with
            | Savings rate -> Some(account, balance, rate)
            | Checking _ -> None)
        |> Array.sortBy (fun (account, _, _) -> account.Id)
        |> Array.map (fun (account, balance, rate) ->
            let result =
                match monthsToTarget rate balance with
                | Some 0 -> "already reached"
                | Some months -> sprintf "%i months" months
                | None -> sprintf "not within %i months" settings.MaxMonths

            sprintf "%-6s %s" account.Id result)
        |> List.ofArray
        |> writeAllOrNone
