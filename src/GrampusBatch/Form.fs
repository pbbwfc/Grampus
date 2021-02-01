namespace GrampusBatch

open System
open System.IO
open System.Drawing
open System.Windows.Forms
open Grampus
open Dialogs

type private GameInfo =
    { Gmno : int
      Welo : int
      Belo : int
      Year : int
      Result : int }

type private TreeData =
    { TotElo : int64
      EloCount : int64
      TotPerf : int64
      PerfCount : int64
      TotYear : int64
      YearCount : int64
      TotScore : int64
      DrawCount : int64
      TotCount : int64 }

type private MvTrees = System.Collections.Generic.Dictionary<string, TreeData>

module Form =
    let img nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("GrampusBatch." + nm)
        Image.FromStream(file)
    
    let ico nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("GrampusBatch." + nm)
        new Icon(file)
    
    type FrmMain() as this =
        inherit Form(Text = "Grampus Batch", Icon = ico "batch.ico", Width = 800, 
                     Height = 700, FormBorderStyle = FormBorderStyle.FixedDialog, 
                     MaximizeBox = false, 
                     StartPosition = FormStartPosition.CenterScreen)
        
        let bfol =
            let pth =
                Path.Combine
                    (System.Environment.GetFolderPath
                         (System.Environment.SpecialFolder.MyDocuments), 
                     "Grampus\\bases")
            Directory.CreateDirectory(pth) |> ignore
            pth
        
        let mutable gmpfile = Path.Combine(bfol, "Dummy.grampus")
        let mutable gmp : GrampusData option = None
        let mutable binfol = ""
        let setbinfol() =
            binfol <- Path.Combine
                          (Path.GetDirectoryName(gmpfile), 
                           Path.GetFileNameWithoutExtension(gmpfile) + "_FILES")
        let mutable iea = [||]
        let mutable hdra = [||]
        let ms = new MenuStrip()
        let ts =
            new ToolStrip(LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow, 
                          Dock = DockStyle.Fill)
        let plydd = new ToolStripComboBox(AutoSize = false, Width = 40)
        let logtb =
            new TextBox(Text = "Log:", Multiline = true, Dock = DockStyle.Fill, 
                        ReadOnly = true, ScrollBars = ScrollBars.Vertical)
        let ss =
            new StatusStrip(LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow, 
                            Dock = DockStyle.Fill)
        let prg = new ToolStripProgressBar(Width = 300, AutoSize = false)
        let lbl = new ToolStripLabel(Text = "Ready", Width = 200)
        let mutable st = DateTime.Now
        let mutable nd = DateTime.Now
        let nl = Environment.NewLine
        let el() = (float ((nd - st).TotalMilliseconds) / 1000.0).ToString()
        let impbtn = new ToolStripButton(Text = "Import PGN", Enabled = true)
        let crbtn = new ToolStripButton(Text = "Create Trees", Enabled = false)
        let crfbtn =
            new ToolStripButton(Text = "Create Filters", Enabled = false)
        let opnm = new ToolStripMenuItem(Text = "&Open")
        let clsm = new ToolStripMenuItem(Text = "&Close", Enabled = false)
        let pgnm =
            new ToolStripMenuItem(Text = "From &Pgn File", Enabled = true)
        let crm = new ToolStripMenuItem(Text = "&Create", Enabled = false)
        let crfm = new ToolStripMenuItem(Text = "&Create", Enabled = false)
        let updateTitle() = this.Text <- "Grampus Batch - " + gmpfile
        
        let updateMenuStates() =
            impbtn.Enabled <- gmp.IsNone
            crbtn.Enabled <- gmp.IsSome
            crfbtn.Enabled <- gmp.IsSome
            clsm.Enabled <- gmp.IsSome
            pgnm.Enabled <- gmp.IsNone
            crm.Enabled <- gmp.IsSome
            crfm.Enabled <- gmp.IsSome
        
        let log (msg) =
            nd <- DateTime.Now
            logtb.Text <- logtb.Text + nl + msg + " in " + el() + " seconds"
            st <- DateTime.Now
        
        let updprg (i) =
            prg.Value <- i
            Application.DoEvents()
        
        let doopen (e) =
            let ndlg =
                new OpenFileDialog(Title = "Open Database", 
                                   Filter = "Grampus databases(*.grampus)|*.grampus", 
                                   InitialDirectory = bfol)
            if ndlg.ShowDialog() = DialogResult.OK then 
                gmpfile <- ndlg.FileName
                gmp <- Some(Grampus.Load gmpfile)
                setbinfol()
                iea <- Index.Load binfol
                hdra <- Headers.Load binfol
                updateMenuStates()
                updateTitle()
        
        let doclose (e) =
            gmpfile <- ""
            gmp <- None
            binfol <- ""
            iea <- [||]
            hdra <- [||]
            updateMenuStates()
            updateTitle()
        
        let doimp (e) =
            let ndlg =
                new OpenFileDialog(Title = "Select PGN file", 
                                   Filter = "PGN files(*.pgn)|*.pgn", 
                                   InitialDirectory = bfol)
            if ndlg.ShowDialog() = DialogResult.OK then 
                let pgn = ndlg.FileName
                let nm = Path.GetFileNameWithoutExtension(pgn)
                let fn = nm + ".grampus"
                let sdlg =
                    new SaveFileDialog(Title = "Select Grampus File", 
                                       Filter = "Grampus databases(*.grampus)|*.grampus", 
                                       FileName = fn, AddExtension = true, 
                                       OverwritePrompt = false, 
                                       InitialDirectory = Path.GetDirectoryName
                                                              (pgn))
                if sdlg.ShowDialog() = DialogResult.OK then 
                    gmpfile <- sdlg.FileName
                    this.Enabled <- false
                    setbinfol()
                    lbl.Text <- "Counting games..."
                    Application.DoEvents()
                    st <- DateTime.Now
                    let numgames = PgnGames.GetNumberOfGames pgn
                    prg.Maximum <- numgames
                    prg.Value <- 0
                    log ("Counted games")
                    lbl.Text <- "Games as sequence..."
                    Application.DoEvents()
                    st <- DateTime.Now
                    let ugms = PgnGames.ReadSeqFromFile pgn
                    log ("Games as sequence")
                    lbl.Text <- "Encoding games as sequence..."
                    let egms = ugms |> Seq.map (Game.Encode)
                    log ("Encoded games as sequence")
                    lbl.Text <- "Saving games..."
                    Games.Save binfol egms updprg
                    gmp <- Some
                               ({ GrampusDataEMP with SourcePgn = pgn
                                                      BaseCreated =
                                                          Some(DateTime.Now) })
                    Grampus.Save(gmpfile, gmp.Value)
                    log ("Saved games")
                    lbl.Text <- "Ready"
                    this.Enabled <- true
                    prg.Value <- 0
                    updateTitle()
                    updateMenuStates()
        
        let docreate (e) =
            let totaldict =
                new System.Collections.Generic.Dictionary<string, MvTrees>()
            let dr = (new DlgTree()).ShowDialog()
            
            let getGmBds i =
                let gm = Games.LoadGame binfol iea.[i] hdra.[i]
                let ply = plydd.SelectedItem |> unbox
                let posns, mvs = Game.GetPosnsMoves ply gm
                let welo = gm.Hdr.W_Elo
                let belo = gm.Hdr.B_Elo
                
                let res =
                    gm.Hdr.Result
                    |> Result.Parse
                    |> Result.ToInt
                
                //{ "*",  "1-0",  "0-1",  "1/2-1/2" }
                let yr = gm.Hdr.Year
                
                let gminfo =
                    { Gmno = i
                      Welo =
                          if welo = "-" then 0
                          else int (welo)
                      Belo =
                          if belo = "-" then 0
                          else int (belo)
                      Year = yr
                      Result = res }
                gminfo, posns, mvs
            
            let processGame i =
                let gminfo, posns, mvs = getGmBds i
                
                let wtd =
                    let perf, ct =
                        if gminfo.Belo = 0 then 0, 0
                        elif gminfo.Result = 1 then //draw
                            gminfo.Belo, 1
                        elif gminfo.Result = 2 then //win
                            (gminfo.Belo + 400), 1
                        else (gminfo.Belo - 400), 1
                    { TotElo = int64 (gminfo.Welo)
                      EloCount =
                          (if gminfo.Welo = 0 then 0L
                           else 1L)
                      TotPerf = int64 (perf)
                      PerfCount = int64 (ct)
                      TotYear = int64 (gminfo.Year)
                      YearCount =
                          (if gminfo.Year = 0 then 0L
                           else 1L)
                      TotScore = int64 (gminfo.Result)
                      DrawCount =
                          (if gminfo.Result = 1 then 1L
                           else 0L)
                      TotCount = 1L }
                
                let btd =
                    let perf, ct =
                        if gminfo.Welo = 0 then 0, 0
                        elif gminfo.Result = 1 then //draw
                            gminfo.Welo, 1
                        elif gminfo.Result = 0 then //win
                            (gminfo.Welo + 400), 1
                        else (gminfo.Welo - 400), 1
                    { TotElo = int64 (gminfo.Belo)
                      EloCount =
                          (if gminfo.Belo = 0 then 0L
                           else 1L)
                      TotPerf = int64 (perf)
                      PerfCount = int64 (ct)
                      TotYear = int64 (gminfo.Year)
                      YearCount =
                          (if gminfo.Year = 0 then 0L
                           else 1L)
                      TotScore = int64 (gminfo.Result)
                      DrawCount =
                          (if gminfo.Result = 1 then 1L
                           else 0L)
                      TotCount = 1L }
                
                //now need to go through the boarda and put in dictionary holding running totals
                for j = 0 to posns.Length - 1 do
                    let bd = posns.[j]
                    let mv = mvs.[j]
                    let isw = bd.EndsWith("w")
                    if totaldict.ContainsKey(bd) then 
                        let mvdct : MvTrees = totaldict.[bd]
                        if mvdct.ContainsKey(mv) then 
                            let cmt = mvdct.[mv]
                            
                            let nmt =
                                if isw then wtd
                                else btd
                            mvdct.[mv] <- { TotElo = cmt.TotElo + nmt.TotElo
                                            EloCount =
                                                cmt.EloCount + nmt.EloCount
                                            TotPerf = cmt.TotPerf + nmt.TotPerf
                                            PerfCount =
                                                cmt.PerfCount + nmt.PerfCount
                                            TotYear = cmt.TotYear + nmt.TotYear
                                            YearCount =
                                                cmt.YearCount + nmt.YearCount
                                            TotScore =
                                                cmt.TotScore + nmt.TotScore
                                            DrawCount =
                                                cmt.DrawCount + nmt.DrawCount
                                            TotCount =
                                                cmt.TotCount + nmt.TotCount }
                        else 
                            mvdct.[mv] <- if isw then wtd
                                          else btd
                    else 
                        let mvdct = new MvTrees()
                        mvdct.[mv] <- if isw then wtd
                                      else btd
                        totaldict.[bd] <- mvdct
            
            let processPos i (vl : MvTrees) =
                let sts = new stats()
                let mvsts = new ResizeArray<mvstats>()
                let totsts = new totstats()
                totsts.TotFreq <- 1.0
                for mtr in vl do
                    let tr = mtr.Value
                    totsts.TotCount <- totsts.TotCount + tr.TotCount
                let mutable ect = 0L
                let mutable pct = 0L
                let mutable yct = 0L
                for mtr in vl do
                    let mv = mtr.Key
                    let tr = mtr.Value
                    let mvst = new mvstats()
                    mvst.Count <- tr.TotCount
                    mvst.Freq <- float (mvst.Count) / float (totsts.TotCount)
                    mvst.WhiteWins <- (tr.TotScore - tr.DrawCount) / 2L
                    totsts.TotWhiteWins <- totsts.TotWhiteWins + mvst.WhiteWins
                    mvst.Draws <- tr.DrawCount
                    totsts.TotDraws <- totsts.TotDraws + mvst.Draws
                    mvst.BlackWins <- mvst.Count - mvst.WhiteWins - mvst.Draws
                    totsts.TotBlackWins <- totsts.TotBlackWins + mvst.BlackWins
                    mvst.Score <- float (tr.TotScore) / float (tr.TotCount * 2L)
                    totsts.TotScore <- totsts.TotScore 
                                       + float (tr.TotScore) 
                                         / float (totsts.TotCount * 2L)
                    mvst.DrawPc <- float (tr.DrawCount) / float (tr.TotCount)
                    totsts.TotDrawPc <- totsts.TotDrawPc 
                                        + float (tr.DrawCount) 
                                          / float (totsts.TotCount)
                    mvst.AvElo <- if tr.EloCount <= 10L then 0L
                                  else tr.TotElo / tr.EloCount
                    totsts.TotAvElo <- totsts.TotAvElo + tr.TotElo
                    ect <- ect + tr.EloCount
                    mvst.Perf <- if tr.PerfCount <= 10L then 0L
                                 else tr.TotPerf / tr.PerfCount
                    totsts.TotPerf <- totsts.TotPerf + tr.TotPerf
                    pct <- pct + tr.PerfCount
                    mvst.AvYear <- if tr.YearCount <= 0L then 0L
                                   else tr.TotYear / tr.YearCount
                    totsts.TotAvYear <- totsts.TotAvYear + tr.TotYear
                    yct <- yct + tr.YearCount
                    mvst.Mvstr <- mv
                    mvsts.Add(mvst)
                totsts.TotAvElo <- if ect = 0L then 0L
                                   else totsts.TotAvElo / ect
                totsts.TotPerf <- if pct = 0L then 0L
                                  else totsts.TotPerf / pct
                totsts.TotAvYear <- if yct = 0L then 0L
                                    else totsts.TotAvYear / yct
                //need to sort by count
                mvsts.Sort(fun a b -> int (b.Count - a.Count))
                sts.MvsStats <- mvsts
                sts.TotStats <- totsts
                if i % 100 = 0 then 
                    prg.Value <- i
                    Application.DoEvents()
                sts
            
            let ndlg =
                new OpenFileDialog(Title = "Open Database", 
                                   Filter = "Grampus databases(*.grampus)|*.grampus", 
                                   InitialDirectory = bfol)
            if ndlg.ShowDialog() = DialogResult.OK then 
                gmpfile <- ndlg.FileName
                this.Enabled <- false
                lbl.Text <- "Opening base..."
                Application.DoEvents()
                st <- DateTime.Now
                gmp <- Some(Grampus.Load gmpfile)
                setbinfol()
                iea <- Index.Load binfol
                hdra <- Headers.Load binfol
                let numgames = iea.Length
                totaldict.Clear()
                prg.Minimum <- 0
                prg.Maximum <- numgames
                log ("Opened base")
                lbl.Text <- "Processing " + numgames.ToString() + " games..."
                Application.DoEvents()
                for i = 0 to numgames - 1 do
                    processGame (i)
                    if i % 100 = 0 then 
                        prg.Value <- i
                        Application.DoEvents()
                log ("Processed Games")
                //now create tree for each
                let numpos = totaldict.Count
                prg.Maximum <- numpos
                lbl.Text <- "Creating arrays..."
                Application.DoEvents()
                let mutable posns = Array.zeroCreate numpos
                let mutable mvtrees = Array.zeroCreate numpos
                totaldict.Keys.CopyTo(posns, 0)
                totaldict.Values.CopyTo(mvtrees, 0)
                totaldict.Clear()
                log ("Created Arrays")
                lbl.Text <- "Processing " + numpos.ToString() + " positions..."
                Application.DoEvents()
                let stsarr = mvtrees |> Array.mapi processPos
                prg.Value <- 0
                log ("Processed Positions")
                lbl.Text <- "Creating dictionary..."
                Application.DoEvents()
                Tree.Create(binfol) |> ignore
                log ("Created dictionary")
                lbl.Text <- "Saving dictionary..."
                Application.DoEvents()
                Tree.Save(posns, stsarr, binfol) |> ignore
                log ("Saved dictionary")
                gmp <- Some
                           ({ gmp.Value with TreesCreated = Some(DateTime.Now)
                                             TreesPly =
                                                 plydd.SelectedItem |> unbox })
                Grampus.Save(gmpfile, gmp.Value)
                this.Enabled <- true
        
        let docreatef (e) =
            let totaldict =
                new System.Collections.Generic.Dictionary<string, int list>()
            
            let getBds i =
                let gm = Games.LoadGame binfol iea.[i] hdra.[i]
                let ply = plydd.SelectedItem |> unbox
                let posns = Game.GetPosns ply gm
                posns
            
            let processGame i =
                let posns = getBds i
                //now need to go through the boarda and put in dictionary holding running totals
                for j = 0 to posns.Length - 1 do
                    let bd = posns.[j]
                    if totaldict.ContainsKey(bd) then 
                        let gms : int list = totaldict.[bd]
                        totaldict.[bd] <- (i :: gms)
                    else totaldict.[bd] <- [ i ]
            
            let ndlg =
                new OpenFileDialog(Title = "Open Database", 
                                   Filter = "Grampus databases(*.grampus)|*.grampus", 
                                   InitialDirectory = bfol)
            if ndlg.ShowDialog() = DialogResult.OK then 
                gmpfile <- ndlg.FileName
                this.Enabled <- false
                lbl.Text <- "Opening base..."
                Application.DoEvents()
                st <- DateTime.Now
                gmp <- Some(Grampus.Load gmpfile)
                setbinfol()
                iea <- Index.Load binfol
                let numgames = iea.Length
                totaldict.Clear()
                prg.Minimum <- 0
                prg.Maximum <- numgames
                log ("Opened base")
                lbl.Text <- "Processing " + numgames.ToString() + " games..."
                Application.DoEvents()
                for i = 0 to numgames - 1 do
                    processGame (i)
                    if i % 100 = 0 then 
                        prg.Value <- i
                        Application.DoEvents()
                log ("Processed Games")
                //now create tree for each
                let numpos = totaldict.Count
                prg.Maximum <- numpos
                lbl.Text <- "Creating arrays..."
                Application.DoEvents()
                let mutable posns = Array.zeroCreate numpos
                let mutable gmls = Array.zeroCreate numpos
                totaldict.Keys.CopyTo(posns, 0)
                totaldict.Values.CopyTo(gmls, 0)
                totaldict.Clear()
                log ("Created Arrays")
                lbl.Text <- "Creating dictionary..."
                Application.DoEvents()
                Filter.Create(binfol) |> ignore
                log ("Created dictionary")
                lbl.Text <- "Saving dictionary..."
                Application.DoEvents()
                let rgmls = gmls |> Array.map (fun l -> List.rev l)
                Filter.Save(posns, rgmls, binfol) |> ignore
                log ("Saved dictionary")
                lbl.Text <- "Ready"
                Application.DoEvents()
                gmp <- Some
                           ({ gmp.Value with FiltersCreated = Some(DateTime.Now)
                                             FiltersPly =
                                                 plydd.SelectedItem |> unbox })
                Grampus.Save(gmpfile, gmp.Value)
                this.Enabled <- true
        
        let createts() =
            let plylbl = new ToolStripLabel(Text = "Select Ply (-1 infinite)")
            ts.Items.Add(impbtn) |> ignore
            ts.Items.Add(crbtn) |> ignore
            ts.Items.Add(crfbtn) |> ignore
            [| -1; 20; 30; 40; 50 |]
            |> Array.map box
            |> plydd.Items.AddRange
            plydd.SelectedIndex <- 1
            ts.Items.Add(new ToolStripSeparator()) |> ignore
            ts.Items.Add(plylbl) |> ignore
            ts.Items.Add(plydd) |> ignore
            impbtn.Click.Add(doimp)
            crbtn.Click.Add(docreate)
            crfbtn.Click.Add(docreatef)
        
        let createms() =
            //base menu
            let bm = new ToolStripMenuItem(Text = "&Base")
            bm.DropDownItems.Add(opnm) |> ignore
            opnm.Click.Add(doopen)
            bm.DropDownItems.Add(clsm) |> ignore
            clsm.Click.Add(doclose)
            bm.DropDownItems.Add(pgnm) |> ignore
            pgnm.Click.Add(doimp)
            ms.Items.Add(bm) |> ignore
            //tree menu
            let tm = new ToolStripMenuItem(Text = "&Tree")
            tm.DropDownItems.Add(crm) |> ignore
            crm.Click.Add(docreate)
            ms.Items.Add(tm) |> ignore
            //filter menu
            let fm = new ToolStripMenuItem(Text = "&Filter")
            fm.DropDownItems.Add(crfm) |> ignore
            crfm.Click.Add(docreatef)
            ms.Items.Add(fm) |> ignore
        
        let btmpnl =
            new Panel(Dock = DockStyle.Bottom, BorderStyle = BorderStyle.Fixed3D, 
                      Height = 30)
        let tppnl =
            new Panel(Dock = DockStyle.Top, BorderStyle = BorderStyle.Fixed3D, 
                      Height = 60)
        let logpnl =
            new Panel(Dock = DockStyle.Fill, BorderStyle = BorderStyle.Fixed3D)
        do 
            createts()
            createms()
            updateMenuStates()
            ss.Items.Add(lbl) |> ignore
            ss.Items.Add(prg) |> ignore
            btmpnl.Controls.Add(ss)
            this.Controls.Add(btmpnl)
            logpnl.Controls.Add(logtb)
            this.Controls.Add(logpnl)
            tppnl.Controls.Add(ts)
            tppnl.Controls.Add(ms)
            this.Controls.Add(tppnl)
