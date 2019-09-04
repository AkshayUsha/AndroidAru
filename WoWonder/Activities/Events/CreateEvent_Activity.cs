using System;
using System.IO;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Location.Places.UI;
using Android.Icu.Text;
using Android.Icu.Util;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Com.Theartofdev.Edmodo.Cropper;
using FFImageLoading;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;
using WoWonder_API.Classes.Event;
using WoWonder_API.Classes.Global;
using WoWonder_API.Requests;
using File = Java.IO.File;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.Events
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class CreateEvent_Activity : AppCompatActivity, View.IOnClickListener
    {
        public void OnClick(View v)
        {
            try
            {
                if (v.Id == TxtStartTime.Id)
                {
                    var frag = TimePickerFragment.NewInstance(delegate(DateTime time)
                    {
                        TxtStartTime.Text = time.ToShortTimeString();
                    });

                    frag.Show(FragmentManager, TimePickerFragment.TAG);
                }
                else if (v.Id == TxtEndTime.Id)
                {
                    var frag = TimePickerFragment.NewInstance(delegate(DateTime time)
                    {
                        TxtEndTime.Text = time.ToShortTimeString();
                    });
                     
                    frag.Show(FragmentManager, TimePickerFragment.TAG);
                }
                else if (v.Id == TxtStartDate.Id)
                {
                    var frag = DatePickerFragment.NewInstance(delegate(DateTime time)
                    {
                        TxtStartDate.Text = time.ToShortDateString();
                    });
                    frag.Show(FragmentManager, DatePickerFragment.TAG);
                }
                else if (v.Id == TxtEndDate.Id)
                {
                    var frag = DatePickerFragment.NewInstance(delegate(DateTime time)
                    {
                        TxtEndDate.Text = time.ToShortDateString();
                    });
                    frag.Show(FragmentManager, DatePickerFragment.TAG);
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

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.CreateEvent_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.CreateEvent_Layout);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_Create_Events);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                TxtEventName = FindViewById<EditText>(Resource.Id.eventname);
                IconStartDate = FindViewById<TextView>(Resource.Id.StartIcondate);
                TxtStartDate = FindViewById<EditText>(Resource.Id.StartDateTextview);
                TxtStartTime = FindViewById<EditText>(Resource.Id.StartTimeTextview);
                IconEndDate = FindViewById<TextView>(Resource.Id.EndIcondate);
                TxtEndDate = FindViewById<EditText>(Resource.Id.EndDateTextview);
                TxtEndTime = FindViewById<EditText>(Resource.Id.EndTimeTextview);
                IconLocation = FindViewById<TextView>(Resource.Id.IconLocation);
                TxtLocation = FindViewById<EditText>(Resource.Id.LocationTextview);
                TxtDescription = FindViewById<EditText>(Resource.Id.description);

                ImageEvent = FindViewById<ImageViewAsync>(Resource.Id.EventCover);
                BtnImage = FindViewById<Button>(Resource.Id.btn_selectimage);

                Txt_Add = FindViewById<TextView>(Resource.Id.toolbar_title);

                IMethods.Set_TextViewIcon("1", IconStartDate, IonIcons_Fonts.AndroidTime);
                IMethods.Set_TextViewIcon("1", IconEndDate, IonIcons_Fonts.AndroidTime);
                IMethods.Set_TextViewIcon("1", IconLocation, IonIcons_Fonts.Location);

                TxtStartTime.SetOnClickListener(this);
                TxtEndTime.SetOnClickListener(this);
                TxtStartDate.SetOnClickListener(this);
                TxtEndDate.SetOnClickListener(this);
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
                Txt_Add.Click += AddButton_OnClick;
                TxtLocation.FocusChange += TxtLocation_OnFocusChange;
                BtnImage.Click += BtnImageOnClick;
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
                Txt_Add.Click -= AddButton_OnClick;
                TxtLocation.FocusChange -= TxtLocation_OnFocusChange;
                BtnImage.Click -= BtnImageOnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Api >> Create new event
        private async void AddButton_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)
                        .Show();
                }
                else
                {
                    if (string.IsNullOrEmpty(TxtEventName.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_name), ToastLength.Short).Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtStartDate.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_start_date), ToastLength.Short)
                            .Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtEndDate.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_end_date), ToastLength.Short)
                            .Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtLocation.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_Location), ToastLength.Short)
                            .Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtStartTime.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_start_time), ToastLength.Short)
                            .Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtEndTime.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_end_time), ToastLength.Short)
                            .Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtDescription.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_Description), ToastLength.Short)
                            .Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(EventPathImage))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_Image), ToastLength.Short)
                            .Show();
                    }
                    else
                    {
                        //Show a progress
                        AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading) + "...");

                        var (Api_status, respond) = await Client.Event.Create_Event(TxtEventName.Text, TxtLocation.Text,
                            TxtDescription.Text, TxtStartDate.Text, TxtEndDate.Text, TxtStartTime.Text, TxtEndTime.Text,
                            EventPathImage);
                        if (Api_status == 200)
                        {
                            if (respond is Create_Event result)
                            {
                                //Add new item to my Event list
                                var user = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);

                                MyEvent_Fragment.MyEventAdapter?.mMyEventList?.Insert(0, new Get_MyEvent_object.My_Events
                                {
                                    id = result.Event_id.ToString(),
                                    description = TxtDescription.Text,
                                    cover = EventPathImage,
                                    end_date = TxtEndDate.Text,
                                    end_time = TxtEndTime.Text,
                                    is_owner = true,
                                    location = TxtLocation.Text,
                                    name = TxtEventName.Text,
                                    start_date = TxtStartDate.Text,
                                    start_time = TxtStartTime.Text,
                                    user_data = new Get_MyEvent_object.User_Data
                                    {
                                        user_id = user?.user_id,
                                        username = user?.username,
                                        email = user?.email,
                                        first_name = user?.first_name,
                                        last_name = user?.last_name,
                                        avatar = user?.avatar,
                                        cover = user?.cover,
                                        background_image = user?.background_image,
                                        relationship_id = user?.relationship_id,
                                        address = user?.address,
                                        working = user?.working,
                                        working_link = user?.working_link,
                                        about = user?.about,
                                        school = user?.school,
                                        gender = user?.gender,
                                        birthday = user?.birthday,
                                        country_id = user?.country_id,
                                        website = user?.website,
                                        facebook = user?.facebook,
                                        google = user?.google,
                                        twitter = user?.twitter,
                                        linkedin = user?.linkedin,
                                        youtube = user?.youtube,
                                        vk = user?.vk,
                                        instagram = user?.instagram,
                                        language = user?.language,
                                        ip_address = user?.ip_address,
                                        follow_privacy = user?.follow_privacy,
                                        friend_privacy = user?.friend_privacy,
                                        post_privacy = user?.post_privacy,
                                        message_privacy = user?.message_privacy,
                                        confirm_followers = user?.confirm_followers,
                                        show_activities_privacy = user?.show_activities_privacy,
                                        birth_privacy = user?.birth_privacy,
                                        visit_privacy = user?.visit_privacy,
                                        lastseen = user?.lastseen,
                                        emailNotification = user?.emailNotification,
                                        e_liked = user?.e_liked,
                                        e_wondered = user?.e_wondered,
                                        e_shared = user?.e_shared,
                                        e_followed = user?.e_followed,
                                        e_commented = user?.e_commented,
                                        e_visited = user?.e_visited,
                                        e_liked_page = user?.e_liked_page,
                                        e_mentioned = user?.e_mentioned,
                                        e_joined_group = user?.e_joined_group,
                                        e_accepted = user?.e_accepted,
                                        e_profile_wall_post = user?.e_profile_wall_post,
                                        e_sentme_msg = user?.e_sentme_msg,
                                        e_last_notif = user?.e_last_notif,
                                        status = user?.status,
                                        active = user?.active,
                                        admin = user?.admin,
                                        registered = user?.registered,
                                        phone_number = user?.phone_number,
                                        is_pro = user?.is_pro,
                                        pro_type = user?.pro_type,
                                        timezone = user?.timezone,
                                        referrer = user?.referrer,
                                        balance = user?.balance,
                                        paypal_email = user?.paypal_email,
                                        notifications_sound = user?.notifications_sound,
                                        order_posts_by = user?.order_posts_by,
                                        device_id = user?.device_id,
                                        web_device_id = user?.web_device_id,
                                        wallet = user?.wallet,
                                        lat = user?.lat,
                                        lng = user?.lng,
                                        last_location_update = user?.last_location_update,
                                        share_my_location = user?.share_my_location,
                                        last_data_update = user?.last_data_update,
                                        last_avatar_mod = user?.last_avatar_mod,
                                        last_cover_mod = user?.last_cover_mod,
                                        avatar_full = user?.avatar_full,
                                        url = user?.url,
                                        name = user?.name,
                                        lastseen_unix_time = user?.lastseen_unix_time,
                                        lastseen_status = user?.lastseen_status
                                    }
                                });
                                AndHUD.Shared.Dismiss(this);
                                AndHUD.Shared.ShowSuccess(this);
                            }
                        }
                        else if (Api_status == 400)
                        {
                            if (respond is Error_Object error)
                            {
                                var errorText = error._errors.Error_text;
                                AndHUD.Shared.Dismiss(this);
                                Snackbar.Make(TxtDescription, errorText, Snackbar.LengthLong).Show();

                                if (errorText.Contains("Invalid or expired access_token"))
                                    API_Request.Logout(this);
                            }
                        }
                        else if (Api_status == 404)
                        {
                            var error = respond.ToString();

                            AndHUD.Shared.Dismiss(this);

                            //Show a Error
                            Snackbar.Make(TxtDescription, error, Snackbar.LengthLong).Show();
                        }

                        AndHUD.Shared.Dismiss(this);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                AndHUD.Shared.Dismiss(this);
            }
        }

        //Add Image
        private void BtnImageOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!Directory.Exists(IMethods.IPath.FolderDcimEvent))
                    Directory.CreateDirectory(IMethods.IPath.FolderDcimEvent);

                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                    //Open Image 
                    var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimEvent,
                        IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Builder()
                        .SetInitialCropWindowPaddingRatio(0)
                        .SetAutoZoomEnabled(true)
                        .SetMaxZoom(4)
                        .SetGuidelines(CropImageView.Guidelines.On)
                        .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                        .SetOutputUri(myUri).Start(this);
                }
                else
                {
                    if (CropImage.IsExplicitCameraPermissionRequired(this))
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.Camera,
                            Manifest.Permission.ReadExternalStorage
                        }, CropImage.PickImagePermissionsRequestCode);
                    }
                    else
                    {
                        //Open Image 
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimEvent,
                            IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                            .SetOutputUri(myUri).Start(this);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Add Location
        private void TxtLocation_OnFocusChange(object sender, View.FocusChangeEventArgs focusChangeEventArgs)
        {
            try
            {
                if (focusChangeEventArgs.HasFocus)
                {
                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                    {
                        try
                        {
                            var builder = new PlacePicker.IntentBuilder();
                            StartActivityForResult(builder.Build(this), 4);
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
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Exception), ToastLength.Short)
                                .Show();
                        }
                    }
                    else
                    {
                        if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted &&
                            CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                            try
                            {
                                var builder = new PlacePicker.IntentBuilder();
                                StartActivityForResult(builder.Build(this), 4);
                            }
                            catch (GooglePlayServicesRepairableException exception)
                            {
                                Crashes.TrackError(exception);
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Not_Available),
                                    ToastLength.Short).Show();
                            }
                            catch (GooglePlayServicesNotAvailableException exception)
                            {
                                Crashes.TrackError(exception);
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Not_Available),
                                    ToastLength.Short).Show();
                            }
                            catch (Exception exception)
                            {
                                Crashes.TrackError(exception);
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Exception), ToastLength.Short)
                                    .Show();
                            }
                        else
                            RequestPermissions(new[]
                            {
                                Manifest.Permission.AccessFineLocation,
                                Manifest.Permission.AccessCoarseLocation
                            }, 101);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //On Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                //If its from Camera or Gallery
                if (requestCode == CropImage.CropImageActivityRequestCode)
                {
                    var result = CropImage.GetActivityResult(data);

                    if (resultCode == Result.Ok)
                    {
                        var imageUri = CropImage.GetPickImageResultUri(this, data);

                        if (result.IsSuccessful)
                        {
                            var resultUri = result.Uri;

                            if (!string.IsNullOrEmpty(resultUri.Path))
                            {
                                EventPathImage = resultUri.Path;
                                ImageServiceLoader.Load_Image(ImageEvent, "ImagePlacholder.jpg", EventPathImage);
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong),ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long)
                                .Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long)
                            .Show();
                    }
                }
                else if (requestCode == CropImage.CropImageActivityResultErrorCode)
                {
                    var result = CropImage.GetActivityResult(data);
                    Exception error = result.Error;
                }
                else if (requestCode == 4 && resultCode == Result.Ok) // Location
                {
                    var placePicked = PlacePicker.GetPlace(this, data);

                    TxtLocation.Text = placePicked?.AddressFormatted?.ToString();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == CropImage.PickImagePermissionsRequestCode)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        //Open Image 
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimEvent,
                            IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                            .SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                    }
                }
                else if (requestCode == CropImage.CameraCapturePermissionsRequestCode)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        CropImage.StartPickImageActivity(this);
                    else
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                }
                else if (requestCode == 101)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        try
                        {
                            var builder = new PlacePicker.IntentBuilder();
                            StartActivityForResult(builder.Build(this), 4);
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
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Exception), ToastLength.Short)
                                .Show();
                        }
                    else
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
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

        public TextView IconStartDate;
        public EditText TxtEventName;
        public EditText TxtStartDate;
        public EditText TxtStartTime;
        public TextView IconEndDate;
        public EditText TxtEndDate;
        public EditText TxtEndTime;
        public TextView IconLocation;
        public EditText TxtLocation;

        public EditText TxtDescription;

        public TextView Txt_Add;

        private ImageViewAsync ImageEvent;
        private Button BtnImage;

        private string EventPathImage = "";

        #endregion
    }
}