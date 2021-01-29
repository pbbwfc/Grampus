namespace FsChess.WinForms

open System.Windows.Forms
open Grampus

[<AutoOpen>]
module TcGamesLib =
    type TcGames() as gmstc =
        inherit TabControl(Width = 800, Height = 250)
        
        let cliptp = new TpGames()
        //events
        let selEvt = new Event<_>()
        let cmpEvt = new Event<_>()
        
        do
            gmstc.TabPages.Add(cliptp)
            cliptp.GmSel|>Observable.add selEvt.Trigger
            cliptp.GmCmp|>Observable.add cmpEvt.Trigger

        ///Refresh the selected tab
        member gmstc.Refrsh(bd:Brd) =
            let tp = gmstc.SelectedTab:?>TpGames
            let bdstr = bd|>Board.ToSimpleStr
            tp.Refrsh(bdstr)

        ///Refresh the selected tab
        member gmstc.SelNum(num:int) =
            let tp = gmstc.SelectedTab:?>TpGames
            tp.SelNum(num)

        ///Add a new tab
        member gmstc.AddTab(nm) =
            let tp = new TpGames()
            tp.Init(nm)
            gmstc.TabPages.Add(tp)
            gmstc.SelectedTab<-gmstc.TabPages.[gmstc.TabPages.Count-1]
            tp.GmSel|>Observable.add selEvt.Trigger
            tp.GmCmp|>Observable.add cmpEvt.Trigger

        ///BaseNum for the selected tab
        member gmstc.BaseName() =
            let tp = gmstc.SelectedTab:?>TpGames
            tp.BaseName()
        
        ///Close the selected tab
        member gmstc.Close() =
            let tp = gmstc.SelectedTab:?>TpGames
            tp.Close()
            gmstc.TabPages.Remove(tp)


        ///Provides the selected Game
        member __.GmSel = selEvt.Publish

        ///Provides the base needing to be compacted
        member __.GmCmp = cmpEvt.Publish
 
