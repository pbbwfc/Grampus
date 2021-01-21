namespace FsChessPgn

open FsChess
open System.Text
open System.IO

module RegParse = 
    type private State = 
        | Unknown
        | InHeader
        | InMove
        | InComment of int
        | InSingleLineComment
        | InRAV of int
        | InNAG
        | InNum
        | InRes
        | FinishedOK
        | Invalid
        | FinishedInvalid
    
    let rec private NextGameRdr(sr : StreamReader) :UnencodedGame = 
        let nl = System.Environment.NewLine
        let rec proclin st cstr s (gm:UnencodedGame) = 
            if s = "" then 
                match st with
                |InMove ->
                    let mte = MoveTextEntry.Parse(cstr)
                    let ngm = {gm with MoveText=mte::gm.MoveText}
                    Unknown,"",ngm
                |InNAG ->
                    let mte = UnencodedNAGEntry(cstr|>int|>Ng)
                    let ngm = {gm with MoveText=mte::gm.MoveText}
                    Unknown,"",ngm
                |InSingleLineComment ->
                    let mte = UnencodedCommentEntry(cstr)
                    let ngm = {gm with MoveText=mte::gm.MoveText}
                    Unknown,"",ngm
                |InRes ->
                    let bits = cstr.Split([|'{'|])
                    let ngm =
                        if bits.Length=1 then
                            let mte = UnencodedGameEndEntry(cstr|>GameResult.Parse)
                            {gm with MoveText=mte::gm.MoveText}
                        else
                            let mte = UnencodedGameEndEntry(bits.[0].Trim()|>GameResult.Parse)
                            let gm1 = {gm with MoveText=mte::gm.MoveText}
                            let mte1 = UnencodedCommentEntry(bits.[1].Trim([|'}'|]))
                            {gm1 with MoveText=mte1::gm1.MoveText}
                    FinishedOK,"",ngm
                |InComment(_) |InRAV(_) -> st, cstr+nl, gm
                |Unknown |InNum -> st, cstr, gm
                |InHeader |Invalid |FinishedOK |FinishedInvalid -> failwith "Invalid state at end of line"
            else 
                let hd = s.[0]
                let tl = s.[1..]
                match st with
                |InComment(cl) -> 
                    if hd='}' && cl=1 then
                        let mte = UnencodedCommentEntry(cstr)
                        let ngm = {gm with MoveText=mte::gm.MoveText}
                        proclin Unknown "" tl ngm
                    elif hd='}' then
                        proclin (InComment(cl-1)) (cstr+hd.ToString()) tl gm
                    elif hd='{' then
                        proclin (InComment(cl+1)) (cstr+hd.ToString()) tl gm
                    else
                        proclin st (cstr+hd.ToString()) tl gm
                |InSingleLineComment ->
                    proclin st (cstr+hd.ToString()) tl gm
                |InRAV(cl) -> 
                    if hd=')' && cl=1 then
                        let byteArray = Encoding.ASCII.GetBytes(cstr)
                        let stream = new MemoryStream(byteArray)
                        let nsr = new StreamReader(stream)
                        let gmr = NextGameRdr(nsr)
                        let mte = UnencodedRAVEntry(gmr.MoveText)
                        let ngm = {gm with MoveText=mte::gm.MoveText}
                        proclin Unknown "" tl ngm
                    elif hd=')' then
                        proclin (InRAV(cl-1)) (cstr+hd.ToString()) tl gm
                    elif hd='(' then
                        proclin (InRAV(cl+1)) (cstr+hd.ToString()) tl gm
                    else
                        proclin st (cstr+hd.ToString()) tl gm
                |InNAG -> 
                    if hd=' ' then
                        let mte = UnencodedNAGEntry(cstr|>int|>Ng)
                        let ngm = {gm with MoveText=mte::gm.MoveText}
                        proclin Unknown "" tl ngm
                    else
                        proclin st (cstr+hd.ToString()) tl gm
                |InNum -> 
                    if System.Char.IsNumber(hd) || hd = '.' || hd = ' ' //&& tl.Length>0 && tl.StartsWith(".")
                    then
                        proclin st (cstr+hd.ToString()) tl gm
                    elif hd='/'||hd='-' then
                        proclin InRes (cstr+hd.ToString()) tl gm
                    else
                        proclin InMove (cstr+hd.ToString()) tl gm
                |InRes -> 
                    proclin st (cstr+hd.ToString()) tl gm
                |Invalid -> 
                    proclin st cstr tl gm
                |InHeader -> 
                    if hd=']' then
                        let ngm = gm|>GameUnencoded.AddTag cstr
                        proclin Unknown "" tl ngm
                    else
                        proclin st (cstr+hd.ToString()) tl gm
                |InMove -> 
                    if hd=' ' then
                        let mte = MoveTextEntry.Parse(cstr)
                        let ngm = {gm with MoveText=mte::gm.MoveText}
                        proclin Unknown "" tl ngm
                    else
                        proclin st (cstr+hd.ToString()) tl gm
                |FinishedOK |FinishedInvalid -> st, cstr, gm
                |Unknown -> 
                    let st, ns = 
                        match hd with
                        | '[' -> InHeader, s.[1..]
                        | '{' -> InComment(1), s.[1..]
                        | '(' -> InRAV(1), s.[1..]
                        | '$' -> InNAG, s.[1..]
                        | '*' -> InRes, s
                        | ';' -> InSingleLineComment, s.[1..]
                        | c when System.Char.IsNumber(c) || c = '.' -> InNum, s
                        | ' ' -> Unknown, s.[1..]
                        | _ -> InMove, s
                    proclin st cstr ns gm
    
        let rec getgm st cstr (gm:UnencodedGame) = 
            let lin = sr.ReadLine()
            if lin |> isNull then { gm with MoveText = (gm.MoveText |> List.rev) }
            else 
                let nst, ncstr, ngm = proclin st cstr lin gm
                if nst = FinishedOK then { ngm with MoveText = (ngm.MoveText |> List.rev) }
                elif nst = FinishedInvalid then UnencodedGameEMP
                else getgm nst ncstr ngm
    
        try
             let gm = getgm Unknown "" UnencodedGameEMP
             gm
        with
        |ex ->  let msg = ex.Message
                UnencodedGameEMP
    
    let AllGamesRdr(sr : System.IO.StreamReader) = 
        seq { 
            while not sr.EndOfStream do
                let gm = NextGameRdr(sr)
                if gm<>UnencodedGameEMP then
                    yield gm
                
        }
    
    let GameFromString(str : string) =
        let byteArray = Encoding.ASCII.GetBytes(str)
        use stream = new MemoryStream(byteArray)
        let sr = new StreamReader(stream)
        let gm = NextGameRdr(sr)
        gm    

