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
module Headers =
    let resolver =
        Resolvers.CompositeResolver.Create
            (FSharpResolver.Instance, StandardResolver.Instance)
    let options = MessagePackSerializerOptions.Standard.WithResolver(resolver)
    
    let Load(fol : string) =
        let fn = Path.Combine(fol, "HEADERS")
        if File.Exists(fn) then 
            let bin = File.ReadAllBytes(fn)
            let ro = new ReadOnlyMemory<byte>(bin)
            let gmr = MessagePackSerializer.Deserialize<Header []>(ro, options)
            gmr
        else [||]
    
    let Save(fol : string, hdrs : Header []) =
        let fn = Path.Combine(fol, "HEADERS")
        let bin = MessagePackSerializer.Serialize<Header []>(hdrs, options)
        File.WriteAllBytes(fn, bin)
