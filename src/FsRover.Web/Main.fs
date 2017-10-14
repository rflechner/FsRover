namespace FsRover.Web

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/photo">] Photo
    | [<EndPoint "/drive">] Drive
    | [<EndPoint "/about">] About

module Templating =
    open WebSharper.UI.Next.Html

    type MainTemplate = Templating.Template<"Main.html">

    // Compute a menubar where the menu item for the given endpoint is active
    let MenuBar (ctx: Context<EndPoint>) endpoint : Doc list =
        let ( => ) txt act =
             liAttr [if endpoint = act then yield attr.``class`` "active"] [
                aAttr [attr.href (ctx.Link act)] [text txt]
             ]
        [
            li ["Home" => EndPoint.Home]
            li ["Photo" => EndPoint.Photo]
            li ["Drive" => EndPoint.Drive]
            li ["About" => EndPoint.About]
        ]

    let Main ctx action (title: string) (body: Doc list) =
        Content.Page(
            MainTemplate()
                .Title(title)
                .MenuBar(MenuBar ctx action)
                .Body(body)
                .Doc()
        )

module Site =
    open WebSharper.UI.Next.Html

    let HomePage ctx =
        Templating.Main ctx EndPoint.Home "Home" [
            h1 [text "Say Hi to the server!"]
            div [client <@ Client.Main() @>]
        ]

    let PhotoPage (ctx:Context<EndPoint>) =
        let uri = ctx.Request.Uri
        let photoUrl = sprintf "%s://%s:%d/api/photo/300/240/jpeg" uri.Scheme uri.Host uri.Port
        Templating.Main ctx EndPoint.Photo "Photo" [
            h1 [text "Photo"]
            imgAttr [attr.src photoUrl] []
        ]

    let DrivePage (ctx:Context<EndPoint>) =
        let uri = ctx.Request.Uri
        Templating.Main ctx EndPoint.Drive "Drive" [
            h1 [text "Drive"]
            div [client <@ Client.Drive() @>]
        ]

    let AboutPage ctx =
        Templating.Main ctx EndPoint.About "About" [
            h1 [text "About"]
            p [text "This is a template WebSharper client-server application."]
        ]

    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> HomePage ctx
            | EndPoint.Photo -> PhotoPage ctx
            | EndPoint.Drive -> DrivePage ctx
            | EndPoint.About -> AboutPage ctx
        )

    open WebSharper.Suave
    open Suave.Web
    open Suave.Logging
    open System
    open System.IO
    open System.Reflection
    open Suave.Http
    open System.Net

    open Suave.Swagger
    open Rest
    open FunnyDsl
    open Swagger
    open System
    open Suave.Successful
    open Suave.WebPart
    open Suave.Filters
    open FsRover.Services
    open Suave.Writers
    open WebSharper.Core
    open Suave.Operators
    open FsRover.Services.Handlers
    open Messages
    open BCM2835Interop

    let subtract(a,b) = OK ((a-b).ToString())
    
    let Binary bytes =
        fun (ctx : HttpContext) ->
            async {
              return!
               { ctx
                  with
                    response =
                      { ctx.response
                          with
                            status = { ctx.response.status with code = 200 }
                            content = Bytes bytes
                      }
               } |> succeed
            }

    let downloadPhoto rotation (width,height,format) =
      let data,mimeType = Vision.takePhoto(width,height,format) rotation
      setMimeType mimeType 
        >=> setHeader "Cache-Control" "no-cache, no-store, must-revalidate"
        >=> setHeader "Pragma" "no-cache"
        >=> setHeader "Expires" "0"
        >=> Binary data

    let forward  =
        fun (ctx : HttpContext) ->
            async {
                physicalMailbox.Post (MoveForward (TimeSpan.FromSeconds 1.))
                return! OK "Success" ctx
            }
    let backward : WebPart =
        fun (ctx : HttpContext) ->
            async {
                physicalMailbox.Post (MoveBackward (TimeSpan.FromSeconds 1.))
                return! OK "Success" ctx
            }
    let turnRight : WebPart =
        fun (ctx : HttpContext) ->
            async {
                physicalMailbox.Post (MoveRight (TimeSpan.FromSeconds 1.))
                return! OK "Success" ctx
            }
    let turnLeft : WebPart =
        fun (ctx : HttpContext) ->
            async {
                physicalMailbox.Post (MoveLeft (TimeSpan.FromSeconds 1.))
                return! OK "Success" ctx
            }
            
    let now : WebPart =
      fun (x : HttpContext) ->
        async {
          return! MODEL DateTime.Now x
        }

    let stopMotors  =
        fun (ctx : HttpContext) ->
            async {
                BridgeH.StopMotors ()
                return! OK "Success" ctx
            }

    let api = 
      swagger {
        // For GET
        for route in getting <| urlFormat "/api/subtract/%d/%d" subtract do
          yield description Of route is "Subtracts two numbers"
        // For POST
        for route in posting <| urlFormat "/api/subtract/%d/%d" subtract do
          yield description Of route is "Subtracts two numbers"

        for route in getting <| urlFormat "/api/photo/%d/%d/%s" (downloadPhoto None) do
          yield description Of route is "Download a photo"

        for route in getting <| urlFormat "/api/photo/reversed/%d/%d/%s" (downloadPhoto (Some Vision.Angle180)) do
          yield description Of route is "Download a reversed photo"

        for route in getting <| (simpleUrl "/api/motors/stop" |> thenReturns stopMotors) do
            yield description Of route is "Stops motors"
            yield route |> tag "Motors"

        for route in getting (simpleUrl "/api/motors/forward" |> thenReturns forward) do
            yield description Of route is "Move forward"
            yield route |> tag "Motors"

        for route in getting <| (simpleUrl "/api/motors/backward" |> thenReturns backward) do
            yield description Of route is "Move backward"
            yield route |> tag "Motors"

        for route in getting <| (simpleUrl "/api/motors/turnLeft" |> thenReturns turnLeft) do
            yield description Of route is "Turn left"
            yield route |> tag "Motors"

        for route in getting <| (simpleUrl "/api/motors/turnRight" |> thenReturns turnRight) do
            yield description Of route is "Turn right"
            yield route |> tag "Motors"
    }

    [<EntryPoint>]
    let main args =
      
      let conf = { defaultConfig with
                      bindings = [HttpBinding.create HTTP IPAddress.Any 8089us] }

      let rootPath = System.AppDomain.CurrentDomain.BaseDirectory
      printfn "pathToAssembly %s" rootPath
      let webSite = WebSharperAdapter.ToWebPart(Main, RootDirectory = rootPath)
      
      let routes = choose 
                    [
                      api.App
                      webSite
                    ]

      if args |> Seq.contains "vsdebug" |> not
      then
        try
          let r1 = initGpio()
          printfn "GPIO started %A" r1
          BridgeH.Init()
          Handlers.StartHandlers()
        with
          | e -> 
              printfn "Error: %A" e.StackTrace
              printfn "Please try to execute this program with root privileges."
        
      startWebServer conf routes
      0
