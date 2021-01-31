namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus

[<TestClass>]
type Piece() =
    [<TestMethod>]
    member this.``ToStr tests``() =
        let ans = Piece.WKnight
        ans
        |> Piece.ToStr
        |> should equal "N"
        let ans = Piece.BKnight
        ans
        |> Piece.ToStr
        |> should equal "n"
