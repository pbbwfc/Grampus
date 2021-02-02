namespace GrampusBatch

open System.Windows.Forms
open System.IO
open FSharp.Json

module Recents =
    let mutable dbs : Set<string> = Set.empty
    let fol = Application.LocalUserAppDataPath
    let recfl = Path.Combine(fol, "recents.json")
    
    let getrecs() =
        if File.Exists(recfl) then 
            let str = File.ReadAllText(recfl)
            dbs <- Json.deserialize (str)
                   |> List.filter (fun db -> File.Exists(db))
                   |> Set.ofList
    
    let saverecs() =
        let str = Json.serialize (dbs |> Set.toList)
        File.WriteAllText(recfl, str)
    
    let addrec recent =
        getrecs()
        dbs <- dbs.Add recent
        saverecs()
