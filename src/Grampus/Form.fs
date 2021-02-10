namespace GrampusUI

open System.IO
open System.Drawing
open System.Windows.Forms
open GrampusWinForms
open Grampus

module Form =
    let img nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("Grampus.Images." + nm)
        Image.FromStream(file)
    
    let ico nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("Grampus.Images." + nm)
        new Icon(file)
    
    type FrmMain() as this =
        inherit Form(Text = "Grampus", WindowState = FormWindowState.Maximized, 
                     IsMdiContainer = true, Icon = ico "grampus.ico")
        
        let bfol =
            let pth =
                Path.Combine
                    (System.Environment.GetFolderPath
                         (System.Environment.SpecialFolder.MyDocuments), 
                     "Grampus\\bases")
            Directory.CreateDirectory(pth) |> ignore
            pth
        
        let tfol =
            let pth =
                Path.Combine
                    (System.Environment.GetFolderPath
                         (System.Environment.SpecialFolder.MyDocuments), 
                     "Grampus\\trees")
            Directory.CreateDirectory(pth) |> ignore
            pth
        
        let bd = new PnlBoard(Dock = DockStyle.Fill)
        let pgn = new PnlPgn(Dock = DockStyle.Fill)
        let sts = new WbStats(Dock = DockStyle.Fill)
        let gmtbs = new TcGames(Dock = DockStyle.Fill)
        let anl = new TcAnl(Dock = DockStyle.Fill)
        let ts = new ToolStrip(GripStyle = ToolStripGripStyle.Hidden)
        let ms = new MenuStrip()
        let saveb =
            new ToolStripButton(Image = img "sav.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "&Save", Enabled = false)
        let savem =
            new ToolStripMenuItem(Image = img "sav.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  ShortcutKeys = (Keys.Control ||| Keys.S), 
                                  Text = "&Save", Enabled = false)
        let closeb =
            new ToolStripButton(Image = img "cls.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "&Close", Enabled = false)
        let closem =
            new ToolStripMenuItem(Image = img "cls.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "&Close", Enabled = false)
        let tcloseb =
            new ToolStripButton(Image = img "tcls.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "&Close Tree", Enabled = false)
        let tclosem =
            new ToolStripMenuItem(Image = img "tcls.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "&Close", Enabled = false)
        let newgm =
            new ToolStripMenuItem(Text = "&New", Image = img "gnew.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Enabled = false)
        let newgb =
            new ToolStripButton(Text = "&New Game", Image = img "gnew.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Enabled = false)
        let copypm =
            new ToolStripMenuItem(Text = "Copy PGN", Image = img "copyp.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Enabled = false)
        let copypb =
            new ToolStripButton(Text = "Copy PGN", Image = img "copyp.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Enabled = false)
        let pastepm =
            new ToolStripMenuItem(Text = "Paste PGN", Image = img "pastep.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Enabled = false)
        let pastepb =
            new ToolStripButton(Text = "Paste PGN", Image = img "pastep.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Enabled = false)
        let edithm =
            new ToolStripMenuItem(Text = "Edit Headers", Image = img "edith.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Enabled = false)
        let edithb =
            new ToolStripButton(Text = "Edit Headers", Image = img "edith.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Enabled = false)
        let setem =
            new ToolStripMenuItem(Text = "Set ECO", Image = img "sete.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Enabled = false)
        let seteb =
            new ToolStripButton(Text = "Set ECO", Image = img "sete.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Enabled = false)
        let remcm =
            new ToolStripMenuItem(Text = "Remove Comments", 
                                  Image = img "remc.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Enabled = false)
        let remcb =
            new ToolStripButton(Text = "Remove Comments", Image = img "remc.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Enabled = false)
        let remvm =
            new ToolStripMenuItem(Text = "Remove Variations", 
                                  Image = img "remv.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Enabled = false)
        let remvb =
            new ToolStripButton(Text = "Remove Variations", 
                                Image = img "remv.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Enabled = false)
        let remnm =
            new ToolStripMenuItem(Text = "Remove NAGs", Image = img "remn.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Enabled = false)
        let remnb =
            new ToolStripButton(Text = "Remove NAGs", Image = img "remn.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Enabled = false)
        let showwb =
            new ToolStripButton(Image = img "white.png", Enabled = false, 
                                Text = "Show White")
        let showwm =
            new ToolStripMenuItem(Image = img "white.png", Text = "Show White", 
                                  CheckState = CheckState.Unchecked, 
                                  Enabled = false)
        let showbb =
            new ToolStripButton(Image = img "black.png", Enabled = false, 
                                Text = "Show Black")
        let showbm =
            new ToolStripMenuItem(Image = img "black.png", Text = "Show Black", 
                                  CheckState = CheckState.Unchecked, 
                                  Enabled = false)
        let ss =
            new StatusStrip(LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow, 
                            Anchor = AnchorStyles.Bottom, Text = "No bases open", 
                            Dock = DockStyle.Bottom)
        let sl = new ToolStripStatusLabel(Text = "Ready")
        
        let SbUpdate(txt) =
            //TODO may add some timing and logging
            sl.Text <- txt
            Application.DoEvents()
        
        let updateMenuStates() =
            closeb.Enabled <- gmtbs.TabCount > 0
            closem.Enabled <- gmtbs.TabCount > 0
            newgb.Enabled <- gmtbs.TabCount > 0
            newgm.Enabled <- gmtbs.TabCount > 0
            copypb.Enabled <- gmtbs.TabCount > 0
            copypm.Enabled <- gmtbs.TabCount > 0
            pastepb.Enabled <- gmtbs.TabCount > 0
            pastepm.Enabled <- gmtbs.TabCount > 0
            edithb.Enabled <- gmtbs.TabCount > 0
            edithm.Enabled <- gmtbs.TabCount > 0
            seteb.Enabled <- gmtbs.TabCount > 0
            setem.Enabled <- gmtbs.TabCount > 0
            remcb.Enabled <- gmtbs.TabCount > 0
            remcm.Enabled <- gmtbs.TabCount > 0
            remvb.Enabled <- gmtbs.TabCount > 0
            remvm.Enabled <- gmtbs.TabCount > 0
            remnb.Enabled <- gmtbs.TabCount > 0
            remnm.Enabled <- gmtbs.TabCount > 0
            showwb.Enabled <- gmtbs.TabCount > 0
            showwm.Enabled <- gmtbs.TabCount > 0
            showbb.Enabled <- gmtbs.TabCount > 0
            showbm.Enabled <- gmtbs.TabCount > 0
        
        let updateTitle() = this.Text <- "Grampus - " + gmtbs.BaseName()
        
        let refreshWindows() =
            updateMenuStates()
            updateTitle()
        
        let waitify f =
            let cu = this.Cursor
            try 
                try 
                    this.Cursor <- Cursors.WaitCursor
                    this.Enabled <- false
                    let st = System.DateTime.Now
                    f()
                    let nd = System.DateTime.Now
                    let el = (nd - st).Seconds
                    ()
                with ex -> 
                    MessageBox.Show(ex.Message, "Process Failed") |> ignore
            finally
                this.Enabled <- true
                this.Cursor <- cu
        
        let donew() =
            let ndlg =
                new SaveFileDialog(Title = "Create New Base", 
                                   Filter = "Grampus databases(*.grampus)|*.grampus", 
                                   AddExtension = true, OverwritePrompt = false, 
                                   InitialDirectory = bfol)
            if ndlg.ShowDialog() = DialogResult.OK then 
                //create database
                let gmpfile = ndlg.FileName
                SbUpdate("Creating base: " + gmpfile)
                let gmp = Grampus.New(gmpfile)
                SbUpdate("Updating windows")
                Recents.addrec gmpfile
                gmtbs.AddTab(gmpfile, gmp.FiltersCreated.IsNone)
                refreshWindows()
                SbUpdate("Ready")
        
        let doopen (ifn : string) =
            let dofun() =
                let ndlg =
                    new OpenFileDialog(Title = "Open Base", 
                                       Filter = "Grampus databases(*.grampus)|*.grampus", 
                                       InitialDirectory = bfol)
                if ifn = "" && ndlg.ShowDialog() = DialogResult.OK then 
                    //open database
                    let gmpfile = ndlg.FileName
                    SbUpdate("Opening base: " + gmpfile)
                    Recents.addrec gmpfile
                    //dotbselect will be called to do the loading
                    let gmp = Grampus.Load(gmpfile)
                    gmtbs.AddTab(gmpfile, gmp.FiltersCreated.IsNone)
                    if gmtbs.TabCount = 1 then 
                        //need to set current
                        let basename = gmtbs.BaseName()
                        pgn.Refrsh(0, basename)
                        let nbd = Board.Start
                        bd.SetBoard(nbd)
                        SbUpdate("Reloading tree")
                        sts.UpdateStr(nbd)
                        SbUpdate("Reloading list of games")
                        gmtbs.Refrsh(nbd)
                        anl.SetBoard(nbd)
                        refreshWindows()
                    SbUpdate("Ready")
                elif ifn <> "" then 
                    //open database
                    let nm = Path.GetFileNameWithoutExtension(ifn)
                    SbUpdate("Opening base: " + ifn)
                    //dotbselect will be called to do the loading
                    let gmp = Grampus.Load(ifn)
                    gmtbs.AddTab(ifn, gmp.FiltersCreated.IsNone)
                    if gmtbs.TabCount = 1 then 
                        //need to set current
                        let basename = gmtbs.BaseName()
                        pgn.Refrsh(0, basename)
                        let nbd = Board.Start
                        bd.SetBoard(nbd)
                        SbUpdate("Reloading tree")
                        sts.UpdateStr(nbd)
                        SbUpdate("Reloading list of games")
                        gmtbs.Refrsh(nbd)
                        anl.SetBoard(nbd)
                        refreshWindows()
                    SbUpdate("Ready")
            waitify (dofun)
        
        let doopentree (ifn : string) =
            let dofun() =
                let ndlg =
                    new OpenFileDialog(Title = "Open Tree", 
                                       Filter = "Grampus databases(*.grampus)|*.grampus", 
                                       InitialDirectory = tfol)
                if ifn = "" && ndlg.ShowDialog() = DialogResult.OK then 
                    //open database
                    let gmpfile = ndlg.FileName
                    SbUpdate("Opening tree: " + gmpfile)
                    let gmp = Grampus.Load(gmpfile)
                    if gmp.TreesCreated.IsSome then 
                        sts.Init(gmpfile)
                        Recents.addtr gmpfile
                        let nbd = bd.GetBoard()
                        sts.UpdateStr(nbd)
                        tclosem.Enabled <- true
                        tcloseb.Enabled <- true
                    SbUpdate("Ready")
                elif ifn <> "" then 
                    //open database
                    let nm = Path.GetFileNameWithoutExtension(ifn)
                    SbUpdate("Opening tree: " + ifn)
                    let gmp = Grampus.Load(ifn)
                    if gmp.TreesCreated.IsSome then 
                        sts.Init(ifn)
                        Recents.addtr ifn
                        let nbd = bd.GetBoard()
                        sts.UpdateStr(nbd)
                        tcloseb.Enabled <- true
                        tclosem.Enabled <- true
                    SbUpdate("Ready")
            waitify (dofun)
        
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
            if saveb.Enabled then pgn.PromptSaveGame()
            //clear tree if holds current base
            if sts.BaseName() = "" then 
                SbUpdate("Closing tree")
                sts.Close()
            //now close tab
            //assume this will switch tabs and then call dotbselect below?
            SbUpdate("Closing list of games")
            gmtbs.Close()
            SbUpdate("Ready")
        
        let doclosetree() =
            SbUpdate("Closing tree")
            sts.Close()
            tcloseb.Enabled <- false
            tclosem.Enabled <- false
            SbUpdate("Ready")
        
        let doexit() =
            //offer to save game if has changed
            if saveb.Enabled then pgn.PromptSaveGame()
            this.Close()
        
        let donewg() =
            SbUpdate("Creating game")
            //clear pgn and set gnum to -1
            let gmpfile = gmtbs.BaseName()
            pgn.NewGame(gmpfile)
            saveb.Enabled <- true
            savem.Enabled <- true
            SbUpdate("Ready")
        
        let docopypgn() = Clipboard.SetText(pgn.GetPgn())
        
        let dopastepgn() =
            let pgnstr = Clipboard.GetText()
            try 
                SbUpdate("Pasting game")
                pgn.SetPgn(pgnstr)
                SbUpdate("Reloading list of games")
                let nbd = Board.Start
                gmtbs.Refrsh(nbd)
                bd.SetBoard(nbd)
                SbUpdate("Reloading tree")
                sts.UpdateStr(nbd)
                anl.SetBoard(nbd)
                SbUpdate("Ready")
            with _ -> 
                MessageBox.Show("Invalid PGN in Clipboard!", "Paste PGN") 
                |> ignore
                SbUpdate("Ready")
        
        let doupdatewhite() =
            SbUpdate("Updating white repertoire")
            let numerrs = Repertoire.UpdateWhite()
            if numerrs <> 0 then 
                MessageBox.Show
                    ("Errors found iduring conversion. Please review contents of: " 
                     + Repertoire.WhiteErrFile(), "Repertoire Errors") |> ignore
            SbUpdate("Ready")
        
        let doshowwhite() =
            showwm.Text <- if showwm.Text = "Show White" then "Hide White"
                           else "Show White"
            showwb.Text <- if showwb.Text = "Show White" then "Hide White"
                           else "Show White"
            sts.LoadWhiteRep(showwm.Text = "Hide White")
        
        let doupdateblack() =
            SbUpdate("Updating black repertoire")
            let numerrs = Repertoire.UpdateBlack()
            if numerrs <> 0 then 
                MessageBox.Show
                    ("Errors found iduring conversion. Please review contents of: " 
                     + Repertoire.BlackErrFile(), "Repertoire Errors") |> ignore
            SbUpdate("Ready")
        
        let doshowblack() =
            showbm.Text <- if showbm.Text = "Show Black" then "Hide Black"
                           else "Show Black"
            showbb.Text <- if showbb.Text = "Show Black" then "Hide Black"
                           else "Show Black"
            sts.LoadBlackRep(showbm.Text = "Hide Black")
        
        let dobdchg (nbd) =
            bd.SetBoard(nbd)
            let dofun() =
                SbUpdate("Reloading tree")
                sts.UpdateStr(nbd)
                SbUpdate("Reloading list of games")
                gmtbs.Refrsh(nbd)
                anl.SetBoard(nbd)
            waitify (dofun)
            SbUpdate("Ready")
        
        let dogmchg (ischg) =
            //set save menus
            saveb.Enabled <- ischg
            savem.Enabled <- ischg
        
        let dohdrchg (ischg) =
            //set save menus
            saveb.Enabled <- ischg
            savem.Enabled <- ischg
        
        let domvsel (mvstr) =
            let dofun() =
                let board = bd.GetBoard()
                let mv = mvstr |> Move.FromSan board
                bd.DoMove(mvstr)
                pgn.DoMove(mv)
                let nbd = bd.GetBoard()
                anl.SetBoard(nbd)
                SbUpdate("Reloading tree")
                sts.UpdateStr(nbd)
                SbUpdate("Reloading list of games")
                gmtbs.Refrsh(nbd)
            waitify (dofun)
            SbUpdate("Ready")
        
        let domvmade (mv) =
            let dofun() =
                pgn.DoMove(mv)
                let nbd = bd.GetBoard()
                anl.SetBoard(nbd)
                SbUpdate("Reloading tree")
                sts.UpdateStr(nbd)
                SbUpdate("Reloading list of games")
                gmtbs.Refrsh(nbd)
            waitify (dofun)
            SbUpdate("Ready")
        
        let dogmsel (rw) =
            pgn.SwitchGame(rw)
            SbUpdate("Ready")
        
        let dotbselect (e : TabControlEventArgs) =
            let dofun() =
                if gmtbs.TabCount > 0 then 
                    //need to set current
                    let basename = gmtbs.BaseName()
                    pgn.Refrsh(0, basename)
                    let nbd = Board.Start
                    bd.SetBoard(nbd)
                    SbUpdate("Reloading tree")
                    sts.UpdateStr(nbd)
                    SbUpdate("Reloading list of games")
                    gmtbs.Refrsh(nbd)
                    anl.SetBoard(nbd)
                    refreshWindows()
            waitify (dofun)
            SbUpdate("Ready")
        
        let createts() =
            // new
            let newb =
                new ToolStripButton(Image = img "new.png", 
                                    ImageTransparentColor = Color.Magenta, 
                                    Text = "&New")
            newb.Click.Add(fun _ -> donew())
            ts.Items.Add(newb) |> ignore
            // open
            let openb =
                new ToolStripButton(Image = img "opn.png", 
                                    ImageTransparentColor = Color.Magenta, 
                                    Text = "&Open")
            openb.Click.Add(fun _ -> doopen (""))
            ts.Items.Add(openb) |> ignore
            // close
            closeb.Click.Add(fun _ -> doclose())
            ts.Items.Add(closeb) |> ignore
            // open tree
            ts.Items.Add(new ToolStripSeparator()) |> ignore
            let topenb =
                new ToolStripButton(Image = img "tree.png", 
                                    ImageTransparentColor = Color.Magenta, 
                                    Text = "&Open Tree")
            topenb.Click.Add(fun _ -> doopentree (""))
            ts.Items.Add(topenb) |> ignore
            tcloseb.Click.Add(fun _ -> doclosetree())
            ts.Items.Add(tcloseb) |> ignore
            // new
            ts.Items.Add(new ToolStripSeparator()) |> ignore
            newgb.Click.Add(fun _ -> donewg())
            ts.Items.Add(newgb) |> ignore
            // save
            saveb.Click.Add(fun _ -> dosave())
            ts.Items.Add(saveb) |> ignore
            // flip
            let orib =
                new ToolStripButton(Image = img "orient.png", 
                                    ImageTransparentColor = Color.Magenta, 
                                    Text = "&Flip")
            orib.Click.Add(fun _ -> bd.Orient())
            ts.Items.Add(orib) |> ignore
            //copy pgn
            copypb.Click.Add(fun _ -> docopypgn())
            ts.Items.Add(copypb) |> ignore
            //paste pgn
            pastepb.Click.Add(fun _ -> dopastepgn())
            ts.Items.Add(pastepb) |> ignore
            //edit headers
            edithb.Click.Add(fun _ -> pgn.EditHeaders())
            ts.Items.Add(edithb) |> ignore
            //set ECO
            seteb.Click.Add(fun _ -> pgn.SetECO())
            ts.Items.Add(seteb) |> ignore
            ts.Items.Add(new ToolStripSeparator()) |> ignore
            // game remove comments
            remcb.Click.Add(fun _ -> pgn.RemoveComments())
            ts.Items.Add(remcb) |> ignore
            // game remove variationss
            remvb.Click.Add(fun _ -> pgn.RemoveRavs())
            ts.Items.Add(remvb) |> ignore
            // game remove nags
            remnb.Click.Add(fun _ -> pgn.RemoveNags())
            ts.Items.Add(remnb) |> ignore
            ts.Items.Add(new ToolStripSeparator()) |> ignore
            //show white
            showwb.Click.Add(fun _ -> doshowwhite())
            ts.Items.Add(showwb) |> ignore
            //show black
            showbb.Click.Add(fun _ -> doshowblack())
            ts.Items.Add(showbb) |> ignore
        
        let createms() =
            // base menu
            let basem = new ToolStripMenuItem(Text = "&Base")
            // file new
            let newm =
                new ToolStripMenuItem(Image = img "new.png", 
                                      ImageTransparentColor = Color.Magenta, 
                                      ShortcutKeys = (Keys.Control ||| Keys.N), 
                                      Text = "&New")
            newm.Click.Add(fun _ -> donew())
            basem.DropDownItems.Add(newm) |> ignore
            // file open
            let openm =
                new ToolStripMenuItem(Image = img "opn.png", 
                                      ImageTransparentColor = Color.Magenta, 
                                      ShortcutKeys = (Keys.Control ||| Keys.O), 
                                      Text = "&Open")
            openm.Click.Add(fun _ -> doopen (""))
            basem.DropDownItems.Add(openm) |> ignore
            // file close
            closem.Click.Add(fun _ -> doclose())
            basem.DropDownItems.Add(closem) |> ignore
            // recents
            let recm = new ToolStripMenuItem(Text = "Recent")
            basem.DropDownItems.Add(recm) |> ignore
            let addrec (rc : string) =
                let mn =
                    new ToolStripMenuItem(Text = Path.GetFileNameWithoutExtension
                                                     (rc))
                mn.Click.Add(fun _ -> doopen (rc))
                recm.DropDownItems.Add(mn) |> ignore
            
            let rcs =
                Recents.getrecs()
                Recents.dbs
            
            rcs |> Seq.iter addrec
            // file exit
            let exitm = new ToolStripMenuItem(Text = "E&xit")
            exitm.Click.Add(fun _ -> doexit())
            basem.DropDownItems.Add(exitm) |> ignore
            // tree menu
            let treem = new ToolStripMenuItem(Text = "&Tree")
            // tree open
            let topenm =
                new ToolStripMenuItem(Image = img "tree.png", 
                                      ImageTransparentColor = Color.Magenta, 
                                      ShortcutKeys = (Keys.Control ||| Keys.O), 
                                      Text = "&Open")
            topenm.Click.Add(fun _ -> doopentree (""))
            treem.DropDownItems.Add(topenm) |> ignore
            // tree close
            tclosem.Click.Add(fun _ -> doclosetree())
            treem.DropDownItems.Add(tclosem) |> ignore
            //recents
            let rectreem = new ToolStripMenuItem(Text = "Recent")
            treem.DropDownItems.Add(rectreem) |> ignore
            let addtr (tr : string) =
                let mn1 =
                    new ToolStripMenuItem(Text = Path.GetFileNameWithoutExtension
                                                     (tr))
                mn1.Click.Add(fun _ -> doopentree (tr))
                rectreem.DropDownItems.Add(mn1) |> ignore
            
            let trs =
                Recents.gettrs()
                Recents.trs
            
            trs |> Seq.iter addtr
            // game menu
            let gamem = new ToolStripMenuItem(Text = "&Game")
            // game new
            newgm.Click.Add(fun _ -> donewg())
            gamem.DropDownItems.Add(newgm) |> ignore
            // game save
            savem.Click.Add(fun _ -> dosave())
            gamem.DropDownItems.Add(savem) |> ignore
            // game new
            let flipm =
                new ToolStripMenuItem(Text = "&Flip Board", 
                                      Image = img "orient.png", 
                                      ImageTransparentColor = Color.Magenta)
            flipm.Click.Add(fun _ -> bd.Orient())
            gamem.DropDownItems.Add(flipm) |> ignore
            gamem.DropDownItems.Add(new ToolStripSeparator()) |> ignore
            // game copy PGN
            copypm.Click.Add(fun _ -> docopypgn())
            gamem.DropDownItems.Add(copypm) |> ignore
            // game copy PGN
            pastepm.Click.Add(fun _ -> dopastepgn())
            gamem.DropDownItems.Add(pastepm) |> ignore
            // game edit headers
            edithm.Click.Add(fun _ -> pgn.EditHeaders())
            gamem.DropDownItems.Add(edithm) |> ignore
            // game set eco
            setem.Click.Add(fun _ -> pgn.SetECO())
            gamem.DropDownItems.Add(setem) |> ignore
            gamem.DropDownItems.Add(new ToolStripSeparator()) |> ignore
            // game remove comments
            remcm.Click.Add(fun _ -> pgn.RemoveComments())
            gamem.DropDownItems.Add(remcm) |> ignore
            // game remove variationss
            remvm.Click.Add(fun _ -> pgn.RemoveRavs())
            gamem.DropDownItems.Add(remvm) |> ignore
            // game remove nags
            remnm.Click.Add(fun _ -> pgn.RemoveNags())
            gamem.DropDownItems.Add(remnm) |> ignore
            // rep menu
            let repm = new ToolStripMenuItem(Text = "&Repertoire")
            // update white repertoire
            let updwm = new ToolStripMenuItem(Text = "Update White")
            updwm.Click.Add(fun _ -> doupdatewhite())
            repm.DropDownItems.Add(updwm) |> ignore
            // show white repertoire
            showwm.Click.Add(fun _ -> doshowwhite())
            repm.DropDownItems.Add(showwm) |> ignore
            // update black repertoire
            let updbm = new ToolStripMenuItem(Text = "Update Black")
            updbm.Click.Add(fun _ -> doupdateblack())
            repm.DropDownItems.Add(updbm) |> ignore
            // show black repertoire
            showbm.Click.Add(fun _ -> doshowblack())
            repm.DropDownItems.Add(showbm) |> ignore
            // about menu
            let abtm = new ToolStripMenuItem("About")
            // docs
            let onl = new ToolStripMenuItem("Online Documentation")
            onl.Click.Add
                (fun _ -> 
                System.Diagnostics.Process.Start
                    (new System.Diagnostics.ProcessStartInfo("https://pbbwfc.github.io/Grampus/", 
                                                             UseShellExecute = true)) 
                |> ignore)
            abtm.DropDownItems.Add(onl) |> ignore
            // source code
            let src = new ToolStripMenuItem("Source Code")
            src.Click.Add
                (fun _ -> 
                System.Diagnostics.Process.Start
                    (new System.Diagnostics.ProcessStartInfo("https://github.com/pbbwfc/Grampus", 
                                                             UseShellExecute = true)) 
                |> ignore)
            abtm.DropDownItems.Add(src) |> ignore
            ms.Items.Add(basem) |> ignore
            ms.Items.Add(treem) |> ignore
            ms.Items.Add(gamem) |> ignore
            ms.Items.Add(repm) |> ignore
            ms.Items.Add(abtm) |> ignore
        
        let bgpnl =
            new Panel(Dock = DockStyle.Fill, BorderStyle = BorderStyle.Fixed3D)
        let lfpnl =
            new Panel(Dock = DockStyle.Left, BorderStyle = BorderStyle.Fixed3D, 
                      Width = 400)
        let rtpnl =
            new Panel(Dock = DockStyle.Fill, BorderStyle = BorderStyle.Fixed3D)
        let lftpnl =
            new Panel(Dock = DockStyle.Top, BorderStyle = BorderStyle.Fixed3D, 
                      Height = 400)
        let lfbpnl =
            new Panel(Dock = DockStyle.Fill, BorderStyle = BorderStyle.Fixed3D)
        let rttpnl =
            new Panel(Dock = DockStyle.Top, BorderStyle = BorderStyle.Fixed3D, 
                      Height = 350)
        let rtmpnl =
            new Panel(Dock = DockStyle.Top, BorderStyle = BorderStyle.Fixed3D, 
                      Height = 100)
        let rtbpnl =
            new Panel(Dock = DockStyle.Fill, BorderStyle = BorderStyle.Fixed3D)
        do 
            //ScincFuncs.Eco.Read("scid.eco")|>ignore
            gmtbs |> rtbpnl.Controls.Add
            rtbpnl |> rtpnl.Controls.Add
            anl |> rtmpnl.Controls.Add
            rtmpnl |> rtpnl.Controls.Add
            sts |> rttpnl.Controls.Add
            rttpnl |> rtpnl.Controls.Add
            rtpnl |> bgpnl.Controls.Add
            pgn |> lfbpnl.Controls.Add
            lfbpnl |> lfpnl.Controls.Add
            bd |> lftpnl.Controls.Add
            lftpnl |> lfpnl.Controls.Add
            lfpnl |> bgpnl.Controls.Add
            bgpnl |> this.Controls.Add
            createts()
            ts |> this.Controls.Add
            createms()
            ms |> this.Controls.Add
            sl
            |> ss.Items.Add
            |> ignore
            ss |> this.Controls.Add
            //Events
            pgn.BdChng |> Observable.add dobdchg
            pgn.GmChng |> Observable.add dogmchg
            pgn.HdrChng |> Observable.add dohdrchg
            sts.MvSel |> Observable.add domvsel
            bd.MvMade |> Observable.add domvmade
            gmtbs.GmSel |> Observable.add dogmsel
            gmtbs.Selected |> Observable.add dotbselect
