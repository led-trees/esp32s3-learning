using System.Runtime.CompilerServices;

namespace interoplib
{
    public static class LedPixelController
    {
        public static void Init(int mosiPin, int misoPin, int clkPin, int csPin, int pixelCount, byte red, byte green, byte blue)
        {
            NativeInit(mosiPin, misoPin, clkPin, csPin, pixelCount, red, green, blue);
        }

        public static void Write(byte[] data)
        {
            NativeWrite(data);
        }

        public static void Set(byte line, ushort cell, byte red, byte green, byte blue)
        {
            NativeSet(line, cell, red, green, blue);
        }

        #region Stubs 

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeInit(int mosiPin, int misoPin, int clkPin, int csPin, int pixelCount, byte red, byte green, byte blue);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeWrite(byte[] data);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeSet(byte line, ushort cell, byte red, byte green, byte blue);

        #endregion stubs
    }
}