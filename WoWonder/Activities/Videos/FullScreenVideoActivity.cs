using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;

namespace WoWonder.Activities.Videos
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.Keyboard | ConfigChanges.Locale |
                               ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout |
                               ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
    public class FullScreenVideoActivity : AppCompatActivity
    {
        public VideoController VideoActionsController;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                var mContentView = Window.DecorView;
                var uiOptions = (int) mContentView.SystemUiVisibility;
                var newUiOptions = uiOptions;

                newUiOptions |= (int) SystemUiFlags.LowProfile;
                newUiOptions |= (int) SystemUiFlags.Fullscreen;
                newUiOptions |= (int) SystemUiFlags.HideNavigation;
                newUiOptions |= (int) SystemUiFlags.Immersive;
                mContentView.SystemUiVisibility = (StatusBarVisibility) newUiOptions;

                var view = MyContextWrapper.GetContentView(this, Settings.Lang,
                    Resource.Layout.FullScreenDialog_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.FullScreenDialog_Layout);

                VideoActionsController = new VideoController(this, "FullScreen");
                VideoActionsController.PlayFullScreen();
                if (Intent.GetStringExtra("Downloaded") == "Downloaded")
                    VideoActionsController.Download_icon.SetImageDrawable(
                        GetDrawable(Resource.Drawable.ic_checked_red));
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        protected override void OnResume()
        {
            try
            {
                GC.Collect(0);
                base.OnResume();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }


        public override void OnBackPressed()
        {
            VideoActionsController.InitFullscreenDialog("Close");
            base.OnBackPressed();
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

        protected override void OnDestroy()
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }
    }
}