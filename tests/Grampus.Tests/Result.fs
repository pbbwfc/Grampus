namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus

[<TestClass>]
type Result () =

    [<TestMethod>]
    member this.``ToStr tests``() = 
        let ans = GameResult.Draw
        ans|>Result.ToStr|> should equal "1/2-1/2"
        let ans = GameResult.WhiteWins
        ans|>Result.ToStr|> should equal "1-0"
        let ans = GameResult.BlackWins
        ans|>Result.ToStr|> should equal "0-1"
        let ans = GameResult.Open
        ans|>Result.ToStr|> should equal "*"

    [<TestMethod>]
    member this.``Parse tests``() = 
        let ans = "1/2-1/2"
        ans|>Result.Parse|> should equal GameResult.Draw
        let ans = "1-0"
        ans|>Result.Parse|> should equal GameResult.WhiteWins
        let ans = "0-1"
        ans|>Result.Parse|> should equal GameResult.BlackWins
        let ans = "*"
        ans|>Result.Parse|> should equal GameResult.Open

    [<TestMethod>]
    member this.``ToInt tests``() = 
        let ans = GameResult.Draw
        ans|>Result.ToInt|> should equal 1
        let ans = GameResult.WhiteWins
        ans|>Result.ToInt|> should equal 2
        let ans = GameResult.BlackWins
        ans|>Result.ToInt|> should equal 0
        let ans = GameResult.Open
        ans|>Result.ToInt|> should equal 1
    