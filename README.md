# Microsoft Cognitive Services Emotion API with Xamarin apps

### The Challenge
In this challenge, you will use one of the [Microsoft Cognitive Services API's](https://www.microsoft.com/cognitive-services/) to bring intelligence to a Xamarin cross-platform application.

The goal is to use the [Microsoft Cognitive Services Emotion API](https://www.microsoft.com/cognitive-services/en-us/emotion-api) to get your happiness percentage after taking a picture of yourself with the device/emulator camera.

The walkthrough below should help you with the challenge, but you can also get in touch with [@benjiiim](https://twitter.com/benjiiim) or [@aspenwilder](https://twitter.com/aspenwilder) with questions--both of whom are on site at Evolve and happy to help!

The walkthrough has been written with Visual Studio 2015 Update 2 (with Xamarin tools installed) but should work in a similar way with Xamarin Studio.  It will require 24.3.4 or newer of the Android SDK.  You will be prompted to upgrade if necessary. 

### Challenge Walkthrough

##### Get a Cognitive Service Emotion API trial key
 
1. To get a trial key and be able to use the Emotion API, go to [www.microsoft.com/cognitive-services](https://www.microsoft.com/cognitive-services/), use the My account link on the top-right corner and login with a Microsoft Account (ex Live ID).

2. Click on the "Request new trials" button, choose the Emotion API product and accept the terms of service and the privacy statement to subscribe.

3. Keep one of the generated keys with you as you will need it later.

##### Create an Android blank project in Visual Studio 2015 with updates and Xamarin setup

1. With Visual Studio 2015, create a new project with the Templates > Visual C# > Android > Blank App (Android) project template. Use a name like "AndroidApp" and a new solution with a name like "XamarinCognitive". Click the OK button and the project will be created for you.

2. Add the following Nuget packages to your project (in the correct order):
    * Microsoft.Bcl.Build
    * Microsoft.ProjectOxford.Emotion

##### Design the UI layer

The UI should be composed of an ImageView, a TextView and a Button. You can create them by opening the ```Resources/layout/Main.axml``` file with the Designer or the Source view. You can copy/paste the following code to save some time:

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:orientation="vertical"
    android:layout_width="fill_parent"
    android:layout_height="fill_parent">
    <ImageView
    android:src="@android:drawable/ic_menu_gallery"
    android:layout_width="fill_parent"
    android:layout_height="300.0dp"
    android:id="@+id/imageView1"
    android:adjustViewBounds="true" />
    <TextView
    android:textAppearance="?android:attr/textAppearanceLarge"
    android:layout_width="fill_parent"
    android:layout_height="wrap_content"
    android:id="@+id/resultText"
    android:textAlignment="center" />
    <Button
    android:id="@+id/GetPictureButton"
    android:layout_width="fill_parent"
    android:layout_height="wrap_content"
    android:text="Take Picture" />
</LinearLayout>
```

Build the solution which will add the control IDs to the ```Resource.Designer.cs``` file so that you can refer to controls by name in code.

##### Get the camera stream, take a picture and send the stream to the shared code

 The logic behind the UI is quite simple.  
 The button is bound to an event, which creates the logic associated to the ActionImageCapture Android Intent.  
 The image is saved as a bitmap, rotated in portrait mode, displayed on the UI and then sent as a stream to the business logic.
 
 Create a new ```BitmapHelper``` class in the Android project and replace the code with the following:
 
```csharp
using Android.Graphics;
using Android.Media;

namespace AndroidApp
{
    public static class BitmapHelpers
    {
        public static Bitmap GetAndRotateBitmap(string fileName)
        {
            Bitmap bitmap = BitmapFactory.DecodeFile(fileName);

            // Images are being saved in landscape, so rotate them back to portrait if they were taken in portrait
            // See https://forums.xamarin.com/discussion/5409/photo-being-saved-in-landscape-not-portrait
            // See http://developer.android.com/reference/android/media/ExifInterface.html
            using (Matrix mtx = new Matrix())
            {
                ExifInterface exif = new ExifInterface(fileName);
                var orientation = (Orientation)exif.GetAttributeInt(ExifInterface.TagOrientation, (int)Orientation.Undefined);

                //TODO : handle FlipHorizontal, FlipVertical, Transpose and Transverse
                //Undefined might be an emulator issue. Taking the assumption that the picture has been taken in portrait mode
                switch (orientation)
                {
                    case Orientation.Undefined:
                    case Orientation.Rotate90:
                        mtx.PreRotate(90);
                        break;
                    case Orientation.Rotate180:
                        mtx.PreRotate(180);
                        break;
                    case Orientation.Rotate270:
                        mtx.PreRotate(270);
                        break;
                    case Orientation.Normal:
                        // Normal, do nothing
                        break;
                    default:
                        break;
                }

                if (mtx != null)
                    bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, mtx, false);
            }

            return bitmap;
        }
    }
}
```
 
 Then, replace the ```MainActivity``` class with the following code:

```csharp
[Activity(Label = "AndroidApp", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
public class MainActivity : Activity
{
    public static File _file;
    public static File _dir;
    public static Bitmap _bitmap; 
    private ImageView _imageView;
    private Button _pictureButton;
    private TextView _resultTextView; 
    private bool _isCaptureMode = true;

    private void CreateDirectoryForPictures()
    {
        _dir = new File(
            Android.OS.Environment.GetExternalStoragePublicDirectory(
                Android.OS.Environment.DirectoryPictures), "CameraAppDemo");
        if (!_dir.Exists())
        {
            _dir.Mkdirs();
        }
    }

    private bool IsThereAnAppToTakePictures()
    {
        Intent intent = new Intent(MediaStore.ActionImageCapture);
        IList<ResolveInfo> availableActivities =
            PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
        return availableActivities != null && availableActivities.Count > 0;
    }

    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);

        SetContentView(Resource.Layout.Main);

        if (IsThereAnAppToTakePictures())
        {
            CreateDirectoryForPictures();

            _pictureButton = FindViewById<Button>(Resource.Id.GetPictureButton);
            _pictureButton.Click += OnActionClick;

            _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            
            _resultTextView = FindViewById<TextView>(Resource.Id.resultText); 
        }
    }

    private void OnActionClick(object sender, EventArgs eventArgs)
    {
        if (_isCaptureMode == true)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            _file = new Java.IO.File(_dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_file));
            StartActivityForResult(intent, 0);
        }
        else
        {
            _imageView.SetImageBitmap(null);
            if (_bitmap != null)
            {
                _bitmap.Recycle();
                _bitmap.Dispose();
                _bitmap = null;
            }
            _pictureButton.Text = "Take Picture";
            _resultTextView.Text = "";
            _isCaptureMode = true;
        }
    }

    protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        try
        {
            //Get the bitmap with the right rotation
            _bitmap = BitmapHelpers.GetAndRotateBitmap(_file.Path);

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                //Get a stream
                _bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                //Get and display the happiness score
                float result = await Core.GetAverageHappinessScore(stream);
                _resultTextView.Text = Core.GetHappinessMessage(result);
            }

            //Display the image
            _imageView.SetImageBitmap(_bitmap);
        }
        catch (Exception ex)
        {
            _resultTextView.Text = ex.Message;
        }
        finally
        {
            _pictureButton.Text = "Reset";
            _isCaptureMode = false;
        }
    }
}
```

Be sure to have the following using statements:

```csharp
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Widget;
using Java.IO;
```
If you build your project, you should have an issue with the usage of "Core", which does not exist yet. This will be the next step.

##### Create a shared project with the logic to call the Cognitive Service API

Create a new project in your solution, with the Templates > Visual C# > Shared Project template. Use a name like "SharedProject".

Create a new class in the project, with the name "Core".

Copy/Paste the following code, by replacing the placeholder with your Emotion API key. This method is calling the Emotion API, through the SDK you've referenced thanks to the Microsoft.ProjectOxford.Emotion Nuget package.

```csharp
private static async Task<Emotion[]> GetHappiness(Stream stream)
{
    string emotionKey = "YourKeyHere";

    EmotionServiceClient emotionClient = new EmotionServiceClient(emotionKey);

    var emotionResults = await emotionClient.RecognizeAsync(stream);

    if (emotionResults == null || emotionResults.Count() == 0)
    {
        throw new Exception("Can't detect face");
    }

    return emotionResults;
}
```

Copy/Paste the following code in the Core class, which are utilities method to do some calculation and some formating on the API results:

```csharp
//Average happiness calculation in case of multiple people
public static async Task<float> GetAverageHappinessScore(Stream stream)
{
    Emotion[] emotionResults = await GetHappiness(stream);

    float score = 0;
    foreach (var emotionResult in emotionResults)
    {
        score = score + emotionResult.Scores.Happiness;
    }

    return score / emotionResults.Count();
}

public static string GetHappinessMessage(float score)
{
    score = score * 100;
    double result = Math.Round(score, 2);

    if (score >= 50)
        return result + " % :-)";
    else
        return result + "% :-(";
}
```

Be sure to use the following using statements in the Core class:

```csharp
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
```

You can now add a reference to your SharedProject project from you Android app, by using the wizard available after a right clicking on the "References" node in the Solution Explorer. One section of this wizard is a listing of all the Shared Projects available in your solution.

Finally, you have to add the following using statement in your MainActivity.cs class:

```csharp
using SharedProject;
```

##### Declare the right capabilities for your app

Right click on the Android Project and select Properties.  Click on the Android Manifest on the left panel and request the Required Permissions:

* CAMERA
* WRITE_EXTERNAL_STORAGE

Save the project properties.

##### Build and run the project

Build the solution and run the project thanks to the Visual Studio Emulator (pressing F5 is a good option for that).

Before playing with the app, you should tell the emulator to use your laptop camera as the emulator front-facing camera. You can do that by opening the Tools menu (">>" icon), "Camera" tab.

By clicking on the "Take a picture" button, you should launch the webcam capture. Validating the capture will send the picture to the API and display the result on the screen.

#### Bonus Challenge #1 Walkthrough

As we have used a Shared Project, the logic can be easily shared between several apps. We're going to demonstrate this by adding a Universal Windows Platform application to the solution, to target the hundreds of millions Windows 10 devices out there.

##### Create a UWP project in your solution

Create a new project in your solution, with the Templates > Visual C# > Windows > Universal > Blank App (Universal Windows) template. Use a name like "WindowsApp" and choose the appropriate target and minimum versions if prompted.

Add the following Nuget package to your project:
* Microsoft.ProjectOxford.Emotion

You can now add a reference to your SharedProject project from you UWP app, the same way you did with the Android project.

##### Design the UI layer

For this single page application, the UI layer can be built in the ```MainPage.xaml``` file. We're going to use a CaptureElement, an Image, a TextBlock and a Button for our UI. You can copy/paste the following code to save some time:

```xml
<Page x:Class="WindowsApp.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:WindowsApp"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d">
    <Grid Background="Black">
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        <CaptureElement Grid.Row="0" x:Name="captureElement" />
        <Image x:Name="previewImage" Grid.Row="0" Visibility="Collapsed"/>
        <TextBlock Grid.Row="1" x:Name="hapinessRatio"
                    Margin="12"
                    Foreground="BlueViolet"
                    FontSize="24"
                    HorizontalAlignment="Center" />
        <Button Grid.Row="2" x:Name="actionButton" 
                Content="Take Picture" 
                HorizontalAlignment="Stretch" 
                Background="BlueViolet"
                Foreground="White"
                Click="OnActionClick" />
    </Grid>
</Page>
```

##### Get the camera stream, take a picture and send the stream to the shared code

The logic behind the UI is quite simple.  
The MediaCapture is triggered during the page load, the button is bound to an event, which saves the picture as a bitmap, display it on the UI and then send it as a stream to the business logic.

Replace the ```MainPage``` class by the following code:

```csharp
public sealed partial class MainPage : Page
{
    private MediaCapture _captureManager = null;
    private BitmapImage _bmpImage = null;
    private StorageFile _file = null;
    private bool _isCaptureMode = true;

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

    private async void OnActionClick(object sender, RoutedEventArgs e)
    {
        if (_isCaptureMode == true)
        {
            await ImageCaptureAndDisplay();

            try
            {
                float result = await Core.GetAverageHappinessScore(await _file.OpenStreamForReadAsync());

                hapinessRatio.Text = Core.GetHappinessMessage(result);

                previewImage.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                hapinessRatio.Text = ex.Message;
            }
            finally
            {
                actionButton.Content = "Reset";
                _isCaptureMode = false;
            }
        }
        else
        {
            previewImage.Visibility = Visibility.Collapsed;
            actionButton.Content = "Take Picture";
            hapinessRatio.Text = "";
            _isCaptureMode = true;
        }
    }
}
```

Be sure to have the following using statements:

```csharp
using SharedProject;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
```

##### Declare the right capabilities for your app

Open the ```Package.appxmanifest``` file, and add the following Capabilities to your application, thanks to the correct checkboxes in the Capabilities tab:
* Microphone
* Webcam

##### Configure, build and run the project

In the Solution Configuration Manager (by right clicking on the solution node in the Solution Explorer), check the Build and Deploy checkboxes for your UWP project.

Build the solution and run the UWP project. You don't need an Emulator by [using Windows 10 as a development machine](https://msdn.microsoft.com/en-us/windows/uwp/get-started/enable-your-device-for-development#enable-your-windows-10-devices).

By clicking on the "Take a picture" button, you will capture a picture, send it to the API and display the result on the screen.
