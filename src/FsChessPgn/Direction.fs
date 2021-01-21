namespace FsChessPgn

open FsChess

type Dirn = 
    | DirN = 8
    | DirE = 1
    | DirS = -8
    | DirW = -1
    | DirNE = 9
    | DirSE = -7
    | DirSW = -9
    | DirNW = 7
    | DirNNE = 17
    | DirEEN = 10
    | DirEES = -6
    | DirSSE = -15
    | DirSSW = -17
    | DirWWS = -10
    | DirWWN = 6
    | DirNNW = 15

module Direction =

    let AllDirectionsKnight = 
        [| Dirn.DirNNE; Dirn.DirEEN; Dirn.DirEES; Dirn.DirSSE; Dirn.DirSSW; Dirn.DirWWS; 
           Dirn.DirWWN; Dirn.DirNNW |]
    let AllDirectionsRook = [| Dirn.DirN; Dirn.DirE; Dirn.DirS; Dirn.DirW |]
    let AllDirectionsBishop = [| Dirn.DirNE; Dirn.DirSE; Dirn.DirSW; Dirn.DirNW |]
    let AllDirectionsQueen = 
        [| Dirn.DirN; Dirn.DirE; Dirn.DirS; Dirn.DirW; Dirn.DirNE; Dirn.DirSE; 
           Dirn.DirSW; Dirn.DirNW |]
    let AllDirections = 
        [| Dirn.DirN; Dirn.DirE; Dirn.DirS; Dirn.DirW; Dirn.DirNE; Dirn.DirSE; 
           Dirn.DirSW; Dirn.DirNW; Dirn.DirNNE; Dirn.DirEEN; Dirn.DirEES; Dirn.DirSSE; 
           Dirn.DirSSW; Dirn.DirWWS; Dirn.DirWWN; Dirn.DirNNW |]

    let IsDirectionRook(dir : Dirn) = 
        match dir with
        | Dirn.DirN | Dirn.DirE | Dirn.DirS | Dirn.DirW -> true
        | _ -> false

    let IsDirectionBishop(dir : Dirn) = 
        match dir with
        | Dirn.DirNW | Dirn.DirNE | Dirn.DirSW | Dirn.DirSE -> true
        | _ -> false

    let IsDirectionKnight(dir : Dirn) = 
        match dir with
        | Dirn.DirNNE | Dirn.DirEEN | Dirn.DirEES | Dirn.DirSSE 
        | Dirn.DirSSW | Dirn.DirWWS | Dirn.DirWWN | Dirn.DirNNW -> 
            true
        | _ -> false

    let Opposite(dir : Dirn) :Dirn = -int (dir)|>enum<Dirn>

    let MyNorth(player : Player) = 
        if player = Player.White then Dirn.DirN

        else Dirn.DirS