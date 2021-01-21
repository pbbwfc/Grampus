namespace FsChessPgn

open FsChess

module GameUnencoded =

    let Start = UnencodedGameEMP

    let MoveCount(mtel:UnencodedMoveTextEntry list) =
        let mc(mte:UnencodedMoveTextEntry) =
            match mte with
            |UnencodedHalfMoveEntry(_) -> 1
            |_ -> 0
        if mtel.IsEmpty then 0
        else
            mtel|>List.map mc|>List.reduce(+)
        
    let FullMoveCount(mtel:UnencodedMoveTextEntry list) = MoveCount(mtel)/2

    let GetMoves(mtel:UnencodedMoveTextEntry list) =
        let gm(mte:UnencodedMoveTextEntry) =
            match mte with
            |UnencodedHalfMoveEntry(_,_,mv) -> [mv]
            |_ -> []
        mtel|>List.map gm|>List.concat
    
    let AddTag (tagstr:string) (gm:UnencodedGame) =
        let k,v = tagstr.Trim().Split([|'"'|])|>Array.map(fun s -> s.Trim())|>fun a -> a.[0],a.[1].Trim('"')
        match k with
        | "Event" -> {gm with Event = v}
        | "Site" -> {gm with Site = v}
        | "Date" -> 
            let yop,mop,dop = v|>DateUtil.FromStr
            {gm with Year = yop; Month = mop; Day = dop}
        | "Round" -> {gm with Round = v}
        | "White" -> {gm with WhitePlayer = v}
        | "Black" -> {gm with BlackPlayer = v}
        | "Result" -> {gm with Result = v|>GameResult.Parse}
        | "WhiteElo" -> {gm with WhiteElo = v}
        | "BlackElo" -> {gm with BlackElo = v}
        | "ECO" -> {gm with ECO = v}
        | "FEN" -> {gm with BoardSetup = v|>FEN.Parse|>Board.FromFEN|>Some}
        | _ ->
            {gm with AdditionalInfo=gm.AdditionalInfo.Add(k,v)}
    
    let AddMoveEntry (mte:UnencodedMoveTextEntry) (gm:UnencodedGame) =
        {gm with MoveText=gm.MoveText@[mte]}

    let RemoveMoveEntry (gm:UnencodedGame) =
        let mtel = gm.MoveText
        let nmtel =
            if mtel.IsEmpty then mtel
            else
                mtel|>List.rev|>List.tail|>List.rev
        {gm with MoveText=nmtel}

    let AddpMove (pmv:pMove) (gm:UnencodedGame) =
        let mtel = gm.MoveText
        let mc = mtel|>MoveCount
        let mn = if mc%2=0 then Some(mc/2+1) else None
        let mte = UnencodedHalfMoveEntry(mn,false,pmv)
        gm|>AddMoveEntry mte
            
    let AddSan (san:string) (gm:UnencodedGame) =
        let pmv = san|>pMove.Parse
        gm|>AddpMove pmv
     
    let pretty(gm:UnencodedGame) = 
        let mtel = gm.MoveText
        if mtel.IsEmpty then "No moves"
        elif mtel.Length<6 then
            let mvstr =mtel|>List.map PgnWrite.MoveTextEntryStr|>List.reduce(fun a b -> a + " " + b)
            "moves: " + mvstr
        else
            let rl = mtel|>List.rev
            let l5 = rl.[0..4]|>List.rev
            let mvstr = l5|>List.map PgnWrite.MoveTextEntryStr|>List.reduce(fun a b -> a + " " + b)
            "moves: ..." + mvstr


