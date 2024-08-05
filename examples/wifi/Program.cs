using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Net.WebSockets.Server;
using System.Threading;
using nanoFramework.WebServer;

namespace NFApp1
{
    public class Program
    {
        static WebSocketServer websocketServer;

        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            //WirelessAP.Disable();
            WirelessAP.SetWifiAp();

            Console.WriteLine($"Connected with wifi credentials. IP Address: {WirelessAP.GetIP()}");

            websocketServer = new WebSocketServer(new WebSocketServerOptions()
            {
                MaxClients = 10,
                IsStandAlone = false
            });
            websocketServer.WebSocketOpened += WebSocketServer_WebSocketOpened;
            websocketServer.WebSocketClosed += WebSocketServer_WebSocketClosed;
            websocketServer.MessageReceived += WebSocketServer_MessageReceived;
            websocketServer.Start();

            //WebServer
            var webServer = new WebServer(80, HttpProtocol.Http);
            webServer.CommandReceived += WebServer_CommandReceived;
            webServer.Start();

            //The webapp url
            Debug.WriteLine($"http://{IPAddress.GetDefaultLocalAddress()}/");

            UdpClient udpClient = new() { EnableBroadcast = true };
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

        static void WebServer_CommandReceived(object obj, WebServerEventArgs e)
        {
            //check the path of the request
            if (e.Context.Request.RawUrl == "/")
            {
                //check if this is a websocket request or a page request 
                if (e.Context.Request.Headers["Upgrade"] == "websocket")
                {
                    //Upgrade to a websocket
                    websocketServer.AddWebSocket(e.Context);
                }
                else
                {
                    //var html = WebServer2.CreateMainPage("test");

                    var html = Resources.GetString(Resources.StringResources.index);

                    //Return the WebApp
                    e.Context.Response.ContentType = "text/html";
                    e.Context.Response.ContentLength64 = html.Length;
                    WebServer.OutPutStream(e.Context.Response, html);
                }
            }
            else
            {
                //Send Page not Found
                e.Context.Response.StatusCode = 404;
                WebServer.OutPutStream(e.Context.Response, "Page not Found!");
            }
        }

        private static void WebSocketServer_WebSocketOpened(object sender, WebSocketOpenedEventArgs e)
        {
            Debug.WriteLine($"connected client: {e.EndPoint}");
        }

        private static void WebSocketServer_WebSocketClosed(object sender, WebSocketClosedEventArgs e)
        {
            Debug.WriteLine($"disconnected client: {e.EndPoint}");
        }

        static void WebSocketServer_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Debug.WriteLine($"received message from: {e.Frame.EndPoint}");

            var wsServer = (WebSocketServer)sender;
            //if (e.Frame.MessageType == WebSocketMessageType.Binary && e.Frame.MessageLength == 3)
            //{
            //    //AtomLite.NeoPixel.Image.SetPixel(0, 0, Color.FromArgb(e.Frame.Buffer[0], e.Frame.Buffer[1], e.Frame.Buffer[2]));
            //    wsServer.BroadCast(e.Frame.Buffer);
            //}

            wsServer.BroadCast(e.Frame.Buffer);
        }
    }
}