[<RequireQualifiedAccess>]
module BridgeH

open BCM2835Interop

let Pin1 = GpioPin.GPIO26
let Pin2 = GpioPin.GPIO19
let Pin3 = GpioPin.GPIO13
let Pin4 = GpioPin.GPIO6

let Init() =
    setPinMode(Pin1, FselMode.OutputMode)
    setPinMode(Pin2, FselMode.OutputMode)
    setPinMode(Pin3, FselMode.OutputMode)
    setPinMode(Pin4, FselMode.OutputMode)

let Release() =
    clearPins [Pin1; Pin2; Pin3; Pin4]

let StopMotors () =
    gpioWriteValues ([Pin1;Pin2;Pin3;Pin4], false)

