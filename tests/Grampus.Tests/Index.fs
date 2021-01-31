namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Index() =
    let tstfol = @"D:\GitHub\Grampus\tests\data\simple-game_FILES"
    let tstfol2 = @"D:\GitHub\Grampus\tests\data\simple-game2_FILES"
    
    [<TestMethod>]
    member this.Load() =
        let ans = Index.Load(tstfol)
        ans.Length |> should equal 1
    
    [<TestMethod>]
    member this.Save() =
        let tstfn2 = Path.Combine(tstfol2, "INDEX")
        if File.Exists(tstfn2) then File.Delete(tstfn2)
        let ans = File.Exists(tstfn2)
        ans |> should equal false
        let iea = Index.Load(tstfol)
        Directory.CreateDirectory(tstfol2) |> ignore
        Index.Save(tstfol2, iea)
        let ans = File.Exists(tstfn2)
        ans |> should equal true
        File.Delete(tstfn2)
