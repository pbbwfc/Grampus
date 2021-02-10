namespace GrampusInternal

open Grampus

/// <summary>This type is for implementation purposes.</summary>
///
/// <exclude />
[<AutoOpen>]
module Util =
    let CasFlg i = enum<CstlFlgs> (i)
    let PcTp i = enum<PieceType> (i)
    let Pc i = enum<Piece> (i)
    let Plyr i = enum<Player> (i)
    let BitB i =
        Microsoft.FSharp.Core.LanguagePrimitives.EnumOfValue<uint64, Bitboard>
            (i)
    let Ng i = enum<NAG> (i)
    let (-!) (r : Rank) (i : int16) : Rank = r - i
    let (+!) (r : Rank) (i : int16) : Rank = r + i
    let (--) (f : File) (i : int16) : File = f - i
    let (++) (f : File) (i : int16) : File = f + i
    let getbinfol (gmpfile : string) =
        System.IO.Path.Combine
            (System.IO.Path.GetDirectoryName(gmpfile), 
             System.IO.Path.GetFileNameWithoutExtension(gmpfile) + "_FILES")
    
    let gettrfol (gmpfile : string) =
        let binfol = getbinfol gmpfile
        System.IO.Path.Combine(binfol, "tree")
    
    let getffol (gmpfile : string) =
        let binfol = getbinfol gmpfile
        System.IO.Path.Combine(binfol, "filters")
