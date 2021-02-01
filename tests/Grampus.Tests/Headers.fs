namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Headers() =
    let tstfol = @"D:\GitHub\Grampus\tests\data\simple-game_FILES"
    let tstfol2 = @"D:\GitHub\Grampus\tests\data\simple-game2_FILES"
    
    [<TestMethod>]
    member this.Load() =
        let ans = Headers.Load(tstfol)
        ans.Length |> should equal 1
    
    [<TestMethod>]
    member this.Save() =
        let tstfn2 = Path.Combine(tstfol2, "HEADERS")
        if File.Exists(tstfn2) then File.Delete(tstfn2)
        let ans = File.Exists(tstfn2)
        ans |> should equal false
        let hdrs = Headers.Load(tstfol)
        Directory.CreateDirectory(tstfol2) |> ignore
        Headers.Save(tstfol2, hdrs)
        let ans = File.Exists(tstfn2)
        ans |> should equal true
        File.Delete(tstfn2)
