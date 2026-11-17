namespace GenericPoints
 
type Point<'T> = 
    {
        X : 'T
        Y : 'T
    }
 
module Point =
 
    let inline moveBy (dx : 'T) (dy : 'T) (p : Point<'T>) =
        {
            X = p.X + dx
            Y = p.Y + dy
        }

// Claude Code: for future final cheat sheet, explain function inlining, what it does and how it works        