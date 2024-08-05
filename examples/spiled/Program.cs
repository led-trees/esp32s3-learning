using System;
using System.Device.Spi;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Hardware.Esp32;

namespace spiled
{
    public class Program
    {
        const int STRIPS_CNT = 2;
        const int LEDS_CNT = 10;

        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            Configuration.SetPinFunction(41, DeviceFunction.SPI2_MOSI);
            Configuration.SetPinFunction(39, DeviceFunction.SPI2_MISO);
            Configuration.SetPinFunction(40, DeviceFunction.SPI2_CLOCK);

            SpiBusInfo spiBusInfo = SpiDevice.GetBusInfo(1);
            Debug.WriteLine($"{nameof(spiBusInfo.MaxClockFrequency)}: {spiBusInfo.MaxClockFrequency}");
            Debug.WriteLine($"{nameof(spiBusInfo.MinClockFrequency)}: {spiBusInfo.MinClockFrequency}");

            SpiDevice spiDevice;
            SpiConnectionSettings connectionSettings;

            connectionSettings = new SpiConnectionSettings(2, 37);
            connectionSettings.ClockFrequency = 2_500_000;
            connectionSettings.DataBitLength = 7;
            connectionSettings.DataFlow = DataFlow.LsbFirst;
            connectionSettings.Mode = SpiMode.Mode0;
            connectionSettings.Configuration = SpiBusConfiguration.Simplex;
            connectionSettings.ChipSelectLineActiveState = true;

            // Then you create your SPI device by passing your settings
            spiDevice = SpiDevice.Create(connectionSettings);

            // You can write a SpanByte
            SpanByte writeBufferSpanByte = new byte[2] { 42, 84 };
            spiDevice.Write(writeBufferSpanByte);

            spiDevice.WriteByte(200);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
