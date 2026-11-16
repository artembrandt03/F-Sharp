namespace Weather   // FIX 2: GOOD. Same namespace as Temperature.fs, so no `open` needed here.

module Report =

    let line (celsius : float) =
        sprintf "%0.1fC = %0.1fF (%s)" celsius (Temperature.toFahrenheit celsius) (Temperature.describe celsius)
