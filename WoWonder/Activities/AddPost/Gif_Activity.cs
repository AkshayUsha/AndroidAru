using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.AddPost.Adapters;
using WoWonder.Helpers;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.AddPost
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Gif_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Gif_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Gif_Layout);

                ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_SelectGif);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                GifRecylerView = FindViewById<RecyclerView>(Resource.Id.Recyler);
                GifAdapter = new GifAdapter(this);
                MLayoutManager = new GridLayoutManager(this, 2);
                GifRecylerView.SetLayoutManager(MLayoutManager);
                GifRecylerView.AddItemDecoration(new GridSpacingItemDecoration(1, 2, true));

                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);

                search_key = "angry";
                Get_GifData_Api(search_key);
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
                GifAdapter.ItemClick += GifAdapterOnItemClick;
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
                GifAdapter.ItemClick -= GifAdapterOnItemClick;
                swipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        public async void Get_GifData_Api(string key, string offset = "")
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
                    if (offset == "") RunOnUiThread(() => { GifAdapter.Clear(); });

                    var result = await API_Request.Search_Gifs_Web(key, offset);
                    if (result.Count > 0)
                        RunOnUiThread(() =>
                        {
                            if (GifAdapter.GifList.Count == 0)
                            {
                                GifAdapter.GifList = new ObservableCollection<GifGiphyClass.Datum>(result);
                                GifRecylerView.SetAdapter(GifAdapter);
                            }
                            else
                            {
                                //Bring new item
                                var listnew = result?.Where(c => !GifAdapter.GifList.Select(fc => fc.id).Contains(c.id))
                                    .ToList();
                                if (listnew.Count > 0)
                                {
                                    var lastCountItem = GifAdapter.ItemCount;

                                    //Results differ
                                    Classes.AddRange(GifAdapter.GifList, listnew);
                                    GifAdapter.NotifyItemRangeInserted(lastCountItem, listnew.Count);
                                }
                            }

                            swipeRefreshLayout.Refreshing = false;

                            //Set Event Scroll
                            if (OnMainScrolEvent == null)
                            {
                                var xamarinRecyclerViewOnScrollListener =
                                    new XamarinRecyclerViewOnScrollListener(MLayoutManager, swipeRefreshLayout);
                                OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                                OnMainScrolEvent.LoadMoreEvent += Gif_OnScroll_OnLoadMoreEvent;
                                GifRecylerView.AddOnScrollListener(OnMainScrolEvent);
                                GifRecylerView.AddOnScrollListener(new ScrollDownDetector());
                            }
                            else
                            {
                                OnMainScrolEvent.IsLoading = false;
                            }
                        });
                }

                if (swipeRefreshLayout.Refreshing)
                    swipeRefreshLayout.Refreshing = false;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_GifData_Api(search_key, offset);
            }
        }

        //Event
        private void GifAdapterOnItemClick(object sender, GifAdapterAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = GifAdapter.GetItem(position);
                    if (item != null)
                    {
                        var resultIntent = new Intent();
                        resultIntent.PutExtra("gif", item.images.preview_gif.url);
                        SetResult(Result.Ok, resultIntent);
                        Finish();
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
                GifAdapter.Clear();
                Get_GifData_Api(search_key);
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

        public GridLayoutManager MLayoutManager;

        public RecyclerView GifRecylerView;
        public GifAdapter GifAdapter;
        public SwipeRefreshLayout swipeRefreshLayout;
        public SearchView _SearchView;
        private Toolbar ToolBar;
        public XamarinRecyclerViewOnScrollListener OnMainScrolEvent;

        private string LastGifid = "";
        private string search_key = "";

        #endregion


        #region Scroll

        //Event Scroll #Gif
        private void Gif_OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = GifAdapter.GifList.LastOrDefault();
                if (item != null) LastGifid = item.id;

                if (LastGifid != "")
                    try
                    {
                        //Run Load More Api 
                        Task.Run(() => { Get_GifData_Api(search_key, LastGifid); });
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
            public GridLayoutManager LayoutManager;
            public SwipeRefreshLayout SwipeRefreshLayout;

            public XamarinRecyclerViewOnScrollListener(GridLayoutManager layoutManager,
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

        #region Menu 

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SearchGif_Menu, menu);

            var item = menu.FindItem(Resource.Id.searchUserBar);
            var searchItem = MenuItemCompat.GetActionView(item);

            _SearchView = searchItem.JavaCast<SearchView>();
            _SearchView.SetIconifiedByDefault(true);
            _SearchView.QueryTextChange += _SearchView_OnTextChange;
            _SearchView.QueryTextSubmit += _SearchView_OnTextSubmit;

            return base.OnCreateOptionsMenu(menu);
        }

        private void _SearchView_OnTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            try
            {
                search_key = e.Query;
                Get_GifData_Api(search_key);

                //Hide keyboard programmatically in MonoDroid
                e.Handled = true;

                _SearchView.ClearFocus();

                var inputManager = (InputMethodManager) GetSystemService(InputMethodService);
                inputManager.HideSoftInputFromWindow(ToolBar.WindowToken, 0);
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
                search_key = e.NewText;
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
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion
    }
}