namespace GrampusInternal

open Grampus

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module GameResult =
    let Parse(s : string) =
        if s = "1-0" then GameResult.WhiteWins
        elif s = "0-1" then GameResult.BlackWins
        elif s = "1/2-1/2" then GameResult.Draw
        else GameResult.Open
    
    let ToStr(result : GameResult) =
        match result with
        | GameResult.WhiteWins -> "1-0"
        | GameResult.BlackWins -> "0-1"
        | GameResult.Draw -> "1/2-1/2"
        | _ -> "*"
    
    let ToUnicode(result : GameResult) =
        match result with
        | GameResult.WhiteWins -> "1-0"
        | GameResult.BlackWins -> "0-1"
        | GameResult.Draw -> "½-½"
        | _ -> "*"
    
    let ToInt(result : GameResult) =
        match result with
        | GameResult.WhiteWins -> 2
        | GameResult.BlackWins -> 0
        | GameResult.Draw -> 1
        | _ -> 1
