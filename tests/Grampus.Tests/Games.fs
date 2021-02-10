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
    let tstfn = @"D:\GitHub\Grampus\tests\data\simple-game.grampus"
    let tstfn2 = @"D:\GitHub\Grampus\tests\data\simple-game2.grampus"
    let tstfn2b = @"D:\GitHub\Grampus\tests\data\simple-game2_B.grampus"
    let tstfn2w = @"D:\GitHub\Grampus\tests\data\simple-game2_W.grampus"
    
    [<TestMethod>]
    member this.Save() =
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
        Directory.CreateDirectory(tstfol2) |> ignore
        File.Copy(Path.Combine(tstfol, "INDEX"), Path.Combine(tstfol2, "INDEX"))
        File.Copy(Path.Combine(tstfol, "GAMES"), Path.Combine(tstfol2, "GAMES"))
        File.Copy
            (Path.Combine(tstfol, "HEADERS"), Path.Combine(tstfol2, "HEADERS"))
        let indx = Index.Load tstfn
        let hdrs = Headers.Load tstfn
        let gm = Games.LoadGame tstfn indx.[0] hdrs.[0]
        gm.MoveText.Length |> should equal 107
        Games.Save tstfn2 [ gm; gm ] (fun i -> ())
        let indx2 = Index.Load tstfn2
        let hdrs2 = Headers.Load tstfn2
        indx2.Length |> should equal 2
        let gm2 = Games.LoadGame tstfn2 indx2.[1] hdrs2.[1]
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
        let indx = Index.Load tstfn
        let hdrs = Headers.Load tstfn
        let gm = Games.LoadGame tstfn indx.[0] hdrs.[0]
        gm.MoveText.Length |> should equal 107
        Games.Add tstfn2 [ gm; gm ] (fun i -> ())
        let indx2 = Index.Load tstfn2
        let hdrs2 = Headers.Load tstfn2
        indx2.Length |> should equal 3
        let gm2 = Games.LoadGame tstfn2 indx2.[1] hdrs2.[1]
        gm2.MoveText.Length |> should equal 107
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
    
    [<TestMethod>]
    member this.AddGmp() =
        if Directory.Exists(tstfol2) then Grampus.Delete(tstfn2)
        Grampus.Copy(tstfn, tstfn2)
        let indx = Index.Load tstfn
        let hdrs = Headers.Load tstfn
        let gm = Games.LoadGame tstfn indx.[0] hdrs.[0]
        gm.MoveText.Length |> should equal 107
        Games.AddGmp tstfn2 tstfn (fun i -> ())
        Games.AddGmp tstfn2 tstfn (fun i -> ())
        let indx2 = Index.Load tstfn2
        let hdrs2 = Headers.Load tstfn2
        indx2.Length |> should equal 3
        let gm2 = Games.LoadGame tstfn2 indx2.[1] hdrs2.[1]
        gm2.MoveText.Length |> should equal 107
        if Directory.Exists(tstfol2) then Grampus.Delete(tstfn2)
    
    [<TestMethod>]
    member this.AppendGame() =
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
        Directory.CreateDirectory(tstfol2) |> ignore
        File.Copy(Path.Combine(tstfol, "INDEX"), Path.Combine(tstfol2, "INDEX"))
        File.Copy(Path.Combine(tstfol, "GAMES"), Path.Combine(tstfol2, "GAMES"))
        File.Copy
            (Path.Combine(tstfol, "HEADERS"), Path.Combine(tstfol2, "HEADERS"))
        let indx = Index.Load tstfn
        let hdrs = Headers.Load tstfn
        let gm = Games.LoadGame tstfn indx.[0] hdrs.[0]
        gm.MoveText.Length |> should equal 107
        Games.AppendGame tstfn2 gm
        let indx2 = Index.Load tstfn2
        let hdrs2 = Headers.Load tstfn2
        indx2.Length |> should equal 2
        let gm2 = Games.LoadGame tstfn2 indx2.[1] hdrs2.[1]
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
        let indx = Index.Load tstfn
        indx.[0].Offset |> should equal 0L
        let hdrs = Headers.Load tstfn
        let gm = Games.LoadGame tstfn indx.[0] hdrs.[0]
        gm.MoveText.Length |> should equal 107
        Games.UpdateGame tstfn2 0 gm
        let indx2 = Index.Load tstfn2
        let hdrs2 = Headers.Load tstfn2
        indx2.Length |> should equal 1
        indx2.[0].Offset |> should equal 1676L
        let gm2 = Games.LoadGame tstfn2 indx2.[0] hdrs2.[0]
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
        let indx = Index.Load tstfn
        indx.[0].Offset |> should equal 0L
        let hdrs = Headers.Load tstfn
        let gm = Games.LoadGame tstfn indx.[0] hdrs.[0]
        gm.MoveText.Length |> should equal 107
        Games.UpdateGame tstfn2 0 gm
        let indx2 = Index.Load tstfn2
        let hdrs2 = Headers.Load tstfn2
        indx2.Length |> should equal 1
        indx2.[0].Offset |> should equal 1676L
        let gm2 = Games.LoadGame tstfn2 indx2.[0] hdrs2.[0]
        gm2.MoveText.Length |> should equal 107
        let msg = Games.Compact tstfn2 (fun i -> ())
        msg |> should equal "Number of games permanently deleted is: 0"
        let indx3 = Index.Load tstfn2
        let hdrs3 = Headers.Load tstfn2
        indx3.Length |> should equal 1
        indx3.[0].Offset |> should equal 0L
        let gm3 = Games.LoadGame tstfn2 indx3.[0] hdrs3.[0]
        gm3.MoveText.Length |> should equal 107
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
    
    [<TestMethod>]
    member this.ExtractNewer() =
        if File.Exists(tstfn2) then Grampus.Delete(tstfn2)
        Games.ExtractNewer tstfn tstfn2 2020 (fun i -> ())
        let nindx = Index.Load tstfn2
        nindx.Length |> should equal 0
        if File.Exists(tstfn2) then Grampus.Delete(tstfn2)
        Games.ExtractNewer tstfn tstfn2 2000 (fun i -> ())
        let nindx = Index.Load tstfn2
        nindx.Length |> should equal 1
        if File.Exists(tstfn2) then Grampus.Delete(tstfn2)
    
    [<TestMethod>]
    member this.ExtractStronger() =
        if File.Exists(tstfn2) then Grampus.Delete(tstfn2)
        Games.ExtractStronger tstfn tstfn2 2200 (fun i -> ())
        let nindx = Index.Load tstfn2
        nindx.Length |> should equal 1
        if File.Exists(tstfn2) then Grampus.Delete(tstfn2)
        Games.ExtractStronger tstfn tstfn2 2800 (fun i -> ())
        let nindx = Index.Load tstfn2
        nindx.Length |> should equal 0
        if File.Exists(tstfn2) then Grampus.Delete(tstfn2)
    
    [<TestMethod>]
    member this.GetPossNames() =
        let nms = Games.GetPossNames tstfn "x" (fun i -> ())
        nms.Length |> should equal 0
        let nms = Games.GetPossNames tstfn "e" (fun i -> ())
        nms.Length |> should equal 1
    
    [<TestMethod>]
    member this.ExtractPlayer() =
        if File.Exists(tstfn2) then Grampus.Delete(tstfn2)
        if File.Exists(tstfn2b) then Grampus.Delete(tstfn2b)
        if File.Exists(tstfn2w) then Grampus.Delete(tstfn2w)
        Games.ExtractPlayer tstfn tstfn2 "Howell, David" (fun i -> ())
        let nindx = Index.Load tstfn2
        nindx.Length |> should equal 1
        if File.Exists(tstfn2) then Grampus.Delete(tstfn2)
        Games.ExtractPlayer tstfn tstfn2 "Fred" (fun i -> ())
        let nindx = Index.Load tstfn2
        nindx.Length |> should equal 0
        if File.Exists(tstfn2) then Grampus.Delete(tstfn2)
        if File.Exists(tstfn2b) then Grampus.Delete(tstfn2b)
        if File.Exists(tstfn2w) then Grampus.Delete(tstfn2w)

    
    [<TestMethod>]
    member this.RemoveDuplicates() =
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
        Grampus.Copy(tstfn, tstfn2)
        let indx = Index.Load tstfn
        indx.[0].Offset |> should equal 0L
        let hdrs = Headers.Load tstfn
        let gm = Games.LoadGame tstfn indx.[0] hdrs.[0]
        gm.MoveText.Length |> should equal 107
        Games.AppendGame tstfn2 gm
        let indx2 = Index.Load tstfn2
        let hdrs2 = Headers.Load tstfn2
        indx2.Length |> should equal 2
        indx2.[1].Offset |> should equal 1676L
        let gm2 = Games.LoadGame tstfn2 indx2.[0] hdrs2.[0]
        gm2.MoveText.Length |> should equal 107
        let msg = Games.RemoveDuplicates tstfn2 (fun i -> ())
        msg |> should equal "Number of games permanently deleted is: 1"
        let indx3 = Index.Load tstfn2
        let hdrs3 = Headers.Load tstfn2
        indx3.Length |> should equal 1
        indx3.[0].Offset |> should equal 0L
        let gm3 = Games.LoadGame tstfn2 indx3.[0] hdrs3.[0]
        gm3.MoveText.Length |> should equal 107
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
    
    [<TestMethod>]
    member this.RemoveComments() =
        if Directory.Exists(tstfol2) then Grampus.Delete(tstfn2)
        Grampus.Copy(tstfn, tstfn2)
        let indx = Index.Load tstfn2
        let hdrs = Headers.Load tstfn2
        let gm = Games.LoadGame tstfn2 indx.[0] hdrs.[0]
        let ans = gm |> Game.ToStr
        ans.Length |> should equal 862
        let ngm = Game.CommentAfter gm [ 1 ] "test"
        Games.UpdateGame tstfn2 0 ngm
        let indx = Index.Load tstfn2
        let hdrs = Headers.Load tstfn2
        let gm = Games.LoadGame tstfn2 indx.[0] hdrs.[0]
        let ans = gm |> Game.ToStr
        ans.Length |> should equal 871
        gm.MoveText.[2] |> should equal (EncodedCommentEntry("test"))
        Games.RemoveComments tstfn2 (fun i -> ())
        let indx = Index.Load tstfn2
        let hdrs = Headers.Load tstfn2
        let gm = Games.LoadGame tstfn2 indx.[0] hdrs.[0]
        let ans = gm |> Game.ToStr
        ans.Length |> should equal 862
        if Directory.Exists(tstfol2) then Grampus.Delete(tstfn2)
    
    [<TestMethod>]
    member this.RemoveNags() =
        if Directory.Exists(tstfol2) then Grampus.Delete(tstfn2)
        Grampus.Copy(tstfn, tstfn2)
        let indx = Index.Load tstfn2
        let hdrs = Headers.Load tstfn2
        let gm = Games.LoadGame tstfn2 indx.[0] hdrs.[0]
        let ans = gm |> Game.ToStr
        ans.Length |> should equal 862
        let ngm = Game.AddNag gm [ 1 ] NAG.Good
        Games.UpdateGame tstfn2 0 ngm
        let indx = Index.Load tstfn2
        let hdrs = Headers.Load tstfn2
        let gm = Games.LoadGame tstfn2 indx.[0] hdrs.[0]
        let ans = gm |> Game.ToStr
        ans.Length |> should equal 865
        gm.MoveText.[2] |> should equal (EncodedNAGEntry(NAG.Good))
        Games.RemoveNags tstfn2 (fun i -> ())
        let indx = Index.Load tstfn2
        let hdrs = Headers.Load tstfn2
        let gm = Games.LoadGame tstfn2 indx.[0] hdrs.[0]
        let ans = gm |> Game.ToStr
        ans.Length |> should equal 862
        if Directory.Exists(tstfol2) then Grampus.Delete(tstfn2)
    
    [<TestMethod>]
    member this.RemoveRavs() =
        if Directory.Exists(tstfol2) then Grampus.Delete(tstfn2)
        Grampus.Copy(tstfn, tstfn2)
        let indx = Index.Load tstfn2
        let hdrs = Headers.Load tstfn2
        let gm = Games.LoadGame tstfn2 indx.[0] hdrs.[0]
        let ans = gm |> Game.ToStr
        ans.Length |> should equal 862
        let mv =
            let mte = gm.MoveText.[0]
            match mte with
            | EncodedHalfMoveEntry(_, _, mv) -> Some(mv)
            | _ -> None
        mv.Value.San |> should equal "e4"
        let bd = mv.Value.PostBrd
        let nmv = Move.FromSan bd "e6"
        let ngm, nirs = Game.AddRav gm [ 0 ] nmv
        nirs.Head |> should equal 2
        nirs.Tail.Head |> should equal 0
        Games.UpdateGame tstfn2 0 ngm
        let indx = Index.Load tstfn2
        let hdrs = Headers.Load tstfn2
        let gm = Games.LoadGame tstfn2 indx.[0] hdrs.[0]
        let ans = gm |> Game.ToStr
        ans.Length |> should equal 876
        Games.RemoveRavs tstfn2 (fun i -> ())
        let indx = Index.Load tstfn2
        let hdrs = Headers.Load tstfn2
        let gm = Games.LoadGame tstfn2 indx.[0] hdrs.[0]
        let ans = gm |> Game.ToStr
        ans.Length |> should equal 862
        if Directory.Exists(tstfol2) then Grampus.Delete(tstfn2)
