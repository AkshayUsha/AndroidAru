using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using FFImageLoading;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using Plugin.CurrentActivity;
using SettingsConnecter;
using WoWonder.Activities;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Helpers;
using WoWonder.Library.OneSignal;
using WoWonder_API;
using Xamarin.Android.Net;
using Exception = System.Exception;
using IMethods = WoWonder.Helpers.IMethods;

namespace WoWonder
{
	//You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public static MainApplication instance;
        public Activity activity;

        public MainApplication(IntPtr handle, JniHandleOwnership transer):base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            try
            {
                base.OnCreate();
                //A great place to initialize Xamarin.Insights and Dependency Services!
                RegisterActivityLifecycleCallbacks(this); 

                instance = this;

                Settings st = new Settings();

                Client.WebsiteUrl = st.WebsiteUrl;
                Current.CurrentInstance.ServerKey = st.ServerKey;
                Current.CurrentInstance.SetWebsiteURL(st.WebsiteUrl, st.ServerKey);

                //Bypass Web Errors 
                //======================================
                if (Settings.TurnSecurityProtocolType3072On)
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    var client = new HttpClient(new AndroidClientHandler());
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                                                           SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                }

                if (Settings.TurnTrustFailureOn_WebException)
                {
                    //If you are Getting this error >>> System.Net.WebException: Error: TrustFailure /// then Set it to true
                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                    var b = new AesCryptoServiceProvider();
                }

                //OneSignal Notification  
                //======================================
                OneSignalNotification.RegisterNotificationDevice();

                // Check Created My Folder Or Not 
                //======================================
                IMethods.IPath.Chack_MyFolder();
                //======================================
                 
                //Init Settings
                WowTime_Main_Settings.Init();

                //Change the Lang
                WowTime_Main_Settings.SetApplicationLang(this, Settings.Lang);
                //======================================

                //App restarted after crash
                //======================================
                AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironmentOnUnhandledExceptionRaiser;

            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }
        
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            try
            {
                MyContextWrapper.Wrap(this, Settings.Lang);
                base.OnConfigurationChanged(newConfig);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
           
        }

        private void AndroidEnvironmentOnUnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            try
            {
                Intent intent = new Intent(this.activity, typeof(SpalshScreen_Activity));
                intent.AddCategory(Intent.CategoryHome);
                intent.PutExtra("crash", true);
                intent.SetAction(Intent.ActionMain);
                intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);

                PendingIntent pendingIntent = PendingIntent.GetActivity(MainApplication.GetInstance().BaseContext, 0, intent, PendingIntentFlags.OneShot);
                AlarmManager mgr = (AlarmManager)MainApplication.GetInstance().BaseContext.GetSystemService(Context.AlarmService);
                mgr.Set(AlarmType.Rtc, JavaSystem.CurrentTimeMillis() + 100, pendingIntent);

                this.activity.Finish();
                JavaSystem.Exit(2);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }
        
        public static MainApplication GetInstance()
        {
            return instance;
        }
        

        public override void OnTerminate() // on stop
        {
            try
            {
                base.OnTerminate();
                UnregisterActivityLifecycleCallbacks(this); 
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }
 
        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            try
            {
                this.activity = activity;
                CrossCurrentActivity.Current.Activity = activity;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void OnActivityDestroyed(Activity activity)
        {
            this.activity = activity;
        }

        public void OnActivityPaused(Activity activity)
        {
            this.activity = activity;
        }

        public void OnActivityResumed(Activity activity)
        {
            try
            {
                this.activity = activity;
                CrossCurrentActivity.Current.Activity = activity;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            this.activity = activity;
        }

        public void OnActivityStarted(Activity activity)
        {
            try
            {
                this.activity = activity;
                CrossCurrentActivity.Current.Activity = activity;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }           
        }

        public void OnActivityStopped(Activity activity)
        {
            this.activity = activity;
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        
    }
}