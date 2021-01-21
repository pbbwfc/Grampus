namespace FsChessPgn

open FsChess

module Board = 
    
    let private PieceMove (mfrom : Square) mto (bd : Brd) = 
        let piece = bd.PieceAt.[int(mfrom)]
        let player = piece|>Piece.PieceToPlayer
        let pieceType = piece|>Piece.ToPieceType
        let pieceat = bd.PieceAt|>List.mapi(fun i p -> if i=int(mto) then piece elif i = int(mfrom) then Piece.EMPTY else p)
        let posBits = (mfrom |> Square.ToBitboard) ||| (mto |> Square.ToBitboard)
        let piecetypes = bd.PieceTypes|>List.mapi(fun i p -> if i=int(pieceType) then p ^^^ posBits else p)
        let wtprbds = if player=Player.White then bd.WtPrBds ^^^ posBits else bd.WtPrBds
        let bkprbds = if player=Player.Black then bd.BkPrBds ^^^ posBits else bd.BkPrBds
        let pieceLocationsAll = bd.PieceLocationsAll ^^^ posBits
        let wtkingpos = if pieceType = PieceType.King && player=Player.White then mto else bd.WtKingPos
        let bkkingpos = if pieceType = PieceType.King && player=Player.Black then mto else bd.BkKingPos
        { bd with PieceAt = pieceat
                  PieceTypes = piecetypes
                  WtPrBds = wtprbds
                  BkPrBds = bkprbds
                  PieceLocationsAll = pieceLocationsAll
                  WtKingPos = wtkingpos
                  BkKingPos = bkkingpos }
 
    let private PieceAdd pos (piece : Piece) (bd : Brd) = 
        let player = piece |> Piece.PieceToPlayer
        let pieceType = piece |> Piece.ToPieceType
        
        let pieceat = 
            bd.PieceAt |> List.mapi (fun i p -> 
                              if i = int (pos) then piece
                              else p)
        
        let posBits = pos |> Square.ToBitboard
        
        let piecetypes = 
            bd.PieceTypes |> List.mapi (fun i p -> 
                                 if i = int (piece |> Piece.ToPieceType) then p ||| posBits
                                 else p)
        
        let piecelocationsall = bd.PieceLocationsAll ||| posBits
        let wtprbds = if (piece |> Piece.PieceToPlayer)=Player.White then bd.WtPrBds ||| posBits else bd.WtPrBds
        let bkprbds = if (piece |> Piece.PieceToPlayer)=Player.Black then bd.BkPrBds ||| posBits else bd.BkPrBds
        let wtkingpos = if pieceType = PieceType.King && player=Player.White then pos else bd.WtKingPos
        let bkkingpos = if pieceType = PieceType.King && player=Player.Black then pos else bd.BkKingPos
        { bd with PieceAt = pieceat
                  PieceTypes = piecetypes
                  PieceLocationsAll = piecelocationsall
                  WtPrBds = wtprbds
                  BkPrBds = bkprbds
                  WtKingPos = wtkingpos
                  BkKingPos = bkkingpos }

    let private PieceRemove (pos : Square) (bd : Brd) = 
        let piece = bd.PieceAt.[int (pos)]
        let player = piece |> Piece.PieceToPlayer
        let pieceType = piece |> Piece.ToPieceType
        
        let pieceat = 
            bd.PieceAt |> List.mapi (fun i p -> 
                              if i = int (pos) then Piece.EMPTY
                              else p)
        let notPosBits = ~~~(pos |> Square.ToBitboard)
        
        let piecetypes = 
            bd.PieceTypes |> List.mapi (fun i p -> 
                                 if i = int (pieceType) then p &&& notPosBits
                                 else p)
        let wtprbds = if player=Player.White then bd.WtPrBds &&& notPosBits else bd.WtPrBds
        let bkprbds = if player=Player.Black then bd.BkPrBds &&& notPosBits else bd.BkPrBds
        let piecelocationsall = bd.PieceLocationsAll &&& notPosBits
        { bd with PieceAt = pieceat
                  PieceTypes = piecetypes
                  WtPrBds = wtprbds
                  BkPrBds = bkprbds
                  PieceLocationsAll = piecelocationsall
                  }
    
    let private PieceChange pos newPiece (bd : Brd) = 
        bd
        |> PieceRemove(pos)
        |> PieceAdd pos newPiece
        
    let private AttacksToBoth (mto : Square) (bd : Brd) = 
        (Attacks.KnightAttacks(mto) &&& bd.PieceTypes.[int (PieceType.Knight)]) 
        ||| ((Attacks.RookAttacks mto bd.PieceLocationsAll) 
             &&& (bd.PieceTypes.[int (PieceType.Queen)] ||| bd.PieceTypes.[int (PieceType.Rook)])) 
        ||| ((Attacks.BishopAttacks mto bd.PieceLocationsAll) 
             &&& (bd.PieceTypes.[int (PieceType.Queen)] ||| bd.PieceTypes.[int (PieceType.Bishop)])) 
        ||| (Attacks.KingAttacks(mto) &&& (bd.PieceTypes.[int (PieceType.King)])) 
        ||| ((Attacks.PawnAttacks mto Player.Black) &&& bd.BkPrBds
             &&& bd.PieceTypes.[int (PieceType.Pawn)]) 
        ||| ((Attacks.PawnAttacks mto Player.White) &&& bd.WtPrBds 
             &&& bd.PieceTypes.[int (PieceType.Pawn)])
    
    ///Gets the Bitboard that defines the squares that attack the specified Square(mto) by the specified Player(by) for this Board(bd) 
    let AttacksTo (mto : Square) (by : Player) (bd : Brd) = bd|> AttacksToBoth(mto) &&& (if by=Player.White then bd.WtPrBds else bd.BkPrBds)
    
    ///Gets the Squares that attack the specified Square(mto) by the specified Player(by) for this Board(bd) 
    let SquareAttacksTo (mto : Square) (by : Player) (bd : Brd) = (AttacksTo mto by bd)|>Bitboard.ToSquares

    ///Is the Square(mto) attacked by the specified Player(by) for this Board(bd)
    let SquareAttacked (mto : Square) (by : Player) (bd : Brd) = bd|> AttacksTo mto by <> Bitboard.Empty
    
    ///Make an encoded Move(move) for this Board(bd) and return the new Board
    let MoveApply (move : Move) (bd : Brd) = 
        let mfrom = move|>Move.From
        let mto = move|>Move.To
        let piece = move|>Move.MovingPiece
        let capture = move|>Move.CapturedPiece

        let bd = 
            if capture <> Piece.EMPTY then bd |> PieceRemove(mto)
            else bd
        
        let bd = bd |> PieceMove mfrom mto
        
        let bd = 
            if move |> Move.IsPromotion then bd |> PieceChange mto (move |> Move.Promote)
            else bd
        
        let bd = 
            if move |> Move.IsCastle then 
                if piece = Piece.WKing && mfrom = E1 && mto = G1 then 
                    bd |> PieceMove H1 F1
                elif piece = Piece.WKing && mfrom = E1 && mto = C1 then 
                    bd |> PieceMove A1 D1
                elif piece = Piece.BKing && mfrom = E8 && mto = G8 then 
                    bd |> PieceMove H8 F8
                else bd |> PieceMove A8 D8
            else bd
        
        let bd = 
            if bd.CastleRights <> CstlFlgs.EMPTY then 
                if mfrom = H1 then { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.WhiteShort}
                elif mfrom = A1 then { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.WhiteLong}
                elif piece = Piece.WKing then { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.WhiteShort &&& ~~~CstlFlgs.WhiteLong}
                elif mfrom = H8 then { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.BlackShort}
                elif mfrom = A8 then { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.BlackLong}
                elif piece = Piece.BKing then { bd with CastleRights = bd.CastleRights &&& ~~~CstlFlgs.BlackShort &&& ~~~CstlFlgs.BlackLong}
                else bd
            else bd
        
        let bd = 
            if move |> Move.IsEnPassant then 
                bd |> PieceRemove(Sq(mto|>Square.ToFile,move|>Move.MovingPlayer|>Player.MyRank(Rank5)))
            else bd
        
        let bd = 
            if bd.EnPassant|>Square.IsInBounds then 
                { bd with EnPassant = OUTOFBOUNDS }
            else bd
        
        let bd = 
            if move |> Move.IsPawnDoubleJump then 
                let ep = mfrom|>Square.PositionInDirectionUnsafe(move|>Move.MovingPlayer|>Direction.MyNorth)
                { bd with EnPassant = ep}
            else bd
        
        let bd = 
            if bd.WhosTurn = Player.Black then { bd with Fullmove = bd.Fullmove + 1 }
            else bd
        
        let bd = 
            if piece <> Piece.WPawn && piece <> Piece.BPawn && capture = Piece.EMPTY then 
                { bd with Fiftymove = bd.Fiftymove + 1 }
            else { bd with Fiftymove = 0 }
        
        let bd = 
            { bd with WhosTurn = bd.WhosTurn|>Player.PlayerOther}
        { bd with Checkers = bd
                             |> AttacksToBoth(if bd.WhosTurn=Player.White then bd.WtKingPos else bd.BkKingPos)
                             &&& (if (bd.WhosTurn|>Player.PlayerOther)=Player.White then bd.WtPrBds else bd.BkPrBds) }
    
    ///Is there a check on the Board(bd)
    let IsChk(bd : Brd) = bd.Checkers <> Bitboard.Empty
    
    ///Is there a check on Player(kingplayer) on the Board(bd)
    let IsChck (kingplayer : Player) (bd : Brd) = 
        let kingpos = if kingplayer=Player.White then bd.WtKingPos else bd.BkKingPos
        bd |> SquareAttacked kingpos (kingplayer|>Player.PlayerOther)
    
    let private PieceInDirection (from : Square) (dir : Dirn) (bd : Brd) = 
        let rec getpospc dist (pos : Square) pc = 
            if not (pos|>Square.IsInBounds) then pc, pos
            else 
                let npc = bd.PieceAt.[int (pos)]
                if npc <> Piece.EMPTY then npc, pos
                elif dir |> Direction.IsDirectionKnight then npc, pos
                else 
                    let npos = pos|>Square.PositionInDirection(dir)
                    getpospc (dist + 1) npos npc
        getpospc 1 (from|>Square.PositionInDirection(dir)) Piece.EMPTY
    
    ///Create a new Board given a Fen(fen)
    let FromFEN (fen : FsChessPgn.Fen) = 
        let bd = BrdEMP

        let rec addpc posl ibd = 
            if List.isEmpty posl then ibd
            else 
                let pos = posl.Head
                let pc = fen.Pieceat.[int(pos)]
                if pc = Piece.EMPTY then addpc posl.Tail ibd
                else addpc posl.Tail (ibd |> PieceAdd pos pc)
        
        let bd = addpc SQUARES bd
        { bd with CastleRights = 
                      CstlFlgs.EMPTY ||| (if fen.CastleWS then CstlFlgs.WhiteShort
                               else CstlFlgs.EMPTY) ||| (if fen.CastleWL then CstlFlgs.WhiteLong
                                              else CstlFlgs.EMPTY) ||| (if fen.CastleBS then CstlFlgs.BlackShort
                                                             else CstlFlgs.EMPTY)
                      ||| (if fen.CastleBL then CstlFlgs.BlackLong
                           else CstlFlgs.EMPTY)
                  WhosTurn = fen.Whosturn
                  EnPassant = fen.Enpassant
                  Fiftymove = fen.Fiftymove
                  Fullmove = fen.Fullmove
                  Checkers = bd
                             |> AttacksToBoth(if fen.Whosturn=Player.White then bd.WtKingPos else bd.BkKingPos)
                             &&& (if fen.Whosturn=Player.Black then bd.WtPrBds else bd.BkPrBds) }

    ///Create a new Board given a FEN string(str)
    let FromStr (str: string) = str|>FEN.Parse|>FromFEN 
    
    ///Gets a FEN string for this Board(bd) 
    let ToStr (bd : Brd) = bd|>FEN.FromBd|>FEN.ToStr

    ///Create a new Board given a simple string(str)
    let FromSimpleStr (str: string) = 
        let emp = BrdEMP
        let bits = str.Split(' ')
        let col = if bits.[1]="w" then Player.White else Player.Black
        let pcs =
            let chars = bits.[0].ToCharArray()
            chars|>Array.map Piece.Parse|>List.ofArray
        let fen = {emp with WhosTurn=col;PieceAt=pcs;CastleRights=CstlFlgs.All}|>FEN.FromBd
        fen|>FromFEN
    
    ///Produces a simple string of characters plus whether white or black to move
    let ToSimpleStr (bd : Brd) =
        let bdstr = bd.PieceAt|>List.map(fun p -> if p = Piece.EMPTY then "." else p|>Piece.PieceToString)|>List.reduce(+)
        let tomv = if bd.WhosTurn=Player.White then " w" else " b"
        bdstr + tomv
    
    ///Prints an ASCII version of this Board(bd) 
    let PrintAscii (bd : Brd) = 
        for irank = 0 to 7 do
            let getch ifile = 
                let rank = RANKS.[irank]
                let file = FILES.[ifile]
                let piece = bd.PieceAt.[int(Sq(file,rank))] 
                if piece = Piece.EMPTY then "." else piece|>Piece.PieceToString
            let ln =
                [0..7]
                |>List.map getch
                |>List.reduce(fun a b -> a + " " + b)
            printfn "%s" ("    " + ln)

    ///The starting Board at the beginning of a game
    let Start = FEN.Start|>FromFEN
