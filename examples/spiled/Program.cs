using System.Device.Spi;
using System.Diagnostics;
using System.Threading;
using interoplib;

namespace spiled
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            var pixels = 100;
            LedPixelController.Init(pixels, 0, 0, 255);

            Thread.Sleep(2000);

            var leds = new Leds(pixels);

            byte color = 0;
            while (color < 255)
            {
                leds.Color(color, color, color);

                Thread.Sleep(10);

                color += 5;
            }

            var colors = new Color[] {
                new (255, 0, 0),
                new ( 0, 255, 0 ),
                new (0, 0, 255 ),
                new (255, 255, 0),
                new (255, 0, 255),
                new (0, 255, 255),
                new (232, 225, 50),
            };

            foreach (var c in colors)
            {
                leds.Color(c);

                Thread.Sleep(1000);
            }

            //var spiBusInfo = SpiDevice.GetBusInfo(2);
            //Debug.WriteLine($"{nameof(spiBusInfo.MaxClockFrequency)}: {spiBusInfo.MaxClockFrequency}");
            //Debug.WriteLine($"{nameof(spiBusInfo.MinClockFrequency)}: {spiBusInfo.MinClockFrequency}");

            //Configuration.SetPinFunction(41, DeviceFunction.SPI2_MOSI);
            //Configuration.SetPinFunction(39, DeviceFunction.SPI2_MISO);
            //Configuration.SetPinFunction(40, DeviceFunction.SPI2_CLOCK);

            var connectionSettings = new SpiConnectionSettings(2, 37);
            //connectionSettings.ClockFrequency = 2_500_000;
            //connectionSettings.DataBitLength = 8;
            //connectionSettings.DataFlow = DataFlow.MsbFirst;
            //connectionSettings.Mode = SpiMode.Mode0;
            //connectionSettings.Configuration = SpiBusConfiguration.HalfDuplex;
            //connectionSettings.ChipSelectLineActiveState = false;

            //var spiDevice = SpiDevice.Create(connectionSettings);

            //var leds = new LedPixels();
            //Thread instanceCaller = new(new ThreadStart(leds.Work));

            //instanceCaller.Start();

            Thread.Sleep(Timeout.Infinite);
        }
    }

    public class Color
    {
        public byte Red = 0;
        public byte Green = 0;
        public byte Blue = 0;

        public Color(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
    }

    public class Leds
    {
        readonly byte[] data;

        public Leds(int countPixels)
        {
            var bufferSize = countPixels * 4 * 3;
            data = new byte[bufferSize];

            for (var i = 0; i < bufferSize; i++)
                data[i] = 0;

            LedPixelController.Write(data);
        }

        public void Color(Color color)
        {
            Color(color.Red, color.Green, color.Blue);
        }

        public void Color(byte red, byte green, byte blue)
        {
            var color = 0;
            for (var i = 0; i < data.Length; i++)
            {
                if (color == 0)
                    data[i] = red;
                else if (color == 1)
                    data[i] = green;
                else if (color == 2)
                    data[i] = blue;

                if (color == 2)
                    color = 0;
                else
                    color++;
            }

            LedPixelController.Write(data);
        }
    }

    public class LedPixels
    {
        const int STRIPS_CNT = 4;
        const int LEDS_CNT = 200;
        const int BUFER_SIZE = STRIPS_CNT * LEDS_CNT * 3;

        readonly Strip[] strips = new Strip[STRIPS_CNT];
        readonly byte[] led = new byte[3] { 0x_FF, 0x_FF, 0x_FF };
        byte[] buffer = new byte[BUFER_SIZE];
        bool back = false;

        public LedPixels()
        {
        }

        public void Work()
        {
            for (var s = 0; s < STRIPS_CNT; s++)
                strips[s] = new Strip(s, LEDS_CNT);

            for (var s = 0; s < BUFER_SIZE; s++)
                buffer[s] = 0x_FF;

            LedPixelController.Init(LEDS_CNT, 0x_00, 0x_00, 0x_FF);

            Timer refreshTimer = new(RefreshCallback, null, 1000, 5);

            Thread.Sleep(Timeout.Infinite);
        }

        void RefreshCallback(object state)
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
                strip.Write(buffer, led);

            LedPixelController.Write(buffer);

            if (back)
                back = false;
            else
                back = true;
        }

        class Strip
        {
            readonly int index = 0;
            readonly int count = 0;

            public Strip(int index, int count)
            {
                this.index = index;
                this.count = count;
            }

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
    }
}