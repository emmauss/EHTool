using Android.App;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using EHTool.Core;

namespace EHTool.Droid
{
    [Activity(Label = "EHTool", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                ActionBar.SetIcon(new ColorDrawable(Android.Graphics.Color.Transparent));
                ActionBar.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.SlateBlue));
                ActionBar.SetDisplayShowTitleEnabled(true);
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window.SetStatusBarColor(Android.Graphics.Color.Argb(255, 90, 77, 174));
                Window.SetNavigationBarColor(Android.Graphics.Color.SlateBlue);
            }
        }
    }
}

