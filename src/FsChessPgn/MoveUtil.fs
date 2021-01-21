namespace FsChessPgn

open FsChess
open System.Text

module MoveUtil = 
    
    let toUci(move : Move) = 
        (move|>Move.From|>Square.Name).ToLower() + (move|>Move.To|>Square.Name).ToLower() 
        + (if move|>Move.Promote <> Piece.EMPTY then (move|>Move.Promote|>Piece.PieceToString).ToLower() else "")
    
    let topMove (board : Brd) (move : Move) = 
        let piece = board.PieceAt.[int(move|>Move.From)]
        let pct = piece|>Piece.ToPieceType
        let fromrank = move|>Move.From|>Square.ToRank
        let fromfile = move|>Move.From|>Square.ToFile
        let pcprom = move|>Move.Promote
        let isprom = pcprom <> Piece.EMPTY
        let ptprom = pcprom|>Piece.ToPieceType
        let sTo = move|>Move.To
        let sFrom = move|>Move.From
    
        let iscap = 
            if (sTo = board.EnPassant && (piece = Piece.WPawn || piece = Piece.BPawn)) then true
            else board.PieceAt.[int(sTo)] <> Piece.EMPTY

        let nbd = board|>Board.MoveApply(move)
        let ischk = nbd|>Board.IsChk
        let ismt = nbd|>MoveGenerate.IsMate

    
        if piece = Piece.WKing && sFrom = E1 && sTo = G1 then 
            pMove.CreateCastle(MoveType.CastleKingSide,"O-O")
        elif piece = Piece.BKing && sFrom = E8 && sTo = G8 then 
            pMove.CreateCastle(MoveType.CastleKingSide,"O-O")
        elif piece = Piece.WKing && sFrom = E1 && sTo = C1 then 
            pMove.CreateCastle(MoveType.CastleQueenSide,"O-O-O")
        elif piece = Piece.BKing && sFrom = E8 && sTo = C8 then 
            pMove.CreateCastle(MoveType.CastleQueenSide,"O-O-O")
        else 
            //do not need this check for pawn moves
            let rec getuniqs pu fu ru attl = 
                if List.isEmpty attl then pu, fu, ru
                else 
                    let att = attl.Head
                    if att = sFrom then getuniqs pu fu ru attl.Tail
                    else 
                        let otherpiece = board.PieceAt.[int(att)]
                        if otherpiece = piece then 
                            let npu = false
                            let nru = 
                                if (att|>Square.ToRank) = fromrank then false
                                else ru
                        
                            let nfu = 
                                if (att|>Square.ToFile) = fromfile then false
                                else fu
                        
                            getuniqs npu nfu nru attl.Tail
                        else getuniqs pu fu ru attl.Tail
        
            let pu, fu, ru = 
                if ((piece=Piece.WPawn)||(piece=Piece.BPawn)) then
                    if iscap then false,true,false else true,true,true
                else getuniqs true true true ((board|>Board.AttacksTo sTo (piece|>Piece.PieceToPlayer))|>Bitboard.ToSquares)

            let uf,ur =
                if pu then None,None
                else
                    if fu then Some(fromfile), None
                    elif ru then None,Some(fromrank)
                    else Some(fromfile),Some(fromrank)
            let mt = if iscap then MoveType.Capture else MoveType.Simple
            let pmv0 = pMove.CreateAll(mt,sTo,Some(pct),uf,ur,(if isprom then Some(ptprom) else None),ischk,false,ismt,"")
            let san = pmv0|>PgnWrite.MoveStr
            {pmv0 with San=san}

    let toPgn (board : Brd) (move : Move) = 
        let pmv = move|>topMove board
        let pgn = pmv|>PgnWrite.MoveStr
        pgn
    
    let Descs moves (board : Brd) isVariation = 
        let sb = new StringBuilder()
        let rec getsb mvl ibd =
            if List.isEmpty mvl then ibd
            else
                let mv = mvl.Head
                if isVariation && ibd.WhosTurn = Player.White then sb.Append(ibd.Fullmove.ToString() + ". ") |> ignore
                sb.Append((toPgn ibd mv) + " ") |> ignore
                if isVariation then getsb mvl.Tail (ibd|>Board.MoveApply mv)
                else getsb mvl.Tail ibd
        board|>getsb moves|>ignore
        sb.ToString()

    let FindMv bd uci =
        let mvs = MoveGenerate.AllMoves bd
        let fmvs = mvs|>List.filter(fun m -> m|>toUci=uci)
        if fmvs.Length=1 then Some(fmvs.Head) else None

    let fromUci bd uci = (FindMv bd uci).Value

    ///Get an encoded move from a SAN Move(move) such as Nf3 for this Board(bd)
    let fromSAN (bd : Brd) (move : string) = 
        let pmv = move|>pMove.Parse
        let mv = pmv|>pMove.ToMove bd
        mv

    ///Make a SAN Move(move) such as Nf3 for this Board(bd) and return the new Board
    let ApplySAN (move : string) (bd : Brd) = 
        let mv = move|>fromSAN bd
        bd|>Board.MoveApply mv

    let UcisToSans (bd:Brd) (ucis:string) =
        let rec getsan cbd (ucil:string list) sanl =
            if ucil.IsEmpty then sanl|>List.rev|>List.reduce(fun a b -> a + " " + b)
            else
                let uci = ucil.Head
                if uci.Length=4 then
                    let sqf = uci.Substring(0,2)|>Square.Parse
                    let sqt = uci.Substring(2,2)|>Square.Parse
                    let psmvs = sqf|>MoveGenerate.PossMoves cbd
                    let mvl = psmvs|>List.filter(fun m ->m|>Move.To=sqt)
                    let mv = mvl.Head
                    let san = mv|>toPgn cbd
                    let nbd = cbd|>Board.MoveApply mv
                    let nsanl = san::sanl
                    getsan nbd ucil.Tail nsanl
                else
                    let sqf = uci.Substring(0,2)|>Square.Parse
                    let sqt = uci.Substring(2,2)|>Square.Parse
                    let ppc = uci.[4]|>PieceType.Parse
                    let psmvs = sqf|>MoveGenerate.PossMoves cbd
                    let mvl = psmvs|>List.filter(fun m ->m|>Move.To=sqt)
                    let nmvl = mvl|>List.filter(fun mv -> mv|>Move.PromoteType=ppc)
                    let mv = nmvl.Head
                    let san = mv|>toPgn cbd
                    let nbd = cbd|>Board.MoveApply mv
                    let nsanl = san::sanl
                    getsan nbd ucil.Tail nsanl
        let ucil = ucis.Trim().Split([|' '|])|>List.ofArray
        let sanstr = getsan bd ucil []
        sanstr
