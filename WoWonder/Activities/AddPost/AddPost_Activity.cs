using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Location.Places.UI;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidHUD;
using Com.Luseen.Autolinklibrary;
using Com.Sothree.Slidinguppanel;
using FFImageLoading;
using FFImageLoading.Views;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using WoWonder.Activities.AddPost.Adapters;
using WoWonder.Helpers;
using Exception = System.Exception;
using Settings = SettingsConnecter.Settings;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.AddPost
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class AddPost_Activity : AppCompatActivity, SlidingPaneLayout.IPanelSlideListener,SlidingUpPanelLayout.IPanelSlideListener, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback,MaterialDialog.IInputCallback
    {
        public void OnInput(MaterialDialog p0, ICharSequence p1)
        {
            try
            {
                if (TypeDialog == "Listening")
                {
                    if (p1.Length() > 0)
                    {
                        var strName = p1.ToString();
                        ListeningText = strName;
                        PostFeelingType = "listening"; //Type Of listening
                    }
                }
                else if (TypeDialog == "Playing")
                {
                    if (p1.Length() > 0)
                    {
                        var strName = p1.ToString();
                        PlayingText = strName;
                        PostFeelingType = "playing"; //Type Of playing
                    }
                }
                else if (TypeDialog == "Watching")
                {
                    if (p1.Length() > 0)
                    {
                        var strName = p1.ToString();
                        WatchingText = strName;
                        PostFeelingType = "watching"; //Type Of watching
                    }
                }
                else if (TypeDialog == "Traveling")
                {
                    if (p1.Length() > 0)
                    {
                        var strName = p1.ToString();
                        TravelingText = strName;
                        PostFeelingType = "traveling"; //Type Of traveling
                    }
                }

                var TextSanitizer = new TextSanitizer(MentionTextview, this);
                TextSanitizer.Load(LoadPostStrings());

                var inputManager = (InputMethodManager) GetSystemService(InputMethodService);
                inputManager.HideSoftInputFromWindow(TopToolBar.WindowToken, 0);

                TopToolBar.ClearFocus();

                SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        // Sent Api  Update_User_Data 
        public void OnSelection(MaterialDialog p0, View p1, int itemid, ICharSequence itemString)
        {
            try
            {
                if (TypeDialog == "PostPrivacy")
                {
                    PostPrivacyButton.Text = itemString.ToString();
                    PostPrivacy = itemid.ToString();

                    //var datauser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                    //if (datauser != null) datauser.post_privacy = PostPrivacy;

                    //Dictionary<string, string> dataPrivacy = new Dictionary<string, string>()
                    //{
                    //    {"post_privacy", PostPrivacy.ToString()},
                    //};

                    //var data = Client.Global.Update_User_Data(dataPrivacy).ConfigureAwait(false);
                }
                else if (TypeDialog == "Feelings")
                {
                    if (itemid == 0) // Feelings
                    {
                        StartActivityForResult(new Intent(this, typeof(Feelings_Activity)), 5);
                    }
                    else if (itemid == 1) //Listening
                    {
                        TypeDialog = "Listening";

                        var dialog = new MaterialDialog.Builder(this);

                        dialog.Title(Resource.String.Lbl_Listening);
                        dialog.Input(Resource.String.Lbl_Comment_Hint_Listening, 0, false, this);
                        dialog.InputType(InputTypes.TextFlagImeMultiLine);
                        dialog.PositiveText(GetText(Resource.String.Lbl_Submit)).OnPositive(this);
                        dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                        dialog.AlwaysCallSingleChoiceCallback();
                        dialog.Build().Show();
                    }
                    else if (itemid == 2) //Playing
                    {
                        TypeDialog = "Playing";

                        var dialog = new MaterialDialog.Builder(this);

                        dialog.Title(Resource.String.Lbl_Playing);
                        dialog.Input(Resource.String.Lbl_Comment_Hint_Playing, 0, false, this);
                        dialog.InputType(InputTypes.TextFlagImeMultiLine);
                        dialog.PositiveText(GetText(Resource.String.Lbl_Submit)).OnPositive(this);
                        dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                        dialog.AlwaysCallSingleChoiceCallback();
                        dialog.Build().Show();
                    }
                    else if (itemid == 3) //Watching
                    {
                        TypeDialog = "Watching";

                        var dialog = new MaterialDialog.Builder(this);

                        dialog.Title(Resource.String.Lbl_Watching);
                        dialog.Input(Resource.String.Lbl_Comment_Hint_Watching, 0, false, this);
                        dialog.InputType(InputTypes.TextFlagImeMultiLine);
                        dialog.PositiveText(GetText(Resource.String.Lbl_Submit)).OnPositive(this);
                        dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                        dialog.AlwaysCallSingleChoiceCallback();
                        dialog.Build().Show();
                    }
                    else if (itemid == 4) //Traveling
                    {
                        TypeDialog = "Traveling";

                        var dialog = new MaterialDialog.Builder(this);

                        dialog.Title(Resource.String.Lbl_Traveling);
                        dialog.Input(Resource.String.Lbl_Comment_Hint_Traveling, 0, false, this);
                        dialog.InputType(InputTypes.TextFlagImeMultiLine);
                        dialog.PositiveText(GetText(Resource.String.Lbl_Submit)).OnPositive(this);
                        dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                        dialog.AlwaysCallSingleChoiceCallback();
                        dialog.Build().Show();
                    }
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
                if (TypeDialog == "PostPrivacy")
                {
                    if (p1 == DialogAction.Positive) p0.Dismiss();
                }
                else if (TypeDialog == "PostBack")
                {
                    if (p1 == DialogAction.Positive)
                    {
                        p0.Dismiss();
                        Finish();
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
                else
                {
                    if (p1 == DialogAction.Positive)
                    {
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
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

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.AddPost_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.AddPost_Layout);

                //Get Intent if page or Group or Userprofile or mypage
                var datapost = Intent.GetStringExtra("Type") ?? "Data not available";
                if (datapost != "Data not available" && !string.IsNullOrEmpty(datapost)) PagePost = datapost;

                var dataid = Intent.GetStringExtra("PostId") ?? "Data not available";
                if (dataid != "Data not available" && !string.IsNullOrEmpty(dataid)) IdPost = dataid;

                TopToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (TopToolBar != null)
                {
                    TopToolBar.Title = GetText(Resource.String.Lbl_AddPost);

                    SetSupportActionBar(TopToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                Txt_AddPost = FindViewById<TextView>(Resource.Id.toolbar_title);

                Txt_ContentPost = FindViewById<EditText>(Resource.Id.editTxtEmail);
                SlidingUpPanel = FindViewById<SlidingUpPanelLayout>(Resource.Id.sliding_layout);
                Postsectionimage = FindViewById<ImageViewAsync>(Resource.Id.postsectionimage);
                PostTypeRecylerView = FindViewById<RecyclerView>(Resource.Id.Recyler);
                AttachementRecylerView = FindViewById<RecyclerView>(Resource.Id.AttachementRecyler);
                Txt_UserName = FindViewById<TextView>(Resource.Id.card_name);
                IconImage = FindViewById<ImageView>(Resource.Id.ImageIcon);
                IconHappy = FindViewById<ImageView>(Resource.Id.Activtyicon);
                IconTag = FindViewById<ImageView>(Resource.Id.TagIcon);
                IconTag.Tag = "Close";

                MentionTextview = FindViewById<AutoLinkTextView>(Resource.Id.MentionTextview);
                PostPrivacyButton = FindViewById<Button>(Resource.Id.cont);

                PostTypeRecylerView.SetLayoutManager(new LinearLayoutManager(this));
                MainPostAdapter = new MainPostAdapter(this);
                PostTypeRecylerView.SetAdapter(MainPostAdapter);

                AttachmentsAdapter = new AttachmentsAdapter(this);
                AttachementRecylerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal,
                    false));
                AttachementRecylerView.SetAdapter(AttachmentsAdapter);

                SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
                SlidingUpPanel.AddPanelSlideListener(this);

                GetPrivacyPost();
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
                PostPrivacyButton.Click += PostPrivacyButton_Click;
                MainPostAdapter.ItemClick += MainPostAdapter_ItemClick;
                Txt_AddPost.Click += TxtAddPost_OnClick;
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
                PostPrivacyButton.Click -= PostPrivacyButton_Click;
                MainPostAdapter.ItemClick -= MainPostAdapter_ItemClick;
                Txt_AddPost.Click -= TxtAddPost_OnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Add post 
        private async void TxtAddPost_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (string.IsNullOrEmpty(Txt_ContentPost.Text) && string.IsNullOrEmpty(MentionText) && AttachmentsAdapter.AttachemntsList.Count == 0)
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_YouCannot_PostanEmptyPost), ToastLength.Long).Show();
                }
                else
                {
                    if (IMethods.CheckConnectivity())
                    {
                        var conntent = Txt_ContentPost.Text + " " + MentionText;

                        //Show a progress
                        AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                        var data = await API_Request.AddNewPost_Async(IdPost, PagePost, conntent, PostPrivacy,
                            PostFeelingType, PostFeelingText, PlaceText, AttachmentsAdapter.AttachemntsList);
                        if (data)
                        {
                            AndHUD.Shared.Dismiss(this);

                            Toast.MakeText(this, GetText(Resource.String.Lbl_Post_Added), ToastLength.Short).Show();

                            RunOnUiThread(() =>
                            {
                                // put the String to pass back into an Intent and close this activity
                                var resultIntent = new Intent();
                                SetResult(Result.Canceled, resultIntent);
                            });

                            Finish();
                        }
                        else
                        {
                            //Show a Error image with a message
                            AndHUD.Shared.ShowError(this, GetText(Resource.String.Lbl_Post_Failed), MaskType.Clear,
                                TimeSpan.FromSeconds(2));
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection),
                            ToastLength.Short).Show();
                    }

                    AndHUD.Shared.Dismiss(this);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                 //Show a Error image with a message
                            AndHUD.Shared.ShowError(this, GetText(Resource.String.Lbl_Post_Failed), MaskType.Clear,
                                TimeSpan.FromSeconds(2));
            }
        }

        public string LoadPostStrings()
        {
            try
            {
                var newActivityText = string.Empty;
                var newFeelingText = string.Empty;
                var newMentionText = string.Empty;
                var newPlaceText = string.Empty;

                if (!string.IsNullOrEmpty(ActivityText))
                    newActivityText = PostActivityType + " " + ActivityText;

                if (!string.IsNullOrEmpty(ListeningText))
                    newFeelingText = GetText(Resource.String.Lbl_ListeningTo) + " " + ListeningText;

                if (!string.IsNullOrEmpty(PlayingText))
                    newFeelingText = GetText(Resource.String.Lbl_Playing) + " " + PlayingText;

                if (!string.IsNullOrEmpty(WatchingText))
                    newFeelingText = GetText(Resource.String.Lbl_Watching) + " " + WatchingText;

                if (!string.IsNullOrEmpty(TravelingText))
                    newFeelingText = GetText(Resource.String.Lbl_Traveling) + " " + TravelingText;

                if (!string.IsNullOrEmpty(FeelingText))
                    newFeelingText = GetText(Resource.String.Lbl_Feeling) + " " + FeelingText;

                if (!string.IsNullOrEmpty(MentionText))
                    newMentionText += " " + GetText(Resource.String.Lbl_With) + " " +
                                      MentionText.Remove(MentionText.Length - 1, 1);

                if (!string.IsNullOrEmpty(PlaceText))
                    newPlaceText += " " + GetText(Resource.String.Lbl_At) + " " + PlaceText;

                var mainString = newActivityText + newFeelingText + newMentionText + newPlaceText;
                return mainString;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return "";
            }
        }

        //Event Open Type page post
        private void MainPostAdapter_ItemClick(object sender, MainPostAdapterClickEventArgs e)
        {
            try
            {
                if (MainPostAdapter.PostTypeList[e.Position] != null)
                {
                    if (MainPostAdapter.PostTypeList[e.Position].ID == 1) //Image Gallery
                    {
                        // Check if we're running on Android 5.0 or higher
                        if ((int) Build.VERSION.SdkInt < 23)
                        {
                            var galleryIntent = new Intent(Intent.ActionPick);
                            galleryIntent.SetAction(Intent.ActionGetContent);
                            galleryIntent.SetType("image/*");
                            galleryIntent.PutExtra(Intent.ExtraAllowMultiple, true);
                            StartActivityForResult(
                                Intent.CreateChooser(galleryIntent, GetText(Resource.String.Lbl_SelectPictures)), 1);
                        }
                        else
                        {
                            if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted &&
                                CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted)
                            {
                                var galleryIntent = new Intent(Intent.ActionPick);
                                galleryIntent.SetAction(Intent.ActionGetContent);
                                galleryIntent.SetType("image/*");
                                galleryIntent.PutExtra(Intent.ExtraAllowMultiple, true);
                                StartActivityForResult(
                                    Intent.CreateChooser(galleryIntent, GetText(Resource.String.Lbl_SelectPictures)),
                                    1);
                            }
                            else
                            {
                                RequestPermissions(new[]
                                {
                                    Manifest.Permission.Camera,
                                    Manifest.Permission.ReadExternalStorage
                                }, 1);
                            }
                        }
                    }
                    else if (MainPostAdapter.PostTypeList[e.Position].ID == 2) //video Gallery
                    {
                        // Check if we're running on Android 5.0 or higher
                        if ((int) Build.VERSION.SdkInt < 23)
                        {
                            var galleryIntent = new Intent(Intent.ActionPick);
                            galleryIntent.SetAction(Intent.ActionGetContent);
                            galleryIntent.SetType("video/*");
                            StartActivityForResult(galleryIntent, 2);
                        }
                        else
                        {
                            if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted &&
                                CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted)
                            {
                                var galleryIntent = new Intent(Intent.ActionPick);
                                galleryIntent.SetAction(Intent.ActionGetContent);
                                galleryIntent.SetType("video/*");
                                StartActivityForResult(galleryIntent, 2);
                            }
                            else
                            {
                                RequestPermissions(new[]
                                {
                                    Manifest.Permission.Camera,
                                    Manifest.Permission.ReadExternalStorage
                                }, 2);
                            }
                        }
                    }
                    else if (MainPostAdapter.PostTypeList[e.Position].ID == 3) // Mention
                    {
                        StartActivityForResult(new Intent(this, typeof(Mention_Activity)), 3);
                    }
                    else if (MainPostAdapter.PostTypeList[e.Position].ID == 4) // Location
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
                                    Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Exception),
                                        ToastLength.Short).Show();
                                }
                            else
                                RequestPermissions(new[]
                                {
                                    Manifest.Permission.AccessFineLocation,
                                    Manifest.Permission.AccessCoarseLocation
                                }, 4);
                        }
                    }
                    else if (MainPostAdapter.PostTypeList[e.Position].ID == 5) // Feeling
                    {
                        //StartActivityForResult(new Intent(this, typeof(Feelings_Activity)), 5);
                        try
                        {
                            TypeDialog = "Feelings";

                            var arrayAdapter = new List<string>();
                            var DialogList = new MaterialDialog.Builder(this);

                            if (Settings.Show_Feeling)
                                arrayAdapter.Add(GetText(Resource.String.Lbl_Feeling));
                            if (Settings.Show_Listening)
                                arrayAdapter.Add(GetText(Resource.String.Lbl_Listening));
                            if (Settings.Show_Playing)
                                arrayAdapter.Add(GetText(Resource.String.Lbl_Playing));
                            if (Settings.Show_Watching)
                                arrayAdapter.Add(GetText(Resource.String.Lbl_Watching));
                            if (Settings.Show_Traveling)
                                arrayAdapter.Add(GetText(Resource.String.Lbl_Traveling));

                            DialogList.Title(GetString(Resource.String.Lbl_What_Are_You_Doing));
                            DialogList.Items(arrayAdapter);
                            DialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                            DialogList.AlwaysCallSingleChoiceCallback();
                            DialogList.ItemsCallback(this).Build().Show();
                        }
                        catch (Exception exception)
                        {
                            Crashes.TrackError(exception);
                        }
                    }
                    //else if (MainPostAdapter.PostTypeList[e.Position].ID == 6) // Activity
                    //{
                    //    StartActivityForResult(new Intent(this, typeof(PostActivites_Activity)), 6);
                    //}
                    else if (MainPostAdapter.PostTypeList[e.Position].ID == 6) // Camera
                    {
                        // Check if we're running on Android 5.0 or higher
                        if ((int) Build.VERSION.SdkInt < 23)
                        {
                            if (IMethods.MultiMedia.IsCameraAvailable())
                            {
                                var cameraIntent = new Intent(MediaStore.ActionImageCapture);
                                StartActivityForResult(cameraIntent, 6);
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Camera_Not_Available),
                                    ToastLength.Short).Show();
                            }
                        }
                        else
                        {
                            if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted &&
                                CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted)
                            {
                                if (IMethods.MultiMedia.IsCameraAvailable())
                                {
                                    var cameraIntent = new Intent(MediaStore.ActionImageCapture);
                                    StartActivityForResult(cameraIntent, 6);
                                }
                                else
                                {
                                    Toast.MakeText(this, GetText(Resource.String.Lbl_Camera_Not_Available),
                                        ToastLength.Short).Show();
                                }
                            }
                            else
                            {
                                RequestPermissions(new[]
                                {
                                    Manifest.Permission.Camera,
                                    Manifest.Permission.ReadExternalStorage
                                }, 6);
                            }
                        }
                    }
                    else if (MainPostAdapter.PostTypeList[e.Position].ID == 7) // Gif
                    {
                        StartActivityForResult(new Intent(this, typeof(Gif_Activity)), 7);
                    }
                    else if (MainPostAdapter.PostTypeList[e.Position].ID == 8) // File
                    {
                        // Check if we're running on Android 5.0 or higher
                        if ((int) Build.VERSION.SdkInt < 23)
                        {
                            var fileIntent = new Intent(Intent.ActionPick);
                            fileIntent.SetAction(Intent.ActionGetContent);
                            fileIntent.SetType("*/*");
                            StartActivityForResult(
                                Intent.CreateChooser(fileIntent, GetText(Resource.String.Lbl_SelectFile)), 8);
                        }
                        else
                        {
                            if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                                CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                            {
                                var fileIntent = new Intent(Intent.ActionPick);
                                fileIntent.SetAction(Intent.ActionGetContent);
                                fileIntent.SetType("*/*");
                                StartActivityForResult(
                                    Intent.CreateChooser(fileIntent, GetText(Resource.String.Lbl_SelectFile)), 8);
                            }
                            else
                            {
                                RequestPermissions(new[]
                                {
                                    Manifest.Permission.ReadExternalStorage,
                                    Manifest.Permission.WriteExternalStorage
                                }, 8);
                            }
                        }
                    }
                    else if (MainPostAdapter.PostTypeList[e.Position].ID == 9) // Music
                    {
                        // Check if we're running on Android 5.0 or higher
                        if ((int) Build.VERSION.SdkInt < 23)
                        {
                            var intent = new Intent(Intent.ActionPick);
                            intent.SetAction(Intent.ActionView);
                            intent.SetType("audio/*");
                            StartActivityForResult(intent, 9);
                        }
                        else
                        {
                            if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted)
                            {
                                RequestPermissions(new[]
                                {
                                    Manifest.Permission.ReadExternalStorage
                                }, 9);
                            }
                            else
                            {
                                var intent = new Intent(Intent.ActionPick);
                                intent.SetAction(Intent.ActionView);
                                intent.SetType("audio/*");
                                StartActivityForResult(intent, 9);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //On Result 
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                Txt_ContentPost.ClearFocus();
                SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);

                if (requestCode == 1 && resultCode == Result.Ok) // Image
                {
                    if (data.ClipData != null)
                    {
                        var mClipData = data.ClipData;
                        var mArrayUri = new List<Uri>();
                        for (var i = 0; i < mClipData.ItemCount; i++)
                        {
                            var item = mClipData.GetItemAt(i);
                            var uri = item.Uri;

                            mArrayUri.Add(Uri.Parse(IMethods.AttachmentFiles.GetActualPathFromFile(this, uri)));
                        }

                        if (mArrayUri.Count > 0)
                        {
                            var videoAttach = AttachmentsAdapter.AttachemntsList
                                .Where(a => a.TypeAttachment != "postPhotos").ToList();
                            if (videoAttach.Count > 0)
                                foreach (var video in videoAttach)
                                    AttachmentsAdapter.Remove(video);

                            foreach (var item in mArrayUri)
                            {
                                var attach = new Attachments
                                {
                                    ID = AttachmentsAdapter.AttachemntsList.Count + 1,
                                    TypeAttachment = "postPhotos",
                                    FileSimple = item.Path,
                                    FileUrl = item.Path
                                };

                                AttachmentsAdapter.Add(attach);
                            }
                        }
                    }
                    else
                    {
                        var filepath = IMethods.AttachmentFiles.GetActualPathFromFile(this, data.Data);
                        if (filepath != null)
                        {
                            var type = IMethods.AttachmentFiles.Check_FileExtension(filepath);
                            if (type == "Image")
                            {
                                var attach = new Attachments();
                                attach.ID = AttachmentsAdapter.AttachemntsList.Count + 1;
                                attach.TypeAttachment = "postPhotos";
                                attach.FileSimple = filepath;
                                attach.FileUrl = filepath;

                                AttachmentsAdapter.Add(attach);
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                        }
                    }
                }
                else if (requestCode == 2 && resultCode == Result.Ok) // Video 
                {
                    var filepath = IMethods.AttachmentFiles.GetActualPathFromFile(this, data.Data);
                    if (filepath != null)
                    {
                        var type = IMethods.AttachmentFiles.Check_FileExtension(filepath);
                        if (type == "Video")
                        {
                            var fileName = filepath.Split('/').Last();
                            var FileNameWithoutExtenion = fileName.Split('.').First();

                            var path = IMethods.IPath.FolderDcimPost + "/" + FileNameWithoutExtenion + ".png";

                            var VidoPlaceHolderImage =
                                IMethods.MultiMedia.GetMediaFrom_Gallery(IMethods.IPath.FolderDcimPost,
                                    FileNameWithoutExtenion + ".png");
                            if (VidoPlaceHolderImage == "File Dont Exists")
                            {
                                var BitmapImage = IMethods.MultiMedia.Retrieve_VideoFrame_AsBitmap(filepath);
                                IMethods.MultiMedia.Export_Bitmap_As_Image(BitmapImage, FileNameWithoutExtenion,
                                    IMethods.IPath.FolderDcimPost);
                            }

                            //remove file the type
                            var imageAttach = AttachmentsAdapter.AttachemntsList
                                .Where(a => a.TypeAttachment != "postVideo").ToList();
                            if (imageAttach.Count > 0)
                                foreach (var image in imageAttach)
                                    AttachmentsAdapter.Remove(image);

                            var attach = new Attachments
                            {
                                ID = AttachmentsAdapter.AttachemntsList.Count + 1,
                                TypeAttachment = "postVideo",
                                FileSimple = path,
                                FileUrl = filepath
                            };

                            AttachmentsAdapter.Add(attach);
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                    }
                }
                else if (requestCode == 3 && resultCode == Result.Ok) // Mention
                {
                    try
                    {
                        var datauser = Mention_Activity.MentionsAdapter.MentionList.Where(a => a.Selected).ToList();
                        if (datauser.Count > 0)
                        {
                            var TextSanitizer = new TextSanitizer(MentionTextview, this);

                            foreach (var item in datauser) MentionText += " @" + item.username + " ,";

                            TextSanitizer.Load(LoadPostStrings());
                        }
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }
                }
                else if (requestCode == 4 && resultCode == Result.Ok) // Location
                {
                    try
                    {
                        var placePicked = PlacePicker.GetPlace(this, data);

                        if (!string.IsNullOrEmpty(PlaceText))
                            PlaceText = string.Empty;

                        var TextSanitizer = new TextSanitizer(MentionTextview, this);

                        if (placePicked?.NameFormatted?.ToString().Contains("°") == true)
                            PlaceText += " /" + placePicked?.AddressFormatted?.ToString();
                        else
                            PlaceText += " /" + placePicked?.NameFormatted?.ToString();

                        TextSanitizer.Load(LoadPostStrings());

                        PlaceText = placePicked?.NameFormatted?.ToString();

                        //var _placeNameTextView.Text = placePicked?.NameFormatted?.ToString();
                        //_placeAddressTextView.Text = placePicked?.AddressFormatted?.ToString();
                        // _placePhoneNumberTextView.Text = placePicked?.PhoneNumberFormatted?.ToString();
                        // _placeWebSiteTextView.Text = placePicked?.WebsiteUri?.ToString();
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }
                }
                else if (requestCode == 5 && resultCode == Result.Ok) // Feeling
                {
                    var feelings = data.GetStringExtra("Feelings") ?? "Data not available";
                    var feelingsDisplayText = data.GetStringExtra("Feelings") ?? "Data not available";
                    if (feelings != "Data not available" && !string.IsNullOrEmpty(feelings))
                    {
                        var TextSanitizer = new TextSanitizer(MentionTextview, this);
                        FeelingText = feelingsDisplayText; //This Will be displayed And translated
                        PostFeelingType = "feelings"; //Type Of feeling
                        PostFeelingText = feelings.ToLower(); //This will be send via API
                        TextSanitizer.Load(LoadPostStrings());
                    }
                }
                //else if (requestCode == 6 && resultCode == Result.Ok) // Activity
                //{
                //    var activitiesType = data.GetStringExtra("ActivitiesType") ?? "Data not available";
                //    var activitiesText = data.GetStringExtra("ActivitiesText") ?? "Data not available";
                //    if (activitiesText != "Data not available" && !String.IsNullOrEmpty(activitiesText))
                //    {
                //        TextSanitizer TextSanitizer = new TextSanitizer(MentionTextview, this);
                //        ActivityText = activitiesText; //This Will be displayed And translated
                //        PostActivityType = activitiesType; //Type Of feeling
                //        PostActivityText = activitiesText.ToLower(); //This will be send via API
                //        TextSanitizer.Load(LoadPostStrings());
                //    }
                //}
                else if (requestCode == 6 && resultCode == Result.Ok) // Camera
                {
                    try
                    {
                        var extras = data.Extras;
                        var BitmapImage = (Bitmap) extras.Get("data");
                        var unixTimestamp = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

                        IMethods.MultiMedia.Export_Bitmap_As_Image(BitmapImage, unixTimestamp.ToString(),
                            IMethods.IPath.FolderDcimPost);

                        //string path2 = MediaStore.Images.Media.InsertImage(this.ContentResolver, BitmapImage, "Title", null);

                        var path = IMethods.IPath.FolderDcimPost + "/" + unixTimestamp + ".png";
                        if (IMethods.MultiMedia.CheckFileIfExits(path) != "File Dont Exists")
                        {
                            //remove file the type
                            var videoAttach = AttachmentsAdapter.AttachemntsList
                                .Where(a => a.TypeAttachment != "postPhotos").ToList();
                            if (videoAttach.Count > 0)
                                foreach (var video in videoAttach)
                                    AttachmentsAdapter.Remove(video);

                            var attach = new Attachments
                            {
                                ID = AttachmentsAdapter.AttachemntsList.Count + 1,
                                TypeAttachment = "postPhotos",
                                FileSimple = path,
                                FileUrl = path
                            };

                            AttachmentsAdapter.Add(attach);
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                        }
                    }
                    catch (Exception e)
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                        Log.Error("Camera", e.ToString());
                    }
                }
                else if (requestCode == 7 && resultCode == Result.Ok) // Gif
                {
                    var giflink = data.GetStringExtra("gif") ?? "Data not available";
                    if (giflink != "Data not available" && !string.IsNullOrEmpty(giflink))
                    {
                        GifFile = giflink;

                        //remove file the type
                        AttachmentsAdapter.RemoveAll();

                        var attach = new Attachments
                        {
                            ID = AttachmentsAdapter.AttachemntsList.Count + 1,
                            TypeAttachment = "postPhotos",
                            FileSimple = GifFile,
                            FileUrl = GifFile
                        };

                        AttachmentsAdapter.Add(attach);
                    }
                }
                else if (requestCode == 8 && resultCode == Result.Ok) // File
                {
                    var filepath = IMethods.AttachmentFiles.GetActualPathFromFile(this, data.Data);
                    if (filepath != null)
                    {
                        var fileName = filepath.Split('/').Last();
                        var FileNameWithoutExtenion = fileName.Split('.').First();

                        //remove file the type
                        AttachmentsAdapter.RemoveAll();

                        var attach = new Attachments
                        {
                            ID = AttachmentsAdapter.AttachemntsList.Count + 1,
                            TypeAttachment = "postFile",
                            FileSimple = "Image_File.jpg",
                            FileUrl = filepath
                        };

                        AttachmentsAdapter.Add(attach);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                    }
                }
                else if (requestCode == 9 && resultCode == Result.Ok) // Music
                {
                    var filepath = IMethods.AttachmentFiles.GetActualPathFromFile(this, data.Data);
                    if (filepath != null)
                    {
                        var type = IMethods.AttachmentFiles.Check_FileExtension(filepath);
                        if (type == "Audio")
                        {
                            var fileName = filepath.Split('/').Last();
                            var FileNameWithoutExtenion = fileName.Split('.').First();

                            //remove file the type
                            AttachmentsAdapter.RemoveAll();

                            var attach = new Attachments
                            {
                                ID = AttachmentsAdapter.AttachemntsList.Count + 1,
                                TypeAttachment = "postMusic",
                                FileSimple = "Audio_File.jpg",
                                FileUrl = filepath
                            };

                            AttachmentsAdapter.Add(attach);
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                    }
                }

                Txt_ContentPost.ClearFocus();
                SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
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

                if (requestCode == 1) //Image Gallery
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        var galleryIntent = new Intent(Intent.ActionPick);
                        galleryIntent.SetAction(Intent.ActionGetContent);
                        galleryIntent.SetType("image/*");
                        StartActivityForResult(galleryIntent, 1);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                    }
                }
                else if (requestCode == 2) //Video Gallery
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        var galleryIntent = new Intent(Intent.ActionPick);
                        galleryIntent.SetAction(Intent.ActionGetContent);
                        galleryIntent.SetType("video/*");
                        StartActivityForResult(galleryIntent, 2);
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                    }
                }
                else if (requestCode == 4) // Location
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        var builder = new PlacePicker.IntentBuilder();
                        StartActivityForResult(builder.Build(this), 4);
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                    }
                }
                else if (requestCode == 6) //Camera
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        var cameraIntent = new Intent(MediaStore.ActionImageCapture);
                        StartActivityForResult(cameraIntent, 6);
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                    }
                }
                else if (requestCode == 8) // File
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        var galleryIntent = new Intent(Intent.ActionPick);
                        galleryIntent.SetAction(Intent.ActionGetContent);
                        galleryIntent.SetType("*/*");
                        StartActivityForResult(galleryIntent, 12);
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                    }
                }
                else if (requestCode == 9) //Music
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        var intent = new Intent(Intent.ActionPick);
                        intent.SetAction(Intent.ActionGetContent);
                        intent.SetType("audio/*");
                        StartActivityForResult(intent, 13);
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        // Event Back
        public override void OnBackPressed()
        {
            try
            {
                if (!string.IsNullOrEmpty(Txt_ContentPost.Text) || !string.IsNullOrEmpty(MentionText) ||
                    AttachmentsAdapter.AttachemntsList.Count > 0)
                {
                    TypeDialog = "PostBack";

                    var dialog = new MaterialDialog.Builder(this);

                    dialog.Title(GetText(Resource.String.Lbl_Title_Back));
                    dialog.Content(GetText(Resource.String.Lbl_Content_Back));
                    dialog.PositiveText(GetText(Resource.String.Lbl_PositiveText_Back)).OnPositive(this);
                    dialog.NegativeText(GetText(Resource.String.Lbl_NegativeText_Back)).OnNegative(this);
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.ItemsCallback(this).Build().Show();
                }
                else
                {
                    base.OnBackPressed();
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

        public Toolbar TopToolBar;
        public SlidingUpPanelLayout SlidingUpPanel;
        public ImageViewAsync Postsectionimage;
        public TextView Txt_AddPost;
        public TextView Txt_UserName;

        public EditText Txt_ContentPost;

        public RecyclerView PostTypeRecylerView;
        public RecyclerView AttachementRecylerView;
        public MainPostAdapter MainPostAdapter;
        public AttachmentsAdapter AttachmentsAdapter;

        public ImageView IconHappy;
        public ImageView IconTag;
        public ImageView IconImage;

        public AutoLinkTextView MentionTextview;
        public Button PostPrivacyButton;
        public string MentionText = "";
        public string PlaceText = "";
        public string FeelingText = "";
        public string ActivityText = "";
        public string ListeningText = "";
        public string PlayingText = "";
        public string WatchingText = "";
        public string TravelingText = "";

        public string GifFile = "";

        public string PagePost = "";
        public string isOwnerCommunities = "";
        public string IdPost = "";
        public string PostPrivacy = "";

        //## Post Parmeeters for API ##
        public string PostFeelingType = "";
        public string PostFeelingText = "";

        public string PostActivityType = "";
        public string PostActivityText = "";

        public string TypeDialog = "";

        #endregion

        #region Panel Item Post

        public void OnPanelClosed(View panel)
        {
            try
            {
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnPanelOpened(View panel)
        {
            try
            {
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        void SlidingPaneLayout.IPanelSlideListener.OnPanelSlide(View panel, float slideOffset)
        {
            try
            {
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnPanelStateChanged(View p0, SlidingUpPanelLayout.PanelState p1, SlidingUpPanelLayout.PanelState p2)
        {
            try
            {
                if (p1 == SlidingUpPanelLayout.PanelState.Expanded && p2 == SlidingUpPanelLayout.PanelState.Dragging)
                {
                    if (IconTag.Tag.ToString() == "Open")
                    {
                        IconTag.SetImageResource(Resource.Drawable.ic__Attach_tag);
                        IconTag.Tag = "Close";
                        IconImage.Visibility = ViewStates.Visible;
                        IconHappy.Visibility = ViewStates.Visible;
                    }
                }
                else if (p1 == SlidingUpPanelLayout.PanelState.Collapsed &&
                         p2 == SlidingUpPanelLayout.PanelState.Dragging)
                {
                    if (IconTag.Tag.ToString() == "Close")
                    {
                        IconTag.SetImageResource(Resource.Drawable.ic_action_arrow_down_sign);
                        IconTag.Tag = "Open";
                        IconImage.Visibility = ViewStates.Invisible;
                        IconHappy.Visibility = ViewStates.Invisible;
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        void SlidingUpPanelLayout.IPanelSlideListener.OnPanelSlide(View p0, float p1)
        {
            try
            {
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion

        #region Privacy

        private void LoadDataUser()
        {
            try
            {
                var dataUser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                if (dataUser != null)
                {
                    var AvatarSplit = dataUser.avatar.Split('/').Last();
                    var getImage_Avatar =IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(Postsectionimage, "no_profile_image.png", getImage_Avatar, 1);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, dataUser.avatar);
                        ImageServiceLoader.Load_Image(Postsectionimage, "no_profile_image.png", dataUser.avatar, 1);
                    }

                    Txt_UserName.Text = dataUser.name;

                    PostPrivacyButton.Text = GetString(Resource.String.Lbl_Everyone);

                    //if (dataUser.post_privacy.Contains("0"))
                    //    PostPrivacyButton.Text = GetString(Resource.String.Lbl_Everyone);
                    //else if (dataUser.post_privacy.Contains("ifollow"))
                    //    PostPrivacyButton.Text = GetString(Resource.String.Lbl_People_i_Follow);
                    //else if (dataUser.post_privacy.Contains("me"))
                    //    PostPrivacyButton.Text = GetString(Resource.String.Lbl_People_Follow_Me);
                    //else
                    //    PostPrivacyButton.Text = GetString(Resource.String.Lbl_No_body);

                    PostPrivacy = "0";
                }
                else
                {
                    Txt_UserName.Text = UserDetails.Username;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void GetPrivacyPost()
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();

                var isOwner = Intent.GetStringExtra("isOwner") ?? "Data not available";
                if (isOwner != "Data not available" && !string.IsNullOrEmpty(isOwner)) isOwnerCommunities = isOwner;

                if (PagePost == "Normal")
                {
                    LoadDataUser();
                }
                else if (PagePost == "SocialGroup")
                {
                    if (isOwnerCommunities == "true" || isOwnerCommunities == "True")
                    {
                        var dataGroup = dbDatabase.Get_ItemIsOwner_Groups(IdPost);
                        if (dataGroup != null)
                        {
                            PostPrivacyButton.Visibility = ViewStates.Gone;

                            var AvatarSplit = dataGroup.Avatar.Split('/').Last();
                            var getImage_Avatar =
                                IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                            if (getImage_Avatar != "File Dont Exists")
                            {
                                ImageServiceLoader.Load_Image(Postsectionimage, "no_profile_image.png", getImage_Avatar,
                                    1);
                            }
                            else
                            {
                                IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage,
                                    dataGroup.Avatar);
                                ImageServiceLoader.Load_Image(Postsectionimage, "no_profile_image.png",
                                    dataGroup.Avatar, 1);
                            }

                            Txt_UserName.Text = dataGroup.GroupName;
                        }
                        else
                        {
                            LoadDataUser();
                        }
                    }
                    else
                    {
                        LoadDataUser();
                    }
                }
                else if (PagePost == "SocialPage")
                {
                    if (isOwnerCommunities == "true" || isOwnerCommunities == "True")
                    {
                        var dataPage = dbDatabase.Get_ItemIsOwner_Page(IdPost);
                        if (dataPage != null)
                        {
                            PostPrivacyButton.Visibility = ViewStates.Gone;

                            var AvatarSplit = dataPage.Avatar.Split('/').Last();
                            var getImage_Avatar =
                                IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                            if (getImage_Avatar != "File Dont Exists")
                            {
                                ImageServiceLoader.Load_Image(Postsectionimage, "no_profile_image.png", getImage_Avatar,
                                    1);
                            }
                            else
                            {
                                IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage,
                                    dataPage.Avatar);
                                ImageServiceLoader.Load_Image(Postsectionimage, "no_profile_image.png", dataPage.Avatar,
                                    1);
                            }

                            Txt_UserName.Text = dataPage.PageName;
                        }
                        else
                        {
                            LoadDataUser();
                        }
                    }
                    else
                    {
                        LoadDataUser();
                    }
                }
                else
                {
                    LoadDataUser();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void PostPrivacyButton_Click(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "PostPrivacy";

                var arrayAdapter = new List<string>();
                var DialogList = new MaterialDialog.Builder(this);

                arrayAdapter.Add(GetString(Resource.String.Lbl_Everyone));
                arrayAdapter.Add(GetString(Resource.String.Lbl_People_i_Follow));
                arrayAdapter.Add(GetText(Resource.String.Lbl_People_Follow_Me));
                arrayAdapter.Add(GetString(Resource.String.Lbl_No_body));

                DialogList.Title(GetText(Resource.String.Lbl_PostPrivacy));
                DialogList.Items(arrayAdapter);
                DialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                DialogList.ItemsCallback(this).Build().Show();
                DialogList.AlwaysCallSingleChoiceCallback();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #endregion
    }
}