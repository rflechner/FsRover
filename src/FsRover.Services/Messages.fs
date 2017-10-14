namespace FsRover.Services

module Messages =
  open System

  type CameraMessages =
      | Echo of string
      | GrabFrame

  type PhysicalMessages =
      | MoveForward of TimeSpan
      | MoveBackward of TimeSpan
      | MoveLeft of TimeSpan
      | MoveRight of TimeSpan
