namespace FsRover.Web

open WebSharper

module Server =
    open FsRover.Services.Handlers
    open FsRover.Services.Messages
    open System

    let moveDuration = TimeSpan.FromMilliseconds 200.

    [<Rpc>]
    let DoSomething input =
        let R (s: string) = System.String(Array.rev(s.ToCharArray()))
        async {
            return R input
        }

    [<Rpc>]
    let MoveForward () =
        physicalMailbox.Post (MoveForward moveDuration)
        true

    [<Rpc>]
    let MoveBackward () =
        physicalMailbox.Post (MoveBackward moveDuration)
        true

    [<Rpc>]
    let MoveLeft () =
        physicalMailbox.Post (MoveLeft moveDuration)
        true

    [<Rpc>]
    let MoveRight () =
        physicalMailbox.Post (MoveRight moveDuration)
        true

