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

namespace WoWonder.Activities.NearBy
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class NearByFilter_Activity : AppCompatActivity, View.IOnClickListener, MaterialDialog.IListCallback,
        MaterialDialog.ISingleButtonCallback
    {
        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (TypeDialog == "Gender")
                {
                    Txt_Gender.Text = itemString.ToString();
                    Gender = itemId;
                }
                else if (TypeDialog == "Status")
                {
                    Txt_Status.Text = itemString.ToString();
                    Status = itemId;
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


                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.NearByFilter_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.NearByFilter_Layout);

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
                Txt_Distance = FindViewById<TextView>(Resource.Id.disincentive);

                Txt_Gender = FindViewById<EditText>(Resource.Id.GenderPicker);
                Txt_Status = FindViewById<EditText>(Resource.Id.StatusPicker);
                DistanceBar = FindViewById<SeekBar>(Resource.Id.distanceSeeker);

                Txt_Gender.SetOnClickListener(this);
                Txt_Status.SetOnClickListener(this);

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
                DistanceBar.ProgressChanged += DistanceBarOnProgressChanged;
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

                var data = dbDatabase.GetNearByFilterById();
                if (data != null)
                {
                    Gender = data.Gender;
                    DistanceCount = data.DistanceValue;
                    Status = data.Status;

                    if (data.Gender == 0)
                        Txt_Gender.Text = GetText(Resource.String.Lbl_All);
                    else if (data.Gender == 1)
                        Txt_Gender.Text = GetText(Resource.String.Radio_Male);
                    else if (data.Gender == 2)
                        Txt_Gender.Text = GetText(Resource.String.Radio_Female);

                    Txt_Distance.Text = GetText(Resource.String.Lbl_distance_away) + " : " + DistanceCount + " " +
                                        GetText(Resource.String.Lbl_km);
                    DistanceBar.Progress = DistanceCount;

                    if (data.Status == 0)
                        Txt_Status.Text = GetText(Resource.String.Lbl_All);
                    else if (data.Status == 1)
                        Txt_Status.Text = GetText(Resource.String.Lbl_Offline);
                    else if (data.Status == 2)
                        Txt_Status.Text = GetText(Resource.String.Lbl_Online);
                }
                else
                {
                    var newSettingsFilter = new DataTables.NearByFilterTB
                    {
                        UserId = UserDetails.User_id,
                        DistanceValue = 0,
                        Gender = 0,
                        Status = 0
                    };
                    dbDatabase.InsertOrUpdate_NearByFilter(newSettingsFilter);

                    Gender = 0;
                    DistanceCount = 0;
                    Status = 0;

                    Txt_Gender.Text = GetText(Resource.String.Lbl_All);
                    Txt_Status.Text = GetText(Resource.String.Lbl_All);
                    Txt_Distance.Text = GetText(Resource.String.Lbl_distance_away) + " : " + DistanceCount + " " +
                                        GetText(Resource.String.Lbl_km);
                    DistanceBar.Progress = DistanceCount;
                }

                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void DistanceBarOnProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            try
            {
                DistanceCount = e.Progress;
                Txt_Distance.Text = GetText(Resource.String.Lbl_distance_away) + " : " + DistanceCount + " " +
                                    GetText(Resource.String.Lbl_km);
                //DistanceBar.Progress = DistanceCount;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Save data filter
        private void TxtSaveOnClick(object sender, EventArgs e)
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();
                var newSettingsFilter = new DataTables.NearByFilterTB
                {
                    UserId = UserDetails.User_id,
                    DistanceValue = DistanceCount,
                    Gender = Gender,
                    Status = Status
                };
                dbDatabase.InsertOrUpdate_NearByFilter(newSettingsFilter);
                dbDatabase.Dispose();

                var resultIntent = new Intent();
                resultIntent.PutExtra("Gender", Gender.ToString());
                resultIntent.PutExtra("Distance", DistanceCount.ToString());
                resultIntent.PutExtra("Status", Status.ToString());
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
        private TextView Txt_Distance;
        private EditText Txt_Status;
        private TextView Txt_Save;

        private SeekBar DistanceBar;

        private int Gender;
        private int Status;
        private int DistanceCount;

        private string TypeDialog = "";

        #endregion
    }
}