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
using WoWonder.Activities.Communities.Pages.Adapters;
using WoWonder.Activities.Search;
using WoWonder.Activities.UserProfile.Adapters;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Group;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.Communities.Pages
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Pages_Activity : AppCompatActivity
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

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Pages_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Pages_Layout);

                var groupsType = Intent.GetStringExtra("PagesType") ?? "Data not available";
                if (groupsType != "Data not available" && !string.IsNullOrEmpty(groupsType))
                    PagesManage_Type = groupsType;

                var dataUser = Intent.GetStringExtra("UserID") ?? "Data not available";
                if (dataUser != "Data not available" && !string.IsNullOrEmpty(groupsType)) UserID = dataUser;

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_ExplorePage);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                PagesSection = FindViewById<LinearLayout>(Resource.Id.pageLiner);
                ManagePagesSection = FindViewById<LinearLayout>(Resource.Id.ManagepageLiner);

                ManagePagesRecylerView = (RecyclerView) FindViewById(Resource.Id.pagesRecyler);
                LikedPagesRecylerView = (RecyclerView) FindViewById(Resource.Id.Recyler);

                Page_Empty = FindViewById<LinearLayout>(Resource.Id.Page_LinerEmpty);

                IconPage_Empty = (TextView) FindViewById(Resource.Id.Page_icon);

                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = true;

                Txt_Count_ManagePages = (TextView) FindViewById(Resource.Id.tv_pagescount);
                IconMore_ManagePages = (TextView) FindViewById(Resource.Id.iv_more_pages);
                Txt_Count_ManagePages.Visibility = ViewStates.Gone;
                IconMore_ManagePages.Visibility = ViewStates.Gone;

                Btn_SearchRandom = FindViewById<Button>(Resource.Id.SearchRandom_Button);

                Txt_Create = FindViewById<TextView>(Resource.Id.toolbar_title);

                IMethods.Set_TextViewIcon("1", IconMore_ManagePages, IonIcons_Fonts.ChevronRight);
                IMethods.Set_TextViewIcon("1", IconPage_Empty, IonIcons_Fonts.Flag);

                Page_Empty.Visibility = ViewStates.Gone;
                //####################################

                LikedPagesRecylerView.SetLayoutManager(new LinearLayoutManager(this));
                PageAdapter = new PageAdapter(this);
                PageAdapter.mPageList = new ObservableCollection<Get_Community_Object.Page>();
                LikedPagesRecylerView.SetAdapter(PageAdapter);
                LikedPagesRecylerView.NestedScrollingEnabled = false;

                //####################################

                ManagePagesRecylerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal,
                    false));
                ManagePagesAdapter = new UserPagesAdapter(this);
                ManagePagesRecylerView.SetAdapter(ManagePagesAdapter);
                ManagePagesRecylerView.NestedScrollingEnabled = false;

                //Get Manage my or user pages 
                //When you have finished fetching the Manage pages, the second connection is initiated by fetching Get_CommunitiesList_Page()
                Get_ManagePages();

                //#################################

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
                Btn_SearchRandom.Click += BtnSearchRandomOnClick;
                Txt_Create.Click += CreateButton_OnClick;
                PageAdapter.ItemClick += PageAdapter_OnItemClick;
                ManagePagesAdapter.ItemClick += ManagePagesAdapter_OnItemClick;
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
                Btn_SearchRandom.Click -= BtnSearchRandomOnClick;
                Txt_Create.Click -= CreateButton_OnClick;
                PageAdapter.ItemClick -= PageAdapter_OnItemClick;
                ManagePagesAdapter.ItemClick -= ManagePagesAdapter_OnItemClick;
                swipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #region ManagePages

        public void Get_ManagePages()
        {
            try
            {
                if (PagesManage_Type == "Manage_UserPages")
                {
                    ManagePagesSection.Visibility = ViewStates.Gone;
                    ManagePagesRecylerView.Visibility = ViewStates.Gone;
                    Txt_Create.Visibility = ViewStates.Gone;
                    Btn_SearchRandom.Visibility = ViewStates.Gone;
                }
                else if (PagesManage_Type == "Manage_MyPages")
                {
                    //Get Manage Pages From Database 
                    var dbDatabase = new SqLiteDatabase();
                    var localList = dbDatabase.GetAll_ManagePages();
                    if (localList != null)
                    {
                        ManagePagesAdapter.mAllUserPagesList =
                            new ObservableCollection<Get_User_Data_Object.Liked_Pages>(localList);
                        ManagePagesAdapter.BindEnd();
                    }

                    dbDatabase.Dispose();
                }


                Get_CommunitiesList_Page();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion

        // Get List Page Using Database
        public void Get_CommunitiesList_Page()
        {
            try
            {
                if (UserID == UserDetails.User_id)
                {
                    //Get Pages From Database 
                    var dbDatabase = new SqLiteDatabase();
                    var localList = dbDatabase.Get_Pages();
                    if (localList != null)
                    {
                        PageAdapter.mPageList = new ObservableCollection<Get_Community_Object.Page>(localList);
                        PageAdapter.BindEnd();
                    }

                    dbDatabase.Dispose();
                }

                //Get Pages From API 
                Get_CommunitiesList_Page_API();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        // Get List Page Using API
        public async void Get_CommunitiesList_Page_API()
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
                    Settings st = new Settings();
                    var (Api_status, Respond) = await Client.Global.Get_Community(st,UserID);
                    if (Api_status == 200)
                    {
                        if (Respond is Get_Community_Object result)
                        {
                            if (result.Groups.Count <= 0 && result.likedPages.Count <= 0 && result.Pages.Count <= 0)
                                swipeRefreshLayout.Refreshing = false;

                            var dbDatabase = new SqLiteDatabase();

                            //Add Data
                            //=======================================
                            if (PagesManage_Type == "Manage_UserPages")
                            {
                                // pages
                                if (result.Pages.Count > 0)
                                    PageAdapter.mPageList =
                                        new ObservableCollection<Get_Community_Object.Page>(result.Pages);


                                if (result.likedPages.Count > 0)
                                {
                                    var likedPagesList =
                                        new ObservableCollection<Get_Community_Object.LikedPages>(result.likedPages);
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

                                    //Bring new item
                                    var listnew = list?.Where(c =>
                                        !PageAdapter.mPageList.Select(fc => fc.PageId).Contains(c.PageId)).ToList();
                                    if (listnew.Count > 0)
                                    {
                                        var lastCountItem = PageAdapter.ItemCount;
                                        //Results differ
                                        Classes.AddRange(PageAdapter.mPageList, listnew);
                                    }
                                }

                                PageAdapter.BindEnd();
                            }
                            else if (PagesManage_Type == "Manage_MyPages")
                            {
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

                                        // pages
                                        if (PageAdapter.mPageList.Count > 0)
                                        {
                                            //Bring new pages
                                            var listNew = list.Where(c =>
                                                    !PageAdapter.mPageList.Select(fc => fc.PageId).Contains(c.PageId))
                                                .ToList();
                                            if (listNew.Count > 0)
                                            {
                                                var chkList = listNew.Where(a => a.UserId != UserDetails.User_id)
                                                    .ToList();
                                                if (chkList.Count > 0)
                                                {
                                                    //Results differ
                                                    Classes.AddRange(PageAdapter.mPageList, chkList);
                                                    PageAdapter.BindEnd();

                                                    //Insert Or Replace Just New Data To Database
                                                    if (UserID == UserDetails.User_id)
                                                        dbDatabase.Insert_Or_Replace_PagesTable(
                                                            new ObservableCollection<Get_Community_Object.Page>(
                                                                chkList));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var chkList = list.Where(a => a.UserId != UserDetails.User_id).ToList();
                                            if (chkList.Count > 0)
                                            {
                                                PageAdapter.mPageList =
                                                    new ObservableCollection<Get_Community_Object.Page>(chkList);
                                                PageAdapter.BindEnd();

                                                //Insert Or Replace Data To Database
                                                if (UserID == UserDetails.User_id)
                                                    dbDatabase.Insert_Or_Replace_PagesTable(PageAdapter.mPageList);
                                            }
                                        }

                                        dbDatabase.Insert_Or_Replace_PagesTable(
                                            new ObservableCollection<Get_Community_Object.Page>(list));
                                    }
                                }

                                //====================================

                                if (result.Pages.Count > 0)
                                {
                                    // pages
                                    if (PageAdapter.mPageList.Count > 0)
                                    {
                                        //Bring new pages
                                        var listNew = result.Pages.Where(c =>
                                            !PageAdapter.mPageList.Select(fc => fc.PageId).Contains(c.PageId)).ToList();
                                        if (listNew.Count > 0)
                                        {
                                            var chkList = listNew.Where(a => a.UserId != UserDetails.User_id).ToList();
                                            if (chkList.Count > 0)
                                            {
                                                //Results differ
                                                Classes.AddRange(PageAdapter.mPageList, chkList);
                                                PageAdapter.BindEnd();

                                                //Insert Or Replace Just New Data To Database
                                                if (UserID == UserDetails.User_id)
                                                    dbDatabase.Insert_Or_Replace_PagesTable(
                                                        new ObservableCollection<Get_Community_Object.Page>(chkList));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var chkList = result.Pages?.Where(a => a.UserId != UserDetails.User_id)
                                            .ToList();
                                        if (chkList.Count > 0)
                                        {
                                            PageAdapter.mPageList =
                                                new ObservableCollection<Get_Community_Object.Page>(chkList);
                                            PageAdapter.BindEnd();

                                            //Insert Or Replace Data To Database
                                            if (UserID == UserDetails.User_id)
                                                dbDatabase.Insert_Or_Replace_PagesTable(PageAdapter.mPageList);
                                        }
                                    }

                                    //Manage 
                                    if (ManagePagesAdapter.mAllUserPagesList.Count > 0)
                                    {
                                        var chkListManage = result.Pages.Where(a => a.UserId == UserDetails.User_id)
                                            .ToList();
                                        if (chkListManage.Count > 0)
                                        {
                                            //Bring new page
                                            var listNew = chkListManage.Where(c =>
                                                !ManagePagesAdapter.mAllUserPagesList.Select(fc => fc.page_id)
                                                    .Contains(c.PageId)).ToList();
                                            if (listNew.Count > 0)
                                            {
                                                var list = listNew.Select(page => new Get_User_Data_Object.Liked_Pages
                                                {
                                                    page_id = page.PageId,
                                                    user_id = page.UserId,
                                                    page_name = page.PageName,
                                                    page_title = page.PageTitle,
                                                    page_description = page.PageDescription,
                                                    avatar = page.Avatar,
                                                    cover = page.Cover,
                                                    page_category = page.PageCategory,
                                                    website = page.Website,
                                                    facebook = page.Facebook,
                                                    google = page.Google,
                                                    vk = page.Vk,
                                                    twitter = page.Twitter,
                                                    linkedin = page.Linkedin,
                                                    company = page.Company,
                                                    phone = page.Phone,
                                                    address = page.Address,
                                                    call_action_type = page.CallActionType,
                                                    call_action_type_url = page.CallActionTypeUrl,
                                                    background_image = page.BackgroundImage,
                                                    background_image_status = page.BackgroundImageStatus,
                                                    instgram = page.Instgram,
                                                    youtube = page.Youtube,
                                                    verified = page.Verified,
                                                    registered = page.Registered,
                                                    boosted = page.Boosted,
                                                    about = page.About,
                                                    id = page.Id,
                                                    type = page.Type,
                                                    url = page.Url,
                                                    name = page.Name,
                                                    //rating = page.Rating,
                                                    category = page.Category,
                                                    is_page_onwer = page.IsPageOnwer,
                                                    username = page.Username
                                                }).ToList();

                                                //Results differ
                                                Classes.AddRange(ManagePagesAdapter.mAllUserPagesList, list);
                                                ManagePagesAdapter.BindEnd();

                                                //Insert Or Replace Just New Data To Database
                                                if (UserID == UserDetails.User_id)
                                                    dbDatabase.InsertOrReplace_ManagePagesTable(
                                                        new ObservableCollection<Get_User_Data_Object.Liked_Pages>(
                                                            list));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var chkListManage = result.Pages.Where(a => a.UserId == UserDetails.User_id)
                                            .ToList();
                                        if (chkListManage.Count > 0)
                                        {
                                            var list = chkListManage.Select(page => new Get_User_Data_Object.Liked_Pages
                                            {
                                                page_id = page.PageId,
                                                user_id = page.UserId,
                                                page_name = page.PageName,
                                                page_title = page.PageTitle,
                                                page_description = page.PageDescription,
                                                avatar = page.Avatar,
                                                cover = page.Cover,
                                                page_category = page.PageCategory,
                                                website = page.Website,
                                                facebook = page.Facebook,
                                                google = page.Google,
                                                vk = page.Vk,
                                                twitter = page.Twitter,
                                                linkedin = page.Linkedin,
                                                company = page.Company,
                                                phone = page.Phone,
                                                address = page.Address,
                                                call_action_type = page.CallActionType,
                                                call_action_type_url = page.CallActionTypeUrl,
                                                background_image = page.BackgroundImage,
                                                background_image_status = page.BackgroundImageStatus,
                                                instgram = page.Instgram,
                                                youtube = page.Youtube,
                                                verified = page.Verified,
                                                registered = page.Registered,
                                                boosted = page.Boosted,
                                                about = page.About,
                                                id = page.Id,
                                                type = page.Type,
                                                url = page.Url,
                                                name = page.Name,
                                                //rating = page.Rating,
                                                category = page.Category,
                                                is_page_onwer = page.IsPageOnwer,
                                                username = page.Username
                                            }).ToList();

                                            ManagePagesAdapter.mAllUserPagesList =
                                                new ObservableCollection<Get_User_Data_Object.Liked_Pages>(list);
                                            ManagePagesAdapter.BindEnd();

                                            //Insert Or Replace Just New Data To Database
                                            if (UserID == UserDetails.User_id)
                                                dbDatabase.InsertOrReplace_ManagePagesTable(
                                                    new ObservableCollection<Get_User_Data_Object.Liked_Pages>(list));
                                        }
                                    }
                                }

                                //====================================

                                // groups
                                if (result.Groups.Count > 0)
                                {
                                    var groupsList =
                                        new ObservableCollection<Get_Community_Object.Group>(result.Groups);

                                    //Update All Data To Database
                                    if (UserID == UserDetails.User_id)
                                        dbDatabase.Insert_Or_Replace_GroupsTable(groupsList);
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

                if (PagesManage_Type == "Manage_UserPages")
                {
                    if (PageAdapter.mPageList.Count > 0)
                    {
                        PagesSection.Visibility = ViewStates.Visible;
                        LikedPagesRecylerView.Visibility = ViewStates.Visible;
                        Page_Empty.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        PagesSection.Visibility = ViewStates.Gone;
                        LikedPagesRecylerView.Visibility = ViewStates.Gone;
                        Page_Empty.Visibility = ViewStates.Visible;
                    }
                }
                else if (PagesManage_Type == "Manage_MyPages")
                {
                    //if Manage_MyPages list count == 0 >> show empty page 
                    if (ManagePagesAdapter.mAllUserPagesList.Count > 0)
                    {
                        Txt_Count_ManagePages.Text = ManagePagesAdapter.mAllUserPagesList.Count.ToString();

                        ManagePagesSection.Visibility = ViewStates.Visible;
                        ManagePagesRecylerView.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        Txt_Count_ManagePages.Text = "0";

                        ManagePagesSection.Visibility = ViewStates.Gone;
                        ManagePagesRecylerView.Visibility = ViewStates.Gone;
                    }

                    if (PageAdapter.mPageList.Count > 0)
                    {
                        PagesSection.Visibility = ViewStates.Visible;
                        LikedPagesRecylerView.Visibility = ViewStates.Visible;
                        Page_Empty.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        PagesSection.Visibility = ViewStates.Gone;
                        LikedPagesRecylerView.Visibility = ViewStates.Gone;
                        Page_Empty.Visibility = ViewStates.Visible;
                    }

                    if (PageAdapter.mPageList.Count == 0 && ManagePagesAdapter.mAllUserPagesList.Count == 0)
                    {
                        Page_Empty.Visibility = ViewStates.Visible;

                        ManagePagesSection.Visibility = ViewStates.Gone;
                        ManagePagesRecylerView.Visibility = ViewStates.Gone;

                        PagesSection.Visibility = ViewStates.Gone;
                        LikedPagesRecylerView.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        Page_Empty.Visibility = ViewStates.Gone;
                    }
                }

                swipeRefreshLayout.Refreshing = false;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_CommunitiesList_Page_API();
            }
        }

        // Event User Manage Pages => Using Get_User_Data_Object.Liked_Pages => Open Page_ProfileActivity
        private void ManagePagesAdapter_OnItemClick(object sender, UserPagesAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = ManagePagesAdapter.GetItem(position);
                    if (item != null)
                    {
                        var Int = new Intent(this, typeof(Page_ProfileActivity));
                        Int.PutExtra("UserPages", JsonConvert.SerializeObject(item));
                        Int.PutExtra("PagesType", "Liked_UserPages");
                        StartActivity(Int);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        // Event Liked Pages => Using Get_Community_Object.Page => Open Page_ProfileActivity
        private void PageAdapter_OnItemClick(object sender, PageAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = PageAdapter.GetItem(position);
                    if (item != null)
                    {
                        var Int = new Intent(this, typeof(Page_ProfileActivity));
                        Int.PutExtra("MyPages", JsonConvert.SerializeObject(item));
                        Int.PutExtra("PagesType", "Liked_MyPages");
                        StartActivity(Int);
                    }
                }
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
                PageAdapter.Clear();
                ManagePagesAdapter.Clear();
                Get_CommunitiesList_Page();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Open Search And Get Page Random
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

        public UserPagesAdapter ManagePagesAdapter;
        public PageAdapter PageAdapter;

        private LinearLayout ManagePagesSection;
        private LinearLayout PagesSection;
        private LinearLayout Page_Empty;

        public RecyclerView ManagePagesRecylerView;
        public RecyclerView LikedPagesRecylerView;
        public SwipeRefreshLayout swipeRefreshLayout;

        private TextView Txt_Count_ManagePages;
        private TextView IconMore_ManagePages;

        private TextView Txt_Create;

        private TextView IconPage_Empty;
        private Button Btn_SearchRandom;

        private string PagesManage_Type = "";
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

        //Event Create New Page
        public void CreateButton_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var intent = new Intent(this, typeof(CreatePage_Activity));
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