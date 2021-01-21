namespace FsChessPgn

open FsChess

module NagUtil = 
    
    let All = [0..6]@[10]@[14..19]|>List.map Ng

    let ToStr(nag:NAG) =
        match nag with
        | NAG.Good -> "!"
        | NAG.Poor -> "?"
        | NAG.VeryGood -> "!!"
        | NAG.VeryPoor -> "??"
        | NAG.Speculative -> "!?"
        | NAG.Questionable -> "?!"
        | NAG.Even -> "="
        | NAG.Wslight -> "⩲"
        | NAG.Bslight -> "⩱"
        | NAG.Wmoderate -> "±"
        | NAG.Bmoderate -> "∓"
        | NAG.Wdecisive -> "+−"
        | NAG.Bdecisive -> "−+"
        |_ -> ""

    let FromStr(str:string) =
        let stra = All|>List.map ToStr|>List.toArray
        let indx = stra|>Array.findIndex(fun s -> s=str)
        All.[indx]

    let ToHtm(nag:NAG) =
        match nag with
        | NAG.Good -> "&#33;"
        | NAG.Poor -> "&#63;"
        | NAG.VeryGood -> "&#33;&#33;"
        | NAG.VeryPoor -> "&#63;&#63;"
        | NAG.Speculative -> "&#33;&#63;"
        | NAG.Questionable -> "&#63;&#33;"
        | NAG.Even -> "&#61;"
        | NAG.Wslight -> "&#10866;"
        | NAG.Bslight -> "&#10865;"
        | NAG.Wmoderate -> "&#0177;"
        | NAG.Bmoderate -> "&#8723;"
        | NAG.Wdecisive -> "&#43;&minus;"
        | NAG.Bdecisive -> "&minus;&#43;"
        |_ -> ""

    let Desc(nag:NAG) =
        match nag with
        | NAG.Good -> "Good"
        | NAG.Poor -> "Poor"
        | NAG.VeryGood -> "Very Good"
        | NAG.VeryPoor -> "Very Poor"
        | NAG.Speculative -> "Speculative"
        | NAG.Questionable -> "Questionable"
        | NAG.Even -> "Even"
        | NAG.Wslight -> "W slight adv" 
        | NAG.Bslight -> "B slight adv"
        | NAG.Wmoderate -> "W mod adv"
        | NAG.Bmoderate -> "B mod adv"
        | NAG.Wdecisive -> "W dec adv"
        | NAG.Bdecisive -> "B dec adv"
        |_ -> "None"
