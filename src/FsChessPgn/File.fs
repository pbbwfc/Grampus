namespace FsChessPgn

open FsChess

module File = 
    
    let Parse(c : char):File = 
        let Filedesclookup = FILE_NAMES|>List.reduce(+)
        let idx = Filedesclookup.IndexOf(c.ToString().ToLower())
        if idx < 0 then failwith (c.ToString() + " is not a valid file")
        else int16(idx)
    
    let FileToString(file : File) = FILE_NAMES.[int(file)]
    let IsInBounds(file : File) = file >= 0s && file <= 7s
