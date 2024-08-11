using System;
using System.Diagnostics;
using System.Threading;
using nanoFramework.System.IO;
using nanoFramework.System.IO.FileSystem;

namespace sdcard
{
    public class Program
    {
        static SDCard mycard;

        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            //var gpioController = new GpioController();
            //var togglersEnable = gpioController.OpenPin(11, PinMode.);

            mycard = new SDCard(new SDCard.SDCardMmcParameters { dataWidth = SDCard.SDDataWidth._1_bit, enableCardDetectPin = false });

            Debug.WriteLine("SDcard inited");

            //NativeMemory.GetMemoryInfo(NativeMemory.MemoryType.Internal, out var totalSize, out var totalFreeSize, out var largestFreeBlock);
            //Debug.WriteLine($"totalSize: {totalSize}");
            //Debug.WriteLine($"totalFreeSize: {totalFreeSize}");
            //Debug.WriteLine($"largestFreeBlock: {largestFreeBlock}");

            // Option 1 - No card detect 
            // Try to mount card
            MountMyCard();

            // Option 2 use events to mount
            // if Card detect available, enable events and mount when card inserted
            // Enable Storage events if you have Card detect on adapter 
            StorageEventManager.RemovableDeviceInserted += StorageEventManager_RemovableDeviceInserted;
            StorageEventManager.RemovableDeviceRemoved += StorageEventManager_RemovableDeviceRemoved;

            // Unmount drive
            UnMountIfMounted();

            Thread.Sleep(Timeout.Infinite);
        }

        static void UnMountIfMounted()
        {
            if (mycard.IsMounted)
                mycard.Unmount();
        }

        static bool MountMyCard()
        {
            try
            {
                mycard.Mount();
                Debug.WriteLine("Card Mounted");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Card failed to mount : {ex.Message}");
                Debug.WriteLine($"IsMounted {mycard.IsMounted}");
            }

            return false;
        }

        #region Storage Events 

        // Storage events can be used to automatically mount SD cards when inserted
        // This only works for SD card adapter that include card detect pin tied to GPIO pin
        // If no Card Detect pin then events not required

        private static void StorageEventManager_RemovableDeviceRemoved(object sender, RemovableDriveEventArgs e)
        {
            Debug.WriteLine($"Card removed - Event:{e.Event} Path:{e.Drive}");
        }

        private static void StorageEventManager_RemovableDeviceInserted(object sender, RemovableDriveEventArgs e)
        {
            Debug.WriteLine($"Card inserted - Event:{e.Event} Path:{e.Drive}");

            // Card just inserted lets try to mount it
            MountMyCard();
        }

        #endregion
    }
}
