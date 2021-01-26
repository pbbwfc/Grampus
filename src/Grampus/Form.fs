namespace ScincNet

open System.IO
open System.Drawing
open System.Windows.Forms
open FsChess.WinForms
open FsChess

module Form =
    let img nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("Grampus.Images." + nm)
        Image.FromStream(file)
    let ico nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("Grampus.Icons." + nm)
        new Icon(file)

    type FrmMain() as this =
        inherit Form(Text = "Grampus", WindowState = FormWindowState.Maximized, IsMdiContainer = true, Icon = ico "scinc.ico")

        let bfol = 
            let pth = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"Grampus\\bases")
            Directory.CreateDirectory(pth)|>ignore
            pth
        let bd = new PnlBoard(Dock=DockStyle.Fill)
        let pgn = new PnlPgn(Dock=DockStyle.Fill)
        let sts = new WbStats(Dock=DockStyle.Fill)
        let gmtbs = new TcGames(Dock=DockStyle.Fill)
        let anl = new TcAnl(Dock=DockStyle.Fill)
        let ts = new ToolStrip(GripStyle=ToolStripGripStyle.Hidden)
        let ms = new MenuStrip()
        let saveb = new ToolStripButton(Image = img "sav.png", ImageTransparentColor = Color.Magenta, DisplayStyle = ToolStripItemDisplayStyle.Image, Text = "&Save", Enabled = false)
        let savem = new ToolStripMenuItem(Image = img "sav.png", ImageTransparentColor = Color.Magenta, ShortcutKeys = (Keys.Control|||Keys.S), Text = "&Save", Enabled = false)
        let closeb = new ToolStripButton(Image = img "cls.png", ImageTransparentColor = Color.Magenta, DisplayStyle = ToolStripItemDisplayStyle.Image, Text = "&Close", Enabled = false)
        let closem = new ToolStripMenuItem(Image = img "cls.png", ImageTransparentColor = Color.Magenta, ShortcutKeys = (Keys.Control|||Keys.W), Text = "&Close", Enabled = false)
        let cmpm = new ToolStripMenuItem(Text = "Compact Base", Enabled = false)
        let impm = new ToolStripMenuItem(Text = "Import PGN file", Enabled = false)
        let ecom = new ToolStripMenuItem(Text = "Set ECOs", Enabled = false)
        let showwb = new ToolStripButton(Image = img "white.png", Enabled = false, Text = "Show White")
        let showwm = new ToolStripMenuItem(Image = img "white.png", Text = "Show White", CheckState = CheckState.Unchecked, Enabled = false)
        let showbb = new ToolStripButton(Image = img "black.png", Enabled = false, Text = "Show Black")
        let showbm = new ToolStripMenuItem(Image = img "black.png", Text = "Show Black", CheckState = CheckState.Unchecked, Enabled = false)
        let ss = new StatusStrip(LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow, Anchor=AnchorStyles.Bottom, Text = "No bases open", Dock = DockStyle.Bottom)
        let sl = new ToolStripStatusLabel(Text="Ready")
                
        let SbUpdate(txt) =
            //TODO may add some timing and logging
            sl.Text <- txt
            Application.DoEvents()

        let updateMenuStates() =
            closeb.Enabled<-gmtbs.TabCount>1
            closem.Enabled<-gmtbs.TabCount>1
            cmpm.Enabled<-gmtbs.TabCount>1
            impm.Enabled<-gmtbs.TabCount>1
            ecom.Enabled<-gmtbs.TabCount>1
            showwb.Enabled<-gmtbs.TabCount>1
            showwm.Enabled<-gmtbs.TabCount>1
            showbb.Enabled<-gmtbs.TabCount>1
            showbm.Enabled<-gmtbs.TabCount>1

        let updateTitle() =
            let gmpfile = ""//gmtbs.Getfilename() 
            this.Text <- "Grampus - " + Path.GetFileNameWithoutExtension(gmpfile)
        
        let refreshWindows() =
            updateMenuStates()
            updateTitle()

        let waitify f =
            let cu = this.Cursor
            try
                try
                    this.Cursor <- Cursors.WaitCursor
                    this.Enabled <-false
                    let st = System.DateTime.Now
                    f()
                    let nd = System.DateTime.Now
                    let el = (nd-st).Seconds
                    ()
                with 
                | ex -> MessageBox.Show(ex.Message,"Process Failed")|>ignore
            finally
                this.Enabled <-true
                this.Cursor <- cu
        
        let donew() =
            let ndlg = new SaveFileDialog(Title="Create New Database",Filter="Grampus databases(*.grampus)|*.grampus",AddExtension=true,OverwritePrompt=false,InitialDirectory=bfol)
            if ndlg.ShowDialog() = DialogResult.OK then
                //create database
                let gmpfile = ndlg.FileName
                SbUpdate("Creating base: " + gmpfile)
                let gmp = GrampusDataEMP
                Grampus.Save(gmpfile,gmp)
                SbUpdate("Updating windows")
                Recents.addrec gmpfile
                let nm = Path.Combine(Path.GetDirectoryName(gmpfile),Path.GetFileNameWithoutExtension(gmpfile))
                gmtbs.AddTab(nm)
                refreshWindows()
                SbUpdate("Ready")

        let doopen(ifn:string,dotree:bool) = 
            let dofun() =
                let ndlg = new OpenFileDialog(Title="Open Database",Filter="Grampus databases(*.grampus)|*.grampus",InitialDirectory=bfol)
                if ifn="" && ndlg.ShowDialog() = DialogResult.OK then
                    //open database
                    let gmpfile = ndlg.FileName
                    SbUpdate("Opening base: " + gmpfile)
                    let gmp = Grampus.Load(gmpfile)
                    Recents.addrec gmpfile
                    let nm = Path.Combine(Path.GetDirectoryName(gmpfile),Path.GetFileNameWithoutExtension(gmpfile))
                    if gmp.TreesCreated.IsSome&&dotree then 
                        sts.Init(nm)
                        Recents.addtr gmpfile
                    //dotbselect will be called to do the loading
                    gmtbs.AddTab(nm)
                    SbUpdate("Ready")
                elif ifn<>"" then
                    //open database
                    let nm = Path.GetFileNameWithoutExtension(ifn)
                    SbUpdate("Opening base: " + ifn)
                    let gmp = Grampus.Load(ifn)
                    let nm = Path.Combine(Path.GetDirectoryName(ifn),Path.GetFileNameWithoutExtension(ifn))
                    if gmp.TreesCreated.IsSome&&dotree then 
                        sts.Init(nm)
                        Recents.addtr ifn
                    //dotbselect will be called to do the loading
                    gmtbs.AddTab(nm)
                    SbUpdate("Ready")
            waitify(dofun)

        let dosave() =
            SbUpdate("Saving game")
            let gnum0 = pgn.GetGameNumber()
            pgn.SaveGame()
            let gnum = pgn.GetGameNumber()
            //need to reload gms and select the right row
            SbUpdate("Reloading list of games")
            let nbd = bd.GetBoard()
            gmtbs.Refrsh(nbd)
            gmtbs.SelNum(gnum)
            SbUpdate("Ready")

        let doclose() = 
            SbUpdate("Closing base")
            //offer to save game if has changed
            if saveb.Enabled then
                pgn.PromptSaveGame()
            //clear tree if holds current base
            if sts.BaseName()="" then //cb then
                SbUpdate("Closing tree")
                sts.Close()
            //now close tab
            //assume this will switch tabs and then call dotbselect below?
            SbUpdate("Closing list of games")
            gmtbs.Close()
            SbUpdate("Ready")
            
        let doexit() =
            //offer to save game if has changed
            if saveb.Enabled then
                pgn.PromptSaveGame()
            this.Close()

        let donewg() =
            SbUpdate("Creating game")
            //clear pgn and set gnum to -1
            let nm = gmtbs.BaseName()
            pgn.NewGame(nm)
            SbUpdate("Ready")
 
        let docompact() =
            SbUpdate("Compacting base")
            //if ScincFuncs.Compact.Games()=0 then
            //    SbUpdate("Reloading list of games")
            //    gmtbs.Refrsh(bd.GetBoard(),sts.BaseNum())
            SbUpdate("Ready")

        let doimppgn() =
            let ndlg = new OpenFileDialog(Title="Import PGN File",Filter="Pgn Files(*.pgn)|*.pgn",InitialDirectory=bfol)
            if ndlg.ShowDialog() = DialogResult.OK then
                let pgn = ndlg.FileName
                SbUpdate("Importing pgn file: " + pgn)
                let mutable num = 0
                let mutable msgs = ""
                ()
                //if ScincFuncs.Base.Import(&num,&msgs,pgn)=0 then
                //    SbUpdate("Reloading list of games")
                //    gmtbs.Refrsh(bd.GetBoard(),sts.BaseNum())
                //    if ScincFuncs.Base.Current()=sts.BaseNum() then 
                //        SbUpdate("Reloading tree")
                //        sts.Refrsh()
            SbUpdate("Ready")


        let doeco() =
            SbUpdate("Adding ECO classifiers")
            //let mutable msgs = ""
            //if ScincFuncs.Eco.Base(&msgs)=0 then
            //    SbUpdate("Reloading list of games")
            //    gmtbs.Refrsh(bd.GetBoard(),sts.BaseNum())
            //else
            //    MessageBox.Show("Process had issues: " + msgs,"Set ECO Issues")|>ignore
            SbUpdate("Ready")

        let docopypgn() =
            Clipboard.SetText(pgn.GetPgn())

        let dopastepgn() =
            let pgnstr = Clipboard.GetText()
            try
                SbUpdate("Pasting game")
                pgn.SetPgn(pgnstr)
                SbUpdate("Reloading list of games")
                let nbd = FsChess.Board.Start
                gmtbs.Refrsh(nbd)
                bd.SetBoard(nbd)
                SbUpdate("Reloading tree")
                sts.UpdateStr(nbd)
                anl.SetBoard(nbd)
                SbUpdate("Ready")
            with
                |_ -> 
                    MessageBox.Show("Invalid PGN in Clipboard!", "Paste PGN")|>ignore
                    SbUpdate("Ready")
        
        let doupdatewhite() =
            SbUpdate("Updating white repertoire")
            let numerrs = FsChess.Repertoire.UpdateWhite()
            if numerrs<>0 then
                MessageBox.Show("Errors found iduring conversion. Please review contents of: " + FsChess.Repertoire.WhiteErrFile(),"Repertoire Errors")|>ignore
            SbUpdate("Ready")
        
        let doshowwhite() =
            showwm.Text<-if showwm.Text="Show White" then "Hide White" else "Show White"
            showwb.Text<-if showwb.Text="Show White" then "Hide White" else "Show White"
            sts.LoadWhiteRep(showwm.Text="Hide White")

        let doupdateblack() =
            SbUpdate("Updating black repertoire")
            let numerrs = FsChess.Repertoire.UpdateBlack()
            if numerrs<>0 then
                MessageBox.Show("Errors found iduring conversion. Please review contents of: " + FsChess.Repertoire.BlackErrFile(),"Repertoire Errors")|>ignore
            SbUpdate("Ready")
        
        let doshowblack() =
            showbm.Text<-if showbm.Text="Show Black" then "Hide Black" else "Show Black"
            showbb.Text<-if showbb.Text="Show Black" then "Hide Black" else "Show Black"
            sts.LoadBlackRep(showbm.Text="Hide Black")
        
        
        let dobdchg(nbd) =
            bd.SetBoard(nbd)
            let dofun() =
                SbUpdate("Reloading tree")
                sts.UpdateStr(nbd)
                SbUpdate("Reloading list of games")
                gmtbs.Refrsh(nbd)
                anl.SetBoard(nbd)
            waitify(dofun)
            SbUpdate("Ready")

        let dogmchg(ischg) =
            //set save menus
            saveb.Enabled<-ischg
            savem.Enabled<-ischg

        let dohdrchg(ischg) =
            //set save menus
            saveb.Enabled<-ischg
            savem.Enabled<-ischg

        let domvsel(mvstr) =
            let dofun() =
                let board = bd.GetBoard()
                let mv = mvstr|>FsChess.Move.FromSan board
                bd.DoMove(mvstr)
                pgn.DoMove(mv)
                let nbd = bd.GetBoard()
                anl.SetBoard(nbd)
                SbUpdate("Reloading tree")
                sts.UpdateStr(nbd)
                SbUpdate("Reloading list of games")
                gmtbs.Refrsh(nbd)
            waitify(dofun)
            SbUpdate("Ready")

        let domvmade(mv) =
            let dofun() =
                pgn.DoMove(mv)
                let nbd = bd.GetBoard()
                anl.SetBoard(nbd)
                SbUpdate("Reloading tree")
                sts.UpdateStr(nbd)
                SbUpdate("Reloading list of games")
                gmtbs.Refrsh(nbd)
            waitify(dofun)
            SbUpdate("Ready")

        let dogmsel(rw) =
             pgn.SwitchGame(rw)
             SbUpdate("Ready")

        let dotbselect(e:TabControlEventArgs) =
            let dofun() =
                //need to set current
                let basename = gmtbs.BaseName()
                pgn.Refrsh(0,basename)
                let nbd = FsChess.Board.Start
                bd.SetBoard(nbd)
                SbUpdate("Reloading tree")
                sts.UpdateStr(nbd)
                SbUpdate("Reloading list of games")
                gmtbs.Refrsh(nbd)
                anl.SetBoard(nbd)
                refreshWindows()
            waitify(dofun)
            SbUpdate("Ready")

        let createts() = 
            // new
            let newb = new ToolStripButton(Image = img "new.png", ImageTransparentColor = Color.Magenta, DisplayStyle = ToolStripItemDisplayStyle.Image, Text = "&New")
            newb.Click.Add(fun _ -> donew())
            ts.Items.Add(newb)|>ignore
            // open
            let openb = new ToolStripButton(Image = img "opn.png", ImageTransparentColor = Color.Magenta, DisplayStyle = ToolStripItemDisplayStyle.Image, Text = "&Open")
            openb.Click.Add(fun _ -> doopen("",false))
            ts.Items.Add(openb)|>ignore
            // close
            closeb.Click.Add(fun _ -> doclose())
            ts.Items.Add(closeb)|>ignore
            // save
            saveb.Click.Add(fun _ -> dosave())
            ts.Items.Add(saveb)|>ignore
            let split = new ToolStripSeparator()
            ts.Items.Add(split)|>ignore
            // flip
            let orib = new ToolStripButton(Image = img "orient.png", ImageTransparentColor = Color.Magenta, DisplayStyle = ToolStripItemDisplayStyle.Image, Text = "&Flip")
            orib.Click.Add(fun _ -> bd.Orient())
            ts.Items.Add(orib)|>ignore
            ts.Items.Add(split)|>ignore
            //show white
            showwb.Click.Add(fun _ -> doshowwhite())
            ts.Items.Add(showwb)|>ignore
            //show black
            showbb.Click.Add(fun _ -> doshowblack())
            ts.Items.Add(showbb)|>ignore

        let createms() = 
            // file menu
            let filem = new ToolStripMenuItem(Text = "&File")
            // file new
            let newm = new ToolStripMenuItem(Image = img "new.png", ImageTransparentColor = Color.Magenta, ShortcutKeys = (Keys.Control|||Keys.N), Text = "&New")
            newm.Click.Add(fun _ -> donew())
            filem.DropDownItems.Add(newm)|>ignore
            // file open
            let openm = new ToolStripMenuItem(Image = img "opn.png", ImageTransparentColor = Color.Magenta, ShortcutKeys = (Keys.Control|||Keys.O), Text = "&Open")
            openm.Click.Add(fun _ -> doopen("",false))
            filem.DropDownItems.Add(openm)|>ignore
            // file close
            closem.Click.Add(fun _ -> doclose())
            filem.DropDownItems.Add(closem)|>ignore
            // file open as tree
            let opentreem = new ToolStripMenuItem(Text = "Open as &Tree")
            opentreem.Click.Add(fun _ -> doopen("",true))
            filem.DropDownItems.Add(opentreem)|>ignore
            // recents
            let recm = new ToolStripMenuItem(Text = "Recent")
            let rectreem = new ToolStripMenuItem(Text = "Recent as Tree")
            filem.DropDownItems.Add(recm)|>ignore
            filem.DropDownItems.Add(rectreem)|>ignore
            let addrec (rc:string) =
                let mn = new ToolStripMenuItem(Text = Path.GetFileNameWithoutExtension(rc))
                mn.Click.Add(fun _ -> doopen(rc,false))
                recm.DropDownItems.Add(mn)|>ignore
            let rcs = 
                Recents.getrecs()
                Recents.dbs
            rcs|>Seq.iter addrec
            let addtr (tr:string) =
                let mn1 = new ToolStripMenuItem(Text = Path.GetFileNameWithoutExtension(tr))
                mn1.Click.Add(fun _ -> doopen(tr,true))
                rectreem.DropDownItems.Add(mn1)|>ignore
            let trs = 
                Recents.gettrs()
                Recents.trs
            trs|>Seq.iter addtr

            // file exit
            let exitm = new ToolStripMenuItem(Text = "Exit")
            exitm.Click.Add(fun _ -> doexit())
            filem.DropDownItems.Add(exitm)|>ignore
            
            // game menu
            let gamem = new ToolStripMenuItem(Text = "&Game")
            // game new
            let newgm = new ToolStripMenuItem(Text = "New")
            newgm.Click.Add(fun _ -> donewg())
            gamem.DropDownItems.Add(newgm)|>ignore
            // game save
            savem.Click.Add(fun _ -> dosave())
            gamem.DropDownItems.Add(savem)|>ignore
            // game copy PGN
            let copypm = new ToolStripMenuItem(Text = "Copy PGN")
            copypm.Click.Add(fun _ -> docopypgn())
            gamem.DropDownItems.Add(copypm)|>ignore
            // game copy PGN
            let pastepm = new ToolStripMenuItem(Text = "Paste PGN")
            pastepm.Click.Add(fun _ -> dopastepgn())
            gamem.DropDownItems.Add(pastepm)|>ignore
            // game edit headers
            let edithm = new ToolStripMenuItem(Text = "Edit Headers")
            edithm.Click.Add(fun _ -> pgn.EditHeaders())
            gamem.DropDownItems.Add(edithm)|>ignore
            // game set eco
            let setem = new ToolStripMenuItem(Text = "Set ECO")
            setem.Click.Add(fun _ -> pgn.SetECO())
            gamem.DropDownItems.Add(setem)|>ignore

            // rep menu
            let repm = new ToolStripMenuItem(Text = "&Repertoire")
            // update white repertoire
            let updwm = new ToolStripMenuItem(Text = "Update White")
            updwm.Click.Add(fun _ -> doupdatewhite())
            repm.DropDownItems.Add(updwm)|>ignore
            // show white repertoire
            showwm.Click.Add(fun _ -> doshowwhite())
            repm.DropDownItems.Add(showwm)|>ignore
            // update black repertoire
            let updbm = new ToolStripMenuItem(Text = "Update Black")
            updbm.Click.Add(fun _ -> doupdateblack())
            repm.DropDownItems.Add(updbm)|>ignore
            // show black repertoire
            showbm.Click.Add(fun _ -> doshowblack())
            repm.DropDownItems.Add(showbm)|>ignore



            // tools menu
            let toolsm = new ToolStripMenuItem(Text = "&Tools")
            // tools compact
            cmpm.Click.Add(fun _ -> docompact())
            toolsm.DropDownItems.Add(cmpm)|>ignore
            // tools import pgn file
            impm.Click.Add(fun _ -> doimppgn())
            toolsm.DropDownItems.Add(impm)|>ignore
            // tools set eco
            ecom.Click.Add(fun _ -> doeco())
            toolsm.DropDownItems.Add(ecom)|>ignore


            // about menu
            let abtm = new ToolStripMenuItem("About")
            // docs
            let onl = new ToolStripMenuItem("Online Documentation")
            onl.Click.Add
                (fun _ -> 
                System.Diagnostics.Process.Start
                    (new System.Diagnostics.ProcessStartInfo("https://pbbwfc.github.io/ScincNet/", UseShellExecute = true)) |> ignore)
            abtm.DropDownItems.Add(onl) |> ignore
            // source code
            let src = new ToolStripMenuItem("Source Code")
            src.Click.Add
                (fun _ -> 
                System.Diagnostics.Process.Start
                    (new System.Diagnostics.ProcessStartInfo("https://github.com/pbbwfc/ScincNet", UseShellExecute = true)) |> ignore)
            abtm.DropDownItems.Add(src) |> ignore
            
          
            ms.Items.Add(filem)|>ignore
            ms.Items.Add(gamem)|>ignore
            ms.Items.Add(repm)|>ignore
            ms.Items.Add(toolsm)|>ignore
            ms.Items.Add(abtm)|>ignore

        let bgpnl = new Panel(Dock=DockStyle.Fill,BorderStyle=BorderStyle.Fixed3D)
        let lfpnl = new Panel(Dock=DockStyle.Left,BorderStyle=BorderStyle.Fixed3D,Width=400)
        let rtpnl = new Panel(Dock=DockStyle.Fill,BorderStyle=BorderStyle.Fixed3D)
        let lftpnl = new Panel(Dock=DockStyle.Top,BorderStyle=BorderStyle.Fixed3D,Height=400)
        let lfbpnl = new Panel(Dock=DockStyle.Fill,BorderStyle=BorderStyle.Fixed3D)
        let rttpnl = new Panel(Dock=DockStyle.Top,BorderStyle=BorderStyle.Fixed3D,Height=350)
        let rtmpnl = new Panel(Dock=DockStyle.Top,BorderStyle=BorderStyle.Fixed3D,Height=100)
        let rtbpnl = new Panel(Dock=DockStyle.Fill,BorderStyle=BorderStyle.Fixed3D)
 
        do
            //ScincFuncs.Eco.Read("scid.eco")|>ignore
            gmtbs|>rtbpnl.Controls.Add
            rtbpnl|>rtpnl.Controls.Add
            anl|>rtmpnl.Controls.Add
            rtmpnl|>rtpnl.Controls.Add
            sts|>rttpnl.Controls.Add
            rttpnl|>rtpnl.Controls.Add
            rtpnl|>bgpnl.Controls.Add
            pgn|>lfbpnl.Controls.Add
            lfbpnl|>lfpnl.Controls.Add
            bd|>lftpnl.Controls.Add
            lftpnl|>lfpnl.Controls.Add
            lfpnl|>bgpnl.Controls.Add
            bgpnl|>this.Controls.Add
            createts()
            ts|>this.Controls.Add
            createms()
            ms|>this.Controls.Add
            sl|>ss.Items.Add|>ignore
            ss|>this.Controls.Add
            //Events
            pgn.BdChng  |> Observable.add dobdchg 
            pgn.GmChng |> Observable.add dogmchg
            pgn.HdrChng |> Observable.add dohdrchg
            sts.MvSel |> Observable.add domvsel
            bd.MvMade |> Observable.add domvmade
            gmtbs.GmSel |> Observable.add dogmsel
            gmtbs.GmCmp |> Observable.add (fun _ -> docompact())
            gmtbs.Selected |>Observable.add dotbselect

   