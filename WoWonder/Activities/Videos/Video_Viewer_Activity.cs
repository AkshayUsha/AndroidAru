using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Views;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Helpers;
using WoWonder_API.Classes.Movies;
using VideoController = WoWonder.Helpers.VideoController;

namespace WoWonder.Activities.Videos
{
    [Activity(Theme = "@style/MyTheme", ResizeableActivity = true,
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Video_Viewer_Activity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Video_Viewer_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Video_Viewer_Layout);

                var Video_id = Intent.GetStringExtra("VideoId") ?? "Data not available";
                if (Video_id != "Data not available" && !string.IsNullOrEmpty(Video_id)) Id_Video = Video_id;

                GetDataVideo();

                //Show Ads
                _adView = (AdView) FindViewById(Resource.Id.adView);
                if (Settings.Show_ADMOB_Banner)
                {
                    _adView.Visibility = ViewStates.Visible;
                    var adRequest = new AdRequest.Builder().Build();
                    _adView.LoadAd(adRequest);
                }
                else
                {
                    _adView.Pause();
                    _adView.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                if (_adView != null) _adView.Pause();
                base.OnPause();
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
                if (_adView != null) _adView.Resume();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void GetDataVideo()
        {
            try
            {
                var data = JsonConvert.DeserializeObject<Get_Movies_Object.Movie>(
                    Intent.GetStringExtra("Viewer_Video"));
                if (data != null)
                {
                    _Video = data;

                    VideoActionsController = new VideoController(this, "Viewer_Video");

                    var dbDatabase = new SqLiteDatabase();

                    var dataVideos = dbDatabase.Get_WatchOfflineVideos_ById(_Video.id);
                    if (dataVideos != null)
                        VideoActionsController.PlayVideo(dataVideos.source, dataVideos);
                    else
                        VideoActionsController.PlayVideo(_Video.source, _Video);
                    dbDatabase.Dispose();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == 2000)
                    if (resultCode == Result.Ok)
                        VideoActionsController.RestartPlayAfterShrinkScreen();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public override void OnBackPressed()
        {
            try
            {
                VideoActionsController.SetStopvideo();
                base.OnBackPressed();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
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
                VideoActionsController.SetStopvideo();
                ImageService.Instance.InvalidateMemoryCache();

                if (_adView != null) _adView.Destroy();

                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Variables Basic

        public VideoController VideoActionsController;
        public static VideoDownloadAsyncControler VideoControler;

        public static string Id_Video = "";
        public static Get_Movies_Object.Movie _Video;

        public static AdView _adView;

        #endregion
    }
}