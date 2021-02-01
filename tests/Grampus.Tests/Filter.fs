namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Filter() =
    let tstfol = @"D:\GitHub\Grampus\tests\data\simple-game_FILES"
    let ss1 =
        "RNBQKBNRPP.PPPPP..........P.....................pppppppprnbqkbnr b"
    
    [<TestMethod>]
    member this.Create() =
        let trfol = tstfol + @"\filters"
        if Directory.Exists(trfol) then Directory.Delete(trfol, true)
        let ans = Directory.Exists(trfol)
        ans |> should equal false
        Filters.Create(tstfol)
        let ans = Directory.Exists(trfol)
        ans |> should equal true
        Directory.Delete(trfol, true)
    
    [<TestMethod>]
    member this.Save() =
        let trfol = tstfol + @"\filters"
        if Directory.Exists(trfol) then Directory.Delete(trfol, true)
        let ans = Directory.Exists(trfol)
        ans |> should equal false
        Filters.Create(tstfol)
        Filters.Save([||], [||], tstfol)
        let ans = Directory.Exists(trfol)
        ans |> should equal true
        Directory.Delete(trfol, true)
    
    [<TestMethod>]
    member this.Read() =
        let trfol = tstfol + @"\filters"
        if Directory.Exists(trfol) then Directory.Delete(trfol, true)
        let ans = Directory.Exists(trfol)
        ans |> should equal false
        Filters.Create(tstfol)
        Filters.Save([| ss1 |], [| [ 0; 5 ] |], tstfol)
        let ans = Filters.Read(ss1, tstfol)
        ans.Length |> should equal 2
        ans.[0] |> should equal 0
        ans.[1] |> should equal 5
        let ans = Directory.Exists(trfol)
        ans |> should equal true
        Directory.Delete(trfol, true)
