using RoverControl.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoverControl.Services
{
    public class CommandService
    {
        public static RoverCommand roverCommand = new RoverCommand();

        public static void SendCommand()
        {
            BleService.WriteToDevice(serializeCommand());
        }

        private static string serializeCommand()
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
    }
}
