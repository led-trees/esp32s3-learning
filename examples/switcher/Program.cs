using System.Device.Gpio;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Hardware.Esp32;

namespace switcher
{
    public class Program
    {
        static GpioController s_GpioController;

        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            var count = 8;
            var pins = new int[] { 35, 36, 6, 7, 15, 48, 47, 21 };
            var leds = new PwmChannel[count];

            s_GpioController = new GpioController();

            for (var i = 0; i < count; i++)
            {
                var pin = pins[i];
                Configuration.SetPinFunction(pin, GetPwmFunction(i));
                var pwmPin = PwmChannel.CreateFromPin(pin, 1000, 0);
                pwmPin.Start();
                leds[i] = pwmPin;
            }

            while (true)
            {
                double value = 0;
                while (value < 1)
                {
                    for (var i = 0; i < count; i++)
                        leds[i].DutyCycle = value;

                    value += 0.05;

                    Thread.Sleep(50);
                }

                value = 1;
                for (var i = 0; i < count; i++)
                    leds[i].DutyCycle = value;

                Thread.Sleep(1000);

                while (value > 0)
                {
                    for (var i = 0; i < count; i++)
                        leds[i].DutyCycle = value;

                    value -= 0.05;

                    Thread.Sleep(50);
                }

                value = 0;
                for (var i = 0; i < count; i++)
                    leds[i].DutyCycle = value;

                Thread.Sleep(1000);
            }
        }

        static DeviceFunction GetPwmFunction(int index)
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