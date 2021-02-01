namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type PgnGames() =
    let fn = @"D:\GitHub\Grampus\tests\data\simple-game.pgn"
    [<TestMethod>]
    member this.GetNumberOfGames() =
        let ans = PgnGames.GetNumberOfGames(fn)
        ans |> should equal 1
