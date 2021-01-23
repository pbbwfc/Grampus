namespace FsChess

module GameDate =

    ///Gets the string symbol for a Piece
    let ToStr = FsChessPgn.DateUtil.ToStr2


module Result =

    ///Gets the string symbol for a Result
    let ToStr = FsChessPgn.GameResult.ToStr

    ///Gets the integer value (2 for white win, 0 for blackwin, 1 otherwise) for a Result
    let ToInt = FsChessPgn.GameResult.ToInt

    ///Gets the string symbol for a Result
    let ToUnicode = FsChessPgn.GameResult.ToUnicode

module Square =

    ///Gets the File for a Square
    let ToFile = FsChessPgn.Square.ToFile

    ///Gets the Rank for a Square
    let ToRank = FsChessPgn.Square.ToRank

    ///Gets the Name for a Square
    let Name = FsChessPgn.Square.Name

module Piece =

    ///Gets the string symbol for a Piece
    let ToStr = FsChessPgn.Piece.PieceToString

    ///Gets the player for a Piece
    let ToPlayer = FsChessPgn.Piece.PieceToPlayer

module Board =

    ///Create a new Board given a FEN string
    let FromStr = FsChessPgn.Board.FromStr
    
    ///Create a FEN string from this Board 
    let ToStr = FsChessPgn.Board.ToStr

    ///Create a new Board given a simple string
    let FromSimpleStr = FsChessPgn.Board.FromSimpleStr
    
    ///Create a simple string from this Board 
    let ToSimpleStr = FsChessPgn.Board.ToSimpleStr

    ///The starting Board at the beginning of a game
    let Start = FsChessPgn.Board.Start

    ///Gets all legal moves for this Board
    let AllMoves = FsChessPgn.MoveGenerate.AllMoves

    ///Gets all possible moves for this Board from the specified Square
    let PossMoves = FsChessPgn.MoveGenerate.PossMoves

    ///Make an encoded Move for this Board and return the new Board
    let Push = FsChessPgn.Board.MoveApply

    ///Make a SAN Move such as Nf3 for this Board and return the new Board
    let PushSAN = FsChessPgn.MoveUtil.ApplySAN

    ///Is there a check on the Board
    let IsCheck = FsChessPgn.Board.IsChk
    
    ///Is the current position on the Board checkmate?
    let IsCheckMate = FsChessPgn.MoveGenerate.IsMate 

    ///Is the current position on the Board stalemate?
    let IsStaleMate = FsChessPgn.MoveGenerate.IsDrawByStalemate 

    ///Is the Square attacked by the specified Player for this Board
    let SquareAttacked = FsChessPgn.Board.SquareAttacked
    
    ///The Squares that attack the specified Square by the specified Player for this Board
    let SquareAttackers = FsChessPgn.Board.SquareAttacksTo

    ///Creates a PNG image ith specified name, flipped if specified for the given Board 
    let ToPng = FsChessPgn.Png.BoardToPng

    ///Prints an ASCII version of this Board 
    let Print = FsChessPgn.Board.PrintAscii

module Move =

    ///Get the source Square for an encoded Move
    let From = FsChessPgn.Move.From

    ///Get the target Square for an encoded Move
    let To = FsChessPgn.Move.To

    ///Get the promoted PieceType for an encoded Move
    let PromPcTp = FsChessPgn.Move.PromoteType

    ///Get an encoded move from a SAN string such as Nf3 for this Board
    let FromSan = FsChessPgn.MoveUtil.fromSAN

    ///Get an encoded move from a UCI string such as g1f3 for this Board
    let FromUci = FsChessPgn.MoveUtil.fromUci

    ///Get a string of encoded moves from a string of UCIs for this Board
    let FromUcis = FsChessPgn.MoveUtil.UcisToSans

    ///Get the UCI string such as g1f3 for a move
    let ToUci = FsChessPgn.MoveUtil.toUci

    ///Get the pMove for a move for this board
    let TopMove = FsChessPgn.MoveUtil.topMove

    ///Get the Encoded Move for a move for this board
    let ToeMove = FsChessPgn.MoveEncoded.FromMove

    ///Get the SAN string such as Nf3 for a move for this board
    let ToSan = FsChessPgn.MoveUtil.toPgn

module Game =

    ///The starting Game with no moves
    let Start = FsChessPgn.GameUnencoded.Start

    ///Make a SAN Move such as Nf3 for this Game and return the new Game
    let PushSAN = FsChessPgn.GameUnencoded.AddSan

    ///Pops a move of the end for this Game and return the new Game
    let Pop = FsChessPgn.GameUnencoded.RemoveMoveEntry

    ///Gets a single move as a string given one of the list from Game.MoveText
    let MoveStr = FsChessPgn.PgnWrite.MoveTextEntryStr

    ///Gets a NAG as a string such as ?? given one of the list from Game.MoveText
    let NAGStr = FsChessPgn.NagUtil.ToStr

    ///Gets a NAG from a string such as ?? 
    let NAGFromStr = FsChessPgn.NagUtil.FromStr

    ///Gets a NAG as HTML such as ?? given one of the list from Game.MoveText
    let NAGHtm = FsChessPgn.NagUtil.ToHtm

    ///Gets a NAG as a description such as Very Good given one of the list from Game.MoveText
    let NAGDesc = FsChessPgn.NagUtil.Desc

    ///Gets a list of all NAGs supported
    let NAGlist = FsChessPgn.NagUtil.All

    ///Adds a Nag in the EncodedGame after the address provided
    let AddNag = FsChessPgn.GameEncoded.AddNag

    ///Deletes a Nag in the Encoded Game at the address provided
    let DeleteNag = FsChessPgn.GameEncoded.DeleteNag

    ///Edits a Nag in the Encoded Game at the address provided
    let EditNag = FsChessPgn.GameEncoded.EditNag

    ///Gets the moves text as a string given the Game.MoveText
    let MovesStr = FsChessPgn.PgnWrite.MoveTextStr

    ///Encodes the Game
    let Encode = FsChessPgn.GameEncoded.Encode

    ///Adds an EncodedMove to the Game given its address
    let AddMv = FsChessPgn.GameEncoded.AddMv

    ///Adds a RAV to the Game given the Encoded Move is contains and its address
    let AddRav = FsChessPgn.GameEncoded.AddRav

    ///Deletes a RAV in the EncodedGame at the address provided
    let DeleteRav = FsChessPgn.GameEncoded.DeleteRav
    
    ///Strips moves until end of game at the address provided
    let Strip = FsChessPgn.GameEncoded.Strip

    ///Adds a comment to the Encoded Game before the address provided
    let CommentBefore = FsChessPgn.GameEncoded.CommentBefore

    ///Adds a comment to the Encoded Game after the address provided
    let CommentAfter = FsChessPgn.GameEncoded.CommentAfter

    ///Edits a comment to the Encoded Game at the address provided
    let EditComment = FsChessPgn.GameEncoded.EditComment

    ///Deletes a comment in the Encoded Game at the address provided
    let DeleteComment = FsChessPgn.GameEncoded.DeleteComment

    ///Get from a PGN string
    let FromStr = FsChessPgn.RegParse.GameFromString

    ///Convert to a PGN string
    let ToStr = FsChessPgn.GameEncoded.ToStr

    ///Compresses an Encoded Game
    let Compress = FsChessPgn.GameEncoded.Compress 

    ///Expands a Compressed Game
    let Expand = FsChessPgn.GameEncoded.Expand

    ///Gets the Positions and Sans up to the specified ply
    let GetPosnsMoves = FsChessPgn.GameEncoded.GetPosnsMoves

    ///Gets the Positionsup to the specified ply
    let GetPosns = FsChessPgn.GameEncoded.GetPosns


module Repertoire =
    
    ///White Repertoire
    let White() = FsChessPgn.Repertoire.WhiteRep
    
    ///Black Repertoire
    let Black() = FsChessPgn.Repertoire.BlackRep
    
    ///White Error File
    let WhiteErrFile() = FsChessPgn.Repertoire.whiteerrs

    ///Black Error File
    let BlackErrFile() = FsChessPgn.Repertoire.blackerrs

    ///Load White Repertoire
    let LoadWhite = FsChessPgn.Repertoire.LoadWhite
    
    ///Load Black Repertoire
    let LoadBlack = FsChessPgn.Repertoire.LoadBlack
    
    /////Update White Repertoire from database
    //let UpdateWhite = FsChessPgn.Repertoire.UpdateWhite
    
    /////Update Black Repertoire from database
    //let UpdateBlack = FsChessPgn.Repertoire.UpdateBlack

    ///Options contaion SAN
    let OptsHaveSan = FsChessPgn.Repertoire.optsHasSan

module StaticTree =

    ///Creates the tree storage given a folder
    let Create = FsChessPgn.StaticTree.Create

    ///Creates big tree storage given a folder
    let CreateBig = FsChessPgn.StaticTree.CreateBig

    ///Saves the tree storage given an array of positions, an array of stats and a folder
    let Save = FsChessPgn.StaticTree.Save

    ///Reads the tree storage given an array of positions and a folder
    let ReadArray = FsChessPgn.StaticTree.ReadArray

    ///Reads the tree storage given a position and a folder
    let Read = FsChessPgn.StaticTree.Read
    
    ///Compavct the tree storage given a folder
    let Compact = FsChessPgn.StaticTree.Compact

module Filter =

    ///Creates the tree storage given a folder
    let Create = FsChessPgn.Filter.Create

    ///Creates big tree storage given a folder
    let CreateBig = FsChessPgn.Filter.CreateBig

    ///Saves the tree storage given an array of positions, an array of stats and a folder
    let Save = FsChessPgn.Filter.Save

    ///Reads the tree storage given an array of positions and a folder
    let ReadArray = FsChessPgn.Filter.ReadArray

    ///Reads the tree storage given a position and a folder
    let Read = FsChessPgn.Filter.Read
    
    ///Compavct the tree storage given a folder
    let Compact = FsChessPgn.Filter.Compact

module Grampus =
    
    ///Load Garmpus File
    let Load = FsChessPgn.Grampus.Load
    
    ///Save Grampus File
    let Save = FsChessPgn.Grampus.Save

module Index =
    
    ///Load Index
    let Load = FsChessPgn.Index.Load

    ///Save Index
    let Save = FsChessPgn.Index.Save

module Games =
    
    ///Load Game
    let LoadGame = FsChessPgn.Games.LoadGame

    ///Save Games
    let Save = FsChessPgn.Games.Save
