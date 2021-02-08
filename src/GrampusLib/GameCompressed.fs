namespace GrampusInternal

open Grampus

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module GameCompressed =
    let RemoveRavs(gm : CompressedGame) =
        let rec setemv (emvl : CompressedMoveTextEntry list) oemvl =
            if emvl |> List.isEmpty then oemvl |> List.rev
            else 
                let mte = emvl.Head
                match mte with
                | CompressedRAVEntry(mtel) -> setemv emvl.Tail oemvl
                | _ -> setemv emvl.Tail (mte :: oemvl)
        
        let nmtel = setemv gm.MoveText []
        { gm with MoveText = nmtel }
    
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
        { gm with MoveText = nmtel }
    
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
