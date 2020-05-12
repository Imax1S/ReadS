using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReadS.Droid;
using Xamarin.Essentials;

[assembly: Dependency(typeof(BaseUrl_Android))]
namespace ReadS.Droid
{
    class BaseUrl_Android:IBaseUrl
    {
        public string Get()
        {
            return "file://" + FileSystem.CacheDirectory + "/";
        }
    }
}