using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Hardware.Esp32;

namespace blink
{
    public class Program
    {
        private static GpioController s_GpioController;

        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            s_GpioController = new GpioController();

            GpioPin led = s_GpioController.OpenPin(Gpio.IO38, PinMode.Output);

            led.Write(PinValue.High);

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}
