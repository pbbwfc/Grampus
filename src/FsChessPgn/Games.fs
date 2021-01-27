﻿namespace FsChessPgn

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

    let LoadGame (fol:string) (ie:IndexEntry) (hdr:Header) =
        let fn = Path.Combine(fol,"GAMES")
        use reader = new BinaryReader(File.Open(fn, FileMode.Open, FileAccess.Read, FileShare.Read))
        reader.BaseStream.Position <- ie.Offset
        let bin = reader.ReadBytes(ie.Length)
        let ro = new ReadOnlyMemory<byte>(bin)
        let gm = MessagePackSerializer.Deserialize<CompressedGame>(ro,options)
        (gm,hdr)|>GameEncoded.Expand
    
    let Save (fol:string) (offset:int64) (gma:seq<EncodedGame>) =
        Directory.CreateDirectory(fol)|>ignore
        let fn = Path.Combine(fol,"GAMES")
        use writer = new BinaryWriter(File.Open(fn, FileMode.OpenOrCreate))
        writer.Seek(int(offset),SeekOrigin.Begin)|>ignore
        let svgm i gm =
            let cgm = gm|>GameEncoded.Compress
            let off = writer.BaseStream.Position
            let bin = MessagePackSerializer.Serialize<CompressedGame>(cgm,options)
            if i%1000=0 then printf "%i..." i
            writer.Write(bin)
            {Offset=off;Length=bin.Length}, gm.Hdr
        let iea,hdrs = gma|>Seq.mapi svgm|>Seq.toArray|>Array.unzip
        Index.Save(fol,iea)
        Headers.Save(fol,hdrs)

    let AppendGame (fol:string) (gnum:int) (gm:EncodedGame) =
        let cgm = gm|>GameEncoded.Compress
        Directory.CreateDirectory(fol)|>ignore
        let fn = Path.Combine(fol,"GAMES")
        use writer = new BinaryWriter(File.Open(fn, FileMode.OpenOrCreate))
        writer.Seek(0,SeekOrigin.End)|>ignore
        let off = writer.BaseStream.Position
        let bin = MessagePackSerializer.Serialize<CompressedGame>(cgm,options)
        writer.Write(bin)
        let ie = {Offset=off;Length=bin.Length}
        let hdr = gm.Hdr
        let iea = Index.Load(fol)
        let hdrs = Headers.Load(fol)
        let niea = Array.append iea [|ie|]
        let nhdrs = Array.append hdrs [|hdr|]
        Index.Save(fol,niea)
        Headers.Save(fol,nhdrs)
    
    let UpdateGame (fol:string) (gnum:int) (gm:EncodedGame) =
        let cgm = gm|>GameEncoded.Compress
        let fn = Path.Combine(fol,"GAMES")
        use writer = new BinaryWriter(File.Open(fn, FileMode.OpenOrCreate))
        writer.Seek(0,SeekOrigin.End)|>ignore
        let off = writer.BaseStream.Position
        let bin = MessagePackSerializer.Serialize<CompressedGame>(cgm,options)
        writer.Write(bin)
        let ie = {Offset=off;Length=bin.Length}
        let hdr = gm.Hdr
        let iea = Index.Load(fol)
        let hdrs = Headers.Load(fol)
        iea.[gnum] <- ie
        hdrs.[gnum] <- hdr
        Index.Save(fol,iea)
        Headers.Save(fol,hdrs)

    let Compact (fol:string) =
        //create temp folder to do the compact
        let tmpfol = Path.Combine(fol,"temp")
        Directory.CreateDirectory(tmpfol)|>ignore
        let ofn = Path.Combine(tmpfol,"GAMES")
        use writer = new BinaryWriter(File.Open(ofn, FileMode.OpenOrCreate))
        //load all the current files
        let ifn = Path.Combine(fol,"GAMES")
        use reader = new BinaryReader(File.Open(ifn, FileMode.Open, FileAccess.Read, FileShare.Read))
        let iea = Index.Load(fol)
        let hdrs = Headers.Load(fol)
        //write in compacted format
        let svgm i =
            let ie = iea.[i]
            let hdr = hdrs.[i]
            let keep = hdr.Deleted<>"D"
            if keep then
                reader.BaseStream.Position <- ie.Offset
                let bin = reader.ReadBytes(ie.Length)
                let off = writer.BaseStream.Position
                if i%1000=0 then printf "%i..." i
                writer.Write(bin)
                keep,{Offset=off;Length=bin.Length}, hdr
            else keep,{Offset=0L;Length=0}, hdr
        let kihs = [|0..iea.Length-1|]|>Array.map svgm
        let niea,nhdrs = 
            kihs|>Array.filter(fun (k,i,h) -> k)
            |>Array.map(fun (k,i,h) -> i,h)
            |>Array.unzip
        Index.Save(tmpfol,niea)
        Headers.Save(tmpfol,nhdrs)
        reader.Close()
        writer.Close()
        //now overwrite with compacted versions
        File.Move(ofn,ifn,true)
        File.Move(Path.Combine(tmpfol,"INDEX"),Path.Combine(fol,"INDEX"),true)
        File.Move(Path.Combine(tmpfol,"ROWS"),Path.Combine(fol,"ROWS"),true)
        