namespace FsChessPgn

open System
open FsChess
open System.IO
open MessagePack
open MessagePack.Resolvers
open MessagePack.FSharp

module Games =
    
    let resolver =
        Resolvers.CompositeResolver.Create(FSharpResolver.Instance,StandardResolver.Instance)
    let options = MessagePackSerializerOptions.Standard.WithResolver(resolver)

    let LoadGame (fol:string) (ie:IndexEntry) =
        let fn = Path.Combine(fol,"GAMES")
        use reader = new BinaryReader(File.Open(fn, FileMode.Open, FileAccess.Read, FileShare.Read))
        reader.BaseStream.Position <- ie.Offset
        let bin = reader.ReadBytes(ie.Length)
        let ro = new ReadOnlyMemory<byte>(bin)
        let gm = MessagePackSerializer.Deserialize<CompressedGame>(ro,options)
        gm|>GameEncoded.Expand
    
    let Save (fol:string) (offset:int64) (gma:seq<CompressedGame>) =
        Directory.CreateDirectory(fol)|>ignore
        let fn = Path.Combine(fol,"GAMES")
        use writer = new BinaryWriter(File.Open(fn, FileMode.OpenOrCreate))
        writer.Seek(int(offset),SeekOrigin.Begin)|>ignore
        let svgm i gm =
            let off = writer.BaseStream.Position
            let bin = MessagePackSerializer.Serialize<CompressedGame>(gm,options)
            if i%1000=0 then printf "%i..." i
            writer.Write(bin)
            {Offset=off;Length=bin.Length}, GameRows.FromGame i gm
        let iea,gmrws = gma|>Seq.mapi svgm|>Seq.toArray|>Array.unzip
        Index.Save(fol,iea)
        GameRows.Save(fol,gmrws)

    let AppendGame (fol:string) (gnum:int) (cgm:CompressedGame) =
        Directory.CreateDirectory(fol)|>ignore
        let fn = Path.Combine(fol,"GAMES")
        use writer = new BinaryWriter(File.Open(fn, FileMode.OpenOrCreate))
        writer.Seek(0,SeekOrigin.End)|>ignore
        let off = writer.BaseStream.Position
        let bin = MessagePackSerializer.Serialize<CompressedGame>(cgm,options)
        writer.Write(bin)
        let ie = {Offset=off;Length=bin.Length}
        let gmrw = GameRows.FromGame gnum cgm
        let iea = Index.Load(fol)
        let gmrws = GameRows.Load(fol)
        let niea = Array.append iea [|ie|]
        let ngmrws = Array.append gmrws [|gmrw|]
        Index.Save(fol,niea)
        GameRows.Save(fol,ngmrws)
    
        