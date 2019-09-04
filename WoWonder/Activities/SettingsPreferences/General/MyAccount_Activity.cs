using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
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

namespace WoWonder.Activities.SettingsPreferences.General
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class MyAccount_Activity : AppCompatActivity, View.IOnClickListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang,
                    Resource.Layout.Settings_MyAccount_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Settings_MyAccount_Layout);

                //Set ToolBar
                var toolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolBar != null)
                {
                    toolBar.Title = GetText(Resource.String.Lbl_My_Account);

                    SetSupportActionBar(toolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                //Get values
                Txt_Username_text = FindViewById<EditText>(Resource.Id.Username_text);

                Txt_Email_text = FindViewById<EditText>(Resource.Id.Email_text);
                Txt_Birthday_text = FindViewById<EditText>(Resource.Id.Birthday_text);
                Txt_Birthday_text.SetOnClickListener(this);

                Txt_Gender_icon = FindViewById<TextView>(Resource.Id.Gender_icon);
                RadioGender = FindViewById<RadioGroup>(Resource.Id.radioGender);
                RB_Male = (RadioButton) FindViewById(Resource.Id.radioMale);
                RB_Female = (RadioButton) FindViewById(Resource.Id.radioFemale);

                Txt_Save = FindViewById<TextView>(Resource.Id.toolbar_title);
               
                Get_Data_User();
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
                RB_Male.CheckedChange += RbMaleOnCheckedChange;
                RB_Female.CheckedChange += RbFemaleOnCheckedChange;
                Txt_Save.Click += SaveData_OnClick;
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
                RB_Male.CheckedChange -= RbMaleOnCheckedChange;
                RB_Female.CheckedChange -= RbFemaleOnCheckedChange;
                Txt_Save.Click -= SaveData_OnClick;
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
                IMethods.Set_TextViewIcon("1", Txt_Gender_icon, IonIcons_Fonts.Male);

                var local = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                if (local != null)
                {
                    Txt_Username_text.Text = local.username;
                    Txt_Email_text.Text = local.email;

                    try
                    {
                        DateTime date = DateTime.Parse(local.birthday);
                        string newFormat = date.Day.ToString() + "/" + date.Month.ToString() + "/" + date.Year.ToString();
                        Txt_Birthday_text.Text = newFormat;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Txt_Birthday_text.Text = local.birthday;
                    }
                    
                    if (local.gender == "male" || local.gender == "Male")
                    {
                        RB_Male.Checked = true;
                        RB_Female.Checked = false;
                        GenderStatus = "male";
                    }
                    else
                    {
                        RB_Male.Checked = false;
                        RB_Female.Checked = true;
                        GenderStatus = "female";
                    }
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

                    string newFormat = DateTime.ParseExact(Txt_Birthday_text.Text, "dd'.'MM'.'yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy");
                    var dictionary = new Dictionary<string, string>
                    {
                        {"username", Txt_Username_text.Text},
                        {"email", Txt_Email_text.Text},
                        {"birthday", newFormat},
                        {"gender", GenderStatus}
                    };

                    var (Api_status, Respond) = await Client.Global.Update_User_Data(new Settings(), dictionary);
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
                //Show a Error image with a message
                AndHUD.Shared.ShowError(this, e.Message, MaskType.Clear, TimeSpan.FromSeconds(2));
            }
        }


        private void RbFemaleOnCheckedChange(object sender,
            CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            try
            {
                var isChecked = RB_Female.Checked;
                if (isChecked)
                {
                    RB_Male.Checked = false;
                    GenderStatus = "female";
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void RbMaleOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            try
            {
                var isChecked = RB_Male.Checked;
                if (isChecked)
                {
                    RB_Female.Checked = false;
                    GenderStatus = "male";
                }
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

        private EditText Txt_Username_text;
               
        private EditText Txt_Email_text;
        private EditText Txt_Birthday_text;
               
        private TextView Txt_Gender_icon;
        private RadioGroup RadioGender;
        private RadioButton RB_Male;
        private RadioButton RB_Female;

        private TextView Txt_Save;

        public string GenderStatus = "";

        #endregion

        public void OnClick(View v)
        {
            try
            {
                if (v.Id == Txt_Birthday_text.Id)
                {
                    var frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                    {
                        Txt_Birthday_text.Text = time.ToShortDateString();
                    });
                    frag.Show(FragmentManager, DatePickerFragment.TAG);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
    }
}