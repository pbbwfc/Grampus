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
    /// Removes all NAGs in the Encoded Game.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <returns>The changed Encoded Game.</returns>
    let RemoveNags gm = GrampusInternal.GameEncoded.RemoveNags gm
    
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
    
    /// <summary>
    /// Deletes a RAV in the EncodedGame at the address provided.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <param name="irs">The address within the game as an int list type.</param>
    /// <returns>The changed Encoded Game.</returns>
    let DeleteRav gm irs = GrampusInternal.GameEncoded.DeleteRav gm irs
    
    /// <summary>
    /// Removes all variations in the Encoded Game.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <returns>The changed Encoded Game.</returns>
    let RemoveRavs gm = GrampusInternal.GameEncoded.RemoveRavs gm
    
    /// <summary>
    /// Strips moves until end of game at the address provided.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <param name="irs">The address within the game as an int list type.</param>
    /// <returns>The changed Encoded Game.</returns>
    let Strip gm irs = GrampusInternal.GameEncoded.Strip gm irs
    
    /// <summary>
    /// Adds a comment to the Encoded Game before the address provided.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <param name="irs">The address within the game as an int list type.</param>
    /// <param name="str">The comments as a string.</param>
    /// <returns>The changed Encoded Game.</returns>
    let CommentBefore gm irs str =
        GrampusInternal.GameEncoded.CommentBefore gm irs str
    
    /// <summary>
    /// Adds a comment to the Encoded Game after the address provided.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <param name="irs">The address within the game as an int list type.</param>
    /// <param name="str">The comments as a string.</param>
    /// <returns>The changed Encoded Game.</returns>
    let CommentAfter gm irs str =
        GrampusInternal.GameEncoded.CommentAfter gm irs str
    
    /// <summary>
    /// Edits a comment to the Encoded Game at the address provided.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <param name="irs">The address within the game as an int list type.</param>
    /// <param name="str">The comments as a string.</param>
    /// <returns>The changed Encoded Game.</returns>
    let EditComment gm irs str =
        GrampusInternal.GameEncoded.EditComment gm irs str
    
    /// <summary>
    /// Deletes a comment in the Encoded Game at the address provided.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <param name="irs">The address within the game as an int list type.</param>
    /// <returns>The changed Encoded Game.</returns>
    let DeleteComment gm irs = GrampusInternal.GameEncoded.DeleteComment gm irs
    
    /// <summary>
    /// Removes all comments in the Encoded Game.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <returns>The changed Encoded Game.</returns>
    let RemoveComments gm = GrampusInternal.GameEncoded.RemoveComments gm
    
    /// <summary>
    /// Create an Encoded Game from a PGN string.
    /// </summary>
    /// <param name="pgnstr">The PGN string.</param>
    /// <returns>The new Encoded Game.</returns>
    let FromStr pgnstr = GrampusInternal.RegParse.GameFromString pgnstr
    
    /// <summary>
    /// Create a PGN string from an Encoded Game.
    /// </summary>
    /// <param name="gm">The Encoded Game.</param>
    /// <returns>The new PGN string.</returns>
    let ToStr gm = GrampusInternal.GameEncoded.ToStr gm
    
    /// <summary>
    /// Gets the Positions and Sans up to the specified ply.
    /// </summary>
    /// <param name="ply">The ply to process to, -1 for all moves.</param>
    /// <param name="gm">The Encoded Game.</param>
    /// <returns>The arrays of Positions and SANs as strings.</returns>
    let GetPosnsMoves ply gm = GrampusInternal.GameEncoded.GetPosnsMoves ply gm
    
    /// <summary>
    /// Gets the Positions up to the specified ply.
    /// </summary>
    /// <param name="ply">The ply to process to, -1 for all moves.</param>
    /// <param name="gm">The Encoded Game.</param>
    /// <returns>The arrays of Positions as strings.</returns>
    let GetPosns ply gm = GrampusInternal.GameEncoded.GetPosns ply gm

/// <summary>
/// Holds the functions related to a Repertoire.
/// </summary>
module Repertoire =
    ///White Repertoire
    let White() = GrampusInternal.Repertoire.WhiteRep
    
    ///Black Repertoire
    let Black() = GrampusInternal.Repertoire.BlackRep
    
    ///White Repertoire Error File
    let WhiteErrFile() = GrampusInternal.Repertoire.whiteerrs()
    
    ///Black Repertoire Error File
    let BlackErrFile() = GrampusInternal.Repertoire.blackerrs()
    
    ///Load White Repertoire
    let LoadWhite() = GrampusInternal.Repertoire.LoadWhite()
    
    ///Load Black Repertoire
    let LoadBlack() = GrampusInternal.Repertoire.LoadBlack()
    
    /// <summary>
    /// Update White Repertoire from database.
    /// </summary>
    /// <returns>The number of errors.</returns>
    let UpdateWhite() = GrampusInternal.Repertoire.UpdateWhite()
    
    /// <summary>
    /// Update Blck Repertoire from database.
    /// </summary>
    /// <returns>The number of errors.</returns>
    let UpdateBlack() = GrampusInternal.Repertoire.UpdateBlack()
    
    /// <summary>
    /// Check whether options contains a particular SAN string.
    /// </summary>
    /// <param name="san">The SAN string.</param>
    /// <param name="opts">The list of options as a RepOpt list type.</param>
    /// <returns>Whether in the options as a bool.</returns>
    let OptsHaveSan san opts = GrampusInternal.Repertoire.optsHasSan san opts

/// <summary>
/// Holds the functions related to a Tree.
/// </summary>
module Tree =
    /// <summary>
    /// Creates the tree storage given a folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>Nothing.</returns>
    let Create(nm) = GrampusInternal.StaticTree.Create(nm)
    
    /// <summary>
    /// Saves the tree storage given an array of positions, an array of stats and a folder.
    /// </summary>
    /// <param name="posns">The array of positions for the tree.</param>
    /// <param name="sts">The array of stats for the tree.</param>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>Nothing.</returns>
    let Save(posns, sts, nm) = GrampusInternal.StaticTree.Save(posns, sts, nm)
    
    /// <summary>
    /// Reads the tree storage given a position and a folder.
    /// </summary>
    /// <param name="posn">The position to read in the tree.</param>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>The tree for the posn as a stats type.</returns>
    let Read(posn, nm) = GrampusInternal.StaticTree.Read(posn, nm)

/// <summary>
/// Holds the functions related to Filters.
/// </summary>
module Filters =
    /// <summary>
    /// Creates the filter storage given a folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>Nothing.</returns>
    let Create(nm) = GrampusInternal.Filters.Create(nm)
    
    /// <summary>
    /// Saves the filter storage given an array of positions, an array of references to games and a folder.
    /// </summary>
    /// <param name="posns">The array of positions for the tree.</param>
    /// <param name="filts">The array of references to games as an int list type.</param>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>Nothing.</returns>
    let Save(posns, filts, nm) = GrampusInternal.Filters.Save(posns, filts, nm)
    
    /// <summary>
    /// Reads the filter storage given a position and a folder.
    /// </summary>
    /// <param name="posn">The position to read in the tree.</param>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>The filter for the posn as an int list type.</returns>
    let Read(posn, nm) = GrampusInternal.Filters.Read(posn, nm)

/// <summary>
/// Holds the functions related to the Grampus file.
/// </summary>
module Grampus =
    /// <summary>
    /// Loads a Grampus File given the full path and file name.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>The contents of the file as a GrampusData type.</returns>
    let Load(nm) = GrampusInternal.GrampusFile.Load(nm)
    
    /// <summary>
    /// Saves a Grampus File given the full path and file name and the data.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="gmp">The contents of the file as a GrampusData type.</param>
    /// <returns>Nothing.</returns>
    let Save(nm, gmp) = GrampusInternal.GrampusFile.Save(nm, gmp)
    
    /// <summary>
    /// Creates a new Grampus File given the full path and file name.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>The contents of the new file as a GrampusData type.</returns>
    let New(nm) = GrampusInternal.GrampusFile.New(nm)
    
    /// <summary>
    /// Deletes a Grampus File and related content given the full path and file name.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>Nothing.</returns>
    let Delete(nm) = GrampusInternal.GrampusFile.Delete(nm)
    
    /// <summary>
    /// Copies a Grampus File and related content given the full path and file name of source and target.
    /// </summary>
    /// <param name="nm">The full path and file name of the source.</param>
    /// <param name="copynm">The full path and file name of the target.</param>
    /// <returns>Nothing.</returns>
    let Copy(nm, copynm) = GrampusInternal.GrampusFile.Copy(nm, copynm)
    
    /// <summary>
    /// Deletes the tree stored in a Base given the full path and file name and returns the new Grampus file.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>The contents of the new file as a GrampusData type.</returns>
    let DeleteTree(nm) = GrampusInternal.GrampusFile.DeleteTree(nm)
    
    /// <summary>
    /// Deletes the filters stored in a Base given the full path and file name and returns the new Grampus file.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>The contents of the new file as a GrampusData type.</returns>
    let DeleteFilters(nm) = GrampusInternal.GrampusFile.DeleteFilters(nm)
    
    /// <summary>
    /// Deletes the filters and games stored in a Base given the full path and file name and returns the new Grampus file.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>The contents of the new file as a GrampusData type.</returns>
    let DeleteGamesFilters(nm) =
        GrampusInternal.GrampusFile.DeleteGamesFilters(nm)

/// <summary>
/// Holds the functions related to an Index.
/// </summary>
module Index =
    /// <summary>
    /// Loads the Index given the folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>The index as an IndexEntry array type.</returns>
    let Load(nm) = GrampusInternal.Index.Load(nm)
    
    /// <summary>
    /// Saves the Index given the folder and the data.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="iea">The index as an IndexEntry array type.</param>
    /// <returns>Nothing.</returns>
    let Save(nm, iea) = GrampusInternal.Index.Save(nm, iea)

/// <summary>
/// Holds the functions related to ECO classification.
/// </summary>
module Eco =
    /// <summary>
    /// Set ECO for the game provided.
    /// </summary>
    /// <param name="gm">The Encoded game.</param>
    /// <returns>The updated game as an EncodedGame type.</returns>
    let ForGame(gm) = GrampusInternal.Eco.ForGame(gm)
    
    /// <summary>
    /// Set ECO for games given the folder containing the base.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="cb">Callback function to report on progress.</param>
    /// <returns>Nothing.</returns>
    let ForBase nm cb = GrampusInternal.Eco.ForBase nm cb

/// <summary>
/// Holds the functions related to Headers.
/// </summary>
module Headers =
    /// <summary>
    /// Loads the headers given the folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <returns>The headers as an Header array type.</returns>
    let Load(nm) = GrampusInternal.Headers.Load(nm)
    
    /// <summary>
    /// Saves the headers given the folder and the data.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="hdrs">The headers as a Header array type.</param>
    /// <returns>Nothing.</returns>
    let Save(nm, hdrs) = GrampusInternal.Headers.Save(nm, hdrs)

/// <summary>
/// Holds the functions related to a set of Games.
/// </summary>
module Games =
    /// <summary>
    /// Loads an encoded game given the folder, index entry and the header.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="ie">The index entry for the game as an IndexEntry type.</param>
    /// <param name="hdr">The header for the game as a Header type.</param>
    /// <returns>The encoded game.</returns>
    let LoadGame nm ie hdr = GrampusInternal.Games.LoadGame nm ie hdr
    
    /// <summary>
    /// Saves a sequence of games to create a new base in the specified folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="gms">The games as an EncodedGame sequence type.</param>
    /// <param name="cb">Callback function to report on progress.</param>
    /// <returns>Nothing.</returns>
    let Save nm gms cb = GrampusInternal.Games.Save nm gms cb
    
    /// <summary>
    /// Adds a sequence of games to an existing base in the specified folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="gms">The games as an EncodedGame sequence type.</param>
    /// <param name="cb">Callback function to report on progress.</param>
    /// <returns>Nothing.</returns>
    let Add nm gms cb = GrampusInternal.Games.Add nm gms cb
    
    /// <summary>
    /// Adds a sequence of games to an existing base in the specified folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="addnm">The full path and file name of the base with the games to add.</param>
    /// <param name="cb">Callback function to report on progress.</param>
    /// <returns>Nothing.</returns>
    let AddGmp nm addnm cb = GrampusInternal.Games.AddGmp nm addnm cb
    
    /// <summary>
    /// Adds game to an existing base in the specified folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="gm">The game as an EncodedGame type.</param>
    /// <returns>Nothing.</returns>
    let AppendGame nm gm = GrampusInternal.Games.AppendGame nm gm
    
    /// <summary>
    /// Updates a game in an existing base in the specified folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="gnum">The game number to update a int type.</param>
    /// <param name="gm">The game as an EncodedGame type.</param>
    /// <returns>Nothing.</returns>
    let UpdateGame nm gnum gm = GrampusInternal.Games.UpdateGame nm gnum gm
    
    /// <summary>
    /// Compacts the base in the specified folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="cb">Callback function to report on progress.</param>
    /// <returns>Messages from compaction process as a string type.</returns>
    let Compact nm cb = GrampusInternal.Games.Compact nm cb
    
    /// <summary>
    /// Extracts newer games to the specified base.
    /// </summary>
    /// <param name="nm">The full path and name of the source base.</param>
    /// <param name="trgnm">The full path and name of the target base.</param>
    /// <param name="year">The first year to include.</param>
    /// <param name="cb">Callback function to report on progress.</param>
    /// <returns>Nothing.</returns>
    let ExtractNewer nm trgnm year cb =
        GrampusInternal.Games.ExtractNewer nm trgnm year cb
    
    /// <summary>
    /// Extracts stronger games to the specified base.
    /// </summary>
    /// <param name="nm">The full path and name of the source base.</param>
    /// <param name="trgnm">The full path and name of the target base.</param>
    /// <param name="grade">The minimum grade to include.</param>
    /// <param name="cb">Callback function to report on progress.</param>
    /// <returns>Nothing.</returns>
    let ExtractStronger nm trgnm grade cb =
        GrampusInternal.Games.ExtractStronger nm trgnm grade cb
    
    /// <summary>
    /// Gets possible names of Players given part of the name string.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="part">The string containing part of the name.</param>
    /// <param name="cb">Callback function to report on progress.</param>
    /// <returns>Possible names as a string array type.</returns>
    let GetPossNames nm cb = GrampusInternal.Games.GetPossNames nm cb
    
    /// <summary>
    /// Extracts player's games to the specified base.
    /// </summary>
    /// <param name="nm">The full path and name of the source base.</param>
    /// <param name="trgnm">The full path and name of the target base.</param>
    /// <param name="player">The name of the player to include.</param>
    /// <param name="cb">Callback function to report on progress.</param>
    /// <returns>Nothing.</returns>
    let ExtractPlayer nm trgnm player cb =
        GrampusInternal.Games.ExtractPlayer nm trgnm player cb
    
    /// <summary>
    /// Removes duplicate games for the base in the specified folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="cb">Callback function to report on progress.</param>
    /// <returns>Messages from process as a string type.</returns>
    let RemoveDuplicates nm cb = GrampusInternal.Games.RemoveDuplicates nm cb
    
    /// <summary>
    /// Removes all Comments for the base in the specified folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="cb">Callback function to report on progress.</param>
    /// <returns>Nothing.</returns>
    let RemoveComments nm cb = GrampusInternal.Games.RemoveComments nm cb
    
    /// <summary>
    /// Removes all Variations for the base in the specified folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="cb">Callback function to report on progress.</param>
    /// <returns>Nothing.</returns>
    let RemoveRavs nm cb = GrampusInternal.Games.RemoveRavs nm cb
    
    /// <summary>
    /// Removes all NAGs for the base in the specified folder.
    /// </summary>
    /// <param name="nm">The full path and file name.</param>
    /// <param name="cb">Callback function to report on progress.</param>
    /// <returns>Nothing.</returns>
    let RemoveNags nm cb = GrampusInternal.Games.RemoveNags nm cb

/// <summary>
/// Holds the functions related to a set of Games in PGN format.
/// </summary>
module PgnGames =
    /// <summary>
    /// Gets a Sequence of Games from a PGN file.
    /// </summary>
    /// <param name="fn">The path and file name of the PGN file.</param>
    /// <returns>The games as an UnencodeGame sequence.</returns>
    let ReadSeqFromFile(fn) = GrampusInternal.PgnGames.ReadSeqFromFile(fn)
    
    /// <summary>
    /// Gets the Number of Games from a PGN file.
    /// </summary>
    /// <param name="fn">The path and file name of the PGN file.</param>
    /// <returns>The number of games as an int type.</returns>
    let GetNumberOfGames(fn) = GrampusInternal.PgnGames.GetNumberOfGames(fn)
