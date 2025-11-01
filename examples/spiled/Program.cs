using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using interoplib;

namespace spiled
{
    public class Program
    {
        public static string SoftApIP { get; set; } = "192.168.4.1";

        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            var gpioController = new GpioController();
            var ledIndicator = new LedIndicator(gpioController)
            {
                Led2 = true
            };

            //try
            //{
            //    WiFi.SetupAP("test", "555111555");

            //    ledIndicator.Led1 = true;
            //}
            //catch
            //{

            //}

            //Thread.Sleep(Timeout.Infinite);
            //return;


            //var togglers = new Togglers(gpioController);
            //var deviceNumber = togglers.Byte;
            //Debug.WriteLine($"Номер платы: {deviceNumber}");


            ushort channelPixels = 200;
            //LedPixelController.Init(41, 39, 40, 37, pixels, 0, 0, 0); // ver1
            LedPixelController.Init(41, 39, 40, 21, channelPixels, 0, 0, 0); // ver2

            var leds = new Leds(channelPixels);

            leds.Random(new Color[] {new (255,0,0),
                    new (0, 255, 0),
                    new (0,0,255),
                    new (50, 63, 248),
                    new (255, 251, 0) });

            var countFlickers = 30;
            var rndPixel = new Random();
            var rndTime = new Random();
            var flickers = new FlickerLed[countFlickers];
            var flickerColor = new Color(255, 255, 255);

            for (var i = 0; i < countFlickers; i++)
            {
                var pixel = (ushort)rndPixel.Next(leds.TotalPixels - 1);
                var prevColor = leds.GetColor(pixel);

                leds.Color(pixel, flickerColor);
                flickers[i] = new FlickerLed { Number = pixel, Deadline = DateTime.UtcNow.AddMilliseconds(rndTime.Next(5)), Prev = prevColor };
            }

            while (true)
            {
                var now = DateTime.UtcNow;
                foreach (var flicker in flickers)
                {
                    if (flicker.Deadline <= now)
                    {
                        leds.Color(flicker.Number, flicker.Prev); // возвращаем цвет

                        while (true)
                        {
                            var nextNumber = (ushort)rndPixel.Next(leds.TotalPixels - 1);

                            var exist = false;
                            foreach (var f in flickers)
                            {
                                if (f.Number == nextNumber)
                                    exist = true;
                            }

                            if (exist)
                                continue;

                            flicker.Number = nextNumber;
                            break;
                        }

                        flicker.Deadline = DateTime.UtcNow.AddMilliseconds(rndTime.Next(5));
                        flicker.Prev = leds.GetColor(flicker.Number);

                        leds.Color(flicker.Number, flickerColor);
                    }
                }

                Thread.Sleep(1);
            }

            while (true)
            {
                ledIndicator.Led2 = true;

                leds.Color(new(255, 255, 255));

                for (ushort i = 0; i < channelPixels; i++)
                {
                    leds.Color(i, new(255, 0, 0));

                    Thread.Sleep(10);
                }

                ledIndicator.Led2 = false;

                //Thread.Sleep(1000);
            }

            while (true)
            {
                ledIndicator.Led2 = true;

                //leds.Color(new(232, 225, 50));
                leds.Colors(new Color[] { new(255, 0, 0), new(255, 255, 255) });

                ledIndicator.Led2 = false;
                Thread.Sleep(1000);

                break;
            }

            leds.Color(new(255, 0, 0), new(0, 255, 0), new(0, 0, 255), new(255, 255, 0));
            // reg, green, blue, yellow

            while (true)
            {
                Thread.Sleep(2000);

                //byte color = 0;
                //while (color < 255)
                //{
                //    leds.Color(color, color, color);

                //    color += 15;
                //}

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

                    ledIndicator.Led2 = true;
                    Thread.Sleep(100);
                    ledIndicator.Led2 = false;
                }

                break;
            }

            //while (true)
            //{
            //    leds.Random();

            //    ledIndicator.Led2 = true;
            //    Thread.Sleep(200);
            //    ledIndicator.Led2 = false;
            //}

            //var spiBusInfo = SpiDevice.GetBusInfo(2);
            //Debug.WriteLine($"{nameof(spiBusInfo.MaxClockFrequency)}: {spiBusInfo.MaxClockFrequency}");
            //Debug.WriteLine($"{nameof(spiBusInfo.MinClockFrequency)}: {spiBusInfo.MinClockFrequency}");

            //Configuration.SetPinFunction(41, DeviceFunction.SPI2_MOSI);
            //Configuration.SetPinFunction(39, DeviceFunction.SPI2_MISO);
            //Configuration.SetPinFunction(40, DeviceFunction.SPI2_CLOCK);

            //var connectionSettings = new SpiConnectionSettings(2, 37);
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

    public class FlickerLed
    {
        public ushort Number { get; set; }
        public DateTime Deadline { get; set; }
        public Color Prev { get; set; }
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
        readonly ushort countPixels;
        public readonly ushort TotalPixels;

        public Leds(ushort countPixels)
        {
            this.countPixels = countPixels;
            TotalPixels = (ushort)(countPixels * 4);

            var bufferSize = countPixels * 4 * 3;
            data = new byte[bufferSize];

            for (var i = 0; i < bufferSize; i++)
                data[i] = 0;

            LedPixelController.Write(data);
        }

        public Color GetColor(ushort pixel)
        {
            var i = pixel * 3;
            return new(data[i + 0], data[i + 1], data[i + 2]);
        }

        public void Color(Color color)
        {
            Color(color.Red, color.Green, color.Blue);
        }

        public void Color(ushort cell, Color color)
        {
            // var i = cell * 3 * 4;
            // 
            // for (byte row = 0; row < 4; row++)
            // {
            //     data[i] = color.Red;
            //     data[i + 1] = color.Green;
            //     data[i + 2] = color.Blue;
            // 
            //     i += 3;
            // }

            var i = cell * 3;

            data[i + 0] = color.Red;
            data[i + 1] = color.Green;
            data[i + 2] = color.Blue;

            LedPixelController.Write(data);

            // LedPixelController.Set(0, cell, color.Red, color.Green, color.Blue);
            // LedPixelController.Set(1, cell, color.Red, color.Green, color.Blue);
            // LedPixelController.Set(2, cell, color.Red, color.Green, color.Blue);
            // LedPixelController.Set(3, cell, color.Red, color.Green, color.Blue);
        }

        public void Color(Color color1, Color color2, Color color3, Color color4)
        {
            var ch = 0;
            for (var i = 0; i < data.Length; i++)
            {
                switch (ch)
                {
                    case 0:
                        data[i] = color1.Red;
                        i++;
                        data[i] = color1.Green;
                        i++;
                        data[i] = color1.Blue;
                        break;
                    case 1:
                        data[i] = color2.Red;
                        i++;
                        data[i] = color2.Green;
                        i++;
                        data[i] = color2.Blue;
                        break;
                    case 2:
                        data[i] = color3.Red;
                        i++;
                        data[i] = color3.Green;
                        i++;
                        data[i] = color3.Blue;
                        break;
                    case 3:
                        data[i] = color4.Red;
                        i++;
                        data[i] = color4.Green;
                        i++;
                        data[i] = color4.Blue;
                        break;
                }

                if (ch < 3)
                    ch++;
                else
                    ch = 0;
            }

            LedPixelController.Write(data);
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

        public void Random()
        {
            var rand = new Random();
            rand.NextBytes(data);

            LedPixelController.Write(data);
        }

        public void Random(Color[] colors)
        {
            if (colors == null)
                throw new ArgumentNullException(nameof(colors));
            if (colors.Length <= 1)
                throw new ArgumentOutOfRangeException(nameof(colors));

            var countPixels = this.countPixels * 4;
            var rand = new Random();
            for (int iPixel = 0; iPixel < countPixels; iPixel++)
            {
                var index = rand.Next(colors.Length - 1);
                var color = colors[index];

                var pixelOffset = iPixel * 3;

                data[pixelOffset] = color.Red;
                data[pixelOffset + 1] = color.Green;
                data[pixelOffset + 2] = color.Blue;
            }

            LedPixelController.Write(data);
        }

        public void Colors(Color[] colors)
        {
            var part = countPixels / colors.Length;
            var diff = countPixels - part * colors.Length;

            var rowOffset = 0;
            foreach (var color in colors)
            {
                for (var iRow = 0; iRow < part; iRow++)
                {
                    var startRow = rowOffset * 4 * 3;
                    for (var iCell = 0; iCell < 4; iCell++)
                    {
                        var startCell = startRow + (iCell * 3);
                        // led 1
                        data[startCell] = color.Red;
                        data[startCell + 1] = color.Green;
                        data[startCell + 2] = color.Blue;
                    }

                    rowOffset += 1;
                }

                //rowOffset += part;
            }

            LedPixelController.Write(data);
        }
    }
}