namespace FsChessPgn

open FsChess

module Piece = 
    
    let Parse(c : char) = 
        match c with
        | 'P' -> Piece.WPawn
        | 'N' -> Piece.WKnight
        | 'B' -> Piece.WBishop
        | 'R' -> Piece.WRook
        | 'Q' -> Piece.WQueen
        | 'K' -> Piece.WKing
        | 'p' -> Piece.BPawn
        | 'n' -> Piece.BKnight
        | 'b' -> Piece.BBishop
        | 'r' -> Piece.BRook
        | 'q' -> Piece.BQueen
        | 'k' -> Piece.BKing
        | '.' -> Piece.EMPTY
        | _ -> failwith (c.ToString() + " is not a valid piece")
    
    let PieceToString(piece : Piece) = 
        match piece with
        | Piece.WPawn -> "P"
        | Piece.WKnight -> "N"
        | Piece.WBishop -> "B"
        | Piece.WRook -> "R"
        | Piece.WQueen -> "Q"
        | Piece.WKing -> "K"
        | Piece.BPawn -> "p"
        | Piece.BKnight -> "n"
        | Piece.BBishop -> "b"
        | Piece.BRook -> "r"
        | Piece.BQueen -> "q"
        | Piece.BKing -> "k"
        | Piece.EMPTY -> " "
        | _ -> failwith ("not a valid piece")
    
    let ToPieceType(piece : Piece) = (int (piece) &&& 7) |> PcTp
    let ParseAsPiece (player : Player) (c : char) = (Parse(c) |> ToPieceType)|>PieceType.ForPlayer(player)
    
    let ToOppositePlayer(piece : Piece) = 
        if piece = Piece.EMPTY then Piece.EMPTY
        else (int (piece) ^^^ 8) |> Pc
    
    let PieceToPlayer(piece : Piece) = (int (piece) >>> 3) |> Plyr
