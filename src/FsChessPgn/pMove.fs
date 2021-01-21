namespace FsChessPgn

open FsChess
open System.Text.RegularExpressions

module pMove =

    let CreateAll(mt,tgs,pc,orf,orr,pp,ic,id,im,san) =
        {Mtype=mt 
         TargetSquare=tgs
         Piece=pc
         OriginFile=orf
         OriginRank=orr
         PromotedPiece=pp
         IsCheck=ic
         IsDoubleCheck=id
         IsCheckMate=im
         San=san}

    let CreateOrig(mt,tgs,pc,orf,orr,san) = CreateAll(mt,tgs,pc,orf,orr,None,false,false,false,san)

    let Create(mt,tgs,pc,san) = CreateOrig(mt,tgs,pc,None,None,san)

    let CreateCastle(mt,san) = CreateOrig(mt,OUTOFBOUNDS,None,None,None,san)
    
    let Parse(s : string) =
        //Active pattern to parse move string
        let (|SimpleMove|Castle|PawnCapture|AmbiguousFile|AmbiguousRank|Promotion|PromCapture|) s =
            if Regex.IsMatch(s, "^[BNRQK][a-h][1-8]$") then 
                SimpleMove(s.[0]|>PieceType.Parse, s.[1..]|>Square.Parse)
            elif Regex.IsMatch(s, "^[a-h][1-8]$") then SimpleMove(PieceType.Pawn, s|>Square.Parse)
            elif s = "O-O" then Castle('K')
            elif s = "O-O-O" then Castle('Q')
            elif Regex.IsMatch(s, "^[a-h][a-h][1-8]$") then 
                PawnCapture(s.[0]|>File.Parse, s.[1..]|>Square.Parse)
            elif Regex.IsMatch(s, "^[BNRQK][a-h][a-h][1-8]$") then 
                AmbiguousFile(s.[0]|>PieceType.Parse, s.[1]|>File.Parse, s.[2..]|>Square.Parse)
            elif Regex.IsMatch(s, "^[BNRQK][1-8][a-h][1-8]$") then 
                AmbiguousRank(s.[0]|>PieceType.Parse, s.[1]|>Rank.Parse, s.[2..]|>Square.Parse)
            elif Regex.IsMatch(s, "^[a-h][1-8][BNRQ]$") then 
                Promotion(s.[0..1]|>Square.Parse, s.[2]|>PieceType.Parse)
            elif Regex.IsMatch(s, "^[a-h][a-h][1-8][BNRQ]$") then 
                PromCapture(s.[0]|>File.Parse, s.[1..2]|>Square.Parse, s.[3]|>PieceType.Parse)
            else failwith ("invalid move: " + s)

        //general failure message
        let fl() =
            failwith ("not done yet, mv: " + s)

        let strip chars =
            String.collect (fun c -> 
                if Seq.exists ((=) c) chars then ""
                else string c)
          
        let m = s |> strip "+x#="|>fun x ->x.Replace("e.p.", "")
        
        let mv0 =
            match m with
            | SimpleMove(p, sq) -> 
                Create((if s.Contains("x") then MoveType.Capture else MoveType.Simple),sq,Some(p),s)
            | Castle(c) ->
                CreateCastle(if c='K' then MoveType.CastleKingSide,s else MoveType.CastleQueenSide,s)
            | PawnCapture(f, sq) -> 
                CreateOrig(MoveType.Capture,sq,Some(PieceType.Pawn),Some(f),None,s)
            | AmbiguousFile(p, f, sq) -> 
                CreateOrig((if s.Contains("x") then MoveType.Capture else MoveType.Simple),sq,Some(p),Some(f),None,s)
            | AmbiguousRank(p, r, sq) -> 
                CreateOrig((if s.Contains("x") then MoveType.Capture else MoveType.Simple),sq,Some(p),None,Some(r),s)
            | Promotion(sq, p) -> 
                CreateAll(MoveType.Simple,sq,Some(PieceType.Pawn),None,None,Some(p),false,false,false,s)
            | PromCapture(f, sq, p) -> 
                CreateAll(MoveType.Capture,sq,Some(PieceType.Pawn),Some(f),None,Some(p),false,false,false,s)
      
        let mv1 =
            if s.Contains("++") then {mv0 with IsDoubleCheck=true} 
            elif s.Contains("+") then {mv0 with IsCheck=true}
            elif s.Contains("#") then {mv0 with IsCheckMate=true}
            else mv0
        
        mv1
    
    let ToMove (bd:Brd) (pmv:pMove) =
        if pmv.Mtype=MoveType.CastleKingSide then
            let mvs = 
                bd|>MoveGenerate.CastleMoves
                |>List.filter(fun mv -> FileG=(mv|>Move.To|>Square.ToFile))
            if mvs.Length=1 then mvs.Head
            else
                failwith "kc"
        elif pmv.Mtype=MoveType.CastleQueenSide then
            let mvs = 
                bd|>MoveGenerate.CastleMoves
                |>List.filter(fun mv -> FileC=(mv|>Move.To|>Square.ToFile))
            if mvs.Length=1 then mvs.Head
            else
                failwith "qc"
        elif pmv.Piece.IsNone then failwith "none"
        else
            match pmv.Piece.Value with
            |PieceType.Pawn ->
                let mvs = 
                    if pmv.PromotedPiece.IsSome then
                        bd|>MoveGenerate.PawnMoves
                        |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                        |>List.filter(fun mv -> pmv.PromotedPiece.Value=(mv|>Move.PromoteType))
                    else
                        bd|>MoveGenerate.PawnMoves
                        |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                if mvs.Length=1 then mvs.Head
                elif pmv.OriginFile.IsSome then
                    let mvs1=mvs|>List.filter(fun mv -> pmv.OriginFile.Value=(mv|>Move.From|>Square.ToFile))
                    if mvs1.Length=1 then mvs1.Head else failwith ("pf " + (pmv|>PgnWrite.MoveStr))
                else
                    failwith ("p " + (pmv|>PgnWrite.MoveStr))
            |PieceType.Knight ->
                let mvs = 
                    bd|>MoveGenerate.KnightMoves
                    |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                if mvs.Length=1 then mvs.Head
                elif pmv.OriginFile.IsSome then
                    let mvs1=mvs|>List.filter(fun mv -> pmv.OriginFile.Value=(mv|>Move.From|>Square.ToFile))
                    if mvs1.Length=1 then mvs1.Head else failwith "nf"
                elif pmv.OriginRank.IsSome then
                    let mvs1=mvs|>List.filter(fun mv -> pmv.OriginRank.Value=(mv|>Move.From|>Square.ToRank))
                    if mvs1.Length=1 then mvs1.Head else failwith "nr"
                else
                    failwith "n"
            |PieceType.Bishop ->
                let mvs = 
                    bd|>MoveGenerate.BishopMoves
                    |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                if mvs.Length=1 then mvs.Head
                elif pmv.OriginFile.IsSome then
                    let mvs1=mvs|>List.filter(fun mv -> pmv.OriginFile.Value=(mv|>Move.From|>Square.ToFile))
                    if mvs1.Length=1 then mvs1.Head else failwith "bf"
                elif pmv.OriginRank.IsSome then
                    let mvs1=mvs|>List.filter(fun mv -> pmv.OriginRank.Value=(mv|>Move.From|>Square.ToRank))
                    if mvs1.Length=1 then mvs1.Head else failwith "br"
                else
                    failwith "b"
            |PieceType.Rook ->
                let mvs = 
                    bd|>MoveGenerate.RookMoves
                    |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                if mvs.Length=1 then mvs.Head
                elif pmv.OriginFile.IsSome then
                    let mvs1=mvs|>List.filter(fun mv -> pmv.OriginFile.Value=(mv|>Move.From|>Square.ToFile))
                    if mvs1.Length=1 then mvs1.Head else failwith "rf"
                elif pmv.OriginRank.IsSome then
                    let mvs1=mvs|>List.filter(fun mv -> pmv.OriginRank.Value=(mv|>Move.From|>Square.ToRank))
                    if mvs1.Length=1 then mvs1.Head else failwith "rr"
                else
                    failwith "r"
            |PieceType.Queen ->
                let mvs = 
                    bd|>MoveGenerate.QueenMoves
                    |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                if mvs.Length=1 then mvs.Head
                elif pmv.OriginFile.IsSome then
                    let mvs1=mvs|>List.filter(fun mv -> pmv.OriginFile.Value=(mv|>Move.From|>Square.ToFile))
                    if mvs1.Length=1 then mvs1.Head else failwith "qf"
                elif pmv.OriginRank.IsSome then
                    let mvs1=mvs|>List.filter(fun mv -> pmv.OriginRank.Value=(mv|>Move.From|>Square.ToRank))
                    if mvs1.Length=1 then mvs1.Head else failwith "qr"
                else
                    failwith "q"
            |PieceType.King ->
                let mvs = 
                    bd|>MoveGenerate.KingMoves
                    |>List.filter(fun mv -> pmv.TargetSquare=(mv|>Move.To))
                if mvs.Length=1 then mvs.Head
                else
                    failwith "k"
            |_ -> failwith "all"

    let Encode (bd:Brd) mno (pmv:pMove) =
        let mv = pmv|>ToMove bd

        {
            San = pmv.San
            Mno = mno
            Isw = bd.WhosTurn=Player.White
            Mv = mv
            PostBrd = bd|>Board.MoveApply mv
        }
