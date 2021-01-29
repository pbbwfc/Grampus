namespace GrampusInternal

open Grampus
open System.IO
open FSharp.Json

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module Repertoire =
    let mutable rfol =
        let def =
            Path.Combine
                (System.Environment.GetFolderPath
                     (System.Environment.SpecialFolder.MyDocuments), 
                 "Grampus\\repertoire")
        Directory.CreateDirectory(def) |> ignore
        def
    
    let mutable BlackRep : RepOpts * RepMove = Map.empty, Map.empty
    let mutable BlackErrors : string list = []
    let mutable WhiteRep : RepOpts * RepMove = Map.empty, Map.empty
    let mutable WhiteErrors : string list = []
    let setfol fol = rfol <- fol
    let whitedb = Path.Combine(rfol, "WhiteRep")
    let whiterep = Path.Combine(rfol, "whte.json")
    let whiteerrs = Path.Combine(rfol, "whteerrs.txt")
    let blackdb = Path.Combine(rfol, "BlackRep")
    let blackrep = Path.Combine(rfol, "blck.json")
    let blackerrs = Path.Combine(rfol, "blckerrs.txt")
    
    let LoadWhite() =
        if File.Exists(whiterep) then 
            let str = File.ReadAllText(whiterep)
            WhiteRep <- Json.deserialize (str)
            WhiteErrors <- File.ReadAllLines(whiteerrs) |> List.ofArray
    
    let LoadBlack() =
        if File.Exists(blackrep) then 
            let str = File.ReadAllText(blackrep)
            BlackRep <- Json.deserialize (str)
            BlackErrors <- File.ReadAllLines(blackerrs) |> List.ofArray
    
    let savewhite() =
        let str = Json.serialize WhiteRep
        File.WriteAllText(whiterep, str)
        File.WriteAllLines(whiteerrs, WhiteErrors |> List.toArray)
    
    let saveblack() =
        let str = Json.serialize BlackRep
        File.WriteAllText(blackrep, str)
        File.WriteAllLines(blackerrs, BlackErrors |> List.toArray)
    
    let optsHasSan (san : string) (opts : RepOpt list) =
        let filt = opts |> List.filter (fun op -> op.San = san)
        filt.Length > 0
    
    let UpdateWhite() =
        let rec domvt cemvo pbd cbd (imtel : EncodedMoveTextEntry list) 
                (repopts : RepOpts) (repmove : RepMove) =
            if List.isEmpty imtel then repopts, repmove
            else 
                let mte = imtel.Head
                match mte with
                | EncodedHalfMoveEntry(_, _, emv) -> 
                    let emvo = emv |> Some
                    let bdstr = cbd |> Board.ToSimpleStr
                    let nbd = emv.PostBrd
                    let isw = emv.Isw
                    let san = emv.San
                    
                    let cro =
                        { San = san
                          Nag = NAG.Null
                          Comm = "" }
                    //doing opts
                    if not isw then 
                        let nrepopts =
                            if repopts.ContainsKey(bdstr) then 
                                let curopts = repopts.[bdstr]
                                if not (optsHasSan san curopts) then 
                                    repopts.Add(bdstr, (cro :: curopts))
                                else repopts
                            else repopts.Add(bdstr, [ cro ])
                        domvt emvo cbd nbd imtel.Tail nrepopts repmove
                    //doing move
                    else 
                        let nrepmove =
                            if repmove.ContainsKey(bdstr) then 
                                let exro = repmove.[bdstr]
                                if cro.San <> exro.San then 
                                    BlackErrors <- ("Duplicate move found for bdstr " 
                                                    + bdstr + " with moves " 
                                                    + cro.San + " and " 
                                                    + exro.San + ".") 
                                                   :: BlackErrors
                                repmove
                            else repmove.Add(bdstr, cro)
                        domvt emvo cbd nbd imtel.Tail repopts nrepmove
                | EncodedRAVEntry(mtel) -> 
                    let nrepopts, nrepmove =
                        domvt cemvo pbd cbd mtel repopts repmove
                    domvt cemvo pbd cbd imtel.Tail nrepopts nrepmove
                | EncodedNAGEntry(ng) -> 
                    if cemvo.IsSome then 
                        let emv = cemvo.Value
                        let bdstr = pbd |> Board.ToSimpleStr
                        let san = emv.San
                        let isw = emv.Isw
                        if not isw then 
                            let curopts = repopts.[bdstr]
                            
                            let newopts =
                                curopts
                                |> List.map (fun ro -> 
                                       if ro.San = san then { ro with Nag = ng }
                                       else ro)
                            
                            let nrepopts = repopts.Add(bdstr, newopts)
                            domvt cemvo pbd cbd imtel.Tail nrepopts repmove
                        else 
                            let cro = repmove.[bdstr]
                            let nro = { cro with Nag = ng }
                            let nrepmove = repmove.Add(bdstr, nro)
                            domvt cemvo pbd cbd imtel.Tail repopts nrepmove
                    else domvt cemvo pbd cbd imtel.Tail repopts repmove
                | EncodedCommentEntry(cm) -> 
                    if cemvo.IsSome then 
                        let emv = cemvo.Value
                        let bdstr = cbd |> Board.ToSimpleStr
                        let san = emv.San
                        let isw = emv.Isw
                        if not isw then 
                            let curopts = repopts.[bdstr]
                            
                            let newopts =
                                curopts
                                |> List.map (fun ro -> 
                                       if ro.San = san then 
                                           { ro with Comm = cm }
                                       else ro)
                            
                            let nrepopts = repopts.Add(bdstr, newopts)
                            domvt cemvo pbd cbd imtel.Tail nrepopts repmove
                        else 
                            let cro = repmove.[bdstr]
                            let nro = { cro with Comm = cm }
                            let nrepmove = repmove.Add(bdstr, nro)
                            domvt cemvo pbd cbd imtel.Tail repopts nrepmove
                    else domvt cemvo pbd cbd imtel.Tail repopts repmove
                | _ -> domvt cemvo pbd cbd imtel.Tail repopts repmove
        
        let fol = whitedb + "_FILES"
        let iea = Index.Load(fol)
        let hdra = Headers.Load(fol)
        let numgames = iea.Length
        WhiteRep <- Map.empty, Map.empty
        WhiteErrors <- []
        for i = 0 to numgames - 1 do
            let gm = Games.LoadGame fol iea.[i] hdra.[i]
            let mvs = gm.MoveText
            
            let bd =
                if gm.BoardSetup.IsSome then gm.BoardSetup.Value
                else Board.Start
            
            let repopts, repmove = WhiteRep
            WhiteRep <- domvt None bd bd mvs repopts repmove
        savewhite()
        WhiteErrors.Length
    
    let UpdateBlack() =
        let rec domvt cemvo pbd cbd (imtel : EncodedMoveTextEntry list) 
                (repopts : RepOpts) (repmove : RepMove) =
            if List.isEmpty imtel then repopts, repmove
            else 
                let mte = imtel.Head
                match mte with
                | EncodedHalfMoveEntry(_, _, emv) -> 
                    let emvo = emv |> Some
                    let bdstr = cbd |> Board.ToSimpleStr
                    let nbd = emv.PostBrd
                    let isw = emv.Isw
                    let san = emv.San
                    
                    let cro =
                        { San = san
                          Nag = NAG.Null
                          Comm = "" }
                    //doing opts
                    if isw then 
                        let nrepopts =
                            if repopts.ContainsKey(bdstr) then 
                                let curopts = repopts.[bdstr]
                                if not (optsHasSan san curopts) then 
                                    repopts.Add(bdstr, (cro :: curopts))
                                else repopts
                            else repopts.Add(bdstr, [ cro ])
                        domvt emvo cbd nbd imtel.Tail nrepopts repmove
                    //doing move
                    else 
                        let nrepmove =
                            if repmove.ContainsKey(bdstr) then 
                                let exro = repmove.[bdstr]
                                if cro.San <> exro.San then 
                                    BlackErrors <- ("Duplicate move found for bdstr " 
                                                    + bdstr + " with moves " 
                                                    + cro.San + " and " 
                                                    + exro.San + ".") 
                                                   :: BlackErrors
                                repmove
                            else repmove.Add(bdstr, cro)
                        domvt emvo cbd nbd imtel.Tail repopts nrepmove
                | EncodedRAVEntry(mtel) -> 
                    let nrepopts, nrepmove =
                        domvt cemvo pbd cbd mtel repopts repmove
                    domvt cemvo pbd cbd imtel.Tail nrepopts nrepmove
                | EncodedNAGEntry(ng) -> 
                    if cemvo.IsSome then 
                        let emv = cemvo.Value
                        let bdstr = pbd |> Board.ToSimpleStr
                        let san = emv.San
                        let isw = emv.Isw
                        if isw then 
                            let curopts = repopts.[bdstr]
                            
                            let newopts =
                                curopts
                                |> List.map (fun ro -> 
                                       if ro.San = san then { ro with Nag = ng }
                                       else ro)
                            
                            let nrepopts = repopts.Add(bdstr, newopts)
                            domvt cemvo pbd cbd imtel.Tail nrepopts repmove
                        else 
                            let cro = repmove.[bdstr]
                            let nro = { cro with Nag = ng }
                            let nrepmove = repmove.Add(bdstr, nro)
                            domvt cemvo pbd cbd imtel.Tail repopts nrepmove
                    else domvt cemvo pbd cbd imtel.Tail repopts repmove
                | EncodedCommentEntry(cm) -> 
                    if cemvo.IsSome then 
                        let emv = cemvo.Value
                        let bdstr = pbd |> Board.ToSimpleStr
                        let san = emv.San
                        let isw = emv.Isw
                        if isw then 
                            let curopts = repopts.[bdstr]
                            
                            let newopts =
                                curopts
                                |> List.map (fun ro -> 
                                       if ro.San = san then 
                                           { ro with Comm = cm }
                                       else ro)
                            
                            let nrepopts = repopts.Add(bdstr, newopts)
                            domvt cemvo pbd cbd imtel.Tail nrepopts repmove
                        else 
                            let cro = repmove.[bdstr]
                            let nro = { cro with Comm = cm }
                            let nrepmove = repmove.Add(bdstr, nro)
                            domvt cemvo pbd cbd imtel.Tail repopts nrepmove
                    else domvt cemvo pbd cbd imtel.Tail repopts repmove
                | _ -> domvt cemvo pbd cbd imtel.Tail repopts repmove
        
        let fol = blackdb + "_FILES"
        let iea = Index.Load(fol)
        let hdra = Headers.Load(fol)
        let numgames = iea.Length
        BlackRep <- Map.empty, Map.empty
        BlackErrors <- []
        for i = 0 to numgames - 1 do
            let gm = Games.LoadGame fol iea.[i] hdra.[i]
            let mvs = gm.MoveText
            
            let bd =
                if gm.BoardSetup.IsSome then gm.BoardSetup.Value
                else Board.Start
            
            let repopts, repmove = BlackRep
            BlackRep <- domvt None bd bd mvs repopts repmove
        saveblack()
        BlackErrors.Length
