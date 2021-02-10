namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Index() =
    let tstfol = @"D:\GitHub\Grampus\tests\data\simple-game_FILES"
    let tstfn = @"D:\GitHub\Grampus\tests\data\simple-game.grampus"
    let tstfol2 = @"D:\GitHub\Grampus\tests\data\simple-game2_FILES"
    let tstfn2 = @"D:\GitHub\Grampus\tests\data\simple-game2.grampus"
    
    [<TestMethod>]
    member this.Load() =
        let ans = Index.Load tstfn
        ans.Length |> should equal 1
    
    [<TestMethod>]
    member this.Save() =
        let tstfni2 = Path.Combine(tstfol2, "INDEX")
        if File.Exists(tstfni2) then File.Delete(tstfni2)
        let ans = File.Exists(tstfni2)
        ans |> should equal false
        let iea = Index.Load tstfn
        Directory.CreateDirectory(tstfol2) |> ignore
        Index.Save(tstfn2, iea)
        let ans = File.Exists(tstfni2)
        ans |> should equal true
        File.Delete(tstfni2)
