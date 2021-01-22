namespace FsChess.Pgn

module Games =

    ///Get an array of Games from a file
    let ReadArrayFromFile = FsChessPgn.PgnGames.ReadArrayFromFile
    
    ///Get a list of Games from a file
    let ReadListFromFile = FsChessPgn.PgnGames.ReadListFromFile
    
    ///Get a list of index * Game from a file
    let ReadIndexListFromFile = FsChessPgn.PgnGames.ReadIndexListFromFile

    ///Get a Sequence of Games from a file
    let ReadSeqFromFile = FsChessPgn.PgnGames.ReadSeqFromFile

    ///Write a list of Games to a file
    let WriteFile = FsChessPgn.PgnWriter.WriteFile

    ///Encodes a sequence of Unencoded Games
    let Encode = FsChessPgn.PgnGames.Encode


    