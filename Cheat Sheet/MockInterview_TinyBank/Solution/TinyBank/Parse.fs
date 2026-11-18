// PARSE: turns text into domain values. It never reads files itself, so every
// function here can be tested by passing it a plain string.

namespace TinyBank

open System
open System.Globalization

module Parse =

    // Returns an option because "not a number" is a normal outcome for input.
    //
    // WHY TryParse and not the `decimal` conversion function? `decimal "abc"`
    // throws. TryParse reports failure as a value. F# turns its .NET "out"
    // parameter into a returned tuple: (succeeded, value).
    //
    // WHY InvariantCulture: on a PC set to some European regions, "12.50" would
    // not be read as twelve and a half. A data file must mean the same thing on
    // every machine. `|||` combines the two allowed-format flags.
    let tryDecimal (text : string) : decimal option =
        let styles = NumberStyles.AllowLeadingSign ||| NumberStyles.AllowDecimalPoint
        match Decimal.TryParse(text.Trim(), styles, CultureInfo.InvariantCulture) with
        | true, value -> Some value
        | false, _ -> None

    // `private`: a detail of this module, so nothing else can depend on it.
    let private splitColumns (row : string) : string[] =
        row.Split('\t') |> Array.map (fun column -> column.Trim())

    // An array pattern checks the column count and names the columns in one
    // step, so there is no `columns.[4]` that could go out of range.
    let transaction (lineNumber : int) (row : string) : Parsed<Transaction> =
        let invalid reason = Invalid(lineNumber, reason)

        let build txId accountId actionName amountText target =
            let valid action =
                Valid { TxId = txId; AccountId = accountId; Action = action }

            match tryDecimal amountText with
            | None -> invalid (sprintf "'%s' is not a valid amount" amountText)
            // `when` adds an extra condition to a pattern.
            | Some amount when amount <= 0m -> invalid "amount must be positive"
            | Some amount ->
                // Matching on a tuple keeps every action/target combination
                // visible in one table, which is easier to check than nested ifs.
                match actionName, target with
                | "Deposit", None -> valid (Deposit amount)
                | "Withdrawal", None -> valid (Withdrawal amount)
                | "Transfer", Some toAccount when toAccount <> "" ->
                    valid (Transfer(toAccount, amount))
                | "Transfer", _ -> invalid "Transfer needs a target account"
                | ("Deposit" | "Withdrawal"), Some _ ->
                    invalid (sprintf "%s must not have a target account" actionName)
                | other, _ -> invalid (sprintf "unknown transaction type '%s'" other)

        match splitColumns row with
        | [| txId; actionName; accountId; amount |] ->
            build txId accountId actionName amount None
        | [| txId; actionName; accountId; amount; target |] ->
            build txId accountId actionName amount (Some target)
        | columns ->
            invalid (sprintf "expected 4 or 5 columns but found %i" columns.Length)

    // WHY THIS RAISES while `transaction` does not: the accounts file is
    // reference data. If it's wrong, every result built on it would be wrong, so
    // we stop. One bad transaction only affects that one transaction.
    let account (lineNumber : int) (row : string) : Account =
        let fail reason =
            raise (FormatException(sprintf "Accounts line %i: %s" lineNumber reason))

        let requireDecimal name text =
            match tryDecimal text with
            | Some value -> value
            | None -> fail (sprintf "%s '%s' is not a number" name text)

        match splitColumns row with
        | [| id; owner; kindName; setting; opening |] ->
            let settingValue = requireDecimal "setting" setting

            if settingValue < 0m then
                fail "setting cannot be negative"

            let kind =
                match kindName with
                | "Checking" -> Checking settingValue
                | "Savings" -> Savings settingValue
                | other -> fail (sprintf "unknown account kind '%s'" other)

            {
                Id = id
                Owner = owner
                Kind = kind
                OpeningBalance = requireDecimal "opening balance" opening
            }
        | columns -> fail (sprintf "expected 5 columns but found %i" columns.Length)
