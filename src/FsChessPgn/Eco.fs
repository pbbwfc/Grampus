namespace FsChessPgn

open System
open System.IO
open FsChess

module Eco =
    let ForGame (gm:EncodedGame) =
        let rec findeco (ipl:string list) =
            if List.isEmpty ipl then None
            else
                let p = ipl.Head
                if EcoMap.ContainsKey p then EcoMap.[p]|>Some
                else findeco ipl.Tail
        let posns = GameEncoded.GetPosns -1 gm
        let pl = posns|>List.ofArray|>List.rev
        let ecoopt = findeco pl
        let ngm =
            if ecoopt.IsNone then gm
            else
                let eco = ecoopt.Value
                let hdr = gm.Hdr
                let nhdr = {hdr with ECO=eco.Code;Opening=eco.Desc}
                {gm with Hdr=nhdr}
        ngm

    let ForBase (fol:string) =
        //load all the current files
        let iea = Index.Load(fol)
        let hdrs = Headers.Load(fol)
        //write in compacted format
        let updhdr i =
            let ie = iea.[i]
            let hdr = hdrs.[i]
            let gm = Games.LoadGame fol ie hdr
            let ngm = gm|>ForGame
            ngm.Hdr
        let nhdrs = [|0..iea.Length-1|]|>Array.map updhdr
        Headers.Save(fol,nhdrs)
