using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.BlockedUsers;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;

namespace WoWonder.Activities.DialogUser
{
    public class Dialog_BlockUser : DialogFragment
    {
        public Dialog_BlockUser(string userid, Get_Blocked_Users_Object.Blocked_Users item)
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

                // Set our view from the "Dialog_BlockUser_Fragment" layout resource
                var view = inflater.Inflate(Resource.Layout.Dialog_BlockUser_Fragment, container, false);

                // Get values
                Txt_Username = view.FindViewById<TextView>(Resource.Id.Txt_Username);
                Txt_Name = view.FindViewById<TextView>(Resource.Id.Txt_SecendreName);

                Btn_UnBlockUser = view.FindViewById<Button>(Resource.Id.UnBlockUser_Button);

                Image_Userprofile = view.FindViewById<ImageViewAsync>(Resource.Id.profileAvatar_image);

                var ImageTrancform = ImageService.Instance.LoadUrl(_Item.avatar);
                ImageTrancform.LoadingPlaceholder("no_profile_image.png", ImageSource.CompiledResource);
                ImageTrancform.ErrorPlaceholder("no_profile_image.png", ImageSource.CompiledResource);
                ImageTrancform.TransformPlaceholders(true);
                ImageTrancform.Transform(new CircleTransformation(5, Settings.MainColor));
                ImageTrancform.Into(Image_Userprofile);

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(_Item.name));

                Txt_Username.Text = name;
                Txt_Name.Text = "@" + _Item.username;

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
                Btn_UnBlockUser.Click += BtnUnBlockUserOnClick;
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

                // Event
                Btn_UnBlockUser.Click -= BtnUnBlockUserOnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //Sets the title bar to invisible
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set the animation
        }

        private void BtnUnBlockUserOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    var local = BlockedUsers_Activity.mAdapter?.mBlockedUsersList?.FirstOrDefault(a =>a.user_id == _Userid);
                    if (local != null) BlockedUsers_Activity.mAdapter?.Remove(local);

                    Toast.MakeText(Application.Context, GetString(Resource.String.Lbl_Unblock_successfully),ToastLength.Short).Show();

                    var data = Client.Global.Block_User(_Userid, false).ConfigureAwait(false); //false >> "un-block"
                }
                else
                {
                    Toast.MakeText(Context, GetString(Resource.String.Lbl_CheckYourInternetConnection),
                        ToastLength.Short).Show();
                }

                Dismiss();
                var x = Resource.Animation.slide_right;
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

        public class OnBlockUserUp_EventArgs : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }
        }

        #region Variables Basic

        private TextView Txt_Username;
        private TextView Txt_Name;

        private Button Btn_UnBlockUser;

        private ImageViewAsync Image_Userprofile;

        public event EventHandler<OnBlockUserUp_EventArgs> _OnBlockUserUpComplete;

        public static string _Userid = "";
        public Get_Blocked_Users_Object.Blocked_Users _Item;

        #endregion
    }
}