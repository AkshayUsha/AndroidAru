using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.Default;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.SettingsPreferences.General
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Password_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang,
                    Resource.Layout.Settings_Password_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Settings_Password_Layout);

                //Set ToolBar
                var toolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolBar != null)
                {
                    toolBar.Title = GetText(Resource.String.Lbl_Change_Password);

                    SetSupportActionBar(toolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }


                //Get values
                Txt_CurrentPassword = FindViewById<EditText>(Resource.Id.CurrentPassword_Edit);
                Txt_NewPassword = FindViewById<EditText>(Resource.Id.NewPassword_Edit);
                Txt_RepeatPassword = FindViewById<EditText>(Resource.Id.RepeatPassword_Edit);

                Txt_Save = FindViewById<TextView>(Resource.Id.toolbar_title);

                Txt_linkForget = FindViewById<TextView>(Resource.Id.linkText);

                //Show Ads
                mAdView = FindViewById<AdView>(Resource.Id.adView);
                if (Settings.Show_ADMOB_Banner)
                {
                    mAdView.Visibility = ViewStates.Visible;
                    var adRequest = new AdRequest.Builder().Build();
                    mAdView.LoadAd(adRequest);
                }
                else
                {
                    mAdView.Pause();
                    mAdView.Visibility = ViewStates.Invisible;
                }
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

                //Add Event
                Txt_Save.Click += SaveData_OnClick;
                Txt_linkForget.Click += TxtLinkForget_OnClick;
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
                base.OnPause();

                //Close Event
                Txt_Save.Click -= SaveData_OnClick;
                Txt_linkForget.Click -= TxtLinkForget_OnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Save data password
        private async void SaveData_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (Txt_CurrentPassword.Text == "" || Txt_NewPassword.Text == "" || Txt_RepeatPassword.Text == "")
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Please_check_your_details), ToastLength.Long)
                        .Show();
                    return;
                }

                if (Txt_NewPassword.Text != Txt_RepeatPassword.Text)
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Your_password_dont_match), ToastLength.Long)
                        .Show();
                }
                else
                {
                    if (IMethods.CheckConnectivity())
                    {
                        //Show a progress
                        AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                        if (Txt_CurrentPassword.Text != null && Txt_NewPassword.Text != null &&
                            Txt_RepeatPassword.Text != null)
                        {
                            var dataPrivacy = new Dictionary<string, string>
                            {
                                {"new_password", Txt_NewPassword.Text},
                                {"current_password", Txt_CurrentPassword.Text}
                            };

                            var (Api_status, Respond) = await Client.Global.Update_User_Data(new Settings(), dataPrivacy);
                            if (Api_status == 200)
                            {
                                if (Respond is Update_User_Data_Object result)
                                {
                                    if (result.message.Contains("updated"))
                                    {
                                        Toast.MakeText(this, result.message, ToastLength.Short).Show();
                                        AndHUD.Shared.Dismiss(this);
                                    }
                                    else
                                    {
                                        //Show a Error image with a message
                                        AndHUD.Shared.ShowError(this, result.message, MaskType.Clear,
                                            TimeSpan.FromSeconds(2));
                                    }
                                }
                            }
                            else if (Api_status == 400)
                            {
                                if (Respond is Error_Object error)
                                {
                                    var errortext = error._errors.Error_text;
                                    //Toast.MakeText(this, errortext, ToastLength.Short).Show();

                                    if (errortext.Contains("Invalid or expired access_token"))
                                        API_Request.Logout(this);
                                }
                            }
                            else if (Api_status == 404)
                            {
                                var error = Respond.ToString();
                                //Toast.MakeText(this, error, ToastLength.Short).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_Please_check_your_details),
                                ToastLength.Long).Show();
                        }

                        AndHUD.Shared.Dismiss(this);
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection),
                            ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                //Show a Error image with a message
                AndHUD.Shared.ShowError(this, exception.Message, MaskType.Clear,TimeSpan.FromSeconds(2));
            }
        }

        private void TxtLinkForget_OnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(ForgetPassword_Activity));
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
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

        #region Variables Basic

        private EditText Txt_CurrentPassword;
        private EditText Txt_NewPassword;
        private EditText Txt_RepeatPassword;
        private TextView Txt_Save;
        private TextView Txt_linkForget;

        private AdView mAdView;

        #endregion
    }
}