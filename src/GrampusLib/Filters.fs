namespace GrampusInternal

open System
open System.IO
open MessagePack
open LevelDB
open MessagePack.Resolvers
open MessagePack.FSharp

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module Filters =
    let resolver =
        Resolvers.CompositeResolver.Create
            (FSharpResolver.Instance, StandardResolver.Instance)
    let options = MessagePackSerializerOptions.Standard.WithResolver(resolver)
    
    let Create(ifol) =
        let fol = ifol + "\\filters"
        Directory.CreateDirectory(fol) |> ignore
        let opts = new Options(CreateIfMissing = true)
        let db = new DB(opts, fol)
        db.Close()
    
    let Save(posns : string [], filts : int list [], ifol : string) =
        let fol = ifol + "\\filters"
        let opts = new Options()
        let db = new DB(opts, fol)
        for i = 0 to posns.Length - 1 do
            db.Put
                (MessagePackSerializer.Serialize<string>(posns.[i]), 
                 MessagePackSerializer.Serialize<int list>(filts.[i], options))
        db.Close()
    
    let ReadArray(posns : string [], ifol : string) =
        let fol = ifol + "\\filters"
        let opts = new Options()
        let db = new DB(opts, fol)
        
        let getv (posn : string) =
            let v = db.Get(MessagePackSerializer.Serialize<string>(posn))
            match v with
            |null -> []
            |_ ->
                let filts =
                    let ro = new ReadOnlyMemory<byte>(v)
                    MessagePackSerializer.Deserialize<int list>(ro, options)
                filts
        
        let vs = posns |> Array.map getv
        db.Close()
        vs
    
    let Read(posn : string, ifol : string) =
        let fol = ifol + "\\filters"
        if Directory.Exists(fol) then 
            let opts = new Options()
            let db = new DB(opts, fol)
            let v = db.Get(MessagePackSerializer.Serialize<string>(posn))
            let filts =
                match v with
                |null -> []
                |_ ->
                    let ro = new ReadOnlyMemory<byte>(v)
                    MessagePackSerializer.Deserialize<int list>(ro, options)
            db.Close()
            filts
        else []
