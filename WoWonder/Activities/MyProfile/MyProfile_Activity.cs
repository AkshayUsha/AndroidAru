using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AT.Markushi.UI;
using Com.Theartofdev.Edmodo.Cropper;
using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Java.IO;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using SettingsConnecter;
using WoWonder.Activities.AddPost;
using WoWonder.Activities.Communities.Pages;
using WoWonder.Activities.MyContacts;
using WoWonder.Activities.MyProfile.Adapters;
using WoWonder.Activities.SettingsPreferences.General;
using WoWonder.Activities.SettingsPreferences.Privacy;
using WoWonder.Activities.UserProfile;
using WoWonder.Activities.UsersPages;
using WoWonder.Helpers;
using WoWonder.Helpers.HybirdView;
using WoWonder_API;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using Client = WoWonder_API.Requests.Client;
using Exception = System.Exception;
using IMethods = WoWonder.Helpers.IMethods;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.MyProfile
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class MyProfile_Activity : AppCompatActivity, MaterialDialog.IListCallback,
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
                    //Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
                    Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                }

                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);


                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.MyProfile_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.MyProfile_Layout);

                News_Empty = (LinearLayout) FindViewById(Resource.Id.News_LinerEmpty);
                News_Icon = (TextView) FindViewById(Resource.Id.News_icon);
                Txt_News_Empty = (TextView) FindViewById(Resource.Id.Txt_LabelEmpty);
                Txt_News_start = (TextView) FindViewById(Resource.Id.Txt_LabelStart);
                Btn_Reload = (Button) FindViewById(Resource.Id.reloadPage_Button);
                IMethods.Set_TextViewIcon("2", News_Icon, "\uf119");
                News_Empty.Visibility = ViewStates.Gone;


                //layout_Pages = (RelativeLayout)FindViewById(Resource.Id.layout_suggestion_Pages);
                layout_Friends = FindViewById<LinearLayout>(Resource.Id.layout_suggestion_Friends);
                //layout_Photo = (LinearLayout)FindViewById(Resource.Id.layout_suggestion_Photo);
                //layout_Groups = (LinearLayout)FindViewById(Resource.Id.layout_suggestion_Groups);

                Layout_CountFollowers = FindViewById<LinearLayout>(Resource.Id.CountFollowersLayout);
                Layout_CountFollowing = FindViewById<LinearLayout>(Resource.Id.CountFollowingLayout);
                Layout_CountLikes = FindViewById<LinearLayout>(Resource.Id.CountLikesLayout);

                Btn_EditDataUser = (CircleButton) FindViewById(Resource.Id.AddUserbutton);
                Btn_EditImage = (CircleButton) FindViewById(Resource.Id.message_button);
                Btn_More = (CircleButton) FindViewById(Resource.Id.morebutton);

                IconBack = (ImageView) FindViewById(Resource.Id.back);

                Txt_username = (TextView) FindViewById(Resource.Id.username_profile);
                FontController.SetFont(Txt_username, 2);

                Txt_CountFollowers = (TextView) FindViewById(Resource.Id.CountFollowers);
                Txt_CountFollowing = (TextView) FindViewById(Resource.Id.CountFollowing);
                Txt_CountLikes = (TextView) FindViewById(Resource.Id.CountLikes);

                Txt_friends_head = FindViewById<TextView>(Resource.Id.friends_head_txt);

                Txt_Followers = FindViewById<TextView>(Resource.Id.txtFollowers);
                Txt_Following = FindViewById<TextView>(Resource.Id.txtFollowing);
                Txt_Likes = FindViewById<TextView>(Resource.Id.txtLikes);

                Txt_About = (TextView) FindViewById(Resource.Id.tv_aboutdescUser);

                Txt_FriendsCounter = (TextView) FindViewById(Resource.Id.friends_counter);
                IconMoreFollowing = (TextView) FindViewById(Resource.Id.iv_more_following);
                FollowingRecylerView = (RecyclerView) FindViewById(Resource.Id.followingRecyler);

                //Txt_PhotosCounter = (TextView)FindViewById(Resource.Id.tv_photoscount);
                //IconMorePhoto = (TextView)FindViewById(Resource.Id.iv_more_photos);
                //ImageRecylerView = (RecyclerView)FindViewById(Resource.Id.photorecyler);

                //PageImage1 = (ImageViewAsync)FindViewById(Resource.Id.image_page_1);
                //PageImage2 = (ImageViewAsync)FindViewById(Resource.Id.image_page_2);
                //PageImage3 = (ImageViewAsync)FindViewById(Resource.Id.image_page_3);

                //Txt_GroupsCounter = (TextView)FindViewById(Resource.Id.tv_groupscount);
                //IconMoreGroup = (TextView)FindViewById(Resource.Id.iv_more_groups);
                //GroupsRecylerView = (RecyclerView)FindViewById(Resource.Id.groupsRecyler);

                UserProfileImage = (ImageViewAsync) FindViewById(Resource.Id.profileimage_head);
                CoverImage = (ImageViewAsync) FindViewById(Resource.Id.cover_image);

                HybirdView = (WebView) FindViewById(Resource.Id.hybirdview);

                IMethods.Set_TextViewIcon("1", IconMoreFollowing, IonIcons_Fonts.ChevronRight);
                //IMethods.Set_TextViewIcon("1", IconMorePhoto, IonIcons_Fonts.ChevronRight);
                //IMethods.Set_TextViewIcon("1", IconMoreGroup, IonIcons_Fonts.ChevronRight);

                if (Settings.ConnectivitySystem == "1") // Following
                    Txt_friends_head.Text = GetText(Resource.String.Lbl_Following);
                else // Friend
                    Txt_friends_head.Text = GetText(Resource.String.Lbl_Friends);

                //#####################################################################

                //Display User Photos limit by 9
                //UserPhotosLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                //ImageRecylerView.SetLayoutManager(UserPhotosLayoutManager);
                //MyPhotosAdapter = new MyPhotosAdapter(this);
                //ImageRecylerView.SetAdapter(MyPhotosAdapter);
                //GroupsRecylerView.NestedScrollingEnabled = false;

                //#####################################################################

                //Display Following limit by 12
                FollowingLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                FollowingRecylerView.SetLayoutManager(FollowingLayoutManager);
                MyFriendsAdapter = new MyFriendsAdapter(this);
                MyFriendsAdapter.mMyFriendsList = new ObservableCollection<Get_User_Data_Object.Following>();
                FollowingRecylerView.NestedScrollingEnabled = false;
                FollowingRecylerView.SetAdapter(MyFriendsAdapter);

                //#####################################################################

                //GroupsLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                //GroupsRecylerView.SetLayoutManager(GroupsLayoutManager);
                //MyGroupsAdapter = new MyGroupsAdapter(this);
                //MyGroupsAdapter.mMyGroupsList = new ObservableCollection<Get_User_Data_Object.Joined_Groups>();
                //GroupsRecylerView.NestedScrollingEnabled = false;
                //GroupsRecylerView.SetAdapter(MyGroupsAdapter);

                //#####################################################################

                MyPagesAdapter = new MyPagesAdapter(this);
                MyPagesAdapter.mAllMyPagesList = new ObservableCollection<Get_User_Data_Object.Liked_Pages>();

                //#####################################################################

                S_UserId = UserDetails.User_id;

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

                    hybridController = new HybirdViewController(this, HybirdView, null);

                    if (Settings.ClearCachSystem)
                        hybridController.HybirdView.ClearCache(true);

                    string Url = Current.URLS.UrlInstance.API_Get_News_Feed + S_UserId;
                    switch (Settings.Lang)
                    {
                        case "en":
                            hybridController.LoadUrl(Url + "&lang=english");
                            break;
                        case "ar":
                            hybridController.LoadUrl(Url + "&lang=arabic");
                            Settings.FlowDirection_RightToLeft = true;
                            break;
                        case "de":
                            hybridController.LoadUrl(Url + "&lang=german");
                            break;
                        case "el":
                            hybridController.LoadUrl(Url + "&lang=greek");
                            break;
                        case "es":
                            hybridController.LoadUrl(Url + "&lang=spanish");
                            break;
                        case "fr":
                            hybridController.LoadUrl(Url + "&lang=french");
                            break;
                        case "it":
                            hybridController.LoadUrl(Url + "&lang=italian");
                            break;
                        case "ja":
                            hybridController.LoadUrl(Url + "&lang=japanese");
                            break;
                        case "nl":
                            hybridController.LoadUrl(Url + "&lang=dutch");
                            break;
                        case "pt":
                            hybridController.LoadUrl(Url + "&lang=portuguese");
                            break;
                        case "ro":
                            hybridController.LoadUrl(Url + "&lang=romanian");
                            break;
                        case "ru":
                            hybridController.LoadUrl(Url + "&lang=russian");
                            break;
                        case "sq":
                            hybridController.LoadUrl(Url + "&lang=albanian");
                            break;
                        case "sr":
                            hybridController.LoadUrl(Url + "&lang=serbian");
                            break;
                        case "tr":
                            hybridController.LoadUrl(Url + "&lang=turkish");
                            break;
                        default:
                            hybridController.LoadUrl(Url);
                            break;
                    } 
                }

                //#####################################################################

                Get_MyProfileData_Local();
                 
                //Add Event
                Btn_Reload.Click += BtnReload_OnClick;
                //layout_Pages.Click += LayoutPages_OnClick;
                Btn_EditDataUser.Click += BtnEditDataUser_OnClick;
                Btn_EditImage.Click += BtnEditImage_OnClick;
                Btn_More.Click += BtnMoreOnClick;
                IconBack.Click += IconBackOnClick;
                IconMoreFollowing.Click += IconMoreFollowing_OnClick;
                Layout_CountFollowers.Click += LayoutCountFollowersOnClick;
                Layout_CountFollowing.Click += LayoutCountFollowingOnClick;
                Layout_CountLikes.Click += LayoutCountLikesOnClick;
                //IconMorePhoto.Click += IconMorePhoto_OnClick;
                //IconMoreGroup.Click += IconMoreGroup_OnClick;
                //MyPhotosAdapter.ItemClick += MyPhotosAdapter_OnItemClick;
                MyFriendsAdapter.ItemClick += MyFriendsAdapterOnItemClick;
                //MyGroupsAdapter.ItemClick += MyGroupsAdapter_OnItemClick;
                hybridController.JavascriptInterface.OnJavascriptInjectionRequest += OnJavascriptInjectionRequest;
                hybridController.DefaultClient.OnPageEventFinished += WoDefaultClient_OnPageEventFinished;
                if (Settings.Show_Error_HybirdView)
                    hybridController.DefaultClient.OnPageEventReceivedError += DefaultClientOnOnPageEventReceivedError;

                //Show Ads
                AdsGoogle.Ad_Interstitial(this);
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
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimImage,
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
                                    Update_Image_Api(ImageType, pathimg);
                                }
                                else if (ImageType == "Avatar")
                                {
                                    pathimg = resultUri.Path;
                                    Update_Image_Api(ImageType, pathimg);
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
                Toast.MakeText(this, GetText(Resource.String.Lbl_Error_path_image), ToastLength.Short).Show();
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

        public LinearLayout News_Empty;
        public TextView News_Icon;
        public TextView Txt_News_Empty;
        public TextView Txt_News_start;
        private Button Btn_Reload;

        //public static MyPhotosAdapter  MyPhotosAdapter;
        public MyFriendsAdapter MyFriendsAdapter;

        public MyFollowers_Adapter MyFollowersAdapter;

        //public static MyGroupsAdapter  MyGroupsAdapter;
        public MyPagesAdapter MyPagesAdapter;

        //private RecyclerView ImageRecylerView;
        private RecyclerView FollowingRecylerView;
        //private RecyclerView GroupsRecylerView;

        //private RecyclerView.LayoutManager UserPhotosLayoutManager;
        private RecyclerView.LayoutManager FollowingLayoutManager;
        //private RecyclerView.LayoutManager GroupsLayoutManager;

        //private RelativeLayout layout_Pages;
        private LinearLayout layout_Friends;
        //private LinearLayout layout_Photo;
        //private LinearLayout layout_Groups;

        private LinearLayout Layout_CountFollowers;
        private LinearLayout Layout_CountFollowing;
        private LinearLayout Layout_CountLikes;

        private CircleButton Btn_EditDataUser;
        private CircleButton Btn_EditImage;
        private CircleButton Btn_More;

        private ImageView IconBack;
        private TextView Txt_username;

        private TextView Txt_CountFollowers;
        private TextView Txt_CountFollowing;
        private TextView Txt_CountLikes;

        private TextView Txt_friends_head;

        private TextView Txt_Followers;
        private TextView Txt_Following;
        private TextView Txt_Likes;

        private TextView Txt_About;

        private TextView Txt_FriendsCounter;
        private TextView IconMoreFollowing;

        //private TextView Txt_PhotosCounter;
        //private TextView IconMorePhoto;

        //public ImageViewAsync PageImage1;
        //public ImageViewAsync PageImage2;
        //public ImageViewAsync PageImage3;

        //private TextView Txt_GroupsCounter;
        //private TextView IconMoreGroup;

        private WebView HybirdView;
        private HybirdViewController hybridController;
        private string S_UserId = "";
        private string S_Url_User = "";

        private string S_Image_Avatar = "";
        private string S_Image_Cover = "";

        private string S_PrivacyBirth = "";
        private string S_PrivacyFollow = "";
        private string S_PrivacyFriend = "";
        private string S_PrivacyMessage = "";

        private int S_Can_follow = 0;
        private string ImageType = "";

        #endregion

        #region Get Data User

        //Get Data User From Database 
        public void Get_MyProfileData_Local()
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();
                var datauser = dbDatabase.Get_MyProfile_CredentialList();
                if (datauser != null)
                {
                    if (Classes.MyProfileList.Count > 0)
                    {
                        var local = Classes.MyProfileList.FirstOrDefault(a => a.user_id == S_UserId);
                        if (local != null) LoadDataUser(local);
                    }
                    else
                    {
                        Classes.MyProfileList = datauser;
                    }
                }

                //var groups = dbDatabase.GetAll_ManageGroups();
                //if (groups?.Count > 0)
                //{
                //    MyGroupsAdapter.mAllMyGroupsList = new ObservableCollection<Get_User_Data_Object.Joined_Groups>(groups);
                //    MyGroupsAdapter.mMyGroupsList = new ObservableCollection<Get_User_Data_Object.Joined_Groups>(groups.Take(12));
                //    MyGroupsAdapter.BindEnd();
                //}

                var pages = dbDatabase.GetAll_ManagePages();
                if (pages?.Count > 0)
                    MyPagesAdapter.mAllMyPagesList = new ObservableCollection<Get_User_Data_Object.Liked_Pages>(pages);

                dbDatabase.Dispose();

                Get_MyProfileData_Api();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Get Data My Profile API
        public async void Get_MyProfileData_Api()
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
                    var (Api_status, Respond) = await Client.Global.Get_User_Data(new Settings(), S_UserId);
                    if (Api_status == 200)
                    {
                        if (Respond is Get_User_Data_Object result)
                        {
                            var dbDatabase = new SqLiteDatabase();

                            //Add Data User
                            //=======================================
                            // user_data
                            if (result.user_data != null)
                            {
                                LoadDataUser(result.user_data);

                                //Insert Or Update All data User From Database 
                                dbDatabase.Insert_Or_Update_To_MyProfileTable(result.user_data);
                            }

                            // following
                            if (result.following.Length > 0)
                            {
                                MyFriendsAdapter.mAllMyFriendsList =
                                    new ObservableCollection<Get_User_Data_Object.Following>(result.following);
                                MyFriendsAdapter.mMyFriendsList =
                                    new ObservableCollection<Get_User_Data_Object.Following>(result.following.Take(12));
                                MyFriendsAdapter.BindEnd();

                                layout_Friends.Visibility = ViewStates.Visible;

                                var list = MyFriendsAdapter.mAllMyFriendsList.Select(user =>
                                    new Classes.UserContacts.User
                                    {
                                        user_id = user.user_id,
                                        username = user.username,
                                        email = user.email,
                                        first_name = user.first_name,
                                        last_name = user.last_name,
                                        avatar = user.avatar,
                                        cover = user.cover,
                                        relationship_id = user.relationship_id,
                                        //lastseen_time_text = user.lastseen_time_text,
                                        address = user.address,
                                        working = user.working,
                                        working_link = user.working_link,
                                        about = user.about,
                                        school = user.school,
                                        gender = user.gender,
                                        birthday = user.birthday,
                                        website = user.website,
                                        facebook = user.facebook,
                                        google = user.google,
                                        twitter = user.twitter,
                                        linkedin = user.linkedin,
                                        youtube = user.youtube,
                                        vk = user.vk,
                                        instagram = user.instagram,
                                        language = user.language,
                                        ip_address = user.ip_address,
                                        follow_privacy = user.follow_privacy,
                                        friend_privacy = user.friend_privacy,
                                        post_privacy = user.post_privacy,
                                        message_privacy = user.message_privacy,
                                        confirm_followers = user.confirm_followers,
                                        show_activities_privacy = user.show_activities_privacy,
                                        birth_privacy = user.birth_privacy,
                                        visit_privacy = user.visit_privacy,
                                        lastseen = user.lastseen,
                                        showlastseen = user.showlastseen,
                                        e_sentme_msg = user.e_sentme_msg,
                                        e_last_notif = user.e_last_notif,
                                        status = user.status,
                                        active = user.active,
                                        admin = user.admin,
                                        registered = user.registered,
                                        phone_number = user.phone_number,
                                        is_pro = user.is_pro,
                                        pro_type = user.pro_type,
                                        joined = user.joined,
                                        timezone = user.timezone,
                                        referrer = user.referrer,
                                        balance = user.balance,
                                        paypal_email = user.paypal_email,
                                        notifications_sound = user.notifications_sound,
                                        order_posts_by = user.order_posts_by,
                                        social_login = user.social_login,
                                        device_id = user.device_id,
                                        web_device_id = user.web_device_id,
                                        wallet = user.wallet,
                                        lat = user.lat,
                                        lng = user.lng,
                                        last_location_update = user.last_location_update,
                                        share_my_location = user.share_my_location,
                                        url = user.url,
                                        name = user.name,
                                        lastseen_unix_time = user.lastseen_unix_time,
                                        //user_platform = user.user_platform,
                                        details = new Details
                                        {
                                            post_count = user.details.post_count,
                                            album_count = user.details.album_count,
                                            following_count = user.details.following_count,
                                            followers_count = user.details.followers_count,
                                            groups_count = user.details.groups_count,
                                            likes_count = user.details.likes_count
                                        }
                                    }).ToList();

                                //Insert Or Update All User
                                dbDatabase.Insert_Or_Replace_MyContactTable(
                                    new ObservableCollection<Classes.UserContacts.User>(list));
                            }
                            else
                            {
                                layout_Friends.Visibility = ViewStates.Gone;
                            }

                            // Followers
                            if (result.followers.Length > 0)
                            {
                                var list = new JavaList<Get_User_Data_Object.Followers>(result.followers);
                                MyFollowersAdapter = new MyFollowers_Adapter(this, list, null);
                                MyFollowersAdapter.mMyFollowersList = list;

                                //Insert Or Update All User
                                dbDatabase.Insert_Or_Replace_MyFollowersTable(
                                    new ObservableCollection<Get_User_Data_Object.Followers>(list));
                            }

                            //// joined_groups
                            //if (result.joined_groups.Length > 0)
                            //{
                            //    //Select all groups just it's me owner
                            //    var chkList = result.joined_groups.Where(a => a.user_id == UserDetails.User_id).ToList();

                            //    MyGroupsAdapter.mAllMyGroupsList = new ObservableCollection<Get_User_Data_Object.Joined_Groups>(chkList);
                            //    MyGroupsAdapter.mMyGroupsList = new ObservableCollection<Get_User_Data_Object.Joined_Groups>(chkList.Take(12));
                            //    MyGroupsAdapter.BindEnd();

                            //    //Insert Or Update All data Database 
                            //    dbDatabase.InsertOrReplace_ManageGroupsTable(MyGroupsAdapter.mAllMyGroupsList);

                            //    layout_Groups.Visibility = ViewStates.Visible;
                            //}
                            //else
                            //{
                            //    layout_Groups.Visibility = ViewStates.Gone;
                            //}

                            //liked_pages
                            if (result.liked_pages.Length > 0)
                            {
                                //Select all pages just it's me owner ,and Insert Or Update All data Database 
                                var chkList = result.liked_pages.Where(a => a.user_id == UserDetails.User_id).ToList();
                                if (chkList.Count > 0)
                                    dbDatabase.InsertOrReplace_ManagePagesTable(
                                        new ObservableCollection<Get_User_Data_Object.Liked_Pages>(chkList));

                                MyPagesAdapter.mAllMyPagesList =
                                    new ObservableCollection<Get_User_Data_Object.Liked_Pages>(chkList);

                                //layout_Pages.Visibility = ViewStates.Visible;

                                //try
                                //{
                                //    for (var i = 0; i < 4; i++)
                                //    {
                                //        if (i == 0)
                                //        {
                                //            ImageServiceLoader.Load_Image(PageImage1, "no_profile_image.png", result.liked_pages[i].avatar, 1, true, 20);
                                //        }
                                //        else if (i == 1)
                                //        {
                                //            ImageServiceLoader.Load_Image(PageImage2, "no_profile_image.png", result.liked_pages[i].avatar, 1, true, 20);
                                //        }
                                //        else if (i == 2)
                                //        {
                                //            ImageServiceLoader.Load_Image(PageImage3, "no_profile_image.png", result.liked_pages[i].avatar, 1, true, 20);
                                //        }
                                //    }
                                //}
                                //catch (Exception e)
                                //{
                                //    Crashes.TrackError(e);
                                //}
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

                    //Albums User
                    //=======================================
                    // Get_AlbumUser_Api();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_MyProfileData_Api();
            }
        }


        private void LoadDataUser(Get_User_Data_Object.User_Data data)
        {
            try
            {
                Txt_username.Text = data.name;

                var dataabout = IMethods.Fun_String.StringNullRemover(data.about);
                if (dataabout != "Empty")
                    Txt_About.Text =
                        IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                else
                    Txt_About.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " + Settings.Application_Name;

                var AvatarSplit = data.avatar.Split('/').Last();
                if (S_Image_Avatar != AvatarSplit)
                {
                    var getImage_Avatar =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar, 1);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, data.avatar);
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", data.avatar, 1);
                    }
                }

                var CoverSplit = data.cover.Split('/').Last();
                if (S_Image_Cover != CoverSplit)
                {
                    var getImage_Cover =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, CoverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, data.cover);
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", data.cover);
                    }
                }

                S_Image_Avatar = data.avatar.Split('/').Last();
                S_Image_Cover = data.cover.Split('/').Last();
                S_Url_User = data.url;

                //Set Privacy User
                //==================================
                S_PrivacyBirth = data.birth_privacy;
                S_PrivacyFollow = data.follow_privacy;
                S_PrivacyFriend = data.friend_privacy;
                S_PrivacyMessage = data.message_privacy;

                // details
                if (data.details != null)
                {
                    if (Settings.ConnectivitySystem == "1") // Following
                    {
                        Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                        Txt_Following.Text = GetText(Resource.String.Lbl_Following);

                        Txt_CountFollowers.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(data.details.followers_count));
                        Txt_CountFollowing.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(data.details.following_count));

                        Layout_CountFollowing.Tag = "Following";
                    }
                    else // Friend
                    {
                        Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                        Txt_Following.Text = GetText(Resource.String.Lbl_Post);

                        Txt_CountFollowers.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(data.details.followers_count));
                        Txt_CountFollowing.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(data.details.post_count));

                        Layout_CountFollowing.Tag = "Post";
                    }

                    Txt_CountFollowers.Text =
                        IMethods.Fun_String.FormatPriceValue(int.Parse(data.details.followers_count));
                    Txt_CountFollowing.Text =
                        IMethods.Fun_String.FormatPriceValue(int.Parse(data.details.following_count));
                    Txt_CountLikes.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(data.details.likes_count));
                    Txt_FriendsCounter.Text =
                        IMethods.Fun_String.FormatPriceValue(int.Parse(data.details.following_count));
                    //Txt_PhotosCounter.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(data.details.album_count));
                    //Txt_GroupsCounter.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(data.details.groups_count));
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion

        #region Event

        //Event Show More : share , Copy Link To Profile , View Privacy , Settings Account
        private void BtnMoreOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var ctw = new ContextThemeWrapper(this, Resource.Style.PopupMenuStyle);
                var popup = new PopupMenu(ctw, Btn_More);
                popup.MenuInflater.Inflate(Resource.Menu.MoroMyProfile_Menu, popup.Menu);
                popup.Show();
                popup.MenuItemClick += (o, e) =>
                {
                    try
                    {
                        var Id = e.Item.ItemId;
                        switch (Id)
                        {
                            case Resource.Id.menu_CopeLink:
                                OnCopeLinkToProfile_Button_Click();
                                break;

                            case Resource.Id.menu_Share:
                                OnShare_Button_Click();
                                break;

                            case Resource.Id.menu_ViewPrivacy:
                                OnViewPrivacy_Button_Click();
                                break;

                            case Resource.Id.menu_SettingsAccount:
                                OnSettingsAccount_Button_Click();
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

        //Event Menu >> Share
        private async void OnShare_Button_Click()
        {
            try
            {
                //Share Plugin same as video
                if (!CrossShare.IsSupported) return;

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = UserDetails.Username,
                    Text = "",
                    Url = S_Url_User
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Menu >> View Privacy Shortcuts
        private void OnViewPrivacy_Button_Click()
        {
            try
            {
                var Intent = new Intent(this, typeof(Privacy_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Menu >> General Account 
        private void OnSettingsAccount_Button_Click()
        {
            try
            {
                var Intent = new Intent(this, typeof(GeneralAccount_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Menu >> Cope Link To Profile
        private void OnCopeLinkToProfile_Button_Click()
        {
            try
            {
                var clipboardManager = (ClipboardManager) GetSystemService(ClipboardService);

                var clipData = ClipData.NewPlainText("text", S_Url_User);
                clipboardManager.PrimaryClip = clipData;


                Toast.MakeText(this, GetText(Resource.String.Lbl_Copied), ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Edit Image Avatar and cover 
        private void BtnEditImage_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var ctw = new ContextThemeWrapper(this, Resource.Style.PopupMenuStyle);
                var popup = new PopupMenu(ctw, Btn_More);
                popup.MenuInflater.Inflate(Resource.Menu.ImageMyProfile_Menu, popup.Menu);
                popup.Show();
                popup.MenuItemClick += (o, e) =>
                {
                    try
                    {
                        var Id = e.Item.ItemId;
                        switch (Id)
                        {
                            case Resource.Id.menu_ImageAvatar:
                                Edit_AvatarImage_OnClick();
                                break;

                            case Resource.Id.menu_ImageCover:
                                Edit_CoverImage_OnClick();
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

        #region Update Image Avatar && Cover

        //Event Update Image avatar
        public void Edit_AvatarImage_OnClick()
        {
            try
            {
                ImageType = "Avatar";

                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                    //Open Image 
                    var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimImage,
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
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimImage,
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

        //Event Update Image Cover
        public void Edit_CoverImage_OnClick()
        {
            try
            {
                ImageType = "Cover";

                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                    //Open Image 
                    var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimImage,
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
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimImage,
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

        public async void Update_Image_Api(string type, string path)
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
                        var (api_status, respond) = await Client.Global.Update_User_Avatar(path);
                        if (api_status == 200)
                        {
                            if (respond is Update_User_Data_Object result)
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
                        var (api_status, respond) = await Client.Global.Update_User_Cover(path);
                        if (api_status == 200)
                        {
                            if (respond is Update_User_Data_Object result)
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

        //Event edit data User
        private void BtnEditDataUser_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var Int = new Intent(this, typeof(EditMyProfile_Activity));
                StartActivity(Int);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Opne profile group Using Group_ProfileActivity
        private void MyGroupsAdapter_OnItemClick(object sender, MyGroupsAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    //var item = MyGroupsAdapter.GetItem(position);
                    //if (item != null)
                    //{
                    //    Intent Int = new Intent(this, typeof(Group_Profile_Activity));
                    //    if (item.user_id != UserDetails.User_id)
                    //    {
                    //        Int.PutExtra("UserGroups", JsonConvert.SerializeObject(item));
                    //        Int.PutExtra("GroupsType", "Joined_UserGroups");
                    //    }
                    //    else
                    //    {
                    //        Int.PutExtra("MyGroups", JsonConvert.SerializeObject(item));
                    //        Int.PutExtra("GroupsType", "Joined_MyGroups");
                    //    }
                    //    StartActivity(Int);
                    //}
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //Event Open Profile User Using User_Profile_Activity
        private void MyFriendsAdapterOnItemClick(object sender, MyFriendsAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = MyFriendsAdapter.GetItem(position);
                    if (item != null)
                    {
                        Intent Int;
                        if (item.user_id != UserDetails.User_id)
                        {
                            Int = new Intent(this, typeof(User_Profile_Activity));
                            Int.PutExtra("UserId", item.user_id);
                            Int.PutExtra("UserType", "MyFollowers");
                            Int.PutExtra("UserItem", JsonConvert.SerializeObject(item));
                        }
                        else
                        {
                            Int = new Intent(this, typeof(MyProfile_Activity));
                            Int.PutExtra("UserId", item.user_id);
                        }
                       
                        StartActivity(Int);
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Show All Page Likes
        private void LayoutCountLikesOnClick(object sender, EventArgs e)
        {
            try
            {
                if (MyPagesAdapter.mAllMyPagesList.Count > 0)
                {
                    var intent = new Intent(this, typeof(Pages_Activity));
                    if (S_UserId != UserDetails.User_id)
                        intent.PutExtra("PagesType", "Manage_UserPages");
                    else
                        intent.PutExtra("PagesType", "Manage_MyPages");
                    intent.PutExtra("UserID", S_UserId);
                    StartActivity(intent);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Show All Users Following  
        private void LayoutCountFollowingOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Layout_CountFollowing.Tag.ToString() == "Following")
                {
                    if (MyFriendsAdapter.mMyFriendsList.Count > 0)
                    {
                        var intent = new Intent(this, typeof(MyContacts_Activity));
                        intent.PutExtra("ContactsType", "Following");
                        StartActivity(intent);
                    }
                }
                else
                {
                    var intent = new Intent(this, typeof(HyberdPostViewer_Activity));
                    intent.PutExtra("Type", "MyProfile");
                    intent.PutExtra("Id", S_UserId);
                    intent.PutExtra("Title", UserDetails.Full_name);
                    StartActivity(intent);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Show All Users Followers 
        private void LayoutCountFollowersOnClick(object sender, EventArgs e)
        {
            try
            {
                if (MyFollowersAdapter.mMyFollowersList.Count > 0)
                {
                    var intent = new Intent(this, typeof(MyContacts_Activity));
                    intent.PutExtra("ContactsType", "Followers");
                    StartActivity(intent);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Open IMG Using ImagePostViewer_Activity
        private void MyPhotosAdapter_OnItemClick(object sender, MyPhotosAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                //var position = adapterClickEvents.Position;
                //if (position >= 0)
                //{
                //    var item = MyPhotosAdapter.GetItem(position);
                //    if (item != null)
                //    {
                //        Intent Int = new Intent(this, typeof(ImagePostViewer_Activity));
                //        Int.PutExtra("ImageUrl", item.postFile_full);
                //        Int.PutExtra("Description", item.postText);
                //        Int.PutExtra("TotatlLikes", item.post_likes);
                //        Int.PutExtra("TotatlWowonder", item.post_wonders);
                //        Int.PutExtra("TotalComments", item.post_comments);
                //        StartActivity(Int);
                //    }
                //}
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Show All Group
        private void IconMoreGroup_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                //if (MyGroupsAdapter.mAllMyGroupsList.Count > 0)
                //{
                //    Intent Int = new Intent(this, typeof(Groups_Activity));
                //    if (S_UserId != UserDetails.User_id)
                //    {
                //        Int.PutExtra("GroupsType", "Manage_UserGroups");
                //    }
                //    else
                //    {
                //        Int.PutExtra("GroupsType", "Manage_MyGroups");
                //    }
                //    Int.PutExtra("UserID", S_UserId);
                //    StartActivity(Int);
                //}
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Show All Pages
        private void LayoutPages_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                //if (MyPagesAdapter.mAllMyPagesList.Count > 0)
                //{
                //    var intent = new Intent(this, typeof(Pages_Activity));
                //    if (S_UserId != UserDetails.User_id)
                //    {
                //        intent.PutExtra("PagesType", "Manage_UserPages");
                //    }
                //    else
                //    {
                //        intent.PutExtra("PagesType", "Manage_MyPages");
                //    }
                //    intent.PutExtra("UserID", S_UserId);
                //    StartActivity(intent);
                //}
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Show All Photo
        private void IconMorePhoto_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                //if (MyPhotosAdapter.mAllMyAlbumsList.Count > 0)
                //{
                //    Intent Int = new Intent(this, typeof(MyPhotosActivity));
                //    Int.PutExtra("UserId", S_UserId);
                //    StartActivity(Int);
                //}
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Show All Following 
        private void IconMoreFollowing_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (MyFriendsAdapter.mMyFriendsList.Count > 0)
                {
                    var intent = new Intent(this, typeof(MyContacts_Activity));
                    intent.PutExtra("ContactsType", "Following");
                    StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Back
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

        #endregion

        #region HybridController

        private Task<string> OnJavascriptInjectionRequest(string eventobj)
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
                            Int.PutExtra("Type", "Normal");
                            Int.PutExtra("PostId", UserDetails.User_id);
                            Int.PutExtra("isOwner", "Normal");
                            StartActivityForResult(Int, 2500);
                        });
                    else
                        return null;
                }

            return null;
        }

        private void WoDefaultClient_OnPageEventFinished(WebView view, string url)
        {
            try
            {
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