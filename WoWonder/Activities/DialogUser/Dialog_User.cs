using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Java.IO;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.UserProfile;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.DialogUser
{
    public class Dialog_User : DialogFragment
    {
        public Dialog_User(string userid, Get_Search_Object.User item)
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

        //open Layout as a message
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                base.OnCreateView(inflater, container, savedInstanceState);

                // Set our view from the "Dialog_User_Fragment" layout resource
                var view = inflater.Inflate(Resource.Layout.Dialog_User_Fragment, container, false);

                // Get values
                Txt_Username = view.FindViewById<TextView>(Resource.Id.Txt_Username);
                Txt_Name = view.FindViewById<TextView>(Resource.Id.Txt_SecendreName);

                Btn_ShowProfile = view.FindViewById<CircleButton>(Resource.Id.ShowProfile_button);

                Btn_Add = view.FindViewById<CircleButton>(Resource.Id.Add_button);
                Btn_Add.Tag = "Add";

                Image_Userprofile = view.FindViewById<ImageViewAsync>(Resource.Id.profileAvatar_image);
                Image_Cover = view.FindViewById<ImageViewAsync>(Resource.Id.profileCover_image);

                //profile_picture
                var AvatarSplit = _Item.Avatar.Split('/').Last();
                if (AvatarSplit == "d-avatar.jpg")
                {
                    var ImageTrancform = ImageService.Instance.LoadUrl("no_profile_image.png");
                    ImageTrancform.LoadingPlaceholder("no_profile_image.png", ImageSource.CompiledResource);
                    ImageTrancform.ErrorPlaceholder("no_profile_image.png", ImageSource.CompiledResource);
                    ImageTrancform.TransformPlaceholders(true);
                    ImageTrancform.Transform(new CircleTransformation(5, "#ffffff"));
                    ImageTrancform.Into(Image_Userprofile);
                }
                else
                {
                    var GetImg = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (GetImg == "File Dont Exists")
                    {
                        Task.Run(() =>
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, _Item.Avatar);
                            IMethods.Load_Image_From_Url(Image_Userprofile, _Item.Avatar);
                        });
                    }
                    else
                    {
                        var file = Uri.FromFile(new File(GetImg));
                        var ImageTrancform = ImageService.Instance.LoadFile(file.Path);
                        ImageTrancform.LoadingPlaceholder("no_profile_image.png", ImageSource.CompiledResource);
                        ImageTrancform.ErrorPlaceholder("no_profile_image.png", ImageSource.CompiledResource);
                        ImageTrancform.TransformPlaceholders(true);
                        ImageTrancform.Transform(new CircleTransformation(5, "#ffffff"));
                        ImageTrancform.FadeAnimation(false);
                        ImageTrancform.Into(Image_Userprofile);
                    }
                }

                IMethods.Load_Image_From_Url_Normally(Image_Cover, _Item.Cover);

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(_Item.Name));

                Txt_Username.Text = name;
                Txt_Name.Text = "@" + _Item.Username;

                if (_Item.is_following == "yes" || _Item.is_following == "Yes") // My Friend
                {
                    Btn_Add.Visibility = ViewStates.Visible;
                    Btn_Add.SetColor(Color.ParseColor("#efefef"));
                    Btn_Add.SetImageResource(Resource.Drawable.ic_tick);
                    Btn_Add.Drawable.SetTint(Color.ParseColor("#444444"));
                    Btn_Add.Tag = "friends";
                }
                else //Not Friend
                {
                    Btn_Add.Visibility = ViewStates.Visible;

                    Btn_Add.SetColor(Color.ParseColor("#444444"));
                    Btn_Add.SetImageResource(Resource.Drawable.ic_add);
                    Btn_Add.Drawable.SetTint(Color.ParseColor("#ffffff"));
                    Btn_Add.Tag = "Add";
                }

                // Event
                Btn_ShowProfile.Click += BtnShowProfile_OnClick;
                Btn_Add.Click += BtnAddOnClick;

                return view;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        }

        private void BtnShowProfile_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                Dismiss();
                var x = Resource.Animation.slide_right;

                var intent = new Intent(Context, typeof(User_Profile_Activity));
                intent.PutExtra("UserID", _Userid);
               intent.PutExtra("UserId", _Userid);
               intent.PutExtra("UserType", "Search");
                intent.PutExtra("UserItem", JsonConvert.SerializeObject(_Item));
                StartActivity(intent);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
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

        private void BtnAddOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    if (Btn_Add.Tag.ToString() == "Add") //(is_following == "0") >> Not Friend
                    {
                        Btn_Add.SetColor(Color.ParseColor("#efefef"));
                        Btn_Add.SetImageResource(Resource.Drawable.ic_tick);
                        Btn_Add.Drawable.SetTint(Color.ParseColor("#444444"));
                        Btn_Add.Tag = "friends";


                        _Item.is_following = "yes";
                    }
                    else //(is_following == "1") >> Friend
                    {
                        Btn_Add.SetColor(Color.ParseColor("#444444"));
                        Btn_Add.SetImageResource(Resource.Drawable.ic_add);
                        Btn_Add.Drawable.SetTint(Color.ParseColor("#ffffff"));
                        Btn_Add.Tag = "Add";

                        _Item.is_following = "no";
                    }

                    var result = Client.Global.Follow_User(_Userid).ConfigureAwait(false);
                    if (_Item.is_following == "yes")
                    {
                        if (Settings.ConnectivitySystem == "1")
                            Toast.MakeText(Application.Context,
                                Application.Context.GetString(Resource.String.Lbl_Sent_successfully_followed),
                                ToastLength.Short).Show();
                        else
                            Toast.MakeText(Application.Context,
                                Application.Context.GetString(Resource.String.Lbl_Sent_successfully_FriendRequest),
                                ToastLength.Short).Show();
                    }
                    else
                    {
                        if (Settings.ConnectivitySystem == "1")
                            Toast.MakeText(Application.Context,
                                Application.Context.GetString(Resource.String.Lbl_Sent_successfully_Unfollowed),
                                ToastLength.Short).Show();
                        else
                            Toast.MakeText(Application.Context,
                                Application.Context.GetString(Resource.String
                                    .Lbl_Sent_successfully_FriendRequestCancelled), ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(Application.Context,
                        Application.Context.GetString(Resource.String.Lbl_CheckYourInternetConnection),
                        ToastLength.Short).Show();
                }
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

        public class OnUserUp_EventArgs : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }
        }

        #region Variables Basic

        private TextView Txt_Username;
        private TextView Txt_Name;

        private CircleButton Btn_ShowProfile;
        private CircleButton Btn_Add;

        private ImageViewAsync Image_Userprofile;
        private ImageViewAsync Image_Cover;

        public event EventHandler<OnUserUp_EventArgs> _OnUserUpComplete;

        public static string _Userid = "";
        public Get_Search_Object.User _Item;

        #endregion
    }
}