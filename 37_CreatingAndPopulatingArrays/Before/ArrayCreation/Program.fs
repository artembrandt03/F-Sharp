open System

[<EntryPoint>]
let main argv =

    let isEven x =
        x % 2 = 0

    let numbers = 
        [|
            for i in 0..9 do
                let x = i * i
                if x |> isEven then
                    x //sometimes use yield (moslty not needed cz of implicit yield)
        |]
    printfn "%A" numbers
    0 

