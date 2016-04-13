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
using SharedProject;

namespace AndroidApp
{
    public static class App
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }

    [Activity(Label = "AndroidApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ImageView _imageView;

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "CameraAppDemo");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
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

                Button pictureButton = FindViewById<Button>(Resource.Id.GetPictureButton);
                _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
                pictureButton.Click += GetPictureButton_Click;

                Button happinessButton = FindViewById<Button>(Resource.Id.GetHappinessButton);
                happinessButton.Click += GetHappinessButton_Click;
            }
        }

        private void GetPictureButton_Click(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new Java.IO.File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }

        private async void GetHappinessButton_Click(object sender, EventArgs eventArgs)
        {
            System.IO.Stream stream = System.IO.File.OpenRead(App._file.Path);

            try {
                float result = await Core.GetHappiness(stream);

                //TODO : Display result
            }
            catch (Exception ex) {
                //TODO : Display error message
            }
            finally {
                stream.Close();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume to much memory
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = _imageView.Height;
            App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
            if (App.bitmap != null)
            {
                _imageView.SetImageBitmap(App.bitmap);
                App.bitmap = null;
            }

            // Dispose of the Java side bitmap.
            GC.Collect();
        }
    }
}

