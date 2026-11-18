// FORECAST: how long until a savings account reaches a target?

namespace TinyBank

open System

module Forecast =

    // An INFINITE lazy sequence: this month's balance, next month's, and so on.
    //
    // WHY Seq.unfold: each value depends on the PREVIOUS value, and that is what
    // unfold's "state" is for. Seq.initInfinite suits values based only on their
    // position. An array is impossible here because we don't know the length.
    //
    // The function returns Some(value to hand out, state for next time).
    // It never returns None, so the sequence never ends by itself.
    let monthlyBalances (monthlyRatePercent : decimal) (start : decimal) : seq<decimal> =
        start
        |> Seq.unfold (fun balance ->
            let interest = Math.Round(balance * monthlyRatePercent / 100m, 2)
            Some(balance, balance + interest))

    // Laziness does the real work here. tryFindIndex stops pulling values at the
    // first match, so only the months we need are ever calculated.
    // truncate is the safety limit: without it, an account that never reaches
    // the target would loop forever.
    //
    // Returns an option because "never within the limit" is a normal answer.
    // Settings come first, so Reporting can partially apply them.
    let monthsToReach
        (maxMonths : int)
        (target : decimal)
        (monthlyRatePercent : decimal)
        (start : decimal)
        : int option =
        // A zero or negative balance never grows towards a positive target.
        // Stopping here also prevents a negative balance from compounding until
        // it overflows decimal at very high rates.
        if start < target && start <= 0m then
            None
        else
            monthlyBalances monthlyRatePercent start
            |> Seq.truncate (maxMonths + 1) // +1 because index 0 is "today"
            |> Seq.tryFindIndex (fun balance -> balance >= target)
