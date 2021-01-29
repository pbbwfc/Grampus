namespace GrampusInternal

open Grampus
open System.IO

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module PgnWriter =

    let WriteStream(stream:Stream,pgnDatabase:UnencodedGame list) =
        use writer = new StreamWriter(stream)
        for game in pgnDatabase do
            PgnWrite.Game(game, writer)
            writer.WriteLine()

    let WriteFile (file:string) (pgnDatabase:UnencodedGame list) =
        let stream = new FileStream(file, FileMode.Create)
        WriteStream(stream,pgnDatabase)

    let WriteString(pgnDatabase:UnencodedGame list) =
        use writer = new StringWriter()
        for game in pgnDatabase do
            PgnWrite.Game(game, writer)
            writer.WriteLine()
        writer.ToString()
 