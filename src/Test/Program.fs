
open System
open System.IO
open FsChess
open FSharp.Json

type Eco = {Code:string;Desc:string}

[<EntryPoint>]
let main argv =
    let pgnfol = @"d:\pgns" 

    let nm = "eco.pgn"
    let tmpfol = @"D:\tmp" 
    let pgn = Path.Combine(pgnfol,nm)
    let ugma = Pgn.Games.ReadSeqFromFile pgn
    let egma = ugma|>Seq.map(Game.Encode)|>Seq.toArray

    let getpn i (gm:EncodedGame) =
        let pns = Game.GetPosns i gm
        pns.[pns.Length-1]
    
    let geteco (gm:EncodedGame) =
        let code = gm.ECO
        let ai = gm.AdditionalInfo
        let desc = 
            if ai.ContainsKey("Variation") then 
                gm.AdditionalInfo.["Opening"] + " " + gm.AdditionalInfo.["Variation"]
            else gm.AdditionalInfo.["Opening"] 
        {Code=code;Desc=desc}

    let pns = egma|>Array.mapi getpn 
    
    let ecos = egma|>Array.map geteco


    0 // return an integer exit code
