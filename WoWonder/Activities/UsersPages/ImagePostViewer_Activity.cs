using System;
using System.Globalization;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using ImageViews.Photo;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using SettingsConnecter;
using WoWonder.Activities.Album;
using WoWonder.Activities.PostData;
using WoWonder.Activities.UserProfile;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using ClipboardManager = Android.Content.ClipboardManager;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.UsersPages
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class ImagePostViewer_Activity : AppCompatActivity, MaterialDialog.ISingleButtonCallback,
        MaterialDialog.IInputCallback
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                }
                else
                {
                    Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                }

                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.ImagePostViewer_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.ImagePostViewer_Layout);

                PassedImage = Intent.GetStringExtra("ImageUrl");

                TopToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (TopToolBar != null)
                {
                    SetSupportActionBar(TopToolBar);
                    SupportActionBar.Title = " ";

                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                PageImage = FindViewById<ImageViewAsync>(Resource.Id.imageview);

                Txt_Description = FindViewById<TextView>(Resource.Id.tv_description);

                Img_Like = FindViewById<ImageView>(Resource.Id.image_like);
                Img_Wowonder = FindViewById<ImageView>(Resource.Id.image_wowonder);

                Txt_CountLike = FindViewById<TextView>(Resource.Id.LikeText1);
                Txt_CountWowonder = FindViewById<TextView>(Resource.Id.WoWonderTextCount);

                Btn_CountLike = FindViewById<LinearLayout>(Resource.Id.linerlikeCount);
                Btn_CountWowonder = FindViewById<LinearLayout>(Resource.Id.linerwowonderCount);

                Btn_Like = FindViewById<LinearLayout>(Resource.Id.linerlike);
                Btn_Comment = FindViewById<LinearLayout>(Resource.Id.linercomment);
                Btn_Share = FindViewById<LinearLayout>(Resource.Id.linershare);

                if (Settings.WonderSystem == false)
                {
                    Txt_CountWowonder.Visibility = ViewStates.Gone;
                    Btn_CountWowonder.Visibility = ViewStates.Gone;
                    Img_Wowonder.Visibility = ViewStates.Gone;
                }


                SetData_Image();
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
                Btn_Like.Click += BtnLikeOnClick;
                Btn_Comment.Click += BtnCommentOnClick;
                Btn_Share.Click += BtnShareOnClick;
                Btn_CountLike.Click += BtnCountLikeOnClick;
                Btn_CountWowonder.Click += BtnCountWowonderOnClick;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();

                //Close Event
                Btn_Like.Click -= BtnLikeOnClick;
                Btn_Comment.Click -= BtnCommentOnClick;
                Btn_Share.Click -= BtnShareOnClick;
                Btn_CountLike.Click -= BtnCountLikeOnClick;
                Btn_CountWowonder.Click -= BtnCountWowonderOnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void SetData_Image()
        {
            try
            {
                _album = JsonConvert.DeserializeObject<Get_User_Albums_Object.Album>(Intent.GetStringExtra("Item"));
                if (_album != null)
                {
                    PassedId = _album.post_id;

                    var AvatarSplit = PassedImage.Split('/').Last();
                    var getImage = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(PageImage, "ImagePlacholder.jpg", getImage);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, PassedImage);
                        ImageServiceLoader.Load_Image(PageImage, "ImagePlacholder.jpg", PassedImage);
                    }

                    Txt_Description.Text = _album.Orginaltext;

                    if (_album.is_liked)
                    {
                        Btn_Like.Tag = "true";
                        Img_Like.SetColorFilter(Color.ParseColor(Settings.MainColor));
                    }
                    else
                    {
                        Btn_Like.Tag = "false";
                    }

                    Txt_CountLike.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(_album.post_likes));
                    Txt_CountWowonder.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(_album.post_wonders));
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_something_went_wrong), ToastLength.Short).Show();
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
        public ImageViewAsync PageImage;

        public TextView IconMap;

        //public PhotoView PageImage;
        public PhotoViewAttacher photoViewAttacher;

        private TextView Txt_Description;

        private LinearLayout Btn_CountLike;
        private ImageView Img_Like;
        private TextView Txt_CountLike;

        private LinearLayout Btn_CountWowonder;
        private ImageView Img_Wowonder;
        private TextView Txt_CountWowonder;

        private LinearLayout Btn_Like;
        private LinearLayout Btn_Comment;
        private LinearLayout Btn_Share;

        private string PassedId = "";
        private string PassedImage = "";
        private Get_User_Albums_Object.Album _album;

        #endregion

        #region Event

        //Event Show all users Wowonder >> Open Post PostData_Activity
        private void BtnCountWowonderOnClick(object sender, EventArgs e)
        {
            try
            {
                var Int = new Intent(ApplicationContext, typeof(PostData_Activity));
                Int.PutExtra("PostId", _album.post_id);
                Int.PutExtra("PostType", "post_wonders");
                ApplicationContext.StartActivity(Int);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Show all users liked >> Open Post PostData_Activity
        private void BtnCountLikeOnClick(object sender, EventArgs e)
        {
            try
            {
                var Int = new Intent(ApplicationContext, typeof(PostData_Activity));
                Int.PutExtra("PostId", _album.post_id);
                Int.PutExtra("PostType", "post_likes");
                ApplicationContext.StartActivity(Int);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Share
        private async void BtnShareOnClick(object sender, EventArgs e)
        {
            try
            {
                //Share Plugin same as video
                if (!CrossShare.IsSupported) return;

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = _album.Orginaltext,
                    Text = "",
                    Url = _album.post_url
                });
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Add Like
        private void BtnLikeOnClick(object sender, EventArgs e)
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
                    if (Btn_Like.Tag.ToString() == "true" || Btn_Like.Tag.ToString() == "True")
                    {
                        Btn_Like.Tag = "false";


                        if (!Txt_CountLike.Text.Contains("K") && !Txt_CountLike.Text.Contains("M"))
                        {
                            var x = Convert.ToDouble(Txt_CountLike.Text);
                            if (x > 0)
                                x--;
                            else
                                x = 0;
                            Txt_CountLike.Text = Convert.ToString(x, CultureInfo.InvariantCulture);
                        }

                        if (!string.IsNullOrEmpty(PassedId))
                        {
                            //Update data list
                            if (_album != null)
                            { 
                                var dataImageUser = User_Profile_Activity.UserPhotosAdapter?.mUserAlbumsList?.FirstOrDefault(w =>
                                        w.id == _album.id);
                                if (dataImageUser != null)
                                {
                                    dataImageUser.is_liked = false;

                                    var x = Convert.ToDouble(dataImageUser.post_likes);
                                    if (x > 0)
                                        x--;
                                    else
                                        x = 0;

                                    dataImageUser.post_likes = Convert.ToString(x, CultureInfo.InvariantCulture);
                                }
                                else
                                {

                                    var dataImage = MyPhotosActivity.photosAdapter?.mMyAlbumsList?.FirstOrDefault(w =>w.id == _album.id);
                                    if (dataImage != null)
                                    {
                                        dataImage.is_liked = false;

                                        var x = Convert.ToDouble(dataImage.post_likes);
                                        if (x > 0)
                                            x--;
                                        else
                                            x = 0;

                                        dataImage.post_likes = Convert.ToString(x, CultureInfo.InvariantCulture);
                                    }
                                }
                            }

                            //sent Api
                            var data = Client.Global.Post_Actions(PassedId, "Dislike").ConfigureAwait(false);

                            //change color
                            Img_Like.SetColorFilter(Color.ParseColor("#efefef"));

                            Toast.MakeText(this, GetText(Resource.String.Btn_Dislike), ToastLength.Short).Show();
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_something_went_wrong), ToastLength.Short)
                                .Show();
                        }
                    }
                    else
                    {
                        Btn_Like.Tag = "true";
                        if (!Txt_CountLike.Text.Contains("K") && !Txt_CountLike.Text.Contains("M"))
                        {
                            var x = Convert.ToDouble(Txt_CountLike.Text);
                            x++;
                            Txt_CountLike.Text = Convert.ToString(x, CultureInfo.InvariantCulture);
                        }

                        if (!string.IsNullOrEmpty(PassedId))
                        {
                            //Update data list
                            if (_album != null)
                            {
                                var dataImageUser =User_Profile_Activity.UserPhotosAdapter?.mUserAlbumsList?.FirstOrDefault(w =>
                                        w.id == _album.id);
                                if (dataImageUser != null)
                                {
                                    dataImageUser.is_liked = true;

                                    var x = Convert.ToDouble(_album.post_likes);
                                    x++;

                                    dataImageUser.post_likes = Convert.ToString(x, CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    var dataImage = MyPhotosActivity.photosAdapter?.mMyAlbumsList.FirstOrDefault(w =>
                                            w.id == _album.id);
                                    if (dataImage != null)
                                    {
                                        dataImage.is_liked = true;

                                        var x = Convert.ToDouble(_album.post_likes);
                                        x++;
                                        dataImage.post_likes = Convert.ToString(x, CultureInfo.InvariantCulture);
                                    }
                                }
                            }

                            //sent Api
                            var data = Client.Global.Post_Actions(PassedId, "like").ConfigureAwait(false);

                            //change color
                            Img_Like.SetColorFilter(Color.ParseColor(Settings.MainColor));

                            Toast.MakeText(this, GetText(Resource.String.Btn_Liked), ToastLength.Short).Show();
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_something_went_wrong), ToastLength.Short)
                                .Show();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Add Comment
        private void BtnCommentOnClick(object sender, EventArgs e)
        {
            try
            {
                var dialog = new MaterialDialog.Builder(this);

                dialog.Title(Resource.String.Lbl_leave_comment);
                dialog.Input(Resource.String.Lbl_Write_comment, 0, false, this);
                dialog.InputType(InputTypes.TextFlagImeMultiLine);
                dialog.PositiveText(GetText(Resource.String.Lbl_Comment)).OnPositive(this);
                dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialog.Build().Show();
                dialog.AlwaysCallSingleChoiceCallback();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
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

        public void OnInput(MaterialDialog p0, ICharSequence p1)
        {
            try
            {
                if (p1.Length() > 0)
                {
                    var strName = p1.ToString();

                    if (!IMethods.CheckConnectivity())
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection),
                            ToastLength.Short).Show();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(PassedId))
                        {
                            var data = Client.Global.Post_Actions(PassedId, "commet", strName).ConfigureAwait(false);
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_something_went_wrong), ToastLength.Short)
                                .Show();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion

        #region Menu

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ImagePost, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                case Resource.Id.download:
                    Download_OnClick();
                    break;

                case Resource.Id.ic_action_comment:
                    Copy_OnClick();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }


        //Event Download Image  
        public void Download_OnClick()
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
                    IMethods.MultiMedia.DownloadMediaTo_GalleryAsync(IMethods.IPath.FolderDiskImage, PassedImage);
                    Toast.MakeText(this, GetText(Resource.String.Lbl_ImageSaved), ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Copy link image 
        public void Copy_OnClick()
        {
            try
            {
                var clipboardManager = (ClipboardManager) GetSystemService(ClipboardService);

                var clipData = ClipData.NewPlainText("text", PassedImage);
                clipboardManager.PrimaryClip = clipData;

                Toast.MakeText(this, GetText(Resource.String.Lbl_Copied), ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion
    }
}