using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using nanoFramework.Hardware.Esp32;
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

            // Option 2 use events to mount
            // if Card detect available, enable events and mount when card inserted
            // Enable Storage events if you have Card detect on adapter 
            //StorageEventManager.RemovableDeviceInserted += StorageEventManager_RemovableDeviceInserted;
            //StorageEventManager.RemovableDeviceRemoved += StorageEventManager_RemovableDeviceRemoved;

            Configuration.SetPinFunction(11, DeviceFunction.SDMMC1_CLOCK);
            Configuration.SetPinFunction(12, DeviceFunction.SDMMC1_COMMAND);
            Configuration.SetPinFunction(10, DeviceFunction.SDMMC1_D0);
            Configuration.SetPinFunction(9, DeviceFunction.SDMMC1_D1);
            Configuration.SetPinFunction(14, DeviceFunction.SDMMC1_D2);
            Configuration.SetPinFunction(13, DeviceFunction.SDMMC1_D3);

            mycard = new SDCard(new SDCardMmcParameters { dataWidth = SDCard.SDDataWidth._4_bit });

            Debug.WriteLine("SDcard inited");

            //NativeMemory.GetMemoryInfo(NativeMemory.MemoryType.Internal, out var totalSize, out var totalFreeSize, out var largestFreeBlock);
            //Debug.WriteLine($"totalSize: {totalSize}");
            //Debug.WriteLine($"totalFreeSize: {totalFreeSize}");
            //Debug.WriteLine($"largestFreeBlock: {largestFreeBlock}");

            // Option 1 - No card detect 
            // Try to mount card
            MountMyCard();

            var drivers = DriveInfo.GetDrives();

            var current = Directory.GetCurrentDirectory();
            var dirs = Directory.GetDirectories("D:\\");
            var files = Directory.GetFiles("D:\\");


            var filePath = "D:\\test.txt";
            if (File.Exists(filePath))
                File.Delete(filePath);
            File.WriteAllText(filePath, "test test");

            // Unmount drive
            UnMountIfMounted();

            Thread.Sleep(Timeout.Infinite);
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

        static void UnMountIfMounted()
        {
            if (mycard.IsMounted)
                mycard.Unmount();
        }

        #region Storage Events 

        // Storage events can be used to automatically mount SD cards when inserted
        // This only works for SD card adapter that include card detect pin tied to GPIO pin
        // If no Card Detect pin then events not required

        static void StorageEventManager_RemovableDeviceRemoved(object sender, RemovableDriveEventArgs e)
        {
            Debug.WriteLine($"Card removed - Event:{e.Event} Path:{e.Drive}");
        }

        static void StorageEventManager_RemovableDeviceInserted(object sender, RemovableDriveEventArgs e)
        {
            Debug.WriteLine($"Card inserted - Event:{e.Event} Path:{e.Drive}");

            // Card just inserted lets try to mount it
            MountMyCard();
        }

        #endregion
    }
}
