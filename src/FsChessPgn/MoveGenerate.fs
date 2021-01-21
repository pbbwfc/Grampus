namespace FsChessPgn

open FsChess

module MoveGenerate = 
    
    let private legal (bd: Brd) (mvs:Move list) =
        let me = bd.WhosTurn

        let rec filt (imvl:Move list) omvl = 
            if imvl.IsEmpty then omvl|>List.rev
            else
                let mv = imvl.Head
                let nbd = bd |> Board.MoveApply(mv)
                let inchk:bool = (nbd |> Board.IsChck me)
                if inchk then filt imvl.Tail omvl
                else filt imvl.Tail (mv::omvl)

        let lmvs = filt mvs []
        lmvs

    let KingMoves (bd : Brd) :Move list = 
        let me = bd.WhosTurn
    
        let targetLocations = 
            (if me=Player.Black then bd.WtPrBds else bd.BkPrBds)|||(~~~bd.PieceLocationsAll)
    
        let kingPos = if me=Player.White then bd.WtKingPos else bd.BkKingPos
    
        let rec getKingAttacks att mvl = 
            if att = Bitboard.Empty then mvl
            else 
                let attPos, natt = Bitboard.PopFirst(att)
                let mv = Move.Create kingPos attPos bd.PieceAt.[int (kingPos)] bd.PieceAt.[int (attPos)]
                getKingAttacks natt (mv :: mvl)
    
        let attacks = Attacks.KingAttacks(kingPos) &&& targetLocations
        let mvl = getKingAttacks attacks []
        
        mvl|>legal(bd)
    
    let CastleMoves (bd : Brd) :Move list = 
        let checkerCount = bd.Checkers |> Bitboard.BitCount
        if (checkerCount > 1) || (bd |> Board.IsChk) then []
        else
            let mvl =
                if bd.WhosTurn = Player.White then 
                    let mvl1 = 
                        let sqatt = bd
                                    |> Board.SquareAttacked E1 Player.Black
                                    || bd |> Board.SquareAttacked F1 Player.Black
                                    || bd |> Board.SquareAttacked G1 Player.Black
                        let sqemp = 
                            bd.PieceAt.[int (F1)] = Piece.EMPTY 
                            && bd.PieceAt.[int (G1)] = Piece.EMPTY
                        if (int (bd.CastleRights &&& CstlFlgs.WhiteShort) <> 0 
                            && bd.PieceAt.[int (E1)] = Piece.WKing 
                            && bd.PieceAt.[int (H1)] = Piece.WRook && sqemp && not sqatt) then 
                            let mv = 
                                Move.Create E1 G1 bd.PieceAt.[int (E1)] 
                                    bd.PieceAt.[int (G1)]
                            [mv]
                        else []

                    let sqatt = bd
                                |> Board.SquareAttacked E1 Player.Black
                                || bd |> Board.SquareAttacked D1 Player.Black
                                || bd |> Board.SquareAttacked C1 Player.Black
                    let sqemp = 
                        bd.PieceAt.[int (B1)] = Piece.EMPTY 
                        && bd.PieceAt.[int (C1)] = Piece.EMPTY 
                        && bd.PieceAt.[int (D1)] = Piece.EMPTY
                    if (int (bd.CastleRights &&& CstlFlgs.WhiteLong) <> 0 
                        && bd.PieceAt.[int (E1)] = Piece.WKing 
                        && bd.PieceAt.[int (A1)] = Piece.WRook && sqemp && not sqatt) then 
                        let mv = 
                            Move.Create E1 C1 bd.PieceAt.[int (E1)] 
                                bd.PieceAt.[int (C1)]
                        mv :: mvl1
                    else mvl1
                else 
                    let mvl2 = 
                        let sqatt = bd
                                    |> Board.SquareAttacked E8 Player.White
                                    || bd |> Board.SquareAttacked F8 Player.White
                                    || bd |> Board.SquareAttacked G8 Player.White
                        let sqemp = 
                            bd.PieceAt.[int (F8)] = Piece.EMPTY 
                            && bd.PieceAt.[int (G8)] = Piece.EMPTY
                        if (int (bd.CastleRights &&& CstlFlgs.BlackShort) <> 0 
                            && bd.PieceAt.[int (E8)] = Piece.BKing 
                            && bd.PieceAt.[int (H8)] = Piece.BRook && sqemp && not sqatt) then 
                            let mv = 
                                Move.Create E8 G8 bd.PieceAt.[int (E8)] 
                                    bd.PieceAt.[int (G8)]
                            [mv]
                        else []

                    let sqatt = bd
                                |> Board.SquareAttacked E8 Player.White
                                || bd |> Board.SquareAttacked D8 Player.White
                                || bd |> Board.SquareAttacked C8 Player.White
                    let sqemp = 
                        bd.PieceAt.[int (B8)] = Piece.EMPTY 
                        && bd.PieceAt.[int (C8)] = Piece.EMPTY 
                        && bd.PieceAt.[int (D8)] = Piece.EMPTY
                    if (int (bd.CastleRights &&& CstlFlgs.BlackLong) <> 0 
                        && bd.PieceAt.[int (E8)] = Piece.BKing 
                        && bd.PieceAt.[int (A8)] = Piece.BRook && sqemp && not sqatt) then 
                        let mv = 
                            Move.Create E8 C8 bd.PieceAt.[int (E8)] 
                                bd.PieceAt.[int (C8)]
                        mv :: mvl2
                    else mvl2
            mvl|>legal(bd)

    let private pcMoves(bd : Brd) (pt:PieceType) (fnsqbb: (Square -> Bitboard -> Bitboard)) :Move list =
        let me = bd.WhosTurn
        let kingPos = if me=Player.White then bd.WtKingPos else bd.BkKingPos
    
        let targetLocations = 
            let checkerCount = bd.Checkers |> Bitboard.BitCount
            if checkerCount = 1 then 
                let checkerPos = bd.Checkers |> Bitboard.NorthMostPosition
                let evasionTargets = 
                    (kingPos |> Square.Between(checkerPos)) ||| (checkerPos |> Square.ToBitboard)
                ((if me=Player.Black then bd.WtPrBds else bd.BkPrBds)|||(~~~bd.PieceLocationsAll)) &&& evasionTargets
            else
                (if me=Player.Black then bd.WtPrBds else bd.BkPrBds)|||(~~~bd.PieceLocationsAll)

        let rec getAttacks psns imvl = 
            if psns = Bitboard.Empty || targetLocations = Bitboard.Empty then imvl
            else 
                let piecepos, npsns = Bitboard.PopFirst(psns)
                let piece = bd.PieceAt.[int (piecepos)]
                let atts = (fnsqbb piecepos bd.PieceLocationsAll) &&& targetLocations 
    
                let rec getAtts att jmvl = 
                    if att = Bitboard.Empty then jmvl
                    else 
                        let attPos, natt = Bitboard.PopFirst(att)
                        let mv = Move.Create piecepos attPos piece bd.PieceAt.[int (attPos)]
                        getAtts natt (mv :: jmvl)
    
                let nimvl = getAtts atts imvl
                getAttacks npsns nimvl
        
        let piecePositions = 
            (if me=Player.White then bd.WtPrBds else bd.BkPrBds) &&& bd.PieceTypes.[int (pt)] 
        let mvl = getAttacks piecePositions []
        mvl|>legal(bd)
    
    let KnightMoves(bd : Brd) :Move list = 
        let checkerCount = bd.Checkers |> Bitboard.BitCount
        if checkerCount > 1 then []
        else
            let fnsqbb: (Square -> Bitboard -> Bitboard) = fun pp bb -> Attacks.KnightAttacks pp
            pcMoves bd PieceType.Knight fnsqbb

    let BishopMoves(bd : Brd) :Move list = 
        let checkerCount = bd.Checkers |> Bitboard.BitCount
        if checkerCount > 1 then []
        else
            let fnsqbb: (Square -> Bitboard -> Bitboard) = fun pp bb -> Attacks.BishopAttacks pp bb
            pcMoves bd PieceType.Bishop fnsqbb

    let RookMoves(bd : Brd) :Move list = 
        let checkerCount = bd.Checkers |> Bitboard.BitCount
        if checkerCount > 1 then []
        else
            let fnsqbb: (Square -> Bitboard -> Bitboard) = fun pp bb -> Attacks.RookAttacks pp bb
            pcMoves bd PieceType.Rook fnsqbb

    let QueenMoves(bd : Brd) :Move list = 
        let checkerCount = bd.Checkers |> Bitboard.BitCount
        if checkerCount > 1 then []
        else
            let fnsqbb: (Square -> Bitboard -> Bitboard) = fun pp bb -> Attacks.QueenAttacks pp bb
            pcMoves bd PieceType.Queen fnsqbb

    let PawnMoves(bd : Brd) =

        let checkerCount = bd.Checkers |> Bitboard.BitCount
        if checkerCount > 1 then []
        else

            let mypawnwest = 
                if bd.WhosTurn = Player.White then Dirn.DirNW
                else Dirn.DirSW

            let mypawneast = 
                if bd.WhosTurn = Player.White then Dirn.DirNE
                else Dirn.DirSE

            let mypawnnorth = 
                if bd.WhosTurn = Player.White then Dirn.DirN
                else Dirn.DirS

            let mypawnsouth = 
                if bd.WhosTurn = Player.White then Dirn.DirS
                else Dirn.DirN

            let myrank8 = 
                if bd.WhosTurn = Player.White then Rank8
                else Rank1

            let myrank2 = 
                if bd.WhosTurn = Player.White then Rank2
                else Rank7

            let me = bd.WhosTurn

            let kingPos = if me=Player.White then bd.WtKingPos else bd.BkKingPos

            let evasionTargets = 
                let checkerCount = bd.Checkers |> Bitboard.BitCount
                if checkerCount = 1 then 
                    let checkerPos = bd.Checkers |> Bitboard.NorthMostPosition
                    (kingPos |> Square.Between(checkerPos)) ||| (checkerPos |> Square.ToBitboard)
                else
                    ~~~Bitboard.Empty

            let piecePositions = (if me=Player.White then bd.WtPrBds else bd.BkPrBds) &&& bd.PieceTypes.[int (PieceType.Pawn)]

            let captureLocations = if me=Player.Black then bd.WtPrBds else bd.BkPrBds
            
            let targLocations = 
                captureLocations &&& evasionTargets ||| (if bd.EnPassant |> Square.IsInBounds then 
                                                            bd.EnPassant |> Square.ToBitboard
                                                         else Bitboard.Empty)
            
            let rec getPcaps capDir att imvl = 
                if att = Bitboard.Empty then 
                    if capDir = mypawneast then 
                        let attacks = (piecePositions |> Bitboard.Shift(mypawnwest)) &&& targLocations
                        getPcaps mypawnwest attacks imvl
                    else imvl
                else 
                    let targetpos, natt = Bitboard.PopFirst(att)
                    let piecepos = 
                        targetpos |> Square.PositionInDirectionUnsafe(capDir |> Direction.Opposite)
                    if (targetpos |> Square.ToRank) = myrank8 then 
                        let mv = 
                            Move.CreateProm piecepos targetpos bd.PieceAt.[int (piecepos)] 
                                bd.PieceAt.[int (targetpos)] PieceType.Queen
                        let imvl = mv :: imvl
                        let mv = 
                            Move.CreateProm piecepos targetpos bd.PieceAt.[int (piecepos)] 
                                bd.PieceAt.[int (targetpos)] PieceType.Rook
                        let imvl = mv :: imvl
                        let mv = 
                            Move.CreateProm piecepos targetpos bd.PieceAt.[int (piecepos)] 
                                bd.PieceAt.[int (targetpos)] PieceType.Bishop
                        let imvl = mv :: imvl
                        let mv = 
                            Move.CreateProm piecepos targetpos bd.PieceAt.[int (piecepos)] 
                                bd.PieceAt.[int (targetpos)] PieceType.Knight
                        let imvl = mv :: imvl
                        getPcaps capDir natt imvl
                    else 
                        let mv = 
                            Move.Create piecepos targetpos bd.PieceAt.[int (piecepos)] 
                                bd.PieceAt.[int (targetpos)]
                        let imvl = mv :: imvl
                        getPcaps capDir natt imvl
            
            let attacks = (piecePositions |> Bitboard.Shift(mypawneast)) &&& targLocations
            let pcaps:Move list = getPcaps mypawneast attacks []
            
            let rec getPones att imvl = 
                if att = Bitboard.Empty then imvl
                else 
                    let piecepos, natt = Bitboard.PopFirst(att)
                    let targetpos = piecepos |> Square.PositionInDirectionUnsafe(mypawnnorth)
                    if (targetpos |> Square.ToRank) = myrank8 then 
                        let mv = 
                            Move.CreateProm piecepos targetpos bd.PieceAt.[int (piecepos)] 
                                bd.PieceAt.[int (targetpos)] PieceType.Queen
                        let imvl = mv :: imvl
                        let mv = 
                            Move.CreateProm piecepos targetpos bd.PieceAt.[int (piecepos)] 
                                bd.PieceAt.[int (targetpos)] PieceType.Rook
                        let imvl = mv :: imvl
                        let mv = 
                            Move.CreateProm piecepos targetpos bd.PieceAt.[int (piecepos)] 
                                bd.PieceAt.[int (targetpos)] PieceType.Bishop
                        let imvl = mv :: imvl
                        let mv = 
                            Move.CreateProm piecepos targetpos bd.PieceAt.[int (piecepos)] 
                                bd.PieceAt.[int (targetpos)] PieceType.Knight
                        let imvl = mv :: imvl
                        getPones natt imvl
                    else 
                        let mv = 
                            Move.Create piecepos targetpos bd.PieceAt.[int (piecepos)] 
                                bd.PieceAt.[int (targetpos)]
                        let imvl = mv :: imvl
                        getPones natt imvl
            
            let moveLocations = (~~~bd.PieceLocationsAll) &&& evasionTargets
            let attacks = (moveLocations |> Bitboard.Shift(mypawnsouth)) &&& piecePositions
            let pones:Move list = getPones attacks []
            
            let rec getPtwos att imvl = 
                if att = Bitboard.Empty then imvl
                else 
                    let piecepos, natt = Bitboard.PopFirst(att)
                    
                    let targetpos = 
                        piecepos
                        |> Square.PositionInDirectionUnsafe(mypawnnorth)
                        |> Square.PositionInDirectionUnsafe(mypawnnorth)
                    
                    let mv = 
                        Move.Create piecepos targetpos bd.PieceAt.[int (piecepos)] 
                            bd.PieceAt.[int (targetpos)]
                    let imvl = mv :: imvl
                    getPtwos natt imvl
            
            let attacks = 
                (myrank2 |> Rank.ToBitboard) &&& piecePositions 
                &&& ((moveLocations |> Bitboard.Shift(mypawnsouth)) |> Bitboard.Shift(mypawnsouth)) 
                &&& (~~~bd.PieceLocationsAll |> Bitboard.Shift(mypawnsouth))
            let ptwos:Move list = getPtwos attacks []

            (ptwos@pones@pcaps)|>legal(bd)

    ///Gets all legal moves for this Board(bd)
    let AllMoves(bd : Brd) :Move list = 
        let checkerCount = bd.Checkers |> Bitboard.BitCount
        if checkerCount > 1 then bd|>KingMoves
        else
            (bd|>CastleMoves) @
            (bd|>PawnMoves) @
            (bd|>KnightMoves) @
            (bd|>BishopMoves) @
            (bd|>RookMoves) @
            (bd|>QueenMoves) @
            (bd|>KingMoves)

    let PossMoves (bd: Brd) (sq: Square) =
        let pc = bd.[sq]
        let plr = pc|>Piece.PieceToPlayer
        if plr<>bd.WhosTurn then []
        else
            let pt = pc|>Piece.ToPieceType
            match pt with
            |PieceType.Pawn -> bd|>PawnMoves|>List.filter(fun m -> m|>Move.From=sq)
            |PieceType.Knight -> bd|>KnightMoves|>List.filter(fun m -> m|>Move.From=sq)
            |PieceType.Bishop -> bd|>BishopMoves|>List.filter(fun m -> m|>Move.From=sq)
            |PieceType.Rook -> bd|>RookMoves|>List.filter(fun m -> m|>Move.From=sq)
            |PieceType.Queen -> bd|>QueenMoves|>List.filter(fun m -> m|>Move.From=sq)
            |PieceType.King -> ((bd|>KingMoves)@(bd|>CastleMoves))|>List.filter(fun m -> m|>Move.From=sq)
            |_ -> []
    
    let IsDrawByStalemate(bd : Brd) = 
        if not (bd |> Board.IsChk) then AllMoves(bd) |> List.isEmpty
        else false
    
    let IsMate(bd : Brd) = 
        if bd |> Board.IsChk then AllMoves(bd) |> List.isEmpty
        else false


