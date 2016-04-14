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
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WindowsApp
{
    public sealed partial class MainPage : Page
    {
        private MediaCapture _captureManager = null;
        private BitmapImage _bmpImage = null;
        private StorageFile _file = null;

        private bool IsCaptureMode = true;

        public MainPage()
        {
            this.InitializeComponent();            this.Loaded += MainPage_Loaded;
        }
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await InitMediaCapture();
        }

        #region Initialization
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
        #endregion

        #region Image Capture
        private async Task<bool> ImageCaptureAndDisplay()
        { 
            ImageEncodingProperties imageFormat = ImageEncodingProperties.CreateJpeg(); 
 
            // create storage file in local app storage 
            _file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                 $"myPhoto_{Guid.NewGuid()}.jpg",
                 CreationCollisionOption.GenerateUniqueName); 
 
            // capture to file 
            await _captureManager.CapturePhotoToStorageFileAsync(imageFormat, _file);

            _bmpImage = new BitmapImage(new Uri(_file.Path));
            previewImage.Source = _bmpImage;

            return true;
        }

        #endregion

        #region Display Emotions 

        //private async Task<bool> DisplayAndSendEmotions(Emotion[] emotions)
        //{
        //    var previewProperties = _captureManager.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
        //    var previewStream = previewProperties as VideoEncodingProperties;

        //    double ratioHeight = previewImage.ActualHeight / previewStream.Height;
        //    double ratioWidth = previewImage.ActualWidth / previewStream.Width;

        //    facesLayer.Height = previewImage.ActualHeight;
        //    facesLayer.Width = previewImage.ActualWidth;

        //    facesLayer.Children.Clear();

        //    foreach (var em in emotions)
        //    {
        //        Rectangle faceBoundingBox = new Rectangle();
        //        faceBoundingBox.Width = em.FaceRectangle.Width * ratioWidth;
        //        faceBoundingBox.Height = em.FaceRectangle.Height * ratioHeight;

        //        Dictionary<string, float> scores = new Dictionary<string, float>();
        //        scores.Add("Anger", em.Scores.Anger);
        //        scores.Add("Contempt", em.Scores.Contempt);
        //        scores.Add("Disgust", em.Scores.Disgust);
        //        scores.Add("Fear", em.Scores.Fear);
        //        scores.Add("Happiness", em.Scores.Happiness);
        //        scores.Add("Neutral", em.Scores.Neutral);
        //        scores.Add("Sadness", em.Scores.Sadness);
        //        scores.Add("Surprise", em.Scores.Surprise);

        //        var emotion = scores.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

        //        faceBoundingBox.Stroke = new SolidColorBrush(Colors.White);
        //        faceBoundingBox.StrokeThickness = 2;

        //        TextBlock emotionText = new TextBlock();
        //        emotionText.Text = emotion;
        //        emotionText.Foreground = new SolidColorBrush(Colors.White);

        //        Canvas.SetLeft(faceBoundingBox, em.FaceRectangle.Left * ratioWidth);
        //        Canvas.SetTop(faceBoundingBox, em.FaceRectangle.Top * ratioHeight);

        //        Canvas.SetLeft(emotionText, (em.FaceRectangle.Left * ratioWidth));
        //        Canvas.SetTop(emotionText, (em.FaceRectangle.Top * ratioHeight) - 20);

        //        facesLayer.Children.Add(faceBoundingBox);
        //        facesLayer.Children.Add(emotionText);
        //    }

        //    return true;
        //}


        #endregion


        private async void GetHappinessButton_Click(object sender, RoutedEventArgs e)
        {
            float result = await Core.GetHappiness("https://pbs.twimg.com/profile_images/719103789379284992/ufCN7Ooi.jpg");
        }

        private async void OnActionClick(object sender, RoutedEventArgs e)
        {
            if (IsCaptureMode == true)
            {
                await ImageCaptureAndDisplay();
                float result = await Core.GetHappiness("https://pbs.twimg.com/profile_images/719103789379284992/ufCN7Ooi.jpg");
                hapinessRatio.Text = result.ToString();
                facesGrid.Visibility = Visibility.Visible;
                actionButton.Content = "Reset";
                IsCaptureMode = false;
            }
            else
            {
                facesGrid.Visibility = Visibility.Collapsed;
                actionButton.Content = "Take Picture";
                IsCaptureMode = true;
            }
        }
    }
}