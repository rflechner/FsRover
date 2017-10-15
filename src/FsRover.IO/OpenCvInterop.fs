module OpenCvInterop
  open System.Runtime.InteropServices
  open System
  #nowarn "9"

  // This file is a port of https://github.com/neutmute/PiCamCV/blob/master/source/LibPiCamCV/PInvoke/CvInvokeRaspiCamCV.cs

  [<StructLayout(LayoutKind.Sequential)>]
  type PiCameraConfig =
    { Width:int
      Height:int
      Bitrate:int
      Framerate:int
      Monochrome:int }
  type CaptureProperty = 
    | CV_CAP_PROP_POS_MSEC = 0
    | CV_CAP_PROP_POS_FRAMES = 1
    | CV_CAP_PROP_POS_AVI_RATIO = 2
    | CV_CAP_PROP_FRAME_WIDTH = 3
    | CV_CAP_PROP_FRAME_HEIGHT = 4
    | CV_CAP_PROP_FPS = 5
    | CV_CAP_PROP_FOURCC = 6
    | CV_CAP_PROP_FRAME_COUNT = 7
    | CV_CAP_PROP_FORMAT = 8
    | CV_CAP_PROP_MODE = 9
    | CV_CAP_PROP_BRIGHTNESS = 10
    | CV_CAP_PROP_CONTRAST = 11
    | CV_CAP_PROP_SATURATION = 12
    | CV_CAP_PROP_HUE = 13
    | CV_CAP_PROP_GAIN = 14
    | CV_CAP_PROP_EXPOSURE = 15
    | CV_CAP_PROP_CONVERT_RGB = 16
    | CV_CAP_PROP_WHITE_BALANCE_BLUE_U = 17
    | CV_CAP_PROP_RECTIFICATION = 18
    | CV_CAP_PROP_MONOCHROME = 19
    | CV_CAP_PROP_ISO_SPEED = 30

  [<Struct>]
  type MIplImage = 
      [<DefaultValue>] val mutable NSize:int
      [<DefaultValue>] val mutable ID:int
      [<DefaultValue>] val mutable NChannels:int
      [<DefaultValue>] val mutable AlphaChannel:int
      [<DefaultValue>] val mutable Depth:int
      [<DefaultValue>] val mutable ColorModel0:byte
      [<DefaultValue>] val mutable ColorModel1:byte
      [<DefaultValue>] val mutable ColorModel2:byte
      [<DefaultValue>] val mutable ColorModel3:byte
      [<DefaultValue>] val mutable ChannelSeq0:byte
      [<DefaultValue>] val mutable ChannelSeq1:byte
      [<DefaultValue>] val mutable ChannelSeq2:byte
      [<DefaultValue>] val mutable ChannelSeq3:byte
      [<DefaultValue>] val mutable DataOrder:int
      [<DefaultValue>] val mutable Origin:int
      [<DefaultValue>] val mutable Align:int
      [<DefaultValue>] val mutable Width:int
      [<DefaultValue>] val mutable Height:int
      [<DefaultValue>] val mutable Roi:IntPtr
      [<DefaultValue>] val mutable MaskROI:IntPtr
      [<DefaultValue>] val mutable ImageId:IntPtr
      [<DefaultValue>] val mutable TileInfo:IntPtr
      [<DefaultValue>] val mutable ImageSize:int
      [<DefaultValue>] val mutable ImageData:IntPtr
      [<DefaultValue>] val mutable WidthStep:int
      [<DefaultValue>] val mutable BorderMode0:int
      [<DefaultValue>] val mutable BorderMode1:int
      [<DefaultValue>] val mutable BorderMode2:int
      [<DefaultValue>] val mutable BorderMode3:int
      [<DefaultValue>] val mutable BorderConst0:int
      [<DefaultValue>] val mutable BorderConst1:int
      [<DefaultValue>] val mutable BorderConst2:int
      [<DefaultValue>] val mutable BorderConst3:int
      [<DefaultValue>] val mutable ImageDataOrigin:IntPtr

  [<DllImport("libraspicamcv.so", EntryPoint="raspiCamCvCreateCameraCapture", CallingConvention = CallingConvention.Cdecl)>]
  extern IntPtr cvCreateCameraCapture(int index)

  [<DllImport("libraspicamcv.so", EntryPoint = "raspiCamCvCreateCameraCapture2", CallingConvention = CallingConvention.Cdecl)>]
  extern IntPtr cvCreateCameraCapture2(int index, PiCameraConfig& config)

  [<DllImport("libraspicamcv.so", EntryPoint="raspiCamCvQueryFrame", CallingConvention = CallingConvention.Cdecl)>]
  extern IntPtr cvQueryFrame(IntPtr capture)

  [<DllImport("libraspicamcv.so", EntryPoint="raspiCamCvReleaseCapture", CallingConvention = CallingConvention.Cdecl)>]
  extern void cvReleaseCapture(IntPtr& capture)

  [<DllImport("libraspicamcv.so", EntryPoint = "raspiCamCvGetCaptureProperty", CallingConvention = CallingConvention.Cdecl)>]
  extern double cvGetCaptureProperty(IntPtr capture, CaptureProperty property)

  [<DllImport("libraspicamcv.so", EntryPoint = "raspiCamCvSetCaptureProperty", CallingConvention = CallingConvention.Cdecl)>]
  extern int cvSetCaptureProperty(IntPtr capture, CaptureProperty property, double value)

