namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Game () =
    let nl = System.Environment.NewLine
    let fol = @"D:\GitHub\Grampus\Tests\data\"
    let fl = Path.Combine(fol,"simple-game.pgn")
    let db = PgnGames.ReadListFromFile fl
    let gm = db.Head|>Game.Encode 
    
    [<TestMethod>]
    member this.``Event to string`` () =
      let ans = gm.Hdr.Event
      ans |> should equal "London Chess Classic"

    [<TestMethod>]
    member this.``Site to string`` () =
      let ans = gm.Site
      ans |> should equal "London"

    [<TestMethod>]
    member this.``Date to string`` () =
      gm.Hdr.Year |> should equal 2009
      gm.Month.Value |> should equal 12
      gm.Day.Value |> should equal 13
      
    [<TestMethod>]
    member this.``Round to string`` () =
      let ans = gm.Round
      ans |> should equal "5"

    [<TestMethod>]
    member this.``White to string`` () =
      let ans = gm.Hdr.White
      ans |> should equal "Howell, David"

    [<TestMethod>]
    member this.``Black to string`` () =
      let ans = gm.Hdr.Black
      ans |> should equal "Kramnik, Vladimir"

    [<TestMethod>]
    member this.``Result to string`` () =
      let ans = gm.Hdr.Result
      ans |> should equal "1/2-1/2"

    [<TestMethod>]
    member this.``Game to string length`` () =
      let ans = gm|>Game.ToStr
      ans.Length |> should equal 865

