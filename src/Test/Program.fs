
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
                         ParamKeys.``fsdocs-release-notes-link``, "https://github.com/pbbwfc/Grampus/releases";
                         ParamKeys.``fsdocs-repository-link``, "https://github.com/pbbwfc/Grampus/";
                         ParamKeys.``fsdocs-license-link``, "https://github.com/pbbwfc/Grampus/blob/main/License.txt";
                         ParamKeys.``fsdocs-collection-name-link``, "https://pbbwfc.github.io/Grampus/";
                         
                         ])
          |>ignore

    0 // return an integer exit code
