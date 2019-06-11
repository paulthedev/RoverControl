using System;
using System.Collections.Generic;
using System.Text;

namespace RoverControl.Models
{
    public class RoverCommand
    {
        public RoverCommand()
        {
            Up = 0;
            Down = 0;
            Left = 0;
            Right = 0;
            HeadLignts = 0;
            RearwheelAccleration = 0;
        }
        public int Up { get; set; }
        public int Down { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int HeadLignts { get; set; }
        public int RearwheelAccleration { get; set; }


    }
}
