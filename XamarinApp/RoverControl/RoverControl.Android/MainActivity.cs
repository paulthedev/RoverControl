using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.OS;
using nexus.protocols.ble;
using Android.Support.V4.App;
using Android;
using Android.Content;
using Acr.UserDialogs;
using System;
using Android.Util;

namespace RoverControl.Droid
{
    [Activity(Label = "RoverControl", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                string[] permissions = { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation };
                if (CheckSelfPermission(permissions[0]) != (int)Permission.Granted || CheckSelfPermission(permissions[1]) != (int)Permission.Granted)
                {
                    using (var builder = new AlertDialog.Builder(this))
                    {
                        builder.SetMessage("Often android OEMs bundle bluetooth functionality with location, bluetooth doesn't work without it. Your location is never tracked.");
                        builder.SetPositiveButton("OK", OkAction);
                        var disclaimer = builder.Create();

                        disclaimer.Show();
                    }

                    void OkAction(object sender, DialogClickEventArgs e)
                    {
                        ActivityCompat.RequestPermissions(this, permissions, 0);
                    }
                }
            }

            //Set UI to Full Screen
            SetImmersiveUI();

            //This line is required to Enable/Disable Bluetooth
            try
            {
                BluetoothLowEnergyAdapter.Init(this);
            }
            catch (Exception ex)
            {
                Log.Debug("RoverControl",ex.Message);
            }

            //Get Bluetooth adapter
            var bluetoothAdapter = BluetoothLowEnergyAdapter.ObtainDefaultAdapter(ApplicationContext);

            //Initialize ACR UserDialogs
            UserDialogs.Init(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(bluetoothAdapter));
        }

        protected override void OnResume()
        {
            //Set UI to Full Screen
            SetImmersiveUI();

            base.OnResume();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void SetImmersiveUI()
        {
            View decorView = Window.DecorView;
            var uiOptions = (int)decorView.SystemUiVisibility;
            var newUiOptions = (int)uiOptions;

            newUiOptions |= (int)SystemUiFlags.LowProfile;
            newUiOptions |= (int)SystemUiFlags.Fullscreen;
            newUiOptions |= (int)SystemUiFlags.HideNavigation;
            newUiOptions |= (int)SystemUiFlags.ImmersiveSticky;

            decorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;
        }
    }
}