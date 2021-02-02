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
        if Directory.Exists(binfol) then Directory.Delete(binfol, true)
        File.Delete(nm)
    
    let DeleteTree(nm : string) =
        let fol = Path.GetDirectoryName(nm)
        let binfol =
            Path.Combine(fol, Path.GetFileNameWithoutExtension(nm) + "_FILES")
        let trfol = Path.Combine(binfol, "tree")
        if Directory.Exists(trfol) then Directory.Delete(trfol, true)
        let gmp = Load(nm)
        
        let ngmp =
            { gmp with TreesCreated = None
                       TreesPly = 20 }
        Save(nm, ngmp)
        ngmp
    
    let DeleteFilters(nm : string) =
        let fol = Path.GetDirectoryName(nm)
        let binfol =
            Path.Combine(fol, Path.GetFileNameWithoutExtension(nm) + "_FILES")
        let ffol = Path.Combine(binfol, "filters")
        if Directory.Exists(ffol) then Directory.Delete(ffol, true)
        let gmp = Load(nm)
        
        let ngmp =
            { gmp with FiltersCreated = None
                       FiltersPly = 20 }
        Save(nm, ngmp)
        ngmp
    
    let DeleteGames(nm : string) =
        let fol = Path.GetDirectoryName(nm)
        let binfol =
            Path.Combine(fol, Path.GetFileNameWithoutExtension(nm) + "_FILES")
        if File.Exists(binfol + @"\GAMES") then File.Delete(binfol + @"\GAMES")
        if File.Exists(binfol + @"\INDEX") then File.Delete(binfol + @"\INDEX")
        if File.Exists(binfol + @"\HEADERS") then 
            File.Delete(binfol + @"\HEADERS")
        let gmp = Load(nm)
        let ngmp = { gmp with BaseCreated = None }
        Save(nm, ngmp)
        ngmp
    
    let DeleteGamesFilters(nm : string) =
        let gmp = DeleteFilters(nm)
        Save(nm, gmp)
        let ngmp = DeleteGames(nm)
        ngmp
