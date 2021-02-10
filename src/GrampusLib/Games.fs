namespace GrampusInternal

open System
open Grampus
open System.IO
open MessagePack
open MessagePack.Resolvers
open MessagePack.FSharp

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module Games =
    let resolver =
        Resolvers.CompositeResolver.Create
            (FSharpResolver.Instance, StandardResolver.Instance)
    let options = MessagePackSerializerOptions.Standard.WithResolver(resolver)
    
    let LoadGame (nm : string) (ie : IndexEntry) (hdr : Header) =
        let fol = getbinfol nm
        let fn = Path.Combine(fol, "GAMES")
        use reader =
            new BinaryReader(File.Open
                                 (fn, FileMode.Open, FileAccess.Read, 
                                  FileShare.Read))
        reader.BaseStream.Position <- ie.Offset
        let bin = reader.ReadBytes(ie.Length)
        let ro = new ReadOnlyMemory<byte>(bin)
        let gm = MessagePackSerializer.Deserialize<CompressedGame>(ro, options)
        (gm, hdr) |> GameEncoded.Expand
    
    let Save (nm : string) (gms : seq<EncodedGame>) cb =
        let fol = getbinfol nm
        Directory.CreateDirectory(fol) |> ignore
        let fn = Path.Combine(fol, "GAMES")
        use writer = new BinaryWriter(File.Open(fn, FileMode.OpenOrCreate))
        writer.Seek(0, SeekOrigin.Begin) |> ignore
        let svgm i gm =
            let cgm = gm |> GameEncoded.Compress
            let off = writer.BaseStream.Position
            let bin =
                MessagePackSerializer.Serialize<CompressedGame>(cgm, options)
            if i % 100 = 0 then cb (i)
            writer.Write(bin)
            { Offset = off
              Length = bin.Length }, gm.Hdr
        
        let iea, hdrs =
            gms
            |> Seq.mapi svgm
            |> Seq.toArray
            |> Array.unzip
        
        Index.Save(nm, iea)
        Headers.Save(nm, hdrs)
    
    let Add (nm : string) (gma : seq<EncodedGame>) cb =
        let fol = getbinfol nm
        let fn = Path.Combine(fol, "GAMES")
        use writer = new BinaryWriter(File.Open(fn, FileMode.OpenOrCreate))
        writer.Seek(0, SeekOrigin.End) |> ignore
        let iea = Index.Load nm
        let hdrs = Headers.Load nm
        let ct = iea.Length
        
        let svgm i gm =
            let cgm = gm |> GameEncoded.Compress
            let off = writer.BaseStream.Position
            let bin =
                MessagePackSerializer.Serialize<CompressedGame>(cgm, options)
            if i % 100 = 0 then cb (i)
            writer.Write(bin)
            { Offset = off
              Length = bin.Length }, { gm.Hdr with Num = i + ct }
        
        let aiea, ahdrs =
            gma
            |> Seq.mapi svgm
            |> Seq.toArray
            |> Array.unzip
        
        let niea = Array.append iea aiea
        let nhdrs = Array.append hdrs ahdrs
        Index.Save(nm, niea)
        Headers.Save(nm, nhdrs)
    
    let AddGmp nm addnm cb = ()
    
    let AppendGame (nm : string) (gm : EncodedGame) =
        let fol = getbinfol nm
        let cgm = gm |> GameEncoded.Compress
        Directory.CreateDirectory(fol) |> ignore
        let fn = Path.Combine(fol, "GAMES")
        use writer = new BinaryWriter(File.Open(fn, FileMode.OpenOrCreate))
        writer.Seek(0, SeekOrigin.End) |> ignore
        let off = writer.BaseStream.Position
        let bin = MessagePackSerializer.Serialize<CompressedGame>(cgm, options)
        writer.Write(bin)
        let ie =
            { Offset = off
              Length = bin.Length }
        
        let iea = Index.Load nm
        let hdrs = Headers.Load nm
        let hdr = { gm.Hdr with Num = iea.Length }
        let niea = Array.append iea [| ie |]
        let nhdrs = Array.append hdrs [| hdr |]
        Index.Save(nm, niea)
        Headers.Save(nm, nhdrs)
    
    let UpdateGame (nm : string) (gnum : int) (gm : EncodedGame) =
        let fol = getbinfol nm
        let cgm = gm |> GameEncoded.Compress
        let fn = Path.Combine(fol, "GAMES")
        use writer = new BinaryWriter(File.Open(fn, FileMode.OpenOrCreate))
        writer.Seek(0, SeekOrigin.End) |> ignore
        let off = writer.BaseStream.Position
        let bin = MessagePackSerializer.Serialize<CompressedGame>(cgm, options)
        writer.Write(bin)
        let ie =
            { Offset = off
              Length = bin.Length }
        
        let hdr = gm.Hdr
        let iea = Index.Load nm
        let hdrs = Headers.Load nm
        iea.[gnum] <- ie
        hdrs.[gnum] <- hdr
        Index.Save(nm, iea)
        Headers.Save(nm, hdrs)
    
    let Compact (nm : string) cb =
        let fol = getbinfol nm
        let ifn = Path.Combine(fol, "GAMES")
        if not (File.Exists(ifn)) then 
            "Compaction Terminated: No Games to compact."
        else 
            let mutable msg = ""
            //create temp folder to do the compact
            let tmp = Path.Combine(fol,"temp.grampus")
            let tmpfol = getbinfol tmp
            Directory.CreateDirectory(tmpfol) |> ignore
            let ofn = Path.Combine(tmpfol, "GAMES")
            use writer = new BinaryWriter(File.Open(ofn, FileMode.OpenOrCreate))
            //load all the current files
            use reader =
                new BinaryReader(File.Open
                                     (ifn, FileMode.Open, FileAccess.Read, 
                                      FileShare.Read))
            let iea = Index.Load nm
            let hdrs = Headers.Load nm
            
            //write in compacted format
            let svgm i =
                let ie = iea.[i]
                let hdr = hdrs.[i]
                let keep = hdr.Deleted <> "D"
                if keep then 
                    reader.BaseStream.Position <- ie.Offset
                    let bin = reader.ReadBytes(ie.Length)
                    let off = writer.BaseStream.Position
                    if i % 100 = 0 then cb (i)
                    writer.Write(bin)
                    keep, 
                    { Offset = off
                      Length = bin.Length }, hdr
                else 
                    keep, 
                    { Offset = 0L
                      Length = 0 }, hdr
            
            let kihs = [| 0..iea.Length - 1 |] |> Array.map svgm
            
            let niea, nhdrs =
                kihs
                |> Array.filter (fun (k, i, h) -> k)
                |> Array.map (fun (k, i, h) -> i, h)
                |> Array.unzip
            
            let del = iea.Length - niea.Length
            msg <- msg + "Number of games permanently deleted is: " 
                   + del.ToString()
            Index.Save(tmp, niea)
            Headers.Save(tmp, nhdrs)
            reader.Close()
            writer.Close()
            //now overwrite with compacted versions
            File.Move(ofn, ifn, true)
            File.Move
                (Path.Combine(tmpfol, "INDEX"), Path.Combine(fol, "INDEX"), true)
            File.Move
                (Path.Combine(tmpfol, "HEADERS"), Path.Combine(fol, "HEADERS"), 
                 true)
            msg
    
    let ExtractNewer (nm : string) (trgnm : string) (year : int) cb =
        let trggmp = GrampusFile.New(trgnm)
        let hdrs = Headers.Load nm
        let iea = Index.Load nm
        
        let svgm i hdr =
            if hdr.Year >= year then 
                let gm = LoadGame nm iea.[i] hdr
                AppendGame trgnm gm
            if i % 100 = 0 then cb (i)
        hdrs |> Array.iteri svgm
        let ntrggmp = { trggmp with BaseCreated = Some(DateTime.Now) }
        GrampusFile.Save(trgnm, ntrggmp)
    
    let ExtractStronger (nm : string) (trgnm : string) (grade : int) cb =
        let trggmp = GrampusFile.New(trgnm)
        let hdrs = Headers.Load nm
        let iea = Index.Load nm
        
        let svgm i hdr =
            let w = hdr.W_Elo
            
            let welo =
                if w = "-" || w = "" then 0
                else int (w)
            
            let b = hdr.B_Elo
            
            let belo =
                if b = "-" || b = "" then 0
                else int (b)
            if welo >= grade || belo >= grade then 
                let gm = LoadGame nm iea.[i] hdr
                AppendGame trgnm gm
            if i % 100 = 0 then cb (i)
        hdrs |> Array.iteri svgm
        let ntrggmp = { trggmp with BaseCreated = Some(DateTime.Now) }
        GrampusFile.Save(trgnm, ntrggmp)
    
    let GetPossNames (nm : string) (ipart : string) cb =
        let fol = getbinfol nm
        let part = ipart.ToLower()
        
        let getnms i (hdr : Header) =
            let wnm =
                if hdr.White.ToLower().Contains(part) then Some(hdr.White)
                else None
            
            let bnm =
                if hdr.Black.ToLower().Contains(part) then Some(hdr.Black)
                else None
            
            if i % 100 = 0 then cb (i)
            wnm, bnm
        
        let hdrs = Headers.Load nm
        
        let wnms, bnms =
            hdrs
            |> Array.mapi getnms
            |> Array.unzip
        
        let wset =
            wnms
            |> Array.filter (fun w -> w.IsSome)
            |> Array.map (fun w -> w.Value)
            |> Set.ofArray
        
        let bset =
            bnms
            |> Array.filter (fun b -> b.IsSome)
            |> Array.map (fun b -> b.Value)
            |> Set.ofArray
        
        let nms = wset + bset
        nms |> Set.toArray
    
    let ExtractPlayer (nm : string) (trgnm : string) (player : string) cb =
        let trggmp = GrampusFile.New(trgnm)
        let hdrs = Headers.Load nm
        let iea = Index.Load nm
        
        let svgm i hdr =
            if hdr.White = player || hdr.Black = player then 
                let gm = LoadGame nm iea.[i] hdr
                AppendGame trgnm gm
            if i % 100 = 0 then cb (i)
        hdrs |> Array.iteri svgm
        let ntrggmp = { trggmp with BaseCreated = Some(DateTime.Now) }
        GrampusFile.Save(trgnm, ntrggmp)
    
    let RemoveDuplicates (nm : string) cb =
        let fol = getbinfol nm
        let ifn = Path.Combine(fol, "GAMES")
        if not (File.Exists(ifn)) then 
            "Process Terminated: No Games to process."
        else 
            //will marke duplicates as D and then call compact
            //load all the current files
            use reader =
                new BinaryReader(File.Open
                                     (ifn, FileMode.Open, FileAccess.Read, 
                                      FileShare.Read))
            let iea = Index.Load nm
            let hdrs = Headers.Load nm
            let numgames = iea.Length
            
            //find duplicates
            let findgm i =
                let hdr = hdrs.[i]
                let skip = hdr.Deleted = "D" || i = numgames - 1
                if not skip then 
                    let w = hdr.White
                    let b = hdr.Black
                    let ie = iea.[i]
                    let gm = LoadGame nm ie hdr
                    let n = gm.MoveText.Length
                    for j = i + 1 to numgames - 1 do
                        let hdr2 = hdrs.[j]
                        if w = hdr2.White && b = hdr2.Black then 
                            let ie2 = iea.[j]
                            let gm2 = LoadGame nm ie2 hdr2
                            if n = gm2.MoveText.Length then 
                                hdrs.[j] <- { hdr2 with Deleted = "D" }
                    if i % 100 = 0 then cb (i)
            [| 0..numgames - 1 |] |> Array.iter findgm
            Headers.Save(nm, hdrs)
            reader.Close()
            //now compact
            let msg = Compact nm cb
            msg
    
    let RemoveComments (nm : string) cb =
        let fol = getbinfol nm
        let ifn = Path.Combine(fol, "GAMES")
        if (File.Exists(ifn)) then 
            //create temp folder to do the compact
            let tmp = Path.Combine(fol,"temp.grampus")
            let tmpfol = getbinfol tmp
            Directory.CreateDirectory(tmpfol) |> ignore
            let ofn = Path.Combine(tmpfol, "GAMES")
            use writer = new BinaryWriter(File.Open(ofn, FileMode.OpenOrCreate))
            //load all the current files
            use reader =
                new BinaryReader(File.Open
                                     (ifn, FileMode.Open, FileAccess.Read, 
                                      FileShare.Read))
            let iea = Index.Load nm
            let hdrs = Headers.Load nm
            
            //write in compacted format
            let svgm i =
                let ie = iea.[i]
                let hdr = hdrs.[i]
                reader.BaseStream.Position <- ie.Offset
                let off = writer.BaseStream.Position
                if i % 100 = 0 then cb (i)
                let bin = reader.ReadBytes(ie.Length)
                let ro = new ReadOnlyMemory<byte>(bin)
                let cgm =
                    MessagePackSerializer.Deserialize<CompressedGame>
                        (ro, options)
                let ncgm = GameCompressed.RemoveComments cgm
                if ncgm.MoveText.Length < cgm.MoveText.Length then 
                    let nbin =
                        MessagePackSerializer.Serialize<CompressedGame>
                            (ncgm, options)
                    writer.Write(nbin)
                    { Offset = off
                      Length = nbin.Length }, hdr
                else 
                    writer.Write(bin)
                    { Offset = off
                      Length = bin.Length }, hdr
            
            let niea, nhdrs =
                [| 0..iea.Length - 1 |]
                |> Array.map svgm
                |> Array.unzip
            
            Index.Save(tmp, niea)
            Headers.Save(tmp, nhdrs)
            reader.Close()
            writer.Close()
            //now overwrite with compacted versions
            File.Move(ofn, ifn, true)
            File.Move
                (Path.Combine(tmpfol, "INDEX"), Path.Combine(fol, "INDEX"), true)
            File.Move
                (Path.Combine(tmpfol, "HEADERS"), Path.Combine(fol, "HEADERS"), 
                 true)
    
    let RemoveRavs (nm : string) cb =
        let fol = getbinfol nm
        let ifn = Path.Combine(fol, "GAMES")
        if (File.Exists(ifn)) then 
            //create temp folder to do the compact
            let tmp = Path.Combine(fol,"temp.grampus")
            let tmpfol = getbinfol tmp
            Directory.CreateDirectory(tmpfol) |> ignore
            let ofn = Path.Combine(tmpfol, "GAMES")
            use writer = new BinaryWriter(File.Open(ofn, FileMode.OpenOrCreate))
            //load all the current files
            use reader =
                new BinaryReader(File.Open
                                     (ifn, FileMode.Open, FileAccess.Read, 
                                      FileShare.Read))
            let iea = Index.Load nm
            let hdrs = Headers.Load nm
            
            //write in compacted format
            let svgm i =
                let ie = iea.[i]
                let hdr = hdrs.[i]
                reader.BaseStream.Position <- ie.Offset
                let off = writer.BaseStream.Position
                if i % 100 = 0 then cb (i)
                let bin = reader.ReadBytes(ie.Length)
                let ro = new ReadOnlyMemory<byte>(bin)
                let cgm =
                    MessagePackSerializer.Deserialize<CompressedGame>
                        (ro, options)
                let ncgm = GameCompressed.RemoveRavs cgm
                if ncgm.MoveText.Length < cgm.MoveText.Length then 
                    let nbin =
                        MessagePackSerializer.Serialize<CompressedGame>
                            (ncgm, options)
                    writer.Write(nbin)
                    { Offset = off
                      Length = nbin.Length }, hdr
                else 
                    writer.Write(bin)
                    { Offset = off
                      Length = bin.Length }, hdr
            
            let niea, nhdrs =
                [| 0..iea.Length - 1 |]
                |> Array.map svgm
                |> Array.unzip
            
            Index.Save(tmp, niea)
            Headers.Save(tmp, nhdrs)
            reader.Close()
            writer.Close()
            //now overwrite with compacted versions
            File.Move(ofn, ifn, true)
            File.Move
                (Path.Combine(tmpfol, "INDEX"), Path.Combine(fol, "INDEX"), true)
            File.Move
                (Path.Combine(tmpfol, "HEADERS"), Path.Combine(fol, "HEADERS"), 
                 true)
    
    let RemoveNags (nm : string) cb =
        let fol = getbinfol nm
        let ifn = Path.Combine(fol, "GAMES")
        if (File.Exists(ifn)) then 
            //create temp folder to do the compact
            let tmp = Path.Combine(fol,"temp.grampus")
            let tmpfol = getbinfol tmp
            Directory.CreateDirectory(tmpfol) |> ignore
            let ofn = Path.Combine(tmpfol, "GAMES")
            use writer = new BinaryWriter(File.Open(ofn, FileMode.OpenOrCreate))
            //load all the current files
            use reader =
                new BinaryReader(File.Open
                                     (ifn, FileMode.Open, FileAccess.Read, 
                                      FileShare.Read))
            let iea = Index.Load nm
            let hdrs = Headers.Load nm
            
            //write in compacted format
            let svgm i =
                let ie = iea.[i]
                let hdr = hdrs.[i]
                reader.BaseStream.Position <- ie.Offset
                let off = writer.BaseStream.Position
                if i % 100 = 0 then cb (i)
                let bin = reader.ReadBytes(ie.Length)
                let ro = new ReadOnlyMemory<byte>(bin)
                let cgm =
                    MessagePackSerializer.Deserialize<CompressedGame>
                        (ro, options)
                let ncgm = GameCompressed.RemoveNags cgm
                if ncgm.MoveText.Length < cgm.MoveText.Length then 
                    let nbin =
                        MessagePackSerializer.Serialize<CompressedGame>
                            (ncgm, options)
                    writer.Write(nbin)
                    { Offset = off
                      Length = nbin.Length }, hdr
                else 
                    writer.Write(bin)
                    { Offset = off
                      Length = bin.Length }, hdr
            
            let niea, nhdrs =
                [| 0..iea.Length - 1 |]
                |> Array.map svgm
                |> Array.unzip
            
            Index.Save(tmp, niea)
            Headers.Save(tmp, nhdrs)
            reader.Close()
            writer.Close()
            //now overwrite with compacted versions
            File.Move(ofn, ifn, true)
            File.Move
                (Path.Combine(tmpfol, "INDEX"), Path.Combine(fol, "INDEX"), true)
            File.Move
                (Path.Combine(tmpfol, "HEADERS"), Path.Combine(fol, "HEADERS"), 
                 true)
