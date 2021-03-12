namespace GrampusWinForms

open System.Drawing
open System.IO
open System.Windows.Forms
open Grampus

[<AutoOpen>]
module TpGamesLib =
    type TpGames() as gmstp =
        inherit TabPage(Width = 800, Height = 250, Text = "")
        let gms =
            new DataGridView(Width = 800, Height = 250, 
                             AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders, 
                             ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single, 
                             AllowUserToAddRows = false, 
                             AllowUserToDeleteRows = false, ReadOnly = true, 
                             CellBorderStyle = DataGridViewCellBorderStyle.Single, 
                             GridColor = Color.Black, MultiSelect = false, 
                             RowHeadersVisible = false, Dock = DockStyle.Fill)
        let mutable crw = -1
        let mutable gmsui = new System.ComponentModel.BindingList<Header>()
        let bs = new BindingSource()
        //scinc related
        let mutable gmpfile : string = "" //base name
        let mutable gn = 0 //number of games
        let mutable fn = 0 //number of games in filter
        let mutable filt = []
        let mutable hdrs = [||]
        let mutable hasnofilt = false
        //events
        let selEvt = new Event<_>()
        
        let settxt() =
            let txt =
                Path.GetFileNameWithoutExtension(gmpfile) + "-" + fn.ToString() 
                + "/" + gn.ToString()
            gmstp.Text <- txt
        
        let color() =
            for rwo in gms.Rows do
                let rw = rwo :?> DataGridViewRow
                if rw.Cells.["Deleted"].Value :?> string = "D" then 
                    rw.DefaultCellStyle.ForeColor <- Color.Red
        
        let dodel (rw : int) =
            let gnum = gms.Rows.[rw].Cells.[0].Value :?> int
            let hdr = hdrs.[gnum]
            let nhdr = { hdr with Deleted = "D" }
            hdrs.[gnum] <- nhdr
            Headers.Save(gmpfile, hdrs)
            gmsui.[rw] <- nhdr
            gms.Rows.[rw].DefaultCellStyle.ForeColor <- Color.Red
        
        let domvup (rw : int) =
            let gnum = gms.Rows.[rw].Cells.[0].Value :?> int
            if gnum>0 then
                let hdr = {hdrs.[gnum] with Num=gnum-1}
                let phdr = {hdrs.[gnum-1] with Num=gnum}
                hdrs.[gnum-1] <- hdr
                hdrs.[gnum] <- phdr
                Headers.Save(gmpfile, hdrs)
                let iea = Index.Load(gmpfile)
                let ie = iea.[gnum]
                let pie = iea.[gnum-1]
                iea.[gnum-1] <- ie
                iea.[gnum] <- pie
                Index.Save(gmpfile, iea)
                gmsui.[rw-1] <- hdr
                gmsui.[rw] <- phdr
                gmstp.SelNum(gnum-1) 

        let domvdn (rw : int) =
            let gnum = gms.Rows.[rw].Cells.[0].Value :?> int
            if gnum<hdrs.Length-1 then
                let hdr = {hdrs.[gnum] with Num=gnum+1}
                let nhdr = {hdrs.[gnum+1] with Num=gnum}
                hdrs.[gnum-1] <- hdr
                hdrs.[gnum+1] <- {hdr with Num=gnum+1}
                hdrs.[gnum] <- {nhdr with Num=gnum}
                Headers.Save(gmpfile, hdrs)
                let iea = Index.Load(gmpfile)
                let ie = iea.[gnum]
                let nie = iea.[gnum+1]
                iea.[gnum+1] <- ie
                iea.[gnum] <- nie
                Index.Save(gmpfile, iea)
                gmsui.[rw+1] <- hdr
                gmsui.[rw] <- nhdr
                gmstp.SelNum(gnum+1) 


        let doload (rw : int) =
            gms.CurrentCell <- gms.Rows.[rw].Cells.[0]
            crw <- gms.Rows.[rw].Cells.[0].Value :?> int
            crw |> selEvt.Trigger
        
        let dodoubleclick (e : DataGridViewCellEventArgs) =
            let rw = e.RowIndex
            doload (rw)
        
        let setup() =
            bs.DataSource <- gmsui
            gms.DataSource <- bs
            gmstp.Controls.Add(gms)
            gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
        
        let dorightclick (e : DataGridViewCellMouseEventArgs) =
            let rw = e.RowIndex
            
            let ctxmnu =
                let m = new ContextMenuStrip()
                //do load
                let load = new ToolStripMenuItem(Text = "Load")
                load.Click.Add(fun _ -> doload (rw))
                m.Items.Add(load) |> ignore
                //do delete
                let del = new ToolStripMenuItem(Text = "Delete")
                del.Click.Add(fun _ -> dodel (rw))
                m.Items.Add(del) |> ignore
                //do move up
                let mvup = new ToolStripMenuItem(Text = "Move Up")
                mvup.Click.Add(fun _ -> domvup (rw))
                m.Items.Add(mvup) |> ignore
                //do move down
                let mvdn = new ToolStripMenuItem(Text = "Move Down")
                mvdn.Click.Add(fun _ -> domvdn (rw))
                m.Items.Add(mvdn) |> ignore

                m
            gms.ContextMenuStrip <- ctxmnu
            if e.Button = MouseButtons.Right then gms.ContextMenuStrip.Show()
        
        do 
            setup()
            gms.CellDoubleClick.Add(dodoubleclick)
            gms.CellMouseDown.Add(dorightclick)
        
        /// initialise
        member gmstp.Init(igmpfile : string, nofilt) =
            gmpfile <- igmpfile
            let indx = Index.Load gmpfile
            gn <- indx.Length
            fn <- gn
            filt <- []
            hdrs <- Headers.Load gmpfile
            hasnofilt <- nofilt
            settxt()
        
        ///Refresh the list
        member gmstp.Refrsh(bdstr : string) =
            gmsui.Clear()
            filt <- Filters.Read(bdstr, gmpfile)
            hdrs <- Headers.Load gmpfile
            if hasnofilt then 
                fn <- hdrs.Length
                let lim =
                    if fn > 100 then 100
                    else fn - 1
                [ 0..lim ] |> List.iter (fun i -> gmsui.Add(hdrs.[i]))
            elif filt.Length = 0 then fn <- 0
            else 
                fn <- filt.Length
                let lim =
                    if fn > 100 then 100
                    else fn - 1
                filt.[0..lim] |> List.iter (fun i -> gmsui.Add(hdrs.[i]))
            gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            settxt()
            color()
        
        ///Refresh the list
        member gmstp.SelNum(num : int) =
            for rwo in gms.Rows do
                let rw = rwo :?> DataGridViewRow
                if rw.Cells.[0].Value :?> int = num then 
                    crw <- num
                    gms.CurrentCell <- rw.Cells.[0]
        
        ///get baseNum
        member gmstp.BaseName() = gmpfile
        
        /// close
        member gmstp.Close() = ()
        
        ///Provides the selected Game
        member __.GmSel = selEvt.Publish
