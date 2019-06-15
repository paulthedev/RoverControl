using RoverControl.Models;
using RoverControl.Services;
using System;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RoverControl.Views
{
    [DesignTimeVisible(false)]
    public partial class DrivePage : ContentPage
    {
        private static RoverCommand roverCommand = new RoverCommand();
        private static bool isTiltEnabled = false;
        private int vDuration = 10;

        public DrivePage()
        {
            InitializeComponent();
            Up.BackgroundColor = Down.BackgroundColor = Left.BackgroundColor = Right.BackgroundColor = Color.Transparent;
        }


        #region Navigation
        private void Up_Pressed(object sender, EventArgs e)
        {
            Up.Source = "ArrowIconPressed";
            roverCommand.Up = 1;
            roverCommand.Down = 0;
            Down.IsEnabled = false;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Up_Released(object sender, EventArgs e)
        {
            Up.Source = "ArrowIconNormal";
            roverCommand.Up = 0;
            Down.IsEnabled = true;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Down_Pressed(object sender, EventArgs e)
        {
            Down.Source = "ArrowIconPressed";
            roverCommand.Up = 0;
            roverCommand.Down = 1;
            Up.IsEnabled = false;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Down_Released(object sender, EventArgs e)
        {
            Down.Source = "ArrowIconNormal";
            roverCommand.Down = 0;
            Up.IsEnabled = true;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Right_Pressed(object sender, EventArgs e)
        {
            Right.Source = "ArrowIconPressed";
            roverCommand.Right = 1;
            roverCommand.Left = 0;
            Left.IsEnabled = false;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Right_Released(object sender, EventArgs e)
        {
            Right.Source = "ArrowIconNormal";
            roverCommand.Right = 0;
            Left.IsEnabled = true;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Left_Pressed(object sender, EventArgs e)
        {
            Left.Source = "ArrowIconPressed";
            roverCommand.Right = 0;
            roverCommand.Left = 1;
            Right.IsEnabled = false;
            Vibration.Vibrate(vDuration);
            SendCommand();
        }

        private void Left_Released(object sender, EventArgs e)
        {
            Left.Source = "ArrowIconNormal";
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

        private void SendCommand()
        {
            BleService.WriteToDevice(serializeCommand());
        }

        private void Tilt_Toggled(object sender, ToggledEventArgs e)
        {
            if(e.Value == true)
            {
                Right.IsEnabled = false;
                Left.IsEnabled = false;
                isTiltEnabled = true;
            }
            else
            {
                Right.IsEnabled = true;
                Left.IsEnabled = true;
                isTiltEnabled = false;
            }
        }
    }
}