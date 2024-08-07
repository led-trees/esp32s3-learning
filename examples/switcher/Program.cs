using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

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
            var leds = new GpioPin[count];

            s_GpioController = new GpioController();

            for (var i = 0; i < count; i++)
            {
                var pin = pins[i];
                var led = s_GpioController.OpenPin(pin, PinMode.Output);
                leds[i] = led;
            }

            while (true)
            {
                for (var i = 0; i < count; i++)
                {
                    var pinOff = (i == 0) ? (count - 1) : (i - 1);

                    leds[i].Write(PinValue.High);
                    leds[pinOff].Write(PinValue.Low);

                    Debug.WriteLine($"gpio{leds[i].PinNumber} ON, gpio{leds[pinOff].PinNumber} OFF");
                }

                Thread.Sleep(1000);
            }
        }
    }
}