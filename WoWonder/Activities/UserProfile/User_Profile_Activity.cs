using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AT.Markushi.UI;
using FFImageLoading;
using FFImageLoading.Views;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.AddPost;
using WoWonder.Activities.Album;
using WoWonder.Activities.Communities.Groups;
using WoWonder.Activities.Communities.Pages;
using WoWonder.Activities.MyContacts;
using WoWonder.Activities.MyProfile;
using WoWonder.Activities.userProfile.Adapters;
using WoWonder.Activities.UserContacts;
using WoWonder.Activities.UserProfile.Adapters;
using WoWonder.Activities.UsersPages;
using WoWonder.Helpers;
using WoWonder.Helpers.HybirdView;
using WoWonder_API;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Product;
using WoWonder_API.Classes.User;
using Client = WoWonder_API.Requests.Client;
using Exception = System.Exception;
using IMethods = WoWonder.Helpers.IMethods;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;

namespace WoWonder.Activities.UserProfile
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class User_Profile_Activity : AppCompatActivity, MaterialDialog.IListCallback,
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


                var view = MyContextWrapper.GetContentView(this, Settings.Lang,
                    Resource.Layout.User_Profile_Main_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.User_Profile_Main_Layout);

                var data = Intent.GetStringExtra("UserId") ?? "Data not available";
                if (data != "Data not available" && !string.IsNullOrEmpty(data)) S_UserId = data;

                var UserType = Intent.GetStringExtra("UserType") ?? "Data not available";
                if (UserType != "Data not available" && !string.IsNullOrEmpty(data)) S_UserType = UserType;

                News_Empty = (LinearLayout) FindViewById(Resource.Id.News_LinerEmpty);
                News_Icon = (TextView) FindViewById(Resource.Id.News_icon);
                Txt_News_Empty = (TextView) FindViewById(Resource.Id.Txt_LabelEmpty);
                Txt_News_start = (TextView) FindViewById(Resource.Id.Txt_LabelStart);
                Btn_Reload = (Button) FindViewById(Resource.Id.reloadPage_Button);
                IMethods.Set_TextViewIcon("2", News_Icon, "\uf119");
                News_Empty.Visibility = ViewStates.Gone;

                layout_Pages = (RelativeLayout) FindViewById(Resource.Id.layout_suggestion_Pages);
                layout_Friends = FindViewById<LinearLayout>(Resource.Id.layout_suggestion_Friends);
                layout_Photo = (LinearLayout) FindViewById(Resource.Id.layout_suggestion_Photo);
                layout_Groups = (LinearLayout) FindViewById(Resource.Id.layout_suggestion_Groups);

                Btn_AddUser = (CircleButton) FindViewById(Resource.Id.AddUserbutton);
                Btn_Message = (CircleButton) FindViewById(Resource.Id.message_button);
                Btn_More = (CircleButton) FindViewById(Resource.Id.morebutton);

                Btn_AddUser.Tag = "Add";

                IconBack = (ImageView) FindViewById(Resource.Id.back);

                Txt_username = (TextView) FindViewById(Resource.Id.username_profile);
                FontController.SetFont(Txt_username, 2);

                Txt_friends_head = FindViewById<TextView>(Resource.Id.friends_head_txt);

                Txt_CountFollowers = (TextView) FindViewById(Resource.Id.CountFollowers);
                Txt_CountFollowing = (TextView) FindViewById(Resource.Id.CountFollowing);
                Txt_CountLikes = (TextView) FindViewById(Resource.Id.CountLikes);

                Txt_Followers = FindViewById<TextView>(Resource.Id.txtFollowers);
                Txt_Following = FindViewById<TextView>(Resource.Id.txtFollowing);
                Txt_Likes = FindViewById<TextView>(Resource.Id.txtLikes);

                Txt_About = (TextView) FindViewById(Resource.Id.tv_aboutdescUser);

                Txt_FriendsCounter = (TextView) FindViewById(Resource.Id.friends_counter);
                IconMoreFolowers = (TextView) FindViewById(Resource.Id.iv_more_folowers);
                FollowersRecylerView = (RecyclerView) FindViewById(Resource.Id.followersRecyler);

                Txt_PhotosCounter = (TextView) FindViewById(Resource.Id.tv_photoscount);
                IconMorePhoto = (TextView) FindViewById(Resource.Id.iv_more_photos);
                ImageRecylerView = (RecyclerView) FindViewById(Resource.Id.photorecyler);

                PageImage1 = (ImageViewAsync) FindViewById(Resource.Id.image_page_1);
                PageImage2 = (ImageViewAsync) FindViewById(Resource.Id.image_page_2);
                PageImage3 = (ImageViewAsync) FindViewById(Resource.Id.image_page_3);

                Txt_GroupsCounter = (TextView) FindViewById(Resource.Id.tv_groupscount);
                IconMoreGroup = (TextView) FindViewById(Resource.Id.iv_more_groups);
                GroupsRecylerView = (RecyclerView) FindViewById(Resource.Id.groupsRecyler);

                UserProfileImage = (ImageViewAsync) FindViewById(Resource.Id.profileimage_head);
                CoverImage = (ImageViewAsync) FindViewById(Resource.Id.cover_image);

                HybirdView = (WebView) FindViewById(Resource.Id.hybirdview);

                IMethods.Set_TextViewIcon("1", IconMoreFolowers, IonIcons_Fonts.ChevronRight);
                IMethods.Set_TextViewIcon("1", IconMorePhoto, IonIcons_Fonts.ChevronRight);
                IMethods.Set_TextViewIcon("1", IconMoreGroup, IonIcons_Fonts.ChevronRight);

                if (Settings.ConnectivitySystem == "1") // Following
                {
                    Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                    Txt_Following.Text = GetText(Resource.String.Lbl_Following);
                    Txt_friends_head.Text = GetText(Resource.String.Lbl_Following);
                }
                else // Friend
                {
                    Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                    Txt_Following.Text = GetText(Resource.String.Lbl_Post);
                    Txt_friends_head.Text = GetText(Resource.String.Lbl_Friends);
                }
                 
                //#####################################################################

                //Display User Photos limit by 9
                UserPhotosLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                ImageRecylerView.SetLayoutManager(UserPhotosLayoutManager);
                UserPhotosAdapter = new UserPhotosAdapter(this);
                ImageRecylerView.SetAdapter(UserPhotosAdapter);
                GroupsRecylerView.NestedScrollingEnabled = false;

                //#####################################################################

                //Display Followers limit by 12
                FollowersLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                FollowersRecylerView.SetLayoutManager(FollowersLayoutManager);
                UserFriendsAdapter = new UserFriendsAdapter(this);
                UserFriendsAdapter.ItemClick += UserFriendsAdapter_ItemClick;
                UserFriendsAdapter.mUserFriendsList = new ObservableCollection<Get_User_Data_Object.Followers>();
                FollowersRecylerView.NestedScrollingEnabled = false;
                FollowersRecylerView.SetAdapter(UserFriendsAdapter);

                //#####################################################################

                GroupsLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                GroupsRecylerView.SetLayoutManager(GroupsLayoutManager);
                UserGroupsAdapter = new UserGroupsAdapter(this);
                UserGroupsAdapter.mUserGroupsList = new ObservableCollection<Get_User_Data_Object.Joined_Groups>();
                GroupsRecylerView.NestedScrollingEnabled = false;
                GroupsRecylerView.SetAdapter(UserGroupsAdapter);

                //#####################################################################

                UserPagesAdapter = new UserPagesAdapter(this);
                UserPagesAdapter.mAllUserPagesList = new ObservableCollection<Get_User_Data_Object.Liked_Pages>();

                //#####################################################################

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

                if (!Settings.Messenger_Integration)
                    Btn_Message.Visibility = ViewStates.Gone;

                Get_Data_local();
                  
                //Show Ads
                AdsGoogle.Ad_Interstitial(this);
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
                layout_Pages.Click += LayoutPages_OnClick;
                Btn_Reload.Click += BtnReload_OnClick;
                Btn_AddUser.Click += BtnAddUserOnClick;
                Btn_Message.Click += BtnMessageOnClick;
                Btn_More.Click += BtnMoreOnClick;
                IconBack.Click += IconBackOnClick;
                layout_Friends.Click += IconMoreFollowers_OnClick;
                layout_Photo.Click += IconMorePhoto_OnClick;
                layout_Groups.Click += IconMoreGroup_OnClick;
                UserPhotosAdapter.ItemClick += UserPhotosAdapter_ItemClick;
                //UserFriendsAdapter.ItemClick += UserFriendsAdapter_ItemClick;
                UserGroupsAdapter.ItemClick += UserGroupsAdapter_OnItemClick;
                hybridController.JavascriptInterface.OnJavascriptInjectionRequest += OnJavascriptInjectionRequest;
                hybridController.DefaultClient.OnPageEventFinished += WoDefaultClient_OnPageEventFinished;
                if (Settings.Show_Error_HybirdView)
                    hybridController.DefaultClient.OnPageEventReceivedError += DefaultClientOnOnPageEventReceivedError;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        protected override void OnPause()
        {
            try
            {
                base.OnPause();

                //Add Event
                layout_Pages.Click -= LayoutPages_OnClick;
                Btn_Reload.Click -= BtnReload_OnClick;
                Btn_AddUser.Click -= BtnAddUserOnClick;
                Btn_Message.Click -= BtnMessageOnClick;
                Btn_More.Click -= BtnMoreOnClick;
                IconBack.Click -= IconBackOnClick;
                layout_Friends.Click -= IconMoreFollowers_OnClick;
                layout_Photo.Click -= IconMorePhoto_OnClick;
                layout_Groups.Click -= IconMoreGroup_OnClick;
                UserPhotosAdapter.ItemClick -= UserPhotosAdapter_ItemClick;
                //UserFriendsAdapter.ItemClick -= UserFriendsAdapter_ItemClick;
                UserGroupsAdapter.ItemClick -= UserGroupsAdapter_OnItemClick;
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

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == 2500)
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

        public static UserPhotosAdapter UserPhotosAdapter;
         
        public UserFriendsAdapter UserFriendsAdapter;
        public UserGroupsAdapter UserGroupsAdapter;
        public UserPagesAdapter UserPagesAdapter;

        private RecyclerView ImageRecylerView;
        private RecyclerView FollowersRecylerView;
        private RecyclerView GroupsRecylerView;

        private RecyclerView.LayoutManager UserPhotosLayoutManager;
        private RecyclerView.LayoutManager FollowersLayoutManager;
        private RecyclerView.LayoutManager GroupsLayoutManager;

        private RelativeLayout layout_Pages;
        private LinearLayout layout_Friends;
        private LinearLayout layout_Photo;
        private LinearLayout layout_Groups;

        private CircleButton Btn_AddUser;
        private CircleButton Btn_Message;
        private CircleButton Btn_More;

        private ImageView IconBack;
        private TextView Txt_username;

        private TextView Txt_friends_head;
        
        private TextView Txt_CountFollowers;
        private TextView Txt_CountFollowing;
        private TextView Txt_CountLikes;


        private TextView Txt_Followers;
        private TextView Txt_Following;
        private TextView Txt_Likes;

        private TextView Txt_About;

        private TextView Txt_FriendsCounter;
        private TextView IconMoreFolowers;

        private TextView Txt_PhotosCounter;
        private TextView IconMorePhoto;

        public ImageViewAsync PageImage1;
        public ImageViewAsync PageImage2;
        public ImageViewAsync PageImage3;

        private TextView Txt_GroupsCounter;
        private TextView IconMoreGroup;

        private WebView HybirdView;
        private HybirdViewController hybridController;

        private string S_UserId = "";
        private string S_Url_User = "";
        private string S_UserType = "";

        private string S_PrivacyBirth = "";
        private string S_PrivacyFollow = "";
        private string S_PrivacyFriend = "";
        private string S_PrivacyMessage = "";

        private int S_Can_follow;

        public ObservableCollection<Classes.UserChat> ListDataUserChat = new ObservableCollection<Classes.UserChat>();

        #endregion

        #region Get Data User

        #region Load Data

        public void Get_Data_local()
        {
            try
            {
                switch (S_UserType)
                {
                    case "Notify":
                        loadData_Item_Notify();
                        break;
                    case "ProductView":
                        loadData_Item_ProductView();
                        break;
                    case "MyContacts":
                        loadData_Item_MyContacts();
                        break;
                    case "MyFollowers":
                        loadData_Item_MyFollowers();
                        break;
                    case "MyFriends":
                        loadData_Item_MyFriends();
                        break;
                    case "NearBy":
                        loadData_Item_NearBy();
                        break;
                    case "Search":
                        loadData_Item_Search();
                        break;
                    case "ProUsers":
                        loadData_Item_ProUsers();
                        break;
                    case "Articles":
                        loadData_Item_Articles();
                        break;
                    case "LikedUsers":
                        loadData_Item_PostLikedUsers();
                        break;
                    case "WonderedUsers":
                        loadData_Item_PostWonderedUsers();
                        break;
                }

                Get_UserProfileData_Api();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_UserProfileData_Api();
            }
        }

        private void loadData_Item_Notify()
        {
            try
            {
                var Item =JsonConvert.DeserializeObject<Get_General_Data_Object.Notification>(Intent.GetStringExtra("UserItem"));
                if (Item != null)
                {
                    ListDataUserChat.Clear();
                    ListDataUserChat.Add(new Classes.UserChat
                    {
                        user_id = Item.notifier.user_id,
                        username = Item.notifier.username,
                        email = Item.notifier.email,
                        first_name = Item.notifier.first_name,
                        last_name = Item.notifier.last_name,
                        avatar = Item.notifier.avatar,
                        cover = Item.notifier.cover,
                        relationship_id = Item.notifier.relationship_id,
                        address = Item.notifier.address,
                        working = Item.notifier.working,
                        working_link = Item.notifier.working_link,
                        about = Item.notifier.about,
                        school = Item.notifier.school,
                        gender = Item.notifier.gender,
                        birthday = Item.notifier.birthday,
                        website = Item.notifier.website,
                        facebook = Item.notifier.facebook,
                        google = Item.notifier.google,
                        twitter = Item.notifier.twitter,
                        linkedin = Item.notifier.linkedin,
                        youtube = Item.notifier.youtube,
                        vk = Item.notifier.vk,
                        instagram = Item.notifier.instagram,
                        language = Item.notifier.language,
                        ip_address = Item.notifier.ip_address,
                        follow_privacy = Item.notifier.follow_privacy,
                        friend_privacy = Item.notifier.friend_privacy,
                        post_privacy = Item.notifier.post_privacy,
                        message_privacy = Item.notifier.message_privacy,
                        confirm_followers = Item.notifier.confirm_followers,
                        show_activities_privacy = Item.notifier.show_activities_privacy,
                        birth_privacy = Item.notifier.birth_privacy,
                        visit_privacy = Item.notifier.visit_privacy,
                        lastseen = Item.notifier.lastseen,
                        e_sentme_msg = Item.notifier.e_sentme_msg,
                        e_last_notif = Item.notifier.e_last_notif,
                        status = Item.notifier.status,
                        active = Item.notifier.active,
                        admin = Item.notifier.admin,
                        registered = Item.notifier.registered,
                        phone_number = Item.notifier.phone_number,
                        is_pro = Item.notifier.is_pro,
                        pro_type = Item.notifier.pro_type,
                        timezone = Item.notifier.timezone,
                        referrer = Item.notifier.referrer,
                        balance = Item.notifier.balance,
                        paypal_email = Item.notifier.paypal_email,
                        notifications_sound = Item.notifier.notifications_sound,
                        order_posts_by = Item.notifier.order_posts_by,
                        device_id = Item.notifier.device_id,
                        web_device_id = Item.notifier.web_device_id,
                        wallet = Item.notifier.wallet,
                        lat = Item.notifier.lat,
                        lng = Item.notifier.lng,
                        last_location_update = Item.notifier.last_location_update,
                        share_my_location = Item.notifier.share_my_location,
                        url = Item.notifier.url,
                        name = Item.notifier.name,
                        lastseen_unix_time = Item.notifier.lastseen_unix_time
                    });

                    S_UserId = Item.notifier.user_id;
                    Txt_username.Text = Item.notifier.name;

                    var dataabout = IMethods.Fun_String.StringNullRemover(Item.notifier.about);
                    if (dataabout != "Empty")
                        Txt_About.Text =
                            IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                    else
                        Txt_About.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " + Settings.Application_Name;

                    var AvatarSplit = Item.notifier.avatar.Split('/').Last();
                    var getImage_Avatar =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar, 1);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage,
                            Item.notifier.avatar);
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", Item.notifier.avatar,
                            1);
                    }

                    var coverSplit = Item.notifier.cover.Split('/').Last();
                    var getImage_Cover =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, coverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage,
                            Item.notifier.cover);
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", Item.notifier.cover);
                    }

                    
                    S_Url_User = Item.notifier.url;

                    //Set Privacy User
                    //==================================

                    S_PrivacyBirth = Item.notifier.birth_privacy;
                    S_PrivacyFollow = Item.notifier.follow_privacy;
                    S_PrivacyFriend = Item.notifier.friend_privacy;
                    S_PrivacyMessage = Item.notifier.message_privacy;


                    if (S_PrivacyFollow == "0") //Lbl_Everyone
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else if (S_PrivacyFollow == "1") //Lbl_People_i_Follow
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else //Lbl_No_body
                        Btn_AddUser.Visibility = ViewStates.Gone;

                    // details
                    if (Item.notifier.details != null)
                    {
                        if (Item.notifier.user_id != UserDetails.User_id)
                        {
                            if (Settings.ConnectivitySystem == "1") // Following
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Following);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(
                                        int.Parse(Item.notifier.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(
                                        int.Parse(Item.notifier.details.following_count));
                            }
                            else // Friend
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Post);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(
                                        int.Parse(Item.notifier.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.notifier.details.post_count));
                            }
                        }
                        else
                        {
                            Btn_AddUser.Visibility = ViewStates.Gone;
                        }

                        Txt_CountLikes.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.notifier.details.likes_count));
                        Txt_FriendsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.notifier.details.following_count));
                        Txt_PhotosCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.notifier.details.album_count));
                        Txt_GroupsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.notifier.details.groups_count));
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void loadData_Item_ProductView()
        {
            try
            {
                var Item =
                    JsonConvert.DeserializeObject<Get_Products_Object.Product>(Intent.GetStringExtra("UserItem"));
                if (Item != null)
                {
                    ListDataUserChat.Clear();
                    ListDataUserChat.Add(new Classes.UserChat
                    {
                        user_id = Item.seller.user_id,
                        username = Item.seller.username,
                        email = Item.seller.email,
                        first_name = Item.seller.first_name,
                        last_name = Item.seller.last_name,
                        avatar = Item.seller.avatar,
                        cover = Item.seller.cover,
                        relationship_id = Item.seller.relationship_id,
                        address = Item.seller.address,
                        working = Item.seller.working,
                        working_link = Item.seller.working_link,
                        about = Item.seller.about,
                        school = Item.seller.school,
                        gender = Item.seller.gender,
                        birthday = Item.seller.birthday,
                        website = Item.seller.website,
                        facebook = Item.seller.facebook,
                        google = Item.seller.google,
                        twitter = Item.seller.twitter,
                        linkedin = Item.seller.linkedin,
                        youtube = Item.seller.youtube,
                        vk = Item.seller.vk,
                        instagram = Item.seller.instagram,
                        language = Item.seller.language,
                        ip_address = Item.seller.ip_address,
                        follow_privacy = Item.seller.follow_privacy,
                        friend_privacy = Item.seller.friend_privacy,
                        post_privacy = Item.seller.post_privacy,
                        message_privacy = Item.seller.message_privacy,
                        confirm_followers = Item.seller.confirm_followers,
                        show_activities_privacy = Item.seller.show_activities_privacy,
                        birth_privacy = Item.seller.birth_privacy,
                        visit_privacy = Item.seller.visit_privacy,
                        lastseen = Item.seller.lastseen,
                        e_sentme_msg = Item.seller.e_sentme_msg,
                        e_last_notif = Item.seller.e_last_notif,
                        status = Item.seller.status,
                        active = Item.seller.active,
                        admin = Item.seller.admin,
                        registered = Item.seller.registered,
                        phone_number = Item.seller.phone_number,
                        is_pro = Item.seller.is_pro,
                        pro_type = Item.seller.pro_type,
                        timezone = Item.seller.timezone,
                        referrer = Item.seller.referrer,
                        balance = Item.seller.balance,
                        paypal_email = Item.seller.paypal_email,
                        notifications_sound = Item.seller.notifications_sound,
                        order_posts_by = Item.seller.order_posts_by,
                        device_id = Item.seller.device_id,
                        web_device_id = Item.seller.web_device_id,
                        wallet = Item.seller.wallet,
                        lat = Item.seller.lat,
                        lng = Item.seller.lng,
                        last_location_update = Item.seller.last_location_update,
                        share_my_location = Item.seller.share_my_location,
                        url = Item.seller.url,
                        name = Item.seller.name,
                        lastseen_unix_time = Item.seller.lastseen_unix_time
                    });
                    S_UserId = Item.seller.user_id;
                    Txt_username.Text = Item.seller.name;

                    var dataabout = IMethods.Fun_String.StringNullRemover(Item.seller.about);
                    if (dataabout != "Empty")
                        Txt_About.Text =
                            IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                    else
                        Txt_About.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " + Settings.Application_Name;

                    var AvatarSplit = Item.seller.avatar.Split('/').Last();
                    var getImage_Avatar =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar, 1);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage,
                            Item.seller.avatar);
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", Item.seller.avatar, 1);
                    }

                    var coverSplit = Item.seller.cover.Split('/').Last();
                    var getImage_Cover =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, coverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage,
                            Item.seller.cover);
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", Item.seller.cover);
                    }
                     
                    S_Url_User = Item.seller.url;

                    //Set Privacy User
                    //==================================

                    S_PrivacyBirth = Item.seller.birth_privacy;
                    S_PrivacyFollow = Item.seller.follow_privacy;
                    S_PrivacyFriend = Item.seller.friend_privacy;
                    S_PrivacyMessage = Item.seller.message_privacy;


                    if (S_PrivacyFollow == "0") //Lbl_Everyone
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else if (S_PrivacyFollow == "1") //Lbl_People_i_Follow
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else //Lbl_No_body
                        Btn_AddUser.Visibility = ViewStates.Gone;

                    // details
                    if (Item.seller.details != null)
                    {
                        if (Item.seller.user_id != UserDetails.User_id)
                        {
                            if (Settings.ConnectivitySystem == "1") // Following
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Following);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(
                                        int.Parse(Item.seller.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(
                                        int.Parse(Item.seller.details.following_count));
                            }
                            else // Friend
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Post);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(
                                        int.Parse(Item.seller.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.seller.details.post_count));
                            }
                        }
                        else
                        {
                            Btn_AddUser.Visibility = ViewStates.Gone;
                        }

                        Txt_CountLikes.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.seller.details.likes_count));
                        Txt_FriendsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.seller.details.following_count));
                        Txt_PhotosCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.seller.details.album_count));
                        Txt_GroupsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.seller.details.groups_count));
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void loadData_Item_MyContacts()
        {
            try
            {
                var Item = JsonConvert.DeserializeObject<Classes.UserContacts.User>(Intent.GetStringExtra("UserItem"));
                if (Item != null)
                {
                    ListDataUserChat.Clear();
                    ListDataUserChat.Add(new Classes.UserChat
                    {
                        user_id = Item.user_id,
                        username = Item.username,
                        email = Item.email,
                        first_name = Item.first_name,
                        last_name = Item.last_name,
                        avatar = Item.avatar,
                        cover = Item.cover,
                        relationship_id = Item.relationship_id,
                        address = Item.address,
                        working = Item.working,
                        working_link = Item.working_link,
                        about = Item.about,
                        school = Item.school,
                        gender = Item.gender,
                        birthday = Item.birthday,
                        website = Item.website,
                        facebook = Item.facebook,
                        google = Item.google,
                        twitter = Item.twitter,
                        linkedin = Item.linkedin,
                        youtube = Item.youtube,
                        vk = Item.vk,
                        instagram = Item.instagram,
                        language = Item.language,
                        ip_address = Item.ip_address,
                        follow_privacy = Item.follow_privacy,
                        friend_privacy = Item.friend_privacy,
                        post_privacy = Item.post_privacy,
                        message_privacy = Item.message_privacy,
                        confirm_followers = Item.confirm_followers,
                        show_activities_privacy = Item.show_activities_privacy,
                        birth_privacy = Item.birth_privacy,
                        visit_privacy = Item.visit_privacy,
                        lastseen = Item.lastseen,
                        e_sentme_msg = Item.e_sentme_msg,
                        e_last_notif = Item.e_last_notif,
                        status = Item.status,
                        active = Item.active,
                        admin = Item.admin,
                        registered = Item.registered,
                        phone_number = Item.phone_number,
                        is_pro = Item.is_pro,
                        pro_type = Item.pro_type,
                        timezone = Item.timezone,
                        referrer = Item.referrer,
                        balance = Item.balance,
                        paypal_email = Item.paypal_email,
                        notifications_sound = Item.notifications_sound,
                        order_posts_by = Item.order_posts_by,
                        device_id = Item.device_id,
                        web_device_id = Item.web_device_id,
                        wallet = Item.wallet,
                        lat = Item.lat,
                        lng = Item.lng,
                        last_location_update = Item.last_location_update,
                        share_my_location = Item.share_my_location,
                        url = Item.url,
                        name = Item.name,
                        lastseen_unix_time = Item.lastseen_unix_time
                    });

                    S_UserId = Item.user_id;

                    Txt_username.Text = Item.name;

                    var dataAbout = IMethods.Fun_String.StringNullRemover(Item.about);
                    if (dataAbout != "Empty")
                        Txt_About.Text =
                            IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataAbout));
                    else
                        Txt_About.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " + Settings.Application_Name;

                    var AvatarSplit = Item.avatar.Split('/').Last();
                    var getImage_Avatar =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar, 1);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.avatar);
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", Item.avatar, 1);
                    }

                    var coverSplit = Item.cover.Split('/').Last();
                    var getImage_Cover =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, coverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.cover);
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", Item.cover);
                    }

                    if (Item.user_id != UserDetails.User_id)
                    {
                        if (S_Can_follow == 0 && Item.is_following == "0")
                            Btn_AddUser.Visibility = ViewStates.Gone;

                        if (Item.is_following == "1") // My Friend
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;
                            Btn_AddUser.SetColor(Color.ParseColor("#efefef"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_tick);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#444444"));
                            Btn_AddUser.Tag = "friends";
                        }
                        else if (Item.is_following == "2") // Request
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;
                            Btn_AddUser.SetColor(Color.ParseColor("#efefef"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_requestAdd);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#444444"));
                            Btn_AddUser.Tag = "request";
                        }
                        else if (Item.is_following == "0") //Not Friend
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;

                            Btn_AddUser.SetColor(Color.ParseColor("#6666ff"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_add);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#ffffff"));
                            Btn_AddUser.Tag = "Add";
                        }
                    }
                    else
                    {
                        Btn_AddUser.Visibility = ViewStates.Gone;
                    }

                    S_Url_User = Item.url;


                    //Set Privacy User
                    //==================================

                    S_PrivacyBirth = Item.birth_privacy;
                    S_PrivacyFollow = Item.follow_privacy;
                    S_PrivacyFriend = Item.friend_privacy;
                    S_PrivacyMessage = Item.message_privacy;


                    if (S_PrivacyFollow == "0") //Lbl_Everyone
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else if (S_PrivacyFollow == "1") //Lbl_People_i_Follow
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else //Lbl_No_body
                        Btn_AddUser.Visibility = ViewStates.Gone;

                    // details
                    if (Item.details != null)
                    {
                        if (Item.user_id != UserDetails.User_id)
                        {
                            if (Settings.ConnectivitySystem == "1") // Following
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Following);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.following_count));
                            }
                            else // Friend
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Post);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.post_count));
                            }
                        }
                        else
                        {
                            Btn_AddUser.Visibility = ViewStates.Gone;
                        }

                        Txt_CountLikes.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.likes_count));
                        Txt_FriendsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.following_count));
                        Txt_PhotosCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.album_count));
                        Txt_GroupsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.groups_count));
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void loadData_Item_MyFollowers()
        {
            try
            {
                var Item =JsonConvert.DeserializeObject<Get_User_Data_Object.Followers>(Intent.GetStringExtra("UserItem"));
                if (Item != null)
                {
                    ListDataUserChat.Clear();
                    ListDataUserChat.Add(new Classes.UserChat
                    {
                        user_id = Item.user_id,
                        username = Item.username,
                        email = Item.email,
                        first_name = Item.first_name,
                        last_name = Item.last_name,
                        avatar = Item.avatar,
                        cover = Item.cover,
                        relationship_id = Item.relationship_id,
                        address = Item.address,
                        working = Item.working,
                        working_link = Item.working_link,
                        about = Item.about,
                        school = Item.school,
                        gender = Item.gender,
                        birthday = Item.birthday,
                        website = Item.website,
                        facebook = Item.facebook,
                        google = Item.google,
                        twitter = Item.twitter,
                        linkedin = Item.linkedin,
                        youtube = Item.youtube,
                        vk = Item.vk,
                        instagram = Item.instagram,
                        language = Item.language,
                        ip_address = Item.ip_address,
                        follow_privacy = Item.follow_privacy,
                        friend_privacy = Item.friend_privacy,
                        post_privacy = Item.post_privacy,
                        message_privacy = Item.message_privacy,
                        confirm_followers = Item.confirm_followers,
                        show_activities_privacy = Item.show_activities_privacy,
                        birth_privacy = Item.birth_privacy,
                        visit_privacy = Item.visit_privacy,
                        lastseen = Item.lastseen,
                        e_sentme_msg = Item.e_sentme_msg,
                        e_last_notif = Item.e_last_notif,
                        status = Item.status,
                        active = Item.active,
                        admin = Item.admin,
                        registered = Item.registered,
                        phone_number = Item.phone_number,
                        is_pro = Item.is_pro,
                        pro_type = Item.pro_type,
                        timezone = Item.timezone,
                        referrer = Item.referrer,
                        balance = Item.balance,
                        paypal_email = Item.paypal_email,
                        notifications_sound = Item.notifications_sound,
                        order_posts_by = Item.order_posts_by,
                        device_id = Item.device_id,
                        web_device_id = Item.web_device_id,
                        wallet = Item.wallet,
                        lat = Item.lat,
                        lng = Item.lng,
                        last_location_update = Item.last_location_update,
                        share_my_location = Item.share_my_location,
                        url = Item.url,
                        name = Item.name,
                        lastseen_unix_time = Item.lastseen_unix_time
                    });

                    S_UserId = Item.user_id;
                    Txt_username.Text = Item.name;

                    var dataabout = IMethods.Fun_String.StringNullRemover(Item.about);
                    if (dataabout != "Empty")
                        Txt_About.Text =
                            IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                    else
                        Txt_About.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " + Settings.Application_Name;

                    var AvatarSplit = Item.avatar.Split('/').Last();
                    var getImage_Avatar =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar, 1);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.avatar);
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", Item.avatar, 1);
                    }

                    var coverSplit = Item.cover.Split('/').Last();
                    var getImage_Cover =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, coverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.cover);
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", Item.cover);
                    }

                    if (Item.user_id != UserDetails.User_id)
                    {
                        if (S_Can_follow == 0 && Item.is_following == 0)
                            Btn_AddUser.Visibility = ViewStates.Gone;

                        if (Item.is_following == 1) // My Friend
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;
                            Btn_AddUser.SetColor(Color.ParseColor("#efefef"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_tick);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#444444"));
                            Btn_AddUser.Tag = "friends";
                        }
                        else if (Item.is_following == 2) // Request
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;
                            Btn_AddUser.SetColor(Color.ParseColor("#efefef"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_requestAdd);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#444444"));
                            Btn_AddUser.Tag = "request";
                        }
                        else if (Item.is_following == 0) //Not Friend
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;

                            Btn_AddUser.SetColor(Color.ParseColor("#6666ff"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_add);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#ffffff"));
                            Btn_AddUser.Tag = "Add";
                        }
                    }
                    else
                    {
                        Btn_AddUser.Visibility = ViewStates.Gone;
                    }

                    S_Url_User = Item.url;

                    //Set Privacy User
                    //==================================

                    S_PrivacyBirth = Item.birth_privacy;
                    S_PrivacyFollow = Item.follow_privacy;
                    S_PrivacyFriend = Item.friend_privacy;
                    S_PrivacyMessage = Item.message_privacy;


                    if (S_PrivacyFollow == "0") //Lbl_Everyone
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else if (S_PrivacyFollow == "1") //Lbl_People_i_Follow
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else //Lbl_No_body
                        Btn_AddUser.Visibility = ViewStates.Gone;

                    // details
                    if (Item.details != null)
                    {
                        if (Item.user_id != UserDetails.User_id)
                        {
                            if (Settings.ConnectivitySystem == "1") // Following
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Following);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.following_count));
                            }
                            else // Friend
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Post);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.post_count));
                            }
                        }
                        else
                        {
                            Btn_AddUser.Visibility = ViewStates.Gone;
                        }

                        Txt_CountLikes.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.likes_count));
                        Txt_FriendsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.following_count));
                        Txt_PhotosCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.album_count));
                        Txt_GroupsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.groups_count));
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void loadData_Item_MyFriends()
        {
            try
            {
                var Item =JsonConvert.DeserializeObject<Get_User_Data_Object.Followers>(Intent.GetStringExtra("UserItem"));
                if (Item != null)
                {
                    ListDataUserChat.Clear();
                    ListDataUserChat.Add(new Classes.UserChat
                    {
                        user_id = Item.user_id,
                        username = Item.username,
                        email = Item.email,
                        first_name = Item.first_name,
                        last_name = Item.last_name,
                        avatar = Item.avatar,
                        cover = Item.cover,
                        relationship_id = Item.relationship_id,
                        address = Item.address,
                        working = Item.working,
                        working_link = Item.working_link,
                        about = Item.about,
                        school = Item.school,
                        gender = Item.gender,
                        birthday = Item.birthday,
                        website = Item.website,
                        facebook = Item.facebook,
                        google = Item.google,
                        twitter = Item.twitter,
                        linkedin = Item.linkedin,
                        youtube = Item.youtube,
                        vk = Item.vk,
                        instagram = Item.instagram,
                        language = Item.language,
                        ip_address = Item.ip_address,
                        follow_privacy = Item.follow_privacy,
                        friend_privacy = Item.friend_privacy,
                        post_privacy = Item.post_privacy,
                        message_privacy = Item.message_privacy,
                        confirm_followers = Item.confirm_followers,
                        show_activities_privacy = Item.show_activities_privacy,
                        birth_privacy = Item.birth_privacy,
                        visit_privacy = Item.visit_privacy,
                        lastseen = Item.lastseen,
                        e_sentme_msg = Item.e_sentme_msg,
                        e_last_notif = Item.e_last_notif,
                        status = Item.status,
                        active = Item.active,
                        admin = Item.admin,
                        registered = Item.registered,
                        phone_number = Item.phone_number,
                        is_pro = Item.is_pro,
                        pro_type = Item.pro_type,
                        timezone = Item.timezone,
                        referrer = Item.referrer,
                        balance = Item.balance,
                        paypal_email = Item.paypal_email,
                        notifications_sound = Item.notifications_sound,
                        order_posts_by = Item.order_posts_by,
                        device_id = Item.device_id,
                        web_device_id = Item.web_device_id,
                        wallet = Item.wallet,
                        lat = Item.lat,
                        lng = Item.lng,
                        last_location_update = Item.last_location_update,
                        share_my_location = Item.share_my_location,
                        url = Item.url,
                        name = Item.name,
                        lastseen_unix_time = Item.lastseen_unix_time
                    });

                    S_UserId = Item.user_id;
                    Txt_username.Text = Item.name;

                    var dataabout = IMethods.Fun_String.StringNullRemover(Item.about);
                    if (dataabout != "Empty")
                        Txt_About.Text =
                            IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                    else
                        Txt_About.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " + Settings.Application_Name;

                    var AvatarSplit = Item.avatar.Split('/').Last();
                    var getImage_Avatar =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar, 1);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.avatar);
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", Item.avatar, 1);
                    }

                    var coverSplit = Item.cover.Split('/').Last();
                    var getImage_Cover =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, coverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.cover);
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", Item.cover);
                    }

                    if (Item.user_id != UserDetails.User_id)
                    {
                        if (S_Can_follow == 0 && Item.is_following == 0)
                            Btn_AddUser.Visibility = ViewStates.Gone;

                        if (Item.is_following == 1) // My Friend
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;
                            Btn_AddUser.SetColor(Color.ParseColor("#efefef"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_tick);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#444444"));
                            Btn_AddUser.Tag = "friends";
                        }
                        else if (Item.is_following == 2) // Request
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;
                            Btn_AddUser.SetColor(Color.ParseColor("#efefef"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_requestAdd);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#444444"));
                            Btn_AddUser.Tag = "request";
                        }
                        else if (Item.is_following == 0) //Not Friend
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;

                            Btn_AddUser.SetColor(Color.ParseColor("#6666ff"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_add);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#ffffff"));
                            Btn_AddUser.Tag = "Add";
                        }
                    }
                    else
                    {
                        Btn_AddUser.Visibility = ViewStates.Gone;
                    }

                    S_Url_User = Item.url;

                    //Set Privacy User
                    //==================================

                    S_PrivacyBirth = Item.birth_privacy;
                    S_PrivacyFollow = Item.follow_privacy;
                    S_PrivacyFriend = Item.friend_privacy;
                    S_PrivacyMessage = Item.message_privacy;


                    if (S_PrivacyFollow == "0") //Lbl_Everyone
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else if (S_PrivacyFollow == "1") //Lbl_People_i_Follow
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else //Lbl_No_body
                        Btn_AddUser.Visibility = ViewStates.Gone;

                    // details
                    if (Item.details != null)
                    {
                        if (Item.user_id != UserDetails.User_id)
                        {
                            if (Settings.ConnectivitySystem == "1") // Following
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Following);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.following_count));
                            }
                            else // Friend
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Post);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.post_count));
                            }
                        }
                        else
                        {
                            Btn_AddUser.Visibility = ViewStates.Gone;
                        }

                        Txt_CountLikes.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.likes_count));
                        Txt_FriendsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.following_count));
                        Txt_PhotosCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.album_count));
                        Txt_GroupsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.groups_count));
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void loadData_Item_NearBy()
        {
            try
            {
                var Item =JsonConvert.DeserializeObject<Get_Nearby_Users_Object.Nearby_Users>(Intent.GetStringExtra("UserItem"));
                if (Item != null)
                {
                    ListDataUserChat.Clear();
                    ListDataUserChat.Add(new Classes.UserChat
                    {
                        user_id = Item.user_id,
                        username = Item.username,
                        email = Item.email,
                        first_name = Item.first_name,
                        last_name = Item.last_name,
                        avatar = Item.avatar,
                        cover = Item.cover,
                        relationship_id = Item.relationship_id,
                        address = Item.address,
                        working = Item.working,
                        working_link = Item.working_link,
                        about = Item.about,
                        school = Item.school,
                        gender = Item.gender,
                        birthday = Item.birthday,
                        website = Item.website,
                        facebook = Item.facebook,
                        google = Item.google,
                        twitter = Item.twitter,
                        linkedin = Item.linkedin,
                        youtube = Item.youtube,
                        vk = Item.vk,
                        instagram = Item.instagram,
                        language = Item.language,
                        ip_address = Item.ip_address,
                        follow_privacy = Item.follow_privacy,
                        friend_privacy = Item.friend_privacy,
                        post_privacy = Item.post_privacy,
                        message_privacy = Item.message_privacy,
                        confirm_followers = Item.confirm_followers,
                        show_activities_privacy = Item.show_activities_privacy,
                        birth_privacy = Item.birth_privacy,
                        visit_privacy = Item.visit_privacy,
                        lastseen = Item.lastseen,
                        e_sentme_msg = Item.e_sentme_msg,
                        e_last_notif = Item.e_last_notif,
                        status = Item.status,
                        active = Item.active,
                        admin = Item.admin,
                        registered = Item.registered,
                        phone_number = Item.phone_number,
                        is_pro = Item.is_pro,
                        pro_type = Item.pro_type,
                        timezone = Item.timezone,
                        referrer = Item.referrer,
                        balance = Item.balance,
                        paypal_email = Item.paypal_email,
                        notifications_sound = Item.notifications_sound,
                        order_posts_by = Item.order_posts_by,
                        device_id = Item.device_id,
                        web_device_id = Item.web_device_id,
                        wallet = Item.wallet,
                        lat = Item.lat,
                        lng = Item.lng,
                        last_location_update = Item.last_location_update,
                        share_my_location = Item.share_my_location,
                        url = Item.url,
                        name = Item.name,
                        lastseen_unix_time = Item.lastseen_unix_time
                    });
                    Txt_username.Text = Item.name;

                    var dataabout = IMethods.Fun_String.StringNullRemover(Item.about);
                    if (dataabout != "Empty")
                        Txt_About.Text =
                            IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                    else
                        Txt_About.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " + Settings.Application_Name;

                    var AvatarSplit = Item.avatar.Split('/').Last();
                    var getImage_Avatar =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar, 1);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.avatar);
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", Item.avatar, 1);
                    }

                    var coverSplit = Item.cover.Split('/').Last();
                    var getImage_Cover =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, coverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.cover);
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", Item.cover);
                    }

                    if (Item.user_id != UserDetails.User_id)
                    {
                        if (S_Can_follow == 0 && (Item.is_following != "yes" || Item.is_following != "Yes"))
                            Btn_AddUser.Visibility = ViewStates.Gone;

                        if (Item.is_following == "yes" || Item.is_following == "Yes") // My Friend
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;
                            Btn_AddUser.SetColor(Color.ParseColor("#efefef"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_tick);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#444444"));
                            Btn_AddUser.Tag = "friends";
                        } 
                        else  //Not Friend
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;

                            Btn_AddUser.SetColor(Color.ParseColor("#6666ff"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_add);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#ffffff"));
                            Btn_AddUser.Tag = "Add";
                        }
                    }
                    else
                    {
                        Btn_AddUser.Visibility = ViewStates.Gone;
                    }

                    S_Url_User = Item.url;

                    //Set Privacy User
                    //==================================

                    S_PrivacyBirth = Item.birth_privacy;
                    S_PrivacyFollow = Item.follow_privacy;
                    S_PrivacyFriend = Item.friend_privacy;
                    S_PrivacyMessage = Item.message_privacy;


                    if (S_PrivacyFollow == "0") //Lbl_Everyone
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else if (S_PrivacyFollow == "1") //Lbl_People_i_Follow
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else //Lbl_No_body
                        Btn_AddUser.Visibility = ViewStates.Gone;

                    // details
                    if (Item.details != null)
                    {
                        if (Item.user_id != UserDetails.User_id)
                        {
                            if (Settings.ConnectivitySystem == "1") // Following
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Following);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.following_count));
                            }
                            else // Friend
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Post);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.post_count));
                            }
                        }
                        else
                        {
                            Btn_AddUser.Visibility = ViewStates.Gone;
                        }

                        Txt_CountLikes.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.likes_count));
                        Txt_FriendsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.following_count));
                        Txt_PhotosCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.album_count));
                        Txt_GroupsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.groups_count));
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void loadData_Item_Search()
        {
            try
            {
                var Item = JsonConvert.DeserializeObject<Get_Search_Object.User>(Intent.GetStringExtra("UserItem"));
                if (Item != null)
                {
                    ListDataUserChat.Clear();
                    ListDataUserChat.Add(new Classes.UserChat
                    {
                        user_id = Item.UserId,
                        username = Item.Username,
                        email = Item.Email,
                        first_name = Item.FirstName,
                        last_name = Item.LastName,
                        avatar = Item.Avatar,
                        cover = Item.Cover,
                        relationship_id = Item.RelationshipId,
                        address = Item.Address,
                        working = Item.Working,
                        working_link = Item.WorkingLink,
                        about = Item.About,
                        school = Item.School,
                        gender = Item.Gender,
                        birthday = Item.Birthday,
                        website = Item.Website,
                        facebook = Item.Facebook,
                        google = Item.Google,
                        twitter = Item.Twitter,
                        linkedin = Item.Linkedin,
                        youtube = Item.Youtube,
                        vk = Item.Vk,
                        instagram = Item.Instagram,
                        language = Item.Language,
                        ip_address = Item.IpAddress,
                        follow_privacy = Item.FollowPrivacy,
                        friend_privacy = Item.FriendPrivacy,
                        post_privacy = Item.PostPrivacy,
                        message_privacy = Item.MessagePrivacy,
                        confirm_followers = Item.ConfirmFollowers,
                        show_activities_privacy = Item.ShowActivitiesPrivacy,
                        birth_privacy = Item.BirthPrivacy,
                        visit_privacy = Item.VisitPrivacy,
                        lastseen = Item.Lastseen,
                        e_sentme_msg = Item.SentmeMsg,
                        e_last_notif = Item.LastNotif,
                        status = Item.Status,
                        active = Item.Active,
                        admin = Item.Admin,
                        registered = Item.Registered,
                        phone_number = Item.PhoneNumber,
                        is_pro = Item.IsPro,
                        pro_type = Item.ProType,
                        timezone = Item.Timezone,
                        referrer = Item.Referrer,
                        balance = Item.Balance,
                        paypal_email = Item.PaypalEmail,
                        notifications_sound = Item.NotificationsSound,
                        order_posts_by = Item.OrderPostsBy,
                        device_id = Item.DeviceId,
                        web_device_id = Item.WebDeviceId,
                        wallet = Item.Wallet,
                        lat = Item.Lat,
                        lng = Item.Lng,
                        last_location_update = Item.LastDataUpdate,
                        share_my_location = Item.ShareMyLocation,
                        url = Item.Url,
                        name = Item.Name,
                        lastseen_unix_time = Item.LastseenUnixTime
                    });

                    S_UserId = Item.UserId;
                    Txt_username.Text = Item.Name;

                    var dataabout = IMethods.Fun_String.StringNullRemover(Item.About);
                    if (dataabout != "Empty")
                        Txt_About.Text =
                            IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                    else
                        Txt_About.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " + Settings.Application_Name;

                    var AvatarSplit = Item.Avatar.Split('/').Last();
                    var getImage_Avatar =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar, 1);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.Avatar);
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", Item.Avatar, 1);
                    }

                    var coverSplit = Item.Cover.Split('/').Last();
                    var getImage_Cover =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, coverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.Cover);
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", Item.Cover);
                    }

                    if (Item.UserId != UserDetails.User_id)
                    {
                        if (S_Can_follow == 0 && Item.is_following == "0")
                            Btn_AddUser.Visibility = ViewStates.Gone;

                        if (Item.is_following == "1") // My Friend
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;
                            Btn_AddUser.SetColor(Color.ParseColor("#efefef"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_tick);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#444444"));
                            Btn_AddUser.Tag = "friends";
                        }
                        else if (Item.is_following == "2") // Request
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;
                            Btn_AddUser.SetColor(Color.ParseColor("#efefef"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_requestAdd);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#444444"));
                            Btn_AddUser.Tag = "request";
                        }
                        else if (Item.is_following == "0") //Not Friend
                        {
                            Btn_AddUser.Visibility = ViewStates.Visible;

                            Btn_AddUser.SetColor(Color.ParseColor("#6666ff"));
                            Btn_AddUser.SetImageResource(Resource.Drawable.ic_add);
                            Btn_AddUser.Drawable.SetTint(Color.ParseColor("#ffffff"));
                            Btn_AddUser.Tag = "Add";
                        }
                    }
                    else
                    {
                        Btn_AddUser.Visibility = ViewStates.Gone;
                    }

                    S_Url_User = Item.Url;

                    //Set Privacy User
                    //==================================

                    //S_PrivacyBirth = Item.birth_privacy;
                    //S_PrivacyFollow = Item.follow_privacy;
                    //S_PrivacyFriend = Item.friend_privacy;
                    //S_PrivacyMessage = Item.message_privacy;


                    if (S_PrivacyFollow == "0") //Lbl_Everyone
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else if (S_PrivacyFollow == "1") //Lbl_People_i_Follow
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else //Lbl_No_body
                        Btn_AddUser.Visibility = ViewStates.Gone;

                    // details
                    if (Item.Details != null)
                    {
                        if (Item.UserId != UserDetails.User_id)
                        {
                            if (Settings.ConnectivitySystem == "1") // Following
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Following);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.following_count));
                            }
                            else // Friend
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Post);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.post_count));
                            }
                        }
                        else
                        {
                            Btn_AddUser.Visibility = ViewStates.Gone;
                        }

                        Txt_CountLikes.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.likes_count));
                        Txt_FriendsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.following_count));
                        Txt_PhotosCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.album_count));
                        Txt_GroupsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.groups_count));
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void loadData_Item_ProUsers()
        {
            try
            {
                var Item =
                    JsonConvert.DeserializeObject<Get_General_Data_Object.Pro_Users>(Intent.GetStringExtra("UserItem"));
                if (Item != null)
                {
                    ListDataUserChat.Clear();
                    ListDataUserChat.Add(new Classes.UserChat
                    {
                        user_id = Item.user_id,
                        username = Item.username,
                        email = Item.email,
                        first_name = Item.first_name,
                        last_name = Item.last_name,
                        avatar = Item.avatar,
                        cover = Item.cover,
                        relationship_id = Item.relationship_id,
                        address = Item.address,
                        working = Item.working,
                        working_link = Item.working_link,
                        about = Item.about,
                        school = Item.school,
                        gender = Item.gender,
                        birthday = Item.birthday,
                        website = Item.website,
                        facebook = Item.facebook,
                        google = Item.google,
                        twitter = Item.twitter,
                        linkedin = Item.linkedin,
                        youtube = Item.youtube,
                        vk = Item.vk,
                        instagram = Item.instagram,
                        language = Item.language,
                        ip_address = Item.ip_address,
                        follow_privacy = Item.follow_privacy,
                        friend_privacy = Item.friend_privacy,
                        post_privacy = Item.post_privacy,
                        message_privacy = Item.message_privacy,
                        confirm_followers = Item.confirm_followers,
                        show_activities_privacy = Item.show_activities_privacy,
                        birth_privacy = Item.birth_privacy,
                        visit_privacy = Item.visit_privacy,
                        lastseen = Item.lastseen,
                        e_sentme_msg = Item.e_sentme_msg,
                        e_last_notif = Item.e_last_notif,
                        status = Item.status,
                        active = Item.active,
                        admin = Item.admin,
                        registered = Item.registered,
                        phone_number = Item.phone_number,
                        is_pro = Item.is_pro,
                        pro_type = Item.pro_type,
                        timezone = Item.timezone,
                        referrer = Item.referrer,
                        balance = Item.balance,
                        paypal_email = Item.paypal_email,
                        notifications_sound = Item.notifications_sound,
                        order_posts_by = Item.order_posts_by,
                        device_id = Item.device_id,
                        web_device_id = Item.web_device_id,
                        wallet = Item.wallet,
                        lat = Item.lat,
                        lng = Item.lng,
                        last_location_update = Item.last_location_update,
                        share_my_location = Item.share_my_location,
                        url = Item.url,
                        name = Item.name,
                        lastseen_unix_time = Item.lastseen_unix_time
                    });

                    S_UserId = Item.user_id;
                    Txt_username.Text = Item.name;

                    var dataabout = IMethods.Fun_String.StringNullRemover(Item.about);
                    if (dataabout != "Empty")
                        Txt_About.Text =
                            IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                    else
                        Txt_About.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " + Settings.Application_Name;

                    var AvatarSplit = Item.avatar.Split('/').Last();
                    var getImage_Avatar =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar, 1);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.avatar);
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", Item.avatar, 1);
                    }

                    var coverSplit = Item.cover.Split('/').Last();
                    var getImage_Cover =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, coverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.cover);
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", Item.cover);
                    }
                     
                    S_Url_User = Item.url;

                    //Set Privacy User
                    //==================================

                    S_PrivacyBirth = Item.birth_privacy;
                    S_PrivacyFollow = Item.follow_privacy;
                    S_PrivacyFriend = Item.friend_privacy;
                    S_PrivacyMessage = Item.message_privacy;


                    if (S_PrivacyFollow == "0") //Lbl_Everyone
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else if (S_PrivacyFollow == "1") //Lbl_People_i_Follow
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else //Lbl_No_body
                        Btn_AddUser.Visibility = ViewStates.Gone;

                    // details
                    if (Item.details != null)
                    {
                        if (Item.user_id != UserDetails.User_id)
                        {
                            if (Settings.ConnectivitySystem == "1") // Following
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Following);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.following_count));
                            }
                            else // Friend
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Post);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.post_count));
                            }
                        }
                        else
                        {
                            Btn_AddUser.Visibility = ViewStates.Gone;
                        }

                        Txt_CountLikes.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.likes_count));
                        Txt_FriendsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.following_count));
                        Txt_PhotosCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.album_count));
                        Txt_GroupsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.details.groups_count));
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void loadData_Item_Articles()
        {
            try
            {
                var Item =
                    JsonConvert.DeserializeObject<Get_Users_Articles_Object.Article>(Intent.GetStringExtra("UserItem"));
                if (Item != null)
                {
                    ListDataUserChat.Clear();
                    ListDataUserChat.Add(new Classes.UserChat
                    {
                        user_id = Item.author.user_id,
                        username = Item.author.username,
                        email = Item.author.email,
                        first_name = Item.author.first_name,
                        last_name = Item.author.last_name,
                        avatar = Item.author.avatar,
                        cover = Item.author.cover,
                        relationship_id = Item.author.relationship_id,
                        address = Item.author.address,
                        working = Item.author.working,
                        working_link = Item.author.working_link,
                        about = Item.author.about,
                        school = Item.author.school,
                        gender = Item.author.gender,
                        birthday = Item.author.birthday,
                        website = Item.author.website,
                        facebook = Item.author.facebook,
                        google = Item.author.google,
                        twitter = Item.author.twitter,
                        linkedin = Item.author.linkedin,
                        youtube = Item.author.youtube,
                        vk = Item.author.vk,
                        instagram = Item.author.instagram,
                        language = Item.author.language,
                        ip_address = Item.author.ip_address,
                        follow_privacy = Item.author.follow_privacy,
                        friend_privacy = Item.author.friend_privacy,
                        post_privacy = Item.author.post_privacy,
                        message_privacy = Item.author.message_privacy,
                        confirm_followers = Item.author.confirm_followers,
                        show_activities_privacy = Item.author.show_activities_privacy,
                        birth_privacy = Item.author.birth_privacy,
                        visit_privacy = Item.author.visit_privacy,
                        lastseen = Item.author.lastseen,
                        e_sentme_msg = Item.author.e_sentme_msg,
                        e_last_notif = Item.author.e_last_notif,
                        status = Item.author.status,
                        active = Item.author.active,
                        admin = Item.author.admin,
                        registered = Item.author.registered,
                        phone_number = Item.author.phone_number,
                        is_pro = Item.author.is_pro,
                        pro_type = Item.author.pro_type,
                        timezone = Item.author.timezone,
                        referrer = Item.author.referrer,
                        balance = Item.author.balance,
                        paypal_email = Item.author.paypal_email,
                        notifications_sound = Item.author.notifications_sound,
                        order_posts_by = Item.author.order_posts_by,
                        device_id = Item.author.device_id,
                        web_device_id = Item.author.web_device_id,
                        wallet = Item.author.wallet,
                        lat = Item.author.lat,
                        lng = Item.author.lng,
                        last_location_update = Item.author.last_location_update,
                        share_my_location = Item.author.share_my_location,
                        url = Item.author.url,
                        name = Item.author.name,
                        lastseen_unix_time = Item.author.lastseen_unix_time
                    });

                    S_UserId = Item.author.user_id;
                    Txt_username.Text = Item.author.name;

                    var dataabout = IMethods.Fun_String.StringNullRemover(Item.author.about);
                    if (dataabout != "Empty")
                        Txt_About.Text =
                            IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                    else
                        Txt_About.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " + Settings.Application_Name;

                    var AvatarSplit = Item.author.avatar.Split('/').Last();
                    var getImage_Avatar =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar, 1);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage,
                            Item.author.avatar);
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", Item.author.avatar, 1);
                    }

                    var coverSplit = Item.author.cover.Split('/').Last();
                    var getImage_Cover =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, coverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage,
                            Item.author.cover);
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", Item.author.cover);
                    }
                    
                    S_Url_User = Item.url;

                    //Set Privacy User
                    //==================================

                    S_PrivacyBirth = Item.author.birth_privacy;
                    S_PrivacyFollow = Item.author.follow_privacy;
                    S_PrivacyFriend = Item.author.friend_privacy;
                    S_PrivacyMessage = Item.author.message_privacy;


                    if (S_PrivacyFollow == "0") //Lbl_Everyone
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else if (S_PrivacyFollow == "1") //Lbl_People_i_Follow
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else //Lbl_No_body
                        Btn_AddUser.Visibility = ViewStates.Gone;

                    // details
                    if (Item.author.details != null)
                    {
                        if (Item.author.user_id != UserDetails.User_id)
                        {
                            if (Settings.ConnectivitySystem == "1") // Following
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Following);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(
                                        int.Parse(Item.author.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(
                                        int.Parse(Item.author.details.following_count));
                            }
                            else // Friend
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Post);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(
                                        int.Parse(Item.author.details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.author.details.post_count));
                            }
                        }
                        else
                        {
                            Btn_AddUser.Visibility = ViewStates.Gone;
                        }

                        Txt_CountLikes.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.author.details.likes_count));
                        Txt_FriendsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.author.details.following_count));
                        Txt_PhotosCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.author.details.album_count));
                        Txt_GroupsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.author.details.groups_count));
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void loadData_Item_PostLikedUsers()
        {
            try
            {
                var Item =
                    JsonConvert.DeserializeObject<Get_Post_Data_Object.PostLikedUsers>(
                        Intent.GetStringExtra("UserItem"));
                if (Item != null)
                {
                    ListDataUserChat.Clear();
                    ListDataUserChat.Add(new Classes.UserChat
                    {
                        user_id = Item.UserId,
                        username = Item.Username,
                        email = Item.Email,
                        first_name = Item.FirstName,
                        last_name = Item.LastName,
                        avatar = Item.Avatar,
                        cover = Item.Cover,
                        relationship_id = Item.RelationshipId,
                        address = Item.Address,
                        working = Item.Working,
                        working_link = Item.WorkingLink,
                        about = Item.About,
                        school = Item.School,
                        gender = Item.Gender,
                        birthday = Item.Birthday,
                        website = Item.Website,
                        facebook = Item.Facebook,
                        google = Item.Google,
                        twitter = Item.Twitter,
                        linkedin = Item.Linkedin,
                        youtube = Item.Youtube,
                        vk = Item.Vk,
                        instagram = Item.Instagram,
                        language = Item.Language,
                        ip_address = Item.IpAddress,
                        follow_privacy = Item.FollowPrivacy,
                        friend_privacy = Item.FriendPrivacy,
                        post_privacy = Item.PostPrivacy,
                        message_privacy = Item.MessagePrivacy,
                        confirm_followers = Item.ConfirmFollowers,
                        show_activities_privacy = Item.ShowActivitiesPrivacy,
                        birth_privacy = Item.BirthPrivacy,
                        visit_privacy = Item.VisitPrivacy,
                        lastseen = Item.Lastseen,
                        e_sentme_msg = Item.SentmeMsg,
                        e_last_notif = Item.LastNotif,
                        status = Item.Status,
                        active = Item.Active,
                        admin = Item.Admin,
                        registered = Item.Registered,
                        phone_number = Item.PhoneNumber,
                        is_pro = Item.IsPro,
                        pro_type = Item.ProType,
                        timezone = Item.Timezone,
                        referrer = Item.Referrer,
                        balance = Item.Balance,
                        paypal_email = Item.PaypalEmail,
                        notifications_sound = Item.NotificationsSound,
                        order_posts_by = Item.OrderPostsBy,
                        device_id = Item.DeviceId,
                        web_device_id = Item.WebDeviceId,
                        wallet = Item.Wallet,
                        lat = Item.Lat,
                        lng = Item.Lng,
                        last_location_update = Item.LastDataUpdate,
                        share_my_location = Item.ShareMyLocation,
                        url = Item.Url,
                        name = Item.Name,
                        lastseen_unix_time = Item.LastseenUnixTime
                    });

                    S_UserId = Item.UserId;
                    Txt_username.Text = Item.Name;

                    var dataabout = IMethods.Fun_String.StringNullRemover(Item.About);
                    if (dataabout != "Empty")
                        Txt_About.Text =
                            IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                    else
                        Txt_About.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " + Settings.Application_Name;

                    var AvatarSplit = Item.Avatar.Split('/').Last();
                    var getImage_Avatar =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar, 1);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.Avatar);
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", Item.Avatar, 1);
                    }

                    var coverSplit = Item.Cover.Split('/').Last();
                    var getImage_Cover =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, coverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.Cover);
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", Item.Cover);
                    }

                    
                    S_Url_User = Item.Url;

                    //Set Privacy User
                    //==================================

                    S_PrivacyBirth = Item.BirthPrivacy;
                    S_PrivacyFollow = Item.FollowPrivacy;
                    S_PrivacyFriend = Item.FriendPrivacy;
                    S_PrivacyMessage = Item.MessagePrivacy;


                    if (S_PrivacyFollow == "0") //Lbl_Everyone
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else if (S_PrivacyFollow == "1") //Lbl_People_i_Follow
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else //Lbl_No_body
                        Btn_AddUser.Visibility = ViewStates.Gone;

                    // details
                    if (Item.Details != null)
                    {
                        if (Item.UserId != UserDetails.User_id)
                        {
                            if (Settings.ConnectivitySystem == "1") // Following
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Following);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.following_count));
                            }
                            else // Friend
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Post);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.post_count));
                            }
                        }
                        else
                        {
                            Btn_AddUser.Visibility = ViewStates.Gone;
                        }

                        Txt_CountLikes.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.likes_count));
                        Txt_FriendsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.following_count));
                        Txt_PhotosCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.album_count));
                        Txt_GroupsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.groups_count));
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void loadData_Item_PostWonderedUsers()
        {
            try
            {
                var Item =
                    JsonConvert.DeserializeObject<Get_Post_Data_Object.PostWonderedUsers>(
                        Intent.GetStringExtra("UserItem"));
                if (Item != null)
                {
                    ListDataUserChat.Clear();
                    ListDataUserChat.Add(new Classes.UserChat
                    {
                        user_id = Item.UserId,
                        username = Item.Username,
                        email = Item.Email,
                        first_name = Item.FirstName,
                        last_name = Item.LastName,
                        avatar = Item.Avatar,
                        cover = Item.Cover,
                        relationship_id = Item.RelationshipId,
                        address = Item.Address,
                        working = Item.Working,
                        working_link = Item.WorkingLink,
                        about = Item.About,
                        school = Item.School,
                        gender = Item.Gender,
                        birthday = Item.Birthday,
                        website = Item.Website,
                        facebook = Item.Facebook,
                        google = Item.Google,
                        twitter = Item.Twitter,
                        linkedin = Item.Linkedin,
                        youtube = Item.Youtube,
                        vk = Item.Vk,
                        instagram = Item.Instagram,
                        language = Item.Language,
                        ip_address = Item.IpAddress,
                        follow_privacy = Item.FollowPrivacy,
                        friend_privacy = Item.FriendPrivacy,
                        post_privacy = Item.PostPrivacy,
                        message_privacy = Item.MessagePrivacy,
                        confirm_followers = Item.ConfirmFollowers,
                        show_activities_privacy = Item.ShowActivitiesPrivacy,
                        birth_privacy = Item.BirthPrivacy,
                        visit_privacy = Item.VisitPrivacy,
                        lastseen = Item.Lastseen,
                        e_sentme_msg = Item.SentmeMsg,
                        e_last_notif = Item.LastNotif,
                        status = Item.Status,
                        active = Item.Active,
                        admin = Item.Admin,
                        registered = Item.Registered,
                        phone_number = Item.PhoneNumber,
                        is_pro = Item.IsPro,
                        pro_type = Item.ProType,
                        timezone = Item.Timezone,
                        referrer = Item.Referrer,
                        balance = Item.Balance,
                        paypal_email = Item.PaypalEmail,
                        notifications_sound = Item.NotificationsSound,
                        order_posts_by = Item.OrderPostsBy,
                        device_id = Item.DeviceId,
                        web_device_id = Item.WebDeviceId,
                        wallet = Item.Wallet,
                        lat = Item.Lat,
                        lng = Item.Lng,
                        last_location_update = Item.LastDataUpdate,
                        share_my_location = Item.ShareMyLocation,
                        url = Item.Url,
                        name = Item.Name,
                        lastseen_unix_time = Item.LastseenUnixTime
                    });

                    S_UserId = Item.UserId;
                    Txt_username.Text = Item.Name;

                    var dataabout = IMethods.Fun_String.StringNullRemover(Item.About);
                    if (dataabout != "Empty")
                        Txt_About.Text =
                            IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                    else
                        Txt_About.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " + Settings.Application_Name;

                    var AvatarSplit = Item.Avatar.Split('/').Last();
                    var getImage_Avatar =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", getImage_Avatar, 1);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.Avatar);
                        ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", Item.Avatar, 1);
                    }

                    var coverSplit = Item.Cover.Split('/').Last();
                    var getImage_Cover =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, coverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, Item.Cover);
                        ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", Item.Cover);
                    }

                    S_Url_User = Item.Url;

                    //Set Privacy User
                    //==================================

                    S_PrivacyBirth = Item.BirthPrivacy;
                    S_PrivacyFollow = Item.FollowPrivacy;
                    S_PrivacyFriend = Item.FriendPrivacy;
                    S_PrivacyMessage = Item.MessagePrivacy;


                    if (S_PrivacyFollow == "0") //Lbl_Everyone
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else if (S_PrivacyFollow == "1") //Lbl_People_i_Follow
                        Btn_AddUser.Visibility = ViewStates.Visible;
                    else //Lbl_No_body
                        Btn_AddUser.Visibility = ViewStates.Gone;

                    // details
                    if (Item.Details != null)
                    {
                        if (Item.UserId != UserDetails.User_id)
                        {
                            if (Settings.ConnectivitySystem == "1") // Following
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Following);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.following_count));
                            }
                            else // Friend
                            {
                                Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                                Txt_Following.Text = GetText(Resource.String.Lbl_Post);

                                Txt_CountFollowers.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.followers_count));
                                Txt_CountFollowing.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.post_count));
                            }
                        }
                        else
                        {
                            Btn_AddUser.Visibility = ViewStates.Gone;
                        }

                        Txt_CountLikes.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.likes_count));
                        Txt_FriendsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.following_count));
                        Txt_PhotosCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.album_count));
                        Txt_GroupsCounter.Text =
                            IMethods.Fun_String.FormatPriceValue(int.Parse(Item.Details.groups_count));
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion

        //Get Data User API
        public async void Get_UserProfileData_Api()
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
                            //Add Data User
                            //=======================================

                            // user_data
                            if (result.user_data != null)
                            {
                                ListDataUserChat.Clear();
                                ListDataUserChat.Add(new Classes.UserChat
                                {
                                    user_id = result.user_data.user_id,
                                    username = result.user_data.username,
                                    email = result.user_data.email,
                                    first_name = result.user_data.first_name,
                                    last_name = result.user_data.last_name,
                                    avatar = result.user_data.avatar,
                                    cover = result.user_data.cover,
                                    relationship_id = result.user_data.relationship_id,
                                    address = result.user_data.address,
                                    working = result.user_data.working,
                                    working_link = result.user_data.working_link,
                                    about = result.user_data.about,
                                    school = result.user_data.school,
                                    gender = result.user_data.gender,
                                    birthday = result.user_data.birthday,
                                    website = result.user_data.website,
                                    facebook = result.user_data.facebook,
                                    google = result.user_data.google,
                                    twitter = result.user_data.twitter,
                                    linkedin = result.user_data.linkedin,
                                    youtube = result.user_data.youtube,
                                    vk = result.user_data.vk,
                                    instagram = result.user_data.instagram,
                                    language = result.user_data.language,
                                    ip_address = result.user_data.ip_address,
                                    follow_privacy = result.user_data.follow_privacy,
                                    friend_privacy = result.user_data.friend_privacy,
                                    post_privacy = result.user_data.post_privacy,
                                    message_privacy = result.user_data.message_privacy,
                                    confirm_followers = result.user_data.confirm_followers,
                                    show_activities_privacy = result.user_data.show_activities_privacy,
                                    birth_privacy = result.user_data.birth_privacy,
                                    visit_privacy = result.user_data.visit_privacy,
                                    lastseen = result.user_data.lastseen,
                                    e_sentme_msg = result.user_data.e_sentme_msg,
                                    e_last_notif = result.user_data.e_last_notif,
                                    status = result.user_data.status,
                                    active = result.user_data.active,
                                    admin = result.user_data.admin,
                                    registered = result.user_data.registered,
                                    phone_number = result.user_data.phone_number,
                                    is_pro = result.user_data.is_pro,
                                    pro_type = result.user_data.pro_type,
                                    timezone = result.user_data.timezone,
                                    referrer = result.user_data.referrer,
                                    balance = result.user_data.balance,
                                    paypal_email = result.user_data.paypal_email,
                                    notifications_sound = result.user_data.notifications_sound,
                                    order_posts_by = result.user_data.order_posts_by,
                                    device_id = result.user_data.device_id,
                                    web_device_id = result.user_data.web_device_id,
                                    wallet = result.user_data.wallet,
                                    lat = result.user_data.lat,
                                    lng = result.user_data.lng,
                                    last_location_update = result.user_data.last_location_update,
                                    share_my_location = result.user_data.share_my_location,
                                    url = result.user_data.url,
                                    name = result.user_data.name,
                                    lastseen_unix_time = result.user_data.lastseen_unix_time
                                });

                                Txt_username.Text = result.user_data.name;

                                var dataabout = IMethods.Fun_String.StringNullRemover(result.user_data.about);
                                if (dataabout != "Empty")
                                    Txt_About.Text =
                                        IMethods.Fun_String.DecodeString(
                                            IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                                else
                                    Txt_About.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " +
                                                     Settings.Application_Name;

                                var AvatarSplit = result.user_data.avatar.Split('/').Last();
                                var getImage_Avatar =
                                    IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                                if (getImage_Avatar != "File Dont Exists")
                                {
                                    ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png",
                                        getImage_Avatar, 1);
                                }
                                else
                                {
                                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage,
                                        result.user_data.avatar);
                                    ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png",
                                        result.user_data.avatar, 1);
                                }

                                var coverSplit = result.user_data.cover.Split('/').Last();
                                var getImage_Cover =
                                    IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, coverSplit);
                                if (getImage_Cover != "File Dont Exists")
                                {
                                    ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                                }
                                else
                                {
                                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage,
                                        result.user_data.cover);
                                    ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg",
                                        result.user_data.cover);
                                }

                                S_Url_User = result.user_data.url;

                                if (result.user_data.user_id != UserDetails.User_id)
                                {
                                    S_Can_follow = result.user_data.can_follow;

                                    if (S_Can_follow == 0 && result.user_data.is_following == 0)
                                        Btn_AddUser.Visibility = ViewStates.Gone;

                                    if (result.user_data.is_following == 1) // My Friend
                                    {
                                        Btn_AddUser.Visibility = ViewStates.Visible;
                                        Btn_AddUser.SetColor(Color.ParseColor("#efefef"));
                                        Btn_AddUser.SetImageResource(Resource.Drawable.ic_tick);
                                        Btn_AddUser.Drawable.SetTint(Color.ParseColor("#444444"));
                                        Btn_AddUser.Tag = "friends";
                                    }
                                    else if (result.user_data.is_following == 2) // Request
                                    {
                                        Btn_AddUser.Visibility = ViewStates.Visible;
                                        Btn_AddUser.SetColor(Color.ParseColor("#efefef"));
                                        Btn_AddUser.SetImageResource(Resource.Drawable.ic_requestAdd);
                                        Btn_AddUser.Drawable.SetTint(Color.ParseColor("#444444"));
                                        Btn_AddUser.Tag = "request";
                                    }
                                    else if (result.user_data.is_following == 0) //Not Friend
                                    {
                                        Btn_AddUser.Visibility = ViewStates.Visible;

                                        Btn_AddUser.SetColor(Color.ParseColor("#6666ff"));
                                        Btn_AddUser.SetImageResource(Resource.Drawable.ic_add);
                                        Btn_AddUser.Drawable.SetTint(Color.ParseColor("#ffffff"));
                                        Btn_AddUser.Tag = "Add";
                                    }
                                }
                                else
                                {
                                    Btn_AddUser.Visibility = ViewStates.Gone;
                                }

                                //Set Privacy User
                                //==================================

                                S_PrivacyBirth = result.user_data.birth_privacy;
                                S_PrivacyFollow = result.user_data.follow_privacy;
                                S_PrivacyFriend = result.user_data.friend_privacy;
                                S_PrivacyMessage = result.user_data.message_privacy;


                                if (S_PrivacyFollow == "0") //Lbl_Everyone
                                    Btn_AddUser.Visibility = ViewStates.Visible;
                                else if (S_PrivacyFollow == "1") //Lbl_People_i_Follow
                                    Btn_AddUser.Visibility = ViewStates.Visible;
                                else //Lbl_No_body
                                    Btn_AddUser.Visibility = ViewStates.Gone;

                                // details
                                if (result.user_data.details != null)
                                {
                                    if (Settings.ConnectivitySystem == "1") // Following
                                    {
                                        Txt_Followers.Text = GetText(Resource.String.Lbl_Followers);
                                        Txt_Following.Text = GetText(Resource.String.Lbl_Following);

                                        Txt_CountFollowers.Text =
                                            IMethods.Fun_String.FormatPriceValue(
                                                int.Parse(result.user_data.details.followers_count));
                                        Txt_CountFollowing.Text =
                                            IMethods.Fun_String.FormatPriceValue(
                                                int.Parse(result.user_data.details.following_count));
                                    }
                                    else // Friend
                                    {
                                        Txt_Followers.Text = GetText(Resource.String.Lbl_Friends);
                                        Txt_Following.Text = GetText(Resource.String.Lbl_Post);

                                        Txt_CountFollowers.Text =
                                            IMethods.Fun_String.FormatPriceValue(
                                                int.Parse(result.user_data.details.followers_count));
                                        Txt_CountFollowing.Text =
                                            IMethods.Fun_String.FormatPriceValue(
                                                int.Parse(result.user_data.details.post_count));
                                    }

                                    Txt_CountLikes.Text =
                                        IMethods.Fun_String.FormatPriceValue(
                                            int.Parse(result.user_data.details.likes_count));
                                    Txt_FriendsCounter.Text =
                                        IMethods.Fun_String.FormatPriceValue(
                                            int.Parse(result.user_data.details.following_count));
                                    Txt_PhotosCounter.Text =
                                        IMethods.Fun_String.FormatPriceValue(
                                            int.Parse(result.user_data.details.album_count));
                                    Txt_GroupsCounter.Text =
                                        IMethods.Fun_String.FormatPriceValue(
                                            int.Parse(result.user_data.details.groups_count));
                                }
                            }

                            if (S_PrivacyFriend == "0" ||
                                result.user_data?.is_following == 1 && S_PrivacyFriend == "1" || S_PrivacyFriend == "2")
                            {
                                // followers
                                if (result.followers.Length > 0)
                                {
                                    UserFriendsAdapter.mAllUserFriendsList =
                                        new ObservableCollection<Get_User_Data_Object.Followers>(result.followers);
                                    UserFriendsAdapter.mUserFriendsList =
                                        new ObservableCollection<Get_User_Data_Object.Followers>(
                                            result.followers.Take(12));
                                    UserFriendsAdapter.BindEnd();

                                    layout_Friends.Visibility = ViewStates.Visible;
                                }
                                else
                                {
                                    layout_Friends.Visibility = ViewStates.Gone;
                                }
                            }
                            else
                            {
                                layout_Friends.Visibility = ViewStates.Gone;
                            }

                            // following
                            if (result.following.Length > 0)
                            {
                            }

                            // joined_groups
                            if (result.joined_groups.Length > 0)
                            {
                                UserGroupsAdapter.mAllUserGroupsList =
                                    new ObservableCollection<Get_User_Data_Object.Joined_Groups>(result.joined_groups);
                                UserGroupsAdapter.mUserGroupsList =
                                    new ObservableCollection<Get_User_Data_Object.Joined_Groups>(
                                        result.joined_groups.Take(12));
                                UserGroupsAdapter.BindEnd();

                                layout_Groups.Visibility = ViewStates.Visible;
                            }
                            else
                            {
                                layout_Groups.Visibility = ViewStates.Gone;
                            }

                            // liked_pages
                            if (result.liked_pages.Length > 0)
                            {
                                UserPagesAdapter.mAllUserPagesList =
                                    new ObservableCollection<Get_User_Data_Object.Liked_Pages>(result.liked_pages);

                                layout_Pages.Visibility = ViewStates.Visible;

                                try
                                {
                                    for (var i = 0; i < 4; i++)
                                        if (i == 0)
                                            ImageServiceLoader.Load_Image(PageImage1, "no_profile_image.png",
                                                result.liked_pages[i].avatar, 1, true, 20);
                                        else if (i == 1)
                                            ImageServiceLoader.Load_Image(PageImage2, "no_profile_image.png",
                                                result.liked_pages[i].avatar, 1, true, 20);
                                        else if (i == 2)
                                            ImageServiceLoader.Load_Image(PageImage3, "no_profile_image.png",
                                                result.liked_pages[i].avatar, 1, true, 20);
                                }
                                catch (Exception e)
                                {
                                    Crashes.TrackError(e);
                                }
                            }
                            else
                            {
                                layout_Pages.Visibility = ViewStates.Gone;
                            }
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
                    Get_AlbumUser_Api();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_UserProfileData_Api();
            }
        }

        //Get Photo API
        public async void Get_AlbumUser_Api()
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    // Toast.MakeText(this, this.GetString(Resource.String.Lbl_CheckYourInternetConnection),ToastLength.Short).Show();
                }
                else
                {
                    var (Api_status, Respond) = await Client.Album.Get_User_Albums(S_UserId);
                    if (Api_status == 200)
                    {
                        if (Respond is Get_User_Albums_Object result)
                        {
                            //Albums User
                            //=======================================
                            if (result.albums.Count > 0)
                            {
                                Txt_PhotosCounter.Text =
                                    IMethods.Fun_String.FormatPriceValue(int.Parse(result.albums.Count.ToString()));

                                UserPhotosAdapter.mAllUserAlbumsList =
                                    new ObservableCollection<Get_User_Albums_Object.Album>(result.albums);

                                UserPhotosAdapter.mUserAlbumsList =
                                    new ObservableCollection<Get_User_Albums_Object.Album>(result.albums.Take(9));
                                UserPhotosAdapter.BindEnd();

                                layout_Photo.Visibility = ViewStates.Visible;
                            }
                            else
                            {
                                layout_Photo.Visibility = ViewStates.Gone;
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
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion

        #region Event

        //Event Show More : Bolck User , Copy Link To Profile 
        private void BtnMoreOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var ctw = new ContextThemeWrapper(this, Resource.Style.PopupMenuStyle);
                var popup = new PopupMenu(ctw, Btn_More);
                popup.MenuInflater.Inflate(Resource.Menu.MoreProfile_menu, popup.Menu);
                popup.Show();
                popup.MenuItemClick += (o, e) =>
                {
                    try
                    {
                        var Id = e.Item.ItemId;
                        switch (Id)
                        {
                            case Resource.Id.menu_BolckUser:
                                OnBlockUser_Button_Click();
                                break;

                            case Resource.Id.menu_CopeLinkToProfile:
                                OnCopeLinkToProfile_Button_Click();
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

        //Event Block User
        private async void OnBlockUser_Button_Click()
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
                    var (api_status, respond) = await Client.Global.Block_User(S_UserId, false).ConfigureAwait(true); //true >> "block"
                    if (api_status == 200)
                    {
                        var dbDatabase = new SqLiteDatabase();
                        dbDatabase.Delete_UsersContact(S_UserId);
                        dbDatabase.Dispose();

                        if (S_UserType == "MyContacts")
                        {
                            var dataMyContacts = MyContacts_Activity.MyContactsAdapter?.mMyContactsList?.FirstOrDefault(a => a.user_id == S_UserId);
                            if (dataMyContacts != null)
                            {
                                MyContacts_Activity.MyContactsAdapter?.Remove(dataMyContacts);
                            }
                        }
                        else if(S_UserType == "MyFollowers")
                        {
                            var dataMyFollowers = MyContacts_Activity.MyFollowersAdapter?.mMyFollowersList?.FirstOrDefault(a => a.user_id == S_UserId);
                            if (dataMyFollowers != null)
                            {
                                MyContacts_Activity.MyFollowersAdapter?.Remove(dataMyFollowers);
                            } 
                        }
                     
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Blocked_successfully), ToastLength.Short).Show();
                        Finish();
                    }
                    else if (api_status == 400)
                    {
                        if (respond is Error_Object error)
                        {
                            var errorText = error._errors.Error_text;
                            //Toast.MakeText(this, errortext, ToastLength.Short).Show();

                            if (errorText.Contains("Invalid or expired access_token"))
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

        //Event Cope Link To Profile
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

        //Event Send Messages To User
        private void BtnMessageOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                IMethods.IApp.OpenApp_BypackageName(this, Settings.Messenger_Package_Name, S_UserId,
                    ListDataUserChat.FirstOrDefault(a => a.user_id == S_UserId));
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Add Friends Or Follower User
        private void BtnAddUserOnClick(object sender, EventArgs eventArgs)
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
                    if (Btn_AddUser.Tag.ToString() == "Add") //(is_following == "0") >> Not Friend
                    {
                        Btn_AddUser.SetColor(Color.ParseColor("#efefef"));
                        Btn_AddUser.SetImageResource(Resource.Drawable.ic_tick);
                        Btn_AddUser.Drawable.SetTint(Color.ParseColor("#444444"));
                        Btn_AddUser.Tag = "friends";
                    }
                    else if (Btn_AddUser.Tag.ToString() == "request") //(is_following == "2") >> Request
                    {
                        Btn_AddUser.SetColor(Color.ParseColor("#efefef"));
                        Btn_AddUser.SetImageResource(Resource.Drawable.ic_tick);
                        Btn_AddUser.Drawable.SetTint(Color.ParseColor("#444444"));
                        Btn_AddUser.Tag = "Add";
                    }
                    else //(is_following == "1") >> Friend
                    {
                        Btn_AddUser.SetColor(Color.ParseColor("#444444"));
                        Btn_AddUser.SetImageResource(Resource.Drawable.ic_add);
                        Btn_AddUser.Drawable.SetTint(Color.ParseColor("#ffffff"));

                        Btn_AddUser.Tag = "Add";

                        var dbDatabase = new SqLiteDatabase();
                        dbDatabase.Delete_UsersContact(S_UserId);
                        dbDatabase.Dispose();
                    }

                    var result = Client.Global.Follow_User(S_UserId).ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Open Profile User Using User_Profile_Activity
        private void UserFriendsAdapter_ItemClick(object sender, UserFriendsAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = UserFriendsAdapter.GetItem(position);
                    if (item != null)
                    {
                        Intent Int;
                        if (item.user_id != UserDetails.User_id)
                        {
                            Int = new Intent(this, typeof(User_Profile_Activity));
                            Int.PutExtra("UserId", item.user_id);
                            Int.PutExtra("UserType", "MyFollowers");
                            Int.AddFlags(ActivityFlags.NewTask);
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

        //Event Open IMG Using ImagePostViewer_Activity
        private void UserPhotosAdapter_ItemClick(object sender, UserPhotosAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = UserPhotosAdapter.GetItem(position);
                    if (item != null)
                    {
                        var Int = new Intent(this, typeof(ImagePostViewer_Activity));
                        Int.PutExtra("Item", JsonConvert.SerializeObject(item));
                        Int.PutExtra("ImageUrl", item.postFile_full);
                        StartActivity(Int);
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Opne profile group Using Group_ProfileActivity
        private void UserGroupsAdapter_OnItemClick(object sender, UserGroupsAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = UserGroupsAdapter.GetItem(position);
                    if (item != null)
                    {
                        var Int = new Intent(this, typeof(Group_Profile_Activity));
                        if (item.user_id != UserDetails.User_id)
                        {
                            Int.PutExtra("UserGroups", JsonConvert.SerializeObject(item));
                            Int.PutExtra("GroupsType", "Joined_UserGroups");
                        }
                        else
                        {
                            Int.PutExtra("MyGroups", JsonConvert.SerializeObject(item));
                            Int.PutExtra("GroupsType", "Joined_MyGroups");
                        }

                        StartActivity(Int);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Show All Group
        private void IconMoreGroup_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (UserGroupsAdapter.mAllUserGroupsList.Count > 0)
                {
                    var Int = new Intent(this, typeof(Groups_Activity));
                    if (S_UserId != UserDetails.User_id)
                        Int.PutExtra("GroupsType", "Manage_UserGroups");
                    else
                        Int.PutExtra("GroupsType", "Manage_MyGroups");
                    Int.PutExtra("UserID", S_UserId);
                    StartActivity(Int);
                }
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
                if (UserPagesAdapter.mAllUserPagesList.Count > 0)
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
                if (UserPhotosAdapter.mAllUserAlbumsList.Count > 0)
                {
                    var Int = new Intent(this, typeof(MyPhotosActivity));
                    Int.PutExtra("UserId", S_UserId);
                    StartActivity(Int);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Show All Followers 
        private void IconMoreFollowers_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (UserFriendsAdapter.mUserFriendsList.Count > 0)
                {
                    if (S_UserId != UserDetails.User_id)
                    {
                        var Int = new Intent(this, typeof(UserContacts_Activity));
                        Int.PutExtra("ContactsType", "UserProfile");
                        StartActivity(Int);
                    }
                    else
                    {
                        var intent = new Intent(this, typeof(MyContacts_Activity));
                        intent.PutExtra("ContactsType", "Following");
                        StartActivity(intent);
                    }
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

        #region hybridController

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
                                Int.PutExtra("Type", "Normal");
                                Int.PutExtra("PostId", S_UserType);
                                Int.PutExtra("isOwner", "Normal");
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