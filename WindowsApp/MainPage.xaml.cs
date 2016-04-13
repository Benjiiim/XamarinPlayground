using Microsoft.ProjectOxford.Emotion;
using SharedProject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WindowsApp
{
    public sealed partial class MainPage : Page
    {
        private MediaCapture _captureManager = null;
        private BitmapImage _bmpImage = null;
        private StorageFile _file = null;

        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await InitMediaCapture();
        }

        private async Task<bool> InitMediaCapture()
        {
            _captureManager = new MediaCapture();
            await _captureManager.InitializeAsync();

            captureElement.Source = _captureManager;
            captureElement.FlowDirection = FlowDirection.RightToLeft;

            // start capture preview
            await _captureManager.StartPreviewAsync();

            return true;
        }

        private async void GetHappinessButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                float result = await Core.GetHappiness(await _file.OpenStreamForReadAsync());

                result = result * 100;
                double score = Math.Round(result, 2);

                string displayTxt;

                if (score >= 50)
                {
                    displayTxt = score + " % :-)";
                }
                else
                {
                    displayTxt = score + "% :-(";
                }

                resultText.Text = displayTxt;
            }
            catch (Exception ex)
            {
                resultText.Text = ex.Message;
            }
        }

        private async void GetPictureButton_Click(object sender, RoutedEventArgs e)
        {
            ImageEncodingProperties imageFormat = ImageEncodingProperties.CreateJpeg();

            // create storage file in local app storage
            _file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                String.Format("myPhoto_{0}.jpg", Guid.NewGuid()),
                CreationCollisionOption.GenerateUniqueName);

            // capture to file
            await _captureManager.CapturePhotoToStorageFileAsync(imageFormat, _file);

            _bmpImage = new BitmapImage(new Uri(_file.Path));
            previewImage.Source = _bmpImage;
        }
    }
}