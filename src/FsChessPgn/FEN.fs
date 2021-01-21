namespace FsChessPgn

open FsChess
open System.Text
open System.Text.RegularExpressions

type Fen = 
    { Pieceat : Piece list
      Whosturn : Player
      CastleWS : bool
      CastleWL : bool
      CastleBS : bool
      CastleBL : bool
      Enpassant : Square
      Fiftymove : int
      Fullmove : int }

module FEN = 
    let ToStr(fen : Fen) = 
        let sb = new StringBuilder(50)
        for irank = 7 downto 0 do
            let rec getect ect ifile = 
                if ifile > 7 then ect
                else 
                    let rank = RANKS.[irank]
                    let file = FILES.[ifile]
                    let piece = fen.Pieceat.[int(Sq(file,rank))] 
                    if piece = Piece.EMPTY then getect (ect + 1) (ifile + 1)
                    else 
                        if ect > 0 then sb.Append(ect.ToString()) |> ignore
                        sb.Append(piece |> Piece.PieceToString) |> ignore
                        getect 0 (ifile + 1)
            
            let ect = getect 0 0
            if ect > 0 then sb.Append(ect.ToString()) |> ignore
            if irank > 0 then sb.Append("/") |> ignore
        if fen.Whosturn = Player.White then sb.Append(" w ") |> ignore
        else sb.Append(" b ") |> ignore
        if (fen.CastleWS || fen.CastleWL || fen.CastleBS || fen.CastleBL) then 
            if fen.CastleWS then sb.Append("K") |> ignore
            if fen.CastleWL then sb.Append("Q") |> ignore
            if fen.CastleBS then sb.Append("k") |> ignore
            if fen.CastleBL then sb.Append("q") |> ignore
        else sb.Append("-") |> ignore
        if fen.Enpassant |> Square.IsInBounds then sb.Append(" " + (fen.Enpassant |> Square.Name)) |> ignore
        else sb.Append(" -") |> ignore
        sb.Append(" " + fen.Fiftymove.ToString()) |> ignore
        sb.Append(" " + fen.Fullmove.ToString()) |> ignore
        sb.ToString()
    
    let FromBd(bd : Brd) = 
        { Pieceat = bd.PieceAt
          Whosturn = bd.WhosTurn
          CastleWS = int (bd.CastleRights &&& CstlFlgs.WhiteShort) <> 0
          CastleWL = int (bd.CastleRights &&& CstlFlgs.WhiteLong) <> 0
          CastleBS = int (bd.CastleRights &&& CstlFlgs.BlackShort) <> 0
          CastleBL = int (bd.CastleRights &&& CstlFlgs.BlackLong) <> 0
          Enpassant = bd.EnPassant
          Fiftymove = bd.Fiftymove
          Fullmove = bd.Fullmove }
    
    let Parse(sFEN : string) = 
        let pieceat = Array.create 64 Piece.EMPTY
        let sbPattern = new StringBuilder()
        sbPattern.Append(@"(?<R8>[\w]{1,8})/") |> ignore
        sbPattern.Append(@"(?<R7>[\w]{1,8})/") |> ignore
        sbPattern.Append(@"(?<R6>[\w]{1,8})/") |> ignore
        sbPattern.Append(@"(?<R5>[\w]{1,8})/") |> ignore
        sbPattern.Append(@"(?<R4>[\w]{1,8})/") |> ignore
        sbPattern.Append(@"(?<R3>[\w]{1,8})/") |> ignore
        sbPattern.Append(@"(?<R2>[\w]{1,8})/") |> ignore
        sbPattern.Append(@"(?<R1>[\w]{1,8})") |> ignore
        sbPattern.Append(@"\s+(?<Player>[wbWB]{1})") |> ignore
        sbPattern.Append(@"\s+(?<Castle>[-KQkq]{1,4})") |> ignore
        sbPattern.Append(@"\s+(?<Enpassant>[-\w]{1,2})") |> ignore
        sbPattern.Append(@"\s+(?<FiftyMove>[-\d]+)") |> ignore
        sbPattern.Append(@"\s+(?<FullMove>[-\d]+)") |> ignore
        let pattern = sbPattern.ToString()
        let regex = new Regex(pattern)
        let matches = regex.Matches(sFEN)
        if matches.Count = 0 then failwith "No valid fen found"
        if matches.Count > 1 then failwith "Multiple FENs in string"
        let matchr = matches.[0]
        let sRanks = RANKS|>List.map(fun r -> matchr.Groups.["R" + (r |> Rank.RankToString)].Value)
        let sPlayer = matchr.Groups.["Player"].Value
        let sCastle = matchr.Groups.["Castle"].Value
        let sEnpassant = matchr.Groups.["Enpassant"].Value
        let sFiftyMove = matchr.Groups.["FiftyMove"].Value
        let sFullMove = matchr.Groups.["FullMove"].Value
        for rank in RANKS do
            let rec getpc (cl : char list) ifl = 
                if not (List.isEmpty cl) then 
                    if ifl > 7 then failwith ("too many pieces in rank " + (rank |> Rank.RankToString))
                    let c = cl.Head
                    if "1234567890".IndexOf(c) >= 0 then getpc cl.Tail (ifl + System.Int32.Parse(c.ToString()))
                    else 
                        pieceat.[int(Sq(FILES.[ifl],rank))] <- Piece.Parse(c)//OK
                        getpc cl.Tail (ifl + 1)
            
            let srank = sRanks.[System.Int32.Parse(rank |> Rank.RankToString) - 1]
            getpc (srank.ToCharArray() |> List.ofArray) 0
        let whosTurn = 
            if sPlayer = "w" then Player.White
            elif sPlayer = "b" then Player.Black
            else failwith (sPlayer + " is not a valid player")
        
        let castleWS = sCastle.IndexOf("K") >= 0
        let castleWL = sCastle.IndexOf("Q") >= 0
        let castleBS = sCastle.IndexOf("k") >= 0
        let castleBL = sCastle.IndexOf("q") >= 0
        
        let enpassant = 
            if sEnpassant <> "-" then Square.Parse(sEnpassant)
            else OUTOFBOUNDS
        
        let fiftyMove = 
            if sFiftyMove <> "-" then System.Int32.Parse(sFiftyMove)
            else 0
        
        let fullMove = 
            if sFullMove <> "-" then System.Int32.Parse(sFullMove)
            else 0
        
        { Pieceat = pieceat|>List.ofArray
          Whosturn = whosTurn
          CastleWS = castleWS
          CastleWL = castleWL
          CastleBS = castleBS
          CastleBL = castleBL
          Enpassant = enpassant
          Fiftymove = fiftyMove
          Fullmove = fullMove }
    
    let StartStr = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
    let Start = Parse StartStr
