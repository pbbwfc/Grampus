namespace GrampusInternal

open System
open Grampus
open System.IO
open MessagePack
open MessagePack.Resolvers
open MessagePack.FSharp

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
module Index =
    
    let resolver =
        Resolvers.CompositeResolver.Create(FSharpResolver.Instance,StandardResolver.Instance)
    let options = MessagePackSerializerOptions.Standard.WithResolver(resolver)

    let Load (fol:string) =
        let fn = Path.Combine(fol,"INDEX")
        if File.Exists(fn) then
            let bin = File.ReadAllBytes(fn)
            let ro = new ReadOnlyMemory<byte>(bin)
            let iea = MessagePackSerializer.Deserialize<IndexEntry[]>(ro,options)
            iea
        else [||]

    let Save (fol:string,iea:IndexEntry[]) =
        let fn = Path.Combine(fol,"INDEX")
        let bin = MessagePackSerializer.Serialize<IndexEntry[]>(iea,options)
        File.WriteAllBytes(fn,bin)
        