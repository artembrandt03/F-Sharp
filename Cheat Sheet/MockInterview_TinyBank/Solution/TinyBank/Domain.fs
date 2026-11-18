// DOMAIN: the vocabulary of the program. Types only, no logic.
//
// WHY START HERE: in an F# interview, write the types first and talk through
// them. They are quick to write, they show you understood the problem, and the
// compiler then guides the rest of the solution.

namespace TinyBank

// A type ALIAS: just another name for string. It makes signatures readable
// (`AccountId -> Balances`), but the compiler still treats it as a string.
// TRADEOFF: a single-case union (`type AccountId = AccountId of string`) would
// stop an owner name being passed by mistake. For a program this small, the
// alias is enough. It's a good improvement to mention if asked.
type AccountId = string

// A DISCRIMINATED UNION, because an account is exactly ONE of these.
// WHY NOT a record with OverdraftLimit and MonthlyRate fields? Then a savings
// account could hold an overdraft limit, which means nothing. With this union,
// that illegal combination cannot even be written.
//
// WHY decimal, NOT float: float is binary, so 0.1 + 0.2 is not exactly 0.3.
// decimal stores base-10 digits exactly, which is what money needs. The `m`
// suffix makes a decimal literal: 10.50m.
type AccountKind =
    | Checking of overdraftLimit : decimal
    | Savings of monthlyRatePercent : decimal

// A RECORD, because an account has ALL of these fields at once.
// Immutable: this describes the account as it was opened. The changing balance
// lives separately in the ledger.
type Account =
    {
        Id : AccountId
        Owner : string
        Kind : AccountKind
        OpeningBalance : decimal
    }

// Each action carries only the data it needs. A Deposit cannot have a target
// account, so no code ever has to check for a meaningless empty target.
type TransactionAction =
    | Deposit of amount : decimal
    | Withdrawal of amount : decimal
    | Transfer of toAccount : AccountId * amount : decimal

// WHY TxId and AccountId, not Id and Account? Account already has `Id` and
// `Kind`. When two records share field names, F# guesses the most recent type,
// and `fun a -> a.Id` can suddenly mean the wrong one. Unique names avoid that.
type Transaction =
    {
        TxId : string
        AccountId : AccountId
        Action : TransactionAction
    }

// A GENERIC union: 'T is a placeholder, so the same "worked or failed" wrapper
// could hold any parsed type.
//
// WHY NOT option? None could only say "failed". We must report WHICH line
// failed and WHY.
// WHY NOT an exception? A bad row is expected in this file and processing must
// continue. Exceptions are for situations we cannot carry on from.
// INTERVIEW NOTE: F# has a built-in Result<'T,'Error> for exactly this. The
// course didn't cover it, so this is written by hand, but say that you know it.
type Parsed<'T> =
    | Valid of 'T
    | Invalid of lineNumber : int * reason : string

// Why a well-formed transaction was refused. It's a union rather than a string
// so each reason keeps its own data, and the report decides the wording.
type RejectionReason =
    | UnknownAccount of AccountId
    | SameAccountTransfer
    | InsufficientFunds of available : decimal * requested : decimal

type Outcome =
    | Applied of Transaction
    | Rejected of Transaction * RejectionReason

// A Map, because balances CHANGE as transactions arrive. Each update produces a
// new immutable Map, so a rule can never half-change the existing balances.
type Balances = Map<AccountId, decimal>

// An immutable picture of the bank at one moment. The report receives this
// instead of the live Bank object, so printing cannot change any state.
type BankSnapshot =
    {
        AccountBalances : (Account * decimal)[]
        AppliedCount : int
        RejectedTransactions : (Transaction * RejectionReason) list
        InvalidRows : (int * string) list
        RecentActivity : Outcome list
    }
