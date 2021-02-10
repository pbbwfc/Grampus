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
        let mutable nm : string = "" //base name
        let mutable gn = 0 //number of games
        let mutable fn = 0 //number of games in filter
        let mutable filt = []
        let mutable hdrs = [||]
        let mutable hasnofilt = false
        //events
        let selEvt = new Event<_>()
        
        let settxt() =
            let txt =
                Path.GetFileNameWithoutExtension(nm) + "-" + fn.ToString() + "/" 
                + gn.ToString()
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
            let gmpfile = nm + ".grampus"
            Headers.Save(gmpfile, hdrs)
            gmsui.[rw] <- nhdr
            gms.Rows.[rw].DefaultCellStyle.ForeColor <- Color.Red
        
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
                m
            gms.ContextMenuStrip <- ctxmnu
            if e.Button = MouseButtons.Right then gms.ContextMenuStrip.Show()
        
        do 
            setup()
            gms.CellDoubleClick.Add(dodoubleclick)
            gms.CellMouseDown.Add(dorightclick)
        
        /// initialise
        member gmstp.Init(inm : string, nofilt) =
            nm <- inm
            let gmpfile = nm + ".grampus"
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
            let fol = nm + "_FILES"
            let gmpfile = nm + ".grampus"
            filt <- Filters.Read(bdstr, fol)
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
        member gmstp.BaseName() = nm
        
        /// close
        member gmstp.Close() = ()
        
        ///Provides the selected Game
        member __.GmSel = selEvt.Publish
