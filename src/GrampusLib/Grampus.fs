namespace Grampus

/// <summary>
/// Holds the functions related to the Game Result.
/// </summary>
module Result =
    /// <summary>
    /// Gets the string symbol for a Result.
    /// </summary>
    /// <param name="result">The Game Result.</param>
    /// <returns>The result as a string, such as 1-0</returns>
    let ToStr(result) = GrampusInternal.GameResult.ToStr(result)
    
    /// <summary>
    /// Gets the GameResult type from a string.
    /// </summary>
    /// <param name="str">The string results such as 1-0.</param>
    /// <returns>The Game Result, such as GameResult.WhiteWins</returns>
    let Parse(str) = GrampusInternal.GameResult.Parse(str)
    
    /// <summary>
    /// Gets the integer value (2 for white win, 0 for blackwin, 1 otherwise) for a Result.
    /// </summary>
    /// <param name="str">The Game Result.</param>
    /// <returns>The result as an int, such as 2 for white win.</returns>
    let ToInt(result) = GrampusInternal.GameResult.ToInt(result)

/// <summary>
/// Holds the functions related to a Piece.
/// </summary>
module Piece =
    /// <summary>
    /// Gets the string symbol for a Piece.
    /// </summary>
    /// <param name="piece">The piece, such as Piece.WKnight.</param>
    /// <returns>The result as a string, such as N.</returns>
    let ToStr(piece) = GrampusInternal.Piece.PieceToString(piece)

/// <summary>
/// Holds the functions related to a Board.
/// </summary>
module Board =
    /// <summary>
    /// Create a new Board given a FEN string.
    /// </summary>
    /// <param name="fenstr">The FEN string.</param>
    /// <returns>The Board as a Brd type.</returns>
    let FromFenStr(fenstr) = GrampusInternal.Board.FromFenStr(fenstr)
    
    /// <summary>
    /// Create a FEN string from this Board.
    /// </summary>
    /// <param name="bd">The Board as a Brd type.</param>
    /// <returns>The FEN string.</returns>
    let ToFenStr(bd) = GrampusInternal.Board.ToFenStr(bd)
    
    /// <summary>
    /// Create a new Board given a simple string.
    /// </summary>
    /// <param name="str">The simple string.</param>
    /// <returns>The Board as a Brd type.</returns>
    let FromSimpleStr(str) = GrampusInternal.Board.FromSimpleStr(str)
    
    /// <summary>
    /// Create a simple string from this Board.
    /// </summary>
    /// <param name="bd">The Board as a Brd type.</param>
    /// <returns>The simple string.</returns>
    let ToSimpleStr(bd) = GrampusInternal.Board.ToSimpleStr(bd)
    
    ///The starting Board at the beginning of a game
    let Start = GrampusInternal.Board.Start
    
    /// <summary>
    /// Gets all possible moves for this Board from the specified Square.
    /// </summary>
    /// <param name="bd">The Board as a Brd type.</param>
    /// <param name="sq">The Square as a Square type.</param>
    /// <returns>The list of all possible moves.</returns>
    let PossMoves bd sq = GrampusInternal.MoveGenerate.PossMoves bd sq
    
    /// <summary>
    /// Make an encoded Move for this Board and return the new Board.
    /// </summary>
    /// <param name="mv">The move as a Move type.</param>
    /// <param name="bd">The Board as a Brd type.</param>
    /// <returns>The new Board as a Brd type.</returns>
    let Push mv bd = GrampusInternal.Board.MoveApply mv bd

/// <summary>
/// Holds the functions related to a Move.
/// </summary>
module Move =
    /// <summary>
    /// Get the source Square for an encoded Move.
    /// </summary>
    /// <param name="mv">The move as a Move type.</param>
    /// <returns>The source square.</returns>
    let From(mv) = GrampusInternal.Move.From(mv)
    
    /// <summary>
    /// Get the target Square for an encoded Move.
    /// </summary>
    /// <param name="mv">The move as a Move type.</param>
    /// <returns>The target square.</returns>
    let To(mv) = GrampusInternal.Move.To(mv)
    
    /// <summary>
    /// Get the promoted PieceType for an encoded Move.
    /// </summary>
    /// <param name="mv">The move as a Move type.</param>
    /// <returns>The promoted piece type as a PieceType type.</returns>
    let PromPcTp(mv) = GrampusInternal.Move.PromoteType(mv)
    
    /// <summary>
    /// Get an encoded move from a SAN string such as Nf3 for this Board.
    /// </summary>
    /// <param name="bd">The Board as a Brd type.</param>
    /// <param name="san">The SAN string, such as Nf3.</param>
    /// <returns>The move as a Move type.</returns>
    let FromSan bd san = GrampusInternal.MoveUtil.fromSAN bd san
    
    /// <summary>
    /// Get an encoded move from a UCI string such as g1f3 for this Board.
    /// </summary>
    /// <param name="bd">The Board as a Brd type.</param>
    /// <param name="uci">The SAN string, such as g1f3.</param>
    /// <returns>The move as a Move type.</returns>
    let FromUci bd uci = GrampusInternal.MoveUtil.fromUci bd uci
    
    /// <summary>
    /// Get a string of multiple SAN strings from a string of UCIs for this Board.
    /// </summary>
    /// <param name="bd">The Board as a Brd type.</param>
    /// <param name="ucis">The string of UCI strings, such as g1f3 g8f6.</param>
    /// <returns>The string of multiple SAN strings, such as Nf3 Nf6.</returns>
    let FromUcis bd ucis = GrampusInternal.MoveUtil.UcisToSans bd ucis
    
    /// <summary>
    /// Get the UCI string such as g1f3 for a move.
    /// </summary>
    /// <param name="mv">The move as a Move type.</param>
    /// <returns>The UCI string, such as g1f3.</returns>
    let ToUci(mv) = GrampusInternal.MoveUtil.toUci (mv)
    
    /// <summary>
    /// Get the SAN string such as Nf3 for a move for this board.
    /// </summary>
    /// <param name="bd">The Board as a Brd type.</param>
    /// <param name="mv">The move as a Move type.</param>
    /// <returns>The SAN string, such as Nf3.</returns>
    let ToSan bd mv = GrampusInternal.MoveUtil.toPgn bd mv

/// <summary>
/// Holds the functions related to a Move.
/// </summary>
module Game =
    ///The starting Game with no moves
    let Start = GrampusInternal.GameEncoded.Start
    
    /// <summary>
    /// Gets a NAG as a string such as ?? given one of the list from Game.MoveText.
    /// </summary>
    /// <param name="nag">The NAG as a NAG type.</param>
    /// <returns>The NAG string.</returns>
    let NAGStr(nag) = GrampusInternal.NagUtil.ToStr(nag)
    
    /// <summary>
    /// Gets a NAG from a string such as ??.
    /// </summary>
    /// <param name="str">The NAG string.</param>
    /// <returns>The NAG type, such as NAG.Good.</returns>
    let NAGFromStr(str) = GrampusInternal.NagUtil.FromStr(str)
    
    /// <summary>
    /// Gets a NAG as HTML such as ?? given one of the list from Game.MoveText.
    /// </summary>
    /// <param name="nag">The NAG as a NAG type.</param>
    /// <returns>The NAG HTML string.</returns>
    let NAGHtm(nag) = GrampusInternal.NagUtil.ToHtm(nag)
    
    /// <summary>
    /// Gets a NAG as a description such as Good given one of the list from Game.MoveText.
    /// </summary>
    /// <param name="nag">The NAG as a NAG type.</param>
    /// <returns>The NAG description.</returns>
    let NAGDesc(nag) = GrampusInternal.NagUtil.Desc(nag)
    
    ///Gets a list of all NAGs supported
    let NAGlist = GrampusInternal.NagUtil.All
    
    /// <summary>
    /// Adds a NAG in the EncodedGame after the address provided.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <param name="irs">The address within the game as an int list type.</param>
    /// <param name="nag">The NAG as a NAG type.</param>
    /// <returns>The changed Encoded Game.</returns>
    let AddNag gm irs nag = GrampusInternal.GameEncoded.AddNag gm irs nag
    
    /// <summary>
    /// Deletes a NAG in the Encoded Game at the address provided.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <param name="irs">The address within the game as an int list type.</param>
    /// <returns>The changed Encoded Game.</returns>
    let DeleteNag gm irs = GrampusInternal.GameEncoded.DeleteNag gm irs
    
    /// <summary>
    /// Edits a NAG in the Encoded Game at the address provided.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <param name="irs">The address within the game as an int list type.</param>
    /// <param name="nag">The NAG as a NAG type.</param>
    /// <returns>The changed Encoded Game.</returns>
    let EditNag gm irs nag = GrampusInternal.GameEncoded.EditNag gm irs nag
    
    /// <summary>
    /// Encodes the Game.
    /// </summary>
    /// <param name="ugm">The Unencoded Game.</param>
    /// <returns>The Encoded Game.</returns>
    let Encode(ugm) = GrampusInternal.GameEncoded.Encode(ugm)
    
    /// <summary>
    /// Adds a Move to the Game given its address.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <param name="irs">The address within the game as an int list type.</param>
    /// <param name="mv">The Move.</param>
    /// <returns>The changed Encoded Game and the new address.</returns>
    let AddMv gm irs mv = GrampusInternal.GameEncoded.AddMv gm irs mv
    
    /// <summary>
    /// Adds a RAV to the Game given the Move is contains and its address.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <param name="irs">The address within the game as an int list type.</param>
    /// <param name="mv">The Move.</param>
    /// <returns>The changed Encoded Game and the new address.</returns>
    let AddRav gm irs mv = GrampusInternal.GameEncoded.AddRav gm irs mv
    
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
