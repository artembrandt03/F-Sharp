namespace Weather

module Temperature =

    let toFahrenheit (celsius : float) =
        celsius * 9.0 / 5.0 + 32.0

    let describe (celsius : float) =
        if celsius < 10.0 then "cold"
        elif celsius < 25.0 then "mild"
        else "hot"
