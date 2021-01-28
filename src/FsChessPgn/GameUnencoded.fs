namespace FsChessPgn

open FsChess

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
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
        | "Event" -> 
            let hdr = {gm.Hdr with Event = v}
            {gm with Hdr = hdr}
        | "Site" -> {gm with Site = v}
        | "Date" -> 
            let yop,mop,dop = v|>DateUtil.FromStr
            let hdr = {gm.Hdr with Year = if yop.IsNone then 0 else yop.Value}
            {gm with Month = mop; Day = dop; Hdr = hdr}
        | "Round" -> {gm with Round = v}
        | "White" -> 
            let hdr = {gm.Hdr with White = v}
            {gm with Hdr = hdr}
        | "Black" -> 
            let hdr = {gm.Hdr with Black = v}
            {gm with Hdr = hdr}
        | "Result" -> 
            let hdr = {gm.Hdr with Result = v|>GameResult.Parse|>GameResult.ToStr}
            {gm with Hdr = hdr}
        | "WhiteElo" -> 
            let hdr = {gm.Hdr with W_Elo = v}
            {gm with Hdr = hdr}
        | "BlackElo" -> 
            let hdr = {gm.Hdr with B_Elo = v}
            {gm with Hdr = hdr}
        | "ECO" -> 
            let hdr = {gm.Hdr with ECO = v}
            {gm with Hdr = hdr}
        | "Opening" ->
            let hdr = {gm.Hdr with Opening = v}
            {gm with Hdr = hdr}
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


