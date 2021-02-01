namespace GrampusBatch

open System.Windows.Forms
open System.Drawing

module Dialogs =
    //dialogs to inherit from
    type Dialog() as this =
        inherit Form(Text = "NOT SET", Height = 150, Width = 280, 
                     FormBorderStyle = FormBorderStyle.FixedDialog)
        let vc =
            new TableLayoutPanel(Dock = DockStyle.Fill, ColumnCount = 1, 
                                 RowCount = 3)
        let hc1 =
            new FlowLayoutPanel(FlowDirection = FlowDirection.LeftToRight, 
                                Height = 30, Width = 260)
        let hc2 =
            new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
                                Height = 30, Width = 260)
        let hc3 =
            new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
                                Height = 30, Width = 260)
        let okbtn = new Button(Text = "OK")
        let cnbtn = new Button(Text = "Cancel")
        
        do 
            this.MaximizeBox <- false
            this.MinimizeBox <- false
            this.ShowInTaskbar <- false
            this.StartPosition <- FormStartPosition.CenterParent
            hc3.Controls.Add(cnbtn)
            hc3.Controls.Add(okbtn)
            [ hc1; hc2; hc3 ] 
            |> List.iteri (fun i c -> vc.Controls.Add(c, 0, i))
            this.Controls.Add(vc)
            this.AcceptButton <- okbtn
            this.CancelButton <- cnbtn
            //events
            cnbtn.Click.Add(fun _ -> this.Close())
            okbtn.Click.Add(this.DoOK)
        
        member this.AddControl1(cnt) = hc1.Controls.Add(cnt)
        member this.AddControl2(cnt) = hc2.Controls.Add(cnt)
        abstract DoOK : System.EventArgs -> unit
        override this.DoOK(e) =
            MessageBox.Show("OK pressed") |> ignore
            this.Close()
    
    type DlgTree() as this =
        inherit Dialog()
        let lbl = new Label(Text = "Select Ply (-1 infinite)", Width = 150)
        let spn =
            new NumericUpDown(Value = 20.0m, Minimum = -1.0m, Maximum = 50.0m, 
                              Width = 50)
        
        do 
            this.Text <- "Create Tree"
            this.AddControl1(lbl)
            this.AddControl1(spn)
        
        member this.SetText(txt) = lbl.Text <- txt
        member this.Ply = spn.Value
