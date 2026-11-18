namespace Classes

open System

type ConsolePrompt(message: string, maxTries: int) = 
    do
        if String.IsNullOrWhiteSpace(message) then
            raise <| ArgumentException("Null or empty", "message")
    let trimmedMessage = message.Trim()
    let mutable tryCount = 0

    member this.GetValue() =
        tryCount <- tryCount + 1 
        printf "%s: " trimmedMessage
        let input = Console.ReadLine()
        if String.IsNullOrWhiteSpace(input) && tryCount < maxTries then
            if this.BeepOnError then
                Console.Beep()
            this.GetValue()
        else
            input

    member val BeepOnError = true
        with get, set