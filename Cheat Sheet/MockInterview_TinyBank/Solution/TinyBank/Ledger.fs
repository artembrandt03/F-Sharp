// LEDGER: the banking rules, and the heart of the program.
//
// Everything here is PURE: same inputs, same outputs, no printing, no files, no
// mutation. That is what makes the rules easy to test and to reason about.

namespace TinyBank

open System.Collections.Generic

module Ledger =

    let opening (accounts : seq<Account>) : Balances =
        accounts
        |> Seq.map (fun account -> account.Id, account.OpeningBalance)
        |> Map.ofSeq

    // Pattern matching keeps the product rule in one obvious place. If a third
    // account kind is added, the compiler warns here until it is handled.
    let availableFunds (account : Account) (balance : decimal) : decimal =
        match account.Kind with
        | Checking overdraftLimit -> balance + overdraftLimit
        | Savings _ -> balance

    // Map.find (which throws) is deliberate. Every directory account is given a
    // balance at the start, so a missing one would be a bug in this program, not
    // bad input. Hiding it behind a default of 0 could silently invent money.
    let private adjust (delta : decimal) (id : AccountId) (balances : Balances) : Balances =
        balances |> Map.add id (Map.find id balances + delta)

    // Returns the new balances AND what happened, as a tuple. A rejected
    // transaction simply returns the original Map. There is nothing to undo,
    // which is a direct benefit of immutability: a transfer can never end up
    // half-applied.
    let apply
        (directory : IReadOnlyDictionary<AccountId, Account>)
        (transaction : Transaction)
        (balances : Balances)
        : Balances * Outcome =

        let applied updated = updated, Applied transaction
        let rejected reason = balances, Rejected(transaction, reason)

        match AccountDirectory.tryFind transaction.AccountId directory with
        | None -> rejected (UnknownAccount transaction.AccountId)
        | Some account ->
            // A small local helper, so withdrawals and transfers share exactly
            // the same funds check instead of two copies that could drift apart.
            let ifAffordable amount updated =
                let available = availableFunds account (Map.find account.Id balances)

                if amount <= available then
                    applied updated
                else
                    rejected (InsufficientFunds(available, amount))

            match transaction.Action with
            | Deposit amount -> balances |> adjust amount account.Id |> applied
            | Withdrawal amount ->
                balances |> adjust -amount account.Id |> ifAffordable amount
            | Transfer(target, _) when target = account.Id -> rejected SameAccountTransfer
            | Transfer(target, amount) ->
                match AccountDirectory.tryFind target directory with
                | None -> rejected (UnknownAccount target)
                | Some _ ->
                    balances
                    |> adjust -amount account.Id
                    |> adjust amount target
                    |> ifAffordable amount
