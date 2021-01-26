namespace FsChessPgn

open System
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

