namespace FsChessPgn

open System
open System.IO
open FsChess
open MessagePack
open LightningDB
open MessagePack.Resolvers
open MessagePack.FSharp

module Filter =
    let resolver =
        Resolvers.CompositeResolver.Create(FSharpResolver.Instance,StandardResolver.Instance)
    let options = MessagePackSerializerOptions.Standard.WithResolver(resolver)
    
    let Create(ifol) =
        let fol = ifol + "\\filters"
        Directory.CreateDirectory(fol)|>ignore
        use env = new LightningEnvironment(fol)
        env.MaxDatabases <- 1
        env.MapSize <- 100000000L
        env.Open()
        use tx = env.BeginTransaction()
        use db = tx.OpenDatabase("Filters",new DatabaseConfiguration(Flags = LightningDB.DatabaseOpenFlags.Create))
        tx.Commit()
    
    let CreateBig(ifol) =
        let fol = ifol + "\\filters"
        Directory.CreateDirectory(fol)|>ignore
        use env = new LightningEnvironment(fol)
        env.MaxDatabases <- 1
        env.MapSize <- 4000000000L
        env.Open()
        use tx = env.BeginTransaction()
        use db = tx.OpenDatabase("Filters",new DatabaseConfiguration(Flags = LightningDB.DatabaseOpenFlags.Create))
        tx.Commit()
    
    let Save(posns:string[],filts:int list [],ifol:string) =
        let fol = ifol + "\\filters"
        use env = new LightningEnvironment(fol)
        env.MaxDatabases <- 1
        env.Open()
        use tx = env.BeginTransaction()
        use db = tx.OpenDatabase("Filters")
        for i = 0 to posns.Length-1 do
            let cd = tx.Put(db,MessagePackSerializer.Serialize<string>(posns.[i]),MessagePackSerializer.Serialize<int list>(filts.[i],options))
            if int(cd)<>0 then 
                let curr_limit = float(env.MapSize)
                let mult = float(posns.Length/i)*1.1
                let new_limit = mult * curr_limit
                tx.Abort()
                failwith (sprintf "Code: %s" (cd.ToString()))

        tx.Commit()

    let ReadArray(posns:string[],ifol:string) =
        let fol = ifol + "\\filters"
        use env = new LightningEnvironment(fol)
        env.MaxDatabases <- 1
        env.Open()
        use tx = env.BeginTransaction()
        use db = tx.OpenDatabase("Filters")
        let getv (posn:string) =
            let cd,k,v = tx.Get(db,MessagePackSerializer.Serialize<string>(posn)).ToTuple()
            if int(cd)<>0 then 
                failwith (sprintf "Code: %s" (cd.ToString()))
            else
                let ro = new ReadOnlyMemory<byte>(v.CopyToNewArray())
                MessagePackSerializer.Deserialize<int list>(ro,options)
        let vs = posns|>Array.map getv
        let cd = tx.Commit()
        if int(cd)<>0 then 
            failwith (sprintf "Final Code: %s" (cd.ToString()))
        else vs
   
    let Read(posn:string,ifol:string) =
        let fol = ifol + "\\filters"
        use env = new LightningEnvironment(fol)
        env.MaxDatabases <- 1
        env.Open()
        use tx = env.BeginTransaction()
        use db = tx.OpenDatabase("Filters")
        let cd,k,v = tx.Get(db,MessagePackSerializer.Serialize<string>(posn)).ToTuple()
        let sts =
            if int(cd)<>0 then 
                failwith (sprintf "Code: %s" (cd.ToString()))
            else
                let ro = new ReadOnlyMemory<byte>(v.CopyToNewArray())
                MessagePackSerializer.Deserialize<int list>(ro,options)
        let cd = tx.Commit()
        if int(cd)<>0 then 
            failwith (sprintf "Final Code: %s" (cd.ToString()))
        else sts
    
    let Compact(ifol:string) =
        let fol = ifol + "\\filters"
        let tmpfol = Path.Combine(fol,"temp")
        Directory.CreateDirectory(tmpfol)|>ignore
        use env = new LightningEnvironment(fol)
        env.MaxDatabases <- 1
        env.Open()
        env.CopyTo(tmpfol,true)
        env.Dispose()
        System.Threading.Thread.Sleep(10000)
        File.Delete(Path.Combine(fol,"lock.mdb"))
        File.Delete(Path.Combine(fol,"data.mdb"))
        System.Threading.Thread.Sleep(1000)
        File.Move(Path.Combine(tmpfol,"data.mdb"),Path.Combine(fol,"data.mdb"))
        System.Threading.Thread.Sleep(1000)
        