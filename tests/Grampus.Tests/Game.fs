namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Game() =
    let nl = System.Environment.NewLine
    let fol = @"D:\GitHub\Grampus\Tests\data\"
    let fl = Path.Combine(fol, "simple-game.pgn")
    let db = PgnGames.ReadListFromFile fl
    let gm = db.Head |> Game.Encode
    
    [<TestMethod>]
    member this.``Event to string``() =
        let ans = gm.Hdr.Event
        ans |> should equal "London Chess Classic"
    
    [<TestMethod>]
    member this.``Site to string``() =
        let ans = gm.Site
        ans |> should equal "London"
    
    [<TestMethod>]
    member this.``Date to string``() =
        gm.Hdr.Year |> should equal 2009
        gm.Month.Value |> should equal 12
        gm.Day.Value |> should equal 13
    
    [<TestMethod>]
    member this.``Round to string``() =
        let ans = gm.Round
        ans |> should equal "5"
    
    [<TestMethod>]
    member this.``White to string``() =
        let ans = gm.Hdr.White
        ans |> should equal "Howell, David"
    
    [<TestMethod>]
    member this.``Black to string``() =
        let ans = gm.Hdr.Black
        ans |> should equal "Kramnik, Vladimir"
    
    [<TestMethod>]
    member this.``Result to string``() =
        let ans = gm.Hdr.Result
        ans |> should equal "1/2-1/2"
    
    [<TestMethod>]
    member this.``Game to string length``() =
        let ans = gm |> Game.ToStr
        ans.Length |> should equal 865
    
    [<TestMethod>]
    member this.NAGStr() =
        let ans = NAG.Good |> Game.NAGStr
        ans |> should equal "!"
    
    [<TestMethod>]
    member this.NAGFromStr() =
        let ans = "!" |> Game.NAGFromStr
        ans |> should equal NAG.Good
    
    [<TestMethod>]
    member this.NAGHtm() =
        let ans = NAG.Good |> Game.NAGHtm
        ans |> should equal "&#33;"
    
    [<TestMethod>]
    member this.NAGDesc() =
        let ans = NAG.Good |> Game.NAGDesc
        ans |> should equal "Good"
    
    [<TestMethod>]
    member this.NAGlist() =
        let ans = Game.NAGlist
        ans.Length |> should equal 14
    
    [<TestMethod>]
    member this.AddNag() =
        let ngm = Game.AddNag gm [ 1 ] NAG.Good
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 868
        ngm.MoveText.[2] |> should equal (EncodedNAGEntry(NAG.Good))
    
    [<TestMethod>]
    member this.DeleteNag() =
        let ngm = Game.AddNag gm [ 1 ] NAG.Good
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 868
        ngm.MoveText.[2] |> should equal (EncodedNAGEntry(NAG.Good))
        let ngm = Game.DeleteNag ngm [ 2 ]
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal (gm |> Game.ToStr).Length
    
    [<TestMethod>]
    member this.EditNag() =
        let ngm = Game.AddNag gm [ 1 ] NAG.Good
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 868
        ngm.MoveText.[2] |> should equal (EncodedNAGEntry(NAG.Good))
        let ngm = Game.EditNag ngm [ 2 ] NAG.Poor
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 868
        ngm.MoveText.[2] |> should equal (EncodedNAGEntry(NAG.Poor))
    
    [<TestMethod>]
    member this.AddMv() =
        let mv =
            let mte = gm.MoveText.[105]
            match mte with
            | EncodedHalfMoveEntry(_, _, mv) -> Some(mv)
            | _ -> None
        mv.Value.San |> should equal "Kg6"
        let bd = mv.Value.PostBrd
        let nmv = Move.FromSan bd "Kxf4"
        let ngm, nirs = Game.AddMv gm [ 105 ] nmv
        nirs.Head |> should equal 106
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 874
    
    [<TestMethod>]
    member this.AddRav() =
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
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 879
    
    [<TestMethod>]
    member this.DeleteRav() =
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
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 879
        let ngm = Game.DeleteRav ngm [ 2; 0 ]
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 865
    
    [<TestMethod>]
    member this.Strip() =
        let ngm = Game.Strip gm [ 1 ]
        ngm.MoveText.Length |> should equal 2
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 221
    
    [<TestMethod>]
    member this.CommentBefore() =
        let cm i (igm : EncodedGame) =
            let mte = igm.MoveText.[i]
            match mte with
            | EncodedCommentEntry(str) -> Some(str)
            | _ -> None
        
        let ngm = Game.CommentBefore gm [ 1 ] "test"
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 879
        let ans = cm 1 ngm
        ans.Value |> should equal "test"
    
    [<TestMethod>]
    member this.CommentAfter() =
        let cm i (igm : EncodedGame) =
            let mte = igm.MoveText.[i]
            match mte with
            | EncodedCommentEntry(str) -> Some(str)
            | _ -> None
        
        let ngm = Game.CommentAfter gm [ 1 ] "test"
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 874
        let ans = cm 2 ngm
        ans.Value |> should equal "test"
    
    [<TestMethod>]
    member this.EditComment() =
        let cm i (igm : EncodedGame) =
            let mte = igm.MoveText.[i]
            match mte with
            | EncodedCommentEntry(str) -> Some(str)
            | _ -> None
        
        let ngm = Game.CommentAfter gm [ 1 ] "test"
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 874
        let ans = cm 2 ngm
        ans.Value |> should equal "test"
        let ngm = Game.EditComment ngm [ 2 ] "test1"
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 875
        let ans = cm 2 ngm
        ans.Value |> should equal "test1"
    
    [<TestMethod>]
    member this.DeleteComment() =
        let cm i (igm : EncodedGame) =
            let mte = igm.MoveText.[i]
            match mte with
            | EncodedCommentEntry(str) -> Some(str)
            | _ -> None
        
        let ngm = Game.CommentAfter gm [ 1 ] "test"
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 874
        let ans = cm 2 ngm
        ans.Value |> should equal "test"
        let ngm = Game.DeleteComment ngm [ 2 ]
        let ans = ngm |> Game.ToStr
        ans.Length |> should equal 865
        let ans = cm 2 ngm
        ans |> should equal None
    
    [<TestMethod>]
    member this.FromStr() =
        let pgnstr = File.ReadAllText fl
        let ngm = Game.FromStr pgnstr
        
        let ans =
            ngm
            |> Game.Encode
            |> Game.ToStr
        ans.Length |> should equal 865
    
    [<TestMethod>]
    member this.GetPosnsMoves() =
        let pns, sns = Game.GetPosnsMoves 20 gm
        pns.Length |> should equal 20
        sns.Length |> should equal 20
        pns.[1] 
        |> should equal 
               "RNBQKBNRPPPP.PPP............P...................pppppppprnbqkbnr b"
        sns.[1] |> should equal "e5"
    
    [<TestMethod>]
    member this.GetPosns() =
        let pns = Game.GetPosns 20 gm
        pns.Length |> should equal 20
        pns.[1] 
        |> should equal 
               "RNBQKBNRPPPP.PPP............P...................pppppppprnbqkbnr b"
