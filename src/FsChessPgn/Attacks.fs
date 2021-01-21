namespace FsChessPgn

open FsChess

module Attacks =

    let Combinations(mask : Bitboard) = 
        let posl = mask |> Bitboard.ToSquares
        let possCombs = 1 <<< (posl.Length)
    
        let getc i = 
            posl
            |> List.mapi (fun b p -> 
                   if i &&& (1 <<< b) = 0 then Bitboard.Empty
                   else p |> Square.ToBitboard)
            |> List.reduce (|||)
        [ 0..possCombs - 1 ] |> List.map getc

    let RookMaskCalc(position : Square) = 
        let rec getr (d:Dirn) (p : Square) rv = 
            if p
               |> Square.PositionInDirection(d)
               = OUTOFBOUNDS then rv
            else 
                let nrv = rv ||| (p |> Square.ToBitboard)
                getr d (p |> Square.PositionInDirection(d)) nrv
        Direction.AllDirectionsRook
        |> Array.map (fun d -> getr d (position |> Square.PositionInDirection(d)) Bitboard.Empty)
        |> Array.reduce (|||)

    let BishopMaskCalc(position : Square) = 
        let rec getr d (p : Square) rv = 
            if p
               |> Square.PositionInDirection(d)
               = OUTOFBOUNDS then rv
            else 
                let nrv = rv ||| (p |> Square.ToBitboard)
                getr d (p |> Square.PositionInDirection(d)) nrv
        Direction.AllDirectionsBishop
        |> Array.map (fun d -> getr d (position |> Square.PositionInDirection(d)) Bitboard.Empty)
        |> Array.reduce (|||)

    let RookAttacksCalc (position : Square) (blockers : Bitboard) = 
        let rec getr d p rv = 
            if p = OUTOFBOUNDS then rv
            else 
                let nrv = rv ||| (p |> Square.ToBitboard)
                if blockers |> Bitboard.ContainsPos(p) then nrv
                else getr d (p |> Square.PositionInDirection(d)) nrv
        Direction.AllDirectionsRook
        |> Array.map (fun d -> getr d (position |> Square.PositionInDirection(d)) Bitboard.Empty)
        |> Array.reduce (|||)

    let BishopAttacksCalc (position : Square) (blockers : Bitboard) = 
        let rec getr d p rv = 
            if p = OUTOFBOUNDS then rv
            else 
                let nrv = rv ||| (p |> Square.ToBitboard)
                if blockers |> Bitboard.ContainsPos(p) then nrv
                else getr d (p |> Square.PositionInDirection(d)) nrv
        Direction.AllDirectionsBishop
        |> Array.map (fun d -> getr d (position |> Square.PositionInDirection(d)) Bitboard.Empty)
        |> Array.reduce (|||)

    let MagicsB = 
        [| 11533720853379293696UL //A8
                                  ; 4634221887939944448UL //B8
                                                          ; 2469107311143059968UL //C8
                                                                                  ; 4631963212403773698UL //D8
                                                                                                          ; 
           4612816453819826288UL //E8
                                 ; 286010531514368UL //F8
                                                     ; 9522318258233933892UL //G8
                                                                             ; 44564589584384UL //H8
                                                                                                ; 4611844485559689760UL //A7
                                                                                                                        ; 
           9234639900802844704UL //B7
                                 ; 1152939099510816896UL //C7
                                                         ; 4612253436254175747UL //D7
                                                                                 ; 163611745976713216UL //E7
                                                                                                        ; 9079773542227200UL //F7
                                                                                                                             ; 
           579279902869555712UL //G7
                                ; 276019808288UL //H7
                                                 ; 2533292243167252UL //A6
                                                                      ; 578818242689180172UL //B6
                                                                                             ; 4521196125167876UL //C6
                                                                                                                  ; 
           4613940019445383360UL //D6
                                 ; 9288709128585248UL //E6
                                                      ; 9223408325043949572UL //F6
                                                                              ; 288511859852706384UL //G6
                                                                                                     ; 2306406097981834240UL //H6
                                                                                                                             ; 
           9242019754598990849UL //A5
                                 ; 2319397788830531718UL //B5
                                                         ; 2306970008900665377UL //C5
                                                                                 ; 6917599675625308672UL //D5
                                                                                                         ; 281543712981000UL //E5
                                                                                                                             ; 
           9799974643371622914UL //F5
                                 ; 864977001780478404UL //G5
                                                        ; 576779610692288648UL //H5
                                                                               ; 5766863789614178434UL //A4
                                                                                                       ; 
           2342507332688152577UL //B4
                                 ; 4629772993306838017UL //C4
                                                         ; 11261200239689856UL //D4
                                                                               ; 2341946590220009728UL //E4
                                                                                                       ; 
           2378041967813656835UL //F4
                                 ; 9259966266318524544UL //G4
                                                         ; 2315133883006616576UL //H4
                                                                                 ; 11260116309381120UL //A3
                                                                                                       ; 81137431994175636UL //B3
                                                                                                                             ; 
           9223935056737747972UL //C3
                                 ; 36169809657856512UL //D3
                                                       ; 36037597411148800UL //E3
                                                                             ; 9223671246053441600UL //F3
                                                                                                     ; 4556930643132672UL //G3
                                                                                                                          ; 
           288516266363846720UL //H3
                                ; 36877689021792364UL //A2
                                                      ; 1848165255562330128UL //B2
                                                                              ; 75333735286019UL //C2
                                                                                                 ; 182431004186124418UL //D2
                                                                                                                        ; 
           324681420266274946UL //E2
                                ; 583849532784640UL //F2
                                                    ; 4538788311334912UL //G2
                                                                         ; 2260630406449152UL //H2
                                                                                              ; 18085883964031488UL //A1
                                                                                                                    ; 
           709176350153515040UL //B1
                                ; 72061992638613568UL //C1
                                                      ; 1152921513742304320UL //D1
                                                                              ; 4611686091979247680UL //E1
                                                                                                      ; 
           9259409771768840320UL //F1
                                 ; 3396400276832386UL //G1
                                                      ; 866978129825105952UL //H1
                                                                             |]
    let MagicsR = 
        [| 5800636870882762768UL //A8
                                 ; 90072129990692872UL //B8
                                                       ; 9871903714778873872UL //C8
                                                                               ; 9259409630235230214UL //D8
                                                                                                       ; 36037593179127810UL //E8
                                                                                                                             ; 
           72066394426179586UL //F8
                               ; 1189091588903534848UL //G8
                                                       ; 1765411603689251072UL // rook H8
                                                                               ; 4644337656791176UL //A7
                                                                                                    ; 8070591407179432064UL //B7
                                                                                                                            ; 
           612630355532316800UL //C7
                                ; 140806208356480UL //D7
                                                    ; 2306124518566924292UL //E7
                                                                            ; 2328923995992228880UL //F7
                                                                                                    ; 868209638683836544UL //G7
                                                                                                                           ; 
           4616893306587056256UL //H7
                                 ; 36033469945481285UL //A6
                                                       ; 1896015718537773056UL //B6
                                                                               ; 282575564177424UL //C6
                                                                                                   ; 1441434455784816648UL //D6
                                                                                                                           ; 
           9148486633128960UL //E6
                              ; 9261935208310243330UL //F6
                                                      ; 3518986981474816UL //G6
                                                                           ; 9223444604689334401UL //H6
                                                                                                   ; 10682538455709270144UL //A5
                                                                                                                            ; 
           5084599199107907649UL //B5
                                 ; 180179171615441024UL //C5
                                                        ; 2305851947041685764UL //D5
                                                                                ; 8798240768128UL //E5
                                                                                                  ; 19141399001825344UL //F5
                                                                                                                        ; 
           54047597886702080UL //G5
                               ; 2450310324478312708UL //H5
                                                       ; 36169809393614880UL //A4
                                                                             ; 1170971156212105216UL //B4
                                                                                                     ; 17594341924864UL //C4
                                                                                                                        ; 
           9655735195424262144UL //D4
                                 ; 36310306372194308UL //E4
                                                       ; 2201179128832UL //F4
                                                                         ; 140746086678784UL //G4
                                                                                             ; 576812888115642497UL //H4
                                                                                                                    ; 
           900790296365793316UL //A3
                                ; 1026891221492056064UL //B3
                                                        ; 1179951898732544064UL //C3
                                                                                ; 324276765490970752UL //D3
                                                                                                       ; 11259016256716809UL //E3
                                                                                                                             ; 
           36591764160675842UL //F3
                               ; 288793343419351041UL //G3
                                                      ; 2291384920965121UL //H3
                                                                           ; 324259310613823552UL //A2
                                                                                                  ; 35253095759936UL //B2
                                                                                                                     ; 
           4665870020165763200UL //C2
                                 ; 8796361490560UL //D2
                                                   ; 2323861805903937664UL //E2
                                                                           ; 578714753288110208UL //F2
                                                                                                  ; 4756082698693378304UL //G2
                                                                                                                          ; 
           10376329827499377152UL //H2
                                  ; 1193489635382149377UL //A1
                                                          ; 9223442409902317585UL //B1
                                                                                  ; 4649984246102884417UL //C1
                                                                                                          ; 
           2814888280327186UL //D1
                              ; 1688918716056578UL //E1
                                                   ; 1155455063334848553UL //F1
                                                                           ; 9331608030229332484UL //G1
                                                                                                   ; 45071337550774534UL //H1
                                                                                                                         |]

    let AttacksKn = 
        let getkn sq = 
            Direction.AllDirectionsKnight
            |> Seq.map (fun d -> 
                   sq
                   |> Square.PositionInDirection(d)
                   |> Square.ToBitboard)
            |> Seq.reduce (|||)
        SQUARES |> List.map getkn

    let AttacksK = 
        let getk sq = 
            Direction.AllDirectionsQueen
            |> Seq.map (fun d -> 
                   sq
                   |> Square.PositionInDirection(d)
                   |> Square.ToBitboard)
            |> Seq.reduce (|||)
        SQUARES |> List.map getk

    let AttacksP, AttacksPF = 
        let getp pr sq = 
            let board = 
                Bitboard.Empty 
                ||| (((sq |> Square.PositionInDirection(pr |> Direction.MyNorth)) 
                      |> Square.PositionInDirection(Dirn.DirE)) |> Square.ToBitboard) 
                ||| (((sq |> Square.PositionInDirection(pr |> Direction.MyNorth)) 
                      |> Square.PositionInDirection(Dirn.DirW)) |> Square.ToBitboard)
            board, (board |> Bitboard.Flood(pr |> Direction.MyNorth))
    
        let getps pr = 
            SQUARES
            |> List.map (getp pr)
            |> List.unzip
    
        Player.AllPlayers
        |> Array.map getps
        |> Array.unzip

    let LookupB, ShiftB, MaskB = 
        let getlsm sq = 
            let mask = BishopMaskCalc(sq)
            let possibleCombinations = 1 <<< (mask |> Bitboard.BitCount)
            let shift = 64 - (mask |> Bitboard.BitCount)
            let magic = MagicsB.[int (sq)]
            let lu = Array.create possibleCombinations Bitboard.Empty
            for bitset in Combinations(mask) do
                let attacks = BishopAttacksCalc sq bitset
                let index = (magic * uint64 (bitset)) >>> shift
                lu.[int (index)] <- attacks //OK
            lu, shift, mask
        SQUARES
        |> List.map getlsm
        |> List.unzip3

    let LookupR, ShiftR, MaskR = 
        let getlsm sq = 
            let mask = RookMaskCalc(sq)
            let possibleCombinations = 1 <<< (mask |> Bitboard.BitCount)
            let shift = 64 - (mask |> Bitboard.BitCount)
            let magic = MagicsR.[int (sq)]
            let lu = Array.create possibleCombinations Bitboard.Empty
            for bitset in Combinations(mask) do
                let attacks = RookAttacksCalc sq bitset
                let index = (magic * uint64 (bitset)) >>> shift
                lu.[int (index)] <- attacks //OK
            lu, shift, mask
        SQUARES
        |> List.map getlsm
        |> List.unzip3

    let KnightAttacks(from : Square) = AttacksKn.[int (from)]
    let KingAttacks(from : Square) = AttacksK.[int (from)]
    let PawnAttacks (from : Square) (player : Player) = AttacksP.[int (player)].[int (from)]
    let PawnAttacksFlood (from : Square) (player : Player) = AttacksPF.[int (player)].[int (from)]

    let BishopAttacks (pos : Square) (allPieces : Bitboard) = 
        let ind1 = uint64 (allPieces &&& MaskB.[int (pos)])
        let ind2 = ind1 * MagicsB.[int (pos)]
        let ind3 = int (ind2 >>> ShiftB.[int (pos)])
        LookupB.[int (pos)].[ind3]

    let BishopMask(pos : Square) = MaskB.[int (pos)]

    let RookAttacks (pos : Square) (allPieces : Bitboard) = 
        let ind1 = uint64 (allPieces &&& MaskR.[int (pos)])
        let ind2 = ind1 * MagicsR.[int (pos)]
        let ind3 = int (ind2 >>> ShiftR.[int (pos)])
        LookupR.[int (pos)].[ind3]

    let RookMask(pos : Square) = MaskR.[int (pos)]
    let QueenAttacks (pos : Square) (allPieces : Bitboard) = 
        (RookAttacks pos allPieces) ||| (BishopAttacks pos allPieces)


