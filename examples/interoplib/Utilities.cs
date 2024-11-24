﻿using System.Runtime.CompilerServices;

namespace interoplib
{
    public class Utilities
    {
        private static byte[] _hardwareSerial;

        /// <summary>
        /// Gets the hardware unique serial ID (12 bytes).
        /// </summary>
        public static byte[] HardwareSerial
        {
            get
            {
                if (_hardwareSerial == null)
                {
                    _hardwareSerial = new byte[6];
                    NativeGetHardwareSerial(_hardwareSerial);
                }

                return _hardwareSerial;
            }
        }

        #region Stubs 

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void NativeGetHardwareSerial(byte[] data);

        #endregion stubs 
    }
}