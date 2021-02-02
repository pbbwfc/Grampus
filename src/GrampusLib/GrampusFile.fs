namespace GrampusInternal

open Grampus
open System.IO
open FSharp.Json

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module GrampusFile =
    let Load(nm) : GrampusData =
        if File.Exists(nm) then 
            let str = File.ReadAllText(nm)
            Json.deserialize (str)
        else GrampusDataEMP
    
    let Save(nm, gmp : GrampusData) =
        let str = Json.serialize gmp
        File.WriteAllText(nm, str)
    
    let New(nm : string) : GrampusData =
        let fol = Path.GetDirectoryName(nm)
        let binfol =
            Path.Combine(fol, Path.GetFileNameWithoutExtension(nm) + "_FILES")
        Directory.CreateDirectory(binfol) |> ignore
        let gmp = GrampusDataEMP
        Save(nm, gmp)
        gmp
    
    let Delete(nm : string) =
        let fol = Path.GetDirectoryName(nm)
        let binfol =
            Path.Combine(fol, Path.GetFileNameWithoutExtension(nm) + "_FILES")
        Directory.Delete(binfol, true)
        File.Delete(nm)
