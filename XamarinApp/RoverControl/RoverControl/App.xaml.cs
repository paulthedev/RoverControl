using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RoverControl.Views;
using nexus.protocols.ble;
using RoverControl.Services;
using Xamarin.Essentials;

namespace RoverControl
{
    public partial class App : Application
    {

        public App(IBluetoothLowEnergyAdapter ble)
        {
            InitializeComponent();
            BleService.bleAdapter = ble;
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            if (DrivePage.isTiltEnabled)
            {
                Accelerometer.Stop();
                Magnetometer.Stop();
            }
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            if (DrivePage.isTiltEnabled)
            {
                Accelerometer.Start(SensorService.speed);
                Magnetometer.Start(SensorService.speed);
            }
        }
    }
}
