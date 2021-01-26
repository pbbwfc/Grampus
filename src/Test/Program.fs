
open System
open System.IO
open FsChess
open FSharp.Json

type Eco = {Code:string;Desc:string}

[<EntryPoint>]
let main argv =
    let createeco() =
        let pgnfol = @"d:\pgns" 
        let nm = "eco.pgn"
        let pgn = Path.Combine(pgnfol,nm)
        let ugma = Pgn.Games.ReadSeqFromFile pgn
        let egma = ugma|>Seq.map(Game.Encode)|>Seq.toArray
        let getpn i (gm:EncodedGame) =
            let pns = Game.GetPosns i gm
            pns.[pns.Length-1]
        let geteco (gm:EncodedGame) =
            let code = gm.Hdr.ECO
            let ai = gm.AdditionalInfo
            let desc = 
                if ai.ContainsKey("Variation") then 
                    gm.Hdr.Opening// + " " + gm.AdditionalInfo.["Variation"]
                else gm.Hdr.Opening
            {Code=code;Desc=desc}
        let pns = egma|>Array.mapi getpn 
        let ecos = egma|>Array.map geteco
        let map = Array.zip pns ecos|>Map.ofArray
        let json = Json.serialize map
        let ecofil = Path.Combine(pgnfol,"eco.json")
        File.WriteAllText(ecofil,json)
    
    let Load(fn:string):Map<string,Eco>=
        let str = File.ReadAllText(fn)  
        Json.deserialize (str)

    createeco()
    let pgnfol = @"d:\pgns" 
    let fn = Path.Combine(pgnfol,"eco.json")
    let map = Load(fn)

    let filt = map|>Map.filter(fun k v -> v.Desc.Length>40)


    0 // return an integer exit code
