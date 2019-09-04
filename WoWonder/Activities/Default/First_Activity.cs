using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder_API;
using IMethods = WoWonder.Helpers.IMethods;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.Default
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class First_Activity : AppCompatActivity
    {
        private CheckBox Chk_agree;
        private ImageViewAsync ImageBg;
        private View IncludeLayout;
        private RelativeLayout layoutBase;
        private Button LoginButton;
        private Button RegisterButton;
        private TextView secPrivacyTextView;

        private TextView secTermTextView;
        private Button SkipButton;
        private VideoView VideoViewer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                //View view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.First_Layout);
                SetContentView(Resource.Layout.First_Layout);

                LoginButton = FindViewById<Button>(Resource.Id.LoginButton);
                RegisterButton = FindViewById<Button>(Resource.Id.RegisterButton);

                IncludeLayout = FindViewById<View>(Resource.Id.IncludeLayout);
                VideoViewer = FindViewById<VideoView>(Resource.Id.videoView);
                secTermTextView = FindViewById<TextView>(Resource.Id.secTermTextView);
                secPrivacyTextView = FindViewById<TextView>(Resource.Id.secPrivacyTextView);
                Chk_agree = FindViewById<CheckBox>(Resource.Id.termCheckBox);
                layoutBase = FindViewById<RelativeLayout>(Resource.Id.Layout_Base);

                //Set Theme
                if (Settings.BackgroundScreenWelcomeType == "Image")
                {
                    layoutBase.SetBackgroundResource(Resource.Drawable.loginBackground);
                    IncludeLayout.Visibility = ViewStates.Gone;
                }
                else if (Settings.BackgroundScreenWelcomeType == "Video")
                {
                    var uri = Uri.Parse("android.resource://" + PackageName + "/" + Resource.Raw.MainVideo);
                    VideoViewer.SetVideoURI(uri);
                    VideoViewer.Start();
                }
                else if (Settings.BackgroundScreenWelcomeType == "Gradient")
                {
                    IncludeLayout.Visibility = ViewStates.Gone;
                    layoutBase.SetBackgroundResource(Resource.Xml.login_background_shape);
                }

                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                    {
                    }
                    else
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage
                        }, 101);
                    }
                }

                // IMethods.IApp.GetKeyHashesConfigured(this);
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
                base.OnResume();

                if (Settings.BackgroundScreenWelcomeType == "Video")
                {
                    if (!VideoViewer.IsPlaying)
                        VideoViewer.Start();

                    VideoViewer.Completion += VideoViewer_Completion;
                }

                secTermTextView.Click += SecTermTextView_Click;
                secPrivacyTextView.Click += SecPrivacyTextView_Click;
                RegisterButton.Click += RegisterButton_Click;
                LoginButton.Click += LoginButton_Click;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();

                //Close Event
                secTermTextView.Click -= SecTermTextView_Click;
                secPrivacyTextView.Click -= SecPrivacyTextView_Click;
                RegisterButton.Click -= RegisterButton_Click;
                LoginButton.Click -= LoginButton_Click;

                if (Settings.BackgroundScreenWelcomeType == "Video")
                    VideoViewer.Completion -= VideoViewer_Completion;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        protected override void OnStop()
        {
            try
            {
                base.OnStop();

                if (Settings.BackgroundScreenWelcomeType == "Video")
                    VideoViewer.StopPlayback();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }


        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 101)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                        Finish();
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void SecPrivacyTextView_Click(object sender, EventArgs e)
        {
            try
            {
                var url = Client.WebsiteUrl + "/terms/privacy-policy";
                IMethods.IApp.OpenbrowserUrl(this, url);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        private void SecTermTextView_Click(object sender, EventArgs e)
        {
            try
            {
                var url = Client.WebsiteUrl + "/terms/terms";
                IMethods.IApp.OpenbrowserUrl(this, url);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Chk_agree.Checked)
                {
                    StartActivity(new Intent(Application.Context, typeof(Register_Activity)));
                    Finish();
                }
                else
                {
                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Warning),
                        GetText(Resource.String.Lbl_You_can_not_access_your_disapproval),
                        GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Chk_agree.Checked)
                {
                    StartActivity(new Intent(Application.Context, typeof(Login_Activity)));
                    Finish();
                }
                else
                {
                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Warning),
                        GetText(Resource.String.Lbl_You_can_not_access_your_disapproval),
                        GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        private void VideoViewer_Completion(object sender, EventArgs e)
        {
            try
            {
                VideoViewer.Start();
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