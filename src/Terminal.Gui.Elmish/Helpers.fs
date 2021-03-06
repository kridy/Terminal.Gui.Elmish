﻿namespace Terminal.Gui.Elmish

module Helpers =

    open System    
    open Terminal.Gui

    let findFocused (view:View) =
        view
        |> Seq.cast<View>            
        |> Seq.collect (fun x -> x |> Seq.cast<View>)
        |> Seq.tryFind (fun x -> x.HasFocus)

    let rec getAllElements (view:View) =
        match view.Subviews |> Seq.toList with
        | [] -> []
        | l ->
            l 
            |> List.collect (
                fun i -> 
                    let subv = 
                        i.Subviews 
                        |> Seq.toList
                        |> List.collect getAllElements
                    
                    i::subv
            )

    let getFocusedElements (view:View) =
        getAllElements view
        |> List.filter (fun e -> e.HasFocus)
        

    type View with
        // terminal gui is not immutable so i have to calculate a unique identifier
        member this.GetId() = 
            let typeName = this.GetType().Name
            let bounds = this.Bounds.GetHashCode()
            let parentName = 
                if this.SuperView <> null then 
                    this.SuperView.GetId()
                else
                    "root"
            let titleOrText =
                let properties = 
                    this.GetType().GetProperties()
                    |> Array.filter (fun pi -> pi.Name = "Title" || pi.Name= "Text")
                    |> Array.map( fun pi -> pi.GetValue(this)|> string)
                    |> Array.collect (fun s -> s.ToCharArray())                    
                String(properties)

                
            sprintf "%s%i%s%s" typeName bounds parentName titleOrText

