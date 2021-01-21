namespace FsChessPgn

open FsChess
open System.Text.RegularExpressions

module MoveEncoded =
    
    let FromMove (bd:Brd) mno (mv:Move) =
        let pmv = MoveUtil.topMove bd mv
        {
            San = pmv.San
            Mno = mno
            Isw = bd.WhosTurn=Player.White
            Mv = mv
            PostBrd = bd|>Board.MoveApply mv
        }

    let Compress (emv:EncodedMove) =
        {
            San = emv.San
            Mno = emv.Mno
            Isw = emv.Isw
            Mv = emv.Mv
        }

    let Expand (bd:Brd) (cmv:CompressedMove) =
        {
            San = cmv.San
            Mno = cmv.Mno
            Isw = cmv.Isw
            Mv = cmv.Mv
            PostBrd = bd|>Board.MoveApply cmv.Mv
        }
        