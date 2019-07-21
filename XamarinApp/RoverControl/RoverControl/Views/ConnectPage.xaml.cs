using System.ComponentModel;
using Xamarin.Forms;

using nexus.protocols.ble;
using nexus.protocols.ble.scan;
using RoverControl.Services;
using System.Threading.Tasks;
using Acr.UserDialogs;

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
            using (UserDialogs.Instance.Loading("Connecting",null,null,true,MaskType.Black))
            {
                BleService.gattServer = null;
                BleService.connection = await BleService.bleAdapter.ConnectToDevice(blePeripheral);
            }
            if (BleService.connection.IsSuccessful())
            {
                BleService.gattServer = BleService.connection.GattServer;
            }

            ToastConfig toastConfig = new ToastConfig("connected to "+ blePeripheral.Advertisement.DeviceName);
            toastConfig.SetDuration(2000);
            UserDialogs.Instance.Toast(toastConfig);
        }
    }
}