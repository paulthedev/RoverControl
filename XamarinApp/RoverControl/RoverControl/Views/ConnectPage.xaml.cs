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

        private void DeviceList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Task.Run(() => ConnectToDevice((IBlePeripheral)e.Item));
        }

        private async Task ConnectToDevice(IBlePeripheral blePeripheral)
        {
            this.IsBusy = true;
            await BleService.connection.GattServer.Disconnect();
            BleService.connection = await BleService.bleAdapter.ConnectToDevice(blePeripheral);
            this.IsBusy = false;
            if (BleService.connection.IsSuccessful())
            {
                BleService.gattServer = BleService.connection.GattServer;
                await DisplayAlert("","Connected to "+blePeripheral.Advertisement.DeviceName,"Ok");
            }
            
        }
    }
}