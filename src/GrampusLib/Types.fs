namespace Grampus

open MessagePack

module AssemblyInfo =
    open System.Runtime.CompilerServices
    
    [<assembly:InternalsVisibleTo("Grampus.Tests")>]
    do ()

[<AutoOpen>]
/// <namespacedoc>
///   <summary>This is the namespace containing all Grampus backend functionality.</summary>
/// </namespacedoc>
/// <summary>Holds all the main types used by Grampus.</summary>
module Types =
    /// <summary>Unsigned integer encoded to hold Move information.</summary>
    type Move = uint32
    
    let MoveEmpty : Move = 0u
    
    /// <summary>Enum holding each type of piece e.g. 1 for Pawn.</summary>
    type PieceType =
        | EMPTY = 0
        | Pawn = 1
        | Knight = 2
        | Bishop = 3
        | Rook = 4
        | Queen = 5
        | King = 6
    
    /// <summary>Enum holding each type of piece for each colour e.g. 1 for WPawn.</summary>
    type Piece =
        | WPawn = 1
        | WKnight = 2
        | WBishop = 3
        | WRook = 4
        | WQueen = 5
        | WKing = 6
        | BPawn = 9
        | BKnight = 10
        | BBishop = 11
        | BRook = 12
        | BQueen = 13
        | BKing = 14
        | EMPTY = 0
    
    /// <summary>Enum holding each type of player e.g. 1 for Black.</summary>
    type Player =
        | White = 0
        | Black = 1
    
    /// <summary>Enum holding each type of result e.g. 0 for Draw.</summary>
    type GameResult =
        | Draw = 0
        | WhiteWins = 1
        | BlackWins = -1
        | Open = 9
    
    /// <summary>Short encoded to hold File.</summary>
    type File = int16
    
    let FileA, FileB, FileC, FileD, FileE, FileF, FileG, FileH : File * File * File * File * File * File * File * File =
        0s, 1s, 2s, 3s, 4s, 5s, 6s, 7s
    let FILES = [ FileA; FileB; FileC; FileD; FileE; FileF; FileG; FileH ]
    let FILE_NAMES = [ "a"; "b"; "c"; "d"; "e"; "f"; "g"; "h" ]
    let FILE_EMPTY : File = 8s
    
    /// <summary>Short encoded to hold Rank.</summary>
    type Rank = int16
    
    let Rank1, Rank2, Rank3, Rank4, Rank5, Rank6, Rank7, Rank8 : Rank * Rank * Rank * Rank * Rank * Rank * Rank * Rank =
        0s, 1s, 2s, 3s, 4s, 5s, 6s, 7s
    let RANKS = [ Rank1; Rank2; Rank3; Rank4; Rank5; Rank6; Rank7; Rank8 ]
    let RANK_NAMES = [ "1"; "2"; "3"; "4"; "5"; "6"; "7"; "8" ]
    let RANK_EMPTY : Rank = 8s
    
    /// <summary>Short encoded to hold Square.</summary>
    type Square = int16
    
    let A1, B1, C1, D1, E1, F1, G1, H1 : Square * Square * Square * Square * Square * Square * Square * Square =
        0s, 1s, 2s, 3s, 4s, 5s, 6s, 7s
    let A2, B2, C2, D2, E2, F2, G2, H2 =
        A1 + 8s, B1 + 8s, C1 + 8s, D1 + 8s, E1 + 8s, F1 + 8s, G1 + 8s, H1 + 8s
    let A3, B3, C3, D3, E3, F3, G3, H3 =
        A2 + 8s, B2 + 8s, C2 + 8s, D2 + 8s, E2 + 8s, F2 + 8s, G2 + 8s, H2 + 8s
    let A4, B4, C4, D4, E4, F4, G4, H4 =
        A3 + 8s, B3 + 8s, C3 + 8s, D3 + 8s, E3 + 8s, F3 + 8s, G3 + 8s, H3 + 8s
    let A5, B5, C5, D5, E5, F5, G5, H5 =
        A4 + 8s, B4 + 8s, C4 + 8s, D4 + 8s, E4 + 8s, F4 + 8s, G4 + 8s, H4 + 8s
    let A6, B6, C6, D6, E6, F6, G6, H6 =
        A5 + 8s, B5 + 8s, C5 + 8s, D5 + 8s, E5 + 8s, F5 + 8s, G5 + 8s, H5 + 8s
    let A7, B7, C7, D7, E7, F7, G7, H7 =
        A6 + 8s, B6 + 8s, C6 + 8s, D6 + 8s, E6 + 8s, F6 + 8s, G6 + 8s, H6 + 8s
    let A8, B8, C8, D8, E8, F8, G8, H8 =
        A7 + 8s, B7 + 8s, C7 + 8s, D7 + 8s, E7 + 8s, F7 + 8s, G7 + 8s, H7 + 8s
    let OUTOFBOUNDS : Square = 64s
    let SQUARES =
        [ A1; B1; C1; D1; E1; F1; G1; H1; A2; B2; C2; D2; E2; F2; G2; H2; A3; B3; 
          C3; D3; E3; F3; G3; H3; A4; B4; C4; D4; E4; F4; G4; H4; A5; B5; C5; D5; 
          E5; F5; G5; H5; A6; B6; C6; D6; E6; F6; G6; H6; A7; B7; C7; D7; E7; F7; 
          G7; H7; A8; B8; C8; D8; E8; F8; G8; H8 ]
    
    let SQUARE_NAMES =
        [ for r in RANK_NAMES do
              for f in FILE_NAMES -> f + r ]
    
    let Sq(f : File, r : Rank) : Square = r * 8s + f
    
    [<System.Flags>]
    /// <summary>Enum holding each type of castling e.g. 1 for WhiteShort.</summary>
    type CstlFlgs =
        | EMPTY = 0
        | WhiteShort = 1
        | WhiteLong = 2
        | BlackShort = 4
        | BlackLong = 8
        | All = 15
    
    [<System.Flags>]
    /// <summary>Enum holding bitboards for squares/ranks, e.g. 1UL for A1.</summary>
    type Bitboard =
        | A1 = 1UL
        | B1 = 2UL
        | C1 = 4UL
        | D1 = 8UL
        | E1 = 16UL
        | F1 = 32UL
        | G1 = 64UL
        | H1 = 128UL
        | A2 = 256UL
        | B2 = 512UL
        | C2 = 1024UL
        | D2 = 2048UL
        | E2 = 4096UL
        | F2 = 8192UL
        | G2 = 16384UL
        | H2 = 32768UL
        | A3 = 65536UL
        | B3 = 131072UL
        | C3 = 262144UL
        | D3 = 524288UL
        | E3 = 1048576UL
        | F3 = 2097152UL
        | G3 = 4194304UL
        | H3 = 8388608UL
        | A4 = 16777216UL
        | B4 = 33554432UL
        | C4 = 67108864UL
        | D4 = 134217728UL
        | E4 = 268435456UL
        | F4 = 536870912UL
        | G4 = 1073741824UL
        | H4 = 2147483648UL
        | A5 = 4294967296UL
        | B5 = 8589934592UL
        | C5 = 17179869184UL
        | D5 = 34359738368UL
        | E5 = 68719476736UL
        | F5 = 137438953472UL
        | G5 = 274877906944UL
        | H5 = 549755813888UL
        | A6 = 1099511627776UL
        | B6 = 2199023255552UL
        | C6 = 4398046511104UL
        | D6 = 8796093022208UL
        | E6 = 17592186044416UL
        | F6 = 35184372088832UL
        | G6 = 70368744177664UL
        | H6 = 140737488355328UL
        | A7 = 281474976710656UL
        | B7 = 562949953421312UL
        | C7 = 1125899906842624UL
        | D7 = 2251799813685248UL
        | E7 = 4503599627370496UL
        | F7 = 9007199254740992UL
        | G7 = 18014398509481984UL
        | H7 = 36028797018963968UL
        | A8 = 72057594037927936UL
        | B8 = 144115188075855872UL
        | C8 = 288230376151711744UL
        | D8 = 576460752303423488UL
        | E8 = 1152921504606846976UL
        | F8 = 2305843009213693952UL
        | G8 = 4611686018427387904UL
        | H8 = 9223372036854775808UL
        | Rank8 = 18374686479671623680UL
        | Rank7 = 71776119061217280UL
        | Rank6 = 280375465082880UL
        | Rank5 = 1095216660480UL
        | Rank4 = 4278190080UL
        | Rank3 = 16711680UL
        | Rank2 = 65280UL
        | Rank1 = 255UL
        | FileA = 72340172838076673UL
        | FileB = 144680345676153346UL
        | FileC = 289360691352306692UL
        | FileD = 578721382704613384UL
        | FileE = 1157442765409226768UL
        | FileF = 2314885530818453536UL
        | FileG = 4629771061636907072UL
        | FileH = 9259542123273814144UL
        | Empty = 0UL
        | Full = 18446744073709551615UL
    
    /// <summary>Option holding each type of move e.g. Capture.</summary>
    type MoveType =
        | Simple
        | Capture
        | CastleKingSide
        | CastleQueenSide
    
    /// <summary>Enum holding each type of NAG e.g. 1 for Good.</summary>
    type NAG =
        | Null = 0
        | Good = 1
        | Poor = 2
        | VeryGood = 3
        | VeryPoor = 4
        | Speculative = 5
        | Questionable = 6
        | Even = 10
        | Wslight = 14
        | Bslight = 15
        | Wmoderate = 16
        | Bmoderate =17
        | Wdecisive = 18
        | Bdecisive = 19
    
    [<MessagePackObject>]
    /// <summary>Record type holding board details such as pieces on each square.</summary>
    type Brd =
        { [<Key(0)>]
          PieceAt : Piece list
          [<Key(1)>]
          WtKingPos : Square
          [<Key(2)>]
          BkKingPos : Square
          [<Key(3)>]
          PieceTypes : Bitboard list
          [<Key(4)>]
          WtPrBds : Bitboard
          [<Key(5)>]
          BkPrBds : Bitboard
          [<Key(6)>]
          PieceLocationsAll : Bitboard
          [<Key(7)>]
          Checkers : Bitboard
          [<Key(8)>]
          WhosTurn : Player
          [<Key(9)>]
          CastleRights : CstlFlgs
          [<Key(10)>]
          EnPassant : Square
          [<Key(11)>]
          Fiftymove : int
          [<Key(12)>]
          Fullmove : int }
        member bd.Item
            with get (sq : Square) = bd.PieceAt.[int (sq)]
        override bd.ToString() =
            let pctostr pc =
                match pc with
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
                | Piece.EMPTY -> "."
                | _ -> failwith "invalid piece"
            
            let bdstr =
                bd.PieceAt
                |> List.map (fun p -> p |> pctostr)
                |> List.reduce (+)
            
            let tomv =
                if bd.WhosTurn = Player.White then " w"
                else " b"
            
            bdstr + tomv
    
    let BrdEMP =
        { PieceAt = Array.create 64 Piece.EMPTY |> List.ofArray
          WtKingPos = OUTOFBOUNDS
          BkKingPos = OUTOFBOUNDS
          PieceTypes = Array.create 7 Bitboard.Empty |> List.ofArray
          WtPrBds = Bitboard.Empty
          BkPrBds = Bitboard.Empty
          PieceLocationsAll = Bitboard.Empty
          Checkers = Bitboard.Empty
          WhosTurn = Player.White
          CastleRights = CstlFlgs.EMPTY
          EnPassant = OUTOFBOUNDS
          Fiftymove = 0
          Fullmove = 0 }
    
    /// <summary>Record type holding details of an unencoded move such as pieces the target square.</summary>
    type pMove =
        { Mtype : MoveType
          TargetSquare : Square
          Piece : PieceType option
          OriginFile : File option
          OriginRank : Rank option
          PromotedPiece : PieceType option
          IsCheck : bool
          IsDoubleCheck : bool
          IsCheckMate : bool
          San : string }
        override x.ToString() = x.San
    
    [<MessagePackObject>]
    /// <summary>Record type holding header information such as the White ELO.</summary>
    type Header =
        { [<Key(0)>]
          Num : int
          [<Key(1)>]
          White : string
          [<Key(2)>]
          W_Elo : string
          [<Key(3)>]
          Black : string
          [<Key(4)>]
          B_Elo : string
          [<Key(5)>]
          Result : string
          [<Key(6)>]
          Year : int
          [<Key(7)>]
          Event : string
          [<Key(8)>]
          ECO : string
          [<Key(9)>]
          Opening : string
          [<Key(10)>]
          Deleted : string }
    
    let HeaderEMP =
        { Num = -1
          White = "?"
          W_Elo = "-"
          Black = "?"
          B_Elo = "-"
          Result = "*"
          Year = 0
          Event = "?"
          ECO = ""
          Opening = ""
          Deleted = "" }
    
    /// <summary>Discriminated Union type holding options for each possible item in game moves, such as a comment.</summary>
    type UnencodedMoveTextEntry =
        | UnencodedHalfMoveEntry of int option * bool * pMove
        | UnencodedCommentEntry of string
        | UnencodedGameEndEntry of GameResult
        | UnencodedNAGEntry of NAG
        | UnencodedRAVEntry of UnencodedMoveTextEntry list
    
    /// <summary>Record type holding unencoded game details such as the list of moves.</summary>
    type UnencodedGame =
        { Hdr : Header
          Month : int option
          Day : int option
          Round : string
          Site : string
          BoardSetup : Brd option
          AdditionalInfo : Map<string, string>
          MoveText : UnencodedMoveTextEntry list }
    
    let UnencodedGameEMP : UnencodedGame =
        { Hdr = HeaderEMP
          Site = "?"
          Month = None
          Day = None
          Round = "?"
          BoardSetup = None
          AdditionalInfo = Map.empty
          MoveText = [] }
    
    /// <summary>Record type holding details of an encoded move such as the move and the move number.</summary>
    type EncodedMove =
        { San : string
          Mno : int
          Isw : bool
          Mv : Move
          PostBrd : Brd }
        override x.ToString() = x.San
    
    /// <summary>Discriminated Union type holding options for each possible item in game moves, such as a comment.</summary>
    type EncodedMoveTextEntry =
        | EncodedHalfMoveEntry of int option * bool * EncodedMove
        | EncodedCommentEntry of string
        | EncodedGameEndEntry of GameResult
        | EncodedNAGEntry of NAG
        | EncodedRAVEntry of EncodedMoveTextEntry list
    
    /// <summary>Record type holding unencoded game details such as the list of moves.</summary>
    type EncodedGame =
        { Hdr : Header
          Month : int option
          Day : int option
          Round : string
          Site : string
          BoardSetup : Brd option
          AdditionalInfo : Map<string, string>
          MoveText : EncodedMoveTextEntry list }
    
    let EncodedGameEMP : EncodedGame =
        { Hdr = HeaderEMP
          Site = "?"
          Month = None
          Day = None
          Round = "?"
          BoardSetup = None
          AdditionalInfo = Map.empty
          MoveText = [] }
    
    [<MessagePackObject>]
    /// <summary>Record type holding details of a compressed move for storage.</summary>
    type CompressedMove =
        { [<Key(0)>]
          San : string
          [<Key(1)>]
          Mno : int
          [<Key(2)>]
          Isw : bool
          [<Key(3)>]
          Mv : Move }
        override x.ToString() = x.San
    
    [<MessagePackObject>]
    /// <summary>Discriminated Union type holding options for each possible item in game moves, such as a comment.</summary>
    type CompressedMoveTextEntry =
        | CompressedHalfMoveEntry of int option * bool * CompressedMove
        | CompressedCommentEntry of string
        | CompressedGameEndEntry of GameResult
        | CompressedNAGEntry of NAG
        | CompressedRAVEntry of CompressedMoveTextEntry list
    
    [<MessagePackObject>]
    /// <summary>Record type holding compressed game details such as the list of moves.</summary>
    type CompressedGame =
        { [<Key(0)>]
          Month : int option
          [<Key(1)>]
          Day : int option
          [<Key(2)>]
          Round : string
          [<Key(3)>]
          Site : string
          [<Key(4)>]
          BoardSetup : Brd option
          [<Key(5)>]
          AdditionalInfo : Map<string, string>
          [<Key(6)>]
          MoveText : CompressedMoveTextEntry list }
    
    let CompressedGameEMP : CompressedGame =
        { Site = "?"
          Month = None
          Day = None
          Round = "?"
          BoardSetup = None
          AdditionalInfo = Map.empty
          MoveText = [] }
    
    [<MessagePackObject>]
    /// <summary>Record type holding the index of a game in the storage.</summary>
    type IndexEntry =
        { [<Key(0)>]
          Offset : int64
          [<Key(1)>]
          Length : int }
    
    /// <summary>Record type holding summary data for a grampus base.</summary>
    type GrampusData =
        { SourcePgn : string
          BaseCreated : System.DateTime option
          TreesCreated : System.DateTime option
          FiltersCreated : System.DateTime option
          TreesPly : int
          FiltersPly : int }
    
    let GrampusDataEMP =
        { SourcePgn = ""
          BaseCreated = None
          TreesCreated = None
          FiltersCreated = None
          TreesPly = 20
          FiltersPly = 20 }
    
    /// <summary>Record type holding low level repertoire data.</summary>
    type RepOpt =
        { San : string
          Nag : NAG
          Comm : string }
    
    /// <summary>Map allowing lookup up of alternatives in a repertoire for a position.</summary>
    type RepOpts = Map<string, RepOpt list>
    
    /// <summary>Map allowing lookup up of preferred move in a repertoire for a position.</summary>
    type RepMove = Map<string, RepOpt>
    
    [<MessagePackObject>]
    /// <summary>Record type holding tree data related to a particular move.</summary>
    type mvstats() =
        
        [<Key(0)>]
        member val Mvstr = "" with get, set
        
        [<Key(1)>]
        member val Count = 0L with get, set
        
        [<Key(2)>]
        member val Freq = 0.0 with get, set
        
        [<Key(3)>]
        member val WhiteWins = 0L with get, set
        
        [<Key(4)>]
        member val Draws = 0L with get, set
        
        [<Key(5)>]
        member val BlackWins = 0L with get, set
        
        [<Key(6)>]
        member val Score = 0.0 with get, set
        
        [<Key(7)>]
        member val DrawPc = 0.0 with get, set
        
        [<Key(8)>]
        member val AvElo = 0L with get, set
        
        [<Key(9)>]
        member val Perf = 0L with get, set
        
        [<Key(10)>]
        member val AvYear = 0L with get, set
    
    [<MessagePackObject>]
    /// <summary>Record type holding tree data summed over all moves.</summary>
    type totstats() =
        
        [<Key(0)>]
        member val TotCount = 0L with get, set
        
        [<Key(1)>]
        member val TotFreq = 0.0 with get, set
        
        [<Key(2)>]
        member val TotWhiteWins = 0L with get, set
        
        [<Key(3)>]
        member val TotDraws = 0L with get, set
        
        [<Key(4)>]
        member val TotBlackWins = 0L with get, set
        
        [<Key(5)>]
        member val TotScore = 0.0 with get, set
        
        [<Key(6)>]
        member val TotDrawPc = 0.0 with get, set
        
        [<Key(7)>]
        member val TotAvElo = 0L with get, set
        
        [<Key(8)>]
        member val TotPerf = 0L with get, set
        
        [<Key(9)>]
        member val TotAvYear = 0L with get, set
    
    [<MessagePackObject>]
    /// <summary>Holds summary data used in the tree.</summary>
    type stats() =
        
        [<Key(0)>]
        member val MvsStats = new System.Collections.Generic.List<mvstats>() with get, set
        
        [<Key(1)>]
        member val TotStats = new totstats() with get, set
    
    /// <summary>Record type for holding Eco code and longer description.</summary>
    type Eco =
        { Code : string
          Desc : string }
    
    let EcoMap : Map<string, Eco> =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let strm = thisExe.GetManifestResourceStream("GrampusLib.eco.json")
        let reader = new System.IO.StreamReader(strm)
        let str = reader.ReadToEnd()
        reader.Close()
        strm.Close()
        FSharp.Json.Json.deserialize (str)
