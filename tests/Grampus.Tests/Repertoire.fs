namespace Grampus.Tests

open FsUnit.MsTest
open Microsoft.VisualStudio.TestTools.UnitTesting
open Grampus
open System.IO

[<TestClass>]
type Repertoire() =
    let deffol =
        Path.Combine
            (System.Environment.GetFolderPath
                 (System.Environment.SpecialFolder.MyDocuments), 
             "Grampus\\repertoire")
    
    [<TestInitialize>]
    member this.Initialize() =
        GrampusInternal.Repertoire.setfol 
            @"D:\GitHub\Grampus\tests\data\repertoire"
    
    [<TestCleanup>]
    member this.Cleanup() = GrampusInternal.Repertoire.setfol deffol
    
    [<TestMethod>]
    member this.White() =
        let ans = Repertoire.White()
        ()
