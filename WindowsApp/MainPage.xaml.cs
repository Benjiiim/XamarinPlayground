using SharedProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WindowsApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void GetWeatherButton_Click(object sender, RoutedEventArgs e)
        {
            Weather weather = Core.GetWeather(ZipCodeEdit.Text).Result;
            if (weather != null)
            {
                ResultsTitle.Text = weather.Title;
                TempText.Text = weather.Temperature;
                WindText.Text = weather.Wind;
                VisibilityText.Text = weather.Visibility;
                HumidityText.Text = weather.Humidity;
                SunriseText.Text = weather.Sunrise;
                SunsetText.Text = weather.Sunset;

                GetWeatherButton.Content = "Search Again";

            }
            else
            {
                ResultsTitle.Text = "Couldn't find any results";
            }
        }
    }
}