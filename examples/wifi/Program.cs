using System;
using System.Diagnostics;
using System.Threading;

namespace NFApp1
{
    public class Program
    {
        private static WebServer _server;

        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            //WirelessAP.Disable();
            WirelessAP.SetWifiAp();

            Console.WriteLine($"Connected with wifi credentials. IP Address: {WirelessAP.GetIP()}");

            _server = new WebServer();
            _server.Start();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}