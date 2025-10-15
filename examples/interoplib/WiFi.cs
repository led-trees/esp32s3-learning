using System.Runtime.CompilerServices;

namespace interoplib
{
    public static class WiFi
    {
        public static void SetupAP(string ssid, string password)
        {
            NativeSetup(ssid, password);
        }

        public static void Connect(string ssid, string password)
        {
            NativeConnect(ssid, password);
        }

        public static void Stop()
        {
            NativeStop();
        }

        #region Stubs

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeSetup(string ssid, string password);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeConnect(string ssid, string password);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeStop();

        #endregion
    }
}