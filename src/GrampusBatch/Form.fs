namespace GrampusBatch

open System
open System.IO
open System.Drawing
open System.Windows.Forms
open Grampus
open Dialogs
open FSharp.Collections.ParallelSeq

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
        let file =
            thisExe.GetManifestResourceStream("GrampusBatch.Images." + nm)
        Image.FromStream(file)
    
    let ico nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file =
            thisExe.GetManifestResourceStream("GrampusBatch.Images." + nm)
        new Icon(file)
    
    type FrmMain() as this =
        inherit Form(Text = "Grampus Batch", Icon = ico "batch.ico", Width = 700, 
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
        let mutable ply = 0
        let ms = new MenuStrip()
        let ts =
            new ToolStrip(LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow, 
                          Dock = DockStyle.Top)
        let ts2 =
            new ToolStrip(LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow, 
                          Dock = DockStyle.Fill)
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
        let el() =
            (float ((nd - st).TotalMilliseconds) / 1000.0).ToString("0.##")
        let clsm =
            new ToolStripMenuItem(Image = img "cls.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "&Close", Enabled = false)
        let clsb =
            new ToolStripButton(Image = img "cls.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "&Close", Enabled = false)
        let delm = new ToolStripMenuItem(Text = "&Delete", Enabled = false)
        let cpym =
            new ToolStripMenuItem(Image = img "copyp.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "Copy", Enabled = false)
        let cpyb =
            new ToolStripButton(Image = img "copyp.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "Copy", Enabled = false)
        let infm =
            new ToolStripMenuItem(Image = img "info.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "&Info", Enabled = false)
        let infb =
            new ToolStripButton(Image = img "info.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "&Info", Enabled = false)
        let addm =
            new ToolStripMenuItem(Image = img "add.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "&Add Base", Enabled = false)
        let addb =
            new ToolStripButton(Image = img "add.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "&Add Base", Enabled = false)
        let crm =
            new ToolStripMenuItem(Image = img "tree.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "&Create", Enabled = false)
        let crbtn =
            new ToolStripButton(Image = img "tree.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "&Create Tree", Enabled = false)
        let deltm = new ToolStripMenuItem(Text = "&Delete", Enabled = false)
        let crfm =
            new ToolStripMenuItem(Image = img "crf.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "&Create", Enabled = false)
        let crfbtn =
            new ToolStripButton(Image = img "crf.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "Create Filters", Enabled = false)
        let deltfm = new ToolStripMenuItem(Text = "&Delete", Enabled = false)
        let deltgfm =
            new ToolStripMenuItem(Text = "Delete &Games and Filters", 
                                  Enabled = false)
        let enewm = new ToolStripMenuItem(Text = "&Newest", Enabled = false)
        let estrm = new ToolStripMenuItem(Text = "&Strongest", Enabled = false)
        let eplym = new ToolStripMenuItem(Text = "For &Player", Enabled = false)
        let pgnm =
            new ToolStripMenuItem(Text = "From &Pgn File", Enabled = true)
        let addpm =
            new ToolStripMenuItem(Text = "Add PGN file", Enabled = false)
        let topm =
            new ToolStripMenuItem(Text = "Export to PGN file", Enabled = false)
        let cmpm =
            new ToolStripMenuItem(Image = img "cmp.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "Compact Base", Enabled = false)
        let cmpb =
            new ToolStripButton(Image = img "cmp.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "Compact Base", Enabled = false)
        let remcm =
            new ToolStripMenuItem(Image = img "remc.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "Remove Comments", Enabled = false)
        let remcb =
            new ToolStripButton(Image = img "remc.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "Remove Comments", Enabled = false)
        let remvm =
            new ToolStripMenuItem(Image = img "remv.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "Remove Variations", Enabled = false)
        let remvb =
            new ToolStripButton(Image = img "remv.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "Remove Variations", Enabled = false)
        let remnm =
            new ToolStripMenuItem(Image = img "remn.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "Remove NAGs", Enabled = false)
        let remnb =
            new ToolStripButton(Image = img "remn.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "Remove NAGs", Enabled = false)
        let remdm =
            new ToolStripMenuItem(Image = img "remd.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "Remove Duplicates", Enabled = false)
        let remdb =
            new ToolStripButton(Image = img "remd.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "Remove Duplicates", Enabled = false)
        let ecom =
            new ToolStripMenuItem(Image = img "sete.png", 
                                  ImageTransparentColor = Color.Magenta, 
                                  Text = "Set ECOs", Enabled = false)
        let ecob =
            new ToolStripButton(Image = img "sete.png", 
                                ImageTransparentColor = Color.Magenta, 
                                Text = "Set ECOs", Enabled = false)
        let updateTitle() = this.Text <- "Grampus Batch - " + gmpfile
        
        let updateMenuStates() =
            crbtn.Enabled <- gmp.IsSome
            crfbtn.Enabled <- gmp.IsSome
            clsm.Enabled <- gmp.IsSome
            clsb.Enabled <- gmp.IsSome
            delm.Enabled <- gmp.IsSome
            cpym.Enabled <- gmp.IsSome
            cpyb.Enabled <- gmp.IsSome
            infm.Enabled <- gmp.IsSome
            infb.Enabled <- gmp.IsSome
            addm.Enabled <- gmp.IsSome
            addb.Enabled <- gmp.IsSome
            crm.Enabled <- gmp.IsSome
            deltm.Enabled <- gmp.IsSome
            deltfm.Enabled <- gmp.IsSome
            deltgfm.Enabled <- gmp.IsSome
            crfm.Enabled <- gmp.IsSome
            enewm.Enabled <- gmp.IsSome
            estrm.Enabled <- gmp.IsSome
            eplym.Enabled <- gmp.IsSome
            pgnm.Enabled <- gmp.IsNone
            addpm.Enabled <- gmp.IsSome
            topm.Enabled <- gmp.IsSome
            cmpm.Enabled <- gmp.IsSome
            cmpb.Enabled <- gmp.IsSome
            remcm.Enabled <- gmp.IsSome
            remcb.Enabled <- gmp.IsSome
            remvm.Enabled <- gmp.IsSome
            remvb.Enabled <- gmp.IsSome
            remnm.Enabled <- gmp.IsSome
            remnb.Enabled <- gmp.IsSome
            remdm.Enabled <- gmp.IsSome
            remdb.Enabled <- gmp.IsSome
            ecom.Enabled <- gmp.IsSome
            ecob.Enabled <- gmp.IsSome
        
        let log (msg) =
            nd <- DateTime.Now
            logtb.Text <- logtb.Text + nl + msg + " in " + el() + " seconds"
            st <- DateTime.Now
        
        let updprg (i) =
            prg.Value <- i
            Application.DoEvents()
        
        let donew() =
            let ndlg =
                new SaveFileDialog(Title = "Create New Base", 
                                   Filter = "Grampus databases(*.grampus)|*.grampus", 
                                   AddExtension = true, OverwritePrompt = false, 
                                   InitialDirectory = bfol)
            if ndlg.ShowDialog() = DialogResult.OK then 
                //create database
                gmpfile <- ndlg.FileName
                gmp <- Some(Grampus.New(gmpfile))
                setbinfol()
                iea <- [||]
                hdra <- [||]
                Recents.addrec gmpfile
                updateMenuStates()
                updateTitle()
        
        let doopen (ifn : string) =
            if ifn = "" then 
                let ndlg =
                    new OpenFileDialog(Title = "Open Base", 
                                       Filter = "Grampus databases(*.grampus)|*.grampus", 
                                       InitialDirectory = bfol)
                if ndlg.ShowDialog() = DialogResult.OK then 
                    gmpfile <- ndlg.FileName
                    Recents.addrec gmpfile
            elif File.Exists(ifn) then gmpfile <- ifn
            if gmpfile <> "" then 
                gmp <- Some(Grampus.Load gmpfile)
                setbinfol()
                iea <- Index.Load gmpfile
                hdra <- Headers.Load gmpfile
                updateMenuStates()
                updateTitle()
        
        let doclose (e) =
            gmpfile <- ""
            gmp <- None
            binfol <- ""
            iea <- [||]
            hdra <- [||]
            ply <- 0
            updateMenuStates()
            updateTitle()
        
        let dodel (e) =
            Grampus.Delete(gmpfile)
            doclose (new EventArgs())
        
        let docopy (e) =
            let copynm = Path.GetFileNameWithoutExtension(gmpfile) + "_Copy"
            let ndlg =
                new SaveFileDialog(Title = "Copy Base", 
                                   Filter = "Grampus databases(*.grampus)|*.grampus", 
                                   AddExtension = true, OverwritePrompt = false, 
                                   InitialDirectory = bfol, 
                                   FileName = copynm + ".grampus")
            if ndlg.ShowDialog() = DialogResult.OK then 
                let copygmpfile = ndlg.FileName
                Grampus.Copy(gmpfile, copygmpfile)
        
        let doinfo (e) =
            logtb.Text <- logtb.Text + nl + "INFO FOR " + gmpfile
            logtb.Text <- logtb.Text + nl + "Pgn Source: " + gmp.Value.SourcePgn
            let gmtxt =
                let crdt = gmp.Value.BaseCreated
                if crdt.IsNone then "No Games Included"
                else 
                    "Games updated at " + crdt.Value.ToLongTimeString() + " on " 
                    + crdt.Value.ToLongDateString()
            logtb.Text <- logtb.Text + nl + gmtxt
            let trtxt =
                let trdt = gmp.Value.TreesCreated
                if trdt.IsNone then "No Tree Included"
                else 
                    "Trees updated at " + trdt.Value.ToLongTimeString() + " on " 
                    + trdt.Value.ToLongDateString() + " to ply " 
                    + gmp.Value.TreesPly.ToString()
            logtb.Text <- logtb.Text + nl + trtxt
            let ftxt =
                let fdt = gmp.Value.FiltersCreated
                if fdt.IsNone then "No Filters Included"
                else 
                    "Filters updated at " + fdt.Value.ToLongTimeString() 
                    + " on " + fdt.Value.ToLongDateString() + " to ply " 
                    + gmp.Value.FiltersPly.ToString()
            logtb.Text <- logtb.Text + nl + ftxt
        
        let doadd (e) =
            let ndlg =
                new SaveFileDialog(Title = "Add Base", 
                                   Filter = "Grampus databases(*.grampus)|*.grampus", 
                                   AddExtension = true, OverwritePrompt = false, 
                                   InitialDirectory = bfol)
            if ndlg.ShowDialog() = DialogResult.OK then 
                this.Enabled <- false
                let addgmpfile = ndlg.FileName
                let addiea = Index.Load addgmpfile
                let numgames = addiea.Length
                prg.Minimum <- 0
                prg.Maximum <- numgames
                st <- DateTime.Now
                lbl.Text <- "Adding " + numgames.ToString() + " games..."
                Application.DoEvents()
                Games.AddGmp gmpfile addgmpfile updprg
                log ("Games Added")
                lbl.Text <- "Ready"
                this.Enabled <- true
                prg.Value <- 0
        
        let docreate (e) =
            let totaldict =
                new System.Collections.Generic.Dictionary<string, MvTrees>()
            
            let processGame i =
                let gm = Games.LoadGame gmpfile iea.[i] hdra.[i]
                let posns, mvs = Game.GetPosnsMoves ply gm
                let welo = gm.Hdr.W_Elo
                let belo = gm.Hdr.B_Elo
                
                let res =
                    gm.Hdr.Result
                    |> Result.Parse
                    |> Result.ToInt
                
                let yr = gm.Hdr.Year
                
                let tryParseInt s =
                    try 
                        s |> int
                    with :? FormatException -> 0
                
                let gminfo =
                    { Gmno = i
                      Welo = welo |> tryParseInt
                      Belo = belo |> tryParseInt
                      Year = yr
                      Result = res }
                
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
                if i % 100 = 0 then updprg (i)
                sts
            
            let dlg = new DlgPly(Text = "Create Tree")
            if dlg.ShowDialog() = DialogResult.OK then 
                ply <- int (dlg.Ply)
                this.Enabled <- false
                let numgames = iea.Length
                totaldict.Clear()
                prg.Minimum <- 0
                prg.Maximum <- numgames
                lbl.Text <- "Processing " + numgames.ToString() + " games..."
                Application.DoEvents()
                for i = 0 to numgames - 1 do
                    processGame (i)
                    if i % 100 = 0 then updprg (i)
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
                lbl.Text <- "Ready"
                gmp <- Some({ gmp.Value with TreesCreated = Some(DateTime.Now)
                                             TreesPly = ply })
                Grampus.Save(gmpfile, gmp.Value)
                this.Enabled <- true
                prg.Value <- 0
        
        let docreatef (e) =
            let totaldict =
                new System.Collections.Generic.Dictionary<string, int list>()
            
            let processGame i =
                let gm = Games.LoadGame gmpfile iea.[i] hdra.[i]
                let posns = Game.GetPosns ply gm
                //now need to go through the boarda and put in dictionary holding running totals
                for j = 0 to posns.Length - 1 do
                    let bd = posns.[j]
                    if totaldict.ContainsKey(bd) then 
                        let gms : int list = totaldict.[bd]
                        totaldict.[bd] <- (i :: gms)
                    else totaldict.[bd] <- [ i ]
            
            let dlg = new DlgPly(Text = "Create Filters")
            if dlg.ShowDialog() = DialogResult.OK then 
                ply <- int (dlg.Ply)
                this.Enabled <- false
                let numgames = iea.Length
                totaldict.Clear()
                prg.Minimum <- 0
                prg.Maximum <- numgames
                st <- DateTime.Now
                lbl.Text <- "Processing " + numgames.ToString() + " games..."
                Application.DoEvents()
                for i = 0 to numgames - 1 do
                    processGame (i)
                    if i % 100 = 0 then updprg (i)
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
                Filters.Create(binfol) |> ignore
                log ("Created dictionary")
                lbl.Text <- "Saving dictionary..."
                Application.DoEvents()
                let rgmls = gmls |> Array.map (fun l -> List.rev l)
                Filters.Save(posns, rgmls, binfol) |> ignore
                log ("Saved dictionary")
                lbl.Text <- "Ready"
                Application.DoEvents()
                gmp <- Some({ gmp.Value with FiltersCreated = Some(DateTime.Now)
                                             FiltersPly = ply })
                Grampus.Save(gmpfile, gmp.Value)
                this.Enabled <- true
                prg.Value <- 0
        
        let dodeltree (e) =
            st <- DateTime.Now
            gmp <- Some(Grampus.DeleteTree(gmpfile))
            log ("Tree deleted")
        
        let dodelfilters (e) =
            st <- DateTime.Now
            gmp <- Some(Grampus.DeleteFilters(gmpfile))
            log ("Filters deleted")
        
        let dodelgamesfilters (e) =
            st <- DateTime.Now
            gmp <- Some(Grampus.DeleteGamesFilters(gmpfile))
            log ("Games and Filters deleted")
        
        let doextractnewest (e) =
            let dlg = new DlgYr(Text = "Extract Newest Games")
            if dlg.ShowDialog() = DialogResult.OK then 
                let yr = int (dlg.Year)
                let copynm =
                    Path.GetFileNameWithoutExtension(gmpfile) + "_Yr" 
                    + yr.ToString()
                let ndlg =
                    new SaveFileDialog(Title = "Extract to Base", 
                                       Filter = "Grampus databases(*.grampus)|*.grampus", 
                                       AddExtension = true, 
                                       OverwritePrompt = false, 
                                       InitialDirectory = bfol, 
                                       FileName = copynm + ".grampus")
                if ndlg.ShowDialog() = DialogResult.OK then 
                    let trgnm = ndlg.FileName
                    this.Enabled <- false
                    let numgames = iea.Length
                    prg.Minimum <- 0
                    prg.Maximum <- numgames
                    st <- DateTime.Now
                    lbl.Text <- "Processing " + numgames.ToString() 
                                + " games..."
                    Application.DoEvents()
                    Games.ExtractNewer gmpfile trgnm yr updprg
                    log ("Extracted Games from Year " + yr.ToString())
                    lbl.Text <- "Ready"
                    this.Enabled <- true
                    prg.Value <- 0
        
        let doextractstrongest (e) =
            let dlg = new DlgGd(Text = "Extract Strongest Games")
            if dlg.ShowDialog() = DialogResult.OK then 
                let gd = int (dlg.Grade)
                let copynm =
                    Path.GetFileNameWithoutExtension(gmpfile) + "_Gd" 
                    + gd.ToString()
                let ndlg =
                    new SaveFileDialog(Title = "Extract to Base", 
                                       Filter = "Grampus databases(*.grampus)|*.grampus", 
                                       AddExtension = true, 
                                       OverwritePrompt = false, 
                                       InitialDirectory = bfol, 
                                       FileName = copynm + ".grampus")
                if ndlg.ShowDialog() = DialogResult.OK then 
                    let trgnm = ndlg.FileName
                    this.Enabled <- false
                    let numgames = iea.Length
                    prg.Minimum <- 0
                    prg.Maximum <- numgames
                    st <- DateTime.Now
                    lbl.Text <- "Processing " + numgames.ToString() 
                                + " games..."
                    Application.DoEvents()
                    Games.ExtractStronger gmpfile trgnm gd updprg
                    log ("Extracted Games from Grade " + gd.ToString())
                    lbl.Text <- "Ready"
                    this.Enabled <- true
                    prg.Value <- 0
        
        let doextractplayer (e) =
            let dlg = new DlgNm(Text = "Enter Part of the Player's Name")
            if dlg.ShowDialog() = DialogResult.OK then 
                let part = dlg.Part
                this.Enabled <- false
                let numgames = iea.Length
                prg.Minimum <- 0
                prg.Maximum <- numgames
                st <- DateTime.Now
                lbl.Text <- "Processing " + numgames.ToString() + " games..."
                Application.DoEvents()
                let nms = Games.GetPossNames gmpfile part updprg
                log 
                    ("Extracted " + (nms.Length.ToString()) + " Names matching " 
                     + part)
                lbl.Text <- "Ready"
                this.Enabled <- true
                prg.Value <- 0
                let pdlg = new DlgPlyr(Text = "Select Player's Name")
                pdlg.SetItems(nms)
                if pdlg.ShowDialog() = DialogResult.OK then 
                    let player = pdlg.Player
                    let copynm = player
                    let ndlg =
                        new SaveFileDialog(Title = "Extract to Base", 
                                           Filter = "Grampus databases(*.grampus)|*.grampus", 
                                           AddExtension = true, 
                                           OverwritePrompt = false, 
                                           InitialDirectory = bfol, 
                                           FileName = copynm + ".grampus")
                    if ndlg.ShowDialog() = DialogResult.OK then 
                        let trgnm = ndlg.FileName
                        this.Enabled <- false
                        let numgames = iea.Length
                        prg.Minimum <- 0
                        prg.Maximum <- numgames
                        st <- DateTime.Now
                        lbl.Text <- "Processing " + numgames.ToString() 
                                    + " games..."
                        Application.DoEvents()
                        Games.ExtractPlayer gmpfile trgnm player updprg
                        log ("Extracted Games from Player " + player)
                        lbl.Text <- "Ready"
                        this.Enabled <- true
                        prg.Value <- 0
        
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
                    lbl.Text <- "Saving " + numgames.ToString() + " games..."
                    Games.Save gmpfile egms updprg
                    gmp <- Some
                               ({ GrampusDataEMP with SourcePgn = pgn
                                                      BaseCreated =
                                                          Some(DateTime.Now) })
                    Grampus.Save(gmpfile, gmp.Value)
                    log ("Saved games")
                    lbl.Text <- "Ready"
                    iea <- Index.Load gmpfile
                    hdra <- Headers.Load gmpfile
                    this.Enabled <- true
                    prg.Value <- 0
                    updateTitle()
                    updateMenuStates()
        
        let doaddpgn() =
            let ndlg =
                new OpenFileDialog(Title = "Add PGN File", 
                                   Filter = "Pgn Files(*.pgn)|*.pgn", 
                                   InitialDirectory = bfol)
            if ndlg.ShowDialog() = DialogResult.OK then 
                this.Enabled <- false
                let pgnf = ndlg.FileName
                let numgames = PgnGames.GetNumberOfGames pgnf
                prg.Minimum <- 0
                prg.Maximum <- numgames
                prg.Value <- 0
                st <- DateTime.Now
                let ugma = PgnGames.ReadSeqFromFile pgnf
                let egma = ugma |> Seq.map (Game.Encode)
                Games.Add gmpfile egma updprg
                log ("PGN file added")
                gmp <- Some({ gmp.Value with SourcePgn = pgnf
                                             BaseCreated = Some(DateTime.Now) })
                Grampus.Save(gmpfile, gmp.Value)
                iea <- Index.Load gmpfile
                hdra <- Headers.Load gmpfile
                prg.Value <- 0
                this.Enabled <- true
        
        let dotopgn() =
            let fn = Path.GetFileNameWithoutExtension(gmpfile) + ".pgn"
            let ndlg =
                new SaveFileDialog(Title = "Select PGN file", 
                                   Filter = "PGN files(*.pgn)|*.pgn", 
                                   FileName = fn, AddExtension = true, 
                                   OverwritePrompt = false, 
                                   InitialDirectory = bfol)
            if ndlg.ShowDialog() = DialogResult.OK then 
                let pgn = ndlg.FileName
                this.Enabled <- false
                use stream = new FileStream(pgn, FileMode.Create)
                use writer = new StreamWriter(stream)
                let numgames = iea.Length
                prg.Minimum <- 0
                prg.Maximum <- numgames
                lbl.Text <- "Processing " + numgames.ToString() + " games..."
                Application.DoEvents()
                for i = 0 to numgames - 1 do
                    let gm = Games.LoadGame gmpfile iea.[i] hdra.[i]
                    let pgnstr = Game.ToStr gm
                    writer.Write(pgnstr)
                    writer.WriteLine()
                    if i % 100 = 0 then updprg (i)
                log ("Processed Games")
                lbl.Text <- "Ready"
                this.Enabled <- true
                prg.Value <- 0
        
        let docompact (e) =
            this.Enabled <- false
            let numgames = iea.Length
            prg.Minimum <- 0
            prg.Maximum <- numgames
            prg.Value <- 0
            st <- DateTime.Now
            lbl.Text <- "Cmopacting base for " + numgames.ToString() 
                        + " games..."
            let msg = Games.Compact gmpfile updprg
            log ("Base Compacted")
            logtb.Text <- logtb.Text + nl + "Messages from Compaction: " + nl 
                          + msg
            lbl.Text <- "Ready"
            prg.Value <- 0
            this.Enabled <- true
        
        let doremcomm() =
            this.Enabled <- false
            let numgames = iea.Length
            prg.Minimum <- 0
            prg.Maximum <- numgames
            prg.Value <- 0
            st <- DateTime.Now
            lbl.Text <- "Removing Comments for " + numgames.ToString() 
                        + " games..."
            Games.RemoveComments gmpfile updprg
            log ("Comments Removed")
            lbl.Text <- "Ready"
            prg.Value <- 0
            this.Enabled <- true
        
        let doremrav() =
            this.Enabled <- false
            let numgames = iea.Length
            prg.Minimum <- 0
            prg.Maximum <- numgames
            prg.Value <- 0
            st <- DateTime.Now
            lbl.Text <- "Removing Variations for " + numgames.ToString() 
                        + " games..."
            Games.RemoveRavs gmpfile updprg
            log ("Variations Removed")
            lbl.Text <- "Ready"
            prg.Value <- 0
            this.Enabled <- true
        
        let doremnag() =
            this.Enabled <- false
            let numgames = iea.Length
            prg.Minimum <- 0
            prg.Maximum <- numgames
            prg.Value <- 0
            st <- DateTime.Now
            lbl.Text <- "Removing NAGs for " + numgames.ToString() + " games..."
            Games.RemoveNags gmpfile updprg
            log ("NAGs Removed")
            lbl.Text <- "Ready"
            prg.Value <- 0
            this.Enabled <- true
        
        let doremdup() =
            this.Enabled <- false
            let numgames = iea.Length
            prg.Minimum <- 0
            prg.Maximum <- numgames
            prg.Value <- 0
            st <- DateTime.Now
            lbl.Text <- "Removing Duplicates for " + numgames.ToString() 
                        + " games..."
            let msg = Games.RemoveDuplicates gmpfile updprg
            log ("Duplicates Removed")
            logtb.Text <- logtb.Text + nl + "Messages from Process: " + nl + msg
            lbl.Text <- "Ready"
            prg.Value <- 0
            this.Enabled <- true
        
        let doeco() =
            this.Enabled <- false
            let numgames = iea.Length
            prg.Minimum <- 0
            prg.Maximum <- numgames
            prg.Value <- 0
            st <- DateTime.Now
            lbl.Text <- "Setting ECOs for " + numgames.ToString() + " games..."
            Eco.ForBase gmpfile updprg
            log ("ECOs set")
            lbl.Text <- "Ready"
            prg.Value <- 0
            this.Enabled <- true
        
        let createts() =
            let newb =
                new ToolStripButton(Image = img "new.png", 
                                    ImageTransparentColor = Color.Magenta, 
                                    Text = "&New")
            newb.Click.Add(fun _ -> donew())
            ts.Items.Add(newb) |> ignore
            // open
            let opnb =
                new ToolStripButton(Image = img "opn.png", 
                                    ImageTransparentColor = Color.Magenta, 
                                    Text = "&Open")
            opnb.Click.Add(fun _ -> doopen (""))
            ts.Items.Add(opnb) |> ignore
            // close
            clsb.Click.Add(fun _ -> doclose())
            ts.Items.Add(clsb) |> ignore
            cpyb.Click.Add(docopy)
            ts.Items.Add(cpyb) |> ignore
            infb.Click.Add(doinfo)
            addb.Click.Add(doadd)
            ts.Items.Add(addb) |> ignore
            ts.Items.Add(infb) |> ignore
            ts.Items.Add(new ToolStripSeparator()) |> ignore
            ts.Items.Add(crbtn) |> ignore
            crbtn.Click.Add(docreate)
            ts.Items.Add(crfbtn) |> ignore
            crfbtn.Click.Add(docreatef)
            // second toolbar
            ts2.Items.Add(cmpb) |> ignore
            cmpb.Click.Add(fun _ -> docompact())
            ts2.Items.Add(remcb) |> ignore
            remcb.Click.Add(fun _ -> doremcomm())
            ts2.Items.Add(remvb) |> ignore
            remvb.Click.Add(fun _ -> doremrav())
            ts2.Items.Add(remnb) |> ignore
            remnb.Click.Add(fun _ -> doremnag())
            ts2.Items.Add(remdb) |> ignore
            remdb.Click.Add(fun _ -> doremdup())
            ts2.Items.Add(ecob) |> ignore
            ecob.Click.Add(fun _ -> doeco())
        
        let createms() =
            //base menu
            let bm = new ToolStripMenuItem(Text = "&Base")
            let newm =
                new ToolStripMenuItem(Image = img "new.png", 
                                      ImageTransparentColor = Color.Magenta, 
                                      ShortcutKeys = (Keys.Control ||| Keys.N), 
                                      Text = "&New")
            newm.Click.Add(fun _ -> donew())
            bm.DropDownItems.Add(newm) |> ignore
            let opnm =
                new ToolStripMenuItem(Image = img "opn.png", 
                                      ImageTransparentColor = Color.Magenta, 
                                      ShortcutKeys = (Keys.Control ||| Keys.O), 
                                      Text = "&Open")
            bm.DropDownItems.Add(opnm) |> ignore
            opnm.Click.Add(fun _ -> doopen (""))
            bm.DropDownItems.Add(clsm) |> ignore
            clsm.Click.Add(doclose)
            bm.DropDownItems.Add(delm) |> ignore
            delm.Click.Add(dodel)
            bm.DropDownItems.Add(cpym) |> ignore
            cpym.Click.Add(docopy)
            bm.DropDownItems.Add(infm) |> ignore
            infm.Click.Add(doinfo)
            bm.DropDownItems.Add(addm) |> ignore
            addm.Click.Add(doadd)
            // recents
            let recm = new ToolStripMenuItem(Text = "Recent")
            bm.DropDownItems.Add(recm) |> ignore
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
            let exitm = new ToolStripMenuItem(Text = "E&xit")
            exitm.Click.Add(fun _ -> this.Close())
            bm.DropDownItems.Add(exitm) |> ignore
            ms.Items.Add(bm) |> ignore
            //tree menu
            let tm = new ToolStripMenuItem(Text = "&Tree")
            tm.DropDownItems.Add(crm) |> ignore
            crm.Click.Add(docreate)
            tm.DropDownItems.Add(deltm) |> ignore
            deltm.Click.Add(dodeltree)
            ms.Items.Add(tm) |> ignore
            //filter menu
            let fm = new ToolStripMenuItem(Text = "&Filters")
            fm.DropDownItems.Add(crfm) |> ignore
            crfm.Click.Add(docreatef)
            fm.DropDownItems.Add(deltfm) |> ignore
            deltfm.Click.Add(dodelfilters)
            fm.DropDownItems.Add(deltgfm) |> ignore
            deltgfm.Click.Add(dodelgamesfilters)
            ms.Items.Add(fm) |> ignore
            //extract menu
            let exm = new ToolStripMenuItem(Text = "&Extract")
            exm.DropDownItems.Add(enewm) |> ignore
            enewm.Click.Add(doextractnewest)
            exm.DropDownItems.Add(estrm) |> ignore
            estrm.Click.Add(doextractstrongest)
            exm.DropDownItems.Add(eplym) |> ignore
            eplym.Click.Add(doextractplayer)
            ms.Items.Add(exm) |> ignore
            //pgn menu
            let pgm = new ToolStripMenuItem(Text = "&Pgn")
            pgm.DropDownItems.Add(pgnm) |> ignore
            pgnm.Click.Add(doimp)
            addpm.Click.Add(fun _ -> doaddpgn())
            pgm.DropDownItems.Add(addpm) |> ignore
            topm.Click.Add(fun _ -> dotopgn())
            pgm.DropDownItems.Add(topm) |> ignore
            ms.Items.Add(pgm) |> ignore
            //tools menu
            let tlm = new ToolStripMenuItem(Text = "Too&ls")
            cmpm.Click.Add(docompact)
            tlm.DropDownItems.Add(cmpm) |> ignore
            ecom.Click.Add(fun _ -> doeco())
            tlm.DropDownItems.Add(remcm) |> ignore
            remcm.Click.Add(fun _ -> doremcomm())
            tlm.DropDownItems.Add(remvm) |> ignore
            remvm.Click.Add(fun _ -> doremrav())
            tlm.DropDownItems.Add(remnm) |> ignore
            remnm.Click.Add(fun _ -> doremnag())
            tlm.DropDownItems.Add(remdm) |> ignore
            remdm.Click.Add(fun _ -> doremdup())
            tlm.DropDownItems.Add(ecom) |> ignore
            ms.Items.Add(tlm) |> ignore
            // about menu
            let abtm = new ToolStripMenuItem("About")
            let onl = new ToolStripMenuItem("Online Documentation")
            onl.Click.Add
                (fun _ -> 
                System.Diagnostics.Process.Start
                    (new System.Diagnostics.ProcessStartInfo("https://pbbwfc.github.io/Grampus/", 
                                                             UseShellExecute = true)) 
                |> ignore)
            abtm.DropDownItems.Add(onl) |> ignore
            let src = new ToolStripMenuItem("Source Code")
            src.Click.Add
                (fun _ -> 
                System.Diagnostics.Process.Start
                    (new System.Diagnostics.ProcessStartInfo("https://github.com/pbbwfc/Grampus", 
                                                             UseShellExecute = true)) 
                |> ignore)
            abtm.DropDownItems.Add(src) |> ignore
            ms.Items.Add(abtm) |> ignore
        
        let btmpnl =
            new Panel(Dock = DockStyle.Bottom, BorderStyle = BorderStyle.Fixed3D, 
                      Height = 30)
        let tppnl =
            new Panel(Dock = DockStyle.Top, BorderStyle = BorderStyle.Fixed3D, 
                      Height = 90)
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
            tppnl.Controls.Add(ts2)
            tppnl.Controls.Add(ts)
            tppnl.Controls.Add(ms)
            this.Controls.Add(tppnl)
