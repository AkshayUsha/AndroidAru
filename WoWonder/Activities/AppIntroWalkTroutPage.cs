using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using AppIntro;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.Tabbes;
using WoWonder.Helpers;

namespace WoWonder.Activities
{
    [Activity(Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class AppIntroWalkTroutPage : AppIntro.AppIntro
    {
        private int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                AddSlide(AppIntroFragment.NewInstance(GetString(Resource.String.Lbl_Title_page1),
                    GetString(Resource.String.Lbl_Description_page1), Resource.Drawable.Image_page1,
                    Color.ParseColor("#2d4155")));
                AddSlide(AppIntroFragment.NewInstance(GetString(Resource.String.Lbl_Title_page2),
                    GetString(Resource.String.Lbl_Description_page2), Resource.Drawable.Image_page2,
                    Color.ParseColor("#fcb840")));
                AddSlide(AppIntroFragment.NewInstance(GetString(Resource.String.Lbl_Title_page3),
                    GetString(Resource.String.Lbl_Description_page3), Resource.Drawable.Image_page3,
                    Color.ParseColor("#2485c3")));
                AddSlide(AppIntroFragment.NewInstance(GetString(Resource.String.Lbl_Title_page4),
                    GetString(Resource.String.Lbl_Description_page4), Resource.Drawable.Image_page4,
                    Color.ParseColor("#9244b1")));

                if (Settings.Walkthrough_SetFlowAnimation)
                    SetFlowAnimation();
                else if (Settings.Walkthrough_SetZoomAnimation)
                    SetZoomAnimation();
                else if (Settings.Walkthrough_SetSlideOverAnimation)
                    SetSlideOverAnimation();
                else if (Settings.Walkthrough_SetDepthAnimation)
                    SetDepthAnimation();
                else if (Settings.Walkthrough_SetFadeAnimation) SetFadeAnimation();

                //AskForPermissions( new string[]{ Manifest.Permission.Camera},3 );
                ShowStatusBar(false);
                SetBarColor(Color.ParseColor("#333639"));
                SetSeparatorColor(Color.ParseColor("#2196f3"));
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override void OnNextPressed()
        {
            try
            {
                base.OnNextPressed();

                var index = slidesNumber;
                if (count == 1)
                {
                   
                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                    {
                    }
                    else
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.ReadContacts,
                            Manifest.Permission.ReadPhoneNumbers,
                            Manifest.Permission.Camera
                        }, 208);
                    }

                    Task.Run(() =>
                    {
                        //Get data profile
                        var data = API_Request.Get_MyProfileData_Api(this).ConfigureAwait(false);
                    });

                    count++;
                }
                else if (count == 2)
                {
                    count++;
                }
                else if (count == 3)
                {
                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                    {
                    }
                    else
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.AccessFineLocation,
                            Manifest.Permission.AccessCoarseLocation
                        }, 208);
                    }

                    Task.Run(() =>
                    {
                        API_Request.is_Friend = true;
                        var data = API_Request.Get_users_friends_Async("").ConfigureAwait(false);
                    });

                    count++;

                    IMethods.AddShortcut();
                }
                else if (count == 4)
                {
                    count++;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                if (grantResults[0] == Permission.Granted)
                {
                }
                else
                {
                    //Permission Denied :(
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                        .Show();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        // Do something when users tap on Done button.
        public override void OnDonePressed()
        {
            try
            {
                StartActivity(new Intent(this, typeof(Tabbed_Main_Activity)));
                Finish();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        // Do something when users tap on Skip button.
        public override void OnSkipPressed()
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                }
                else
                {
                    RequestPermissions(new[]
                    {
                        Manifest.Permission.Camera,
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.AccessCoarseLocation
                    }, 208);
                }

                Task.Run(() =>
                {
                    //Get data profile
                    var data = API_Request.Get_MyProfileData_Api(this).ConfigureAwait(false);
                });

                StartActivity(new Intent(this, typeof(Tabbed_Main_Activity)));
                Finish();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
    }
}