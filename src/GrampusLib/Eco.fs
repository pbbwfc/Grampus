namespace GrampusInternal

open System
open System.IO
open Grampus

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module Eco =
    //Backup code if need to rebuild
    //let createeco() =
    //    let pgnfol = @"d:\pgns" 
    //    let nm = "eco.pgn"
    //    let pgn = Path.Combine(pgnfol,nm)
    //    let ugma = Pgn.Games.ReadSeqFromFile pgn
    //    let egma = ugma|>Seq.map(Game.Encode)|>Seq.toArray
    //    let getpn i (gm:EncodedGame) =
    //        let pns = Game.GetPosns i gm
    //        pns.[pns.Length-1]
    //    let geteco (gm:EncodedGame) =
    //        let code = gm.Hdr.ECO
    //        let ai = gm.AdditionalInfo
    //        let desc = 
    //            if ai.ContainsKey("Variation") then 
    //                gm.Hdr.Opening// + " " + gm.AdditionalInfo.["Variation"]
    //            else gm.Hdr.Opening
    //        {Code=code;Desc=desc}
    //    let pns = egma|>Array.mapi getpn 
    //    let ecos = egma|>Array.map geteco
    //    let map = Array.zip pns ecos|>Map.ofArray
    //    let json = Json.serialize map
    //    let ecofil = Path.Combine(pgnfol,"eco.json")
    //    File.WriteAllText(ecofil,json)
    let ForGame(gm : EncodedGame) =
        let rec findeco (ipl : string list) =
            if List.isEmpty ipl then None
            else 
                let p = ipl.Head
                if EcoMap.ContainsKey p then EcoMap.[p] |> Some
                else findeco ipl.Tail
        
        let posns = GameEncoded.GetPosns -1 gm
        
        let pl =
            posns
            |> List.ofArray
            |> List.rev
        
        let ecoopt = findeco pl
        
        let ngm =
            if ecoopt.IsNone then gm
            else 
                let eco = ecoopt.Value
                let hdr = gm.Hdr
                
                let nhdr =
                    { hdr with ECO = eco.Code
                               Opening = eco.Desc }
                { gm with Hdr = nhdr }
        ngm
    
    let ForBase(fol : string) cb =
        //load all the current files
        let iea = Index.Load(fol)
        let hdrs = Headers.Load(fol)
        
        //write in compacted format
        let updhdr i =
            let ie = iea.[i]
            let hdr = hdrs.[i]
            let gm = Games.LoadGame fol ie hdr
            let ngm = gm |> ForGame
            if i % 100 = 0 then cb (i)
            ngm.Hdr
        
        let nhdrs = [| 0..iea.Length - 1 |] |> Array.map updhdr
        Headers.Save(fol, nhdrs)
