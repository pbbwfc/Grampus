namespace FsChess.WinForms

open System.Windows.Forms
open System.Drawing
open FsChess

[<AutoOpen>]
module PnlPgnLib =
    let private img nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("FsChess.WinForms.Images." + nm)
        Image.FromStream(file)

    type PnlPgn() as pgnpnl =
        inherit Panel()
        
        let tppnl = new Panel(Dock=DockStyle.Top,BorderStyle=BorderStyle.Fixed3D,Height=50)
        let gmlbl = new Label(Text="Game: White v. Black",Width=400,TextAlign=ContentAlignment.MiddleLeft,Font = new Font(new FontFamily("Arial"), 12.0f),Dock=DockStyle.Bottom)
        let ttpnl = new Panel(Left=100,Height=25)
        let ts = new ToolStrip(GripStyle=ToolStripGripStyle.Hidden)
        let pgn = new WebBrowser(AllowWebBrowserDrop = false,IsWebBrowserContextMenuEnabled = false,WebBrowserShortcutsEnabled = false,Dock=DockStyle.Fill)
        //mutables
        let mutable game = EncodedGameEMP
        let mutable board = Board.Start
        let mutable oldstyle:(HtmlElement*string) option = None
        let mutable irs = [-1]
        let mutable rirs = [-1]
        let mutable ccm = ""
        let mutable cng = NAG.Null
        let mutable gmchg = false
        
        //scinc related
        let mutable gnum = 0
        let mutable bnum = 0

        //events
        let bdchngEvt = new Event<_>()
        let gmchngEvt = new Event<_>()
        
        //functions
        let sethdr() =
            gmlbl.Text <- game.WhitePlayer + " v. " + game.BlackPlayer

        let hdr = "<html><body>"
        let ftr = "</body></html>"
        //given a rav id get then list of indexes to locate
        //[2;3;5] indicates go to RAV at index 2, withing this go to RAV at index 3 and then get item at index 5
        let rec getirs (ir:int64) irl =
            if ir<256L then (ir|>int)::irl
            else
                let nir = ir >>> 8
                let i = ir &&& (0x3F|>int64)
                getirs nir ((i|>int)::irl)
        //get a rav id from a list of indexes to locate
        //[2;,3;5] indicates go to RAV at index 2, withing this go to RAV at index 3 and then get item at index 5
        let getir (iirl:int list) =
            let rec dogetir (irl:int list) ir =
                if irl.IsEmpty then ir
                else
                    let nir = (irl.Head|>int64)|||(ir<<<8)
                    dogetir irl.Tail nir
            dogetir iirl 0L
        
        let removehighlight() =
            if oldstyle.IsSome then
                let omve,ostyle = oldstyle.Value
                omve.Style <- ostyle

        let highlight (mve:HtmlElement) =
            removehighlight()
            let curr = mve.Style
            oldstyle <- Some(mve,curr)
            mve.Style <- "BACKGROUND-COLOR: powderblue"
        
        let rec mvtag (ravno:int64) i (mte:EncodedMoveTextEntry) =
            let ir = (i|>int64)|||(ravno<<<8)

            let idstr = "id = \"" + ir.ToString() + "\""
            match mte with
            |EncodedHalfMoveEntry(mn,ic,emv) ->
                let str = 
                    let mnstr = if mn.IsSome then mn.Value.ToString() else ""
                    let dotstr = if mn.IsSome then (if ic then "... " else ". ") else ""
                    mnstr + dotstr + emv.San

                if ravno=0L then " <span " + idstr + " class=\"mv\" style=\"color:black\">" + str + "</span>"
                else " <span " + idstr + " class=\"mv\" style=\"color:darkslategray\">" + str + "</span>"
            |EncodedCommentEntry(c) ->
                let str = c
                "<div " + idstr + " class=\"cm\" style=\"color:green\">" + str + "</div>"
            |EncodedGameEndEntry(r) ->
                let str = Result.ToStr(r)
                " <span " + idstr + " class=\"ge\" style=\"color:blue\">" + str + "</span>"
            |EncodedNAGEntry(ng) ->
                let str = ng|>Game.NAGHtm
                "<span " + idstr + " class=\"ng\" style=\"color:darkred\">" + str + "</span>"
            |EncodedRAVEntry(mtel) ->
                let indent = 
                    let rirs = irs|>getirs ir
                    let ind = rirs.Length * 2
                    ";margin-left: " + ind.ToString() + "px"
                let str = mtel|>List.mapi (mvtag ir)|>List.reduce(+)
                "<div style=\"color:darkslategray" + indent + "\">(" + str + ")</div>"

        let mvtags() = 
            let mt = game.MoveText
            if mt.IsEmpty then hdr+ftr
            else
                hdr +
                (mt|>List.mapi (mvtag 0L)|>List.reduce(+))
                + ftr
        
        //dialogs
        let dlgcomm(offset,cm) = 
            let txt = if offset= -1 then "Add Comment Before" elif offset=1 then "Add Comment After" else "Edit Comment"
            let dlg = new Form(Text = txt, Height = 400, Width = 400, FormBorderStyle = FormBorderStyle.FixedToolWindow,StartPosition=FormStartPosition.CenterParent)
            let hc2 =
                new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
                                    Height = 30, Width = 400,Dock=DockStyle.Bottom)
            let okbtn = new Button(Text = "OK")
            let cnbtn = new Button(Text = "Cancel")
            //if edit need to load content
            let comm =
                new TextBox(Text = cm, Dock = DockStyle.Fill, 
                            Multiline = true, 
                            Font = new Font("Microsoft Sans Serif", 10.0f))
            let dook(e) = 
                //write comm.Text to comment
                if offset = -1 then
                    game <- Game.CommentBefore game rirs comm.Text
                    pgn.DocumentText <- mvtags()
                elif offset = 1 then 
                    game <- Game.CommentAfter game rirs comm.Text
                    pgn.DocumentText <- mvtags()
                else 
                    game <- Game.EditComment game rirs comm.Text
                    pgn.DocumentText <- mvtags()
                gmchg <- true
                gmchg|>gmchngEvt.Trigger
                dlg.Close()

            do 
                dlg.MaximizeBox <- false
                dlg.MinimizeBox <- false
                dlg.ShowInTaskbar <- false
                dlg.StartPosition <- FormStartPosition.CenterParent
                hc2.Controls.Add(cnbtn)
                hc2.Controls.Add(okbtn)
                dlg.Controls.Add(hc2)
                dlg.Controls.Add(comm)
                dlg.CancelButton <- cnbtn
                //events
                cnbtn.Click.Add(fun _ -> dlg.Close())
                okbtn.Click.Add(dook)

            dlg
       
        let dlgnag(offset,ing:NAG) = 
            let txt = if offset=1 then "Add NAG" else "Edit NAG"
            let dlg = new Form(Text = txt, Height = 300, Width = 400, FormBorderStyle = FormBorderStyle.FixedToolWindow,StartPosition=FormStartPosition.CenterParent)
            let hc2 =
                new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
                                    Height = 30, Width = 400,Dock=DockStyle.Bottom)
            let okbtn = new Button(Text = "OK")
            let cnbtn = new Button(Text = "Cancel")
            //if edit need to load NAG
            let tc = 
                new TableLayoutPanel(ColumnCount = 2, RowCount = 6, 
                                    Height = 260, Width = 200,Dock=DockStyle.Fill)
            let nags =
                //need to add radio buttons for each possible NAG
                let rbs = 
                   Game.NAGlist|>List.toArray
                   |>Array.map(fun ng -> (ng|>Game.NAGStr) + "   " + (ng|>Game.NAGDesc),ng)
                   |>Array.map(fun (lb,ng) -> new RadioButton(Text=lb,Width=200,Checked=(ng=ing)))
                rbs

            let dook(e) = 
                //get selected nag
                let indx = nags|>Array.findIndex(fun rb -> rb.Checked)
                let selNag = Game.NAGlist.[indx]
                //write nag to NAGEntry
                if offset = 1 && indx<>0 then 
                    game <- Game.AddNag game rirs selNag
                    pgn.DocumentText <- mvtags()
                elif offset = 0 && indx<>0 then 
                    game <- Game.EditNag game rirs selNag
                    pgn.DocumentText <- mvtags()
                else 
                    game <- Game.DeleteNag game rirs
                    pgn.DocumentText <- mvtags()

                gmchg<-true
                gmchg|>gmchngEvt.Trigger
                dlg.Close()

            do 
                dlg.MaximizeBox <- false
                dlg.MinimizeBox <- false
                dlg.ShowInTaskbar <- false
                dlg.StartPosition <- FormStartPosition.CenterParent
                hc2.Controls.Add(cnbtn)
                hc2.Controls.Add(okbtn)
                dlg.Controls.Add(hc2)
                nags.[0..6]|>Array.iteri(fun i rb -> tc.Controls.Add(rb,0,i))
                nags.[7..13]|>Array.iteri(fun i rb -> tc.Controls.Add(rb,1,i))
                dlg.Controls.Add(tc)
                dlg.CancelButton <- cnbtn
                //events
                cnbtn.Click.Add(fun _ -> dlg.Close())
                okbtn.Click.Add(dook)

            dlg

        let dlghdr() = 
            let dlg = new Form(Text = "Edit Headers", Height = 370, Width = 370, FormBorderStyle = FormBorderStyle.FixedToolWindow,StartPosition=FormStartPosition.CenterParent)
            let hc2 =
                new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
                                    Height = 30, Width = 400,Dock=DockStyle.Bottom)
            let okbtn = new Button(Text = "OK")
            let cnbtn = new Button(Text = "Cancel")
            //if edit need to load NAG
            let tc = 
                new TableLayoutPanel(ColumnCount = 2, RowCount = 9, 
                                    Height = 350, Width = 360,Dock=DockStyle.Fill)
            let wlbl = new Label(Text="White")
            let wtb = new TextBox(Text=game.WhitePlayer,Width=200)
            let blbl = new Label(Text="Black")
            let btb = new TextBox(Text=game.BlackPlayer,Width=200)
            let rslbl = new Label(Text="Result")
            let rscb = new ComboBox(Text=(game.Result|>Result.ToStr),Width=200)
            let dtlbl = new Label(Text="Date")
            let dttb = new TextBox(Text=(game|>GameDate.ToStr),Width=200)
            let evlbl = new Label(Text="Event")
            let evtb = new TextBox(Text=game.Event,Width=200)
            let welbl = new Label(Text="White Elo")
            let wetb = new TextBox(Text=game.WhiteElo,Width=200)
            let belbl = new Label(Text="Black Elo")
            let betb = new TextBox(Text=game.BlackElo,Width=200)
            let rdlbl = new Label(Text="Round")
            let rdtb = new TextBox(Text=game.Round,Width=200)
            let stlbl = new Label(Text="Site")
            let sttb = new TextBox(Text=game.Site,Width=200)
            
            let dook(e) = 
                let results = [|GameResult.WhiteWins;GameResult.BlackWins;GameResult.Draw;GameResult.Open|]
                let res = if rscb.SelectedIndex= -1 then game.Result else results.[rscb.SelectedIndex]
                //now update game end if result changed
                if res<>game.Result then
                    let chngres (mte:EncodedMoveTextEntry) =
                        match mte with
                        |EncodedGameEndEntry(_) -> EncodedGameEndEntry(res)
                        |_ -> mte

                    let nmvtxt = game.MoveText|>List.map chngres
                    game <- {game with MoveText=nmvtxt}

                let yo,mo,dyo =
                    let bits=dttb.Text.Split([|'.'|])
                    if bits.Length=3 then
                        let tryToInt (s:string) = 
                            match System.Int32.TryParse s with
                            | true, v -> Some v
                            | false, _ -> None
                        
                        bits.[0]|>tryToInt,bits.[1]|>tryToInt,bits.[2]|>tryToInt
                    else None,None,None
                game <- {game with WhitePlayer=wtb.Text;WhiteElo=wetb.Text;
                                   BlackPlayer=btb.Text;BlackElo=betb.Text;
                                   Result=res;Year=yo;Month=mo;Day=dyo;
                                   Event=evtb.Text;Round=rdtb.Text;Site=sttb.Text}
                
                gmchg<-true
                gmchg|>gmchngEvt.Trigger
                dlg.Close()


            do 
                dlg.MaximizeBox <- false
                dlg.MinimizeBox <- false
                dlg.ShowInTaskbar <- false
                dlg.StartPosition <- FormStartPosition.CenterParent
                hc2.Controls.Add(cnbtn)
                hc2.Controls.Add(okbtn)
                dlg.Controls.Add(hc2)
                tc.Controls.Add(wlbl,0,0)
                tc.Controls.Add(wtb,1,0)
                tc.Controls.Add(blbl,0,1)
                tc.Controls.Add(btb,1,1)
                tc.Controls.Add(rslbl,0,2)
                [|GameResult.WhiteWins;GameResult.BlackWins;GameResult.Draw;GameResult.Open|]
                |>Array.map(Result.ToStr)
                |>Array.iter(fun r -> rscb.Items.Add(r)|>ignore)
                tc.Controls.Add(rscb,1,2)
                tc.Controls.Add(dtlbl,0,3)
                tc.Controls.Add(dttb,1,3)
                tc.Controls.Add(evlbl,0,4)
                tc.Controls.Add(evtb,1,4)
                tc.Controls.Add(welbl,0,5)
                tc.Controls.Add(wetb,1,5)
                tc.Controls.Add(belbl,0,6)
                tc.Controls.Add(betb,1,6)
                tc.Controls.Add(rdlbl,0,7)
                tc.Controls.Add(rdtb,1,7)
                tc.Controls.Add(stlbl,0,8)
                tc.Controls.Add(sttb,1,8)

                dlg.Controls.Add(tc)
                dlg.CancelButton <- cnbtn
                //events
                cnbtn.Click.Add(fun _ -> dlg.Close())
                okbtn.Click.Add(dook)

            dlg
        
        let onclick(mve:HtmlElement) = 
            let i = mve.Id|>int64
            irs <- getirs i []
            let mv =
                if irs.Length>1 then 
                    let rec getmv (mtel:EncodedMoveTextEntry list) (intl:int list) =
                        if intl.Length=1 then mtel.[intl.Head]
                        else
                            let ih = intl.Head
                            let mte = mtel.[ih]
                            match mte with
                            |EncodedRAVEntry(nmtel) -> getmv nmtel intl.Tail
                            |_ -> failwith "should be a RAV"
                    getmv game.MoveText irs
                else
                    game.MoveText.[i|>int]
            match mv with
            |EncodedHalfMoveEntry(_,_,emv) ->
                board <- emv.PostBrd
                mve|>highlight
                pgn.Refresh()
                while(pgn.ReadyState <> WebBrowserReadyState.Complete) do
                        Application.DoEvents()
                board|>bdchngEvt.Trigger
                    
            |_ -> failwith "not done yet"
        
        let mvctxmnu = 
            let delrav(e) =
                game <- Game.DeleteRav game rirs
                pgn.DocumentText <- mvtags()
                gmchg <- true
                gmchg|>gmchngEvt.Trigger
               
            let strip(e) =
                game <- Game.Strip game rirs
                pgn.DocumentText <- mvtags()
                gmchg <- true
                gmchg|>gmchngEvt.Trigger

            let m = new ContextMenuStrip()
            //do edit comm before
            let adb =
                new ToolStripMenuItem(Text = "Add Comment Before")
            adb.Click.Add(fun _ -> dlgcomm(-1,"").ShowDialog() |> ignore)
            m.Items.Add(adb) |> ignore
            //do edit comm after
            let ada =
                new ToolStripMenuItem(Text = "Add Comment After")
            ada.Click.Add(fun _ -> dlgcomm(1,"").ShowDialog() |> ignore)
            m.Items.Add(ada) |> ignore
            //do add nag 
            let nag =
                new ToolStripMenuItem(Text = "Add NAG")
            nag.Click.Add(fun _ -> dlgnag(1,NAG.Null).ShowDialog() |> ignore)
            m.Items.Add(nag) |> ignore
            //do edit hdrs
            let hdr =
                new ToolStripMenuItem(Text = "Edit Headers")
            hdr.Click.Add(fun _ -> dlghdr().ShowDialog() |> ignore)
            m.Items.Add(hdr) |> ignore
            //do delete rav
            let dlr =
                new ToolStripMenuItem(Text = "Delete Variation")
            dlr.Click.Add(delrav)
            m.Items.Add(dlr) |> ignore
            //do strip moves
            let str =
                new ToolStripMenuItem(Text = "Strip Moves to End")
            str.Click.Add(strip)
            m.Items.Add(str) |> ignore
            m

        let cmctxmnu = 
            let delcm(e) =
                game <- Game.DeleteComment game rirs
                pgn.DocumentText <- mvtags()
                gmchg <- true
                gmchg|>gmchngEvt.Trigger
               
            
            let m = new ContextMenuStrip()
            //do edit comm 
            let ed =
                new ToolStripMenuItem(Text = "Edit Comment")
            ed.Click.Add(fun _ -> dlgcomm(0,ccm).ShowDialog() |> ignore)
            m.Items.Add(ed) |> ignore
            //do delete comm 
            let dl =
                new ToolStripMenuItem(Text = "Delete Comment")
            dl.Click.Add(delcm)
            m.Items.Add(dl) |> ignore
            m

        let ngctxmnu = 
            let m = new ContextMenuStrip()
            //do edit nag 
            let ed =
                new ToolStripMenuItem(Text = "Edit NAG")
            ed.Click.Add(fun _ -> dlgnag(0,cng).ShowDialog() |> ignore)
            m.Items.Add(ed) |> ignore
            m

        let onrightclick(el:HtmlElement,psn) = 
            rirs <- getirs (el.Id|>int64) []
            if el.GetAttribute("className") = "mv" then mvctxmnu.Show(pgn,psn)
            elif el.GetAttribute("className") = "cm" then 
                ccm <- el.InnerText
                cmctxmnu.Show(pgn,psn)
            elif el.GetAttribute("className") = "ng" then 
                cng <- el.InnerText|>Game.NAGFromStr
                ngctxmnu.Show(pgn,psn)

        let setclicks e = 
            for el in pgn.Document.GetElementsByTagName("span") do
                if el.GetAttribute("className") = "mv" then
                    el.MouseDown.Add(fun e -> if e.MouseButtonsPressed=MouseButtons.Left then onclick(el) else onrightclick(el,e.MousePosition))
                elif el.GetAttribute("className") = "ng" then
                    el.MouseDown.Add(fun e -> if e.MouseButtonsPressed=MouseButtons.Left then () else onrightclick(el,e.MousePosition))
            for el in pgn.Document.GetElementsByTagName("div") do
                if el.GetAttribute("className") = "cm" then 
                    el.MouseDown.Add(fun e -> if e.MouseButtonsPressed=MouseButtons.Left then () else onrightclick(el,e.MousePosition))

            let id = getir irs
            for el in pgn.Document.GetElementsByTagName("span") do
                if el.GetAttribute("className") = "mv" then
                    if el.Id=id.ToString() then
                        el|>highlight

        let createts() = 
            // new
            let homeb = new ToolStripButton(Image = img "homeButton.png", ImageTransparentColor = Color.Magenta, DisplayStyle = ToolStripItemDisplayStyle.Image, Text = "Home")
            homeb.Click.Add(fun _ -> pgnpnl.FirstMove())
            ts.Items.Add(homeb)|>ignore
            let prevb = new ToolStripButton(Image = img "prevButton.png", ImageTransparentColor = Color.Magenta, DisplayStyle = ToolStripItemDisplayStyle.Image, Text = "Previous")
            prevb.Click.Add(fun _ -> pgnpnl.PrevMove(true))
            ts.Items.Add(prevb)|>ignore
            let nextb = new ToolStripButton(Image = img "nextButton.png", ImageTransparentColor = Color.Magenta, DisplayStyle = ToolStripItemDisplayStyle.Image, Text = "Next")
            nextb.Click.Add(fun _ -> pgnpnl.NextMove(true))
            ts.Items.Add(nextb)|>ignore
            let endb = new ToolStripButton(Image = img "endButton.png", ImageTransparentColor = Color.Magenta, DisplayStyle = ToolStripItemDisplayStyle.Image, Text = "End")
            endb.Click.Add(fun _ -> pgnpnl.LastMove())
            ts.Items.Add(endb)|>ignore
 
        do
            pgn.DocumentText <- mvtags()
            pgn.DocumentCompleted.Add(setclicks)
            //pgn.ObjectForScripting <- pgn
            pgnpnl.Controls.Add(pgn)
            createts()
            ttpnl.Controls.Add(ts)
            tppnl.Controls.Add(ttpnl)
            tppnl.Controls.Add(gmlbl)
            pgnpnl.Controls.Add(tppnl)

        ///Gets the Game that is displayed
        member _.GetGame() = 
            game

        ///Gets the Game that is displayed
        member _.GetGameNumber() = 
            gnum
        
        ///Saves the Game that is displayed
        member _.SaveGame() = 
            let pgnstr = Game.ToStr(game)
            //ScincFuncs.ScidGame.SavePgn(pgnstr,uint(gnum),bnum)|>ignore
            //need to reset gnum for new game
            //if gnum=0 then gnum<-ScincFuncs.Base.NumGames()
            gmchg<-false
            gmchg|>gmchngEvt.Trigger

        ///Saves the game with prompt
        member _.PromptSaveGame() = 
            let dr = MessageBox.Show("Do you want to save the game: " + gmlbl.Text + " ?","Save Game",MessageBoxButtons.YesNo)
            if dr=DialogResult.Yes then 
                pgnpnl.SaveGame()
            else
                gmchg<-false
                gmchg|>gmchngEvt.Trigger
            
        ///Switches to another game with the same position
        member pgnpnl.SwitchGame(rw:int) = 
            gnum <- rw
            //select game
            //ScincFuncs.ScidGame.Load(uint32(rw))|>ignore
            //load game
            let mutable pgnstr = ""
            //ScincFuncs.ScidGame.Pgn(&pgnstr)|>ignore
            let gm = Game.FromStr(pgnstr)
            //need to check if want to save
            if gmchg then pgnpnl.PromptSaveGame()
            game <- gm|>Game.Encode
            pgn.DocumentText <- mvtags()
            //need to select move that matches current board
            let rec getnxt ci (mtel:EncodedMoveTextEntry list) =
                if mtel.IsEmpty then -1
                else
                    let mte = mtel.Head
                    match mte with
                    |EncodedHalfMoveEntry(_,_,emv) ->
                        if board = emv.PostBrd then ci
                        else getnxt (ci+1) mtel.Tail
                    |_ -> getnxt (ci+1) mtel.Tail
            let ni = getnxt 0 game.MoveText
            irs <- [ni]
            //now need to select the element
            let id = getir irs
            for el in pgn.Document.GetElementsByTagName("span") do
                if el.GetAttribute("className") = "mv" then
                    if el.Id=id.ToString() then
                        el|>highlight
            sethdr()
 
        ///Sets the Game to be displayed
        member pgnpnl.SetGame(gm:UnencodedGame) = 
            //need to check if want to save
            if gmchg then pgnpnl.PromptSaveGame()
            game <- gm|>Game.Encode
            pgn.DocumentText <- mvtags()
            board <- Board.Start
            oldstyle <- None
            irs <- [-1]
            sethdr()

        member pgnpnl.NewGame(ibnum) =
            let gm = Game.Start
            pgnpnl.SetGame(gm)
            gnum <- 0
            bnum <- ibnum

        member pgnpnl.Refrsh(ignum:int,ibnum) =
            let mutable pgnstr = ""
            ()
            //if ScincFuncs.ScidGame.Pgn(&pgnstr)=0 then
            //    let gm = Game.FromStr(pgnstr)
            //    pgnpnl.SetGame(gm)
            //    gnum <- ignum
            //    bnum <- ibnum
        
        member pgnpnl.SetPgn(pgnstr:string) =
            let gm = Game.FromStr(pgnstr)
            gmchg<-false
            pgnpnl.SetGame(gm)
            gmchg<-true
            gmchg|>gmchngEvt.Trigger

        member pgnpnl.GetPgn() =
            let pgnstr = Game.ToStr(game)
            pgnstr

        ///Goes to the next Move in the Game
        member pgnpnl.NextMove(one:bool) = 
            let rec getnxt oi ci (mtel:EncodedMoveTextEntry list) =
                if ci=mtel.Length then oi
                else
                    let mte = mtel.[ci]
                    match mte with
                    |EncodedHalfMoveEntry(_,_,emv) ->
                        board <- emv.PostBrd
                        if one then board|>bdchngEvt.Trigger
                        ci
                    |_ -> getnxt oi (ci+1) mtel
            if irs.Length>1 then 
                let rec getmv (mtel:EncodedMoveTextEntry list) (intl:int list) =
                    if intl.Length=1 then
                        let oi = intl.Head
                        let ni = getnxt oi (oi+1) mtel
                        let st = irs|>List.rev|>List.tail|>List.rev
                        irs <- st@[ni]
                    else
                        let ih = intl.Head
                        let mte = mtel.[ih]
                        match mte with
                        |EncodedRAVEntry(nmtel) -> getmv nmtel intl.Tail
                        |_ -> failwith "should be a RAV"
                getmv game.MoveText irs
            else
                let ni = getnxt irs.Head (irs.Head+1) game.MoveText
                irs <- [ni]
            //now need to select the element
            let id = getir irs
            for el in pgn.Document.GetElementsByTagName("span") do
                if el.GetAttribute("className") = "mv" then
                    if el.Id=id.ToString() then
                        el|>highlight
        
        ///Goes to the last Move in the Variation
        member pgnpnl.LastMove() = 
            let rec gofwd lirs =
                pgnpnl.NextMove(false)
                if irs<>lirs then gofwd irs
            if irs<>[-1] then
                gofwd irs
                board|>bdchngEvt.Trigger
        
        ///Goes to the previous Move in the Game
        member pgnpnl.PrevMove(one:bool) = 
            let rec getprv oi ci (mtel:EncodedMoveTextEntry list) =
                if ci<0 then oi
                else
                    let mte = mtel.[ci]
                    match mte with
                    |EncodedHalfMoveEntry(_,_,emv) ->
                        board <- emv.PostBrd
                        if one then board|>bdchngEvt.Trigger
                        ci
                    |_ -> getprv oi (ci-1) mtel
            if irs=[-1] then ()
            elif irs=[0] then
                let mte = game.MoveText.[0]
                match mte with
                |EncodedHalfMoveEntry(_,_,emv) ->
                    board <- if game.BoardSetup.IsSome then game.BoardSetup.Value else Board.Start
                    if one then board|>bdchngEvt.Trigger
                    removehighlight()
                    irs<-[-1]
                |_ -> ()
            else
                if irs.Length>1 then 
                    let rec getmv (mtel:EncodedMoveTextEntry list) (intl:int list) =
                        if intl.Length=1 then
                            let oi = intl.Head
                            let ni = getprv oi (oi-1) mtel
                            let st = irs|>List.rev|>List.tail|>List.rev
                            irs <- st@[ni]
                        else
                            let ih = intl.Head
                            let mte = mtel.[ih]
                            match mte with
                            |EncodedRAVEntry(nmtel) -> getmv nmtel intl.Tail
                            |_ -> failwith "should be a RAV"
                    getmv game.MoveText irs
                else
                    let ni = getprv irs.Head (irs.Head-1) game.MoveText
                    irs <- [ni]
                //now need to select the element
                if one then
                    let id = getir irs
                    for el in pgn.Document.GetElementsByTagName("span") do
                        if el.GetAttribute("className") = "mv" then
                            if el.Id=id.ToString() then
                                el|>highlight

        ///Goes to the first Move in the Variation
        member pgnpnl.FirstMove() = 
            let rec goback lirs =
                pgnpnl.PrevMove(false)
                if irs<>lirs then goback irs
            if irs<>[-1] then 
                goback irs
                board|>bdchngEvt.Trigger

        ///Make a Move in the Game - may change the Game or just select a Move
        member pgnpnl.DoMove(mv:Move) =
            let rec getnxt oi ci (mtel:EncodedMoveTextEntry list) =
                if ci=mtel.Length then ci,false,true//implies is an extension
                else
                    let mte = mtel.[ci]
                    match mte with
                    |EncodedHalfMoveEntry(_,_,emv) ->
                        if emv.Mv=mv then
                            board <- emv.PostBrd
                            ci,true,false
                        else ci,false,false
                    |_ -> getnxt oi (ci+1) mtel
            let isnxt,isext =
                if irs.Length>1 then 
                    let rec getmv (mtel:EncodedMoveTextEntry list) (intl:int list) =
                        if intl.Length=1 then
                            let oi = intl.Head
                            let ni,fnd,isext = getnxt oi (oi+1) mtel
                            if fnd then
                                let st = irs|>List.rev|>List.tail|>List.rev
                                irs <- st@[ni]
                            fnd,isext
                        else
                            let ih = intl.Head
                            let mte = mtel.[ih]
                            match mte with
                            |EncodedRAVEntry(nmtel) -> getmv nmtel intl.Tail
                            |_ -> failwith "should be a RAV"
                    getmv game.MoveText irs
                else
                    let ni,fnd,isext = getnxt irs.Head (irs.Head+1) game.MoveText
                    if fnd then irs <- [ni]
                    fnd,isext
            if isnxt then
                //now need to select the element
                let id = getir irs
                for el in pgn.Document.GetElementsByTagName("span") do
                    if el.GetAttribute("className") = "mv" then
                        if el.Id=id.ToString() then
                            el|>highlight
            elif isext then
                let ngame,nirs = Game.AddMv game irs mv 
                game <- ngame
                irs <- nirs
                board <- board|>Board.Push mv
                pgn.DocumentText <- mvtags()
                gmchg<-true
                gmchg|>gmchngEvt.Trigger
            else
                //Check if first move in RAV
                let rec inrav oi ci (mtel:EncodedMoveTextEntry list) =
                    if ci=mtel.Length then ci,false //Should not hit this as means has no moves
                    else
                        let mte = mtel.[ci]
                        match mte with
                        |EncodedHalfMoveEntry(_,_,emv) ->
                            if emv.Mv=mv then
                                board <- emv.PostBrd
                                ci,true
                            else ci,false
                        |_ -> inrav oi (ci+1) mtel
                //next see if moving into RAV
                let rec getnxtrv oi ci mct (mtel:EncodedMoveTextEntry list) =
                    if ci=mtel.Length then ci,0,false //TODO this is an extension to RAV or Moves
                    else
                        let mte = mtel.[ci]
                        if mct = 0 then
                            match mte with
                            |EncodedHalfMoveEntry(_,_,emv) ->
                                getnxtrv oi (ci+1) (mct+1) mtel
                            |_ -> getnxtrv oi (ci+1) mct mtel
                        else
                            match mte with
                            |EncodedHalfMoveEntry(_,_,emv) ->
                                ci,0,false
                            |EncodedRAVEntry(nmtel) ->
                                //now need to see if first move in rav is mv
                                let sci,fnd = inrav 0 0 nmtel
                                if fnd then
                                    ci,sci,fnd
                                else getnxtrv oi (ci+1) mct mtel
                            |_ -> getnxtrv oi (ci+1) mct mtel
                let isnxtrv =
                    if irs.Length>1 then 
                        let rec getmv (mtel:EncodedMoveTextEntry list) (intl:int list) =
                            if intl.Length=1 then
                                let oi = intl.Head
                                let ni,sci,fnd = getnxtrv oi (oi+1) 0 mtel
                                if fnd then
                                    let st = irs|>List.rev|>List.tail|>List.rev
                                    irs <- st@[ni;sci]
                                fnd
                            else
                                let ih = intl.Head
                                let mte = mtel.[ih]
                                match mte with
                                |EncodedRAVEntry(nmtel) -> getmv nmtel intl.Tail
                                |_ -> failwith "should be a RAV"
                        getmv game.MoveText irs
                    else
                        let ni,sci,fnd = getnxtrv irs.Head (irs.Head+1) 0 game.MoveText
                        if fnd then irs <- [ni;sci]
                        fnd
                if isnxtrv then
                    //now need to select the element
                    let id = getir irs
                    for el in pgn.Document.GetElementsByTagName("span") do
                        if el.GetAttribute("className") = "mv" then
                            if el.Id=id.ToString() then
                                el|>highlight
                    else
                        //need to create a new RAV
                        let ngame,nirs = Game.AddRav game irs mv  
                        game <- ngame
                        irs <- nirs
                        board <- board|>Board.Push mv
                        pgn.DocumentText <- mvtags()
                        gmchg<-true
                        gmchg|>gmchngEvt.Trigger

        ///edit game headers
        member pgnlpnl.EditHeaders() =
            dlghdr().ShowDialog() |> ignore

        ///set ECO for game
        member pgnlpnl.SetECO() =
            let mutable eco = ""
            ()
            //if ScincFuncs.Eco.ScidGame(&eco)=0 then
            //    game <- {game with ECO=eco}
            //    gmchg<-true
            //    gmchg|>gmchngEvt.Trigger

        //publish
        ///Provides the new Board after a change
        member __.BdChng = bdchngEvt.Publish

        ///Provides the new Game after a change
        member __.GmChng = gmchngEvt.Publish
