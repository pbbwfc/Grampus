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
        let bin = File.ReadAllBytes(fn)
        let ro = new ReadOnlyMemory<byte>(bin)
        let gmr = MessagePackSerializer.Deserialize<GameRow[]>(ro,options)
        gmr

    let Save (fol:string,gmr:GameRow[]) =
        let fn = Path.Combine(fol,"ROWS")
        let bin = MessagePackSerializer.Serialize<GameRow[]>(gmr,options)
        File.WriteAllBytes(fn,bin)
        