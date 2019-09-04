using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using WoWonder.Helpers;

namespace WoWonder.Activities.SettingsPreferences.CleanCashed
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class CleanCashed_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            IMethods.IApp.FullScreenApp(this);
        }
    }
}