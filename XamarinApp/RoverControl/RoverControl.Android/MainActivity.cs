using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using nexus.protocols.ble;

namespace RoverControl.Droid
{
    [Activity(Label = "RoverControl", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            // If you want to enable/disable the Bluetooth adapter from code, you must call this.
            BluetoothLowEnergyAdapter.Init(this);
            // Obtain the bluetooth adapter so we can pass it into our (shared-code) Xamarin Forms app. There are
            // additional Obtain() methods on BluetoothLowEnergyAdapter if you have more specific needs (e.g. if you
            // need to support devices with multiple Bluetooth adapters)
            var bluetooth = BluetoothLowEnergyAdapter.ObtainDefaultAdapter(ApplicationContext);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(bluetooth));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}