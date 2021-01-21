namespace FsChessPgn

open FsChess

module MoveTextEntry =

    let Parse(s : string) =
        let mn =
            if System.Char.IsNumber(s.[0]) then
                let bits = s.Split([|'.'|])
                bits.[0]|>int|>Some
            else None
        
        let ic = s.Contains("...") 

        let mv =
            let bits = s.Trim().Split([|' ';'.'|])
            let mvtxt = bits.[bits.Length-1].Trim()
            pMove.Parse(mvtxt)

        UnencodedHalfMoveEntry(mn,ic,mv)
    
