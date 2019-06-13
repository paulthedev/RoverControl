using nexus.core;
using nexus.protocols.ble;
using nexus.protocols.ble.gatt;
using nexus.protocols.ble.scan;
using nexus.protocols.ble.scan.advertisement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RoverControl.Views
{
    [DesignTimeVisible(false)]
    public partial class ConnectPage : ContentPage
    {
        public static IBleGattServerConnection gattServer = null;
        public static BlePeripheralConnectionRequest connection;

        private static bool isScanning = false;
        private static List<string> hashTable = new List<string>();

        private static ObservableCollection<IBlePeripheral> devices = new ObservableCollection<IBlePeripheral>();
        public ConnectPage()
        {
            InitializeComponent();
            DeviceList.ItemsSource = devices;
            //DeviceList.BindingContext = "Advertisement.DeviceName";
            _ = ScanForDevices();
            DeviceList.RefreshCommand = new Command(() =>
            {
                RefreshList();
                DeviceList.IsRefreshing = false;
            });


        }

        private void RefreshList()
        {
            if (!isScanning)
                _ = ScanForDevices();
            devices.AddAll(MainPage.bleAdapter.DiscoveredPeripherals.Where(p => (!string.IsNullOrEmpty(p.Advertisement.DeviceName)) && IsUniqueAddress(p.Address)));
        }

        private bool IsUniqueAddress(byte[] byteArray)
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

        private async Task ScanForDevices()
        {
            isScanning = true;
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await MainPage.bleAdapter.ScanForBroadcasts(
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
                       //AdvertisedServiceIsInList = new List<Guid>() { DrivePage.Service },
                   },

                   // ignore repeated advertisements from the same device during this scan
                   IgnoreRepeatBroadcasts = true
               },
               // Your IObserver<IBlePeripheral> or Action<IBlePeripheral> will be triggered for each discovered
               // peripheral based on the provided scan settings and filter (if any).
               (IBlePeripheral peripheral) =>
               {
                   RefreshList();
               },
               // Provide a CancellationToken to stop the scan, or use the overload that takes a TimeSpan.
               // If you omit this argument, the scan will timeout after BluetoothLowEnergyUtils.DefaultScanTimeout
               cts.Token
            );
            isScanning = false;
        }

        private async void DeviceList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            connection = await MainPage.bleAdapter.ConnectToDevice((IBlePeripheral)e.SelectedItem);
            if (connection.IsSuccessful())
            {
                gattServer = connection.GattServer;
                await DisplayAlert(string.Empty, "Connected Established", "Ok");
            }
        }
    }
}