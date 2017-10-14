namespace FsRover.Web

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html
open WebSharper.JQuery

[<JavaScript>]
module Client =

    type KeyCode =
      | KeyArrowLeft = 37
      | KeyArrowUp = 38
      | KeyArrowRight = 39
      | KeyArrowDown = 40

    let (|IsKey|_|) (k:KeyCode) (v:int) =
      let vk = int k
      if vk = v
      then Some ()
      else None

    let log s = JS.Eval (sprintf """console.log("%s");""" s) |> ignore

    let Main () =
        let rvInput = Var.Create ""
        let submit = Submitter.CreateOption rvInput.View
        let vReversed =
            submit.View.MapAsync(function
                | None -> async { return "" }
                | Some input -> Server.DoSomething input
            )
        div [
            Doc.Input [] rvInput
            Doc.Button "Send" [] submit.Trigger
            hr []
            h4Attr [attr.``class`` "text-muted"] [text "The server responded:"]
            divAttr [attr.``class`` "jumbotron"] [h1 [textView vReversed]]
        ]

    let Drive () =
      
      let forwardButton = Doc.Button "Forward" [] (fun _ -> Server.MoveForward() |> ignore)
      let leftButton = Doc.Button "Left" [] (fun _ -> Server.MoveBackward() |> ignore)
      let rightButton = Doc.Button "Right" [] (fun _ -> Server.MoveRight() |> ignore)
      let backwardButton = Doc.Button "Backward" [] (fun _ -> Server.MoveLeft() |> ignore)
      let cam = img []
      
      JQuery.Of("html").Keydown(
            fun _ event ->
                match event.Which with
                | IsKey KeyCode.KeyArrowLeft -> Server.MoveLeft()
                | IsKey KeyCode.KeyArrowDown -> Server.MoveBackward()
                | IsKey KeyCode.KeyArrowUp -> Server.MoveForward()
                | IsKey KeyCode.KeyArrowRight -> Server.MoveRight()
                | _ -> false
                |> ignore
            ) |> ignore
      
      let displayPhoto () = 
        let location = JS.Window.Location
        let url = sprintf "//%s/api/photo/reversed/300/240/jpeg" location.Host
        cam.SetAttribute("src", url)
      
      JS.SetInterval displayPhoto 150 |> ignore

      let createRow leftItems middleItems rightItems =
        divAttr [attr.``class`` "row"] 
          [
            divAttr [attr.``class`` "col-xs-6 col-sm-4"] leftItems
            divAttr [attr.``class`` "col-xs-6 col-sm-4"] middleItems
            divAttr [attr.``class`` "clearfix visible-xs-block"] []
            divAttr [attr.``class`` "col-xs-6 col-sm-4"] rightItems
          ]

      div [
        h2 [text "Command panel"]
        
        createRow [] [forwardButton] []
        createRow [leftButton] [cam] [rightButton]
        createRow [] [backwardButton] []

      ]
