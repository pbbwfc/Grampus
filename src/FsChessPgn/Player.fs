namespace FsChessPgn

open FsChess

module Player = 
    let AllPlayers = [| Player.White; Player.Black |]
    let PlayerOther(player : Player) = (int (player) ^^^ 1) |> Plyr
    
    let MyRanks = 
        [| [| Rank1; Rank2; Rank3; Rank4; Rank5; Rank6; Rank7; Rank8 |]
           [| Rank8; Rank7; Rank6; Rank5; Rank4; Rank3; Rank2; Rank1 |] |]
    
    let MyRank (rank : Rank) (player : Player) = MyRanks.[int (player)].[int (rank)]
