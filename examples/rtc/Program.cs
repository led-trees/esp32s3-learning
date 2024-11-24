using System.Device.Wifi;
using System.Diagnostics;
using System.Threading;

namespace rtc
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            WifiAdapter wifi = WifiAdapter.FindAllAdapters()[0];

            wifi.AvailableNetworksChanged += WifiAdapter_AvailableNetworksChanged;
            wifi.ScanAsync();

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }

        private static void WifiAdapter_AvailableNetworksChanged(WifiAdapter sender, object e)
        {
            Debug.WriteLine("Wifi_AvailableNetworksChanged - get report");

            // Get Report of all scanned Wifi networks
            WifiNetworkReport report = sender.NetworkReport;

            // Enumerate though networks looking for our network
            foreach (WifiAvailableNetwork net in report.AvailableNetworks)
            {
                // Show all networks found
                Debug.WriteLine($"Net SSID :{net.Ssid},  BSSID : {net.Bsid},  rssi : {net.NetworkRssiInDecibelMilliwatts.ToString()},  signal : {net.SignalBars.ToString()}");

                // If its our Network then try to connect
                //if (net.Ssid == MYSSID)
                //{
                //    // Disconnect in case we are already connected
                //    sender.Disconnect();

                //    // Connect to network
                //    WifiConnectionResult result = sender.Connect(net, WifiReconnectionKind.Automatic, MYPASSWORD);

                //    // Display status
                //    if (result.ConnectionStatus == WifiConnectionStatus.Success)
                //    {
                //        Debug.WriteLine("Connected to Wifi network");
                //    }
                //    else
                //    {
                //        Debug.WriteLine($"Error {result.ConnectionStatus.ToString()} connecting o Wifi network");
                //    }
                //}
            }
        }
    }
}
