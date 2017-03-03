using Android.App;
using Android.Widget;
using Android.OS;

namespace HDK_ANDROID
{
    [Activity(Label = "HDK_ANDROID", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }
    }
}

