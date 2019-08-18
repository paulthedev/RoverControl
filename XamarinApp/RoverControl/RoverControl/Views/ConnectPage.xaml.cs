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
            if (BleService.bleAdapter.AdapterCanBeEnabled && BleService.bleAdapter.CurrentState.IsDisabledOrDisabling())
            {
                BleService.bleAdapter.EnableAdapter();
            }

            Task.Run(() => BleService.ScanForDevices());
        }

        private void DeviceList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Task.Run(() => ConnectToDevice((IBlePeripheral)e.Item));
        }

        private async Task ConnectToDevice(IBlePeripheral blePeripheral)
        {
            BleService.gattServer = null;
            ToastConfig toastConfig;

            int timeout = 5000;
            Task task = EstablishConnection(blePeripheral);
            UserDialogs.Instance.ShowLoading("Connecting", MaskType.Black, true);
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                UserDialogs.Instance.HideLoading();
                toastConfig = new ToastConfig("Connected to " + blePeripheral.Advertisement.DeviceName);
            }
            else
            {
                UserDialogs.Instance.HideLoading();
                toastConfig = new ToastConfig("Couldnot connect, is the device turned on?");
            }

            toastConfig.SetDuration(3000);
            UserDialogs.Instance.Toast(toastConfig);
        }

        private async Task EstablishConnection(IBlePeripheral blePeripheral)
        {
            BleService.connection = await BleService.bleAdapter.ConnectToDevice(blePeripheral);
            BleService.gattServer = BleService.connection.GattServer;
        }
    }
}