namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Games() =
    let fol = @"D:\GitHub\Grampus\Tests\data\"
    let tstfol = @"D:\GitHub\Grampus\tests\data\simple-game_FILES"
    let tstfol2 = @"D:\GitHub\Grampus\tests\data\simple-game2_FILES"
    
    [<TestMethod>]
    member this.Save() =
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
        Directory.CreateDirectory(tstfol2) |> ignore
        File.Copy(Path.Combine(tstfol, "INDEX"), Path.Combine(tstfol2, "INDEX"))
        File.Copy(Path.Combine(tstfol, "GAMES"), Path.Combine(tstfol2, "GAMES"))
        File.Copy
            (Path.Combine(tstfol, "HEADERS"), Path.Combine(tstfol2, "HEADERS"))
        let indx = Index.Load tstfol
        let hdrs = Headers.Load tstfol
        let gm = Games.LoadGame tstfol indx.[0] hdrs.[0]
        gm.MoveText.Length |> should equal 107
        Games.Save tstfol2 [ gm; gm ]
        let indx2 = Index.Load tstfol2
        let hdrs2 = Headers.Load tstfol2
        indx2.Length |> should equal 2
        let gm2 = Games.LoadGame tstfol2 indx2.[1] hdrs2.[1]
        gm2.MoveText.Length |> should equal 107
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
    
    [<TestMethod>]
    member this.Add() =
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
        Directory.CreateDirectory(tstfol2) |> ignore
        File.Copy(Path.Combine(tstfol, "INDEX"), Path.Combine(tstfol2, "INDEX"))
        File.Copy(Path.Combine(tstfol, "GAMES"), Path.Combine(tstfol2, "GAMES"))
        File.Copy
            (Path.Combine(tstfol, "HEADERS"), Path.Combine(tstfol2, "HEADERS"))
        let indx = Index.Load tstfol
        let hdrs = Headers.Load tstfol
        let gm = Games.LoadGame tstfol indx.[0] hdrs.[0]
        gm.MoveText.Length |> should equal 107
        Games.Add tstfol2 [ gm; gm ]
        let indx2 = Index.Load tstfol2
        let hdrs2 = Headers.Load tstfol2
        indx2.Length |> should equal 3
        let gm2 = Games.LoadGame tstfol2 indx2.[1] hdrs2.[1]
        gm2.MoveText.Length |> should equal 107
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
    
    [<TestMethod>]
    member this.AppendGame() =
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
        Directory.CreateDirectory(tstfol2) |> ignore
        File.Copy(Path.Combine(tstfol, "INDEX"), Path.Combine(tstfol2, "INDEX"))
        File.Copy(Path.Combine(tstfol, "GAMES"), Path.Combine(tstfol2, "GAMES"))
        File.Copy
            (Path.Combine(tstfol, "HEADERS"), Path.Combine(tstfol2, "HEADERS"))
        let indx = Index.Load tstfol
        let hdrs = Headers.Load tstfol
        let gm = Games.LoadGame tstfol indx.[0] hdrs.[0]
        gm.MoveText.Length |> should equal 107
        Games.AppendGame tstfol2 gm
        let indx2 = Index.Load tstfol2
        let hdrs2 = Headers.Load tstfol2
        indx2.Length |> should equal 2
        let gm2 = Games.LoadGame tstfol2 indx2.[1] hdrs2.[1]
        gm2.MoveText.Length |> should equal 107
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
    
    [<TestMethod>]
    member this.UpdateGame() =
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
        Directory.CreateDirectory(tstfol2) |> ignore
        File.Copy(Path.Combine(tstfol, "INDEX"), Path.Combine(tstfol2, "INDEX"))
        File.Copy(Path.Combine(tstfol, "GAMES"), Path.Combine(tstfol2, "GAMES"))
        File.Copy
            (Path.Combine(tstfol, "HEADERS"), Path.Combine(tstfol2, "HEADERS"))
        let indx = Index.Load tstfol
        indx.[0].Offset |> should equal 0L
        let hdrs = Headers.Load tstfol
        let gm = Games.LoadGame tstfol indx.[0] hdrs.[0]
        gm.MoveText.Length |> should equal 107
        Games.UpdateGame tstfol2 0 gm
        let indx2 = Index.Load tstfol2
        let hdrs2 = Headers.Load tstfol2
        indx2.Length |> should equal 1
        indx2.[0].Offset |> should equal 1676L
        let gm2 = Games.LoadGame tstfol2 indx2.[0] hdrs2.[0]
        gm2.MoveText.Length |> should equal 107
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
    
    [<TestMethod>]
    member this.Compact() =
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
        Directory.CreateDirectory(tstfol2) |> ignore
        File.Copy(Path.Combine(tstfol, "INDEX"), Path.Combine(tstfol2, "INDEX"))
        File.Copy(Path.Combine(tstfol, "GAMES"), Path.Combine(tstfol2, "GAMES"))
        File.Copy
            (Path.Combine(tstfol, "HEADERS"), Path.Combine(tstfol2, "HEADERS"))
        let indx = Index.Load tstfol
        indx.[0].Offset |> should equal 0L
        let hdrs = Headers.Load tstfol
        let gm = Games.LoadGame tstfol indx.[0] hdrs.[0]
        gm.MoveText.Length |> should equal 107
        Games.UpdateGame tstfol2 0 gm
        let indx2 = Index.Load tstfol2
        let hdrs2 = Headers.Load tstfol2
        indx2.Length |> should equal 1
        indx2.[0].Offset |> should equal 1676L
        let gm2 = Games.LoadGame tstfol2 indx2.[0] hdrs2.[0]
        gm2.MoveText.Length |> should equal 107
        Games.Compact tstfol2
        let indx3 = Index.Load tstfol2
        let hdrs3 = Headers.Load tstfol2
        indx3.Length |> should equal 1
        indx3.[0].Offset |> should equal 0L
        let gm3 = Games.LoadGame tstfol2 indx3.[0] hdrs3.[0]
        gm3.MoveText.Length |> should equal 107
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
