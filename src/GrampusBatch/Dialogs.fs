namespace GrampusBatch

open System.Windows.Forms
open System.Drawing

module Dialogs =
    //dialogs to inherit from
    type Dialog() as this =
        inherit Form(Text = "NOT SET", Height = 110, Width = 280, 
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
        let okbtn = new Button(Text = "OK")
        let cnbtn = new Button(Text = "Cancel")
        
        do 
            this.MaximizeBox <- false
            this.MinimizeBox <- false
            this.ShowInTaskbar <- false
            this.StartPosition <- FormStartPosition.CenterParent
            hc2.Controls.Add(cnbtn)
            hc2.Controls.Add(okbtn)
            [ hc1; hc2 ] |> List.iteri (fun i c -> vc.Controls.Add(c, 0, i))
            this.Controls.Add(vc)
            this.AcceptButton <- okbtn
            this.CancelButton <- cnbtn
            //events
            cnbtn.Click.Add(fun _ -> this.Close())
            okbtn.Click.Add(this.DoOK)
        
        member this.AddControl(cnt) = hc1.Controls.Add(cnt)
        abstract DoOK : System.EventArgs -> unit
        override this.DoOK(e) =
            MessageBox.Show("OK pressed") |> ignore
            this.Close()
    
    type DlgPly() as this =
        inherit Dialog()
        let lbl =
            new Label(Text = "Select Ply ", AutoSize = false, 
                      TextAlign = ContentAlignment.MiddleLeft, Width = 80)
        let chb = new CheckBox(Text = "infinite")
        let spn =
            new NumericUpDown(Value = 20.0m, Minimum = -1.0m, Maximum = 50.0m, 
                              Width = 50)
        
        let docb (e) =
            if chb.Checked then 
                spn.Value <- -1m
                spn.Enabled <- false
            else 
                spn.Value <- 20m
                spn.Enabled <- true
        
        do 
            this.AddControl(lbl)
            this.AddControl(chb)
            this.AddControl(spn)
            chb.CheckStateChanged.Add(docb)
        
        override this.DoOK(e) =
            this.DialogResult <- DialogResult.OK
            this.Close()
        
        member this.SetText(txt) = lbl.Text <- txt
        member this.Ply = spn.Value
    
    type DlgYr() as this =
        inherit Dialog()
        let lbl =
            new Label(Text = "Select Minimum Year ", AutoSize = false, 
                      TextAlign = ContentAlignment.MiddleLeft, Width = 150)
        let spn =
            new NumericUpDown(Minimum = 1960.0m, Maximum = 2021.0m, Width = 50)
        
        do 
            this.AddControl(lbl)
            this.AddControl(spn)
            spn.Value <- 2000.0m
        
        override this.DoOK(e) =
            this.DialogResult <- DialogResult.OK
            this.Close()
        
        member this.SetText(txt) = lbl.Text <- txt
        member this.Year = spn.Value
    
    type DlgGd() as this =
        inherit Dialog()
        let lbl =
            new Label(Text = "Select Minimum Grade ", AutoSize = false, 
                      TextAlign = ContentAlignment.MiddleLeft, Width = 150)
        let spn =
            new NumericUpDown(Minimum = 1500.0m, Maximum = 2800.0m, Width = 50)
        
        do 
            this.AddControl(lbl)
            this.AddControl(spn)
            spn.Value <- 2100.0m
        
        override this.DoOK(e) =
            this.DialogResult <- DialogResult.OK
            this.Close()
        
        member this.SetText(txt) = lbl.Text <- txt
        member this.Grade = spn.Value
    
    type DlgNm() as this =
        inherit Dialog()
        let lbl =
            new Label(Text = "Part of Name ", AutoSize = false, 
                      TextAlign = ContentAlignment.MiddleLeft, Width = 80)
        let tb = new TextBox(Width = 150)
        
        do 
            this.AddControl(lbl)
            this.AddControl(tb)
        
        override this.DoOK(e) =
            this.DialogResult <- DialogResult.OK
            this.Close()
        
        member this.SetText(txt) = lbl.Text <- txt
        member this.Part = tb.Text
    
    type DlgPlyr() as this =
        inherit Dialog()
        let lbl =
            new Label(Text = "Player ", AutoSize = false, 
                      TextAlign = ContentAlignment.MiddleLeft, Width = 60)
        let cb =
            new ComboBox(Width = 170, DropDownStyle = ComboBoxStyle.DropDownList)
        
        do 
            this.AddControl(lbl)
            this.AddControl(cb)
        
        override this.DoOK(e) =
            this.DialogResult <- DialogResult.OK
            this.Close()
        
        member this.SetItems(nms : string []) =
            if nms.Length > 0 then 
                cb.Items.AddRange(nms |> Array.map box)
                cb.SelectedIndex <- 0
        
        member this.Player = cb.SelectedItem.ToString()
