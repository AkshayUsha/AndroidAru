using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using Com.Facebook.Drawee.Backends.Pipeline;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.Default;
using WoWonder.Activities.Tabbes;
using Settings = SettingsConnecter.Settings;

namespace WoWonder.Activities
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/SpalchScreenTheme", NoHistory = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Locale)]
    public class SpalshScreen_Activity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                
                    View mContentView = Window.DecorView;
                    var uiOptions = (int)mContentView.SystemUiVisibility;
                    var newUiOptions = (int)uiOptions;

                    newUiOptions |= (int)SystemUiFlags.Fullscreen;
                    newUiOptions |= (int)SystemUiFlags.HideNavigation;
                    mContentView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

                    Window.AddFlags(WindowManagerFlags.Fullscreen);
                    //context.Window.RequestFeature(WindowFeatures.NoTitle);

               
                base.OnCreate(savedInstanceState);

                // Create your application here
                
                //if (Intent.GetBooleanExtra("crash", true))
                //    Toast.MakeText(this, "App restarted after crash", ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();

                //StartActivity(new Intent(Application.Context, typeof(Register_Activity)));

                var startupWork = new Task(SimulateStartup);
                startupWork.Start();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void SimulateStartup()
        {
            try
            {
                FirstRunExcute();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
         
        public void FirstRunExcute()
        {
            try
            {
                Fresco.Initialize(this);
                
                if (Settings.Show_ADMOB_Banner || Settings.Show_ADMOB_Interstitial || Settings.Show_ADMOB_RewardVideo || Settings.Show_ADMOB_Native)
                    MobileAds.Initialize(this, Settings.Ad_App_ID);

                var dbDatabase = new SqLiteDatabase();

                var result = dbDatabase.Get_data_Login_Credentials();
                if (result != null)
                {
                    if (result.Status == "Active")
                        StartActivity(new Intent(Application.Context, typeof(Tabbed_Main_Activity)));
                    else if (result.Status == "Pending")
                        StartActivity(new Intent(Application.Context, typeof(Tabbed_Main_Activity)));
                    else
                        StartActivity(new Intent(Application.Context, typeof(First_Activity)));
                }
                else
                {
                    StartActivity(new Intent(Application.Context, typeof(First_Activity)));
                }

                dbDatabase.Dispose();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                Toast.MakeText(this, exception.Message, ToastLength.Short).Show();
            }
        }
    }
}