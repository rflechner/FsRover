module BCM2835Interop
    open System.Runtime.InteropServices

    type FselMode = 
        | InputMode = 0us
        | OutputMode = 1us
        | BCM2835_GPIO_FSEL_ALT0 = 0x04us
        | BCM2835_GPIO_FSEL_ALT1 = 0x05us
        | BCM2835_GPIO_FSEL_ALT2 = 0x06us
        | BCM2835_GPIO_FSEL_ALT3 = 0x07us
        | BCM2835_GPIO_FSEL_ALT4 = 0x03us
        | BCM2835_GPIO_FSEL_ALT5 = 0x02us
        | BCM2835_GPIO_FSEL_MASK = 0x07us

    type GpioPin =
        | GPIO4 = 4us
        | GPIO17 = 17us
        | GPIO27 = 27us
        | GPIO22 = 22us
        | GPIO5 = 5us
        | GPIO6 = 6us
        | GPIO13 = 13us
        | GPIO19 = 19us
        | GPIO26 = 26us
        | GPIO18 = 18us
        | GPIO23 = 23us
        | GPIO24 = 24us
        | GPIO25 = 25us
        | GPIO12 = 12us
        | GPIO16 = 16us
        | GPIO20 = 20us
        | GPIO21 = 21us

    type GpioClockDivider =
        | BCM2835_PWM_CLOCK_DIVIDER_2048  = 2048    
        | BCM2835_PWM_CLOCK_DIVIDER_1024  = 1024    
        | BCM2835_PWM_CLOCK_DIVIDER_512   = 512
        | BCM2835_PWM_CLOCK_DIVIDER_256   = 256     
        | BCM2835_PWM_CLOCK_DIVIDER_128   = 128     
        | BCM2835_PWM_CLOCK_DIVIDER_64    = 64     
        | BCM2835_PWM_CLOCK_DIVIDER_32    = 32      
        | BCM2835_PWM_CLOCK_DIVIDER_16    = 16      
        | BCM2835_PWM_CLOCK_DIVIDER_8     = 8       
        | BCM2835_PWM_CLOCK_DIVIDER_4     = 4
        | BCM2835_PWM_CLOCK_DIVIDER_2     = 2       
        | BCM2835_PWM_CLOCK_DIVIDER_1     = 1 

    [<DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_fsel")>]
    extern void setPinMode(GpioPin pin, FselMode mode);

    [<DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_set")>]
    extern void setPin(GpioPin pin);

    [<DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_clr")>]
    extern void clearPin(GpioPin pin);

    let clearPins (pins:GpioPin list) =
        for pin in pins do
            clearPin pin

    [<DllImport("libbcm2835.so", EntryPoint = "bcm2835_init")>]
    extern bool initGpio()

    [<DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_write")>]
    extern void gpioWrite(GpioPin pin, bool on)

    let gpioWriteValues (pins:GpioPin list, on) =
        for pin in pins do
            gpioWrite (pin, on)

    [<DllImport("libbcm2835.so", EntryPoint = "bcm2835_delay")>]
    extern void gpioDelay(uint32 millis);

    [<DllImport("libbcm2835.so", EntryPoint = "bcm2835_delayMicroseconds")>]
    extern void gpioDelayMicroseconds(uint64 micros);


    [<DllImport("libbcm2835.so", EntryPoint = "bcm2835_pwm_set_clock")>]
    extern void pwmSetClock(GpioClockDivider divisor);

    [<DllImport("libbcm2835.so", EntryPoint = "bcm2835_pwm_set_mode")>]
    extern void pwmSetMode(uint16 pin, uint16 markspace, uint16 enabled);

    [<DllImport("libbcm2835.so", EntryPoint = "bcm2835_pwm_set_range")>]
    extern void pwmSetRange(uint16 pin, uint32 range);

    [<DllImport("libbcm2835.so", EntryPoint = "bcm2835_pwm_set_data")>]
    extern void pwmSetData(uint16 pin, uint32 data);

    [<DllImport("libbcm2835.so", EntryPoint = "bcm2835_close")>]
    extern void gpioClose();

