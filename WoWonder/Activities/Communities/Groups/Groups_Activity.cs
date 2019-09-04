using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.Communities.Groups.Adapters;
using WoWonder.Activities.Search;
using WoWonder.Activities.userProfile.Adapters;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Group;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.Communities.Groups
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Groups_Activity : AppCompatActivity
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

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Groups_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Groups_Layout);

                var groupsType = Intent.GetStringExtra("GroupsType") ?? "Data not available";
                if (groupsType != "Data not available" && !string.IsNullOrEmpty(groupsType))
                    GroupsManage_Type = groupsType;

                var dataUser = Intent.GetStringExtra("UserID") ?? "Data not available";
                if (dataUser != "Data not available" && !string.IsNullOrEmpty(groupsType)) UserID = dataUser;

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_ExploreGroup);

                    Txt_Create = FindViewById<TextView>(Resource.Id.toolbar_title);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                GroupsSection = (LinearLayout) FindViewById(Resource.Id.groupLiner);
                ManageGroupsSection = (LinearLayout) FindViewById(Resource.Id.ManagegroupLiner);

                GroupsRecylerView = (RecyclerView) FindViewById(Resource.Id.Recyler);
                ManageGroupsRecylerView = (RecyclerView) FindViewById(Resource.Id.groupsRecyler);

                Groups_Empty = (LinearLayout) FindViewById(Resource.Id.Page_LinerEmpty);

                IconGroups_Empty = (TextView) FindViewById(Resource.Id.Group_icon);

                Txt_Count_ManageGroups = (TextView) FindViewById(Resource.Id.tv_groupscount);
                IconMore_ManageGroups = (TextView) FindViewById(Resource.Id.iv_more_groups);
                Txt_Count_ManageGroups.Visibility = ViewStates.Gone;
                IconMore_ManageGroups.Visibility = ViewStates.Gone;

                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                Btn_SearchRandom = FindViewById<Button>(Resource.Id.SearchRandom_Button);

                IMethods.Set_TextViewIcon("1", IconMore_ManageGroups, IonIcons_Fonts.ChevronRight);
                IMethods.Set_TextViewIcon("2", IconGroups_Empty, "\uf0c0");

                Groups_Empty.Visibility = ViewStates.Gone;

                //*************************************************************************

                GroupsRecylerView.SetLayoutManager(new GridLayoutManager(this, 3));
                GroupsAdapter = new GroupsAdapter(this);
                GroupsRecylerView.AddItemDecoration(new GridSpacingItemDecoration(2, 3, true));
                GroupsRecylerView.SetAdapter(GroupsAdapter);
                GroupsRecylerView.NestedScrollingEnabled = false;

                //*************************************************************************

                //Get Manage my or user groups and set any type adapter
                ManageGroupsRecylerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal,
                    false));
                ManageGroupsAdapter = new UserGroupsAdapter(this);
                ManageGroupsRecylerView.SetAdapter(ManageGroupsAdapter);
                ManageGroupsRecylerView.NestedScrollingEnabled = false;

                //When you have finished fetching the Manage Groups, the second connection is initiated by fetching Get_CommunitiesList_Group()
                Get_ManageGroups();

                //*************************************************************************

                //Show Ads
                AdsGoogle.Ad_RewardedVideo(this);
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
                Txt_Create.Click += CreateButton_OnClick;
                Btn_SearchRandom.Click += BtnSearchRandomOnClick;
                GroupsAdapter.ItemClick += GroupsAdapter_OnItemClick;
                ManageGroupsAdapter.ItemClick += UserManageGroupsAdapter_OnItemClick;
                swipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
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
                Txt_Create.Click -= CreateButton_OnClick;
                Btn_SearchRandom.Click -= BtnSearchRandomOnClick;
                GroupsAdapter.ItemClick -= GroupsAdapter_OnItemClick;
                ManageGroupsAdapter.ItemClick -= UserManageGroupsAdapter_OnItemClick;
                swipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #region ManageGroups

        public void Get_ManageGroups()
        {
            try
            {
                if (GroupsManage_Type == "Manage_UserGroups")
                {
                    ManageGroupsSection.Visibility = ViewStates.Gone;
                    ManageGroupsRecylerView.Visibility = ViewStates.Gone;
                    Txt_Create.Visibility = ViewStates.Gone;
                    Btn_SearchRandom.Visibility = ViewStates.Gone;
                }
                else if (GroupsManage_Type == "Manage_MyGroups")
                {
                    //Get Group From Database 
                    var dbDatabase = new SqLiteDatabase();
                    var localList = dbDatabase.GetAll_ManageGroups();
                    if (localList != null)
                    {
                        ManageGroupsAdapter.mUserGroupsList =
                            new ObservableCollection<Get_User_Data_Object.Joined_Groups>(localList);
                        ManageGroupsAdapter.BindEnd();
                    }

                    dbDatabase.Dispose();
                }


                Get_CommunitiesList_Group();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion

        //Get List Group Using Database
        public void Get_CommunitiesList_Group()
        {
            try
            {
                if (UserID == UserDetails.User_id)
                {
                    //Get Group From Database 
                    var dbDatabase = new SqLiteDatabase();
                    var localList = dbDatabase.Get_Groups();
                    if (localList != null)
                    {
                        GroupsAdapter.mGroupsList = new ObservableCollection<Get_Community_Object.Group>(localList);
                        GroupsAdapter.BindEnd();
                    }

                    dbDatabase.Dispose();
                }

                //Get Group From API 
                Get_CommunitiesList_Group_API();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Get List Group Using API
        public async void Get_CommunitiesList_Group_API()
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    swipeRefreshLayout.Refreshing = false;
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)
                        .Show();
                }
                else
                {
                    var (Api_status, Respond) = await Client.Global.Get_Community(new Settings(), UserID);
                    if (Api_status == 200)
                    {
                        if (Respond is Get_Community_Object result)
                        {
                            if (result.Groups.Count <= 0 && result.likedPages.Count <= 0 && result.Pages.Count <= 0)
                                swipeRefreshLayout.Refreshing = false;

                            var dbDatabase = new SqLiteDatabase();

                            //Add Data
                            //=======================================
                            if (GroupsManage_Type == "Manage_UserGroups")
                            {
                                GroupsAdapter.mGroupsList =
                                    new ObservableCollection<Get_Community_Object.Group>(result.Groups);
                                GroupsAdapter.BindEnd();
                            }
                            else if (GroupsManage_Type == "Manage_MyGroups")
                            {
                                // Groups
                                if (GroupsAdapter.mGroupsList.Count > 0)
                                {
                                    //Bring new groups
                                    var listNew = result.Groups.Where(c =>
                                            !GroupsAdapter.mGroupsList.Select(fc => fc.GroupId).Contains(c.GroupId))
                                        .ToList();
                                    if (listNew.Count > 0)
                                    {
                                        var chkList = listNew.Where(a => a.UserId != UserDetails.User_id).ToList();
                                        if (chkList.Count > 0)
                                        {
                                            //Results differ
                                            Classes.AddRange(GroupsAdapter.mGroupsList, chkList);
                                            GroupsAdapter.BindEnd();

                                            //Insert Or Replace Just New Data To Database
                                            if (UserID == UserDetails.User_id)
                                                dbDatabase.Insert_Or_Replace_GroupsTable(
                                                    new ObservableCollection<Get_Community_Object.Group>(chkList));
                                        }
                                    }
                                }
                                else
                                {
                                    var chkList = result.Groups.Where(a => a.UserId != UserDetails.User_id).ToList();
                                    if (chkList.Count > 0)
                                    {
                                        GroupsAdapter.mGroupsList =
                                            new ObservableCollection<Get_Community_Object.Group>(chkList);
                                        GroupsAdapter.BindEnd();

                                        //Insert Or Replace Just New Data To Database
                                        if (UserID == UserDetails.User_id)
                                            dbDatabase.Insert_Or_Replace_GroupsTable(
                                                new ObservableCollection<Get_Community_Object.Group>(chkList));
                                    }
                                }

                                //====================================

                                //Manage 
                                if (ManageGroupsAdapter.mUserGroupsList.Count > 0)
                                {
                                    var chkListManage = result.Groups.Where(a => a.UserId == UserDetails.User_id)
                                        .ToList();
                                    if (chkListManage.Count > 0)
                                    {
                                        //Bring new groups
                                        var listNew = chkListManage.Where(c =>
                                            !ManageGroupsAdapter.mUserGroupsList.Select(fc => fc.group_id)
                                                .Contains(c.GroupId)).ToList();
                                        if (listNew.Count > 0)
                                        {
                                            var list = chkListManage.Select(group =>
                                                new Get_User_Data_Object.Joined_Groups
                                                {
                                                    id = group.Id,
                                                    user_id = group.UserId,
                                                    group_name = group.GroupName,
                                                    group_title = group.GroupTitle,
                                                    avatar = group.Avatar,
                                                    cover = group.Cover,
                                                    about = group.About,
                                                    category = group.Category,
                                                    privacy = group.Privacy,
                                                    join_privacy = group.JoinPrivacy,
                                                    active = group.Active,
                                                    registered = group.Registered,
                                                    group_id = group.GroupId,
                                                    url = group.Url,
                                                    name = group.Name,
                                                    category_id = group.CategoryId,
                                                    type = group.Type,
                                                    username = group.Username
                                                }).ToList();

                                            //Results differ
                                            Classes.AddRange(ManageGroupsAdapter.mUserGroupsList, list);
                                            ManageGroupsAdapter.BindEnd();

                                            //Insert Or Replace Just New Data To Database
                                            if (UserID == UserDetails.User_id)
                                                dbDatabase.InsertOrReplace_ManageGroupsTable(
                                                    new ObservableCollection<Get_User_Data_Object.Joined_Groups>(list));
                                        }
                                    }
                                }
                                else
                                {
                                    var chkListManage = result.Groups.Where(a => a.UserId == UserDetails.User_id)
                                        .ToList();
                                    if (chkListManage.Count > 0)
                                    {
                                        var list = chkListManage.Select(group => new Get_User_Data_Object.Joined_Groups
                                        {
                                            id = group.Id,
                                            user_id = group.UserId,
                                            group_name = group.GroupName,
                                            group_title = group.GroupTitle,
                                            avatar = group.Avatar,
                                            cover = group.Cover,
                                            about = group.About,
                                            category = group.Category,
                                            privacy = group.Privacy,
                                            join_privacy = group.JoinPrivacy,
                                            active = group.Active,
                                            registered = group.Registered,
                                            group_id = group.GroupId,
                                            url = group.Url,
                                            name = group.Name,
                                            category_id = group.CategoryId,
                                            type = group.Type,
                                            username = group.Username
                                        }).ToList();

                                        ManageGroupsAdapter.mUserGroupsList =
                                            new ObservableCollection<Get_User_Data_Object.Joined_Groups>(list);
                                        ManageGroupsAdapter.BindEnd();

                                        //Insert Or Replace Just New Data To Database
                                        if (UserID == UserDetails.User_id)
                                            dbDatabase.InsertOrReplace_ManageGroupsTable(
                                                new ObservableCollection<Get_User_Data_Object.Joined_Groups>(list));
                                    }
                                }

                                //====================================

                                // pages
                                if (result.Pages.Count > 0)
                                {
                                    var pagesList = new ObservableCollection<Get_Community_Object.Page>(result.Pages);

                                    //Update All Data To Database
                                    if (UserID == UserDetails.User_id)
                                        dbDatabase.Insert_Or_Replace_PagesTable(pagesList);
                                }

                                //====================================

                                // likedPages
                                if (result.likedPages.Count > 0)
                                {
                                    var likedPagesList =
                                        new ObservableCollection<Get_Community_Object.LikedPages>(result.likedPages);

                                    //Update All Data To Database  
                                    if (UserID == UserDetails.User_id)
                                    {
                                        var list = likedPagesList.Select(page => new Get_Community_Object.Page
                                        {
                                            PageId = page.PageId,
                                            UserId = page.UserId,
                                            PageName = page.PageName,
                                            PageTitle = page.PageTitle,
                                            PageDescription = page.PageDescription,
                                            Avatar = page.Avatar,
                                            Cover = page.Cover,
                                            PageCategory = page.PageCategory,
                                            Website = page.Website,
                                            Facebook = page.Facebook,
                                            Google = page.Google,
                                            Vk = page.Vk,
                                            Twitter = page.Twitter,
                                            Linkedin = page.Linkedin,
                                            Company = page.Company,
                                            Phone = page.Phone,
                                            Address = page.Address,
                                            CallActionType = page.CallActionType,
                                            CallActionTypeUrl = page.CallActionTypeUrl,
                                            BackgroundImage = page.BackgroundImage,
                                            BackgroundImageStatus = page.BackgroundImageStatus,
                                            Instgram = page.Instgram,
                                            Youtube = page.Youtube,
                                            Verified = page.Verified,
                                            Registered = page.Registered,
                                            Boosted = page.Boosted,
                                            About = page.About,
                                            Id = page.Id,
                                            Type = page.Type,
                                            Url = page.Url,
                                            Name = page.Name,
                                            //Rating = page.Rating,
                                            Category = page.Category,
                                            IsPageOnwer = page.IsPageOnwer,
                                            Username = page.Username
                                        }).ToList();

                                        dbDatabase.Insert_Or_Replace_PagesTable(
                                            new ObservableCollection<Get_Community_Object.Page>(list));
                                    }
                                }
                            }

                            dbDatabase.Dispose();
                        }
                    }
                    else if (Api_status == 400)
                    {
                        if (Respond is Error_Object error)
                        {
                            var errorText = error._errors.Error_text;
                            //Toast.MakeText(this, errortext, ToastLength.Short).Show();

                            if (errorText.Contains("Invalid or expired access_token"))
                                API_Request.Logout(this);
                        }
                    }
                    else if (Api_status == 404)
                    {
                        var error = Respond.ToString();
                        //Toast.MakeText(this, error, ToastLength.Short).Show();
                    }
                }

                //Show Empty Page >> 
                //===============================================================
                if (GroupsAdapter.mGroupsList.Count > 0)
                {
                    GroupsSection.Visibility = ViewStates.Visible;
                    GroupsRecylerView.Visibility = ViewStates.Visible;
                }
                else
                {
                    GroupsSection.Visibility = ViewStates.Gone;
                    GroupsRecylerView.Visibility = ViewStates.Gone;
                }

                //if Manage_MyGroups list count == 0 >> show empty page 
                if (ManageGroupsAdapter.mUserGroupsList.Count > 0)
                {
                    Txt_Count_ManageGroups.Text = ManageGroupsAdapter.mUserGroupsList.Count.ToString();

                    ManageGroupsSection.Visibility = ViewStates.Visible;
                    ManageGroupsRecylerView.Visibility = ViewStates.Visible;
                }
                else
                {
                    Txt_Count_ManageGroups.Text = "0";

                    ManageGroupsSection.Visibility = ViewStates.Gone;
                    ManageGroupsRecylerView.Visibility = ViewStates.Gone;
                }

                if (GroupsAdapter.mGroupsList.Count == 0 && ManageGroupsAdapter.mUserGroupsList.Count == 0)
                {
                    Groups_Empty.Visibility = ViewStates.Visible;

                    ManageGroupsSection.Visibility = ViewStates.Gone;
                    ManageGroupsRecylerView.Visibility = ViewStates.Gone;

                    GroupsSection.Visibility = ViewStates.Gone;
                    GroupsRecylerView.Visibility = ViewStates.Gone;
                }
                else
                {
                    Groups_Empty.Visibility = ViewStates.Gone;
                }

                swipeRefreshLayout.Refreshing = false;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_CommunitiesList_Group_API();
            }
        }

        //Event User Manage Groups => Using Get_User_Data_Object.Joined_Groups => Open Group_ProfileActivity
        private void UserManageGroupsAdapter_OnItemClick(object sender,
            UserGroupsAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = ManageGroupsAdapter.GetItem(position);
                    if (item != null)
                    {
                        var Int = new Intent(this, typeof(Group_Profile_Activity));
                        Int.PutExtra("UserGroups", JsonConvert.SerializeObject(item));
                        Int.PutExtra("GroupsType", "Joined_UserGroups");
                        StartActivity(Int);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Joined Groups => Using Get_Community_Object.Group => Open Group_ProfileActivity
        private void GroupsAdapter_OnItemClick(object sender, GroupsAdapteClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = GroupsAdapter.GetItem(position);
                    if (item != null)
                    {
                        var Int = new Intent(this, typeof(Group_Profile_Activity));
                        Int.PutExtra("MyGroups", JsonConvert.SerializeObject(item));
                        Int.PutExtra("GroupsType", "Joined_MyGroups");
                        StartActivity(Int);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Open Search And Get Group Random
        private void BtnSearchRandomOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var intent = new Intent(this, typeof(Search_Tabbed_Activity));
                intent.PutExtra("Key", "Random");
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Refresh Data Page
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                ManageGroupsAdapter.Clear();
                GroupsAdapter.Clear();

                Get_CommunitiesList_Group();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
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

        public UserGroupsAdapter ManageGroupsAdapter;
        public GroupsAdapter GroupsAdapter;

        private LinearLayout GroupsSection;
        private LinearLayout ManageGroupsSection;
        private LinearLayout Groups_Empty;

        public RecyclerView GroupsRecylerView;
        public RecyclerView ManageGroupsRecylerView;

        public SwipeRefreshLayout swipeRefreshLayout;

        private TextView Txt_Create;

        private TextView Txt_Count_ManageGroups;
        private TextView IconMore_ManageGroups;

        private TextView IconGroups_Empty;

        private Button Btn_SearchRandom;

        private string GroupsManage_Type = "";
        private string UserID = "";

        #endregion

        #region Menu

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

        //Event Create New Group
        public void CreateButton_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var intent = new Intent(this, typeof(CreateGroup_Activity));
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion
    }
}