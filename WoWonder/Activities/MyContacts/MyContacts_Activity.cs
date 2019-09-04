using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.BlockedUsers;
using WoWonder.Activities.InviteFriends;
using WoWonder.Activities.MyContacts.Adapters;
using WoWonder.Activities.MyProfile;
using WoWonder.Activities.MyProfile.Adapters;
using WoWonder.Activities.Search;
using WoWonder.Activities.UserProfile;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.MyContacts
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class MyContacts_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Contacts_Main_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Contacts_Main_Layout);

                ContactsRecyler = FindViewById<RecyclerView>(Resource.Id.Recyler);
                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                Contacts_Empty = FindViewById<LinearLayout>(Resource.Id.Contacts_LinerEmpty);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    if (Settings.ConnectivitySystem == "1") // Following
                        ToolBar.Title = GetText(Resource.String.Lbl_Following);
                    else // Friend
                        ToolBar.Title = GetText(Resource.String.Lbl_Friends);
                     
                    ToolBar.SetSubtitleTextColor(Color.White);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);

                Icon_UserContacts = FindViewById<TextView>(Resource.Id.Contactsusers_icon);
                Btn_SearchRandom = FindViewById<Button>(Resource.Id.SearchRandom_Button);


                IMethods.Set_TextViewIcon("1", Icon_UserContacts, IonIcons_Fonts.IosPeopleOutline);
                Icon_UserContacts.SetTextColor(Color.ParseColor(Settings.MainColor));

                ContactsLayoutManager = new LinearLayoutManager(this);
                ContactsRecyler.SetLayoutManager(ContactsLayoutManager);


                ContactsRecyler.HasFixedSize = (true);
                ContactsRecyler.SetItemViewCacheSize(10);
                ContactsRecyler.GetLayoutManager().ItemPrefetchEnabled = true;
                ContactsRecyler.DrawingCacheEnabled = (true);
                ContactsRecyler.DrawingCacheQuality = DrawingCacheQuality.High;



                Contacts_Empty.Visibility = ViewStates.Gone;
                ContactsRecyler.Visibility = ViewStates.Visible;

                var ContactsType = Intent.GetStringExtra("ContactsType") ?? "Data not available";
                if (ContactsType != "Data not available" && !string.IsNullOrEmpty(ContactsType))
                    Type_Contacts = ContactsType;

                if (Type_Contacts == "Following")
                    Get_MyContact();
                else
                    Get_MyFollowers();


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
                Btn_SearchRandom.Click += BtnSearchRandomOnClick;
                swipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
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
                Btn_SearchRandom.Click -= BtnSearchRandomOnClick;
                swipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void Get_MyContact(int lastId = 0)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    if (Settings.ConnectivitySystem == "1") // Following
                        SupportActionBar.Title = GetText(Resource.String.Lbl_Following);
                    else // Friend
                        SupportActionBar.Title = GetText(Resource.String.Lbl_Friends);
                });

                //Get All User From Database 
                var dbDatabase = new SqLiteDatabase();
                var localList = dbDatabase.Get_MyContact(lastId, 25);
                if (localList != null)
                {
                    RunOnUiThread(() =>
                    {
                        var list = new JavaList<Classes.UserContacts.User>(localList);
                        if (list.Count > 0)
                        {
                            var listNew = list?.Where(c => !UserContactsList.Select(fc => fc.user_id).Contains(c.user_id)).ToList();
                            if (listNew.Count > 0)
                            {
                                Classes.AddRange(UserContactsList, listNew);

                                var listOrder = new JavaList<Classes.UserContacts.User>(UserContactsList.OrderBy(a => a.name));
                                if (MyContactsAdapter == null)
                                {
                                    //Results differ
                                    MyContactsAdapter = new MyContacts_Adapter(this, listOrder, ContactsRecyler);
                                    ContactsRecyler.SetAdapter(MyContactsAdapter);
                                    MyContactsAdapter.ItemClick += MyContactsAdapter_OnItemClick;

                                    var lastCountItem = MyContactsAdapter.ItemCount;
                                    MyContactsAdapter.NotifyItemRangeInserted(lastCountItem, listNew.Count);
                                }
                                else
                                {
                                    MyContactsAdapter.mMyContactsList = new JavaList<Classes.UserContacts.User>(listOrder);

                                    var lastCountItem = MyContactsAdapter.ItemCount;
                                    MyContactsAdapter.NotifyItemRangeInserted(lastCountItem, listNew.Count);
                                }
                            }
                            else
                            {
                                if (ShowSnackbar)
                                {
                                    Snackbar.Make(ContactsRecyler, GetText(Resource.String.Lbl_Loading_From_Server), Snackbar.LengthLong).Show();
                                    ShowSnackbar = false;
                                }
                                Get_Contacts_APi();
                            }

                            if (swipeRefreshLayout != null)
                                swipeRefreshLayout.Refreshing = false;
                        }
                        else
                        {
                            if (ShowSnackbar)
                            {
                                Snackbar.Make(ContactsRecyler, GetText(Resource.String.Lbl_Loading_From_Server), Snackbar.LengthLong).Show();
                                ShowSnackbar = false;
                            }
                            Get_Contacts_APi();
                        }

                        //Set Event Scroll
                        if (OnMainScrolEvent == null)
                        {
                            var xamarinRecyclerViewOnScrollListener =
                                new XamarinRecyclerViewOnScrollListener(ContactsLayoutManager, swipeRefreshLayout);
                            OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                            OnMainScrolEvent.LoadMoreEvent += MyContact_OnScroll_OnLoadMoreEvent;
                            ContactsRecyler.AddOnScrollListener(OnMainScrolEvent);
                            ContactsRecyler.AddOnScrollListener(new ScrollDownDetector());
                        }
                        else
                        {
                            OnMainScrolEvent.IsLoading = false;
                        }
                    });
                }
                else
                {
                    if (ShowSnackbar)
                    {
                        Snackbar.Make(ContactsRecyler, GetText(Resource.String.Lbl_Loading_From_Server), Snackbar.LengthLong).Show();
                        ShowSnackbar = false;
                    }
                    Get_Contacts_APi();
                }

                dbDatabase.Dispose();

                if (UserContactsList?.Count <= 24 || UserContactsList?.Count == 0)
                {
                    swipeRefreshLayout.Refreshing = true;
                    Get_Contacts_APi();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Api
        public async void Get_Contacts_APi()
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    RunOnUiThread(() => { swipeRefreshLayout.Refreshing = false; });
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)
                        .Show();
                }
                else
                {
                    var lastIdUser = UserContactsList?.LastOrDefault()?.user_id ?? "0";

                    var (api_status, respond) = await API_Request.Get_users_friends_Async(lastIdUser);
                    if (api_status == 200)
                    {
                        if (respond is Classes.UserContacts.Rootobject result)
                            RunOnUiThread(() =>
                            {
                                if (result.users.Length <= 0)
                                {
                                }
                                else if (result.users.Length > 0)
                                {
                                    var listNew = result.users?.Where(c =>!UserContactsList.Select(fc => fc.user_id).Contains(c.user_id)).ToList();
                                    if (listNew.Count > 0)
                                    {
                                        Classes.AddRange(UserContactsList, listNew);

                                        var listOrder =new JavaList<Classes.UserContacts.User>(UserContactsList.OrderBy(a => a.name));
                                        if (MyContactsAdapter == null)
                                        {
                                            //Results differ
                                            MyContactsAdapter = new MyContacts_Adapter(this, listOrder, ContactsRecyler);
                                            ContactsRecyler.SetAdapter(MyContactsAdapter);
                                            MyContactsAdapter.ItemClick += MyContactsAdapter_OnItemClick;

                                            var lastCountItem = MyContactsAdapter.ItemCount;
                                            MyContactsAdapter.NotifyItemRangeInserted(lastCountItem, listNew.Count);
                                        }
                                        else
                                        { 
                                            MyContactsAdapter.mMyContactsList = new JavaList<Classes.UserContacts.User>(listOrder);

                                            var lastCountItem = MyContactsAdapter.ItemCount;
                                            MyContactsAdapter.NotifyItemRangeInserted(lastCountItem, listNew.Count);
                                        }

                                        //Insert Or Update All data UsersContact to database
                                        var dbDatabase = new SqLiteDatabase();
                                        dbDatabase.Insert_Or_Replace_MyContactTable(UserContactsList);
                                        dbDatabase.Dispose();
                                    }
                                    else
                                    {
                                        if (ShowSnackbarNoMore)
                                        {
                                            Snackbar.Make(ContactsRecyler, GetText(Resource.String.Lbl_No_have_more_users), Snackbar.LengthLong).Show();
                                            ShowSnackbarNoMore = false;
                                        }
                                    }

                                    if (swipeRefreshLayout != null)
                                        swipeRefreshLayout.Refreshing = false;
                                }
                            });
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

                //Show Empty Page >> 
                //===============================================================
                RunOnUiThread(() =>
                {
                    if (UserContactsList?.Count > 0)
                    {
                        Contacts_Empty.Visibility = ViewStates.Gone;
                        ContactsRecyler.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        Contacts_Empty.Visibility = ViewStates.Visible;
                        ContactsRecyler.Visibility = ViewStates.Gone;
                    }

                    swipeRefreshLayout.Refreshing = false;

                    //Set Event Scroll
                    if (OnMainScrolEvent == null)
                    {
                        var xamarinRecyclerViewOnScrollListener =
                            new XamarinRecyclerViewOnScrollListener(ContactsLayoutManager, swipeRefreshLayout);
                        OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                        OnMainScrolEvent.LoadMoreEvent += MyContact_OnScroll_OnLoadMoreEvent;
                        ContactsRecyler.AddOnScrollListener(OnMainScrolEvent);
                        ContactsRecyler.AddOnScrollListener(new ScrollDownDetector());
                    }
                    else
                    {
                        OnMainScrolEvent.IsLoading = false;
                    }
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_Contacts_APi();
            }
        }

        public void Get_MyFollowers()
        {
            try
            {
                if (Settings.ConnectivitySystem == "1") // Following
                    SupportActionBar.Title = GetText(Resource.String.Lbl_People_Followers);
                else // Friend
                    SupportActionBar.Title = GetText(Resource.String.Lbl_Friends);
                 
                //Get All User From Database 
                var dbDatabase = new SqLiteDatabase();
                var localList = dbDatabase.Get_MyFollowers();
                if (localList != null)
                {
                    UserFollowersList = new ObservableCollection<Get_User_Data_Object.Followers>(localList);

                    var list = new JavaList<Get_User_Data_Object.Followers>(localList.OrderBy(a => a.name));
                    if (list.Count > 0)
                    {
                        //Set Adapter
                        ContactsLayoutManager = new LinearLayoutManager(this);
                        ContactsRecyler.SetLayoutManager(ContactsLayoutManager);
                        MyFollowersAdapter = new MyFollowers_Adapter(this, list, ContactsRecyler);
                        ContactsRecyler.SetAdapter(MyFollowersAdapter);
                        MyFollowersAdapter.ItemClick += MyFollowersAdapter_OnItemClick;
                        MyFollowersAdapter.BindEnd();
                    }
                }

                //Show Empty Page >> 
                //===============================================================
                if (MyFollowersAdapter.mMyFollowersList.Count > 0)
                {
                    swipeRefreshLayout.Refreshing = false;

                    Contacts_Empty.Visibility = ViewStates.Gone;
                    ContactsRecyler.Visibility = ViewStates.Visible;
                }
                else
                {
                    swipeRefreshLayout.Refreshing = false;

                    Contacts_Empty.Visibility = ViewStates.Visible;
                    ContactsRecyler.Visibility = ViewStates.Gone;
                }

                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void MyFollowersAdapter_OnItemClick(object sender, MyFollowers_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = MyFollowersAdapter.GetItem(position);
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

        private void MyContactsAdapter_OnItemClick(object sender, MyContacts_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = MyContactsAdapter.GetItem(position);
                    if (item != null)
                    {
                        Intent Int;
                        if (item.user_id != UserDetails.User_id)
                        {
                            Int = new Intent(this, typeof(User_Profile_Activity));
                            Int.PutExtra("UserId", item.user_id);
                            Int.PutExtra("UserType", "MyContacts");
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

        //Event Open Search And Get Users Random
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
                if (Type_Contacts == "Following")
                {
                    MyContactsAdapter?.Clear();
                    Get_MyContact();
                }
                else
                {
                    MyFollowersAdapter?.Clear();
                    Get_MyFollowers();
                }

                swipeRefreshLayout.Refreshing = true;
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

        public static MyFollowers_Adapter MyFollowersAdapter;
        public static MyContacts_Adapter MyContactsAdapter;
        private RecyclerView ContactsRecyler;
        public LinearLayoutManager ContactsLayoutManager;
        private SwipeRefreshLayout swipeRefreshLayout;
        private LinearLayout Contacts_Empty;
        private Button Btn_SearchRandom;
        private TextView Icon_UserContacts;

        public SearchView _SearchView;

        private string Type_Contacts = "";

        public ObservableCollection<Classes.UserContacts.User> UserContactsList =
            new ObservableCollection<Classes.UserContacts.User>();

        public ObservableCollection<Get_User_Data_Object.Followers> UserFollowersList =
            new ObservableCollection<Get_User_Data_Object.Followers>();

        public XamarinRecyclerViewOnScrollListener OnMainScrolEvent;

        public bool ShowSnackbar = true;
        public bool ShowSnackbarNoMore = true;

        #endregion

        #region Menu 

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.User_Search_Menu, menu);

            var item = menu.FindItem(Resource.Id.searchUserBar);
            var searchItem = MenuItemCompat.GetActionView(item);

            _SearchView = searchItem.JavaCast<SearchView>();
            _SearchView.SetIconifiedByDefault(true);
            _SearchView.QueryTextChange += _SearchView_OnTextChange;
            _SearchView.QueryTextSubmit += _SearchView_OnTextSubmit;

            var refresh = menu.FindItem(Resource.Id.menue_refresh);
            refresh.SetVisible(false);

            return base.OnCreateOptionsMenu(menu);
        }

        private void _SearchView_OnTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            try
            {
                MyContactsAdapter.Filter.InvokeFilter(e.Query);
                e.Handled = true;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        private void _SearchView_OnTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            try
            {
                MyContactsAdapter.Filter.InvokeFilter(e.NewText);
                ContactsRecyler.Invalidate();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                case Resource.Id.menue_invite_friend:
                    Invite_friend_OnClick();
                    break;

                case Resource.Id.menue_refresh:
                    Refresh_OnClick();
                    break;

                case Resource.Id.menue_blockList:
                    BlockList_OnClick();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void BlockList_OnClick()
        {
            try
            {
                var Intent = new Intent(this, typeof(BlockedUsers_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void Refresh_OnClick()
        {
            try
            {
                if (Type_Contacts == "Following")
                {
                    swipeRefreshLayout.Refreshing = true;

                    Get_MyContact();
                }
                else
                {
                    swipeRefreshLayout.Refreshing = true;

                    Get_MyFollowers();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void Invite_friend_OnClick()
        {
            try
            {
                var Intent = new Intent(this, typeof(Invite_Friends_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion

        #region Scroll

        //Event Scroll #MyContact
        private void MyContact_OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                int afterContact = UserContactsList.Count();
                if (afterContact >= 0)
                    try
                    {
                        //Run Load More Api 
                        Task.Run(() =>
                        {
                            if (Type_Contacts == "Following")
                                Get_MyContact(afterContact);
                            //else
                            //    Get_MyFollowers();
                        });
                    }
                    catch (Exception exception)
                    {
                        Crashes.TrackError(exception);
                    }

                swipeRefreshLayout.Refreshing = false;
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
            public SwipeRefreshLayout SwipeRefreshLayout;

            public XamarinRecyclerViewOnScrollListener(LinearLayoutManager layoutManager,
                SwipeRefreshLayout swipeRefreshLayout)
            {
                try
                {
                    LayoutManager = layoutManager;
                    SwipeRefreshLayout = swipeRefreshLayout;
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