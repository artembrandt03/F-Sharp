module Report =

    let line (celsius : float) =
        sprintf "%0.1fC = %0.1fF (%s)" celsius (Temperature.toFahrenheit celsius) (Temperature.describe celsius)
