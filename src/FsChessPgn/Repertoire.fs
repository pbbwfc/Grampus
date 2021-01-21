namespace FsChessPgn

open FsChess
open System.IO
open FSharp.Json

module Repertoire =

    let mutable rfol = 
        let def = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ScincNet\\repertoire")
        Directory.CreateDirectory(def)|>ignore
        def

    let mutable BlackRep:RepOpts*RepMove = Map.empty,Map.empty
    let mutable BlackErrors:string list = []
    let mutable WhiteRep:RepOpts*RepMove = Map.empty,Map.empty
    let mutable WhiteErrors:string list = []
    
    let setfol fol = rfol <- fol
    let whitedb = Path.Combine(rfol,"WhiteRep")
    let whiterep = Path.Combine(rfol,"whte.json")
    let whiteerrs = Path.Combine(rfol,"whteerrs.txt")
    let blackdb = Path.Combine(rfol,"BlackRep")
    let blackrep = Path.Combine(rfol,"blck.json")
    let blackerrs = Path.Combine(rfol,"blckerrs.txt")
    
    
    let LoadWhite() =
        if File.Exists(whiterep) then 
            let str = File.ReadAllText(whiterep)  
            WhiteRep <- Json.deserialize (str)
            WhiteErrors <- File.ReadAllLines(whiteerrs)|>List.ofArray

    let LoadBlack() =
        if File.Exists(blackrep) then 
            let str = File.ReadAllText(blackrep)  
            BlackRep <- Json.deserialize (str)
            BlackErrors <- File.ReadAllLines(blackerrs)|>List.ofArray

    let savewhite() =
        let str = Json.serialize WhiteRep
        File.WriteAllText(whiterep, str)
        File.WriteAllLines(whiteerrs,WhiteErrors|>List.toArray)
    
    let saveblack() =
        let str = Json.serialize BlackRep
        File.WriteAllText(blackrep, str)
        File.WriteAllLines(blackerrs,BlackErrors|>List.toArray)
    
    let optsHasSan (san:string) (opts:RepOpt list) =
        let filt = opts|>List.filter(fun op -> op.San=san)
        filt.Length>0
    
    //let UpdateWhite () =
    //    let rec domvt cemvo cbd (imtel:EncodedMoveTextEntry list) (repopts:RepOpts) (repmove:RepMove) = 
    //        if List.isEmpty imtel then repopts,repmove
    //        else
    //            let mte = imtel.Head
    //            match mte with
    //            |EncodedHalfMoveEntry(_,_,emv) -> 
    //                let emvo = emv|>Some
    //                let fen = cbd|>Board.ToStr
    //                let nbd = emv.PostBrd
    //                let isw = emv.Isw
    //                let san = emv.San
    //                let cro = {San = san; Nag = NAG.Null; Comm = ""}
    //                //doing opts
    //                if not isw then
    //                    let nrepopts = 
    //                        if repopts.ContainsKey(fen) then
    //                            let curopts = repopts.[fen]
    //                            if not (optsHasSan san curopts) then
    //                                repopts.Add(fen,(cro::curopts))
    //                            else repopts
    //                        else
    //                            repopts.Add(fen,[cro])
    //                    domvt emvo nbd imtel.Tail nrepopts repmove
    //                //doing move
    //                else
    //                    let nrepmove =
    //                        if repmove.ContainsKey(fen) then
    //                            let exro = repmove.[fen]
    //                            if cro.San<>exro.San then
    //                                BlackErrors<-("Duplicate move found for fen " + fen + " with moves " + cro.San + " and " + exro.San + ".")::BlackErrors
    //                            repmove
    //                        else
    //                            repmove.Add(fen,cro)
    //                    domvt emvo nbd imtel.Tail repopts nrepmove
    //            |EncodedRAVEntry(mtel) -> 
    //                let nrepopts,nrepmove = domvt cemvo cbd mtel repopts repmove
    //                domvt cemvo cbd imtel.Tail nrepopts nrepmove
    //            |EncodedNAGEntry(ng) -> 
    //                if cemvo.IsSome then
    //                    let emv = cemvo.Value
    //                    let fen = cbd|>Board.ToStr
    //                    let san = emv.San
    //                    let isw = emv.Isw
    //                    if not isw then
    //                        let curopts = repopts.[fen]
    //                        let newopts = curopts|>List.map(fun ro -> if ro.San=san then {ro with Nag=ng} else ro)
    //                        let nrepopts = repopts.Add(fen,newopts)
    //                        domvt cemvo cbd imtel.Tail nrepopts repmove
    //                    else
    //                        let cro = repmove.[fen]
    //                        let nro = {cro with Nag=ng}
    //                        let nrepmove = repmove.Add(fen,nro)
    //                        domvt cemvo cbd imtel.Tail repopts nrepmove
    //                else domvt cemvo cbd imtel.Tail repopts repmove
    //            |EncodedCommentEntry(cm) -> 
    //                if cemvo.IsSome then
    //                    let emv = cemvo.Value
    //                    let fen = cbd|>Board.ToStr
    //                    let san = emv.San
    //                    let isw = emv.Isw
    //                    if not isw then
    //                        let curopts = repopts.[fen]
    //                        let newopts = curopts|>List.map(fun ro -> if ro.San=san then {ro with Comm=cm} else ro)
    //                        let nrepopts = repopts.Add(fen,newopts)
    //                        domvt cemvo cbd imtel.Tail nrepopts repmove
    //                    else
    //                        let cro = repmove.[fen]
    //                        let nro = {cro with Comm=cm}
    //                        let nrepmove = repmove.Add(fen,nro)
    //                        domvt cemvo cbd imtel.Tail repopts nrepmove
    //                else domvt cemvo cbd imtel.Tail repopts repmove
    //            |_ -> domvt cemvo cbd imtel.Tail repopts repmove
    //    ScincFuncs.Base.Open(whitedb)|>ignore
    //    let numgames = ScincFuncs.Base.NumGames()
    //    WhiteRep <- Map.empty,Map.empty
    //    WhiteErrors <- []
    //    for i = 1 to numgames do
    //        ScincFuncs.ScidGame.Load(uint(i))|>ignore
    //        let mutable pgnstr = ""
    //        ScincFuncs.ScidGame.Pgn(&pgnstr)|>ignore
    //        let gm = RegParse.GameFromString(pgnstr)
    //        let mvs = (gm|>GameEncoded.Encode).MoveText
    //        let bd = if gm.BoardSetup.IsSome then gm.BoardSetup.Value else Board.Start
    //        let repopts,repmove = WhiteRep
    //        WhiteRep <- domvt None bd mvs repopts repmove
    //    savewhite()
    //    ScincFuncs.Base.Close()|>ignore
    //    WhiteErrors.Length

    //let UpdateBlack () =
    //    let rec domvt cemvo cbd (imtel:EncodedMoveTextEntry list) (repopts:RepOpts) (repmove:RepMove) = 
    //        if List.isEmpty imtel then repopts,repmove
    //        else
    //            let mte = imtel.Head
    //            match mte with
    //            |EncodedHalfMoveEntry(_,_,emv) -> 
    //                let emvo = emv|>Some
    //                let fen = cbd|>Board.ToStr
    //                let nbd = emv.PostBrd
    //                let isw = emv.Isw
    //                let san = emv.San
    //                let cro = {San = san; Nag = NAG.Null; Comm = ""}
    //                //doing opts
    //                if isw then
    //                    let nrepopts = 
    //                        if repopts.ContainsKey(fen) then
    //                            let curopts = repopts.[fen]
    //                            if not (optsHasSan san curopts) then
    //                                repopts.Add(fen,(cro::curopts))
    //                            else repopts
    //                        else
    //                            repopts.Add(fen,[cro])
    //                    domvt emvo nbd imtel.Tail nrepopts repmove
    //                //doing move
    //                else
    //                    let nrepmove =
    //                        if repmove.ContainsKey(fen) then
    //                            let exro = repmove.[fen]
    //                            if cro.San<>exro.San then
    //                                BlackErrors<-("Duplicate move found for fen " + fen + " with moves " + cro.San + " and " + exro.San + ".")::BlackErrors
    //                            repmove
    //                        else
    //                            repmove.Add(fen,cro)
    //                    domvt cemvo nbd imtel.Tail repopts nrepmove
    //            |EncodedRAVEntry(mtel) -> 
    //                let nrepopts,nrepmove = domvt cemvo cbd mtel repopts repmove
    //                domvt cemvo cbd imtel.Tail nrepopts nrepmove
    //            |EncodedNAGEntry(ng) -> 
    //                if cemvo.IsSome then
    //                    let emv = cemvo.Value
    //                    let fen = cbd|>Board.ToStr
    //                    let san = emv.San
    //                    let isw = emv.Isw
    //                    if isw then
    //                        let curopts = repopts.[fen]
    //                        let newopts = curopts|>List.map(fun ro -> if ro.San=san then {ro with Nag=ng} else ro)
    //                        let nrepopts = repopts.Add(fen,newopts)
    //                        domvt cemvo cbd imtel.Tail nrepopts repmove
    //                    else
    //                        let cro = repmove.[fen]
    //                        let nro = {cro with Nag=ng}
    //                        let nrepmove = repmove.Add(fen,nro)
    //                        domvt cemvo cbd imtel.Tail repopts nrepmove
    //                else domvt cemvo cbd imtel.Tail repopts repmove
    //            |EncodedCommentEntry(cm) -> 
    //                if cemvo.IsSome then
    //                    let emv = cemvo.Value
    //                    let fen = cbd|>Board.ToStr
    //                    let san = emv.San
    //                    let isw = emv.Isw
    //                    if isw then
    //                        let curopts = repopts.[fen]
    //                        let newopts = curopts|>List.map(fun ro -> if ro.San=san then {ro with Comm=cm} else ro)
    //                        let nrepopts = repopts.Add(fen,newopts)
    //                        domvt cemvo cbd imtel.Tail nrepopts repmove
    //                    else
    //                        let cro = repmove.[fen]
    //                        let nro = {cro with Comm=cm}
    //                        let nrepmove = repmove.Add(fen,nro)
    //                        domvt cemvo cbd imtel.Tail repopts nrepmove
    //                else domvt cemvo cbd imtel.Tail repopts repmove
    //            |_ -> domvt cemvo cbd imtel.Tail repopts repmove
    //    ScincFuncs.Base.Open(blackdb)|>ignore
    //    let numgames = ScincFuncs.Base.NumGames()
    //    BlackRep <- Map.empty,Map.empty
    //    BlackErrors <- []
    //    for i = 1 to numgames do
    //        ScincFuncs.ScidGame.Load(uint(i))|>ignore
    //        let mutable pgnstr = ""
    //        ScincFuncs.ScidGame.Pgn(&pgnstr)|>ignore
    //        let gm = RegParse.GameFromString(pgnstr)
    //        let mvs = (gm|>GameEncoded.Encode).MoveText
    //        let bd = if gm.BoardSetup.IsSome then gm.BoardSetup.Value else Board.Start
    //        let repopts,repmove = BlackRep
    //        BlackRep <- domvt None bd mvs repopts repmove
    //    saveblack()
    //    ScincFuncs.Base.Close()|>ignore
    //    BlackErrors.Length
