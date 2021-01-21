namespace FsChessPgn

open FsChess

module Rank = 
    
    let Parse(c : char) :Rank = 
        let Rankdesclookup = RANK_NAMES|>List.reduce(+)
        let idx = Rankdesclookup.IndexOf(c.ToString().ToLower())
        if idx < 0 then failwith (c.ToString() + " is not a valid rank")
        else int16(idx) 
    
    let RankToString(rank : Rank) = RANK_NAMES.[int(rank)]
    let IsInBounds(rank : Rank) = rank >= 0s && rank <= 7s
    
    let ToBitboard(rank : Rank) = 
        if rank=Rank1 then Bitboard.Rank1
        elif rank=Rank2 then Bitboard.Rank2
        elif rank=Rank3 then Bitboard.Rank3
        elif rank=Rank4 then Bitboard.Rank4
        elif rank=Rank5 then Bitboard.Rank5
        elif rank=Rank6 then Bitboard.Rank6
        elif rank=Rank7 then Bitboard.Rank7
        elif rank=Rank8 then Bitboard.Rank8
        else Bitboard.Empty
