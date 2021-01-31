namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus

[<TestClass>]
type Repertoire() =
    
    [<TestInitialize>]
    member this.Initialize() =
        GrampusInternal.Repertoire.setfol 
            @"D:\GitHub\Grampus\tests\data\repertoire"
    
    [<TestMethod>]
    member this.White() =
        let ans = Repertoire.White()
        ()
