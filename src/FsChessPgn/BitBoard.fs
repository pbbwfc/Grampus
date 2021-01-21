namespace FsChessPgn

open FsChess

module Bitboard =

    let DebrujinPositions = 
        [| 0s; 1s; 28s; 2s; 29s; 14s; 24s; 3s; 30s; 22s; 20s; 15s; 25s; 17s; 4s; 8s; 31s; 27s; 13s; 23s; 21s; 19s; 16s; 7s; 26s; 12s; 18s; 6s; 11s; 5s; 
           10s; 9s |]
    
    let DebrujinLSB num = 
        let ind1 = uint32 (num &&& -num) * 0x077CB531u
        let ind2 = ind1 >>> 27
        DebrujinPositions.[int (ind2)]
    
    let private byteBitcount = 
        [| 0; 1; 1; 2; 1; 2; 2; 3; 1; 2; 2; 3; 2; 3; 3; 4; 1; 2; 2; 3; 2; 3; 3; 4; 2; 3; 3; 4; 3; 4; 4; 5; 1; 2; 2; 
            3; 2; 3; 3; 4; 2; 3; 3; 4; 3; 4; 4; 5; 2; 3; 3; 4; 3; 4; 4; 5; 3; 4; 4; 5; 4; 5; 5; 6; 1; 2; 2; 3; 2; 3; 
            3; 4; 2; 3; 3; 4; 3; 4; 4; 5; 2; 3; 3; 4; 3; 4; 4; 5; 3; 4; 4; 5; 4; 5; 5; 6; 2; 3; 3; 4; 3; 4; 4; 5; 3; 
            4; 4; 5; 4; 5; 5; 6; 3; 4; 4; 5; 4; 5; 5; 6; 4; 5; 5; 6; 5; 6; 6; 7; 1; 2; 2; 3; 2; 3; 3; 4; 2; 3; 3; 4; 
            3; 4; 4; 5; 2; 3; 3; 4; 3; 4; 4; 5; 3; 4; 4; 5; 4; 5; 5; 6; 2; 3; 3; 4; 3; 4; 4; 5; 3; 4; 4; 5; 4; 5; 5; 
            6; 3; 4; 4; 5; 4; 5; 5; 6; 4; 5; 5; 6; 5; 6; 6; 7; 2; 3; 3; 4; 3; 4; 4; 5; 3; 4; 4; 5; 4; 5; 5; 6; 3; 4; 
            4; 5; 4; 5; 5; 6; 4; 5; 5; 6; 5; 6; 6; 7; 3; 4; 4; 5; 4; 5; 5; 6; 4; 5; 5; 6; 5; 6; 6; 7; 4; 5; 5; 6; 5; 
            6; 6; 7; 5; 6; 6; 7; 6; 7; 7; 8 |]

    let BitCount(ibb : Bitboard) = 
        let rec getv vl rv = 
            if vl = 0UL then rv
            else 
                let nrv = rv + byteBitcount.[int (vl &&& 255UL)]
                let nvl = vl >>> 8
                getv nvl nrv
        getv (uint64 (ibb)) 0
    
    let NorthMostPosition(ibb : Bitboard):Square = 
        if (uint64 (ibb) &&& 0xFFFFFFFFUL) <> 0UL then 
            let x = uint64 (ibb) &&& 0xFFFFFFFFUL
            DebrujinLSB(int (x)) 
        else 
            let x = uint64 (ibb) >>> 32
            (DebrujinLSB(int (x)) + 32s)
    
    let SouthMostPosition(ibb : Bitboard) = 
        let x = uint64 (ibb)
        let x = x ||| (x >>> 1)
        let x = x ||| (x >>> 2)
        let x = x ||| (x >>> 4)
        let x = x ||| (x >>> 8)
        let x = x ||| (x >>> 16)
        let x = x ||| (x >>> 32)
        NorthMostPosition((x &&& ~~~(x >>> 1)) |> BitB)
    
    let Reverse(ibb : Bitboard) = 
        ((uint64 (ibb &&& Bitboard.Rank1) <<< 56) ||| (uint64 (ibb &&& Bitboard.Rank2) <<< 40) 
         ||| (uint64 (ibb &&& Bitboard.Rank3) <<< 24) ||| (uint64 (ibb &&& Bitboard.Rank4) <<< 8) 
         ||| (uint64 (ibb &&& Bitboard.Rank5) >>> 8) ||| (uint64 (ibb &&& Bitboard.Rank6) >>> 24) 
         ||| (uint64 (ibb &&& Bitboard.Rank7) >>> 40) ||| (uint64 (ibb &&& Bitboard.Rank8) >>> 56)) |> BitB
    let ShiftDirN(ibb : Bitboard) = (uint64 (ibb &&& ~~~Bitboard.Rank8) <<< 8) |> BitB
    let ShiftDirE(ibb : Bitboard) = (uint64 (ibb &&& ~~~Bitboard.FileH) >>> 1) |> BitB
    let ShiftDirS(ibb : Bitboard) = (uint64 (ibb &&& ~~~Bitboard.Rank1) >>> 8) |> BitB
    let ShiftDirW(ibb : Bitboard) = (uint64 (ibb &&& ~~~Bitboard.FileA) <<< 1) |> BitB
    let ShiftDirNE(ibb : Bitboard) = (uint64 (ibb &&& ~~~Bitboard.Rank8 &&& ~~~Bitboard.FileH) <<< 9) |> BitB
    let ShiftDirSE(ibb : Bitboard) = (uint64 (ibb &&& ~~~Bitboard.Rank1 &&& ~~~Bitboard.FileH) >>> 7) |> BitB
    let ShiftDirSW(ibb : Bitboard) = (uint64 (ibb &&& ~~~Bitboard.Rank1 &&& ~~~Bitboard.FileA) >>> 9) |> BitB
    let ShiftDirNW(ibb : Bitboard) = (uint64 (ibb &&& ~~~Bitboard.Rank8 &&& ~~~Bitboard.FileA) <<< 7) |> BitB
    
    let Shift dir (ibb : Bitboard) = 
        match dir with
        | Dirn.DirN -> ibb |> ShiftDirN
        | Dirn.DirE -> ibb |> ShiftDirE
        | Dirn.DirS -> ibb |> ShiftDirS
        | Dirn.DirW -> ibb |> ShiftDirW
        | Dirn.DirNE -> ibb |> ShiftDirNE
        | Dirn.DirSE -> ibb |> ShiftDirSE
        | Dirn.DirSW -> ibb |> ShiftDirSW
        | Dirn.DirNW -> ibb |> ShiftDirNW
        | Dirn.DirNNE -> 
            ibb
            |> ShiftDirNE
            |> ShiftDirN
        | Dirn.DirEEN -> 
            ibb
            |> ShiftDirNE
            |> ShiftDirE
        | Dirn.DirEES -> 
            ibb
            |> ShiftDirSE
            |> ShiftDirE
        | Dirn.DirSSE -> 
            ibb
            |> ShiftDirSE
            |> ShiftDirS
        | Dirn.DirSSW -> 
            ibb
            |> ShiftDirSW
            |> ShiftDirS
        | Dirn.DirWWS -> 
            ibb
            |> ShiftDirSW
            |> ShiftDirW
        | Dirn.DirWWN -> 
            ibb
            |> ShiftDirNW
            |> ShiftDirW
        | Dirn.DirNNW -> 
            ibb
            |> ShiftDirNW
            |> ShiftDirN
        | _ -> failwith "invalid dir"
    
    let Flood dir (ibb : Bitboard) = 
        let rec getb (bb : Bitboard) = 
            let shift = bb |> Shift(dir)
            if (shift &&& bb) = shift then bb
            else getb (shift ||| bb)
        getb ibb
    
    let ContainsPos (pos : Square) ibb = (ibb &&& (pos |> Square.ToBitboard)) <> Bitboard.Empty
    let Contains (other : Bitboard) ibb = (ibb &&& other) <> Bitboard.Empty
    let IsEmpty ibb = ibb = Bitboard.Empty
    
    let ToSquares(ibb : Bitboard) = 
        let rec getp (bb : Bitboard) ol = 
            if bb = Bitboard.Empty then ol |> List.rev
            else 
                let first = bb |> NorthMostPosition
                let nbb = bb &&& ~~~(first |> Square.ToBitboard)
                getp nbb (first :: ol)
        getp ibb []
    
    let GetFirstPos ibb :Square =
        let num = uint64 (ibb) &&& 0xFFFFFFFFUL
        if num <> 0UL then 
            let number = int (num)
            DebrujinPositions.[int ((uint32 (number &&& -number) * 0x077CB531u) >>> 27)] 
        else 
            let number = int (uint64 (ibb) >>> 32)
            (DebrujinPositions.[int ((uint32 (number &&& -number) * 0x077CB531u) >>> 27)] + 32s) 

    let GetRemainPos first ibb = ibb &&& ~~~(first |> Square.ToBitboard)
    
    let PopFirst ibb = 
        let first = GetFirstPos ibb
        let bb = GetRemainPos first ibb
        first, bb

    


