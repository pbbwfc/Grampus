namespace FsChessPgn

open System
open FsChess
open System.IO
open MessagePack
open MessagePack.Resolvers
open MessagePack.FSharp

module Index =
    
    let resolver =
        Resolvers.CompositeResolver.Create(FSharpResolver.Instance,StandardResolver.Instance)
    let options = MessagePackSerializerOptions.Standard.WithResolver(resolver)

    let Load (fol:string) =
        let fn = Path.Combine(fol,"INDEX")
        let bin = File.ReadAllBytes(fn)
        let ro = new ReadOnlyMemory<byte>(bin)
        let iea = MessagePackSerializer.Deserialize<IndexEntry[]>(ro,options)
        iea

    let Save (fol:string,iea:IndexEntry[]) =
        let fn = Path.Combine(fol,"INDEX")
        let bin = MessagePackSerializer.Serialize<IndexEntry[]>(iea,options)
        File.WriteAllBytes(fn,bin)
        