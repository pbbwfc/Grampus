namespace FsChessPgn

open System
open FsChess
open System.IO
open MessagePack
open MessagePack.Resolvers
open MessagePack.FSharp

module GameRows =
    
    let resolver =
        Resolvers.CompositeResolver.Create(FSharpResolver.Instance,StandardResolver.Instance)
    let options = MessagePackSerializerOptions.Standard.WithResolver(resolver)

    let Load (fol:string) =
        let fn = Path.Combine(fol,"ROWS")
        if File.Exists(fn) then
            let bin = File.ReadAllBytes(fn)
            let ro = new ReadOnlyMemory<byte>(bin)
            let gmr = MessagePackSerializer.Deserialize<GameRow[]>(ro,options)
            gmr
        else [||]

    let Save (fol:string,gmr:GameRow[]) =
        let fn = Path.Combine(fol,"ROWS")
        let bin = MessagePackSerializer.Serialize<GameRow[]>(gmr,options)
        File.WriteAllBytes(fn,bin)
        
    let FromGame i (gm:CompressedGame) =
        {
            Num = i
            White = gm.WhitePlayer
            W_Elo = gm.WhiteElo
            Black = gm.BlackPlayer
            B_Elo = gm.BlackElo
            Result = gm.Result|>GameResult.ToStr
            Year = if gm.Year.IsNone then 0 else gm.Year.Value
            Event = gm.Event
            ECO = gm.ECO
            Deleted = ""
        }

    let FromGames gms =
        gms|>Array.mapi FromGame
