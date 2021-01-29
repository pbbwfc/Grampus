namespace ScincNet

open System.Windows.Forms
open System.IO
open FSharp.Json

module Recents =
    let mutable dbs : Set<string> = Set.empty
    let mutable trs : Set<string> = Set.empty
    let fol = Application.LocalUserAppDataPath
    let recfl = Path.Combine(fol, "recents.json")
    let trfl = Path.Combine(fol, "recenttrees.json")
    
    let getrecs() =
        if File.Exists(recfl) then 
            let str = File.ReadAllText(recfl)
            dbs <- Json.deserialize (str)
                   |> List.filter (fun db -> File.Exists(db))
                   |> Set.ofList
    
    let gettrs() =
        if File.Exists(trfl) then 
            let str = File.ReadAllText(trfl)
            trs <- Json.deserialize (str)
                   |> List.filter (fun db -> File.Exists(db))
                   |> Set.ofList
    
    let saverecs() =
        let str = Json.serialize (dbs |> Set.toList)
        File.WriteAllText(recfl, str)
    
    let savetrs() =
        let str = Json.serialize (trs |> Set.toList)
        File.WriteAllText(trfl, str)
    
    let addrec recent =
        getrecs()
        dbs <- dbs.Add recent
        saverecs()
    
    let addtr recent =
        gettrs()
        trs <- trs.Add recent
        savetrs()
