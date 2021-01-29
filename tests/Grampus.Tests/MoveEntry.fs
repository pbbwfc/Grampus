namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus

[<TestClass>]
type MoveEntry () =

    [<TestMethod>]
    member this.``Parse Move Pair`` () =
            let ans = Game.FromStr("53. Nf3 g6")
            ans.MoveText.Head|>Game.MoveStr|> should equal "53. Nf3 "
            ans.MoveText.Tail.Head|>Game.MoveStr|> should equal "g6 "

        [<TestMethod>]
        member this.``Parse Half Move White`` () =
            let ans = Game.FromStr("1. e4")
            ans.MoveText.Head|>Game.MoveStr|> should equal "1. e4 "
      
        [<TestMethod>]
        member this.``Parse Half Move Black`` () =
            let ans = Game.FromStr("13... Nf3")
            ans.MoveText.Head|>Game.MoveStr|> should equal "13... Nf3 "
      
        [<TestMethod>]
        member this.``Parse Comment`` () =
            let ans = Game.FromStr("{this is a comment}")
            ans.MoveText.Head|>Game.MoveStr|>should equal "
{this is a comment} "
      
        [<TestMethod>]
        member this.``Parse Game End`` () =
            let ans = Game.FromStr("1-0")
            ans.MoveText.Head|>Game.MoveStr|>should equal "1-0"
      
        [<TestMethod>]
        member this.``Parse NAG Entry`` () =
            let ans = Game.FromStr("$6")
            ans.MoveText.Head|>Game.MoveStr|>should equal "$6 "
      
        [<TestMethod>]
        member this.``Parse RAV Entry`` () =
            let ans = Game.FromStr("(6. Bd3 cxd4 7. exd4 d5 { - B14 })")
            ans.MoveText.Head|>Game.MoveStr|>should equal "
(6. Bd3 cxd4 7. exd4 d5 
{ - B14 } )
"
      