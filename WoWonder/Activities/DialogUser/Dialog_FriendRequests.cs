using System;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using FFImageLoading;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using WoWonder.Activities.Tabbes;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Requests;

namespace WoWonder.Activities.DialogUser
{
    public class Dialog_FriendRequests : DialogFragment
    {
        public Dialog_FriendRequests(string userid, Get_General_Data_Object.Friend_Requests item)
        {
            try
            {
                _Userid = userid;
                _Item = item;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                base.OnCreateView(inflater, container, savedInstanceState);

                // Set our view from the "Dialog_FriendRequest_Fragment" layout resource
                var view = inflater.Inflate(Resource.Layout.Dialog_FriendRequest_Fragment, container, false);

                // Get values
                Txt_Username = view.FindViewById<TextView>(Resource.Id.Txt_Username);
                Txt_Name = view.FindViewById<TextView>(Resource.Id.Txt_SecendreName);

                Btn_Delete = view.FindViewById<CircleButton>(Resource.Id.delete_button);
                Btn_Delete.Tag = "false";

                Btn_Accept = view.FindViewById<CircleButton>(Resource.Id.Add_button);
                Btn_Accept.Tag = "false";

                Image_Userprofile = view.FindViewById<ImageViewAsync>(Resource.Id.profileAvatar_image);
                Image_Cover = view.FindViewById<ImageViewAsync>(Resource.Id.profileCover_image);

                LoadData();

                return view;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();

                //Add Event
                Btn_Delete.Click += BtnDelete_OnClick;
                Btn_Accept.Click += BtnAccept_OnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                base.OnPause();

                //Close Event
                Btn_Delete.Click -= BtnDelete_OnClick;
                Btn_Accept.Click -= BtnAccept_OnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        public void LoadData()
        {
            try
            {
                var AvatarSplit = _Item.avatar.Split('/').Last();
                var getImage_Avatar =
                    IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                if (getImage_Avatar != "File Dont Exists")
                {
                    ImageServiceLoader.Load_Image(Image_Userprofile, "no_profile_image.png", getImage_Avatar, 4);
                }
                else
                {
                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, _Item.avatar);
                    ImageServiceLoader.Load_Image(Image_Userprofile, "no_profile_image.png", _Item.avatar, 4);
                }

                var CoverSplit = _Item.cover.Split('/').Last();
                var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, CoverSplit);
                if (getImage_Cover != "File Dont Exists")
                {
                    ImageServiceLoader.Load_Image(Image_Cover, "ImagePlacholder.jpg", getImage_Cover);
                }
                else
                {
                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, _Item.cover);
                    ImageServiceLoader.Load_Image(Image_Cover, "ImagePlacholder.jpg", _Item.cover);
                }

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(_Item.name));
                Txt_Username.Text = name;
                Txt_Name.Text = "@" + _Item.username;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        // Event button Accept User
        private void BtnAccept_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection),
                        ToastLength.Short).Show();
                }
                else
                {
                    if (Btn_Accept.Tag.ToString() == "false")
                    {
                        Btn_Accept.SetColor(Color.ParseColor("#efefef"));
                        Btn_Accept.SetImageResource(Resource.Drawable.ic_tick);
                        Btn_Accept.Drawable.SetTint(Color.ParseColor("#444444"));
                        Btn_Accept.Tag = "true";
                    }
                    else
                    {
                        Btn_Accept.SetColor(Color.ParseColor("#444444"));
                        Btn_Accept.SetImageResource(Resource.Drawable.ic_add);
                        Btn_Accept.Drawable.SetTint(Color.ParseColor("#ffffff"));

                        Btn_Accept.Tag = "false";
                    }

                    Btn_Delete.Visibility = ViewStates.Gone;
                    Btn_Accept.Enabled = false;

                    var local = Notifications_Fragment.FriendRequestsAdapter?.mFriendRequestsList?.FirstOrDefault(a =>
                        a.user_id == _Userid);
                    if (local != null) Notifications_Fragment.FriendRequestsAdapter?.Remove(local);

                    Toast.MakeText(Application.Context, GetString(Resource.String.Lbl_Done), ToastLength.Short).Show();

                    var result = Client.Global.Follow_Request_Action(_Item.user_id, true)
                        .ConfigureAwait(false); // true >> Accept 
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        // Event button Decline User
        private void BtnDelete_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection),
                        ToastLength.Short).Show();
                }
                else
                {
                    if (Btn_Delete.Tag.ToString() == "false")
                    {
                        Btn_Delete.SetColor(Color.ParseColor("#efefef"));
                        Btn_Delete.SetImageResource(Resource.Drawable.ic_tick);
                        Btn_Delete.Drawable.SetTint(Color.ParseColor("#444444"));
                        Btn_Delete.Tag = "true";
                    }
                    else
                    {
                        Btn_Delete.SetColor(Color.ParseColor("#444444"));
                        Btn_Delete.SetImageResource(Resource.Drawable.ic_close_padded);
                        Btn_Delete.Drawable.SetTint(Color.ParseColor("#ffffff"));

                        Btn_Delete.Tag = "false";
                    }

                    Btn_Accept.Visibility = ViewStates.Gone;
                    Btn_Delete.Enabled = false;


                    var local = Notifications_Fragment.FriendRequestsAdapter?.mFriendRequestsList?.FirstOrDefault(a =>
                        a.user_id == _Userid);
                    if (local != null) Notifications_Fragment.FriendRequestsAdapter?.Remove(local);

                    Toast.MakeText(Application.Context, GetString(Resource.String.Lbl_Done), ToastLength.Short).Show();

                    var result = Client.Global.Follow_Request_Action(_Item.user_id, false)
                        .ConfigureAwait(false); // false >> Decline

                    var th = new Thread(ActLikeARequest);
                    th.Start();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void ActLikeARequest()
        {
            var x = Resource.Animation.slide_right;
        }


        //animations
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            try
            {
                Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //Sets the title bar to invisible
                base.OnActivityCreated(savedInstanceState);
                Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set the animation
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

        public class OnFriendRequestsUp_EventArgs : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }
        }

        #region Variables Basic

        private TextView Txt_Username;
        private TextView Txt_Name;

        private CircleButton Btn_Delete;
        private CircleButton Btn_Accept;

        private ImageViewAsync Image_Userprofile;
        private ImageViewAsync Image_Cover;

        public event EventHandler<OnFriendRequestsUp_EventArgs> _OnUserUpComplete;

        public static string _Userid = "";
        public Get_General_Data_Object.Friend_Requests _Item;

        #endregion
    }
}