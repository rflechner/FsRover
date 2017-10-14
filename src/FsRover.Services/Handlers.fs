namespace FsRover.Services

open Camera
open Messages
open System
open BCM2835Interop

module Registry =
  let cam = PhotoGraber 0
  cam.SetFrameSize(240., 240.)
  let lastFrame:byte array ref = ref Array.empty


module Handlers =

  let cameraMailbox = new MailboxProcessor<CameraMessages>(fun inbox ->
      let rec loop count =
          async {
                  let! msg = inbox.Receive()
                  match msg with
                  | Echo text ->
                      printfn "Message received. Contents: %s" text
                  | GrabFrame ->
                      let raw,_ = Registry.cam.GrabPicture()
                      Registry.lastFrame := raw
                  return! loop( count + 1) }
      loop 0)

  let physicalMailbox = new MailboxProcessor<PhysicalMessages>(fun inbox ->
      let moveWith pins (duration:TimeSpan) =
          async {
              BridgeH.StopMotors ()
              gpioWriteValues (pins, true)
              do! Async.Sleep (int duration.TotalMilliseconds)
              BridgeH.StopMotors ()
          }
      let rec loop count =
          async {
                  let! msg = inbox.Receive()
                  match msg with
                  | MoveForward duration -> do! moveWith [BridgeH.Pin1;BridgeH.Pin3] duration
                  | MoveBackward duration -> do! moveWith [BridgeH.Pin2;BridgeH.Pin4] duration
                  | MoveLeft duration -> do! moveWith [BridgeH.Pin2;BridgeH.Pin3] duration
                  | MoveRight duration -> do! moveWith [BridgeH.Pin1;BridgeH.Pin4] duration
                  return! loop( count + 1) }
      loop 0)

  let private timer = new System.Timers.Timer(100.)
  timer.AutoReset <- true
  timer.Elapsed.Add(fun _ ->
      if cameraMailbox.CurrentQueueLength < 10
      then cameraMailbox.Post GrabFrame
  )

  let StartHandlers () =
    cameraMailbox.Start()
    physicalMailbox.Start()
    timer.Start()

