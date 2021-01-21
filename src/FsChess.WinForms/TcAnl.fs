namespace FsChess.WinForms

open System.Windows.Forms
open FsChess
open System.IO


[<AutoOpen>]
module PnlAnlLib =
    type TpAnl() as this =
        inherit TabPage(Width = 400, Text = "Engine Stopped")
        let mutable isanl = false
        let mutable prc = new System.Diagnostics.Process()
        let mutable cbd = Board.Start
        let mutable num = 0
        let mutable fol = ""
        let mutable engnm = ""

        let sbtn = 
            new System.Windows.Forms.Button(Text = "Start", 
                                            Dock = DockStyle.Right)
        let dg = 
            new WebBrowser(AllowWebBrowserDrop = false,IsWebBrowserContextMenuEnabled = false,WebBrowserShortcutsEnabled = false,Dock=DockStyle.Fill)
        let nl = System.Environment.NewLine
        let hdr = 
            "<html><body>" + nl +
            "<table style=\"width:100%;border-collapse: collapse;\">" + nl +
            "<tr><th style=\"text-align: left;\">Score</th><th style=\"text-align: left;\">Line</th>" + nl
        let ftr = 
            "</table>" + nl +
            "</body></html>" + nl
  
        ///send message to engine
        let Send(command:string) = 
            prc.StandardInput.WriteLine(command)
        
        ///set up engine
        let ComputeAnswer(fen, depth) = 
            Send("ucinewgame")
            Send("setoption name Threads value " + (System.Environment.ProcessorCount - 1).ToString())
            Send("position startpos")
            Send("position fen " + fen + " ")
            Send("go depth " + depth.ToString())
        
        ///set up process
        let SetUpPrc () = 
            prc.StartInfo.CreateNoWindow <- true
            prc.StartInfo.FileName <- Path.Combine(fol, engnm + ".cmd")
            prc.StartInfo.WorkingDirectory <- fol
            prc.StartInfo.RedirectStandardOutput <- true
            prc.StartInfo.UseShellExecute <- false
            prc.StartInfo.RedirectStandardInput <- true
            prc.StartInfo.WindowStyle <- System.Diagnostics.ProcessWindowStyle.Hidden
            prc.Start() |> ignore
            prc.BeginOutputReadLine()
        
        ///Gets the Score and Line from a message
        let GetScrLn(msg:string,bd:Brd) =
            if msg.StartsWith("info") then
                let ln = 
                    let st = msg.LastIndexOf("pv")
                    let ucis = msg.Substring(st+2)
                    let sanstr = ucis|>Move.FromUcis bd
                    sanstr
                let scr =
                    let st = msg.LastIndexOf("cp")
                    let ss = msg.Substring(st+2,10).Trim()
                    let bits = ss.Split([|' '|])
                    let cp = float(bits.[0])/100.0
                    cp.ToString("0.00")

                scr,ln
            else
                "0.00",msg
        
        //add Message
        let addMsg (scr,ln) = 
            let addmsg() = 
                let nr = "<tr><td style='width: 50px;'>" + scr.ToString() + "</td><td>" + ln + "</td></tr>" + nl
                dg.DocumentText <- hdr + nr + ftr
            if (this.InvokeRequired) then 
                try 
                    this.Invoke(MethodInvoker(fun () -> addmsg())) |> ignore
                with _ -> ()
            else addmsg()

        //set header
        let setHdr msg = 
            if (this.InvokeRequired) then 
                try 
                    this.Invoke(MethodInvoker(fun () -> this.Text <- msg)) 
                    |> ignore
                with _ -> ()
            else this.Text <- msg
        
        let AnlpStart() = 
            prc <- new System.Diagnostics.Process()
            let mutable oldln = ""
            //p_out
            let pOut (e : System.Diagnostics.DataReceivedEventArgs) = 
                if not (e.Data = null || e.Data = "") then 
                    let msg = e.Data.ToString().Trim()
                    if not (msg.StartsWith("info") && not (msg.Contains(" cp "))) then 
                        let scr,ln = (msg,cbd)|>GetScrLn
                        //should only update ln if new or if new line is longer or at least 6 moves long
                        let bits = ln.Split(' ')
                        let oldbits = oldln.Split(' ') 
                        if bits.[0]<>oldbits.[0]||bits.Length>oldbits.Length||bits.Length>5 then
                            oldln <- ln
                            (scr,ln) |> addMsg
            prc.OutputDataReceived.Add(pOut)
            //Start process
            SetUpPrc()
            // call calcs
            // need to send game position moves as UCI
            let fen = cbd|>Board.ToStr
            ComputeAnswer(fen, 99)
            isanl <- true
            ("Engine " + num.ToString() + " - " + engnm + ": Calculating...") |> setHdr

        let AnlpStop() = 
            if prc <> null then 
                prc.Kill(true)
            isanl <- false
            ("Engine " + num.ToString() + " - " + engnm + ": Stopped") |> setHdr
        
        //set up analysis
        let setAnl start = 
            let setanl() = 
                if start then 
                    dg.DocumentText <- hdr + ftr
            if (this.InvokeRequired) then 
                try 
                    this.Invoke(MethodInvoker(fun () -> setanl())) |> ignore
                with _ -> ()
            else setanl()
    
        //start or stop
        let startstop() =
            if sbtn.Text="Start" then
                sbtn.Text<-"Stop"
                AnlpStart()
            else
                sbtn.Text<-"Start"
                AnlpStop()

        do 
            this.Controls.Add(dg)
            this.Controls.Add(sbtn)
            setAnl(true)
            sbtn.Click.Add(fun _ -> startstop())

        member this.SetBoard(ibd) =
            if not isanl then cbd<-ibd

        member this.Init(inum:int) =
            num <- inum
            fol <- Path.GetDirectoryName
                    (System.Reflection.Assembly.GetExecutingAssembly().Location) + 
                    "\\Engines\\" + num.ToString() + "\\" 
            let fn = Directory.GetFiles(fol,"*.cmd").[0]
            engnm <- Path.GetFileNameWithoutExtension(fn)
            setHdr("Engine " + num.ToString() + " - " + engnm)

 
    type TcAnl() as anltc =
        inherit TabControl(Width = 800, Height = 250)
    
        let anl1tp = 
            let tp = new TpAnl()
            tp.Init(1)
            tp
        let anl2tp = 
            let tp = new TpAnl()
            tp.Init(2)
            tp

        do
            anltc.TabPages.Add(anl1tp)
            anltc.TabPages.Add(anl2tp)

        ///Refresh the selected tab
        member gmstc.SetBoard(ibd) =
            anl1tp.SetBoard(ibd)
            anl2tp.SetBoard(ibd)

