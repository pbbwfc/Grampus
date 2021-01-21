namespace FsChessPgn

open FsChess

module Move = 
    let Create (pfrom : Square) (pto : Square) (piece : Piece) (captured : Piece) :Move = 
        (uint32 (pfrom) ||| (uint32 (pto) <<< 6) ||| (uint32 (piece) <<< 12) ||| (uint32 (captured) <<< 16))
    let CreateProm (pfrom : Square) (pto : Square) (piece : Piece) (captured : Piece) (promoteType : PieceType) :Move = 
        (uint32 (pfrom) ||| (uint32 (pto) <<< 6) ||| (uint32 (piece) <<< 12) ||| (uint32 (captured) <<< 16) 
         ||| (uint32 (promoteType) <<< 20))
    let From(move : Move) :Square = int16(int(move) &&& 0x3F)
    let To(move : Move) :Square = int16(int(move) >>> 6 &&& 0x3F)
    let MovingPiece(move : Move) = (int(move) >>> 12 &&& 0xF) |> Pc
    let IsW(move : Move) = move|>MovingPiece|>int<9
    let MovingPieceType(move : Move) = (int(move) >>> 12 &&& 0x7) |> PcTp
    let MovingPlayer(move : Move) = (int(move) >>> 15 &&& 0x1) |> Plyr
    let IsCapture(move : Move) = (int(move) >>> 16 &&& 0xF) <> 0
    let CapturedPiece(move : Move) = (int(move) >>> 16 &&& 0xF) |> Pc
    let CapturedPieceType(move : Move) = (int(move) >>> 16 &&& 0x7) |> PcTp
    let IsPromotion(move : Move) = (int (move) >>> 20 &&& 0x7) <> 0
    let PromoteType(move : Move) = (int (move) >>> 20 &&& 0x7) |> PcTp

    let Promote(move : Move) = 
        if move
           |> PromoteType
           = PieceType.EMPTY then Piece.EMPTY
        else (move |> PromoteType)|>PieceType.ForPlayer(move|>MovingPlayer)

    let IsEnPassant(move : Move) = 
        move|>MovingPieceType = PieceType.Pawn && not (move|>IsCapture) 
                                && (move|>From|>Square.ToFile) <> (move|>To|>Square.ToFile)
    let IsCastle(move : Move) = 
        move|>MovingPieceType = PieceType.King && abs(int (move|>From) - int (move|>To)) = 2
    let IsPawnDoubleJump(move : Move) = 
        move|>MovingPieceType = PieceType.Pawn && abs(int (move|>From) - int (move|>To)) = 16
