// ACCOUNT DIRECTORY: the fixed list of accounts, loaded once at startup.

namespace TinyBank

open System
open System.IO
open System.Collections.Generic

module AccountDirectory =

    // WHY readOnlyDict HERE BUT Map FOR BALANCES?
    //   Accounts never change after loading and are looked up for every
    //   transaction, so a read-only hash table is the right fit: fast, and
    //   nobody can modify it.
    //   Balances change constantly, and Map gives us a new immutable version
    //   on every change.
    // The return type is an INTERFACE, so callers depend on the ability to
    // look things up, not on one concrete class.
    let ofAccounts (accounts : Account[]) : IReadOnlyDictionary<AccountId, Account> =
        // readOnlyDict would silently keep only one of two duplicate ids, which
        // would hide money. So we look for duplicates first.
        let duplicates =
            accounts
            |> Array.countBy (fun account -> account.Id)
            |> Array.filter (fun (_, count) -> count > 1)
            |> Array.map fst

        if duplicates.Length > 0 then
            let ids = String.Join(", ", duplicates)
            raise (FormatException(sprintf "Duplicate account ids: %s" ids))

        accounts
        |> Array.map (fun account -> account.Id, account)
        |> readOnlyDict

    // ReadAllLines gives an ARRAY: this file is small and we want all of it
    // validated before processing starts. Compare with the lazy read of the
    // possibly huge transactions file in Program.fs.
    let load (path : string) : IReadOnlyDictionary<AccountId, Account> =
        let rows = File.ReadAllLines path

        if rows.Length = 0 then
            raise (FormatException "Accounts file is empty")

        rows
        // mapi comes BEFORE skip and filter so that line numbers still match
        // what you'd see when opening the file in an editor.
        |> Array.mapi (fun index row -> index + 1, row)
        |> Array.skip 1
        |> Array.filter (fun (_, row) -> not (String.IsNullOrWhiteSpace row))
        |> Array.map (fun (lineNumber, row) -> Parse.account lineNumber row)
        |> ofAccounts

    // Wraps the .NET lookup so the rest of the program gets an option, and so
    // a missing key can't throw as `directory.[id]` would.
    let tryFind (id : AccountId) (directory : IReadOnlyDictionary<AccountId, Account>) =
        match directory.TryGetValue id with
        | true, account -> Some account
        | false, _ -> None
