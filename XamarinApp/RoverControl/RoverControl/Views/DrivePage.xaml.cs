using nexus.protocols.ble;
using nexus.protocols.ble.gatt;
using nexus.protocols.ble.scan;
using RoverControl.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RoverControl.Views
{
    [DesignTimeVisible(false)]
    public partial class DrivePage : ContentPage
    {
        private RoverCommand roverCommand = new RoverCommand();
        private int vDuration = 10;
        //GATT Service for our Rover
        //Guid is specific. Tells us this is our rover device
        public static Guid Service = Guid.Parse("adeff3c9-7d59-4470-a847-da82025400e2");
        public static Guid CommandCharacteristic = Guid.Parse("716e54c6-edd1-4732-bde1-aade233caeaa");
        public static Guid BatteryCharacteristic = Guid.Parse("0fe66e1e-d67f-49c9-ba5a-b7b1eaa0673e");

        public DrivePage()
        {
            InitializeComponent();
            Up.BackgroundColor = Down.BackgroundColor = Left.BackgroundColor = Right.BackgroundColor = Color.Transparent;
        }

        private string serializeCommand()
        {
            string rvc = "";
            rvc += roverCommand.Up;
            rvc += roverCommand.Down;
            rvc += roverCommand.Right;
            rvc += roverCommand.Left;
            rvc += roverCommand.HeadLignts;
            rvc += roverCommand.RearwheelAccleration.ToString("D3");

            return rvc;
        }

        #region Navigation
        private void Up_Pressed(object sender, EventArgs e)
        {
            roverCommand.Up = 1;
            roverCommand.Down = 0;
            Down.IsEnabled = false;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Up_Released(object sender, EventArgs e)
        {
            roverCommand.Up = 0;
            Down.IsEnabled = true;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Down_Pressed(object sender, EventArgs e)
        {
            roverCommand.Up = 0;
            roverCommand.Down = 1;
            Up.IsEnabled = false;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Down_Released(object sender, EventArgs e)
        {
            roverCommand.Down = 0;
            Up.IsEnabled = true;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Right_Pressed(object sender, EventArgs e)
        {
            roverCommand.Right = 1;
            roverCommand.Left = 0;
            Left.IsEnabled = false;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Right_Released(object sender, EventArgs e)
        {
            roverCommand.Right = 0;
            Left.IsEnabled = true;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Left_Pressed(object sender, EventArgs e)
        {
            roverCommand.Right = 0;
            roverCommand.Left = 1;
            Right.IsEnabled = false;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Left_Released(object sender, EventArgs e)
        {
            roverCommand.Left = 0;
            Right.IsEnabled = true;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Accleration_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            roverCommand.RearwheelAccleration = (int)e.NewValue;
        }
        #endregion

        private async void SendCommand()
        {
            Debug.Write(serializeCommand());
            if (ConnectPage.connection.IsSuccessful())
            {
                try
                {
                    var value = await ConnectPage.gattServer.WriteCharacteristicValue(
                       Service,
                       CommandCharacteristic,
                       Encoding.UTF8.GetBytes(serializeCommand()));
                }
                catch (GattException ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            //else
            //{
            //    await DisplayAlert(string.Empty, "Connect to a Rover and try again", "Ok");
            //}
        }
    }
}