namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Grampus() =
    let tstfn = @"D:\GitHub\Grampus\tests\data\simple-game.grampus"
    let tstfn2 = @"D:\GitHub\Grampus\tests\data\simple-game2.grampus"
    let tstfol = @"D:\GitHub\Grampus\tests\data\simple-game_FILES"
    
    [<TestMethod>]
    member this.Load() =
        let ans = Grampus.Load(tstfn)
        ans.TreesPly |> should equal 20
    
    [<TestMethod>]
    member this.Save() =
        if File.Exists(tstfn2) then File.Delete(tstfn2)
        let ans = File.Exists(tstfn2)
        ans |> should equal false
        let gmp = Grampus.Load(tstfn)
        Grampus.Save(tstfn2, gmp)
        let ans = File.Exists(tstfn2)
        ans |> should equal true
        File.Delete(tstfn2)
    
    [<TestMethod>]
    member this.Delete() =
        if File.Exists(tstfn2) then File.Delete(tstfn2)
        let ans = File.Exists(tstfn2)
        ans |> should equal false
        let gmp = Grampus.New(tstfn2)
        let ans = File.Exists(tstfn2)
        ans |> should equal true
        Grampus.Delete(tstfn2)
        let ans = File.Exists(tstfn2)
        ans |> should equal false
    
    [<TestMethod>]
    member this.DeleteTree() =
        let trfol = tstfol + @"\tree"
        if Directory.Exists(trfol) then Directory.Delete(trfol, true)
        let ans = Directory.Exists(trfol)
        ans |> should equal false
        Tree.Create(tstfol)
        let ans = Directory.Exists(trfol)
        ans |> should equal true
        let ngmp = Grampus.DeleteTree(tstfn)
        let ans = Directory.Exists(trfol)
        ans |> should equal false
        ngmp.TreesPly |> should equal 20
        ngmp.TreesCreated |> should equal None
