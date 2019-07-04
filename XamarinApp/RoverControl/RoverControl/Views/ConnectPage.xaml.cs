using System.ComponentModel;
using Xamarin.Forms;

using nexus.protocols.ble;
using nexus.protocols.ble.scan;
using RoverControl.Services;
using System.Threading.Tasks;

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
            Task.Run(() => BleService.ScanForDevices());
            DeviceList.RefreshCommand = new Command(() =>
            {
                Task.Run(() => RefreshList());
                DeviceList.IsRefreshing = false;
            });
        }

        protected override void OnAppearing()
        {
            RefreshList();
        }

        private void RefreshList()
        {
            if (!BleService.isScanning)
            {
                Task.Run(() => BleService.ScanForDevices());
            }    
        }

        private void DeviceList_ItemTapped(object sender, SelectedItemChangedEventArgs e)
        {
            Task.Run(() => ConnectToDevice((IBlePeripheral)e.SelectedItem));
        }

        private async Task ConnectToDevice(IBlePeripheral blePeripheral)
        {
            BleService.connection = await BleService.bleAdapter.ConnectToDevice(blePeripheral);
            if (BleService.connection.IsSuccessful())
            {
                BleService.gattServer = BleService.connection.GattServer;
            }
        }
    }
}