using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace blink
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            var gpioController = new GpioController();
            var led1 = gpioController.OpenPin(1, PinMode.Output);
            var led2 = gpioController.OpenPin(2, PinMode.Output);

            var on = false;
            while (true)
            {
                if (on)
                {
                    on = false;
                    led1.Write(PinValue.Low);
                    led2.Write(PinValue.High);
                }
                else
                {
                    on = true;
                    led1.Write(PinValue.High);
                    led2.Write(PinValue.Low);
                }

                Thread.Sleep(1000);
            }
        }
    }
}