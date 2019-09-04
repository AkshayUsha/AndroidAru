using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Gms.Common;
using Android.Gms.Location.Places.UI;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.MyProfile
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class EditMyProfile_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.EditMyProfile_layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.EditMyProfile_layout);

                var toolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolBar != null)
                {
                    SetSupportActionBar(toolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                Txt_Update = FindViewById<TextView>(Resource.Id.toolbar_title);

                Txt_firstname = FindViewById<EditText>(Resource.Id.firstnameet);
                Txt_lastname = FindViewById<EditText>(Resource.Id.lastnameet);
                Txt_Location = FindViewById<EditText>(Resource.Id.Locationet);
                Txt_phone = FindViewById<EditText>(Resource.Id.phoneet);
                Txt_website = FindViewById<EditText>(Resource.Id.websiteet);
                Txt_work = FindViewById<EditText>(Resource.Id.worket);
                Txt_school = FindViewById<EditText>(Resource.Id.schoolet);


                Get_Data_User();

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
                Txt_Update.Click += SaveData_OnClick;
                Txt_Location.FocusChange += TxtLocation_OnFocusChange;
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
                Txt_Update.Click -= SaveData_OnClick;
                Txt_Location.FocusChange -= TxtLocation_OnFocusChange;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void TxtLocation_OnFocusChange(object sender, View.FocusChangeEventArgs focusChangeEventArgs)
        {
            try
            {
                if (focusChangeEventArgs.HasFocus)
                    try
                    {
                        var builder = new PlacePicker.IntentBuilder();
                        StartActivityForResult(builder.Build(this), 3);
                    }
                    catch (GooglePlayServicesRepairableException exception)
                    {
                        Crashes.TrackError(exception);
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Not_Available), ToastLength.Short)
                            .Show();
                    }
                    catch (GooglePlayServicesNotAvailableException exception)
                    {
                        Crashes.TrackError(exception);
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Not_Available), ToastLength.Short)
                            .Show();
                    }
                    catch (Exception exception)
                    {
                        Crashes.TrackError(exception);
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Exception), ToastLength.Short).Show();
                    }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        private void Get_Data_User()
        {
            try
            {
                var local = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                if (local != null)
                {
                    Txt_firstname.Text = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(local.first_name));
                    Txt_lastname.Text = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(local.last_name));
                    Txt_Location.Text = local.address;
                    Txt_phone.Text = local.phone_number;
                    Txt_website.Text = local.website;
                    Txt_work.Text = local.working;
                    Txt_school.Text = local.school;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Save data 
        private async void SaveData_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var dictionary = new Dictionary<string, string>
                    {
                        {"first_name", Txt_firstname.Text},
                        {"last_name", Txt_lastname.Text},
                        {"address", Txt_Location.Text},
                        {"phone_number", Txt_phone.Text},
                        {"website", Txt_website.Text},
                        {"working", Txt_work.Text},
                        {"school", Txt_school.Text}
                    };
                    Settings st = new Settings();
                    var (Api_status, Respond) = await Client.Global.Update_User_Data(st,dictionary);
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
                                AndHUD.Shared.ShowError(this, result.message, MaskType.Clear, TimeSpan.FromSeconds(2));
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

                    AndHUD.Shared.Dismiss(this);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)
                        .Show();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                if (requestCode == 3 && resultCode == Result.Ok) GetPlaceFromPicker(data);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void GetPlaceFromPicker(Intent data)
        {
            try
            {
                var placePicked = PlacePicker.GetPlace(this, data);

                if (!string.IsNullOrEmpty(PlaceText))
                    PlaceText = string.Empty;

                PlaceText = placePicked?.AddressFormatted?.ToString();
                Txt_Location.Text = PlaceText;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
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

        private TextView Txt_Update;

        private EditText Txt_firstname;
        private EditText Txt_lastname;
        private EditText Txt_Location;
        private EditText Txt_phone;
        private EditText Txt_website;
        private EditText Txt_work;
        private EditText Txt_school;

        public string PlaceText;

        private AdView mAdView;

        #endregion
    }
}