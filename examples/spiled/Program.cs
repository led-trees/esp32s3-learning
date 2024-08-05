using System;
using System.Device.Spi;
using System.Diagnostics;
using nanoFramework.Hardware.Esp32;

namespace spiled
{
    public class Program
    {
        //const int SPI_NUMBER = 2;
        const int STRIPS_CNT = 2;
        const int LEDS_CNT = 10;

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
            connectionSettings.ClockFrequency = 4_000_000;
            connectionSettings.DataBitLength = 8;
            connectionSettings.DataFlow = DataFlow.MsbFirst;
            connectionSettings.Mode = SpiMode.Mode0;
            connectionSettings.Configuration = SpiBusConfiguration.FullDuplex;
            connectionSettings.ChipSelectLineActiveState = false;

            var spiDevice = SpiDevice.Create(connectionSettings);

            SpanByte writeBufferSpanByte = new byte[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            byte i = 0;
            bool back = false;
            while (true)
            {
                for (var b = 0; b < writeBufferSpanByte.Length; b++)
                    writeBufferSpanByte[b] = i;

                spiDevice.Write(writeBufferSpanByte);

                if (i == 0 && back)
                    back = false;
                else if (i == 255 && !back)
                    back = true;

                if (back)
                    i--;
                else
                    i++;

                //Thread.Sleep(1);
            }
        }
    }
}