namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Tree() =
    let tstfol = @"D:\GitHub\Grampus\tests\data\simple-game_FILES"
    let ss1 =
        "RNBQKBNRPP.PPPPP..........P.....................pppppppprnbqkbnr b"
    
    [<TestMethod>]
    member this.Create() =
        let trfol = tstfol + @"\tree"
        if Directory.Exists(trfol) then Directory.Delete(trfol, true)
        let ans = Directory.Exists(trfol)
        ans |> should equal false
        Tree.Create(tstfol)
        let ans = Directory.Exists(trfol)
        ans |> should equal true
        Directory.Delete(trfol, true)
    
    [<TestMethod>]
    member this.Save() =
        let trfol = tstfol + @"\tree"
        if Directory.Exists(trfol) then Directory.Delete(trfol, true)
        let ans = Directory.Exists(trfol)
        ans |> should equal false
        Tree.Create(tstfol)
        Tree.Save([||], [||], tstfol)
        let ans = Directory.Exists(trfol)
        ans |> should equal true
        Directory.Delete(trfol, true)
    
    [<TestMethod>]
    member this.Read() =
        let trfol = tstfol + @"\tree"
        if Directory.Exists(trfol) then Directory.Delete(trfol, true)
        let ans = Directory.Exists(trfol)
        ans |> should equal false
        Tree.Create(tstfol)
        Tree.Save([| ss1 |], [| new stats() |], tstfol)
        let ans = Tree.Read(ss1, tstfol)
        ans.MvsStats.Count |> should equal 0
        let ans = Directory.Exists(trfol)
        ans |> should equal true
        Directory.Delete(trfol, true)
