namespace TinyBank

open System
open System.IO
open System.Collections.Generic

module AccountDirecotry = 

    let ofAccounts (accounts: Account[]) : IReadOnlyDictionary<AccountId, Account> = 

        let duplicates = 
            accounts
            |> Array.countBy (fun account -> account.Id)
            |> Array.filter (fun (_, count) -> count > 1)
            |> Array.map fst
        
        if duplicates.Length > 0 then
            let ids = String.Join(",", duplicates)
            raise (FormatException(sprintf "Duplicate account ids: %s" ids))
        
        accounts
        |> Array.map (fun account -> account.Id, account)
        |> readOnlyDict
    
    let load (path: string) : IReadOnlyDictionary<AccountId, Account>
        let rows = File.ReadAllLines path

        if rows.Length = 0 then
            raise (FormatException "Account file is empty")
        
        rows
        |> Array.mapi (fun index row -> index + 1, row)
        |> Array.skip
        |> Array.filter (fun (_, row) -> not (String.IsNullOrWhiteSpace row))
        |> Array.map (fun (lineNumber, row) -> Parse.account lineNumber row)
        |> ofAccounts
    
    // Wraps the .NET lookup so the rest of the program gets an option, and so
    // a missing key can't throw as `directory.[id]` would.
    let tryFind (id : AccountId) (directory : IReadOnlyDictionary<AccountId, Account>) =
        match directory.TryGetValue id with
        | true, account -> Some account
        | false, _ -> None
        