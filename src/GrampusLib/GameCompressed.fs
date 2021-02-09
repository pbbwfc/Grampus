namespace GrampusInternal

open Grampus

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module GameCompressed =
    let fixnums (gm : CompressedGame) =
        let rec fixemv pmvo (emvl : CompressedMoveTextEntry list) oemvl =
            if emvl |> List.isEmpty then oemvl |> List.rev
            else 
                let mte = emvl.Head
                match mte with
                | CompressedRAVEntry(mtel) -> 
                    let nmtel = fixemv None mtel []
                    let nmte = CompressedRAVEntry(nmtel)
                    fixemv None emvl.Tail (nmte :: oemvl)
                | CompressedHalfMoveEntry(n, b, mv) -> 
                    if pmvo.IsSome then 
                        match pmvo.Value with
                        | CompressedCommentEntry(_) -> 
                            fixemv (Some(mte)) emvl.Tail (mte :: oemvl)
                        | _ -> 
                            let nmte =
                                if mv.Isw then mte
                                else CompressedHalfMoveEntry(None, false, mv)
                            fixemv (Some(nmte)) emvl.Tail (nmte :: oemvl)
                    else fixemv (Some(mte)) emvl.Tail (mte :: oemvl)
                | _ -> fixemv (Some(mte)) emvl.Tail (mte :: oemvl)
        
        let nmtel = fixemv None gm.MoveText []
        { gm with MoveText = nmtel }
    
    let RemoveRavs(gm : CompressedGame) =
        let rec setemv (emvl : CompressedMoveTextEntry list) oemvl =
            if emvl |> List.isEmpty then oemvl |> List.rev
            else 
                let mte = emvl.Head
                match mte with
                | CompressedRAVEntry(mtel) -> setemv emvl.Tail oemvl
                | _ -> setemv emvl.Tail (mte :: oemvl)
        
        let nmtel = setemv gm.MoveText []
        let ngm = { gm with MoveText = nmtel }
        ngm |> fixnums
    
    let RemoveComments(gm : CompressedGame) =
        let rec setemv (emvl : CompressedMoveTextEntry list) oemvl =
            if emvl |> List.isEmpty then oemvl |> List.rev
            else 
                let mte = emvl.Head
                match mte with
                | CompressedRAVEntry(mtel) -> 
                    let nmtel = setemv mtel []
                    let nmte = CompressedRAVEntry(nmtel)
                    setemv emvl.Tail (nmte :: oemvl)
                | CompressedCommentEntry(c) -> setemv emvl.Tail oemvl
                | _ -> setemv emvl.Tail (mte :: oemvl)
        
        let nmtel = setemv gm.MoveText []
        let ngm = { gm with MoveText = nmtel }
        ngm |> fixnums
    
    let RemoveNags(gm : CompressedGame) =
        let rec setemv (emvl : CompressedMoveTextEntry list) oemvl =
            if emvl |> List.isEmpty then oemvl |> List.rev
            else 
                let mte = emvl.Head
                match mte with
                | CompressedRAVEntry(mtel) -> 
                    let nmtel = setemv mtel []
                    let nmte = CompressedRAVEntry(nmtel)
                    setemv emvl.Tail (nmte :: oemvl)
                | CompressedNAGEntry(_) -> setemv emvl.Tail oemvl
                | _ -> setemv emvl.Tail (mte :: oemvl)
        
        let nmtel = setemv gm.MoveText []
        { gm with MoveText = nmtel }
