using nexus.protocols.ble;
using RoverControl.Models;
using RoverControl.Services;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RoverControl.Views
{
    [DesignTimeVisible(false)]
    public partial class DrivePage : ContentPage
    {

        public static bool isTiltEnabled = false;
        private int vDuration = 10;
        private static SensorService sensor = new SensorService();
        private bool isInView = false;
        private int BattLevel = 0;
        private static bool isBatteryPolling = false;

        public DrivePage()
        {
            InitializeComponent();
            Up.BackgroundColor = Down.BackgroundColor = Left.BackgroundColor = Right.BackgroundColor = Light.BackgroundColor = Color.Transparent;
            if (DeviceDisplay.MainDisplayInfo.Height <= 1080 && (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Height) < 1.8)
            {
                Up.HeightRequest = Down.HeightRequest = Left.HeightRequest = Right.HeightRequest = 80;
                Up.WidthRequest = Down.WidthRequest = Left.WidthRequest = Right.WidthRequest = 80;
                Light.WidthRequest = 50;
                Light.HeightRequest = 70;
                Battery.HeightRequest = Battery.WidthRequest = 50;
            }
        }

        protected override void OnAppearing()
        {
            isInView = true;

            if (BleService.connection.IsSuccessful())
            {
                ReadBattery();
            }
            if (!isBatteryPolling)
            {
                Task.Run(() => StartBatteryPolling());
            }
        }

        #region Navigation
        private void Up_Pressed(object sender, EventArgs e)
        {
            Up.Source = "ArrowIconPressed";
            CommandService.roverCommand.Up = 1;
            CommandService.roverCommand.Down = 0;
            Down.IsEnabled = false;
            Vibration.Vibrate(vDuration);
            Task.Run(() => Task.Run(() => CommandService.SendCommand()));
        }

        private void Up_Released(object sender, EventArgs e)
        {
            Up.Source = "ArrowIconNormal";
            CommandService.roverCommand.Up = 0;
            Down.IsEnabled = true;
            Vibration.Vibrate(vDuration);
            Task.Run(() => CommandService.SendCommand());
        }

        private void Down_Pressed(object sender, EventArgs e)
        {
            Down.Source = "ArrowIconPressed";
            CommandService.roverCommand.Up = 0;
            CommandService.roverCommand.Down = 1;
            Up.IsEnabled = false;
            Vibration.Vibrate(vDuration);
            Task.Run(() => CommandService.SendCommand());
        }

        private void Down_Released(object sender, EventArgs e)
        {
            Down.Source = "ArrowIconNormal";
            CommandService.roverCommand.Down = 0;
            Up.IsEnabled = true;
            Vibration.Vibrate(vDuration);
            Task.Run(() => CommandService.SendCommand());
        }

        private void Right_Pressed(object sender, EventArgs e)
        {
            Right.Source = "ArrowIconPressed";
            CommandService.roverCommand.Right = 1;
            CommandService.roverCommand.Left = 0;
            Left.IsEnabled = false;
            Vibration.Vibrate(vDuration);
            Task.Run(() => CommandService.SendCommand());
        }

        private void Right_Released(object sender, EventArgs e)
        {
            Right.Source = "ArrowIconNormal";
            CommandService.roverCommand.Right = 0;
            Left.IsEnabled = true;
            Vibration.Vibrate(vDuration);
            Task.Run(() => CommandService.SendCommand());
        }

        private void Left_Pressed(object sender, EventArgs e)
        {
            Left.Source = "ArrowIconPressed";
            CommandService.roverCommand.Right = 0;
            CommandService.roverCommand.Left = 1;
            Right.IsEnabled = false;
            Vibration.Vibrate(vDuration);
            Task.Run(() => CommandService.SendCommand());
        }

        private void Left_Released(object sender, EventArgs e)
        {
            Left.Source = "ArrowIconNormal";
            CommandService.roverCommand.Left = 0;
            Right.IsEnabled = true;
            Vibration.Vibrate(vDuration);
            Task.Run(() => CommandService.SendCommand());
        }

        private void Accleration_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            CommandService.roverCommand.RearwheelAccleration = (int)e.NewValue;
        }
        #endregion


        private void Tilt_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value == true)
            {
                Right.IsEnabled = false;
                Left.IsEnabled = false;
                isTiltEnabled = true;
                Task.Run(() => sensor.ToggleSensor());
            }
            else
            {
                Right.IsEnabled = true;
                Left.IsEnabled = true;
                isTiltEnabled = false;
                Task.Run(() => sensor.ToggleSensor());

                //Reset Steering on Tilt Disable
                CommandService.roverCommand.Right = 0;
                CommandService.roverCommand.Left = 0;
                Task.Run(() => CommandService.SendCommand());
            }
        }

        private void Light_Toggle(object sender, EventArgs e)
        {
            if (CommandService.roverCommand.HeadLights == 0)
            {
                CommandService.roverCommand.HeadLights = 1;
                Vibration.Vibrate(vDuration);
                Light.Source = "LightsOn";
                Task.Run(() => CommandService.SendCommand());
            }
            else
            {
                CommandService.roverCommand.HeadLights = 0;
                Vibration.Vibrate(vDuration);
                Light.Source = "LightsOff";
                Task.Run(() => CommandService.SendCommand());
            }
        }
        private void StartBatteryPolling()
        {
            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                if (BleService.connection.IsSuccessful() && isInView)
                {
                    ReadBattery();
                    isBatteryPolling = true;
                    return true;
                }
                else
                {
                    isBatteryPolling = false;
                    return false;
                }

            });
        }

        private async void ReadBattery()
        {
            try
            {
                var msg = await BleService.ReadFromDevice();
                if (!string.IsNullOrEmpty(msg))
                {
                    int BattLevel = Convert.ToInt32(msg);
                    if (this.BattLevel == 0) // First time battery read
                    {
                        this.BattLevel = BattLevel;
                    }

                    if (Math.Abs(this.BattLevel - BattLevel) < 10)//Reading correct
                    {
                        this.BattLevel = BattLevel;
                        if (BattLevel > 60)
                        {
                            Battery.Source = "BatteryFull";
                        }
                        else if (BattLevel < 60 && BattLevel > 30)
                        {
                            Battery.Source = "BatteryMedium";
                        }
                        else if (BattLevel < 30)
                        {
                            Battery.Source = "BatteryLow";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Battery.Source = null;
                Debug.WriteLine(ex.Message);
            }
        }

        protected override void OnDisappearing()
        {
            isInView = false;
        }
    }
}