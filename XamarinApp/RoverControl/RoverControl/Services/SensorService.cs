using RoverControl.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Essentials;

namespace RoverControl.Services
{
    public class SensorService
    {
        // Set speed delay for monitoring changes.
        public static SensorSpeed speed = SensorSpeed.UI;
        private static SensorData sensorData = new SensorData();

        public SensorService()
        {
            // Register for reading changes
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            Magnetometer.ReadingChanged += Magnetometer_ReadingChanged;
        }

        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;
            sensorData.accelX = data.Acceleration.X;
            sensorData.accelY = data.Acceleration.Y;
            sensorData.accelZ = data.Acceleration.Z;
            CalculateRotationVectors();
            if (sensorData.pitch < -17 && CommandService.roverCommand.Left == 0)
            {
                CommandService.roverCommand.Left = 1;
                CommandService.roverCommand.Right = 0;
                CommandService.SendCommand();
            }
            else if(sensorData.pitch < 17 && sensorData.pitch > -17 && (CommandService.roverCommand.Right == 1 || CommandService.roverCommand.Left == 1))
            {
                CommandService.roverCommand.Left = 0;
                CommandService.roverCommand.Right = 0;
                CommandService.SendCommand();
            }
            else if(sensorData.pitch > 17 && CommandService.roverCommand.Right == 0)
            {
                CommandService.roverCommand.Right = 1;
                CommandService.roverCommand.Left = 0;
                CommandService.SendCommand();
            }
            //Debug.WriteLine("Pitch "+sensorData.pitch+" Roll "+sensorData.roll+" Yaw "+sensorData.yaw);
        }
        void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            var data = e.Reading;
            sensorData.magX = data.MagneticField.X;
            sensorData.magY = data.MagneticField.Y;
            sensorData.magZ = data.MagneticField.Z;
            CalculateRotationVectors();
        }

        public void CalculateRotationVectors()
        {
            //Euler angle from accel
            sensorData.pitch = Math.Atan2(sensorData.accelY, (Math.Sqrt((sensorData.accelX * sensorData.accelX) + (sensorData.accelZ * sensorData.accelZ))));
            sensorData.roll = Math.Atan2(-sensorData.accelX, (Math.Sqrt((sensorData.accelY * sensorData.accelY) + (sensorData.accelZ * sensorData.accelZ))));

            // yaw from mag
            float Yh = (sensorData.magY * (float)Math.Cos(sensorData.roll)) - (sensorData.magZ * (float)Math.Sin(sensorData.roll));
            float Xh = (sensorData.magX * (float)Math.Cos(sensorData.pitch)) + (sensorData.magY * (float)Math.Sin(sensorData.roll) * (float)Math.Sin(sensorData.pitch)) + (sensorData.magZ * (float)Math.Cos(sensorData.roll) * (float)Math.Sin(sensorData.pitch));

            sensorData.yaw = Math.Atan2(Yh, Xh);


            sensorData.roll = sensorData.roll * 57.3;
            sensorData.pitch = sensorData.pitch * 57.3;
            sensorData.yaw = sensorData.yaw * 57.3;
        }

        public void ToggleSensor()
        {
            try
            {
                if (Accelerometer.IsMonitoring || Gyroscope.IsMonitoring)
                {
                    Accelerometer.Stop();
                    Magnetometer.Stop();
                }
                else
                {
                    Accelerometer.Start(speed);
                    Magnetometer.Start(speed);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }
    }
}
