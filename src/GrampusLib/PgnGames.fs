namespace GrampusInternal

open Grampus
open System.IO
open System.Text

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module PgnGames =
    let ReadFromStream(stream : Stream) =
        let sr = new StreamReader(stream)
        let db = RegParse.AllGamesRdr(sr)
        db
    
    let ReadSeqFromFile(fn : string) =
        let stream =
            new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.Read)
        let db = ReadFromStream(stream)
        db
    
    let ReadArrayFromFile(file : string) =
        let stream = new FileStream(file, FileMode.Open)
        let result = ReadFromStream(stream) |> Seq.toArray
        stream.Close()
        result
    
    let ReadListFromFile(file : string) =
        let stream = new FileStream(file, FileMode.Open)
        let result = ReadFromStream(stream) |> Seq.toList
        stream.Close()
        result
    
    let ReadIndexListFromFile(file : string) =
        ReadListFromFile(file) |> List.indexed
    
    let ReadFromString(str : string) =
        let byteArray = Encoding.ASCII.GetBytes(str)
        let stream = new MemoryStream(byteArray)
        let result = ReadFromStream(stream) |> Seq.toList
        stream.Close()
        result
    
    let ReadOneFromString(str : string) =
        let gms = str |> ReadFromString
        gms.Head
    
    let Encode gms =
        gms
        |> Seq.toArray
        |> Array.Parallel.map GameEncoded.Encode
