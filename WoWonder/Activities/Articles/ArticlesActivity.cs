using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.Articles.Adapters;
using WoWonder.Activities.UsersPages;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.Articles
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class ArticlesActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Articles_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Articles_Layout);

                ArticlsRecylerView = (RecyclerView) FindViewById(Resource.Id.Recyler);
                Articls_Empty = FindViewById<LinearLayout>(Resource.Id.Article_LinerEmpty);

                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                Icon_Article = FindViewById<TextView>(Resource.Id.Article_icon);
                IMethods.Set_TextViewIcon("2", Icon_Article, "\uf15c");
                Icon_Article.SetTextColor(Color.ParseColor(Settings.MainColor));

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_ExploreArticle);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                ArticlesAdapter = new Articles_Adapter(this);

                mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                ArticlsRecylerView.SetLayoutManager(mLayoutManager);
                ArticlsRecylerView.SetAdapter(ArticlesAdapter);

                Articls_Empty.Visibility = ViewStates.Gone;
                ArticlsRecylerView.Visibility = ViewStates.Visible;

                Get_Data_local();
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
                ArticlesAdapter.ItemClick += ArticlesAdapterOnItemClick;
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
                ArticlesAdapter.ItemClick -= ArticlesAdapterOnItemClick;
                swipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void Get_Data_local()
        {
            try
            {
                if (ArticlesAdapter != null)
                    if (Classes.ListChachedData_Article.Count > 0)
                    {
                        ArticlesAdapter.ArticlesList = Classes.ListChachedData_Article;
                        ArticlesAdapter.BindEnd();
                    }

                Get_Articles_Api();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Get Articles API
        public async void Get_Articles_Api(string offset = "")
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
                    var (Api_status, Respond) = await Client.Article.Get_Articles("25", offset);
                    if (Api_status == 200)
                    {
                        if (Respond is Get_Users_Articles_Object result)
                            RunOnUiThread(() =>
                            {
                                if (result.articles.Length <= 0)
                                {
                                    if (swipeRefreshLayout != null)
                                        swipeRefreshLayout.Refreshing = false;
                                }
                                else if (result.articles.Length > 0)
                                {
                                    if (ArticlesAdapter.ArticlesList.Count <= 0)
                                    {
                                        ArticlesAdapter.ArticlesList =
                                            new ObservableCollection<Get_Users_Articles_Object.Article>(
                                                result.articles);
                                        ArticlesAdapter.BindEnd();
                                    }
                                    else
                                    {
                                        //Bring new item
                                        var listnew = result.articles?.Where(c =>
                                            !ArticlesAdapter.ArticlesList.Select(fc => fc.id).Contains(c.id)).ToList();
                                        if (listnew.Count > 0)
                                        {
                                            var lastCountItem = ArticlesAdapter.ItemCount;

                                            //Results differ
                                            Classes.AddRange(ArticlesAdapter.ArticlesList, listnew);
                                            ArticlesAdapter.NotifyItemRangeInserted(lastCountItem, listnew.Count);
                                        }

                                        if (swipeRefreshLayout.Refreshing)
                                            swipeRefreshLayout.Refreshing = false;
                                    }
                                }
                            });
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

                    //Show Empty Page
                    //===========================================
                    RunOnUiThread(() =>
                    {
                        if (ArticlesAdapter.ArticlesList.Count > 0)
                        {
                            Articls_Empty.Visibility = ViewStates.Gone;
                            ArticlsRecylerView.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            Articls_Empty.Visibility = ViewStates.Visible;
                            ArticlsRecylerView.Visibility = ViewStates.Gone;
                        }

                        swipeRefreshLayout.Refreshing = false;

                        //Set Event Scroll
                        if (OnMainScrolEvent == null)
                        {
                            var xamarinRecyclerViewOnScrollListener =
                                new XamarinRecyclerViewOnScrollListener(mLayoutManager, swipeRefreshLayout);
                            OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                            OnMainScrolEvent.LoadMoreEvent += Article_OnScroll_OnLoadMoreEvent;
                            ArticlsRecylerView.AddOnScrollListener(OnMainScrolEvent);
                            ArticlsRecylerView.AddOnScrollListener(new ScrollDownDetector());
                        }
                        else
                        {
                            OnMainScrolEvent.IsLoading = false;
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_Articles_Api(offset);
            }
        }

        private void ArticlesAdapterOnItemClick(object sender, Articles_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = ArticlesAdapter.GetItem(position);
                    if (item != null)
                    {
                        var intent = new Intent(this, typeof(HyberdPostViewer_Activity));
                        intent.PutExtra("Type", "Article");
                        intent.PutExtra("Id", item.url);
                        intent.PutExtra("Title", item.title);
                        StartActivity(intent);
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
                ArticlesAdapter.Clear();
                Get_Articles_Api();
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
                if (ArticlesAdapter.ArticlesList.Count > 0)
                    Classes.ListChachedData_Article = ArticlesAdapter.ArticlesList;

                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Variables Basic

        public LinearLayoutManager mLayoutManager;

        public Articles_Adapter ArticlesAdapter;
        public RecyclerView ArticlsRecylerView;

        private SwipeRefreshLayout swipeRefreshLayout;
        private LinearLayout Articls_Empty;
        private TextView Icon_Article;

        private string LastArticleid = "";

        public XamarinRecyclerViewOnScrollListener OnMainScrolEvent;

        #endregion

        #region Scroll

        //Event Scroll #Article
        private void Article_OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = ArticlesAdapter.ArticlesList.LastOrDefault();
                if (item != null) LastArticleid = item.id;

                if (LastArticleid != "")
                    try
                    {
                        //Run Load More Api 
                        Task.Run(() => { Get_Articles_Api(LastArticleid); });
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