using nexus.protocols.ble;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RoverControl.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrivePage : ContentPage
    {
        public DrivePage(IBluetoothLowEnergyAdapter ble)
        {
            InitializeComponent();
        }
    }
}