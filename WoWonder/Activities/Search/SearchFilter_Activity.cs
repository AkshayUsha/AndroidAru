using System;
using System.Collections.Generic;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;
using WoWonder.SQLite;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.Search
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class SearchFilter_Activity : AppCompatActivity, View.IOnClickListener, MaterialDialog.IListCallback,MaterialDialog.ISingleButtonCallback, MaterialDialog.IInputCallback
    {
        public void OnInput(MaterialDialog p0, ICharSequence p1)
        {

        }

        public void OnSelection(MaterialDialog p0, View p1, int itemid, ICharSequence itemString)
        {
            try
            {
                if (TypeDialog == "Gender")
                {
                    Txt_Gender.Text = itemString.ToString();
                    Gender = itemid;
                }
                else if (TypeDialog == "ProfilePicture")
                {
                    Txt_ProfilePicture.Text = itemString.ToString();
                    ProfilePicture = itemid;
                }
                else if (TypeDialog == "Status")
                {
                    Txt_Status.Text = itemString.ToString();
                    Status = itemid;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnClick(View v)
        {
            try
            {
                if (v.Id == Txt_Gender.Id)
                {
                    TypeDialog = "Gender";

                    var arrayAdapter = new List<string>();
                    var DialogList = new MaterialDialog.Builder(this);

                    arrayAdapter.Add(GetText(Resource.String.Lbl_All));
                    arrayAdapter.Add(GetText(Resource.String.Radio_Male));
                    arrayAdapter.Add(GetText(Resource.String.Radio_Female));

                    DialogList.Title(GetText(Resource.String.Lbl_Gender));
                    DialogList.Items(arrayAdapter);
                    DialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                    DialogList.AlwaysCallSingleChoiceCallback();
                    DialogList.ItemsCallback(this).Build().Show();
                }
                else if (v.Id == Txt_ProfilePicture.Id)
                {
                    TypeDialog = "ProfilePicture";

                    var arrayAdapter = new List<string>();
                    var DialogList = new MaterialDialog.Builder(this);

                    arrayAdapter.Add(GetText(Resource.String.Lbl_All));
                    arrayAdapter.Add(GetText(Resource.String.Lbl_Yes));
                    arrayAdapter.Add(GetText(Resource.String.Lbl_No));

                    DialogList.Title(GetText(Resource.String.Lbl_Profile_Picture));
                    DialogList.Items(arrayAdapter);
                    DialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                    DialogList.AlwaysCallSingleChoiceCallback();
                    DialogList.ItemsCallback(this).Build().Show();
                }
                else if (v.Id == Txt_Status.Id)
                {
                    TypeDialog = "Status";

                    var arrayAdapter = new List<string>();
                    var DialogList = new MaterialDialog.Builder(this);

                    arrayAdapter.Add(GetText(Resource.String.Lbl_All));
                    arrayAdapter.Add(GetText(Resource.String.Lbl_Offline));
                    arrayAdapter.Add(GetText(Resource.String.Lbl_Online));

                    DialogList.Title(GetText(Resource.String.Lbl_Status));
                    DialogList.Items(arrayAdapter);
                    DialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                    DialogList.AlwaysCallSingleChoiceCallback();
                    DialogList.ItemsCallback(this).Build().Show();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);


                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.SearchFilter_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.SearchFilter_Layout);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    Title = GetText(Resource.String.Lbl_Search_Filter);
                    SetSupportActionBar(ToolBar);

                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayShowTitleEnabled(false);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                Txt_Save = FindViewById<TextView>(Resource.Id.toolbar_title);

                Txt_Gender = FindViewById<EditText>(Resource.Id.GenderPicker);
                Txt_ProfilePicture = FindViewById<EditText>(Resource.Id.ProfilePicturePicker);
                Txt_Status = FindViewById<EditText>(Resource.Id.StatusPicker);

                Txt_Gender.SetOnClickListener(this);
                Txt_ProfilePicture.SetOnClickListener(this); //wael >> Not Found 
                Txt_Status.SetOnClickListener(this); //wael >> Not Found 

                GetFilter();
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
                Txt_Save.Click += TxtSaveOnClick;
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
                Txt_Save.Click -= TxtSaveOnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void GetFilter()
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();

                var data = dbDatabase.GetSearchFilterById();
                if (data != null)
                {
                    Gender = data.Gender;
                    ProfilePicture = data.ProfilePicture;
                    Status = data.Status;

                    if (data.Gender == 0)
                        Txt_Gender.Text = GetText(Resource.String.Lbl_All);
                    else if (data.Gender == 1)
                        Txt_Gender.Text = GetText(Resource.String.Radio_Male);
                    else if (data.Gender == 2)
                        Txt_Gender.Text = GetText(Resource.String.Radio_Female);

                    if (data.ProfilePicture == 0)
                        Txt_ProfilePicture.Text = GetText(Resource.String.Lbl_All);
                    else if (data.ProfilePicture == 1)
                        Txt_ProfilePicture.Text = GetText(Resource.String.Lbl_Yes);
                    else if (data.ProfilePicture == 2)
                        Txt_ProfilePicture.Text = GetText(Resource.String.Lbl_No);

                    if (data.Status == 0)
                        Txt_Status.Text = GetText(Resource.String.Lbl_All);
                    else if (data.Status == 1)
                        Txt_Status.Text = GetText(Resource.String.Lbl_Offline);
                    else if (data.Status == 2)
                        Txt_Status.Text = GetText(Resource.String.Lbl_Online);
                }
                else
                {
                    var newSettingsFilter = new DataTables.SearchFilterTB
                    {
                        UserId = UserDetails.User_id, Gender = 0, ProfilePicture = 0, Status = 0
                    };
                    dbDatabase.InsertOrUpdate_SearchFilter(newSettingsFilter);

                    Gender = 0;
                    ProfilePicture = 0;
                    Status = 0;

                    Txt_Gender.Text = GetText(Resource.String.Lbl_All);
                    Txt_ProfilePicture.Text = GetText(Resource.String.Lbl_All);
                    Txt_Status.Text = GetText(Resource.String.Lbl_All);
                }

                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //Event Save data filter
        private void TxtSaveOnClick(object sender, EventArgs e)
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();
                var newSettingsFilter = new DataTables.SearchFilterTB
                {
                    UserId = UserDetails.User_id,
                    Gender = Gender,
                    ProfilePicture = ProfilePicture,
                    Status = Status
                };
                dbDatabase.InsertOrUpdate_SearchFilter(newSettingsFilter);
                dbDatabase.Dispose();

                // put the String to pass back into an Intent and close this activity
                var resultIntent = new Intent();
                resultIntent.PutExtra("gender", Gender.ToString());
                resultIntent.PutExtra("profilePicture", ProfilePicture.ToString());
                resultIntent.PutExtra("status", Status.ToString());
                SetResult(Result.Ok, resultIntent);
                Finish();
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

        private EditText Txt_Gender;
        private EditText Txt_ProfilePicture;
        private EditText Txt_Status;
        private TextView Txt_Save;

        private int Gender;
        private int ProfilePicture;
        private int Status;

        private string TypeDialog = "";

        #endregion
    }
}