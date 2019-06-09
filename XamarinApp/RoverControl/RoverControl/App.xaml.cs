using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RoverControl.Services;
using RoverControl.Views;
using nexus.protocols.ble;

namespace RoverControl
{
    public partial class App : Application
    {

        public App(IBluetoothLowEnergyAdapter ble)
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage(ble);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
