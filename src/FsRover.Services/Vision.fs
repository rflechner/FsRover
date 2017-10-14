namespace FsRover.Services

open Camera
open Messages
open System
open BCM2835Interop

module Vision =
  open System.Drawing
  open System.Drawing.Imaging
  open System.Runtime.InteropServices
  open System.IO

  type ImageRotation =
    | Angle90
    | Angle180
    | Angle270

  let clamp x s e =
    if x > e then e
    elif x < s then s
    else x

  let bToFloat (b:byte) =
    (float(b) - 128.)

  let rgbToYuv(r:byte,g:byte,b:byte) =
    let y = 0.299 * float(r) + 0.587 * float(g) + 0.114 * float(b)
    let u = 0.492 * (float(b) - y)
    let v = 0.877 * (float(r) - y)
    y,u,v

  let yuvToRgbSlow(y:float,u:float,v:float) =
    let rTmp = y + 1.140 * v
    let gTmp = y - 0.395 * u - 0.581 * v
    let bTmp = y + 2.032 * u
    let r = clamp rTmp 0. 255.
    let g = clamp gTmp 0. 255.
    let b = clamp bTmp 0. 255.
    r,g,b

  let yuvToRgb(y:float,u:float,v:float) =
    let rTmp = y + 1.140 * v
    let gTmp = y - 0.395 * u - 0.581 * v
    let bTmp = y + 2.032 * u
    let r = byte rTmp
    let g = byte gTmp
    let b = byte bTmp
    r,g,b

  let yToByte (y:float) = byte (clamp (int(y) + 128) 0 255)

  type YuvColor = 
    { Y:float; U:float; V:float }
    static member Empty = { Y=0.; U=0.; V=0. }
    static member FromBytes y u v = 
      { Y=bToFloat(y); U=bToFloat(u); V=bToFloat(v) }
  type Color with
    member __.ToYuv() =
      let (y,u,v) = rgbToYuv(__.R, __.G, __.B)
      { Y=y; U=u; V=v }
    static member FromYuv (y,u,v) =
      let (r,g,b) = yuvToRgb (y,u,v)
      Color.FromArgb(int(r), int(g), int (b))
    static member FromYuv (yuv:YuvColor) =
      Color.FromYuv(yuv.Y, yuv.U, yuv.V)

  type RawImage = 
    { Content:byte array
      Width:int
      Height:int
      NChannels:int
      WidthStep:int }
    member __.GetPixel x y =
      let i = y * __.WidthStep
      let offset = x*3
      let b = __.Content.[i+offset]
      let g = __.Content.[i+offset+1]
      let r = __.Content.[i+offset+2]
      Color.FromArgb(int r, int g, int b)
    member __.SetPixel x y (color:Color) =
      let i = y * __.WidthStep
      let offset = x*3
      __.Content.[i+offset] <- color.B
      __.Content.[i+offset+1] <- color.G
      __.Content.[i+offset+2] <- color.R
    member __.SetLuminance x y (l:byte) =
      let yuv = (__.GetPixel x y).ToYuv()
      Color.FromYuv(bToFloat l, yuv.U, yuv.V)
      |> __.SetPixel x y
    member __.SetChrominance x y (u:byte) (v:byte) =
      let yuv = (__.GetPixel x y).ToYuv()
      Color.FromYuv(yuv.Y, bToFloat u, bToFloat v)
      |> __.SetPixel x y

  module Sampling = 
    
    type ImageEncoder () =

      member __.DecodeRawImageSlow(img:RawImage) =
        if img.NChannels <> 3
        then raise(new NotSupportedException("Cannot decode anything else than 3 channels for the moment"))
        let bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb)
        for y in 0..(img.Height-1) do
          for x in 0..(img.Width-1) do
            let color = img.GetPixel x y
            bmp.SetPixel(x, y, color)
        bmp

      member __.DecodeRawImage(img:RawImage) =
        if img.NChannels <> 3
        then raise(new NotSupportedException("Cannot decode anything else than 3 channels for the moment"))
        let bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb)
        let data =
          seq {
                for y=0 to (img.Height-1) do
                  for x=0 to (img.Width-1) do
                      let i = y * 1920
                      let offset = x*3
                      let b = img.Content.[i+offset]
                      let g = img.Content.[i+offset+1]
                      let r = img.Content.[i+offset+2]
                      yield b; yield g; yield r;
          } |> Seq.toArray
        let area = new Rectangle(0, 0, bmp.Width, bmp.Height)
        let bmpData = bmp.LockBits(area, ImageLockMode.WriteOnly, bmp.PixelFormat)
        Marshal.Copy(data, 0, bmpData.Scan0, data.Length)
        bmp.UnlockBits bmpData
        bmp

  let takePhoto (width,height,format) (rotation: ImageRotation option) =
    Registry.cam.SetFrameSize(float width, float height)
    let raw = !Registry.lastFrame
    let img = { Width=width; Height=height; WidthStep=1920; NChannels=3; Content=raw }
    let encoder = Sampling.ImageEncoder()
    use bmp = encoder.DecodeRawImage img
    
    match rotation with
    | None -> ()
    | Some Angle90 -> bmp.RotateFlip(RotateFlipType.Rotate90FlipX)
    | Some Angle180 -> bmp.RotateFlip(RotateFlipType.Rotate180FlipX)
    | Some Angle270 -> bmp.RotateFlip(RotateFlipType.Rotate270FlipX)
    
    use memory = new MemoryStream()
    let (f, mimetype) =
        match format with
        | "jpg"
        | "jpeg" ->
            (ImageFormat.Jpeg, "image/jpeg")
        | "gif" -> (ImageFormat.Gif, "image/gif")
        | "png" -> (ImageFormat.Png, "image/png")
        | "tiff" -> (ImageFormat.Tiff, "image/tiff")
        | _ -> (ImageFormat.Bmp, "image/bmp")
    bmp.Save(memory, f)
    memory.Flush()
    memory.Position <- 0L
    memory.ToArray(), mimetype

