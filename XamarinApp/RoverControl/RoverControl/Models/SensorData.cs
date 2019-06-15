using System;
using System.Collections.Generic;
using System.Text;

namespace RoverControl.Models
{
    public class SensorData
    {
        public SensorData()
        {
            accelX = 0;
            accelY = 0;
            accelZ = 0;
            magX = 0;
            magY = 0;
            magZ = 0;
        }
        public float accelX { get; set; }
        public float accelY { get; set; }
        public float accelZ { get; set; }
        public float magX { get; set; }
        public float magY { get; set; }
        public float magZ { get; set; }

        public double pitch { get; set; }
        public double roll { get; set; }
        public double yaw { get; set; }

    }
}
