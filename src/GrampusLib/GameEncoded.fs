namespace GrampusInternal

open Grampus

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module GameEncoded =
    let Start = EncodedGameEMP
    
    let Encode(gm : UnencodedGame) =
        let rec setemv (pmvl : UnencodedMoveTextEntry list) mct prebd bd oemvl =
            if pmvl |> List.isEmpty then oemvl |> List.rev
            else 
                let mte = pmvl.Head
                match mte with
                | UnencodedHalfMoveEntry(mn, ic, mv) -> 
                    let emv = mv |> pMove.Encode bd mct
                    let nmte = EncodedHalfMoveEntry(mn, ic, emv)
                    
                    let nmct =
                        if bd.WhosTurn = Player.White then mct
                        else mct + 1
                    setemv pmvl.Tail nmct bd emv.PostBrd (nmte :: oemvl)
                | UnencodedRAVEntry(mtel) -> 
                    let nmct =
                        if prebd.WhosTurn = Player.Black then mct - 1
                        else mct
                    
                    let nmtel = setemv mtel nmct prebd prebd []
                    let nmte = EncodedRAVEntry(nmtel)
                    setemv pmvl.Tail mct prebd bd (nmte :: oemvl)
                | UnencodedCommentEntry(c) -> 
                    setemv pmvl.Tail mct prebd bd 
                        (EncodedCommentEntry(c) :: oemvl)
                | UnencodedGameEndEntry(r) -> 
                    setemv pmvl.Tail mct prebd bd 
                        (EncodedGameEndEntry(r) :: oemvl)
                | UnencodedNAGEntry(n) -> 
                    setemv pmvl.Tail mct prebd bd (EncodedNAGEntry(n) :: oemvl)
        
        let ibd =
            if gm.BoardSetup.IsSome then gm.BoardSetup.Value
            else Board.Start
        
        let nmt =
            try 
                setemv gm.MoveText 1 ibd ibd []
            with ex -> 
                let msg = ex.Message
                []
        
        let egm0 = EncodedGameEMP
        
        let egm1 =
            { egm0 with Hdr = gm.Hdr
                        Month = gm.Month
                        Day = gm.Day
                        Round = gm.Round
                        Site = gm.Site
                        BoardSetup = gm.BoardSetup
                        AdditionalInfo = gm.AdditionalInfo
                        MoveText = nmt }
        egm1
    
    let AddRav (gm : EncodedGame) (irs : int list) (mv : Move) =
        let rec getadd mct ci nmte (imtel : EncodedMoveTextEntry list) 
                (omtel : EncodedMoveTextEntry list) =
            if ci > omtel.Length then 
                getadd mct ci nmte imtel.Tail (imtel.Head :: omtel)
            elif imtel.IsEmpty then 
                (EncodedRAVEntry([ nmte ]) :: omtel) |> List.rev, omtel.Length
            else 
                //ignore first move
                let mte = imtel.Head
                if mct = 0 then 
                    match mte with
                    | EncodedHalfMoveEntry(_) -> 
                        getadd 1 ci nmte imtel.Tail (imtel.Head :: omtel)
                    | _ -> getadd 0 ci nmte imtel.Tail (imtel.Head :: omtel)
                else 
                    match mte with
                    | EncodedGameEndEntry(_) -> 
                        ((EncodedRAVEntry([ nmte ]) :: omtel) |> List.rev) 
                        @ imtel, omtel.Length
                    | EncodedHalfMoveEntry(_, _, emv) -> 
                        //need to fix black move
                        let mn, isw = emv.Mno, emv.Isw
                        
                        let fmte =
                            if isw then mte
                            else EncodedHalfMoveEntry(mn |> Some, true, emv)
                        (fmte :: (EncodedRAVEntry([ nmte ]) :: omtel) 
                         |> List.rev) @ imtel.Tail, omtel.Length
                    | _ -> getadd 1 ci nmte imtel.Tail (imtel.Head :: omtel)
        if irs.Length = 1 then 
            //before moves
            if irs.Head = -1 then 
                let emv = MoveEncoded.FromMove Board.Start 1 mv
                let nmte = EncodedHalfMoveEntry(1 |> Some, false, emv)
                { gm with MoveText =
                              gm.MoveText.Head 
                              :: EncodedRAVEntry([ nmte ]) :: gm.MoveText.Tail }, 
                [ 1; 0 ]
            else 
                let cmv = gm.MoveText.[irs.Head]
                
                let bd, lmn, lisw =
                    match cmv with
                    | EncodedHalfMoveEntry(_, _, emv) -> 
                        emv.PostBrd, emv.Mno, emv.Isw
                    | _ -> failwith "should be a move"
                
                let mn =
                    if lisw then lmn
                    else lmn + 1
                
                let emv = mv |> MoveEncoded.FromMove bd mn
                let nmte =
                    EncodedHalfMoveEntry
                        (mn |> Some, bd.WhosTurn = Player.Black, emv)
                let nmtel, ni = getadd 0 (irs.Head + 1) nmte gm.MoveText []
                { gm with MoveText = nmtel }, [ ni; 0 ]
        else 
            let rec getcur indx (cirs : int list) 
                    (mtel : EncodedMoveTextEntry list) =
                if cirs.Length = 1 && indx = cirs.Head then mtel.Head
                elif indx = cirs.Head then 
                    let rv = mtel.Head
                    match rv with
                    | EncodedRAVEntry(nmtel) -> getcur 0 cirs.Tail nmtel
                    | _ -> failwith "should be RAV"
                else 
                    let mte = mtel.Head
                    match mte with
                    | EncodedHalfMoveEntry(_) -> 
                        getcur (indx + 1) cirs mtel.Tail
                    | _ -> getcur (indx + 1) cirs mtel.Tail
            
            let cmv = getcur 0 irs gm.MoveText
            
            let bd, lmn, lisw =
                match cmv with
                | EncodedHalfMoveEntry(_, _, emv) -> 
                    emv.PostBrd, emv.Mno, emv.Isw
                | _ -> failwith "should be a move"
            
            let mn =
                if lisw then lmn
                else lmn + 1
            
            let emv = mv |> MoveEncoded.FromMove bd mn
            let nmte =
                EncodedHalfMoveEntry
                    (mn |> Some, bd.WhosTurn = Player.Black, emv)
            
            let rec getnmtel (cirs : int list) 
                    (mtel : EncodedMoveTextEntry list) =
                if cirs.Length = 1 then 
                    let nmtel, _ = getadd 0 (cirs.Head + 1) nmte mtel []
                    nmtel
                else 
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    | EncodedRAVEntry(nmtel) -> 
                        mtel.[..i - 1] 
                        @ [ EncodedRAVEntry(getnmtel cirs.Tail nmtel) ] 
                          @ mtel.[i + 1..]
                    | _ -> failwith "should be RAV"
            
            let rec getnmirs (cirs : int list) (pirs : int list) 
                    (mtel : EncodedMoveTextEntry list) =
                if cirs.Length = 1 then 
                    let _, ni = getadd 0 (cirs.Head + 1) nmte mtel []
                    (0 :: ni :: pirs) |> List.rev
                else 
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    | EncodedRAVEntry(nmtel) -> 
                        getnmirs cirs.Tail (i :: pirs) nmtel
                    | _ -> failwith "should be RAV"
            
            let nmtel = getnmtel irs gm.MoveText
            let nirs = getnmirs irs [] gm.MoveText
            { gm with MoveText = nmtel }, nirs
    
    let DeleteRav (gm : EncodedGame) (iirs : int list) =
        if iirs.Length = 1 then { gm with MoveText = [] }
        else 
            let irs = iirs.[..iirs.Length - 2]
            if irs.Length = 1 then 
                let i = irs.Head
                
                let nmtel =
                    if i = 0 then gm.MoveText.Tail
                    else 
                        //fix Black by removing mn and cont
                        let pmte =
                            let hm = gm.MoveText.[i + 1]
                            match hm with
                            | EncodedHalfMoveEntry(_, _, emv) -> 
                                let isw = emv.Isw
                                if isw then hm
                                else EncodedHalfMoveEntry(None, false, emv)
                            | _ -> hm
                        gm.MoveText.[..i - 1] @ [ pmte ] @ gm.MoveText.[i + 2..]
                { gm with MoveText = nmtel }
            else 
                let rec getnmtel (cirs : int list) 
                        (mtel : EncodedMoveTextEntry list) =
                    if cirs.Length = 1 then 
                        let i = cirs.Head
                        
                        let nmtel =
                            if i = 0 then mtel.Tail
                            elif i = mtel.Length - 1 then mtel.[..i - 1]
                            else 
                                //fix Black by removing mn and cont
                                let pmte =
                                    let hm = gm.MoveText.[i + 1]
                                    match hm with
                                    | EncodedHalfMoveEntry(_, _, emv) -> 
                                        let isw = emv.Isw
                                        if isw then hm
                                        else 
                                            EncodedHalfMoveEntry
                                                (None, false, emv)
                                    | _ -> hm
                                mtel.[..i - 1] @ [ pmte ] @ mtel.[i + 2..]
                        nmtel
                    else 
                        let i = cirs.Head
                        let rav = mtel.[i]
                        match rav with
                        | EncodedRAVEntry(nmtel) -> 
                            mtel.[..i - 1] 
                            @ [ EncodedRAVEntry(getnmtel cirs.Tail nmtel) ] 
                              @ mtel.[i + 1..]
                        | _ -> failwith "should be RAV"
                
                let nmtel = getnmtel irs gm.MoveText
                { gm with MoveText = nmtel }
    
    let fixnums (gm : EncodedGame) =
        let rec fixemv pmvo (emvl : EncodedMoveTextEntry list) oemvl =
            if emvl |> List.isEmpty then oemvl |> List.rev
            else 
                let mte = emvl.Head
                match mte with
                | EncodedRAVEntry(mtel) -> 
                    let nmtel = fixemv None mtel []
                    let nmte = EncodedRAVEntry(nmtel)
                    fixemv None emvl.Tail (nmte :: oemvl)
                | EncodedHalfMoveEntry(n, b, mv) -> 
                    if pmvo.IsSome then 
                        match pmvo.Value with
                        | EncodedCommentEntry(_) -> 
                            fixemv (Some(mte)) emvl.Tail (mte :: oemvl)
                        | _ -> 
                            let nmte =
                                if mv.Isw then mte
                                else EncodedHalfMoveEntry(None, false, mv)
                            fixemv (Some(nmte)) emvl.Tail (nmte :: oemvl)
                    else fixemv (Some(mte)) emvl.Tail (mte :: oemvl)
                | _ -> fixemv (Some(mte)) emvl.Tail (mte :: oemvl)
        
        let nmtel = fixemv None gm.MoveText []
        { gm with MoveText = nmtel }
    
    let RemoveRavs(gm : EncodedGame) =
        let rec setemv (emvl : EncodedMoveTextEntry list) oemvl =
            if emvl |> List.isEmpty then oemvl |> List.rev
            else 
                let mte = emvl.Head
                match mte with
                | EncodedRAVEntry(mtel) -> setemv emvl.Tail oemvl
                | _ -> setemv emvl.Tail (mte :: oemvl)
        
        let nmtel = setemv gm.MoveText []
        let ngm = { gm with MoveText = nmtel }
        ngm |> fixnums
    
    let AddMv (gm : EncodedGame) (irs : int list) (mv : Move) =
        let rec getext ci nmte (imtel : EncodedMoveTextEntry list) 
                (omtel : EncodedMoveTextEntry list) =
            if ci > omtel.Length then 
                getext ci nmte imtel.Tail (imtel.Head :: omtel)
            elif imtel.IsEmpty then 
                //need to remove iscont and mn if black, if after a move
                match omtel.Head with
                | EncodedHalfMoveEntry(_) | EncodedNAGEntry(_) -> 
                    match nmte with
                    | EncodedHalfMoveEntry(mn, _, emv) -> 
                        let nmn =
                            if emv.PostBrd.WhosTurn = Player.White then None
                            else mn
                        (EncodedHalfMoveEntry(nmn, false, emv) :: omtel) 
                        |> List.rev, omtel.Length
                    | _ -> failwith "can't reach here"
                | _ -> (nmte :: omtel) |> List.rev, omtel.Length
            else 
                let mte = imtel.Head
                match mte with
                | EncodedGameEndEntry(_) -> 
                    //need to include before this
                    if omtel.Length = 0 then 
                        ((nmte :: omtel) |> List.rev) @ imtel, omtel.Length
                    else 
                        //need to remove iscont if after a move
                        match omtel.Head with
                        | EncodedHalfMoveEntry(_) | EncodedNAGEntry(_) -> 
                            match nmte with
                            | EncodedHalfMoveEntry(mn, _, emv) -> 
                                let nmn =
                                    if emv.PostBrd.WhosTurn = Player.White then 
                                        None
                                    else mn
                                ((EncodedHalfMoveEntry(nmn, false, emv) :: omtel) 
                                 |> List.rev) @ imtel, omtel.Length
                            | _ -> failwith "can't reach here"
                        | _ -> 
                            ((nmte :: omtel) |> List.rev) @ imtel, omtel.Length
                | _ -> getext ci nmte imtel.Tail (imtel.Head :: omtel)
        if irs.Length = 1 then 
            //allow for empty list
            if gm.MoveText.IsEmpty then 
                let bd =
                    if gm.BoardSetup.IsSome then gm.BoardSetup.Value
                    else Board.Start
                
                let mn = 1
                let emv = mv |> MoveEncoded.FromMove bd mn
                let nmte =
                    EncodedHalfMoveEntry
                        (mn |> Some, bd.WhosTurn = Player.Black, emv)
                { gm with MoveText = [ nmte ] }, [ 0 ]
            //if not with a selected move
            elif irs.Head = -1 then 
                let bd =
                    if gm.BoardSetup.IsSome then gm.BoardSetup.Value
                    else Board.Start
                
                let mn = 1
                let emv = mv |> MoveEncoded.FromMove bd mn
                let nmte =
                    EncodedHalfMoveEntry
                        (mn |> Some, bd.WhosTurn = Player.Black, emv)
                let nmtel, ni = getext (irs.Head + 1) nmte gm.MoveText []
                { gm with MoveText = nmtel }, [ ni ]
            else 
                let cmv = gm.MoveText.[irs.Head]
                
                let bd, lmn, lisw =
                    match cmv with
                    | EncodedHalfMoveEntry(_, _, emv) -> 
                        emv.PostBrd, emv.Mno, emv.Isw
                    | _ -> failwith "should be a move"
                
                let mn =
                    if lisw then lmn
                    else lmn + 1
                
                let emv = mv |> MoveEncoded.FromMove bd mn
                let nmte =
                    EncodedHalfMoveEntry
                        (mn |> Some, bd.WhosTurn = Player.Black, emv)
                let nmtel, ni = getext (irs.Head + 1) nmte gm.MoveText []
                { gm with MoveText = nmtel }, [ ni ]
        else 
            let rec getcur indx (cirs : int list) 
                    (mtel : EncodedMoveTextEntry list) =
                if cirs.Length = 1 && indx = cirs.Head then mtel.Head
                elif indx = cirs.Head then 
                    let rv = mtel.Head
                    match rv with
                    | EncodedRAVEntry(nmtel) -> getcur 0 cirs.Tail nmtel
                    | _ -> failwith "should be RAV"
                else 
                    let mte = mtel.Head
                    match mte with
                    | EncodedHalfMoveEntry(_) -> 
                        getcur (indx + 1) cirs mtel.Tail
                    | _ -> getcur (indx + 1) cirs mtel.Tail
            
            let cmv = getcur 0 irs gm.MoveText
            
            let bd, lmn, lisw =
                match cmv with
                | EncodedHalfMoveEntry(_, _, emv) -> 
                    emv.PostBrd, emv.Mno, emv.Isw
                | _ -> failwith "should be a move"
            
            let mn =
                if lisw then lmn
                else lmn + 1
            
            let emv = mv |> MoveEncoded.FromMove bd mn
            let nmte =
                EncodedHalfMoveEntry
                    (mn |> Some, bd.WhosTurn = Player.Black, emv)
            
            let rec getnmtel (cirs : int list) 
                    (mtel : EncodedMoveTextEntry list) =
                if cirs.Length = 1 then 
                    let nmtel, _ = getext (cirs.Head + 1) nmte mtel []
                    nmtel
                else 
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    | EncodedRAVEntry(nmtel) -> 
                        mtel.[..i - 1] 
                        @ [ EncodedRAVEntry(getnmtel cirs.Tail nmtel) ] 
                          @ mtel.[i + 1..]
                    | _ -> failwith "should be RAV"
            
            let rec getnmirs (cirs : int list) (pirs : int list) 
                    (mtel : EncodedMoveTextEntry list) =
                if cirs.Length = 1 then 
                    let _, ni = getext (cirs.Head + 1) nmte mtel []
                    (ni :: pirs) |> List.rev
                else 
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    | EncodedRAVEntry(nmtel) -> 
                        getnmirs cirs.Tail (i :: pirs) nmtel
                    | _ -> failwith "should be RAV"
            
            let nmtel = getnmtel irs gm.MoveText
            let nirs = getnmirs irs [] gm.MoveText
            { gm with MoveText = nmtel }, nirs
    
    let CommentBefore (gm : EncodedGame) (irs : int list) (str : string) =
        let mte = EncodedCommentEntry(str)
        if irs.Length = 1 then 
            //allow for empty list
            if gm.MoveText.IsEmpty then { gm with MoveText = [ mte ] }
            else 
                let i = irs.Head
                //fix Black by adding mn and cont
                let hm = gm.MoveText.[i]
                
                let nhm =
                    match hm with
                    | EncodedHalfMoveEntry(mn, ic, emv) -> 
                        let mn, isw = emv.Mno, emv.Isw
                        if isw then hm
                        else EncodedHalfMoveEntry(mn |> Some, true, emv)
                    | _ -> failwith "should be a move"
                
                let nmtel =
                    if i = 0 then mte :: nhm :: gm.MoveText.Tail
                    else 
                        gm.MoveText.[..i - 1] 
                        @ [ mte; nhm ] @ gm.MoveText.[i + 1..]
                
                { gm with MoveText = nmtel }
        else 
            let rec getnmtel (cirs : int list) 
                    (mtel : EncodedMoveTextEntry list) =
                if cirs.Length = 1 then 
                    let i = cirs.Head
                    //fix Black by adding mn and cont
                    let hm = mtel.[i]
                    
                    let nhm =
                        match hm with
                        | EncodedHalfMoveEntry(mn, ic, emv) -> 
                            let mn, isw = emv.Mno, emv.Isw
                            if isw then hm
                            else EncodedHalfMoveEntry(mn |> Some, true, emv)
                        | _ -> failwith "should be a move"
                    
                    let nmtel =
                        if i = 0 then mte :: nhm :: mtel.Tail
                        else 
                            //TODO fix Black by adding mn and cont
                            mtel.[..i - 1] @ [ mte; nhm ] @ mtel.[i + 1..]
                    
                    nmtel
                else 
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    | EncodedRAVEntry(nmtel) -> 
                        mtel.[..i - 1] 
                        @ [ EncodedRAVEntry(getnmtel cirs.Tail nmtel) ] 
                          @ mtel.[i + 1..]
                    | _ -> failwith "should be RAV"
            
            let nmtel = getnmtel irs gm.MoveText
            { gm with MoveText = nmtel }
    
    let CommentAfter (gm : EncodedGame) (irs : int list) (str : string) =
        let mte = EncodedCommentEntry(str)
        if irs.Length = 1 then 
            //allow for empty list
            if gm.MoveText.IsEmpty then { gm with MoveText = [ mte ] }
            else 
                let i = irs.Head
                
                let nmtel =
                    if i = gm.MoveText.Length - 1 then gm.MoveText @ [ mte ]
                    else 
                        //fix Black by adding mn and cont
                        let pmte =
                            let hm = gm.MoveText.[i + 1]
                            match hm with
                            | EncodedHalfMoveEntry(mn, ic, emv) -> 
                                let mn, isw = emv.Mno, emv.Isw
                                if isw then hm
                                else EncodedHalfMoveEntry(mn |> Some, true, emv)
                            | _ -> hm
                        gm.MoveText.[..i] 
                        @ [ mte; pmte ] @ gm.MoveText.[i + 2..]
                { gm with MoveText = nmtel }
        else 
            let rec getnmtel (cirs : int list) 
                    (mtel : EncodedMoveTextEntry list) =
                if cirs.Length = 1 then 
                    let i = cirs.Head
                    
                    let nmtel =
                        if i = mtel.Length - 1 then mtel @ [ mte ]
                        else 
                            //fix Black by adding mn and cont
                            let pmte =
                                let hm = mtel.[i + 1]
                                match hm with
                                | EncodedHalfMoveEntry(mn, ic, emv) -> 
                                    let mn, isw = emv.Mno, emv.Isw
                                    if isw then hm
                                    else 
                                        EncodedHalfMoveEntry
                                            (mn |> Some, true, emv)
                                | _ -> hm
                            mtel.[..i] @ [ mte; pmte ] @ mtel.[i + 2..]
                    nmtel
                else 
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    | EncodedRAVEntry(nmtel) -> 
                        mtel.[..i - 1] 
                        @ [ EncodedRAVEntry(getnmtel cirs.Tail nmtel) ] 
                          @ mtel.[i + 1..]
                    | _ -> failwith "should be RAV"
            
            let nmtel = getnmtel irs gm.MoveText
            { gm with MoveText = nmtel }
    
    let EditComment (gm : EncodedGame) (irs : int list) (str : string) =
        let mte = EncodedCommentEntry(str)
        if irs.Length = 1 then 
            //allow for empty list
            if gm.MoveText.IsEmpty then { gm with MoveText = [ mte ] }
            else 
                let i = irs.Head
                
                let nmtel =
                    if i = 0 then mte :: gm.MoveText.Tail
                    else gm.MoveText.[..i - 1] @ [ mte ] @ gm.MoveText.[i + 1..]
                { gm with MoveText = nmtel }
        else 
            let rec getnmtel (cirs : int list) 
                    (mtel : EncodedMoveTextEntry list) =
                if cirs.Length = 1 then 
                    let i = cirs.Head
                    
                    let nmtel =
                        if i = 0 then mte :: mtel.Tail
                        elif i = mtel.Length - 1 then mtel.[..i - 1] @ [ mte ]
                        else mtel.[..i - 1] @ [ mte ] @ mtel.[i + 1..]
                    nmtel
                else 
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    | EncodedRAVEntry(nmtel) -> 
                        mtel.[..i - 1] 
                        @ [ EncodedRAVEntry(getnmtel cirs.Tail nmtel) ] 
                          @ mtel.[i + 1..]
                    | _ -> failwith "should be RAV"
            
            let nmtel = getnmtel irs gm.MoveText
            { gm with MoveText = nmtel }
    
    let DeleteComment (gm : EncodedGame) (irs : int list) =
        if irs.Length = 1 then 
            let i = irs.Head
            
            let nmtel =
                if i = 0 then gm.MoveText.Tail
                else 
                    //fix Black by removing mn and cont
                    let pmte =
                        let hm = gm.MoveText.[i + 1]
                        match hm with
                        | EncodedHalfMoveEntry(_, _, emv) -> 
                            let isw = emv.Isw
                            if isw then hm
                            else EncodedHalfMoveEntry(None, false, emv)
                        | _ -> hm
                    gm.MoveText.[..i - 1] @ [ pmte ] @ gm.MoveText.[i + 2..]
            { gm with MoveText = nmtel }
        else 
            let rec getnmtel (cirs : int list) 
                    (mtel : EncodedMoveTextEntry list) =
                if cirs.Length = 1 then 
                    let i = cirs.Head
                    
                    let nmtel =
                        if i = 0 then mtel.Tail
                        elif i = mtel.Length - 1 then mtel.[..i - 1]
                        else 
                            //fix Black by removing mn and cont
                            let pmte =
                                let hm = mtel.[i + 1]
                                match hm with
                                | EncodedHalfMoveEntry(_, _, emv) -> 
                                    let isw = emv.Isw
                                    if isw then hm
                                    else EncodedHalfMoveEntry(None, false, emv)
                                | _ -> hm
                            mtel.[..i - 1] @ [ pmte ] @ mtel.[i + 2..]
                    nmtel
                else 
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    | EncodedRAVEntry(nmtel) -> 
                        mtel.[..i - 1] 
                        @ [ EncodedRAVEntry(getnmtel cirs.Tail nmtel) ] 
                          @ mtel.[i + 1..]
                    | _ -> failwith "should be RAV"
            
            let nmtel = getnmtel irs gm.MoveText
            { gm with MoveText = nmtel }
    
    let RemoveComments(gm : EncodedGame) =
        let rec setemv (emvl : EncodedMoveTextEntry list) oemvl =
            if emvl |> List.isEmpty then oemvl |> List.rev
            else 
                let mte = emvl.Head
                match mte with
                | EncodedRAVEntry(mtel) -> 
                    let nmtel = setemv mtel []
                    let nmte = EncodedRAVEntry(nmtel)
                    setemv emvl.Tail (nmte :: oemvl)
                | EncodedCommentEntry(c) -> setemv emvl.Tail oemvl
                | _ -> setemv emvl.Tail (mte :: oemvl)
        
        let nmtel = setemv gm.MoveText []
        let ngm = { gm with MoveText = nmtel }
        ngm |> fixnums
    
    let AddNag (gm : EncodedGame) (irs : int list) (ng : NAG) =
        let mte = EncodedNAGEntry(ng)
        if irs.Length = 1 then 
            //allow for empty list
            if gm.MoveText.IsEmpty then { gm with MoveText = [ mte ] }
            else 
                let i = irs.Head
                
                let nmtel =
                    if i = gm.MoveText.Length - 1 then gm.MoveText @ [ mte ]
                    else gm.MoveText.[..i] @ [ mte ] @ gm.MoveText.[i + 1..]
                { gm with MoveText = nmtel }
        else 
            let rec getnmtel (cirs : int list) 
                    (mtel : EncodedMoveTextEntry list) =
                if cirs.Length = 1 then 
                    let i = cirs.Head
                    
                    let nmtel =
                        if i = mtel.Length - 1 then mtel @ [ mte ]
                        else mtel.[..i] @ [ mte ] @ mtel.[i + 1..]
                    nmtel
                else 
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    | EncodedRAVEntry(nmtel) -> 
                        mtel.[..i - 1] 
                        @ [ EncodedRAVEntry(getnmtel cirs.Tail nmtel) ] 
                          @ mtel.[i + 1..]
                    | _ -> failwith "should be RAV"
            
            let nmtel = getnmtel irs gm.MoveText
            { gm with MoveText = nmtel }
    
    let EditNag (gm : EncodedGame) (irs : int list) (ng : NAG) =
        let mte = EncodedNAGEntry(ng)
        if irs.Length = 1 then 
            //allow for empty list
            if gm.MoveText.IsEmpty then { gm with MoveText = [ mte ] }
            else 
                let i = irs.Head
                
                let nmtel =
                    if i = 0 then mte :: gm.MoveText.Tail
                    else gm.MoveText.[..i - 1] @ [ mte ] @ gm.MoveText.[i + 1..]
                { gm with MoveText = nmtel }
        else 
            let rec getnmtel (cirs : int list) 
                    (mtel : EncodedMoveTextEntry list) =
                if cirs.Length = 1 then 
                    let i = cirs.Head
                    
                    let nmtel =
                        if i = 0 then mte :: mtel.Tail
                        elif i = mtel.Length - 1 then mtel.[..i - 1] @ [ mte ]
                        else mtel.[..i - 1] @ [ mte ] @ mtel.[i + 1..]
                    nmtel
                else 
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    | EncodedRAVEntry(nmtel) -> 
                        mtel.[..i - 1] 
                        @ [ EncodedRAVEntry(getnmtel cirs.Tail nmtel) ] 
                          @ mtel.[i + 1..]
                    | _ -> failwith "should be RAV"
            
            let nmtel = getnmtel irs gm.MoveText
            { gm with MoveText = nmtel }
    
    let DeleteNag (gm : EncodedGame) (irs : int list) =
        if irs.Length = 1 then 
            let i = irs.Head
            
            let nmtel =
                if i = 0 then gm.MoveText.Tail
                else gm.MoveText.[..i - 1] @ gm.MoveText.[i + 1..]
            { gm with MoveText = nmtel }
        else 
            let rec getnmtel (cirs : int list) 
                    (mtel : EncodedMoveTextEntry list) =
                if cirs.Length = 1 then 
                    let i = cirs.Head
                    
                    let nmtel =
                        if i = 0 then mtel.Tail
                        elif i = mtel.Length - 1 then mtel.[..i - 1]
                        else mtel.[..i - 1] @ mtel.[i + 1..]
                    nmtel
                else 
                    let i = cirs.Head
                    let rav = mtel.[i]
                    match rav with
                    | EncodedRAVEntry(nmtel) -> 
                        mtel.[..i - 1] 
                        @ [ EncodedRAVEntry(getnmtel cirs.Tail nmtel) ] 
                          @ mtel.[i + 1..]
                    | _ -> failwith "should be RAV"
            
            let nmtel = getnmtel irs gm.MoveText
            { gm with MoveText = nmtel }
    
    let RemoveNags(gm : EncodedGame) =
        let rec setemv (emvl : EncodedMoveTextEntry list) oemvl =
            if emvl |> List.isEmpty then oemvl |> List.rev
            else 
                let mte = emvl.Head
                match mte with
                | EncodedRAVEntry(mtel) -> 
                    let nmtel = setemv mtel []
                    let nmte = EncodedRAVEntry(nmtel)
                    setemv emvl.Tail (nmte :: oemvl)
                | EncodedNAGEntry(_) -> setemv emvl.Tail oemvl
                | _ -> setemv emvl.Tail (mte :: oemvl)
        
        let nmtel = setemv gm.MoveText []
        { gm with MoveText = nmtel }
    
    let Strip (gm : EncodedGame) (iirs : int list) =
        let rec getnmtel (cirs : int list) (mtel : EncodedMoveTextEntry list) =
            if cirs.Length = 1 then 
                let i = cirs.Head
                mtel.[..i]
            else 
                let i = cirs.Head
                let rav = mtel.[i]
                match rav with
                | EncodedRAVEntry(nmtel) -> 
                    mtel.[..i - 1] 
                    @ [ EncodedRAVEntry(getnmtel cirs.Tail nmtel) ] 
                      @ mtel.[i + 1..]
                | _ -> failwith "should be RAV"
        
        let nmtel = getnmtel iirs gm.MoveText
        { gm with MoveText = nmtel }
    
    let ToStr(game : EncodedGame) =
        let writer = new System.Text.StringBuilder()
        let ResultString = GameResult.ToStr
        
        let rec MoveTextEntry(entry : EncodedMoveTextEntry) =
            match entry with
            | EncodedHalfMoveEntry(mn, ic, emv) -> 
                if mn.IsSome then 
                    writer.Append(mn.Value) |> ignore
                    writer.Append(if ic then "... "
                                  else ". ")
                    |> ignore
                writer.Append(emv.San) |> ignore
                writer.Append(" ") |> ignore
            | EncodedCommentEntry(str) -> 
                writer.AppendLine() |> ignore
                writer.Append("{" + str + "} ") |> ignore
            | EncodedGameEndEntry(gr) -> 
                writer.Append(ResultString(gr)) |> ignore
            | EncodedNAGEntry(cd) -> 
                writer.Append("$" + (cd |> int).ToString()) |> ignore
                writer.Append(" ") |> ignore
            | EncodedRAVEntry(ml) -> 
                writer.AppendLine() |> ignore
                writer.Append("(") |> ignore
                MoveText(ml) |> ignore
                writer.AppendLine(")") |> ignore
        
        and MoveText(ml : EncodedMoveTextEntry list) =
            let doent i m = MoveTextEntry(m)
            //if i<ml.Length-1 then writer.Write(" ")
            ml |> List.iteri doent
        
        let Tag(name : string, value : string) =
            writer.Append("[") |> ignore
            writer.Append(name + " \"") |> ignore
            writer.Append(value) |> ignore
            writer.AppendLine("\"]") |> ignore
        
        Tag("Event", game.Hdr.Event)
        Tag("Site", game.Site)
        Tag("Date", game |> DateUtil.ToStr2)
        Tag("Round", game.Round)
        Tag("White", game.Hdr.White)
        Tag("Black", game.Hdr.Black)
        Tag("Result", game.Hdr.Result)
        Tag("WhiteElo", game.Hdr.W_Elo)
        Tag("BlackElo", game.Hdr.B_Elo)
        Tag("ECO", game.Hdr.ECO)
        for info in game.AdditionalInfo do
            Tag(info.Key, info.Value)
        writer.AppendLine() |> ignore
        MoveText(game.MoveText)
        writer.AppendLine() |> ignore
        writer.ToString()
    
    let Compress(gm : EncodedGame) =
        let rec setemv (emvl : EncodedMoveTextEntry list) ocmvl =
            if emvl |> List.isEmpty then ocmvl |> List.rev
            else 
                let mte = emvl.Head
                match mte with
                | EncodedHalfMoveEntry(mn, ic, mv) -> 
                    let cmv = mv |> MoveEncoded.Compress
                    let nmte = CompressedHalfMoveEntry(mn, ic, cmv)
                    setemv emvl.Tail (nmte :: ocmvl)
                | EncodedRAVEntry(mtel) -> 
                    let nmtel = setemv mtel []
                    let nmte = CompressedRAVEntry(nmtel)
                    setemv emvl.Tail (nmte :: ocmvl)
                | EncodedCommentEntry(c) -> 
                    setemv emvl.Tail (CompressedCommentEntry(c) :: ocmvl)
                | EncodedGameEndEntry(r) -> 
                    setemv emvl.Tail (CompressedGameEndEntry(r) :: ocmvl)
                | EncodedNAGEntry(n) -> 
                    setemv emvl.Tail (CompressedNAGEntry(n) :: ocmvl)
        
        let nmt =
            try 
                setemv gm.MoveText []
            with ex -> 
                let msg = ex.Message
                []
        
        let cgm0 = CompressedGameEMP
        
        let cgm1 =
            { cgm0 with Month = gm.Month
                        Day = gm.Day
                        Round = gm.Round
                        Site = gm.Site
                        BoardSetup = gm.BoardSetup
                        AdditionalInfo = gm.AdditionalInfo
                        MoveText = nmt }
        cgm1
    
    let Expand(gm : CompressedGame, hdr : Header) =
        let rec setemv pbd cbd (pmvl : CompressedMoveTextEntry list) oemvl =
            if pmvl |> List.isEmpty then oemvl |> List.rev
            else 
                let mte = pmvl.Head
                match mte with
                | CompressedHalfMoveEntry(mn, ic, mv) -> 
                    let emv = mv |> MoveEncoded.Expand cbd
                    let nmte = EncodedHalfMoveEntry(mn, ic, emv)
                    setemv cbd emv.PostBrd pmvl.Tail (nmte :: oemvl)
                | CompressedRAVEntry(mtel) -> 
                    let nmtel = setemv pbd pbd mtel []
                    let nmte = EncodedRAVEntry(nmtel)
                    setemv pbd cbd pmvl.Tail (nmte :: oemvl)
                | CompressedCommentEntry(c) -> 
                    setemv pbd cbd pmvl.Tail (EncodedCommentEntry(c) :: oemvl)
                | CompressedGameEndEntry(r) -> 
                    setemv pbd cbd pmvl.Tail (EncodedGameEndEntry(r) :: oemvl)
                | CompressedNAGEntry(n) -> 
                    setemv pbd cbd pmvl.Tail (EncodedNAGEntry(n) :: oemvl)
        
        let ibd =
            if gm.BoardSetup.IsSome then gm.BoardSetup.Value
            else Board.Start
        
        let nmt =
            try 
                setemv ibd ibd gm.MoveText []
            with ex -> 
                let msg = ex.Message
                []
        
        let egm0 = EncodedGameEMP
        
        let egm1 =
            { egm0 with Hdr = hdr
                        Month = gm.Month
                        Day = gm.Day
                        Round = gm.Round
                        Site = gm.Site
                        BoardSetup = gm.BoardSetup
                        AdditionalInfo = gm.AdditionalInfo
                        MoveText = nmt }
        egm1
    
    let GetPosnsMoves (ply : int) (gm : EncodedGame) =
        let revply =
            if ply = -1 then 999
            else ply
        
        let rec getnext (imtel : EncodedMoveTextEntry list) (opns : string list) 
                (omvs : string list) =
            if List.isEmpty imtel || omvs.Length = revply then 
                opns.Tail
                |> List.rev
                |> List.toArray, 
                omvs
                |> List.rev
                |> List.toArray
            else 
                let mte = imtel.Head
                match mte with
                | EncodedHalfMoveEntry(_, _, mv) -> 
                    let nmvs = mv.San :: omvs
                    let npns = (mv.PostBrd |> Board.ToSimpleStr) :: opns
                    getnext imtel.Tail npns nmvs
                | _ -> getnext imtel.Tail opns omvs
        
        let ibd =
            if gm.BoardSetup.IsSome then gm.BoardSetup.Value
            else Board.Start
        
        getnext gm.MoveText [ ibd |> Board.ToSimpleStr ] []
    
    let GetPosns (ply : int) (gm : EncodedGame) =
        let revply =
            if ply = -1 then 999
            else ply
        
        let rec getnext (imtel : EncodedMoveTextEntry list) (opns : string list) =
            if List.isEmpty imtel || opns.Length = revply then 
                opns
                |> List.rev
                |> List.toArray
            else 
                let mte = imtel.Head
                match mte with
                | EncodedHalfMoveEntry(_, _, mv) -> 
                    let npns = (mv.PostBrd |> Board.ToSimpleStr) :: opns
                    getnext imtel.Tail npns
                | _ -> getnext imtel.Tail opns
        
        let ibd =
            if gm.BoardSetup.IsSome then gm.BoardSetup.Value
            else Board.Start
        
        getnext gm.MoveText [ ibd |> Board.ToSimpleStr ]
