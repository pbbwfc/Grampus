
open System
open System.IO
open FsChess
open FSharp.Formatting.ApiDocs


[<EntryPoint>]
let main argv =
    let root = @"D:\GitHub\Grampus"
    let file = Path.Combine(root, @"debug\net5.0\FsChessPgn.dll")
    let input = ApiDocInput.FromFile(file,publicOnly = true)
    let bin = Path.Combine(root, @"debug\net5.0\")
    ApiDocs.GenerateHtml
        ( [ input ], 
          output=Path.Combine(root, "docs"),
          collectionName="FsChessPgn",
          libDirs = [bin],
          root = "/Grampus/",
          //template=Path.Combine(root, "templates", "template.html"),
          substitutions=[])|>ignore

    0 // return an integer exit code
