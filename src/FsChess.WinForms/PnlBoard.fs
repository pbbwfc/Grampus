namespace FsChess.WinForms

open System.Drawing
open System.Windows.Forms
open FsChess

[<AutoOpen>]
module PnlBoardLib =
    
    let private img nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("FsChess.WinForms.Images." + nm)
        Image.FromStream(file)

    let private cur nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        //let nms = thisExe.GetManifestResourceNames()
        let file = thisExe.GetManifestResourceStream("FsChess.WinForms.Cursors." + nm)
        new Cursor(file)
 
    type PnlBoard() as bd =
        inherit Panel(Width = 400, Height = 420)

        let mutable board = Board.Start
        let mutable sqTo = -1
        let mutable cCur = Cursors.Default
        let mutable prompctp = PieceType.EMPTY
        let mutable isw = true
        let bdpnl = new Panel(Dock = DockStyle.Top, Height = 400)
        let sqpnl = new Panel(Width = 420, Height = 420, Left = 29, Top = 13)
        
        let edges =
            [ new Panel(BackgroundImage = img "Back.jpg", Width = 342, 
                        Height = 8, Left = 24, Top = 6)
              
              new Panel(BackgroundImage = img "Back.jpg", Width = 8, 
                        Height = 350, Left = 24, Top = 8)
              
              new Panel(BackgroundImage = img "Back.jpg", Width = 8, 
                        Height = 350, Left = 366, Top = 6)
              
              new Panel(BackgroundImage = img "Back.jpg", Width = 342, 
                        Height = 8, Left = 32, Top = 350) ]
        
        let sqs : PictureBox [] = Array.zeroCreate 64
        let flbls : Label [] = Array.zeroCreate 8
        let rlbls : Label [] = Array.zeroCreate 8

        let dlgprom = 
            let dlg = new Form(Text = "Select Piece", Height = 88, Width = 182, FormBorderStyle = FormBorderStyle.FixedToolWindow,StartPosition=FormStartPosition.CenterParent)
            let sqs : PictureBox [] = Array.zeroCreate 4
        
            let bpcims =
                [ img "BlackQueen.png"
                  img "BlackRook.png"
                  img "BlackKnight.png"
                  img "BlackBishop.png" ]
        
            let wpcims =
                [ img "WhiteQueen.png"
                  img "WhiteRook.png"
                  img "WhiteKnight.png"
                  img "WhiteBishop.png" ]
        
            ///set pieces on squares
            let setsq i (sq : PictureBox) =
                let sq = new PictureBox(Height = 42, Width = 42, SizeMode = PictureBoxSizeMode.CenterImage)
                sq.BackColor <- if i % 2 = 0 then Color.Green else Color.PaleGreen
                sq.Top <- 1
                sq.Left <- i * 42 + 1
                sq.Image <- if board.WhosTurn=Player.White then wpcims.[i] else bpcims.[i]
                //events
                let pctps = [ PieceType.Queen;PieceType.Rook;PieceType.Knight;PieceType.Bishop]
                sq.Click.Add(fun e -> 
                    prompctp <- pctps.[i]
                    dlg.Close())
                sqs.[i] <- sq
        
            do 
                sqs |> Array.iteri setsq
                sqs |> Array.iter dlg.Controls.Add
            dlg

        //events
        let mvEvt = new Event<_>()

        //functions
        /// get cursor given char
        let getcur c =
            match c with
            | "P" -> cur "WhitePawn.cur"
            | "B" -> cur "WhiteBishop.cur"
            | "N" -> cur "WhiteKnight.cur"
            | "R" -> cur "WhiteRook.cur"
            | "K" -> cur "WhiteKing.cur"
            | "Q" -> cur "WhiteQueen.cur"
            | "p" -> cur "BlackPawn.cur"
            | "b" -> cur "BlackBishop.cur"
            | "n" -> cur "BlackKnight.cur"
            | "r" -> cur "BlackRook.cur"
            | "k" -> cur "BlackKing.cur"
            | "q" -> cur "BlackQueen.cur"
            | _ -> Cursors.Default
 
        /// get image given char
        let getim c =
            match c with
            | "P" -> img "WhitePawn.png"
            | "B" -> img "WhiteBishop.png"
            | "N" -> img "WhiteKnight.png"
            | "R" -> img "WhiteRook.png"
            | "K" -> img "WhiteKing.png"
            | "Q" -> img "WhiteQueen.png"
            | "p" -> img "BlackPawn.png"
            | "b" -> img "BlackBishop.png"
            | "n" -> img "BlackKnight.png"
            | "r" -> img "BlackRook.png"
            | "k" -> img "BlackKing.png"
            | "q" -> img "BlackQueen.png"
            | _ -> failwith "invalid piece"

        ///set pieces on squares
        let setpcsmvs () =
            let setpcsmvs() =
                board.PieceAt
                |>List.map Piece.ToStr
                |> List.iteri (fun i c -> sqs.[i].Image <- if c = " " then null else getim c)
            if (bd.InvokeRequired) then 
                try 
                    bd.Invoke(MethodInvoker(setpcsmvs)) |> ignore
                with _ -> ()
            else setpcsmvs()

        ///orient board
        let orient() =
            isw <- not isw
            let possq i (sq : PictureBox) =
                let r = i / 8
                let f = i % 8
                if not isw then 
                    sq.Top <- r * 42 + 1
                    sq.Left <- 7 * 42 - f * 42 + 1
                else 
                    sq.Left <- f * 42 + 1
                    sq.Top <- 7 * 42 - r * 42 + 1
            sqs |> Array.iteri possq
            flbls
            |> Array.iteri (fun i l -> 
                    if isw then l.Left <- i * 42 + 30
                    else l.Left <- 7 * 42 - i * 42 + 30)
            rlbls
            |> Array.iteri (fun i l -> 
                    if isw then l.Top <- 7 * 42 - i * 42 + 16
                    else l.Top <- i * 42 + 16)
 
        ///highlight possible squares
        let highlightsqs sl =
            sqs
            |> Array.iteri (fun i sq -> 
                   sqs.[i].BackColor <- if (i % 8 + i / 8) % 2 = 0 then 
                                            Color.Green
                                        else Color.PaleGreen)
            sl
            |> List.iter (fun s -> 
                   sqs.[s].BackColor <- if (s % 8 + s / 8) % 2 = 0 then 
                                            Color.YellowGreen
                                        else Color.Yellow)

        /// Action for GiveFeedback
        let giveFeedback (e : GiveFeedbackEventArgs) =
            e.UseDefaultCursors <- false
            sqpnl.Cursor <- cCur

        /// Action for Drag Over
        let dragOver (e : DragEventArgs) = e.Effect <- DragDropEffects.Move

        /// Action for Drag Drop
        let dragDrop (p : PictureBox, e) =
            sqTo <- System.Convert.ToInt32(p.Tag)
            sqpnl.Cursor <- Cursors.Default

        /// Action for Mouse Down
        let mouseDown (p : PictureBox, e : MouseEventArgs) =
            if e.Button = MouseButtons.Left then 
                let sqFrom = System.Convert.ToInt32(p.Tag)
                let sqf:Square = sqFrom|>int16
                let psmvs = sqf|>Board.PossMoves board
                let pssqs = psmvs|>List.map(fun m -> m|>Move.To|>int)
                pssqs|>highlightsqs
                let oimg = p.Image
                p.Image <- null
                p.Refresh()
                let c = board.PieceAt.[sqFrom]|>Piece.ToStr
                cCur <- getcur c
                sqpnl.Cursor <- cCur
                if pssqs.Length > 0 && (p.DoDragDrop(oimg, DragDropEffects.Move) = DragDropEffects.Move) then 
                    let mvl = psmvs|>List.filter(fun m ->m|>Move.To|>int=sqTo)
                    if mvl.Length=1 then
                        board <- board|>Board.Push mvl.Head
                        setpcsmvs()
                        mvl.Head|>mvEvt.Trigger
                    //need to allow for promotion
                    elif mvl.Length=4 then
                        dlgprom.ShowDialog() |> ignore
                        let nmvl = mvl|>List.filter(fun mv -> mv|>Move.PromPcTp=prompctp)
                        board <- board|>Board.Push nmvl.Head
                        setpcsmvs()
                        nmvl.Head|>mvEvt.Trigger
                    else p.Image <- oimg
                else p.Image <- oimg
                sqpnl.Cursor <- Cursors.Default
                []|>highlightsqs

        /// creates file label
        let flbl i lbli =
            let lbl = new Label()
            lbl.Text <- FILE_NAMES.[i]
            lbl.Font <- new Font("Arial", 12.0F, FontStyle.Bold, 
                                 GraphicsUnit.Point, byte (0))
            lbl.ForeColor <- Color.Green
            lbl.Height <- 21
            lbl.Width <- 42
            lbl.TextAlign <- ContentAlignment.MiddleCenter
            lbl.Left <- i * 42 + 30
            lbl.Top <- 8 * 42 + 24
            flbls.[i] <- lbl

        /// creates rank label
        let rlbl i lbli =
            let lbl = new Label()
            lbl.Text <- (i + 1).ToString()
            lbl.Font <- new Font("Arial", 12.0F, FontStyle.Bold, 
                                 GraphicsUnit.Point, byte (0))
            lbl.ForeColor <- Color.Green
            lbl.Height <- 42
            lbl.Width <- 21
            lbl.TextAlign <- ContentAlignment.MiddleCenter
            lbl.Left <- 0
            lbl.Top <- 7 * 42 - i * 42 + 16
            rlbls.[i] <- lbl
       
        ///set board colours and position of squares
        let setsq i sqi =
            let r = i / 8
            let f = i % 8
            let sq =
                new PictureBox(Height = 42, Width = 42, 
                               SizeMode = PictureBoxSizeMode.CenterImage)
            sq.BackColor <- if (f + r) % 2 = 0 then Color.Green
                            else Color.PaleGreen
            sq.Left <- f * 42 + 1
            sq.Top <- 7 * 42 - r * 42 + 1
            sq.Tag <- i
            //events
            sq.MouseDown.Add(fun e -> mouseDown (sq, e))
            sq.DragDrop.Add(fun e -> dragDrop (sq, e))
            sq.AllowDrop <- true
            sq.DragOver.Add(dragOver)
            sq.GiveFeedback.Add(giveFeedback)
            sqs.[i] <- sq
        
        do 
            sqs |> Array.iteri setsq
            sqs |> Array.iter sqpnl.Controls.Add
            setpcsmvs()
            edges |> List.iter bdpnl.Controls.Add
            flbls |> Array.iteri flbl
            flbls |> Array.iter bdpnl.Controls.Add
            rlbls |> Array.iteri rlbl
            rlbls |> Array.iter bdpnl.Controls.Add
            sqpnl |> bdpnl.Controls.Add
            bdpnl |> bd.Controls.Add

        ///Sets the Board to be displayed
        member bd.SetBoard(ibd:Brd) =
            board<-ibd
            setpcsmvs()

        ///Gets the Board to be displayed
        member bd.GetBoard() =
            board
        
        ///Orients the Board depending on whether White
        member bd.Orient() =
            orient()

        ///Sets the board given a new move in SAN format
        member bd.DoMove(san:string) =
            let mv = san|>Move.FromSan board
            board <- board|>Board.Push mv
            setpcsmvs()
            
        //publish
        ///Provides the Move made on the board
        member __.MvMade = mvEvt.Publish
