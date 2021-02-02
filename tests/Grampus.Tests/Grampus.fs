namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Grampus() =
    let tstfn = @"D:\GitHub\Grampus\tests\data\simple-game.grampus"
    let tstfn2 = @"D:\GitHub\Grampus\tests\data\simple-game2.grampus"
    
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
