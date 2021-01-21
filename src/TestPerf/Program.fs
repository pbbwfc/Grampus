// Learn more about F# at http://fsharp.org

open System
open System.IO
open FsChess
open MessagePack
open MessagePack.Resolvers
open MessagePack.FSharp
//open MBrace.FsPickler

[<EntryPoint>]
let main argv =
    let tstfol = 
        let pth = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ScincNet\\test")
        Directory.CreateDirectory(pth)|>ignore
        pth
    let mutable st = DateTime.Now
    let mutable nd = DateTime.Now
    let logtime() = 
        let el0 = nd-st
        let el = float(el0.TotalMilliseconds)/1000.0
        printfn "Elapsed time %f seconds" el
    let resolver =
        Resolvers.CompositeResolver.Create(FSharpResolver.Instance,StandardResolver.Instance)
    let options = MessagePackSerializerOptions.Standard.WithResolver(resolver)

    let SaveGame (fol:string) (offset:int64) (gm:CompressedGame) =
        Directory.CreateDirectory(fol)|>ignore
        let bin = MessagePackSerializer.Serialize<CompressedGame>(gm,options)
        let fn = Path.Combine(fol,"GAMES")
        use writer = new BinaryWriter(File.Open(fn, FileMode.OpenOrCreate))
        writer.Seek(int(offset),SeekOrigin.Begin)|>ignore
        writer.Write(bin)
        {Offset=offset;Length=bin.Length},writer.BaseStream.Position

    let SaveGames (fol:string) (offset:int64) (gma:seq<CompressedGame>) =
        Directory.CreateDirectory(fol)|>ignore
        let fn = Path.Combine(fol,"GAMES")
        use writer = new BinaryWriter(File.Open(fn, FileMode.OpenOrCreate))
        writer.Seek(int(offset),SeekOrigin.Begin)|>ignore
        let svgm i gm =
            let off = writer.BaseStream.Position
            let bin = MessagePackSerializer.Serialize<CompressedGame>(gm,options)
            if i%1000=0 then printf "%i..." i
            writer.Write(bin)
            {Offset=off;Length=bin.Length}
        let iea = gma|>Seq.mapi svgm
        let ifn = Path.Combine(fol,"INDEX")
        let bin = MessagePackSerializer.Serialize<IndexEntry[]>(iea|>Seq.toArray,options)
        File.WriteAllBytes(ifn,bin)

    let LoadIndex (fol:string) =
        let fn = Path.Combine(fol,"INDEX")
        let bin = File.ReadAllBytes(fn)
        let ro = new ReadOnlyMemory<byte>(bin)
        let iea = MessagePackSerializer.Deserialize<IndexEntry[]>(ro,options)
        iea
    
    let LoadGame (fol:string) (ie:IndexEntry) =
        let fn = Path.Combine(fol,"GAMES")
        use reader = new BinaryReader(File.Open(fn, FileMode.Open, FileAccess.Read, FileShare.Read))
        reader.BaseStream.Position <- ie.Offset
        let bin = reader.ReadBytes(ie.Length)
        let ro = new ReadOnlyMemory<byte>(bin)
        let gm = MessagePackSerializer.Deserialize<EncodedGame>(ro,options)
        gm

    //TODO try compressedgame..
    let nm = "recent10000"
    //let nm = "fritz"
    //let nm = "fritz1"
    //let nm = "one"
    let pgn = Path.Combine(tstfol,nm + ".pgn")
    let binfol = Path.Combine(tstfol,nm + "_FILES")
    printfn "Importing games for: %s" nm 
    st <- DateTime.Now
    let ugma = Pgn.Games.ReadSeqFromFile pgn
    //printfn "Number of games: %i" ugma.Length
    nd <- DateTime.Now
    logtime()
    printfn "Encoding games for: %s" nm 
    st <- DateTime.Now
    //let gma = ugma|>Pgn.Games.Encode
    let egma = ugma|>Seq.map(Game.Encode)
    nd <- DateTime.Now
    logtime()
    printfn "Compressing games for: %s" nm 
    st <- DateTime.Now
    //let gma = ugma|>Pgn.Games.Encode
    let cgma = egma|>Seq.map(Game.Compress)
    nd <- DateTime.Now
    logtime()

    printfn "Saving games" 
    st <- DateTime.Now
    SaveGames binfol 0L cgma
    nd <- DateTime.Now
    logtime()
    printfn "Loading index" 
    st <- DateTime.Now
    let iea = LoadIndex binfol
    nd <- DateTime.Now
    logtime()
    printfn "Loading second game" 
    st <- DateTime.Now
    //let gm = LoadGame binfol iea.[1]
    nd <- DateTime.Now
    logtime()

    
    //let binfol2 = Path.Combine(tstfol,"fritz1" + "_FILES")
    //let iea = LoadIndex binfol2
    //let num = iea.Length
    //let gm = LoadGame binfol2 iea.[99]
    //let pgn2 = Game.ToStr gm

    0 // return an integer exit code
