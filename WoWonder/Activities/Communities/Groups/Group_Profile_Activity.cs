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
using WoWonder_API.Classes.User;
using Client = WoWonder_API.Requests.Client;
using Exception = System.Exception;
using File = Java.IO.File;
using IMethods = WoWonder.Helpers.IMethods;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.Communities.Groups
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Group_Profile_Activity : AppCompatActivity, MaterialDialog.IListCallback,
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

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Group_Profile_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Group_Profile_Layout);

                var groupsType = Intent.GetStringExtra("GroupsType") ?? "Data not available";
                if (groupsType != "Data not available" && !string.IsNullOrEmpty(groupsType)) Groups_Type = groupsType;

                UserProfileImage = (ImageViewAsync) FindViewById(Resource.Id.image_profile);
                CoverImage = (ImageViewAsync) FindViewById(Resource.Id.iv1);

                News_Empty = (LinearLayout) FindViewById(Resource.Id.News_LinerEmpty);
                News_Icon = (TextView) FindViewById(Resource.Id.News_icon);
                Txt_News_Empty = (TextView) FindViewById(Resource.Id.Txt_LabelEmpty);
                Txt_News_start = (TextView) FindViewById(Resource.Id.Txt_LabelStart);
                Btn_Reload = (Button) FindViewById(Resource.Id.reloadPage_Button);
                IMethods.Set_TextViewIcon("2", News_Icon, "\uf119");
                News_Empty.Visibility = ViewStates.Gone;

                HybirdView = (WebView) FindViewById(Resource.Id.hybirdview);
                NestedScrollControll = (NestedScrollView) FindViewById(Resource.Id.ScrollView);

                IconBack = (ImageView) FindViewById(Resource.Id.image_back);

                Edit_AvatarImageGroup = (LinearLayout) FindViewById(Resource.Id.LinearEdit);

                Txt_EditGroupinfo = (TextView) FindViewById(Resource.Id.tv_EditGroupinfo);

                Txt_GroupName = (TextView) FindViewById(Resource.Id.Group_name);
                Txt_GroupUsername = (TextView) FindViewById(Resource.Id.Group_Username);

                Btn_join = (Button) FindViewById(Resource.Id.joinButton);

                Btn_More = (ImageButton) FindViewById(Resource.Id.morebutton);

                IconPrivacy = (TextView) FindViewById(Resource.Id.IconPrivacy);
                PrivacyText = (TextView) FindViewById(Resource.Id.PrivacyText);

                IconCategory = (TextView) FindViewById(Resource.Id.IconCategory);
                CategoryText = (TextView) FindViewById(Resource.Id.CategoryText);

                IconEdit = (TextView) FindViewById(Resource.Id.IconEdit);
                AboutDesc = (TextView) FindViewById(Resource.Id.aboutdesc);

                FloatingActionButtonView = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonView);

                IMethods.Set_TextViewIcon("1", IconPrivacy, IonIcons_Fonts.Earth);
                IMethods.Set_TextViewIcon("1", IconCategory, IonIcons_Fonts.Pricetag);
                IMethods.Set_TextViewIcon("1", IconEdit, IonIcons_Fonts.Edit);

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
                IconBack.Click += IconBackOnClick;
                Edit_AvatarImageGroup.Click += Edit_AvatarImageGroup_OnClick;
                Txt_EditGroupinfo.Click += Edit_CoverImageGroup_OnClick;
                Btn_join.Click += BtnJoin_OnClick;
                Btn_More.Click += BtnMore_OnClick;
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
                IconBack.Click -= IconBackOnClick;
                Edit_AvatarImageGroup.Click -= Edit_AvatarImageGroup_OnClick;
                Txt_EditGroupinfo.Click -= Edit_CoverImageGroup_OnClick;
                Btn_join.Click -= BtnJoin_OnClick;
                Btn_More.Click -= BtnMore_OnClick;
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


        // Add Data This Group By Id using Local List
        public void AddEvents_AndControl()
        {
            try
            {
                if (Groups_Type == "Joined_UserGroups")
                {
                    _JoinedGroups_Data =
                        JsonConvert.DeserializeObject<Get_User_Data_Object.Joined_Groups>(
                            Intent.GetStringExtra("UserGroups"));
                    if (_JoinedGroups_Data != null)
                    {
                        Groups_Id = _JoinedGroups_Data.group_id;

                        Edit_AvatarImageGroup.Visibility = ViewStates.Gone;
                        Txt_EditGroupinfo.Visibility = ViewStates.Gone;

                        var AvatarSplit = _JoinedGroups_Data.avatar.Split('/').Last();
                        var getImage_Avatar =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskGroup, AvatarSplit);
                        if (getImage_Avatar != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskGroup,
                                _JoinedGroups_Data.avatar);
                            ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png",
                                _JoinedGroups_Data.avatar);
                        }

                        var CoverSplit = _JoinedGroups_Data.cover.Split('/').Last();
                        var getImage_Cover =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskGroup, CoverSplit);
                        if (getImage_Cover != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskGroup,
                                _JoinedGroups_Data.cover);
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", _JoinedGroups_Data.cover);
                        }

                        Txt_GroupUsername.Text = "@" + _JoinedGroups_Data.username;
                        Txt_GroupName.Text = _JoinedGroups_Data.name;

                        if (_JoinedGroups_Data.privacy == "1")
                            PrivacyText.Text = GetText(Resource.String.Radio_Public);
                        else
                            PrivacyText.Text = GetText(Resource.String.Radio_Private);

                        CategoriesController cat = new CategoriesController();
                        CategoryText.Text = cat.Get_Translate_Categories_Communities(_JoinedGroups_Data.category_id, _JoinedGroups_Data.category);
                         
                        var readMoreOption = new ReadMoreOption.Builder(this)
                            .TextLength(200)
                            .MoreLabel(GetText(Resource.String.Lbl_ReadMore))
                            .LessLabel(GetText(Resource.String.Lbl_ReadLess))
                            .MoreLabelColor(Color.ParseColor(Settings.MainColor))
                            .LessLabelColor(Color.ParseColor(Settings.MainColor))
                            .LabelUnderLine(true)
                            .Build();

                        if (IMethods.Fun_String.StringNullRemover(_JoinedGroups_Data.about) != "Empty")
                        {
                            var about = IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_JoinedGroups_Data.about));
                            readMoreOption.AddReadMoreTo(AboutDesc, about);
                        }
                        else
                        {
                            readMoreOption.AddReadMoreTo(AboutDesc, GetText(Resource.String.Lbl_Empty));
                        }


                        Groups_Url = _JoinedGroups_Data.url;
                        Groups_Name = _JoinedGroups_Data.group_name;
                        Groups_Descriptions =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_JoinedGroups_Data.about));
                    }
                }
                else if (Groups_Type == "Joined_MyGroups")
                {
                    _MyGroups_Data =
                        JsonConvert.DeserializeObject<Get_Community_Object.Group>(Intent.GetStringExtra("MyGroups"));
                    if (_MyGroups_Data != null)
                    {
                        Groups_Id = _MyGroups_Data.GroupId;

                        var AvatarSplit = _MyGroups_Data.Avatar.Split('/').Last();
                        var getImage_Avatar =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskGroup, AvatarSplit);
                        if (getImage_Avatar != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskGroup,
                                _MyGroups_Data.Avatar);
                            ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png",
                                _MyGroups_Data.Avatar);
                        }

                        var CoverSplit = _MyGroups_Data.Cover.Split('/').Last();
                        var getImage_Cover =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskGroup, CoverSplit);
                        if (getImage_Cover != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskGroup,
                                _MyGroups_Data.Cover);
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", _MyGroups_Data.Cover);
                        }

                        Txt_GroupUsername.Text = "@" + _MyGroups_Data.Username;
                        Txt_GroupName.Text = _MyGroups_Data.GroupName;

                        if (_MyGroups_Data.Privacy == "1")
                            PrivacyText.Text = GetText(Resource.String.Radio_Public);
                        else
                            PrivacyText.Text = GetText(Resource.String.Radio_Private);

                        CategoriesController cat = new CategoriesController();
                        CategoryText.Text = cat.Get_Translate_Categories_Communities(_MyGroups_Data.CategoryId, _MyGroups_Data.Category);

                        var readMoreOption = new ReadMoreOption.Builder(this)
                            .TextLength(200)
                            .MoreLabel(GetText(Resource.String.Lbl_ReadMore))
                            .LessLabel(GetText(Resource.String.Lbl_ReadLess))
                            .MoreLabelColor(Color.ParseColor(Settings.MainColor))
                            .LessLabelColor(Color.ParseColor(Settings.MainColor))
                            .LabelUnderLine(true)
                            .Build();

                        if (IMethods.Fun_String.StringNullRemover(_MyGroups_Data.About) != "Empty")
                        {
                            var about = IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_MyGroups_Data.About));
                            readMoreOption.AddReadMoreTo(AboutDesc, about);
                        }
                        else
                        {
                            readMoreOption.AddReadMoreTo(AboutDesc, GetText(Resource.String.Lbl_Empty));
                        }


                        Groups_Url = _MyGroups_Data.Url;
                        Groups_Name = _MyGroups_Data.GroupName;
                        Groups_Descriptions =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_MyGroups_Data.About));
                    }
                }
                else if (Groups_Type == "Search_Groups")
                {
                    _SearchGroups_Data =
                        JsonConvert.DeserializeObject<Get_Search_Object.Group>(Intent.GetStringExtra("SearchGroups"));
                    if (_SearchGroups_Data != null)
                    {
                        Groups_Id = _SearchGroups_Data.GroupId;

                        var AvatarSplit = _SearchGroups_Data.Avatar.Split('/').Last();
                        var getImage_Avatar =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskGroup, AvatarSplit);
                        if (getImage_Avatar != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskGroup,
                                _SearchGroups_Data.Avatar);
                            ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png",
                                _SearchGroups_Data.Avatar);
                        }

                        var CoverSplit = _SearchGroups_Data.Cover.Split('/').Last();
                        var getImage_Cover =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskGroup, CoverSplit);
                        if (getImage_Cover != "File Dont Exists")
                        {
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                        }
                        else
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskGroup,
                                _SearchGroups_Data.Cover);
                            ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", _SearchGroups_Data.Cover);
                        }

                        Txt_GroupUsername.Text = "@" + _SearchGroups_Data.Username;
                        Txt_GroupName.Text = _SearchGroups_Data.Name;

                        if (_SearchGroups_Data.Privacy == "1")
                            PrivacyText.Text = GetText(Resource.String.Radio_Public);
                        else
                            PrivacyText.Text = GetText(Resource.String.Radio_Private);

                        CategoriesController cat = new CategoriesController();
                        CategoryText.Text = cat.Get_Translate_Categories_Communities(_SearchGroups_Data.CategoryId, _SearchGroups_Data.Category);

                        var readMoreOption = new ReadMoreOption.Builder(this)
                            .TextLength(200)
                            .MoreLabel(GetText(Resource.String.Lbl_ReadMore))
                            .LessLabel(GetText(Resource.String.Lbl_ReadLess))
                            .MoreLabelColor(Color.ParseColor(Settings.MainColor))
                            .LessLabelColor(Color.ParseColor(Settings.MainColor))
                            .LabelUnderLine(true)
                            .Build();

                        if (IMethods.Fun_String.StringNullRemover(_SearchGroups_Data.About) != "Empty")
                        {
                            var about = IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_SearchGroups_Data.About));
                            readMoreOption.AddReadMoreTo(AboutDesc, about);
                        }
                        else
                        {
                            readMoreOption.AddReadMoreTo(AboutDesc, GetText(Resource.String.Lbl_Empty));
                        }

                        Groups_Url = _SearchGroups_Data.Url;
                        Groups_Name = _SearchGroups_Data.GroupName;
                        Groups_Descriptions =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_SearchGroups_Data.About));
                    }
                }
                else if (Groups_Type == "Joined_NotifyGroups")
                {
                    _NotifyPages_Data =
                        JsonConvert.DeserializeObject<Get_General_Data_Object.Notification>(
                            Intent.GetStringExtra("NotifyGroups"));
                    if (_NotifyPages_Data != null) Groups_Id = _NotifyPages_Data.group_id;
                }
                else if (Groups_Type == "Joined_WebUserGroups")
                {
                    var groupsId = Intent.GetStringExtra("WebUserGroups_ID") ?? "Data not available";
                    if (groupsId != "Data not available" && !string.IsNullOrEmpty(groupsId)) Groups_Id = groupsId;
                }

                if (!Directory.Exists(IMethods.IPath.FolderDiskImage))
                    Directory.CreateDirectory(IMethods.IPath.FolderDiskImage);

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

                    //Set to Groups id to load the news feed
                    if (!string.IsNullOrEmpty(Groups_Id))
                    {
                        Get_DataGroup_Api(Groups_Id);

                        if (Settings.ClearCachSystem)
                            hybridController.HybirdView.ClearCache(true);

                        if (Settings.FlowDirection_RightToLeft)
                            hybridController.LoadUrl(Current.URLS.UrlInstance.API_Get_News_Feed_Group + Groups_Id +
                                                     "&lang=arabic");
                        else
                            hybridController.LoadUrl(Current.URLS.UrlInstance.API_Get_News_Feed_Group + Groups_Id);
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

        // Get Data This Group By Id 
        public async void Get_DataGroup_Api(string idGroup)
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
                    var (api_status, respond) = await Client.Group.Get_Group_Data(idGroup);
                    if (api_status == 200)
                    {
                        if (respond is Get_Group_Data_Object result)
                        {
                            _Group_Data = result.group_data;

                            //Extra
                            if (result.group_data.is_joined)
                                Btn_join.Text = GetText(Resource.String.Btn_Joined);
                            else
                                Btn_join.Text = GetText(Resource.String.Btn_Join_Group);

                            if (result.group_data.is_owner)
                            {
                                isOwner_Groups = result.group_data.is_owner;

                                Edit_AvatarImageGroup.Visibility = ViewStates.Visible;
                                Txt_EditGroupinfo.Visibility = ViewStates.Visible;
                            }
                            else
                            {
                                isOwner_Groups = result.group_data.is_owner;

                                Edit_AvatarImageGroup.Visibility = ViewStates.Gone;
                                Txt_EditGroupinfo.Visibility = ViewStates.Gone;
                            }

                            var AvatarSplit = result.group_data.avatar.Split('/').Last();
                            var getImage_Avatar =
                                IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskGroup, AvatarSplit);
                            if (getImage_Avatar != "File Dont Exists")
                            {
                                ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png",
                                    getImage_Avatar);
                            }
                            else
                            {
                                IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskGroup,
                                    result.group_data.avatar);
                                ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png",
                                    result.group_data.avatar);
                            }

                            var CoverSplit = result.group_data.cover.Split('/').Last();
                            var getImage_Cover =
                                IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskGroup, CoverSplit);
                            if (getImage_Cover != "File Dont Exists")
                            {
                                ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                            }
                            else
                            {
                                IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskGroup,
                                    result.group_data.cover);
                                ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg",
                                    result.group_data.cover);
                            }

                            Txt_GroupUsername.Text = "@" + result.group_data.username;
                            Txt_GroupName.Text = result.group_data.name;

                            if (result.group_data.privacy == "1")
                                PrivacyText.Text = GetText(Resource.String.Radio_Public);
                            else
                                PrivacyText.Text = GetText(Resource.String.Radio_Private);

                            CategoriesController cat = new CategoriesController();
                            CategoryText.Text = cat.Get_Translate_Categories_Communities(result.group_data.category_id, result.group_data.category);

                            var readMoreOption = new ReadMoreOption.Builder(this)
                                .TextLength(200)
                                .MoreLabel(GetText(Resource.String.Lbl_ReadMore))
                                .LessLabel(GetText(Resource.String.Lbl_ReadLess))
                                .MoreLabelColor(Color.ParseColor(Settings.MainColor))
                                .LessLabelColor(Color.ParseColor(Settings.MainColor))
                                .LabelUnderLine(true)
                                .Build();

                            if (IMethods.Fun_String.StringNullRemover(result.group_data.about) != "Empty")
                            {
                                var about = IMethods.Fun_String.DecodeString(
                                    IMethods.Fun_String.DecodeStringWithEnter(result.group_data.about));
                                readMoreOption.AddReadMoreTo(AboutDesc, about);
                            }
                            else
                            {
                                readMoreOption.AddReadMoreTo(AboutDesc, GetText(Resource.String.Lbl_Empty));
                            }

                            Groups_Url = result.group_data.url;
                            Groups_Name = result.group_data.group_name;
                            Groups_Descriptions =
                                IMethods.Fun_String.DecodeString(
                                    IMethods.Fun_String.DecodeStringWithEnter(result.group_data.about));
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
            catch (Exception e)
            {
                Crashes.TrackError(e);

                if (!string.IsNullOrEmpty(Groups_Id))
                    Get_DataGroup_Api(Groups_Id);
                else
                    Toast.MakeText(this, GetString(Resource.String.Lbl_something_went_wrong), ToastLength.Short).Show();
            }
        }

        #region Update Image Avatar && Cover

        // Function Update Image Group : Avatar && Cover
        public async void Update_ImageGroup_Api(string type, string path)
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
                        var (api_status, respond) = await Client.Group.Update_Group_Avatar(Groups_Id, path);
                        if (api_status == 200)
                        {
                            if (respond is Update_Group_Data_Object result)
                            {
                                Toast.MakeText(this, result.Message, ToastLength.Short).Show();

                                //Set image 
                                var file = Uri.FromFile(new File(path));
                                var ImageTrancform = ImageService.Instance.LoadFile(file.Path);
                                ImageTrancform.LoadingPlaceholder("ImagePlacholder.jpg", ImageSource.CompiledResource);
                                ImageTrancform.ErrorPlaceholder("ImagePlacholder.jpg", ImageSource.CompiledResource);
                                ImageTrancform.DownSampleMode(InterpolationMode.Medium);
                                ImageTrancform.Retry(3, 5000);
                                ImageTrancform.WithCache(CacheType.All);
                                ImageTrancform.Into(UserProfileImage);
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
                        var (api_status, respond) = await Client.Group.Update_Group_Cover(Groups_Id, path);
                        if (api_status == 200)
                        {
                            if (respond is Update_Group_Data_Object result)
                            {
                                Toast.MakeText(this, result.Message, ToastLength.Short).Show();

                                //Set image 
                                var file = Uri.FromFile(new File(path));
                                var ImageTrancform = ImageService.Instance.LoadFile(file.Path);
                                ImageTrancform.LoadingPlaceholder("ImagePlacholder.jpg", ImageSource.CompiledResource);
                                ImageTrancform.ErrorPlaceholder("ImagePlacholder.jpg", ImageSource.CompiledResource);
                                ImageTrancform.DownSampleMode(InterpolationMode.Medium);
                                ImageTrancform.Retry(3, 5000);
                                ImageTrancform.WithCache(CacheType.All);
                                ImageTrancform.Into(CoverImage);
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
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimGroup,
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
                                var pathimg = "";
                                if (ImageType == "Cover")
                                {
                                    pathimg = resultUri.Path;
                                    Update_ImageGroup_Api(ImageType, pathimg);
                                }
                                else if (ImageType == "Avatar")
                                {
                                    pathimg = resultUri.Path;
                                    Update_ImageGroup_Api(ImageType, pathimg);
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

        private ImageViewAsync UserProfileImage;
        private ImageViewAsync CoverImage;
        private WebView HybirdView;
        private HybirdViewController hybridController;

        public LinearLayout News_Empty;
        public TextView News_Icon;
        public TextView Txt_News_Empty;
        public TextView Txt_News_start;
        private Button Btn_Reload;

        private NestedScrollView NestedScrollControll;

        private ImageView IconBack;

        private TextView Txt_GroupName;
        private TextView Txt_GroupUsername;

        private Button Btn_join;
        private ImageButton Btn_More;

        private TextView IconPrivacy;
        private TextView PrivacyText;

        private TextView CategoryText;
        private TextView IconCategory;

        private TextView IconEdit;
        private TextView AboutDesc;

        private FloatingActionButton FloatingActionButtonView;

        private LinearLayout Edit_AvatarImageGroup;
        private TextView Txt_EditGroupinfo;

        private string Groups_Type = "";
        private string Groups_Id = "";
        private string ImageType = "";
        private bool isOwner_Groups;
        private string Groups_Url, Groups_Name, Groups_Descriptions;

        private Get_User_Data_Object.Joined_Groups _JoinedGroups_Data;
        private Get_Community_Object.Group _MyGroups_Data;
        private Get_Search_Object.Group _SearchGroups_Data;

        private Get_General_Data_Object.Notification _NotifyPages_Data;
        private Get_Group_Data_Object.Group_Data _Group_Data;

        #endregion

        #region Event

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

        //Event Update Image avatar Group
        private void Edit_AvatarImageGroup_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (isOwner_Groups)
                {
                    ImageType = "Avatar";

                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                    {
                        //Open Image 
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimGroup,
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
                            var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimGroup,
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

        //Event Update Image Cover Group
        private void Edit_CoverImageGroup_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (isOwner_Groups)
                {
                    ImageType = "Cover";

                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                    {
                        //Open Image 
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimGroup,
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
                            var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimGroup,
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

        //Event Join_Group => Joined , Join Group 
        private async void BtnJoin_OnClick(object sender, EventArgs eventArgs)
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
                    var (api_status, respond) = await Client.Group.Join_Group(Groups_Id);
                    if (api_status == 200)
                    {
                        if (respond is Join_Group_Object result)
                        {
                            Toast.MakeText(this, result.Join_status, ToastLength.Short).Show();

                            // Add Group Or Remove in DB  
                            var dbDatabase = new SqLiteDatabase();
                            if (Groups_Type == "Joined_UserGroups")
                            {
                                var item = _JoinedGroups_Data;
                                var data = new DataTables.GroupsTB
                                {
                                    Id = item.id,
                                    UserId = item.user_id,
                                    GroupName = item.group_name,
                                    GroupTitle = item.group_title,
                                    Avatar = item.avatar,
                                    Cover = item.cover,
                                    About = item.about,
                                    Category = item.category,
                                    Privacy = item.privacy,
                                    JoinPrivacy = item.join_privacy,
                                    Active = item.active,
                                    Registered = item.registered,
                                    GroupId = item.group_id,
                                    Url = item.url,
                                    Name = item.name,
                                    CategoryId = item.category_id,
                                    Type = item.type,
                                    Username = item.username
                                };

                                dbDatabase.Insert_Or_Delete_OneGroupsTable(Groups_Id, data);
                            }
                            else if (Groups_Type == "Joined_MyGroups")
                            {
                                dbDatabase.Insert_Or_Delete_OneGroupsTable(Groups_Id, _MyGroups_Data);
                            }
                            else if (Groups_Type == "Search_Groups")
                            {
                                var item = _SearchGroups_Data;
                                var data = new DataTables.GroupsTB
                                {
                                    Id = item.Id,
                                    UserId = item.UserId,
                                    GroupName = item.GroupName,
                                    GroupTitle = item.GroupTitle,
                                    Avatar = item.Avatar,
                                    Cover = item.Cover,
                                    About = item.About,
                                    Category = item.Category,
                                    Privacy = item.Privacy,
                                    JoinPrivacy = item.JoinPrivacy,
                                    Active = item.Active,
                                    Registered = item.Registered,
                                    GroupId = item.GroupId,
                                    Url = item.Url,
                                    Name = item.Name,
                                    CategoryId = item.CategoryId,
                                    Type = item.Type,
                                    Username = item.Username
                                };
                                dbDatabase.Insert_Or_Delete_OneGroupsTable(Groups_Id, data);
                            }
                            else if (_Group_Data != null)
                            {
                                var item = _Group_Data;
                                var data = new DataTables.GroupsTB
                                {
                                    //Id = item.id,
                                    UserId = item.user_id,
                                    GroupName = item.group_name,
                                    GroupTitle = item.group_title,
                                    Avatar = item.avatar,
                                    Cover = item.cover,
                                    About = item.about,
                                    Category = item.category,
                                    Privacy = item.privacy,
                                    JoinPrivacy = item.join_privacy,
                                    Active = item.active,
                                    Registered = item.registered,
                                    GroupId = item.group_id,
                                    Url = item.url,
                                    Name = item.name,
                                    CategoryId = item.category_id,
                                    // Type = item.type,
                                    Username = item.username
                                };
                                dbDatabase.Insert_Or_Delete_OneGroupsTable(item.group_id, data);
                            }

                            dbDatabase.Dispose();
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
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Show More : Copy Link , Share , Edit (If user isOwner_Groups)
        private void BtnMore_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var ctw = new ContextThemeWrapper(this, Resource.Style.PopupMenuStyle);
                var popup = new PopupMenu(ctw, Btn_More);
                if (isOwner_Groups)
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
                                OnCopeLink_Button_Click();
                                break;

                            case Resource.Id.menu_Share:
                                OnShare_Button_Click();
                                break;

                            case Resource.Id.menu_Edit:
                                EditInfoGroup_OnClick();
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

        //Event Menu >> Cope Link
        private void OnCopeLink_Button_Click()
        {
            try
            {
                var clipboardManager = (ClipboardManager) GetSystemService(ClipboardService);

                var clipData = ClipData.NewPlainText("text", Groups_Url);
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
                    Title = Groups_Name,
                    Text = Groups_Descriptions,
                    Url = Groups_Url
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Menu >> Edit Info Group if user == is_owner 
        private void EditInfoGroup_OnClick()
        {
            try
            {
                if (isOwner_Groups)
                {
                    var Int = new Intent(this, typeof(EditInfoGroup_Activity));
                    if (Groups_Type == "Joined_UserGroups")
                    {
                        var item = _JoinedGroups_Data;
                        Int.PutExtra("UserGroups", JsonConvert.SerializeObject(item));
                        Int.PutExtra("GroupsType", "Joined_UserGroups");
                    }
                    else if (Groups_Type == "Joined_MyGroups")
                    {
                        var item = _MyGroups_Data;
                        Int.PutExtra("MyGroups", JsonConvert.SerializeObject(item));
                        Int.PutExtra("GroupsType", "Joined_MyGroups");
                    }
                    else if (Groups_Type == "Search_Groups")
                    {
                        var item = _SearchGroups_Data;
                        Int.PutExtra("SearchGroups", JsonConvert.SerializeObject(item));
                        Int.PutExtra("GroupsType", "Search_Groups");
                    }
                    else if (_Group_Data != null)
                    {
                        Int.PutExtra("GroupData", JsonConvert.SerializeObject(_Group_Data));
                        Int.PutExtra("GroupsType", "Group_Data");
                    }

                    Int.PutExtra("GroupsId", Groups_Id);
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
                Int.PutExtra("Type", "SocialGroup");
                Int.PutExtra("PostId", Groups_Id);
                Int.PutExtra("isOwner", isOwner_Groups.ToString());
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
                                Int.PutExtra("Type", "SocialGroup");
                                Int.PutExtra("PostId", Groups_Id);
                                Int.PutExtra("isOwner", isOwner_Groups.ToString());
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