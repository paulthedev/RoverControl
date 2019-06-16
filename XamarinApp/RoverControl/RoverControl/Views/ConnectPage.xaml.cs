using System.ComponentModel;
using Xamarin.Forms;

using nexus.protocols.ble;
using nexus.protocols.ble.scan;
using RoverControl.Services;

namespace RoverControl.Views
{
    [DesignTimeVisible(false)]
    public partial class ConnectPage : ContentPage
    {
        public ConnectPage()
        {
            InitializeComponent();

            DeviceList.ItemsSource = BleService.devices;

            if (BleService.bleAdapter.AdapterCanBeEnabled && BleService.bleAdapter.CurrentState.IsDisabledOrDisabling())
            {
                BleService.bleAdapter.EnableAdapter();
            }
            _ = BleService.ScanForDevices();
            DeviceList.RefreshCommand = new Command(() =>
            {
                RefreshList();
                DeviceList.IsRefreshing = false;
            });
        }

        private void RefreshList()
        {
            if (!BleService.isScanning)
                _ = BleService.ScanForDevices();
        }

        private async void DeviceList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            BleService.connection = await BleService.bleAdapter.ConnectToDevice((IBlePeripheral)e.SelectedItem);
            if (BleService.connection.IsSuccessful())
            {
                BleService.gattServer = BleService.connection.GattServer;
            }
        }
    }
}