namespace TinyBank

type AccountId = string

type AccountKind =
    | Checking of overdraftLimit : decimal
    | Savings of monthlyRatePercent : decimal

type Account = 
    {
        Id: AccountId
        Owner: string
        Kind: AccountKind
        OpeningBalance: decimal
    }

type TransactionAction =
    | Deposit of amount : decimal
    | Withdrawal of amount : decimal
    | Transfer of toAccount : AccountId * amount : decimal

type Transaction =
    {
        TxId : string
        AccountId : AccountId
        Action : TransactionAction
    }