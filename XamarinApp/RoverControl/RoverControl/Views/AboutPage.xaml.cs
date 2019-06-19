using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RoverControl.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }


        private async void ProjectPage_Clicked(object sender, EventArgs e)
        {
            await Browser.OpenAsync("https://github.com/paulthedev/RoverControl", BrowserLaunchMode.SystemPreferred);
        }
    }
}