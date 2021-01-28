
open System
open System.IO
open FsChess
open FSharp.Formatting.ApiDocs
open FSharp.Formatting.Templating



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
          template=Path.Combine(root, "src/Test", "_template.html"),
          substitutions=[ParamKeys.``root``, "/Grampus/";
                         ParamKeys.``fsdocs-authors``, "pbbwfc";
                         ParamKeys.``fsdocs-navbar-position``, "fixed-right";
                         ParamKeys.``fsdocs-theme``, "default";
                         ParamKeys.``fsdocs-watch-script``, "";
                         ParamKeys.``fsdocs-collection-name``, "Grampus";
                         ParamKeys.``fsdocs-list-of-documents``, "";
                         ParamKeys.``fsdocs-logo-link``,"https://pbbwfc.github.io/Grampus/";
                         ParamKeys.``fsdocs-logo-src``,"https://pbbwfc.github.io/Grampus/images/logo64.png";
                         ])
          |>ignore

    0 // return an integer exit code
