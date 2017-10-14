module Camera

open OpenCvInterop
open System
open System.Runtime.InteropServices
  

type PhotoGraber(id:int) =
  let mutable handle = cvCreateCameraCapture id

  member __.SetFrameSize(height, width) =
    cvSetCaptureProperty(handle, CaptureProperty.CV_CAP_PROP_FRAME_HEIGHT, height) |> ignore
    cvSetCaptureProperty(handle, CaptureProperty.CV_CAP_PROP_FRAME_WIDTH, width) |> ignore

  member __.GrabPicture() =
    let imgp = cvQueryFrame handle
    let miplImage = Marshal.PtrToStructure(imgp, typeof<MIplImage>) :?> MIplImage
    let size = miplImage.ImageSize
    let managedArray:byte array = Array.zeroCreate size
    Marshal.Copy(miplImage.ImageData, managedArray, 0, size)
    managedArray, miplImage

  member __.Close() =
    cvReleaseCapture (&handle)

