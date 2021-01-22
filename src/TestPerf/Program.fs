// Learn more about F# at http://fsharp.org

open System
open System.IO
open FsChess

[<EntryPoint>]
let main argv =
    let tstfol = 
        let pth = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"Grampus\\bases")
        Directory.CreateDirectory(pth)|>ignore
        pth
    let mutable st = DateTime.Now
    let mutable nd = DateTime.Now
    let logtime() = 
        let el0 = nd-st
        let el = float(el0.TotalMilliseconds)/1000.0
        printfn "Elapsed time %f seconds" el

    let nm = "french"
    let nm = "recent10000"
    let tmpfol = @"D:\tmp" 
    let pgn = Path.Combine(tmpfol,nm + ".pgn")
    let binfol = Path.Combine(tstfol,nm + "_FILES")
    let gmpfil = Path.Combine(tstfol,nm + ".grampus")
    printfn "Importing games for: %s" nm 
    let ugma = Pgn.Games.ReadSeqFromFile pgn
    printfn "Encoding games for: %s" nm 
    let egma = ugma|>Seq.map(Game.Encode)
    printfn "Compressing games for: %s" nm 
    let cgma = egma|>Seq.map(Game.Compress)
    printfn "Saving games" 
    st <- DateTime.Now
    Games.Save binfol 0L cgma
    nd <- DateTime.Now
    logtime()
    let gmp = {GrampusDataEMP with SourcePgn=pgn}
    Grampus.Save(gmpfil,gmp)
    printfn "Loading index" 
    st <- DateTime.Now
    let iea = Index.Load binfol
    nd <- DateTime.Now
    logtime()
    printfn "Loading second game" 
    st <- DateTime.Now
    let gm = Games.LoadGame binfol iea.[1]
    nd <- DateTime.Now
    logtime()

    
    //let binfol2 = Path.Combine(tstfol,"fritz1" + "_FILES")
    //let iea = LoadIndex binfol2
    //let num = iea.Length
    //let gm = LoadGame binfol2 iea.[99]
    //let pgn2 = Game.ToStr gm

    0 // return an integer exit code
