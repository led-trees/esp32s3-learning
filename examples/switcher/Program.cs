using System.Device.Gpio;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Hardware.Esp32;

namespace switcher
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            var gpioController = new GpioController();

            var togglersEnable = gpioController.OpenPin(3, PinMode.Output); // 15 - CLK_EN выставить в 0 - это разрешение тактировани€. ћожно один раз выставить при загрузке программы
            var togglersRefresh = gpioController.OpenPin(8, PinMode.Output);   // 12 - LATCH/CS0
            var togglersClk = gpioController.OpenPin(40, PinMode.Output);  // 33 - SPI_CLK на clk
            var togglersValue = gpioController.OpenPin(39, PinMode.Input);  // 32 - DATA_SER_OUT/MISO на MISO

            // enable toggler function
            togglersEnable.Write(PinValue.Low);

            // init PWM pins
            var count = 8;
            var pins = new int[] { 35, 36, 6, 7, 15, 48, 47, 21 };
            var leds = new PwmChannel[count];
            for (var i = 0; i < count; i++)
            {
                var pin = pins[i];
                Configuration.SetPinFunction(pin, GetPWMFunction(i));
                var pwmPin = PwmChannel.CreateFromPin(pin, 1000, 0);
                pwmPin.Start();
                leds[i] = pwmPin;
            }

            while (true)
            {
                // ensure toggler registers
                togglersRefresh.Write(PinValue.Low);
                togglersRefresh.Write(PinValue.High);

                // read toggler state
                var togglers = new bool[8];
                for (var i = 7; i >= 0; i--)
                {
                    var pValue = togglersValue.Read();
                    togglers[i] = pValue != PinValue.High;

                    togglersClk.Write(PinValue.High);
                    togglersClk.Write(PinValue.Low);
                }

                double value = 0;
                while (value < 1)
                {
                    for (var i = 0; i < count; i++)
                    {
                        if (togglers[i] != true)
                            continue;
                        leds[i].DutyCycle = value;
                    }

                    value += 0.05;

                    Thread.Sleep(50);
                }

                value = 1;
                for (var i = 0; i < count; i++)
                {
                    if (togglers[i] != true)
                        continue;
                    leds[i].DutyCycle = value;
                }

                Thread.Sleep(1000);

                while (value > 0)
                {
                    for (var i = 0; i < count; i++)
                    {
                        if (togglers[i] != true)
                            continue;
                        leds[i].DutyCycle = value;
                    }

                    value -= 0.05;

                    Thread.Sleep(50);
                }

                value = 0;
                for (var i = 0; i < count; i++)
                {
                    if (togglers[i] != true)
                        continue;
                    leds[i].DutyCycle = value;
                }

                Thread.Sleep(1000);
            }
        }

        static DeviceFunction GetPWMFunction(int index)
        {
            switch (index)
            {
                case 0:
                    return DeviceFunction.PWM1;
                case 1:
                    return DeviceFunction.PWM2;
                case 2:
                    return DeviceFunction.PWM3;
                case 3:
                    return DeviceFunction.PWM4;
                case 4:
                    return DeviceFunction.PWM5;
                case 5:
                    return DeviceFunction.PWM6;
                case 6:
                    return DeviceFunction.PWM7;
                case 7:
                    return DeviceFunction.PWM8;
                default:
                    throw new System.Exception();
            }
        }
    }
}