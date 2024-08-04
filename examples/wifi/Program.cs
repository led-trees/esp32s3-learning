using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
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

            UdpClient udpClient = new();
            udpClient.EnableBroadcast = true;
            IPEndPoint endpointClient = new(IPAddress.Broadcast, 7223);

            byte[] buffer = new byte[1024];
            while (true)
            {
                Debug.WriteLine("send");
                udpClient.Send(buffer, endpointClient);

                Thread.Sleep(1000);
            }

            /*
            
            UDP Broadcast client

            using var udpServer = new UdpClient(new System.Net.IPEndPoint(IPAddress.Any, 7223));
            var result = await udpServer.ReceiveAsync();

             */

            //Thread.Sleep(Timeout.Infinite);
        }
    }
}