namespace GrampusInternal

open Grampus
open System.IO
open FSharp.Json

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module Grampus =
    
    let Load(nm):GrampusData =
        if File.Exists(nm) then 
            let str = File.ReadAllText(nm)  
            Json.deserialize (str)
        else GrampusDataEMP

    let Save(nm,gmp:GrampusData) =
        let str = Json.serialize gmp
        File.WriteAllText(nm, str)
        