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
        let binfol = getbinfol nm
        Directory.CreateDirectory(binfol) |> ignore
        let gmp = GrampusDataEMP
        Save(nm, gmp)
        gmp
    
    let Delete(nm : string) =
        let binfol = getbinfol nm
        if Directory.Exists(binfol) then Directory.Delete(binfol, true)
        File.Delete(nm)
    
    let Copy(nm : string, copynm : string) =
        let rec directoryCopy srcPath dstPath copySubDirs =
            if not <| System.IO.Directory.Exists(srcPath) then 
                let msg =
                    System.String.Format
                        ("Source directory does not exist or could not be found: {0}", 
                         srcPath)
                raise (System.IO.DirectoryNotFoundException(msg))
            if not <| System.IO.Directory.Exists(dstPath) then 
                System.IO.Directory.CreateDirectory(dstPath) |> ignore
            let srcDir = new System.IO.DirectoryInfo(srcPath)
            for file in srcDir.GetFiles() do
                let temppath = System.IO.Path.Combine(dstPath, file.Name)
                file.CopyTo(temppath, true) |> ignore
            if copySubDirs then 
                for subdir in srcDir.GetDirectories() do
                    let dstSubDir = System.IO.Path.Combine(dstPath, subdir.Name)
                    directoryCopy subdir.FullName dstSubDir copySubDirs
        
        let binfol = getbinfol nm
        let copybinfol = getbinfol copynm
        File.Copy(nm, copynm, true)
        directoryCopy binfol copybinfol true
    
    let DeleteTree(nm : string) =
        let trfol = gettrfol nm
        if Directory.Exists(trfol) then Directory.Delete(trfol, true)
        let gmp = Load(nm)
        
        let ngmp =
            { gmp with TreesCreated = None
                       TreesPly = 20 }
        Save(nm, ngmp)
        ngmp
    
    let DeleteFilters(nm : string) =
        let ffol = getffol nm
        if Directory.Exists(ffol) then Directory.Delete(ffol, true)
        let gmp = Load(nm)
        
        let ngmp =
            { gmp with FiltersCreated = None
                       FiltersPly = 20 }
        Save(nm, ngmp)
        ngmp
    
    let DeleteGames(nm : string) =
        let binfol = getbinfol nm
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
