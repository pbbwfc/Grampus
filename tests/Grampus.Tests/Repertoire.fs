namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Repertoire() =
    let tstfol = @"D:\GitHub\Grampus\tests\data\repertoire"
    let tstfol2 = @"D:\GitHub\Grampus\tests\data\repertoire2"
    let deffol =
        Path.Combine
            (System.Environment.GetFolderPath
                 (System.Environment.SpecialFolder.MyDocuments), 
             "Grampus\\repertoire")
    let ss1 =
        "RNBQKBNRPP.PPPPP..........P.....................pppppppprnbqkbnr b"
    
    [<TestMethod>]
    member this.White() =
        GrampusInternal.Repertoire.setfol tstfol
        Repertoire.LoadWhite()
        let ro, rm = Repertoire.White()
        ro.Count |> should equal 39
        rm.Count |> should equal 39
        let errs = File.ReadAllText(Repertoire.WhiteErrFile())
        errs.Length |> should equal 0
    
    [<TestMethod>]
    member this.Black() =
        GrampusInternal.Repertoire.setfol tstfol
        Repertoire.LoadBlack()
        let ro, rm = Repertoire.Black()
        ro.Count |> should equal 92
        rm.Count |> should equal 97
        let errs = File.ReadAllText(Repertoire.BlackErrFile())
        errs.Length |> should equal 0
    
    [<TestMethod>]
    member this.UpdateWhite() =
        File.Delete(Path.Combine(tstfol2, "whte.json"))
        File.Delete(Path.Combine(tstfol2, "whteerrs.txt"))
        GrampusInternal.Repertoire.setfol tstfol2
        Repertoire.LoadWhite()
        let ro, rm = Repertoire.White()
        ro.Count |> should equal 0
        rm.Count |> should equal 0
        File.Exists(Repertoire.WhiteErrFile()) |> should equal false
        Repertoire.UpdateWhite() |> should equal 0
        Repertoire.LoadWhite()
        let ro, rm = Repertoire.White()
        ro.Count |> should equal 39
        rm.Count |> should equal 39
        let errs = File.ReadAllText(Repertoire.WhiteErrFile())
        errs.Length |> should equal 0
    
    [<TestMethod>]
    member this.UpdateBlack() =
        File.Delete(Path.Combine(tstfol2, "blck.json"))
        File.Delete(Path.Combine(tstfol2, "blckerrs.txt"))
        GrampusInternal.Repertoire.setfol tstfol2
        Repertoire.LoadBlack()
        let ro, rm = Repertoire.Black()
        ro.Count |> should equal 0
        rm.Count |> should equal 0
        File.Exists(Repertoire.BlackErrFile()) |> should equal false
        Repertoire.UpdateBlack() |> should equal 0
        Repertoire.LoadBlack()
        let ro, rm = Repertoire.Black()
        ro.Count |> should equal 92
        rm.Count |> should equal 97
        let errs = File.ReadAllText(Repertoire.BlackErrFile())
        errs.Length |> should equal 0
    
    [<TestMethod>]
    member this.OptsHaveSan() =
        GrampusInternal.Repertoire.setfol tstfol
        Repertoire.LoadWhite()
        let ro, rm = Repertoire.White()
        let ans = Repertoire.OptsHaveSan "d4" (ro.[ss1])
        ans |> should equal false
        let ans = Repertoire.OptsHaveSan "e5" (ro.[ss1])
        ans |> should equal true
