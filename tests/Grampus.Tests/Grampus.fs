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
    let tstfol2 = @"D:\GitHub\Grampus\tests\data\simple-game2_FILES"
    
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
    member this.Copy() =
        if File.Exists(tstfn2) then File.Delete(tstfn2)
        let ans = File.Exists(tstfn2)
        ans |> should equal false
        Grampus.Copy(tstfn, tstfn2)
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
        Tree.Create(tstfn)
        let ans = Directory.Exists(trfol)
        ans |> should equal true
        let ngmp = Grampus.DeleteTree(tstfn)
        let ans = Directory.Exists(trfol)
        ans |> should equal false
        ngmp.TreesPly |> should equal 20
        ngmp.TreesCreated |> should equal None
    
    [<TestMethod>]
    member this.DeleteFilters() =
        let ffol = tstfol + @"\filters"
        if Directory.Exists(ffol) then Directory.Delete(ffol, true)
        let ans = Directory.Exists(ffol)
        ans |> should equal false
        Filters.Create(tstfol)
        let ans = Directory.Exists(ffol)
        ans |> should equal true
        let ngmp = Grampus.DeleteFilters(tstfn)
        let ans = Directory.Exists(ffol)
        ans |> should equal false
        ngmp.FiltersPly |> should equal 20
        ngmp.FiltersCreated |> should equal None
    
    [<TestMethod>]
    member this.DeleteGamesFilters() =
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
        Directory.CreateDirectory(tstfol2) |> ignore
        File.Copy(Path.Combine(tstfol, "INDEX"), Path.Combine(tstfol2, "INDEX"))
        File.Copy(Path.Combine(tstfol, "GAMES"), Path.Combine(tstfol2, "GAMES"))
        File.Copy
            (Path.Combine(tstfol, "HEADERS"), Path.Combine(tstfol2, "HEADERS"))
        let ffol = tstfol2 + @"\filters"
        if Directory.Exists(ffol) then Directory.Delete(ffol, true)
        let ans = Directory.Exists(ffol)
        ans |> should equal false
        Filters.Create(tstfol2)
        let ans = Directory.Exists(ffol)
        ans |> should equal true
        let ngmp = Grampus.DeleteGamesFilters(tstfn2)
        let ans = Directory.Exists(ffol)
        ans |> should equal false
        ngmp.FiltersPly |> should equal 20
        ngmp.FiltersCreated |> should equal None
        File.Exists(Path.Combine(tstfol2, "INDEX")) |> should equal false
        ngmp.BaseCreated |> should equal None
        if Directory.Exists(tstfol2) then Directory.Delete(tstfol2, true)
