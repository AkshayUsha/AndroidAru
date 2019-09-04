using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidHUD;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.Tabbes;
using WoWonder.Activities.Tabbes.Adapters;
using WoWonder.Activities.UsersPages;
using WoWonder.Adapters;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.Search
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Search_Tabbed_Activity : AppCompatActivity
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
                    Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
                    Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                }

                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);


                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Search_Tabbed_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Search_Tabbed_Layout);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.Searchtoolbar);
                if (ToolBar != null)
                {
                    SetSupportActionBar(ToolBar);

                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayShowTitleEnabled(false);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }


                tab_Layout = FindViewById<TabLayout>(Resource.Id.Searchtabs);
                viewPager = FindViewById<ViewPager>(Resource.Id.Searchviewpager);

                viewPager.OffscreenPageLimit = 3;
                SetUpViewPager(viewPager);
                tab_Layout.SetupWithViewPager(viewPager);

                appBarLayout = FindViewById<AppBarLayout>(Resource.Id.mainAppBarLayout);
                appBarLayout.SetExpanded(true);

                HashRecylerView = FindViewById<RecyclerView>(Resource.Id.HashRecyler);

                if (Settings.Show_Trending_Hashtags)
                {
                    if (Notifications_Fragment.HashtagUserAdapter != null)
                    {
                        HashRecylerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal,
                            false));
                        HashRecylerView.SetAdapter(Notifications_Fragment.HashtagUserAdapter);
                        Notifications_Fragment.HashtagUserAdapter.ItemClick += HashtagUserAdapter_OnItemClick;
                        HashRecylerView.SetAdapter(Notifications_Fragment.HashtagUserAdapter);
                        Notifications_Fragment.HashtagUserAdapter.BindEnd();

                        HashRecylerView.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        HashRecylerView.Visibility = ViewStates.Invisible;
                    }
                }
                else
                {
                    HashRecylerView.Visibility = ViewStates.Invisible;
                }

                FloatingActionButtonView = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonView);

                var data = Intent.GetStringExtra("Key") ?? "Data not available";
                if (data != "Data not available" && !string.IsNullOrEmpty(data))
                {
                    if (SearchUser_Tab.mSearchAdapter.mSearchUserList.Count > 0)
                        SearchUser_Tab.mSearchAdapter.Clear();

                    if (SearchPages_Tab.mSaerchPageAdapter.mSearchPageList.Count > 0)
                        SearchPages_Tab.mSaerchPageAdapter.Clear();

                    if (SearchGroups_Tab.mSaerchGroupAdapter.mSearchGroupList.Count > 0)
                        SearchGroups_Tab.mSaerchGroupAdapter.Clear();

                    showData_Page = true;
                    showData_Group = true;

                    if (search_key == "Random")
                    {
                        search_key = "a";
                        GetSearch_Result("a");
                    }
                    else
                    {
                        search_key = data;
                        if (_SearchView != null)
                        {
                            _SearchView.SetQuery(search_key, false);
                            _SearchView.ClearFocus();
                            _SearchView.OnActionViewCollapsed();
                        }

                        GetSearch_Result(search_key);
                    }
                } 
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        protected override void OnResume()
        {
            try
            {
                if (_SearchView != null)
                {
                    _SearchView.SetQuery("", false);
                    _SearchView.ClearFocus();
                    _SearchView.OnActionViewCollapsed();
                }

                base.OnResume();
                //Add Event
                FloatingActionButtonView.Click += Filter_OnClick;

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
                FloatingActionButtonView.Click -= Filter_OnClick;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Get Search Api
        public async void GetSearch_Result(string key, string user_offset = "", string group_offset = "",
            string page_offset = "")
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
                    RunOnUiThread(() =>
                    {
                        if (user_offset == "" && group_offset == "" && page_offset == "")
                            AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));
                    });

                    var (api_status, respond) = await Client.Global.Get_Search(UserDetails.User_id, key, "35",
                        user_offset, group_offset, page_offset, Filter_gender);
                    if (api_status == 200)
                    {
                        if (respond is Get_Search_Object result)
                        {
                            if (result.Users.Count <= 0 && result.Groups.Count <= 0 && result.Pages.Count <= 0)
                                return;

                            RunOnUiThread(() =>
                            {
                                //Add result users
                                //*===========================================*
                                if (result.Users.Count > 0 && result.Users.Count != 0)
                                {
                                    if (SearchUser_Tab.mSearchAdapter.mSearchUserList.Count <= 0)
                                    {
                                        SearchUser_Tab.mSearchAdapter.mSearchUserList =
                                            new ObservableCollection<Get_Search_Object.User>(result.Users);
                                        SearchUser_Tab.mSearchAdapter.BindEnd();
                                    }
                                    else
                                    {
                                        //Bring new item
                                        var listNew = result.Users?.Where(c =>
                                            !SearchUser_Tab.mSearchAdapter.mSearchUserList.Select(fc => fc.UserId)
                                                .Contains(c.UserId)).ToList();
                                        if (listNew.Count > 0)
                                        {
                                            var lastCountItem = SearchUser_Tab.mSearchAdapter.ItemCount;

                                            //Results differ
                                            Classes.AddRange(SearchUser_Tab.mSearchAdapter.mSearchUserList, listNew);
                                            SearchUser_Tab.mSearchAdapter.NotifyItemRangeInserted(lastCountItem,
                                                listNew.Count);
                                        }
                                    }
                                }

                                //Add result pages
                                //*===========================================*
                                if (result.Pages.Count > 0 && result.Pages.Count != 0)
                                {
                                    if (SearchPages_Tab.mSaerchPageAdapter.mSearchPageList.Count <= 0)
                                    {
                                        SearchPages_Tab.mSaerchPageAdapter.mSearchPageList =
                                            new ObservableCollection<Get_Search_Object.Page>(result.Pages);
                                        //SearchPages_Tab.mSaerchPageAdapter.BindEnd();
                                    }
                                    else
                                    {
                                        //Bring new item
                                        var listNew = result.Pages?.Where(c =>
                                            !SearchPages_Tab.mSaerchPageAdapter.mSearchPageList.Select(fc => fc.PageId)
                                                .Contains(c.PageId)).ToList();
                                        if (listNew.Count > 0)
                                        {
                                            var lastCountItem = SearchPages_Tab.mSaerchPageAdapter.ItemCount;

                                            //Results differ
                                            Classes.AddRange(SearchPages_Tab.mSaerchPageAdapter.mSearchPageList,
                                                listNew);
                                            SearchPages_Tab.mSaerchPageAdapter.NotifyItemRangeInserted(lastCountItem,
                                                listNew.Count);
                                        }
                                    }
                                }

                                //Add result groups
                                //*===========================================*
                                if (result.Groups.Count > 0 && result.Groups.Count != 0)
                                {
                                    if (SearchGroups_Tab.mSaerchGroupAdapter.mSearchGroupList.Count <= 0)
                                    {
                                        SearchGroups_Tab.mSaerchGroupAdapter.mSearchGroupList =
                                            new ObservableCollection<Get_Search_Object.Group>(result.Groups);
                                        //SearchGroups_Tab.mSaerchGroupAdapter.BindEnd();
                                    }
                                    else
                                    {
                                        //Bring new item
                                        var listNew = result.Groups?.Where(c =>
                                            !SearchGroups_Tab.mSaerchGroupAdapter.mSearchGroupList
                                                .Select(fc => fc.GroupId).Contains(c.GroupId)).ToList();
                                        if (listNew.Count > 0)
                                        {
                                            var lastCountItem = SearchGroups_Tab.mSaerchGroupAdapter.ItemCount;

                                            //Results differ
                                            Classes.AddRange(SearchGroups_Tab.mSaerchGroupAdapter.mSearchGroupList,
                                                listNew);
                                            SearchGroups_Tab.mSaerchGroupAdapter.NotifyItemRangeInserted(lastCountItem,
                                                listNew.Count);
                                        }
                                    }
                                }
                            });
                        }
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

                    RunOnUiThread(() =>
                    {
                        //Show Empty Page
                        //===========================================
                        if (SearchUser_Tab.mSearchAdapter.mSearchUserList.Count > 0)
                        {
                            SearchUser_Tab.Search_Recyler.Visibility = ViewStates.Visible;
                            SearchUser_Tab.Search_Empty.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            SearchUser_Tab.Search_Recyler.Visibility = ViewStates.Gone;
                            SearchUser_Tab.Search_Empty.Visibility = ViewStates.Visible;
                        }

                        //Set Event Scroll >> Users
                        if (User_OnMainScrolEvent == null)
                        {
                            var xamarinRecyclerViewOnScrollListener =
                                new XamarinRecyclerViewOnScrollListener(SearchUser_Tab.mSearchLayoutManager);
                            User_OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                            User_OnMainScrolEvent.LoadMoreEvent += LastUsers_OnScroll_OnLoadMoreEvent;
                            SearchUser_Tab.Search_Recyler.AddOnScrollListener(User_OnMainScrolEvent);
                            SearchUser_Tab.Search_Recyler.AddOnScrollListener(new ScrollDownDetector());
                        }
                        else
                        {
                            User_OnMainScrolEvent.IsLoading = false;
                        }

                        //Show Empty Page
                        //===========================================
                        if (SearchGroups_Tab.mSaerchGroupAdapter.mSearchGroupList.Count > 0)
                        {
                            SearchGroups_Tab.SearchGroup_Recyler.Visibility = ViewStates.Visible;
                            SearchGroups_Tab.SearchGroup_Empty.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            SearchGroups_Tab.SearchGroup_Recyler.Visibility = ViewStates.Gone;
                            SearchGroups_Tab.SearchGroup_Empty.Visibility = ViewStates.Visible;
                        }

                        //Set Event Scroll >> Groups
                        if (Group_OnMainScrolEvent == null)
                        {
                            var xamarinRecyclerViewOnScrollListener =
                                new XamarinRecyclerViewOnScrollListener(SearchGroups_Tab.mSearchLayoutManager);
                            Group_OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                            Group_OnMainScrolEvent.LoadMoreEvent += LastGroups_OnScroll_OnLoadMoreEvent;
                            SearchGroups_Tab.SearchGroup_Recyler.AddOnScrollListener(Group_OnMainScrolEvent);
                            SearchGroups_Tab.SearchGroup_Recyler.AddOnScrollListener(new ScrollDownDetector());
                        }
                        else
                        {
                            Group_OnMainScrolEvent.IsLoading = false;
                        }

                        //Show Empty Page
                        //===========================================
                        if (SearchPages_Tab.mSaerchPageAdapter.mSearchPageList.Count > 0)
                        {
                            SearchPages_Tab.SearchPage_Recyler.Visibility = ViewStates.Visible;
                            SearchPages_Tab.SearchPage_Empty.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            SearchPages_Tab.SearchPage_Recyler.Visibility = ViewStates.Gone;
                            SearchPages_Tab.SearchPage_Empty.Visibility = ViewStates.Visible;
                        }

                        //Set Event Scroll >> Pages
                        if (Page_OnMainScrolEvent == null)
                        {
                            var xamarinRecyclerViewOnScrollListener =
                                new XamarinRecyclerViewOnScrollListener(SearchPages_Tab.mSearchLayoutManager);
                            Page_OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                            Page_OnMainScrolEvent.LoadMoreEvent += LastPage_OnScroll_OnLoadMoreEvent;
                            SearchPages_Tab.SearchPage_Recyler.AddOnScrollListener(Page_OnMainScrolEvent);
                            SearchPages_Tab.SearchPage_Recyler.AddOnScrollListener(new ScrollDownDetector());
                        }
                        else
                        {
                            Page_OnMainScrolEvent.IsLoading = false;
                        }

                        _SearchView.ClearFocus();
                        AndHUD.Shared.Dismiss(this);
                    });
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                GetSearch_Result(search_key, LastUserid, LastGroupid, LastPageid);
            }
        }

        //Event Hash tag
        private void HashtagUserAdapter_OnItemClick(object sender, HashtagUser_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = Notifications_Fragment.HashtagUserAdapter.GetItem(position);
                    if (item != null)
                    {
                        var id = item.hash.Replace("#", "").Replace("_", " ");
                        var tag = item.tag.Replace("#", "");
                        var Int = new Intent(this, typeof(HyberdPostViewer_Activity));
                        Int.PutExtra("Id", tag);
                        Int.PutExtra("Type", "Hashtag");
                        Int.PutExtra("Title", tag);
                        StartActivity(Int);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Set Filter
        private void Filter_OnClick(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(this, typeof(SearchFilter_Activity));
                StartActivityForResult(intent, 60000);
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
                if (requestCode == 60000 && resultCode == Result.Ok)
                {
                    var gender = data.GetStringExtra("gender");
                    if (gender != "Data not available" && !string.IsNullOrEmpty(gender))
                        Filter_gender = gender;

                    var image = data.GetStringExtra("profilePicture");
                    if (image != "Data not available" && !string.IsNullOrEmpty(image))
                        Filter_image = image;

                    var status = data.GetStringExtra("status");
                    if (status != "Data not available" && !string.IsNullOrEmpty(status))
                        Filter_status = status;

                    // GetSearch_Result(search_key);
                }

                base.OnActivityResult(requestCode, resultCode, data);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

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

        private static float PERCENTAGE_TO_SHOW_TITLE_AT_TOOLBAR = 0.9f;
        private static float PERCENTAGE_TO_HIDE_TITLE_DETAILS = 0.3f;
        private static int ALPHA_ANIMATIONS_DURATION = 200;

        public static AppBarLayout appBarLayout;
        private TabLayout tab_Layout;
        private ViewPager viewPager;

        private SearchView _SearchView;
        private Toolbar actionBarToolBar;

        private RecyclerView HashRecylerView;

        private string search_key = "";
        private string Filter_gender = "";
        private string Filter_image = "";
        private string Filter_status = "";

        public SearchUser_Fragment SearchUser_Tab;
        public SearchPages_Fragment SearchPages_Tab;
        public SearchGroups_Fragment SearchGroups_Tab;

        private FloatingActionButton FloatingActionButtonView;

        private string LastUserid = "";
        private string LastPageid = "";
        private string LastGroupid = "";

        public XamarinRecyclerViewOnScrollListener User_OnMainScrolEvent;
        public XamarinRecyclerViewOnScrollListener Page_OnMainScrolEvent;
        public XamarinRecyclerViewOnScrollListener Group_OnMainScrolEvent;

        private bool showData_Page = true;
        private bool showData_Group = true;

        #endregion

        #region Set Tab 

        private void SetUpViewPager(ViewPager viewPager)
        {
            try
            {
                SearchUser_Tab = new SearchUser_Fragment();
                SearchPages_Tab = new SearchPages_Fragment();
                SearchGroups_Tab = new SearchGroups_Fragment();

                var adapter = new Main_Tab_Adapter(SupportFragmentManager);
                adapter.AddFragment(SearchUser_Tab, GetText(Resource.String.Lbl_Users));
                adapter.AddFragment(SearchPages_Tab, GetText(Resource.String.Lbl_Pages));
                adapter.AddFragment(SearchGroups_Tab, GetText(Resource.String.Lbl_Groups));

                viewPager.PageSelected += ViewPagerOnPageSelected;
                viewPager.Adapter = adapter;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        private void ViewPagerOnPageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            try
            {
                var p = e.Position;
                if (p == 0) //users
                {
                }
                else if (p == 1) // Pages
                {
                    if (showData_Page)
                    {
                        showData_Page = false;
                        SearchPages_Tab.mSaerchPageAdapter.BindEnd();
                    }
                }
                else if (p == 2) // Groups
                {
                    if (showData_Group)
                    {
                        showData_Group = false;
                        SearchGroups_Tab.mSaerchGroupAdapter.BindEnd();
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #endregion

        #region Menu

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Search_Menu, menu);

            var item = menu.FindItem(Resource.Id.searchUserBar);
            var searchItem = MenuItemCompat.GetActionView(item);
            _SearchView = searchItem.JavaCast<SearchView>();
            _SearchView.SetQuery("", false);
            _SearchView.ClearFocus();
            _SearchView.OnActionViewExpanded();

            _SearchView.SetIconifiedByDefault(true);
            _SearchView.OnActionViewExpanded();

            _SearchView.QueryTextChange += SearchViewOnQueryTextChange;
            _SearchView.QueryTextSubmit += SearchViewOnQueryTextSubmit;

            return base.OnCreateOptionsMenu(menu);
        }

        public void SearchViewOnQueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            try
            {
                search_key = e.Query;

                if (SearchUser_Tab.mSearchAdapter.mSearchUserList.Count > 0)
                    SearchUser_Tab.mSearchAdapter.Clear();

                if (SearchPages_Tab.mSaerchPageAdapter.mSearchPageList.Count > 0)
                    SearchPages_Tab.mSaerchPageAdapter.Clear();

                if (SearchGroups_Tab.mSaerchGroupAdapter.mSearchGroupList.Count > 0)
                    SearchGroups_Tab.mSaerchGroupAdapter.Clear();

                showData_Page = true;
                showData_Group = true;

                GetSearch_Result(search_key);

                //Hide keyboard programmatically in MonoDroid
                e.Handled = true;

                _SearchView.ClearFocus();

                var inputManager = (InputMethodManager) GetSystemService(InputMethodService);
                inputManager.HideSoftInputFromWindow(actionBarToolBar.WindowToken, 0);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void SearchViewOnQueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            try
            {
                search_key = e.NewText;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #endregion

        #region Scroll

        //Event Scroll #LastUsers
        private void LastUsers_OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = SearchUser_Tab.mSearchAdapter.mSearchUserList.LastOrDefault();
                if (item != null) LastUserid = item.UserId;

                if (LastUserid != "")
                    try
                    {
                        //Run Load More Api 
                        Task.Run(() => { GetSearch_Result(search_key, LastUserid); });
                    }
                    catch (Exception exception)
                    {
                        Crashes.TrackError(exception);
                    }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Scroll #LastPage
        private void LastPage_OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = SearchPages_Tab.mSaerchPageAdapter.mSearchPageList.LastOrDefault();
                if (item != null) LastPageid = item.PageId;

                if (LastPageid != "")
                    try
                    {
                        //Run Load More Api 
                        Task.Run(() => { GetSearch_Result(search_key, "", "", LastPageid); });
                    }
                    catch (Exception exception)
                    {
                        Crashes.TrackError(exception);
                    }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Scroll #LastGroups
        private void LastGroups_OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = SearchGroups_Tab.mSaerchGroupAdapter.mSearchGroupList.LastOrDefault();
                if (item != null) LastGroupid = item.GroupId;

                if (LastGroupid != "")
                    try
                    {
                        //Run Load More Api 
                        Task.Run(() => { GetSearch_Result(search_key, "", LastGroupid); });
                    }
                    catch (Exception exception)
                    {
                        Crashes.TrackError(exception);
                    }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public class XamarinRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
        {
            public delegate void LoadMoreEventHandler(object sender, EventArgs e);

            public bool IsLoading;
            public LinearLayoutManager LayoutManager;

            public XamarinRecyclerViewOnScrollListener(LinearLayoutManager layoutManager)
            {
                try
                {
                    LayoutManager = layoutManager;
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                }
            }

            public event LoadMoreEventHandler LoadMoreEvent;

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                try
                {
                    base.OnScrolled(recyclerView, dx, dy);

                    var visibleItemCount = recyclerView.ChildCount;
                    var totalItemCount = recyclerView.GetAdapter().ItemCount;

                    var pastVisiblesItems = LayoutManager.FindFirstVisibleItemPosition();
                    if (visibleItemCount + pastVisiblesItems + 8 >= totalItemCount)
                        if (IsLoading == false)
                        {
                            LoadMoreEvent?.Invoke(this, null);
                            IsLoading = true;
                        }
                }
                catch (Exception exception)
                {
                    Crashes.TrackError(exception);
                }
            }
        }

        public class ScrollDownDetector : RecyclerView.OnScrollListener
        {
            public Action Action;
            private bool readyForAction;

            public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
            {
                try
                {
                    base.OnScrollStateChanged(recyclerView, newState);

                    if (newState == RecyclerView.ScrollStateDragging)
                    {
                        //The user starts scrolling
                        readyForAction = true;

                        if (recyclerView.ScrollState == (int) ScrollState.Fling)
                            ImageService.Instance
                                .SetPauseWork(true); // all image loading requests will be silently canceled
                        else if (recyclerView.ScrollState == (int) ScrollState.Fling)
                            ImageService.Instance.SetPauseWork(false);
                    }
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                }
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                try
                {
                    base.OnScrolled(recyclerView, dx, dy);

                    if (readyForAction && dy > 0)
                    {
                        //The scroll direction is down
                        readyForAction = false;
                        Action();
                    }
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                }
            }
        }

        #endregion
    }
}