namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Eco() =
    let fol = @"D:\GitHub\Grampus\Tests\data\"
    let fl = Path.Combine(fol, "simple-game.pgn")
    let db = PgnGames.ReadSeqFromFile fl |> Seq.toList
    let gm = db.Head |> Game.Encode
    let tstfol = @"D:\GitHub\Grampus\tests\data\simple-game_FILES"
    let tstfn = @"D:\GitHub\Grampus\tests\data\simple-game.grampus"
    let tstfol2 = @"D:\GitHub\Grampus\tests\data\simple-game2_FILES"
    let tstfn2 = @"D:\GitHub\Grampus\tests\data\simple-game2.grampus"
    
    [<TestMethod>]
    member this.ForGame() =
        gm.Hdr.Opening |> should equal ""
        gm.Hdr.ECO |> should equal ""
        let ngm = Eco.ForGame(gm)
        ngm.Hdr.ECO |> should equal "C42"
        ngm.Hdr.Opening |> should equal "Petrov"
    
    [<TestMethod>]
    member this.ForGames() =
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
        Directory.CreateDirectory(tstfol2) |> ignore
        File.Copy(Path.Combine(tstfol, "INDEX"), Path.Combine(tstfol2, "INDEX"))
        File.Copy(Path.Combine(tstfol, "GAMES"), Path.Combine(tstfol2, "GAMES"))
        File.Copy
            (Path.Combine(tstfol, "HEADERS"), Path.Combine(tstfol2, "HEADERS"))
        let indx = Index.Load tstfn
        let hdrs = Headers.Load tstfn
        let gm = Games.LoadGame tstfn indx.[0] hdrs.[0]
        gm.Hdr.ECO |> should equal ""
        gm.Hdr.Opening |> should equal ""
        Eco.ForBase tstfn2 (fun i -> ())
        let indx2 = Index.Load tstfn2
        let hdrs2 = Headers.Load tstfn2
        let gm2 = Games.LoadGame tstfn2 indx2.[0] hdrs2.[0]
        gm2.Hdr.ECO |> should equal "C42"
        gm2.Hdr.Opening |> should equal "Petrov"
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
