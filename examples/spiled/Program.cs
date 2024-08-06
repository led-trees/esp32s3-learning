using System.Device.Spi;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Hardware.Esp32;

namespace spiled
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            //var spiBusInfo = SpiDevice.GetBusInfo(2);
            //Debug.WriteLine($"{nameof(spiBusInfo.MaxClockFrequency)}: {spiBusInfo.MaxClockFrequency}");
            //Debug.WriteLine($"{nameof(spiBusInfo.MinClockFrequency)}: {spiBusInfo.MinClockFrequency}");

            var server = new ServerClass();
            Thread instanceCaller = new(new ThreadStart(server.Work));

            instanceCaller.Start();

            Thread.Sleep(Timeout.Infinite);
        }
    }

    public class ServerClass
    {
        const int STRIPS_CNT = 4;
        const int LEDS_CNT = 100;
        const int BUFER_SIZE = STRIPS_CNT * LEDS_CNT * 3;

        // The method that will be called when the thread is started.
        public void Work()
        {
            Configuration.SetPinFunction(41, DeviceFunction.SPI2_MOSI);
            Configuration.SetPinFunction(39, DeviceFunction.SPI2_MISO);
            Configuration.SetPinFunction(40, DeviceFunction.SPI2_CLOCK);

            var connectionSettings = new SpiConnectionSettings(2, 37);
            connectionSettings.ClockFrequency = 2_500_000;
            connectionSettings.DataBitLength = 8;
            connectionSettings.DataFlow = DataFlow.MsbFirst;
            connectionSettings.Mode = SpiMode.Mode0;
            connectionSettings.Configuration = SpiBusConfiguration.HalfDuplex;
            connectionSettings.ChipSelectLineActiveState = false;

            var spiDevice = SpiDevice.Create(connectionSettings);

            var strips = new Strip[STRIPS_CNT];
            for (var s = 0; s < STRIPS_CNT; s++)
                strips[s] = new Strip(s, LEDS_CNT);

            var writeBufferSpanByte = new byte[BUFER_SIZE];

            var led = new byte[3] { 0x_FF, 0x_FF, 50 };
            foreach (var strip in strips)
                strip.Write(writeBufferSpanByte, led);
            spiDevice.Write(writeBufferSpanByte);

            bool back = false;
            while (true)
            {
                if (led[0] == 255 && led[1] == 255 && led[2] == 50)
                {
                    back = true;
                }

                if (led[0] < 0x_FF)
                    led[0] += 5;
                else if (led[0] == 0x_FF && led[1] < 0x_FF)
                    led[1] += 5;
                else if (led[1] == 0x_FF && led[2] < 0x_FF)
                    led[2] += 5;
                else if (led[2] == 0x_FF)
                {
                    led[0] = 0x_00;
                    led[1] = 0x_00;
                    led[2] = 0x_00;
                }

                foreach (var strip in strips)
                    strip.Write(writeBufferSpanByte, led);

                spiDevice.Write(writeBufferSpanByte);

                if (back)
                    back = false;
                else
                    back = true;

                Thread.Sleep(10);
            }
        }
    }

    class Strip
    {
        readonly int index = 0;
        readonly int count = 0;
        //readonly byte[][] leds;

        public Strip(int index, int count)
        {
            this.index = index;
            this.count = count;

            //leds = new byte[count][];
            //for (int i = 0; i < count; i++)
            //    leds[i] = new byte[3] { 0x_00, 0x_00, 0x_00 };
        }

        //public void Increment()
        //{
        //    foreach (var led in leds)
        //    {
        //        if (led[0] < 0x_FF)
        //            led[0] += 5;
        //        else if (led[0] == 0x_FF && led[1] < 0x_FF)
        //            led[1] += 5;
        //        else if (led[1] == 0x_FF && led[2] < 0x_FF)
        //            led[2] += 5;
        //        else if (led[2] == 0x_FF)
        //        {
        //            led[0] = 0x_00;
        //            led[1] = 0x_00;
        //            led[2] = 0x_00;
        //        }
        //    }
        //}

        //public void Set(byte red, byte green, byte blue)
        //{
        //    foreach (var led in leds)
        //    {
        //        led[0] = red;
        //        led[1] = green;
        //        led[2] = blue;
        //    }
        //}

        public void Write(byte[] buffer, byte[] pixel)
        {
            var offset = index * 3;
            for (var i = 0; i < count; i++)
            {
                buffer[offset] = pixel[0];
                buffer[offset + 1] = pixel[1];
                buffer[offset + 2] = pixel[2];

                offset += 4 * 3;
            }
        }
    }

    class Led
    {
        byte red = 0x_00;
        byte green = 0x_00;
        byte blue = 0x_00;

        public byte Red => red;
        public byte Green => green;
        public byte Blue => blue;

        public void Set(byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        public void Increment()
        {
            if (red < 0x_FF)
                red += 5;
            else if (red == 0x_FF && green < 0x_FF)
                green += 5;
            else if (green == 0x_FF && blue < 0x_FF)
                blue += 5;
            else if (blue == 0x_FF)
            {
                red = 0x_00;
                green = 0x_00;
                blue = 0x_00;
            }
        }
    }
}