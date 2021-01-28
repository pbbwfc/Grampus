namespace FsChessPgn

open System
open System.IO
open FsChess
open MessagePack
open LevelDB

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module StaticTree =

    let Create(ifol) =
        let fol = ifol + "\\trees"
        Directory.CreateDirectory(fol)|>ignore
        let options = new Options(CreateIfMissing = true)
        let db = new DB(options, fol)
        db.Close()
        
    let Save(posns:string[],stss:stats[],ifol:string) =
        let fol = ifol + "\\trees"
        let options = new Options()
        let db = new DB(options, fol)
        for i = 0 to posns.Length-1 do
            db.Put(MessagePackSerializer.Serialize<string>(posns.[i]),MessagePackSerializer.Serialize<stats>(stss.[i]))
        db.Close()

    let ReadArray(posns:string[],ifol:string) =
        let fol = ifol + "\\trees"
        let options = new Options()
        let db = new DB(options, fol)
        let getv (posn:string) =
            let v = db.Get(MessagePackSerializer.Serialize<string>(posn))
            let ro = new ReadOnlyMemory<byte>(v)
            MessagePackSerializer.Deserialize<stats>(ro)
        let vs = posns|>Array.map getv
        db.Close()
        vs
   
    let Read(posn:string,ifol:string) =
        let fol = ifol + "\\trees"
        if Directory.Exists(fol) then
            let options = new Options()
            let db = new DB(options, fol)
            let v = db.Get(MessagePackSerializer.Serialize<string>(posn))
            let sts =
                let ro = new ReadOnlyMemory<byte>(v)
                MessagePackSerializer.Deserialize<stats>(ro)
            db.Close()
            sts
        else new stats()
    
        