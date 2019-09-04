using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Com.Devs.ReadMoreOptionLib;
using Com.Theartofdev.Edmodo.Cropper;
using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using SettingsConnecter;
using WoWonder.Activities.AddPost;
using WoWonder.Helpers;
using WoWonder.Helpers.HybirdView;
using WoWonder.SQLite;
using WoWonder_API;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Group;
using WoWonder_API.Classes.Page;
using WoWonder_API.Classes.User;
using Client = WoWonder_API.Requests.Client;
using Exception = System.Exception;
using File = Java.IO.File;
using IMethods = WoWonder.Helpers.IMethods;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.Communities.Pages
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Page_ProfileActivity : AppCompatActivity, MaterialDialog.IListCallback,
        MaterialDialog.ISingleButtonCallback
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

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Page_Profile_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Page_Profile_Layout);

                var PagesType = Intent.GetStringExtra("PagesType") ?? "Data not available";
                if (PagesType != "Data not available" && !string.IsNullOrEmpty(PagesType)) Pages_Type = PagesType;

                News_Empty = (LinearLayout) FindViewById(Resource.Id.News_LinerEmpty);
                News_Icon = (TextView) FindViewById(Resource.Id.News_icon);
                Txt_News_Empty = (TextView) FindViewById(Resource.Id.Txt_LabelEmpty);
                Txt_News_start = (TextView) FindViewById(Resource.Id.Txt_LabelStart);
                Btn_Reload = (Button) FindViewById(Resource.Id.reloadPage_Button);
                IMethods.Set_TextViewIcon("2", News_Icon, "\uf119");
                News_Empty.Visibility = ViewStates.Gone;

                ProfileImage = (ImageViewAsync) FindViewById(Resource.Id.image_profile);
                CoverImage = (ImageViewAsync) FindViewById(Resource.Id.iv1);

                HybirdView = (WebView) FindViewById(Resource.Id.hybirdview);
                NestedScrollControll = (NestedScrollView) FindViewById(Resource.Id.ScrollView);

                IconBack = (ImageView) FindViewById(Resource.Id.image_back);

                Edit_AvatarImagePage = (LinearLayout) FindViewById(Resource.Id.LinearEdit);

                Txt_EditPageinfo = (TextView) FindViewById(Resource.Id.tv_EditPageinfo);

                Txt_PageName = (TextView) FindViewById(Resource.Id.Page_name);
                Txt_PageUsername = (TextView) FindViewById(Resource.Id.Page_Username);

                Btn_like = (Button) FindViewById(Resource.Id.likeButton);

                Btn_More = (ImageButton) FindViewById(Resource.Id.morebutton);

                IconLike = (TextView) FindViewById(Resource.Id.IconLike);
                LikeCountText = (TextView) FindViewById(Resource.Id.LikeCountText);

                IconCategory = (TextView) FindViewById(Resource.Id.IconCategory);
                CategoryText = (TextView) FindViewById(Resource.Id.CategoryText);

                IconEdit = (TextView) FindViewById(Resource.Id.IconEdit);
                AboutDesc = (TextView) FindViewById(Resource.Id.aboutdesc);

                FloatingActionButtonView = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonView);

                IMethods.Set_TextViewIcon("1", IconLike, IonIcons_Fonts.Thumbsup);
                IMethods.Set_TextViewIcon("1", IconCategory, IonIcons_Fonts.Pricetag);
                IMethods.Set_TextViewIcon("1", IconEdit, IonIcons_Fonts.Edit);

                if (!Directory.Exists(IMethods.IPath.FolderDcimPage))
                    Directory.CreateDirectory(IMethods.IPath.FolderDcimPage);

                hybridController = new HybirdViewController(this, HybirdView, null);

                AddEvents_AndControl();
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
                Btn_Reload.Click += BtnReload_OnClick;
                CoverImage.Click += Edit_CoverImagePage_OnClick;
                Btn_like.Click += Btn_like_OnClick;
                Btn_More.Click += BtnMore_OnClick;
                Txt_EditPageinfo.Click += Edit_CoverImagePage_OnClick;
                IconBack.Click += IconBackOnClick;
                Edit_AvatarImagePage.Click += Edit_AvatarImageGroup_OnClick;
                FloatingActionButtonView.Click += FloatingActionButtonViewOnClick;

                hybridController.JavascriptInterface.OnJavascriptInjectionRequest += OnJavascriptInjectionRequest;
                hybridController.DefaultClient.OnPageEventFinished += WoDefaultClient_OnPageEventFinished;
                if (Settings.Show_Error_HybirdView)
                    hybridController.DefaultClient.OnPageEventReceivedError += DefaultClientOnOnPageEventReceivedError;
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
                Btn_Reload.Click -= BtnReload_OnClick;
                CoverImage.Click -= Edit_CoverImagePage_OnClick;
                Btn_like.Click -= Btn_like_OnClick;
                Btn_More.Click -= BtnMore_OnClick;
                Txt_EditPageinfo.Click -= Edit_CoverImagePage_OnClick;
                IconBack.Click -= IconBackOnClick;
                Edit_AvatarImagePage.Click -= Edit_AvatarImageGroup_OnClick;
                FloatingActionButtonView.Click -= FloatingActionButtonViewOnClick;

                hybridController.JavascriptInterface.OnJavascriptInjectionRequest -= OnJavascriptInjectionRequest;
                hybridController.DefaultClient.OnPageEventFinished -= WoDefaultClient_OnPageEventFinished;
                if (Settings.Show_Error_HybirdView)
                    hybridController.DefaultClient.OnPageEventReceivedError -= DefaultClientOnOnPageEventReceivedError;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        // Add Data This Page By Id using Local List
        public void AddEvents_AndControl()
        {
            try
            {
                if (Pages_Type == "Liked_UserPages")
                {
                    _LikedPages_Data =
                        JsonConvert.DeserializeObject<Get_User_Data_Object.Liked_Pages>(
                            Intent.GetStringExtra("UserPages"));
                    if (_LikedPages_Data != null)
                    {
                        Pages_Id = _LikedPages_Data.page_id;

                        Edit_AvatarImagePage.Visibility = ViewStates.Gone;
                        Txt_EditPageinfo.Visibility = ViewStates.Gone;

                        var AvatarSplit = _LikedPages_Data.avatar.Split('/').Last();
                        var getImage_Avatar =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, AvatarSplit);
                        if (getImage_Avatar != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(ProfileImage, "no_profile_image.png", getImage_Avatar);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage,
                                _LikedPages_Data.avatar);
                            ImageServiceLoader.Load_Image(ProfileImage, "no_profile_image.png",
                                _LikedPages_Data.avatar);
                        }

                        var CoverSplit = _LikedPages_Data.cover.Split('/').Last();
                        var getImage_Cover =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, CoverSplit);
                        if (getImage_Cover != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage,
                                _LikedPages_Data.cover);
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", _LikedPages_Data.cover);
                        }

                        Txt_PageUsername.Text = "@" + _LikedPages_Data.username;
                        Txt_PageName.Text = _LikedPages_Data.name;

                        CategoriesController cat = new CategoriesController();
                        CategoryText.Text = cat.Get_Translate_Categories_Communities(_LikedPages_Data.page_category, _LikedPages_Data.category);

                        var readMoreOption = new ReadMoreOption.Builder(this)
                            .TextLength(200)
                            .MoreLabel(GetText(Resource.String.Lbl_ReadMore))
                            .LessLabel(GetText(Resource.String.Lbl_ReadLess))
                            .MoreLabelColor(Color.ParseColor(Settings.MainColor))
                            .LessLabelColor(Color.ParseColor(Settings.MainColor))
                            .LabelUnderLine(true)
                            .Build();

                        if (IMethods.Fun_String.StringNullRemover(_LikedPages_Data.about) != "Empty")
                        {
                            var about = IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_LikedPages_Data.about));
                            readMoreOption.AddReadMoreTo(AboutDesc, about);
                        }
                        else
                        {
                            readMoreOption.AddReadMoreTo(AboutDesc, GetText(Resource.String.Lbl_Empty));
                        }

                        if (_LikedPages_Data.is_page_onwer)
                        {
                            isOwner_Pages = _LikedPages_Data.is_page_onwer;

                            Edit_AvatarImagePage.Visibility = ViewStates.Visible;
                            Txt_EditPageinfo.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            isOwner_Pages = _LikedPages_Data.is_page_onwer;

                            Edit_AvatarImagePage.Visibility = ViewStates.Gone;
                            Txt_EditPageinfo.Visibility = ViewStates.Gone;
                        }

                        LikeCountText.Text = "0";

                        Pages_Url = _LikedPages_Data.url;
                        Pages_Name = _LikedPages_Data.page_name;
                        Pages_Descriptions =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_LikedPages_Data.about));
                    }
                }
                else if (Pages_Type == "Liked_MyPages")
                {
                    _MyPages_Data =
                        JsonConvert.DeserializeObject<Get_Community_Object.Page>(Intent.GetStringExtra("MyPages"));
                    if (_MyPages_Data != null)
                    {
                        Pages_Id = _MyPages_Data.PageId;

                        var AvatarSplit = _MyPages_Data.Avatar.Split('/').Last();
                        var getImage_Avatar =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, AvatarSplit);
                        if (getImage_Avatar != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(ProfileImage, "no_profile_image.png", getImage_Avatar);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage,
                                _MyPages_Data.Avatar);
                            ImageServiceLoader.Load_Image(ProfileImage, "no_profile_image.png", _MyPages_Data.Avatar);
                        }

                        var CoverSplit = _MyPages_Data.Cover.Split('/').Last();
                        var getImage_Cover =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, CoverSplit);
                        if (getImage_Cover != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage,
                                _MyPages_Data.Cover);
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", _MyPages_Data.Cover);
                        }


                        Txt_PageUsername.Text = "@" + _MyPages_Data.Username;
                        Txt_PageName.Text = _MyPages_Data.Name;

                        CategoriesController cat = new CategoriesController();
                        CategoryText.Text = cat.Get_Translate_Categories_Communities(_MyPages_Data.PageCategory, _MyPages_Data.Category);

                        var readMoreOption = new ReadMoreOption.Builder(this)
                            .TextLength(200)
                            .MoreLabel(GetText(Resource.String.Lbl_ReadMore))
                            .LessLabel(GetText(Resource.String.Lbl_ReadLess))
                            .MoreLabelColor(Color.ParseColor(Settings.MainColor))
                            .LessLabelColor(Color.ParseColor(Settings.MainColor))
                            .LabelUnderLine(true)
                            .Build();

                        if (IMethods.Fun_String.StringNullRemover(_MyPages_Data.About) != "Empty")
                        {
                            var about = IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_MyPages_Data.About));
                            readMoreOption.AddReadMoreTo(AboutDesc, about);
                        }
                        else
                        {
                            readMoreOption.AddReadMoreTo(AboutDesc, GetText(Resource.String.Lbl_Empty));
                        }

                        if (_MyPages_Data.IsPageOnwer)
                        {
                            isOwner_Pages = _MyPages_Data.IsPageOnwer;

                            Edit_AvatarImagePage.Visibility = ViewStates.Visible;
                            Txt_EditPageinfo.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            isOwner_Pages = _MyPages_Data.IsPageOnwer;

                            Edit_AvatarImagePage.Visibility = ViewStates.Gone;
                            Txt_EditPageinfo.Visibility = ViewStates.Gone;
                        }


                        LikeCountText.Text = "0";
                        Btn_like.Text = GetText(Resource.String.Btn_Liked);

                        Pages_Url = _MyPages_Data.Url;
                        Pages_Name = _MyPages_Data.PageName;
                        Pages_Descriptions =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_MyPages_Data.About));
                    }
                }
                else if (Pages_Type == "Saerch_Pages")
                {
                    _SearchPages_Data =
                        JsonConvert.DeserializeObject<Get_Search_Object.Page>(Intent.GetStringExtra("SaerchPages"));
                    if (_SearchPages_Data != null)
                    {
                        Pages_Id = _SearchPages_Data.PageId;

                        var AvatarSplit = _SearchPages_Data.Avatar.Split('/').Last();
                        var getImage_Avatar =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, AvatarSplit);
                        if (getImage_Avatar != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(ProfileImage, "no_profile_image.png", getImage_Avatar);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage,
                                _SearchPages_Data.Avatar);
                            ImageServiceLoader.Load_Image(ProfileImage, "no_profile_image.png",
                                _SearchPages_Data.Avatar);
                        }

                        var CoverSplit = _SearchPages_Data.Cover.Split('/').Last();
                        var getImage_Cover =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, CoverSplit);
                        if (getImage_Cover != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage,
                                _SearchPages_Data.Cover);
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", _SearchPages_Data.Cover);
                        }

                        Txt_PageUsername.Text = "@" + _SearchPages_Data.Username;
                        Txt_PageName.Text = _SearchPages_Data.Name;

                        CategoriesController cat = new CategoriesController();
                        CategoryText.Text = cat.Get_Translate_Categories_Communities(_SearchPages_Data.PageCategory, _SearchPages_Data.Category);

                        var readMoreOption = new ReadMoreOption.Builder(this)
                            .TextLength(200)
                            .MoreLabel(GetText(Resource.String.Lbl_ReadMore))
                            .LessLabel(GetText(Resource.String.Lbl_ReadLess))
                            .MoreLabelColor(Color.ParseColor(Settings.MainColor))
                            .LessLabelColor(Color.ParseColor(Settings.MainColor))
                            .LabelUnderLine(true)
                            .Build();

                        if (IMethods.Fun_String.StringNullRemover(_SearchPages_Data.About) != "Empty")
                        {
                            var about = IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_SearchPages_Data.About));
                            readMoreOption.AddReadMoreTo(AboutDesc, about);
                        }
                        else
                        {
                            readMoreOption.AddReadMoreTo(AboutDesc, GetText(Resource.String.Lbl_Empty));
                        }

                        if (_SearchPages_Data.IsPageOnwer)
                        {
                            isOwner_Pages = _SearchPages_Data.IsPageOnwer;

                            Edit_AvatarImagePage.Visibility = ViewStates.Visible;
                            Txt_EditPageinfo.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            isOwner_Pages = _SearchPages_Data.IsPageOnwer;

                            Edit_AvatarImagePage.Visibility = ViewStates.Gone;
                            Txt_EditPageinfo.Visibility = ViewStates.Gone;
                        }


                        LikeCountText.Text = "0";

                        Pages_Url = _SearchPages_Data.Url;
                        Pages_Name = _SearchPages_Data.PageName;
                        Pages_Descriptions =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_SearchPages_Data.About));
                    }
                }
                else if (Pages_Type == "Liked_NotifyPages")
                {
                    _NotifyPages_Data =
                        JsonConvert.DeserializeObject<Get_General_Data_Object.Notification>(
                            Intent.GetStringExtra("NotifyPages"));
                    if (_NotifyPages_Data != null) Pages_Id = _NotifyPages_Data.page_id;
                }
                else if (Pages_Type == "Liked_PromotedPages")
                {
                    _PromotedPages_Data =
                        JsonConvert.DeserializeObject<Get_General_Data_Object.Promoted_Pages>(
                            Intent.GetStringExtra("PromotedPages"));
                    if (_PromotedPages_Data != null)
                    {
                        Pages_Id = _PromotedPages_Data.page_id;

                        var AvatarSplit = _PromotedPages_Data.avatar.Split('/').Last();
                        var getImage_Avatar =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, AvatarSplit);
                        if (getImage_Avatar != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(ProfileImage, "no_profile_image.png", getImage_Avatar);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage,
                                _PromotedPages_Data.avatar);
                            ImageServiceLoader.Load_Image(ProfileImage, "no_profile_image.png",
                                _PromotedPages_Data.avatar);
                        }

                        var CoverSplit = _PromotedPages_Data.cover.Split('/').Last();
                        var getImage_Cover =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, CoverSplit);
                        if (getImage_Cover != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage,
                                _PromotedPages_Data.cover);
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", _PromotedPages_Data.cover);
                        }

                        Txt_PageUsername.Text = "@" + _PromotedPages_Data.username;
                        Txt_PageName.Text = _PromotedPages_Data.name;

                        CategoriesController cat = new CategoriesController();
                        CategoryText.Text = cat.Get_Translate_Categories_Communities(_PromotedPages_Data.page_category, _PromotedPages_Data.category);

                        var readMoreOption = new ReadMoreOption.Builder(this)
                            .TextLength(200)
                            .MoreLabel(GetText(Resource.String.Lbl_ReadMore))
                            .LessLabel(GetText(Resource.String.Lbl_ReadLess))
                            .MoreLabelColor(Color.ParseColor(Settings.MainColor))
                            .LessLabelColor(Color.ParseColor(Settings.MainColor))
                            .LabelUnderLine(true)
                            .Build();

                        if (IMethods.Fun_String.StringNullRemover(_PromotedPages_Data.about) != "Empty")
                        {
                            var about = IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_PromotedPages_Data.about));
                            readMoreOption.AddReadMoreTo(AboutDesc, about);
                        }
                        else
                        {
                            readMoreOption.AddReadMoreTo(AboutDesc, GetText(Resource.String.Lbl_Empty));
                        }

                        if (_PromotedPages_Data.is_page_onwer)
                        {
                            isOwner_Pages = _PromotedPages_Data.is_page_onwer;

                            Edit_AvatarImagePage.Visibility = ViewStates.Visible;
                            Txt_EditPageinfo.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            isOwner_Pages = _PromotedPages_Data.is_page_onwer;

                            Edit_AvatarImagePage.Visibility = ViewStates.Gone;
                            Txt_EditPageinfo.Visibility = ViewStates.Gone;
                        }

                        LikeCountText.Text = "0";

                        Pages_Url = _PromotedPages_Data.url;
                        Pages_Name = _PromotedPages_Data.page_name;
                        Pages_Descriptions =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_PromotedPages_Data.about));
                    }
                }
                else if (Pages_Type == "Liked_WebUserPages")
                {
                    var pagesId = Intent.GetStringExtra("WebUserPages_ID") ?? "Data not available";
                    if (pagesId != "Data not available" && !string.IsNullOrEmpty(pagesId)) Pages_Id = pagesId;
                }

                //Set WebView and Load url to be rendered on WebView
                if (!IMethods.CheckConnectivity())
                {
                    HybirdView.Visibility = ViewStates.Gone;
                    News_Empty.Visibility = ViewStates.Visible;

                    Txt_News_Empty.Text = GetText(Resource.String.Lbl_Empty_News);
                    Txt_News_start.Text = GetText(Resource.String.Lbl_CheckYourInternetConnection);
                }
                else
                {
                    HybirdView.Visibility = ViewStates.Visible;
                    News_Empty.Visibility = ViewStates.Gone;

                    if (!string.IsNullOrEmpty(Pages_Id))
                    {
                        Get_DataPage_Api(Pages_Id);

                        if (Settings.ClearCachSystem)
                            hybridController.HybirdView.ClearCache(true);

                        if (Settings.FlowDirection_RightToLeft)
                            hybridController.LoadUrl(Current.URLS.UrlInstance.API_Get_News_Feed_Page + Pages_Id +
                                                     "&lang=arabic");
                        else
                            hybridController.LoadUrl(Current.URLS.UrlInstance.API_Get_News_Feed_Page + Pages_Id);
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_something_went_wrong), ToastLength.Short)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        // Get Data This Page By Id 
        public async void Get_DataPage_Api(string idPage)
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
                    var (Api_status, Respond) = await Client.Page.Get_Page_Data(idPage);
                    if (Api_status == 200)
                    {
                        if (Respond is Get_Page_Data_Object result)
                        {
                            _Page_Data = result.page_data;

                            //Extra  
                            LikeCountText.Text = _Page_Data.likes_count;

                            if (result.page_data.is_liked)
                                Btn_like.Text = GetText(Resource.String.Btn_Liked);
                            else
                                Btn_like.Text = GetText(Resource.String.Btn_Like);

                            if (result.page_data.is_page_onwer)
                            {
                                isOwner_Pages = result.page_data.is_page_onwer;

                                Edit_AvatarImagePage.Visibility = ViewStates.Visible;
                                Txt_EditPageinfo.Visibility = ViewStates.Visible;
                            }
                            else
                            {
                                isOwner_Pages = result.page_data.is_page_onwer;

                                Edit_AvatarImagePage.Visibility = ViewStates.Gone;
                                Txt_EditPageinfo.Visibility = ViewStates.Gone;
                            }

                            var AvatarSplit = result.page_data.avatar.Split('/').Last();
                            var getImage_Avatar =
                                IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, AvatarSplit);
                            if (getImage_Avatar != "File Dont Exists")
                            {
                                ImageServiceLoader.Load_Image(ProfileImage, "no_profile_image.png", getImage_Avatar);
                            }
                            else
                            {
                                IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage,
                                    result.page_data.avatar);
                                ImageServiceLoader.Load_Image(ProfileImage, "no_profile_image.png",
                                    result.page_data.avatar);
                            }

                            var CoverSplit = result.page_data.cover.Split('/').Last();
                            var getImage_Cover =
                                IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, CoverSplit);
                            if (getImage_Cover != "File Dont Exists")
                            {
                                ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                            }
                            else
                            {
                                IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage,
                                    result.page_data.cover);
                                ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg",
                                    result.page_data.cover);
                            }

                            Txt_PageUsername.Text = "@" + result.page_data.username;
                            Txt_PageName.Text = result.page_data.name;

                            CategoriesController cat = new CategoriesController();
                            CategoryText.Text = cat.Get_Translate_Categories_Communities(result.page_data.page_category, result.page_data.category);

                            var readMoreOption = new ReadMoreOption.Builder(this)
                                .TextLength(200)
                                .MoreLabel(GetText(Resource.String.Lbl_ReadMore))
                                .LessLabel(GetText(Resource.String.Lbl_ReadLess))
                                .MoreLabelColor(Color.ParseColor(Settings.MainColor))
                                .LessLabelColor(Color.ParseColor(Settings.MainColor))
                                .LabelUnderLine(true)
                                .Build();

                            if (IMethods.Fun_String.StringNullRemover(result.page_data.about) != "Empty")
                            {
                                var about = IMethods.Fun_String.DecodeString(
                                    IMethods.Fun_String.DecodeStringWithEnter(result.page_data.about));
                                readMoreOption.AddReadMoreTo(AboutDesc, about);
                            }
                            else
                            {
                                readMoreOption.AddReadMoreTo(AboutDesc, GetText(Resource.String.Lbl_Empty));
                            }


                            Pages_Url = result.page_data.url;
                            Pages_Name = result.page_data.page_name;
                            Pages_Descriptions =
                                IMethods.Fun_String.DecodeString(
                                    IMethods.Fun_String.DecodeStringWithEnter(result.page_data.about));
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
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);

                if (!string.IsNullOrEmpty(Pages_Id))
                    Get_DataPage_Api(Pages_Id);
                else
                    Toast.MakeText(this, GetString(Resource.String.Lbl_something_went_wrong), ToastLength.Short).Show();
            }
        }

        #region Update Image Avatar && Cover

        // Function Update Image Page : Avatar && Cover
        public async void Update_ImagePage_Api(string type, string path)
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
                    if (type == "Avatar")
                    {
                        var (api_status, respond) = await Client.Page.Update_Page_Avatar(Pages_Id, path);
                        if (api_status == 200)
                        {
                            if (respond is Update_Page_Data_Object result)
                            {
                                Toast.MakeText(this, result.message, ToastLength.Short).Show();

                                //Set image 
                                var file = Uri.FromFile(new File(path));
                                var ImageTrancform = ImageService.Instance.LoadFile(file.Path);
                                ImageTrancform.LoadingPlaceholder("ImagePlacholder.jpg", ImageSource.CompiledResource);
                                ImageTrancform.ErrorPlaceholder("ImagePlacholder.jpg", ImageSource.CompiledResource);
                                ImageTrancform.DownSampleMode(InterpolationMode.Medium);
                                ImageTrancform.Retry(3, 5000);
                                ImageTrancform.WithCache(CacheType.All);
                                ImageTrancform.Into(ProfileImage);
                            }
                        }
                        else if (api_status == 400)
                        {
                            if (respond is Error_Object error)
                            {
                                var errortext = error._errors.Error_text;
                                //Toast.MakeText(this, errortext, ToastLength.Short).Show();

                                if (errortext.Contains("Invalid or expired access_token"))
                                    API_Request.Logout(this);
                            }
                        }
                        else if (api_status == 404)
                        {
                            var error = respond.ToString();
                            //Toast.MakeText(this, error, ToastLength.Short).Show();
                        }
                    }
                    else if (type == "Cover")
                    {
                        var (api_status, respond) = await Client.Page.Update_Page_Cover(Pages_Id, path);
                        if (api_status == 200)
                        {
                            if (respond is Update_Page_Data_Object result)
                            {
                                Toast.MakeText(this, result.message, ToastLength.Short).Show();

                                //Set image 
                                var file = Uri.FromFile(new File(path));
                                var imageTrancform = ImageService.Instance.LoadFile(file.Path);
                                imageTrancform.LoadingPlaceholder("ImagePlacholder.jpg", ImageSource.CompiledResource);
                                imageTrancform.ErrorPlaceholder("ImagePlacholder.jpg", ImageSource.CompiledResource);
                                imageTrancform.DownSampleMode(InterpolationMode.Medium);
                                imageTrancform.Retry(3, 5000);
                                imageTrancform.WithCache(CacheType.All);
                                imageTrancform.Into(CoverImage);
                            }
                        }
                        else if (api_status == 400)
                        {
                            if (respond is Error_Object error)
                            {
                                var errortext = error._errors.Error_text;
                                //Toast.MakeText(this, errortext, ToastLength.Short).Show();

                                if (errortext.Contains("Invalid or expired access_token"))
                                    API_Request.Logout(this);
                            }
                        }
                        else if (api_status == 404)
                        {
                            var error = respond.ToString();
                            //Toast.MakeText(this, error, ToastLength.Short).Show();
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

        //Permission
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
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimPage,
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
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                //If its from Camera or Gallary
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
                                var pathimg = "";
                                if (ImageType == "Cover")
                                {
                                    pathimg = resultUri.Path;
                                    Update_ImagePage_Api(ImageType, pathimg);
                                }
                                else if (ImageType == "Avatar")
                                {
                                    pathimg = resultUri.Path;
                                    Update_ImagePage_Api(ImageType, pathimg);
                                }
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong),
                                    ToastLength.Long).Show();
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
                else if (requestCode == 2500)
                {
                    hybridController.EvaluateJavascript("Wo_GetNewPosts();");
                }
                else if (requestCode == 3500)
                {
                    string ID = data.GetStringExtra("PostId");
                    string Text = data.GetStringExtra("PostText");

                    string JavaCode = "$('#post-' + " + ID + ").find('#edit-post').attr('onclick', '{*type*:*edit_post*,*post_id*:*" + ID + "*,*edit_text*:*" + Text + "*}');";
                    string Decode = JavaCode.Replace("*", "&quot;");

                    hybridController.EvaluateJavascript(Decode);
                    hybridController.EvaluateJavascript("$('#post-' + " + ID + ").find('.post-description p').html('" + Text + "');");
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Toast.MakeText(this, "Error path image", ToastLength.Short).Show();
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

        private ImageViewAsync ProfileImage;
        private ImageViewAsync CoverImage;
        private WebView HybirdView;

        public LinearLayout News_Empty;
        public TextView News_Icon;
        public TextView Txt_News_Empty;
        public TextView Txt_News_start;
        private Button Btn_Reload;


        private HybirdViewController hybridController;

        private NestedScrollView NestedScrollControll;

        private ImageView IconBack;

        private TextView Txt_PageName;
        private TextView Txt_PageUsername;

        private Button Btn_like;
        private ImageButton Btn_More;

        private TextView CategoryText;
        private TextView IconCategory;

        private TextView IconEdit;
        private TextView AboutDesc;

        public TextView IconLike;
        public TextView LikeCountText;

        private FloatingActionButton FloatingActionButtonView;

        private LinearLayout Edit_AvatarImagePage;
        private TextView Txt_EditPageinfo;

        private string Pages_Type = "";
        private string Pages_Id = "";
        private string ImageType = "";
        private bool isOwner_Pages;
        private string Pages_Url, Pages_Name, Pages_Descriptions;

        private Get_User_Data_Object.Liked_Pages _LikedPages_Data;
        private Get_Community_Object.Page _MyPages_Data;
        private Get_Search_Object.Page _SearchPages_Data;
        private Get_General_Data_Object.Notification _NotifyPages_Data;
        private Get_Page_Data_Object.Page_Data _Page_Data;
        private Get_General_Data_Object.Promoted_Pages _PromotedPages_Data;

        #endregion

        #region Event

        //Event Like => like , dislike 
        private async void Btn_like_OnClick(object sender, EventArgs eventArgs)
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
                    var (Api_status, Respond) = await Client.Page.Like_Page(Pages_Id);
                    if (Api_status == 200)
                    {
                        if (Respond is Like_Page_Object result)
                        {
                            Toast.MakeText(this, result.like_status, ToastLength.Short).Show();

                            // Add Page Or Remove in DB 
                            var dbDatabase = new SqLiteDatabase();
                            if (Pages_Type == "Liked_UserPages")
                            {
                                var item = _LikedPages_Data;
                                var data = new DataTables.PageTB
                                {
                                    PageId = item.page_id,
                                    UserId = item.user_id,
                                    PageName = item.page_name,
                                    PageTitle = item.page_title,
                                    PageDescription = item.page_description,
                                    Avatar = item.avatar,
                                    Cover = item.cover,
                                    PageCategory = item.page_category,
                                    Website = item.website,
                                    Facebook = item.facebook,
                                    Google = item.google,
                                    Vk = item.vk,
                                    Twitter = item.twitter,
                                    Linkedin = item.linkedin,
                                    Company = item.company,
                                    Phone = item.phone,
                                    Address = item.address,
                                    CallActionType = item.call_action_type,
                                    CallActionTypeUrl = item.call_action_type_url,
                                    BackgroundImage = item.background_image,
                                    BackgroundImageStatus = item.background_image_status,
                                    Instgram = item.instgram,
                                    Youtube = item.youtube,
                                    Verified = item.verified,
                                    Registered = item.registered,
                                    Boosted = item.boosted,
                                    About = item.about,
                                    Id = item.id,
                                    Type = item.type,
                                    Url = item.url,
                                    Name = item.name,
                                    //Rating = item.rating,
                                    Category = item.category,
                                    IsPageOnwer = Convert.ToString(item.is_page_onwer),
                                    Username = item.username
                                };
                                dbDatabase.Insert_Or_Delete_OnePagesTable(item.page_id, data);
                            }
                            else if (Pages_Type == "Liked_MyPages")
                            {
                                var item = _MyPages_Data;
                                var data = new DataTables.PageTB
                                {
                                    PageId = item.PageId,
                                    UserId = item.UserId,
                                    PageName = item.PageName,
                                    PageTitle = item.PageTitle,
                                    PageDescription = item.PageDescription,
                                    Avatar = item.Avatar,
                                    Cover = item.Cover,
                                    PageCategory = item.PageCategory,
                                    Website = item.Website,
                                    Facebook = item.Facebook,
                                    Google = item.Google,
                                    Vk = item.Vk,
                                    Twitter = item.Twitter,
                                    Linkedin = item.Linkedin,
                                    Company = item.Company,
                                    Phone = item.Phone,
                                    Address = item.Address,
                                    CallActionType = item.CallActionType,
                                    CallActionTypeUrl = item.CallActionTypeUrl,
                                    BackgroundImage = item.BackgroundImage,
                                    BackgroundImageStatus = item.BackgroundImageStatus,
                                    Instgram = item.Instgram,
                                    Youtube = item.Youtube,
                                    Verified = item.Verified,
                                    Registered = item.Registered,
                                    Boosted = item.Boosted,
                                    About = item.About,
                                    Id = item.Id,
                                    Type = item.Type,
                                    Url = item.Url,
                                    Name = item.Name,
                                    //Rating = item.Rating,
                                    Category = item.Category,
                                    IsPageOnwer = Convert.ToString(item.IsPageOnwer),
                                    Username = item.Username
                                };
                                dbDatabase.Insert_Or_Delete_OnePagesTable(item.PageId, data);
                            }
                            else if (Pages_Type == "Saerch_Pages")
                            {
                                var item = _SearchPages_Data;
                                var data = new DataTables.PageTB
                                {
                                    PageId = item.PageId,
                                    UserId = item.UserId,
                                    PageName = item.PageName,
                                    PageTitle = item.PageTitle,
                                    PageDescription = item.PageDescription,
                                    Avatar = item.Avatar,
                                    Cover = item.Cover,
                                    PageCategory = item.PageCategory,
                                    Website = item.Website,
                                    Facebook = item.Facebook,
                                    Google = item.Google,
                                    Vk = item.Vk,
                                    Twitter = item.Twitter,
                                    Linkedin = item.Linkedin,
                                    Company = item.Company,
                                    Phone = item.Phone,
                                    Address = item.Address,
                                    CallActionType = item.CallActionType,
                                    CallActionTypeUrl = item.CallActionTypeUrl,
                                    BackgroundImage = item.BackgroundImage,
                                    BackgroundImageStatus = item.BackgroundImageStatus,
                                    Instgram = item.Instgram,
                                    Youtube = item.Youtube,
                                    Verified = item.Verified,
                                    Registered = item.Registered,
                                    Boosted = item.Boosted,
                                    About = item.About,
                                    Id = item.Id,
                                    Type = item.Type,
                                    Url = item.Url,
                                    Name = item.Name,
                                    //Rating = item.Rating,
                                    Category = item.Category,
                                    IsPageOnwer = Convert.ToString(item.IsPageOnwer),
                                    Username = item.Username
                                };
                                dbDatabase.Insert_Or_Delete_OnePagesTable(item.PageId, data);
                            }
                            else if (Pages_Type == "Liked_PromotedPages")
                            {
                                var item = _PromotedPages_Data;
                                var data = new DataTables.PageTB
                                {
                                    PageId = item.page_id,
                                    UserId = item.user_id,
                                    PageName = item.page_name,
                                    PageTitle = item.page_title,
                                    PageDescription = item.page_description,
                                    Avatar = item.avatar,
                                    Cover = item.cover,
                                    PageCategory = item.page_category,
                                    Website = item.website,
                                    Facebook = item.facebook,
                                    Google = item.google,
                                    Vk = item.vk,
                                    Twitter = item.twitter,
                                    Linkedin = item.linkedin,
                                    Company = item.company,
                                    Phone = item.phone,
                                    Address = item.address,
                                    CallActionType = item.call_action_type,
                                    CallActionTypeUrl = item.call_action_type_url,
                                    BackgroundImage = item.background_image,
                                    BackgroundImageStatus = item.background_image_status,
                                    Instgram = item.instgram,
                                    Youtube = item.youtube,
                                    Verified = item.verified,
                                    Registered = item.registered,
                                    Boosted = item.boosted,
                                    About = item.about,
                                    Id = item.id,
                                    Type = item.type,
                                    Url = item.url,
                                    Name = item.name,
                                    //Rating = item.rating,
                                    Category = item.category,
                                    IsPageOnwer = Convert.ToString(item.is_page_onwer),
                                    Username = item.username
                                };
                                dbDatabase.Insert_Or_Delete_OnePagesTable(item.page_id, data);
                            }
                            else
                            {
                                var item = _Page_Data;
                                var data = new DataTables.PageTB
                                {
                                    PageId = item.page_id,
                                    UserId = item.user_id,
                                    PageName = item.page_name,
                                    PageTitle = item.page_title,
                                    PageDescription = item.page_description,
                                    Avatar = item.avatar,
                                    Cover = item.cover,
                                    PageCategory = item.page_category,
                                    Website = item.website,
                                    Facebook = item.facebook,
                                    Google = item.google,
                                    Vk = item.vk,
                                    Twitter = item.twitter,
                                    Linkedin = item.linkedin,
                                    Company = item.company,
                                    Phone = item.phone,
                                    Address = item.address,
                                    CallActionType = item.call_action_type,
                                    CallActionTypeUrl = item.call_action_type_url,
                                    BackgroundImage = item.background_image,
                                    //BackgroundImageStatus = item.background_image_status,
                                    Instgram = item.instgram,
                                    Youtube = item.youtube,
                                    Verified = item.verified,
                                    Registered = item.registered,
                                    Boosted = item.boosted,
                                    About = item.about,
                                    //Id = item.id,
                                    //Type = item.type,
                                    Url = item.url,
                                    Name = item.name,
                                    //Rating = item.rating,
                                    Category = item.category,
                                    IsPageOnwer = Convert.ToString(item.is_page_onwer),
                                    Username = item.username
                                };
                                dbDatabase.Insert_Or_Delete_OnePagesTable(item.page_id, data);
                            }

                            dbDatabase.Dispose();
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
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Update Image avatar Group
        private void Edit_AvatarImageGroup_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (isOwner_Pages)
                {
                    ImageType = "Avatar";
                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                    {
                        //Open Image 
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimPage,
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
                            var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimPage,
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
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Update Image Cover Page
        private void Edit_CoverImagePage_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (isOwner_Pages)
                {
                    ImageType = "Cover";
                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                    {
                        //Open Image 
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimPage,
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
                            var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimPage,
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
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Icon Back
        private void IconBackOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                Finish();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event More >> Show Menu (CopeLink , Share , Edit)
        private void BtnMore_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var ctw = new ContextThemeWrapper(this, Resource.Style.PopupMenuStyle);
                var popup = new PopupMenu(ctw, Btn_More);
                if (isOwner_Pages)
                    popup.MenuInflater.Inflate(Resource.Menu.MoreCommunities_Menu, popup.Menu);
                else
                    popup.MenuInflater.Inflate(Resource.Menu.MoreCommunities_NotEdit_Menu, popup.Menu);
                popup.Show();
                popup.MenuItemClick += (o, e) =>
                {
                    try
                    {
                        var Id = e.Item.ItemId;
                        switch (Id)
                        {
                            case Resource.Id.menu_CopeLink:
                                OnCopyLink_Button_Click();
                                break;

                            case Resource.Id.menu_Share:
                                OnShare_Button_Click();
                                break;

                            case Resource.Id.menu_Edit:
                                EditInfoPage_OnClick();
                                break;
                        }
                    }
                    catch (Exception exception)
                    {
                        Crashes.TrackError(exception);
                    }
                };
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Menu >> Copy Link
        private void OnCopyLink_Button_Click()
        {
            try
            {
                var clipboardManager = (ClipboardManager) GetSystemService(ClipboardService);

                var clipData = ClipData.NewPlainText("text", Pages_Url);
                clipboardManager.PrimaryClip = clipData;

                Toast.MakeText(this, GetText(Resource.String.Lbl_Copied), ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Menu >> Share
        private async void OnShare_Button_Click()
        {
            try
            {
                //Share Plugin same as video
                if (!CrossShare.IsSupported) return;

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = Pages_Name,
                    Text = Pages_Descriptions,
                    Url = Pages_Url
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Menu >> Edit Info Like if user == is_owner 
        private void EditInfoPage_OnClick()
        {
            try
            {
                if (isOwner_Pages)
                {
                    var Int = new Intent(this, typeof(EditInfoPage_Activity));
                    if (Pages_Type == "Liked_UserPages")
                    {
                        var item = _LikedPages_Data;
                        Int.PutExtra("UserPages", JsonConvert.SerializeObject(item));
                        Int.PutExtra("PagesType", "Liked_UserPages");
                    }
                    else if (Pages_Type == "Liked_MyPages")
                    {
                        var item = _MyPages_Data;
                        Int.PutExtra("MyPages", JsonConvert.SerializeObject(item));
                        Int.PutExtra("PagesType", "Liked_MyPages");
                    }
                    else if (Pages_Type == "Saerch_Pages")
                    {
                        var item = _SearchPages_Data;
                        Int.PutExtra("SaerchPages", JsonConvert.SerializeObject(item));
                        Int.PutExtra("PagesType", "Saerch_Pages");
                    }
                    else if (Pages_Type == "Liked_PromotedPages")
                    {
                        var item = _PromotedPages_Data;
                        Int.PutExtra("PromotedPages", JsonConvert.SerializeObject(item));
                        Int.PutExtra("PagesType", "Liked_PromotedPages");
                    }
                    else
                    {
                        Int.PutExtra("PageData", JsonConvert.SerializeObject(_Page_Data));
                        Int.PutExtra("PagesType", "Page_Data");
                    }

                    Int.PutExtra("PagesId", Pages_Id);
                    StartActivity(Int);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Add New post 
        private void FloatingActionButtonViewOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var Int = new Intent(this, typeof(AddPost_Activity));
                Int.PutExtra("Type", "SocialPage");
                Int.PutExtra("PostId", Pages_Id);
                Int.PutExtra("isOwner", isOwner_Pages.ToString());
                StartActivityForResult(Int, 2500);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion

        #region HybridController

        private Task<string> OnJavascriptInjectionRequest(string eventobj)
        {
            try
            {
                if (!string.IsNullOrEmpty(eventobj))
                    if (eventobj.Contains("type"))
                    {
                        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(eventobj);
                        var type = data["type"].ToString();

                        if (type == "delete_post")
                            RunOnUiThread(() =>
                            {
                                try
                                {
                                    var DialogList = new MaterialDialog.Builder(this);
                                    DialogList.Tag(data["post_id"].ToString());
                                    DialogList.Title(GetText(Resource.String.Lbl_Title_DeletePost));
                                    DialogList.Content(GetText(Resource.String.Lbl_Content_DeletePost));
                                    DialogList.NegativeText(GetText(Resource.String.Lbl_Cancel));
                                    DialogList.OnNegative(this);
                                    DialogList.PositiveText(GetText(Resource.String.Lbl_Delete));
                                    DialogList.OnPositive(this);
                                    DialogList.Build().Show();
                                }
                                catch (Exception exception)
                                {
                                    Crashes.TrackError(exception);
                                }
                            });
                        else if (type == "publisher-box")
                            RunOnUiThread(() =>
                            {
                                var Int = new Intent(this, typeof(AddPost_Activity));
                                Int.PutExtra("Type", "SocialPage");
                                Int.PutExtra("PostId", Pages_Id);
                                Int.PutExtra("isOwner", isOwner_Pages.ToString());
                                StartActivityForResult(Int, 2500);
                            });
                        else
                            return null;
                    }

                return null;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        }

        private void WoDefaultClient_OnPageEventFinished(WebView view, string url)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    if (HybirdView.Visibility != ViewStates.Visible)
                    {
                        HybirdView.Visibility = ViewStates.Visible;
                        News_Empty.Visibility = ViewStates.Gone;
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void DefaultClientOnOnPageEventReceivedError(WebView view, IWebResourceRequest request,
            WebResourceError error, string textError)
        {
            try
            {
                HybirdView.Visibility = ViewStates.Gone;
                News_Empty.Visibility = ViewStates.Visible;

                Txt_News_Empty.Text = textError;
                Txt_News_start.Text = error.Description;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Reload Page
        private void BtnReload_OnClick(object sender, EventArgs e)
        {
            try
            {
                HybirdView.Reload();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void OnSelection(MaterialDialog p0, View p1, int p2, ICharSequence p3)
        {
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (!string.IsNullOrEmpty(p0.Tag.ToString()))
                {
                    if (p1 == DialogAction.Positive)
                    {
                        RunOnUiThread(() =>
                        {
                            var id = p0.Tag.ToString();
                            //Fire Javascript Event
                            hybridController.EvaluateJavascript(
                                "$('#post-' + " + id + ").slideUp(200, function () { $(this).remove();}); ");
                            p0.Dismiss();
                        });

                        //Delete Post from database
                        JsBrigeInvoker.Post_Manager("delete_post", p0.Tag.ToString()).ConfigureAwait(false);
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

        #endregion
    }
}