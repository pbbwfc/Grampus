namespace FsChessPgn

open FsChess
open System.IO

module Png =
    
    ///Creates a PNG image file with the specified name(nm), flipped if specified for the given Board(bd) 
    let BoardToPng (nm: string) (flip : bool) (bd : Brd) =
         let imgsfol =
            #if INTERACTIVE
            Path.Combine(__SOURCE_DIRECTORY__,"imgs")
            #else
            Path.Combine (Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly().Location), "imgs")
            #endif

     
         let c2tl i (c:string) =
             let x = i % 8
             let y = i / 8
         
             let sqcol =
                 if (x + y) % 2 = 0 then "l"
                 else "d"
         
             let pccol =
                 if c.ToUpper() = c then "l"
                 else "d"
         
             let pc = c
         
             let fn =
                 if c = " " then "Chess_" + sqcol + "44.png"
                 else "Chess_" + pc + pccol + sqcol + "44.png"
             let pfn:string = System.IO.Path.Combine(imgsfol, fn) 
             let img:System.Drawing.Bitmap = new System.Drawing.Bitmap(pfn)
             img
     
         let tls = bd.PieceAt|>List.map Piece.PieceToString |> List.mapi c2tl
         let w = 44
         let h = 44
         let size = new System.Drawing.Size(w, h)
         let image = new System.Drawing.Bitmap(8 * w, 8 * h, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
         for y = 0 to 7 do
             for x = 0 to 7 do
                 let location =
                     if flip then new System.Drawing.Point((7 - x) * w, (7 - y) * h)
                     else new System.Drawing.Point(x * w, y * h)
             
                 let tl = tls.[y * 8 + x]
                 System.Drawing.Graphics.FromImage(image)
                         .DrawImage(tl, new System.Drawing.Rectangle(location, size))
         image.Save(nm, System.Drawing.Imaging.ImageFormat.Png)

