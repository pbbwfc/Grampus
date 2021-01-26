namespace FsChess.WinForms

open System.Drawing
open System.IO
open System.Windows.Forms
open FsChess

[<AutoOpen>]
module TpGamesLib =

    type TpGames() as gmstp =
        inherit TabPage(Width = 800, Height = 250, 
                Text = "Clipbase")
        let gms = new DataGridView(Width = 800, Height = 250, 
                        AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders, 
                        ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                        AllowUserToAddRows = false,
                        AllowUserToDeleteRows = false,
                        ReadOnly = true,
                        CellBorderStyle = DataGridViewCellBorderStyle.Single,
                        GridColor = Color.Black, MultiSelect = false,
                        RowHeadersVisible=false, Dock=DockStyle.Fill
                        )
        let mutable crw = -1
        let mutable gmsui = new System.ComponentModel.BindingList<Header>()
        let bs = new BindingSource()
        //scinc related
        let mutable nm = "clipbase" //base name
        let mutable gn = 0 //number of games
        let mutable fn = 0 //number of games in filter
        let mutable filt = []
        let mutable hdrs = [||]

        //events
        let selEvt = new Event<_>()
        let cmpEvt = new Event<_>()

        let settxt() =
            let txt = Path.GetFileNameWithoutExtension(nm) + "-" + fn.ToString() + "/" + gn.ToString()
            gmstp.Text <- txt

        let color() =
            for rwo in gms.Rows do
                let rw = rwo:?>DataGridViewRow
                if rw.Cells.["Deleted"].Value:?>string = "D" then
                    rw.DefaultCellStyle.ForeColor <- Color.Red;
        
        let docmp() =
            nm|>cmpEvt.Trigger
        
        let dodel(rw:int) =
             let gnum = gms.Rows.[rw].Cells.[0].Value:?>int
             ()
             //if ScincFuncs.ScidGame.Delete(uint(gnum))=0 then
             //   gms.Rows.[rw].Cells.["Deleted"].Value<-"D"
             //   gms.Rows.[rw].DefaultCellStyle.ForeColor <- Color.Red;
 
        let doload(rw:int) =
            gms.CurrentCell <- gms.Rows.[rw].Cells.[0]
            crw <- gms.Rows.[rw].Cells.[0].Value:?>int
            crw|>selEvt.Trigger
        
        let dodoubleclick(e:DataGridViewCellEventArgs) =
            let rw = e.RowIndex
            doload(rw)
        
        let setup() =
            bs.DataSource <- gmsui
            gms.DataSource <- bs
            gmstp.Controls.Add(gms)
            gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)

         
        let dorightclick(e:DataGridViewCellMouseEventArgs) =
            let rw = e.RowIndex

            let ctxmnu = 
                 let m = new ContextMenuStrip()
                 //do load
                 let load =
                     new ToolStripMenuItem(Text = "Load")
                 load.Click.Add(fun _ -> doload(rw))
                 m.Items.Add(load) |> ignore
                 //do delete
                 let del =
                     new ToolStripMenuItem(Text = "Delete")
                 del.Click.Add(fun _ -> dodel(rw))
                 m.Items.Add(del) |> ignore
                 //do compact
                 let cmp =
                     new ToolStripMenuItem(Text = "Compact Base")
                 cmp.Click.Add(fun _ -> docmp())
                 m.Items.Add(cmp) |> ignore
                 m
            gms.ContextMenuStrip<-ctxmnu
            if e.Button=MouseButtons.Right then
                gms.ContextMenuStrip.Show()

        do 
            setup()
            gms.CellDoubleClick.Add(dodoubleclick)
            gms.CellMouseDown.Add(dorightclick)

        /// initialise
        member _.Init(inm:string) =
            nm <- inm
            let indx = Index.Load(inm + "_FILES")
            gn <- indx.Length
            fn <- gn
            filt <- []
            hdrs <- Headers.Load(inm + "_FILES")
            settxt()
            
        ///Refresh the list
        member _.Refrsh(bdstr:string) =
            gmsui.Clear()
            let fol = nm + "_FILES"
            filt <- Filter.Read(bdstr,fol)
            hdrs <- Headers.Load(fol)
            if filt.Length = 0 then
                fn <- hdrs.Length
                let lim = if fn>100 then 100 else fn-1
                [0..lim]|>List.iter(fun i -> gmsui.Add(hdrs.[i]))
            else
                fn <- filt.Length
                let lim = if fn>100 then 100 else fn-1
                filt.[0..lim]|>List.iter(fun i -> gmsui.Add(hdrs.[i]))
            gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            settxt()
            color()

        ///Refresh the list
        member _.SelNum(num:int) =
            for rwo in gms.Rows do
                let rw = rwo:?>DataGridViewRow
                if rw.Cells.[0].Value:?>int = num then
                    crw <- num
                    gms.CurrentCell <- rw.Cells.[0]

        //get baseNum
        member _.BaseName() = nm

        /// close
        member _.Close() =
            //update the treecach file before closing
            //ScincFuncs.Tree.Write(b)|>ignore
            //ScincFuncs.Base.Close()|>ignore
            ()
 
        ///Provides the selected Game
        member __.GmSel = selEvt.Publish

        ///Provides the base needing to be compacted
        member __.GmCmp = cmpEvt.Publish

        