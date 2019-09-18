using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using nexus.core;
using nexus.protocols.ble;
using nexus.protocols.ble.gatt;
using nexus.protocols.ble.scan;

namespace RoverControl.Services
{
    public class BleService
    {
        public static IBluetoothLowEnergyAdapter bleAdapter;
        public static IBleGattServerConnection gattServer = null;
        public static BlePeripheralConnectionRequest connection;
        public static ObservableCollection<IBlePeripheral> devices = new ObservableCollection<IBlePeripheral>();

        //Stores SHA1 hashes of BT Peripheral adresses 
        private static List<string> hashTable = new List<string>();

        //GATT Service for our Rover
        //Guid is specific. Tells us this is our rover device
        private static Guid Service = Guid.Parse("adeff3c9-7d59-4470-a847-da82025400e2");
        private static Guid CommandCharacteristic = Guid.Parse("716e54c6-edd1-4732-bde1-aade233caeaa");
        private static Guid BattCharacteristic = Guid.Parse("31da16da-b080-4934-b09d-60b877186aea");


        public static async void WriteToDevice(string msg)
        {
            Debug.Write(msg);
            if (gattServer != null)
            {
                try
                {
                    var value = await gattServer.WriteCharacteristicValue(
                       Service,
                       CommandCharacteristic,
                       Encoding.UTF8.GetBytes(msg));
                }
                catch (GattException ex)
                {
                    Debug.WriteLine(ex.ToString());
                    ToastDeviceDisconnected();
                }
            }
        }

        public static async Task<string> ReadFromDevice()
        {
            string msg = "err";
            if (gattServer != null)
            {
                try
                {
                    byte[] buffer = await gattServer.ReadCharacteristicValue(
                       Service,
                       BattCharacteristic);
                    msg = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    Debug.WriteLine(msg);
                }
                catch (GattException ex)
                {
                    Debug.WriteLine(ex.ToString());
                    ToastDeviceDisconnected();
                }
            }
            return msg;
        }

        private static void ToastDeviceDisconnected()
        {
            gattServer = null;
            var toastConfig = new ToastConfig("Connection lost. Please Reconnect");
            toastConfig.SetDuration(3000);
            UserDialogs.Instance.Toast(toastConfig);
        }

        public static async Task ScanForDevices()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await bleAdapter.ScanForBroadcasts(
               // providing ScanSettings is optional
               new ScanSettings()
               {
                   // Setting the scan mode is currently only applicable to Android and has no effect on other platforms.
                   // If not provided, defaults to ScanMode.Balanced
                   Mode = ScanMode.LowPower,

                   // Optional scan filter to ensure that the observer will only receive peripherals
                   // that pass the filter. If you want to scan for everything around, omit the filter.
                   Filter = new ScanFilter()
                   {
                       // peripherals must advertise at-least-one of any GUIDs in this list
                       //This is how we know its one of our rover
                       AdvertisedServiceIsInList = new List<Guid>() { Service }
                   },

                   // ignore repeated advertisements from the same device during this scan
                   IgnoreRepeatBroadcasts = true
               },
               // Your IObserver<IBlePeripheral> or Action<IBlePeripheral> will be triggered for each discovered
               // peripheral based on the provided scan settings and filter (if any).
               (IBlePeripheral peripheral) =>
               {
                   devices.AddAll(BleService.bleAdapter.DiscoveredPeripherals.Where(p => (!string.IsNullOrEmpty(p.Advertisement.DeviceName)) && IsUniqueAddress(p.Address)));
               },
               // Provide a CancellationToken to stop the scan, or use the overload that takes a TimeSpan.
               // If you omit this argument, the scan will timeout after BluetoothLowEnergyUtils.DefaultScanTimeout
               cts.Token
            );
        }

        private static bool IsUniqueAddress(byte[] byteArray)
        {
            string hash;
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                hash = Convert.ToBase64String(sha1.ComputeHash(byteArray));
            }
            if (!hashTable.Contains(hash))
            {
                hashTable.Add(hash);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
