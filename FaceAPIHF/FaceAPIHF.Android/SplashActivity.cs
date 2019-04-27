using Android.App;
using Android.OS;

namespace FaceAPIHF.Droid
{
    [Activity(Label = "SplashActivity", Theme = "@style/Splash", MainLauncher = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            StartActivity(typeof(MainActivity));

            Finish();
        }
    }
}