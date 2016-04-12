using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SharedProject;
using System.IO;
using System.Linq;
using Microsoft.ProjectOxford.Emotion;

namespace AndroidApp
{
    [Activity(Label = "AndroidApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            float result = Core.GetHappiness("https://pbs.twimg.com/profile_images/719103789379284992/ufCN7Ooi.jpg").Result;
        }
    }
}

