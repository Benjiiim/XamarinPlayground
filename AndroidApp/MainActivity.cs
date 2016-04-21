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
    [Activity(Label = "AndroidApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public static File _file;
        public static File _dir;
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
                _pictureButton.Text = "Take Picture";
                _resultTextView.Text = "";
                _isCaptureMode = true;
            }
        }

        private void DisplayImage()
        {
            Bitmap bitmap = BitmapFactory.DecodeFile(_file.Path);
            if (bitmap != null)
            {
                _imageView.SetImageBitmap(bitmap);
                bitmap = null;
            }

            // Dispose of the Java side bitmap.
            GC.Collect();
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            try
            {
                DisplayImage();

                float result = await Core.GetAverageHappinessScore(System.IO.File.OpenRead(_file.Path));

                _resultTextView.Text = Core.GetHappinessMessage(result);
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
}