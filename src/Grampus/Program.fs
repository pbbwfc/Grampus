namespace ScincNet

open System
open System.Windows.Forms
open Form

module Main =
    [<STAThread>]
    Application.EnableVisualStyles()
    let frm = new FrmMain()
    Application.Run(frm)