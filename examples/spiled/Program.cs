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

            var server = new LedPixels(spiDevice);
            Thread instanceCaller = new(new ThreadStart(server.Work));

            instanceCaller.Start();

            Thread.Sleep(Timeout.Infinite);
        }
    }

    public class LedPixels
    {
        const int STRIPS_CNT = 4;
        const int LEDS_CNT = 100;
        const int BUFER_SIZE = STRIPS_CNT * LEDS_CNT * 3;

        readonly SpiDevice spiDevice;
        readonly Strip[] strips = new Strip[STRIPS_CNT];
        readonly byte[] led = new byte[3] { 0x_FF, 0x_FF, 0x_FF };
        readonly byte[] buffer = new byte[BUFER_SIZE];
        bool back = false;

        public LedPixels(SpiDevice spiDevice)
        {
            this.spiDevice = spiDevice;
        }

        public void Work()
        {
            for (var s = 0; s < STRIPS_CNT; s++)
                strips[s] = new Strip(s, LEDS_CNT);

            for (var s = 0; s < BUFER_SIZE; s++)
                buffer[s] = 0x_FF;
            spiDevice.Write(buffer);

            Timer refreshTimer = new(RefreshCallback, null, 1000, 5);

            Thread.Sleep(Timeout.Infinite);
        }

        void RefreshCallback(object state)
        {
            //while (true)
            // {
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

            spiDevice.Write(buffer);

            if (back)
                back = false;
            else
                back = true;

            //Thread.Sleep(10);
            //    //}
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