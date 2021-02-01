namespace GrampusBatch

open System
open System.IO
open System.Drawing
open System.Windows.Forms
open GrampusWinForms
open Grampus

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
        
        let mutable bd = Board.Start
        let mutable gmpfile = Path.Combine(bfol, "Dummy.grampus")
        let mutable gmp = GrampusDataEMP
        let mutable binfol = ""
        let setbinfol() =
            binfol <- Path.Combine
                          (Path.GetDirectoryName(gmpfile), 
                           Path.GetFileNameWithoutExtension(gmpfile) + "_FILES")
        let mutable iea = [||]
        let mutable hdra = [||]
        let sts = new WbStats(Dock = DockStyle.Fill)
        let ts =
            new ToolStrip(LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow, 
                          Dock = DockStyle.Fill)
        let impbtn = new ToolStripButton(Text = "Import PGN")
        let crbtn = new ToolStripButton(Text = "Create Trees")
        let vwbtn = new ToolStripButton(Text = "View Trees/Filters")
        let crfbtn = new ToolStripButton(Text = "Create Filters")
        let plylbl = new ToolStripLabel(Text = "Select Ply (-1 infinite)")
        let plydd = new ToolStripComboBox(AutoSize = false, Width = 40)
        let logtb =
            new TextBox(Text = "Log:", Multiline = true, Dock = DockStyle.Fill, 
                        ReadOnly = true, ScrollBars = ScrollBars.Vertical)
        let gmstb =
            new TextBox(Text = "Games:", Dock = DockStyle.Fill, ReadOnly = true)
        let ss =
            new StatusStrip(LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow, 
                            Dock = DockStyle.Fill)
        let prg = new ToolStripProgressBar(Width = 200)
        let lbl = new ToolStripLabel(Text = "Ready", Width = 200)
        let mutable st = DateTime.Now
        let mutable nd = DateTime.Now
        let nl = Environment.NewLine
        let el() = (float ((nd - st).TotalMilliseconds) / 1000.0).ToString()
        
        let log (msg) =
            nd <- DateTime.Now
            logtb.Text <- logtb.Text + nl + msg + " in " + el() + " seconds"
            st <- DateTime.Now
        
        let showfilt (bd) =
            let bdstr = bd |> Board.ToSimpleStr
            let filt = Filter.Read(bdstr, binfol)
            let ln = filt.Length
            
            let lim =
                if ln > 20 then 20
                else ln
            
            let filtstr =
                filt.[..lim]
                |> List.map (fun i -> i.ToString())
                |> List.reduce (fun a b -> a + "," + b)
            
            gmstb.Text <- "Games: " + filtstr
        
        let domvsel (mvstr) =
            let mv = mvstr |> Move.FromSan bd
            bd <- bd |> Board.Push mv
            sts.UpdateStr(bd)
            showfilt (bd)
        
        let doimp (e) =
            let ndlg =
                new OpenFileDialog(Title = "Open PGN files", 
                                   Filter = "PGN files(*.pgn)|*.pgn", 
                                   InitialDirectory = bfol)
            if ndlg.ShowDialog() = DialogResult.OK then 
                this.Enabled <- false
                let pgn = ndlg.FileName
                let nm = Path.GetFileNameWithoutExtension(pgn)
                gmpfile <- Path.Combine(bfol, nm + ".grampus")
                setbinfol()
                lbl.Text <- "Importing games..."
                Application.DoEvents()
                st <- DateTime.Now
                let ugma = PgnGames.ReadSeqFromFile pgn
                log ("Imported games")
                lbl.Text <- "Encoding games..."
                let egma = ugma |> Seq.map (Game.Encode)
                log ("Encoded games")
                lbl.Text <- "Saving games..."
                Games.Save binfol egma
                gmp <- { GrampusDataEMP with SourcePgn = pgn
                                             BaseCreated = Some(DateTime.Now) }
                Grampus.Save(gmpfile, gmp)
                log ("Saved games")
                lbl.Text <- "Ready"
                this.Enabled <- true
        
        let docreate (e) =
            let totaldict =
                new System.Collections.Generic.Dictionary<string, MvTrees>()
            
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
                gmp <- Grampus.Load gmpfile
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
                lbl.Text <- "Initializing View..."
                Application.DoEvents()
                sts.Init
                    (Path.Combine
                         (Path.GetDirectoryName(gmpfile), 
                          Path.GetFileNameWithoutExtension(gmpfile)))
                log ("Initialized View")
                lbl.Text <- "Loading View..."
                Application.DoEvents()
                sts.Refrsh()
                log ("Loaded View")
                lbl.Text <- "Ready"
                Application.DoEvents()
                gmp <- { gmp with TreesCreated = Some(DateTime.Now)
                                  TreesPly = plydd.SelectedItem |> unbox }
                Grampus.Save(gmpfile, gmp)
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
                gmp <- Grampus.Load gmpfile
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
                lbl.Text <- "Initializing View..."
                Application.DoEvents()
                sts.Init
                    (Path.Combine
                         (Path.GetDirectoryName(gmpfile), 
                          Path.GetFileNameWithoutExtension(gmpfile)))
                log ("Initialized View")
                lbl.Text <- "Loading View..."
                Application.DoEvents()
                sts.Refrsh()
                log ("Loaded View")
                lbl.Text <- "Ready"
                Application.DoEvents()
                gmp <- { gmp with FiltersCreated = Some(DateTime.Now)
                                  FiltersPly = plydd.SelectedItem |> unbox }
                Grampus.Save(gmpfile, gmp)
                this.Enabled <- true
        
        let dovw (e) =
            let ndlg =
                new OpenFileDialog(Title = "Open Database", 
                                   Filter = "Grampus databases(*.grampus)|*.grampus", 
                                   InitialDirectory = bfol)
            if ndlg.ShowDialog() = DialogResult.OK then 
                gmpfile <- ndlg.FileName
                setbinfol()
                sts.Init
                    (Path.Combine
                         (Path.GetDirectoryName(gmpfile), 
                          Path.GetFileNameWithoutExtension(gmpfile)))
                sts.Refrsh()
        
        let bgpnl =
            new Panel(Dock = DockStyle.Fill, BorderStyle = BorderStyle.Fixed3D)
        let btmpnl =
            new Panel(Dock = DockStyle.Bottom, BorderStyle = BorderStyle.Fixed3D, 
                      Height = 30)
        let tppnl =
            new Panel(Dock = DockStyle.Top, BorderStyle = BorderStyle.Fixed3D, 
                      Height = 30)
        let gmspnl =
            new Panel(Dock = DockStyle.Top, BorderStyle = BorderStyle.Fixed3D, 
                      Height = 30)
        let logpnl =
            new Panel(Dock = DockStyle.Top, BorderStyle = BorderStyle.Fixed3D, 
                      Height = 190)
        do 
            bgpnl.Controls.Add(sts)
            this.Controls.Add(bgpnl)
            ss.Items.Add(lbl) |> ignore
            ss.Items.Add(prg) |> ignore
            btmpnl.Controls.Add(ss)
            this.Controls.Add(btmpnl)
            gmspnl.Controls.Add(gmstb)
            this.Controls.Add(gmspnl)
            logpnl.Controls.Add(logtb)
            this.Controls.Add(logpnl)
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
            ts.Items.Add(vwbtn) |> ignore
            tppnl.Controls.Add(ts)
            this.Controls.Add(tppnl)
            impbtn.Click.Add(doimp)
            crbtn.Click.Add(docreate)
            crfbtn.Click.Add(docreatef)
            vwbtn.Click.Add(dovw)
            sts.MvSel |> Observable.add domvsel
