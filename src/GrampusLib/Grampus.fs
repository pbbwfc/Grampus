namespace Grampus

module GameDate =
    ///Gets the string symbol for a Piece
    let ToStr = GrampusInternal.DateUtil.ToStr2

module Result =
    ///Gets the string symbol for a Result
    let ToStr = GrampusInternal.GameResult.ToStr
    
    ///Gets the GameResult type from a string
    let Parse = GrampusInternal.GameResult.Parse
    
    ///Gets the integer value (2 for white win, 0 for blackwin, 1 otherwise) for a Result
    let ToInt = GrampusInternal.GameResult.ToInt
    
    ///Gets the string symbol for a Result
    let ToUnicode = GrampusInternal.GameResult.ToUnicode

module Square =
    ///Gets the File for a Square
    let ToFile = GrampusInternal.Square.ToFile
    
    ///Gets the Rank for a Square
    let ToRank = GrampusInternal.Square.ToRank
    
    ///Gets the Name for a Square
    let Name = GrampusInternal.Square.Name

module Piece =
    ///Gets the string symbol for a Piece
    let ToStr = GrampusInternal.Piece.PieceToString
    
    ///Gets the player for a Piece
    let ToPlayer = GrampusInternal.Piece.PieceToPlayer

module Board =
    ///Create a new Board given a FEN string
    let FromStr = GrampusInternal.Board.FromStr
    
    ///Create a FEN string from this Board 
    let ToStr = GrampusInternal.Board.ToStr
    
    ///Create a new Board given a simple string
    let FromSimpleStr = GrampusInternal.Board.FromSimpleStr
    
    ///Create a simple string from this Board 
    let ToSimpleStr = GrampusInternal.Board.ToSimpleStr
    
    ///The starting Board at the beginning of a game
    let Start = GrampusInternal.Board.Start
    
    ///Gets all legal moves for this Board
    let AllMoves = GrampusInternal.MoveGenerate.AllMoves
    
    ///Gets all possible moves for this Board from the specified Square
    let PossMoves = GrampusInternal.MoveGenerate.PossMoves
    
    ///Make an encoded Move for this Board and return the new Board
    let Push = GrampusInternal.Board.MoveApply
    
    ///Make a SAN Move such as Nf3 for this Board and return the new Board
    let PushSAN = GrampusInternal.MoveUtil.ApplySAN
    
    ///Is there a check on the Board
    let IsCheck = GrampusInternal.Board.IsChk
    
    ///Is the current position on the Board checkmate?
    let IsCheckMate = GrampusInternal.MoveGenerate.IsMate
    
    ///Is the current position on the Board stalemate?
    let IsStaleMate = GrampusInternal.MoveGenerate.IsDrawByStalemate
    
    ///Is the Square attacked by the specified Player for this Board
    let SquareAttacked = GrampusInternal.Board.SquareAttacked
    
    ///The Squares that attack the specified Square by the specified Player for this Board
    let SquareAttackers = GrampusInternal.Board.SquareAttacksTo
    
    ///Creates a PNG image ith specified name, flipped if specified for the given Board 
    let ToPng = GrampusInternal.Png.BoardToPng
    
    ///Prints an ASCII version of this Board 
    let Print = GrampusInternal.Board.PrintAscii

module Move =
    ///Get the source Square for an encoded Move
    let From = GrampusInternal.Move.From
    
    ///Get the target Square for an encoded Move
    let To = GrampusInternal.Move.To
    
    ///Get the promoted PieceType for an encoded Move
    let PromPcTp = GrampusInternal.Move.PromoteType
    
    ///Get an encoded move from a SAN string such as Nf3 for this Board
    let FromSan = GrampusInternal.MoveUtil.fromSAN
    
    ///Get an encoded move from a UCI string such as g1f3 for this Board
    let FromUci = GrampusInternal.MoveUtil.fromUci
    
    ///Get a string of encoded moves from a string of UCIs for this Board
    let FromUcis = GrampusInternal.MoveUtil.UcisToSans
    
    ///Get the UCI string such as g1f3 for a move
    let ToUci = GrampusInternal.MoveUtil.toUci
    
    ///Get the pMove for a move for this board
    let TopMove = GrampusInternal.MoveUtil.topMove
    
    ///Get the Encoded Move for a move for this board
    let ToeMove = GrampusInternal.MoveEncoded.FromMove
    
    ///Get the SAN string such as Nf3 for a move for this board
    let ToSan = GrampusInternal.MoveUtil.toPgn

module Game =
    ///The starting Game with no moves
    let Start = GrampusInternal.GameEncoded.Start
    
    ///Make a SAN Move such as Nf3 for this Game and return the new Game
    let PushSAN = GrampusInternal.GameUnencoded.AddSan
    
    ///Pops a move of the end for this Game and return the new Game
    let Pop = GrampusInternal.GameUnencoded.RemoveMoveEntry
    
    ///Gets a single move as a string given one of the list from Game.MoveText
    let MoveStr = GrampusInternal.PgnWrite.MoveTextEntryStr
    
    ///Gets a NAG as a string such as ?? given one of the list from Game.MoveText
    let NAGStr = GrampusInternal.NagUtil.ToStr
    
    ///Gets a NAG from a string such as ?? 
    let NAGFromStr = GrampusInternal.NagUtil.FromStr
    
    ///Gets a NAG as HTML such as ?? given one of the list from Game.MoveText
    let NAGHtm = GrampusInternal.NagUtil.ToHtm
    
    ///Gets a NAG as a description such as Very Good given one of the list from Game.MoveText
    let NAGDesc = GrampusInternal.NagUtil.Desc
    
    ///Gets a list of all NAGs supported
    let NAGlist = GrampusInternal.NagUtil.All
    
    ///Adds a Nag in the EncodedGame after the address provided
    let AddNag = GrampusInternal.GameEncoded.AddNag
    
    ///Deletes a Nag in the Encoded Game at the address provided
    let DeleteNag = GrampusInternal.GameEncoded.DeleteNag
    
    ///Edits a Nag in the Encoded Game at the address provided
    let EditNag = GrampusInternal.GameEncoded.EditNag
    
    ///Gets the moves text as a string given the Game.MoveText
    let MovesStr = GrampusInternal.PgnWrite.MoveTextStr
    
    ///Encodes the Game
    let Encode = GrampusInternal.GameEncoded.Encode
    
    ///Adds an EncodedMove to the Game given its address
    let AddMv = GrampusInternal.GameEncoded.AddMv
    
    ///Adds a RAV to the Game given the Encoded Move is contains and its address
    let AddRav = GrampusInternal.GameEncoded.AddRav
    
    ///Deletes a RAV in the EncodedGame at the address provided
    let DeleteRav = GrampusInternal.GameEncoded.DeleteRav
    
    ///Strips moves until end of game at the address provided
    let Strip = GrampusInternal.GameEncoded.Strip
    
    ///Adds a comment to the Encoded Game before the address provided
    let CommentBefore = GrampusInternal.GameEncoded.CommentBefore
    
    ///Adds a comment to the Encoded Game after the address provided
    let CommentAfter = GrampusInternal.GameEncoded.CommentAfter
    
    ///Edits a comment to the Encoded Game at the address provided
    let EditComment = GrampusInternal.GameEncoded.EditComment
    
    ///Deletes a comment in the Encoded Game at the address provided
    let DeleteComment = GrampusInternal.GameEncoded.DeleteComment
    
    ///Get from a PGN string
    let FromStr = GrampusInternal.RegParse.GameFromString
    
    ///Convert to a PGN string
    let ToStr = GrampusInternal.GameEncoded.ToStr
    
    ///Compresses an Encoded Game
    let Compress = GrampusInternal.GameEncoded.Compress
    
    ///Expands a Compressed Game
    let Expand = GrampusInternal.GameEncoded.Expand
    
    ///Gets the Positions and Sans up to the specified ply
    let GetPosnsMoves = GrampusInternal.GameEncoded.GetPosnsMoves
    
    ///Gets the Positionsup to the specified ply
    let GetPosns = GrampusInternal.GameEncoded.GetPosns

module Repertoire =
    ///White Repertoire
    let White() = GrampusInternal.Repertoire.WhiteRep
    
    ///Black Repertoire
    let Black() = GrampusInternal.Repertoire.BlackRep
    
    ///White Error File
    let WhiteErrFile() = GrampusInternal.Repertoire.whiteerrs
    
    ///Black Error File
    let BlackErrFile() = GrampusInternal.Repertoire.blackerrs
    
    ///Load White Repertoire
    let LoadWhite = GrampusInternal.Repertoire.LoadWhite
    
    ///Load Black Repertoire
    let LoadBlack = GrampusInternal.Repertoire.LoadBlack
    
    ///Update White Repertoire from database
    let UpdateWhite = GrampusInternal.Repertoire.UpdateWhite
    
    ///Update Black Repertoire from database
    let UpdateBlack = GrampusInternal.Repertoire.UpdateBlack
    
    ///Options contaion SAN
    let OptsHaveSan = GrampusInternal.Repertoire.optsHasSan

module StaticTree =
    ///Creates the tree storage given a folder
    let Create = GrampusInternal.StaticTree.Create
    
    ///Saves the tree storage given an array of positions, an array of stats and a folder
    let Save = GrampusInternal.StaticTree.Save
    
    ///Reads the tree storage given an array of positions and a folder
    let ReadArray = GrampusInternal.StaticTree.ReadArray
    
    ///Reads the tree storage given a position and a folder
    let Read = GrampusInternal.StaticTree.Read

module Filter =
    ///Creates the tree storage given a folder
    let Create = GrampusInternal.Filter.Create
    
    ///Saves the tree storage given an array of positions, an array of stats and a folder
    let Save = GrampusInternal.Filter.Save
    
    ///Reads the tree storage given an array of positions and a folder
    let ReadArray = GrampusInternal.Filter.ReadArray
    
    ///Reads the tree storage given a position and a folder
    let Read = GrampusInternal.Filter.Read

module Grampus =
    ///Load Garmpus File
    let Load = GrampusInternal.GrampusFile.Load
    
    ///Save Grampus File
    let Save = GrampusInternal.GrampusFile.Save

module Index =
    ///Load Index
    let Load = GrampusInternal.Index.Load
    
    ///Save Index
    let Save = GrampusInternal.Index.Save

module Eco =
    //Set eco for game
    let ForGame = GrampusInternal.Eco.ForGame
    //Set eco for games
    let ForBase = GrampusInternal.Eco.ForBase

module Headers =
    ///Load Game Rows
    let Load = GrampusInternal.Headers.Load
    
    ///Save Game Rows
    let Save = GrampusInternal.Headers.Save

module Games =
    ///Load Game
    let LoadGame = GrampusInternal.Games.LoadGame
    
    ///Save Games
    let Save = GrampusInternal.Games.Save
    
    ///Add Games
    let Add = GrampusInternal.Games.Add
    
    ///Append Game
    let AppendGame = GrampusInternal.Games.AppendGame
    
    ///Update Game
    let UpdateGame = GrampusInternal.Games.UpdateGame
    
    ///Compact base
    let Compact = GrampusInternal.Games.Compact

module PgnGames =
    ///Get an array of Games from a file
    let ReadArrayFromFile = GrampusInternal.PgnGames.ReadArrayFromFile
    
    ///Get a list of Games from a file
    let ReadListFromFile = GrampusInternal.PgnGames.ReadListFromFile
    
    ///Get a list of index * Game from a file
    let ReadIndexListFromFile = GrampusInternal.PgnGames.ReadIndexListFromFile
    
    ///Get a Sequence of Games from a file
    let ReadSeqFromFile = GrampusInternal.PgnGames.ReadSeqFromFile
    
    ///Write a list of Games to a file
    let WriteFile = GrampusInternal.PgnWriter.WriteFile
    
    ///Encodes a sequence of Unencoded Games
    let Encode = GrampusInternal.PgnGames.Encode
