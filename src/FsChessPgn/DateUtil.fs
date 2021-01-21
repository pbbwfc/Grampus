namespace FsChessPgn

open FsChess

module DateUtil = 
    
    let (|?) (lhs:int option) rhs = (if lhs.IsNone then rhs else lhs.Value.ToString("00"))
    let (-?) (lhs:int option) rhs = (if lhs.IsNone then rhs else lhs.Value.ToString("0000"))
    
    let ToStr(gm:UnencodedGame) =
        (gm.Year -? "????") + (".") +
        (gm.Month |? "??") + (".") +
        (gm.Day |? "??")

    let ToStr2(gm:EncodedGame) =
        (gm.Year -? "????") + (".") +
        (gm.Month |? "??") + (".") +
        (gm.Day |? "??")
    
    let FromStr(dtstr:string) =
        let a = dtstr.Split([|'.'|])|>Array.map(fun s -> s.Trim())
        let y,m,d = if a.Length=3 then a.[0],a.[1],a.[2] else a.[0],"??","??"
        let yop = if y="????" then None else Some(int y)
        let mop = if m="??" then None else Some(int m)
        let dop = if d="??" then None else Some(int d)
        yop,mop,dop
