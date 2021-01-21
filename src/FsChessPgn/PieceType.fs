namespace FsChessPgn

open FsChess

module PieceType = 
    let Parse(c : char) = 
        match c with
        | 'P' -> PieceType.Pawn
        | 'N' -> PieceType.Knight
        | 'B' -> PieceType.Bishop
        | 'R' -> PieceType.Rook
        | 'Q' -> PieceType.Queen
        | 'K' -> PieceType.King
        | 'p' -> PieceType.Pawn
        | 'n' -> PieceType.Knight
        | 'b' -> PieceType.Bishop
        | 'r' -> PieceType.Rook
        | 'q' -> PieceType.Queen
        | 'k' -> PieceType.King
        | _ -> failwith (c.ToString() + " is not a valid piece")
    
    let ForPlayer (player : Player) (pt : PieceType) : Piece = (int (pt) ||| (int (player) <<< 3)) |> Pc
    

