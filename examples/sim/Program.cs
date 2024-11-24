using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using Iot.Device.AtModem;
using Iot.Device.AtModem.DTOs;
using Iot.Device.AtModem.Modem;
using nanoFramework.Hardware.Esp32;

namespace sim
{
    public class Program
    {
        static SerialPort modemPort;

        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            modemPort = OpenSerialPort();

            var atChannel = AtChannel.Create(modemPort);
            var modem = new Sim7672(atChannel);

            modem.WaitForNetworkRegistration(CancellationToken.None);

            var connected = modem.Network.Connect(null, new AccessPointConfiguration("internet"), 10);
            if (!connected)
                modem.Network.Reconnect();

            var response = modem.HttpClient.Get("http://elka.store");
            var code = response.StatusCode;
            var html = response.Content.ReadAsString();

            modem.SmsProvider.SendSmsInTextFormat(new PhoneNumber("+79231145449"), "test");

            Thread.Sleep(Timeout.Infinite);
        }

        static SerialPort OpenSerialPort(
            string port = "COM3",
            int baudRate = 115200,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            Handshake handshake = Handshake.None,
            int dataBits = 8,
            int readTimeout = Timeout.Infinite,
            int writeTimeout = Timeout.Infinite)
        {
            // This section is specific to ESP32 targets
            // Configure GPIOs 16 and 17 to be used in UART2 (that's refered as COM3)
            Configuration.SetPinFunction(18, DeviceFunction.COM3_RX);
            Configuration.SetPinFunction(17, DeviceFunction.COM3_TX);

            var serialPort = new SerialPort(port)
            {
                //Set parameters
                BaudRate = baudRate,
                Parity = parity,
                StopBits = stopBits,
                Handshake = handshake,
                DataBits = dataBits,
                ReadTimeout = readTimeout,
                WriteTimeout = writeTimeout
            };

            // Open the serial port
            serialPort.Open();
            serialPort.NewLine = "\r\n";

            return serialPort;
        }
    }
}