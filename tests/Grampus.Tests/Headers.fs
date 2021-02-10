namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Headers() =
    let tstfol = @"D:\GitHub\Grampus\tests\data\simple-game_FILES"
    let tstfn = @"D:\GitHub\Grampus\tests\data\simple-game.grampus"
    let tstfol2 = @"D:\GitHub\Grampus\tests\data\simple-game2_FILES"
    let tstfn2 = @"D:\GitHub\Grampus\tests\data\simple-game2.grampus"
    
    [<TestMethod>]
    member this.Load() =
        let ans = Headers.Load tstfn
        ans.Length |> should equal 1
    
    [<TestMethod>]
    member this.Save() =
        let tstfnh2 = Path.Combine(tstfol2, "HEADERS")
        if File.Exists(tstfnh2) then File.Delete(tstfnh2)
        let ans = File.Exists(tstfnh2)
        ans |> should equal false
        let hdrs = Headers.Load tstfn
        Directory.CreateDirectory(tstfol2) |> ignore
        Headers.Save(tstfn2, hdrs)
        let ans = File.Exists(tstfnh2)
        ans |> should equal true
        File.Delete(tstfnh2)
