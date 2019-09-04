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
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.Movies.Adapters;
using WoWonder.Activities.Videos;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Movies;
using WoWonder_API.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.Movies
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Movies_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Movies_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Movies_Layout);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_Movies);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                //Get values
                MoviesRecyler = FindViewById<RecyclerView>(Resource.Id.MoviesRecylerview);
                Movies_Empty = FindViewById<LinearLayout>(Resource.Id.Movies_LinerEmpty);
                Icon_Movies = FindViewById<TextView>(Resource.Id.Movies_icon);

                MoviesRecyler.Visibility = ViewStates.Visible;
                Movies_Empty.Visibility = ViewStates.Gone;

                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                IMethods.Set_TextViewIcon("2", Icon_Movies, "\uf03d");
                Icon_Movies.SetTextColor(Color.ParseColor(Settings.MainColor));

                //Set Adapter
                mLayoutManager = new LinearLayoutManager(this);
                MoviesRecyler.SetLayoutManager(mLayoutManager);
                MoviesAdapter = new Movies_Adapter(this);
                MoviesRecyler.SetAdapter(MoviesAdapter);

                Get_Data_local();

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
                MoviesAdapter.ItemClick += MoviesAdapterOnItemClick;
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
                MoviesAdapter.ItemClick -= MoviesAdapterOnItemClick;
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
                if (MoviesAdapter != null)
                    if (Classes.ListChachedData_Movie.Count > 0)
                    {
                        MoviesAdapter.mMoviesList = Classes.ListChachedData_Movie;
                        MoviesAdapter.BindEnd();
                    }

                Get_MoviesList_API();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //Get Data Movies Using Api
        public async void Get_MoviesList_API(string offset = "")
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
                    var (api_status, respond) = await Client.Movies.Get_Movies("10", offset);
                    if (api_status == 200)
                    {
                        if (respond is Get_Movies_Object result)
                            RunOnUiThread(() =>
                            {
                                if (result.movies.Length <= 0)
                                {
                                    if (swipeRefreshLayout.Refreshing)
                                        swipeRefreshLayout.Refreshing = false;
                                }
                                else if (result.movies.Length > 0)
                                {
                                    if (MoviesAdapter.mMoviesList.Count > 0)
                                    {
                                        //Bring new users
                                        var listnew = result.movies.Where(c =>
                                            !MoviesAdapter.mMoviesList.Select(fc => fc.id).Contains(c.id)).ToList();
                                        if (listnew.Count > 0) Classes.AddRange(MoviesAdapter.mMoviesList, listnew);
                                    }
                                    else
                                    {
                                        MoviesAdapter.mMoviesList =
                                            new ObservableCollection<Get_Movies_Object.Movie>(result.movies);
                                        MoviesAdapter.BindEnd();
                                    }
                                }
                            });
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

                //Show Empty Page >> 
                //===============================================================
                RunOnUiThread(() =>
                {
                    if (MoviesAdapter.mMoviesList.Count > 0)
                    {
                        MoviesRecyler.Visibility = ViewStates.Visible;
                        Movies_Empty.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        MoviesRecyler.Visibility = ViewStates.Gone;
                        Movies_Empty.Visibility = ViewStates.Visible;
                    }

                    swipeRefreshLayout.Refreshing = false;

                    //Set Event Scroll
                    if (OnMainScrolEvent == null)
                    {
                        var xamarinRecyclerViewOnScrollListener =
                            new XamarinRecyclerViewOnScrollListener(mLayoutManager, swipeRefreshLayout);
                        OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                        OnMainScrolEvent.LoadMoreEvent += Movies_OnScroll_OnLoadMoreEvent;
                        MoviesRecyler.AddOnScrollListener(OnMainScrolEvent);
                        MoviesRecyler.AddOnScrollListener(new ScrollDownDetector());
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
                Get_MoviesList_API(offset);
            }
        }


        private void MoviesAdapterOnItemClick(object sender, Movies_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = MoviesAdapter.GetItem(position);
                    if (item != null)
                    {
                        var Int = new Intent(this, typeof(Video_Viewer_Activity));
                        Int.PutExtra("Viewer_Video", JsonConvert.SerializeObject(item));
                        Int.PutExtra("VideoId", item.id);
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
                MoviesAdapter.Clear();
                Get_MoviesList_API();
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
                if (MoviesAdapter.mMoviesList.Count > 0)
                    Classes.ListChachedData_Movie = MoviesAdapter.mMoviesList;

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

        private RecyclerView MoviesRecyler;
        public Movies_Adapter MoviesAdapter;

        private LinearLayout Movies_Empty;
        private TextView Icon_Movies;

        public SwipeRefreshLayout swipeRefreshLayout;

        private string LastMoviesid = "";

        public XamarinRecyclerViewOnScrollListener OnMainScrolEvent;

        #endregion

        #region Scroll

        //Event Scroll #Movies
        private void Movies_OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = MoviesAdapter.mMoviesList.LastOrDefault();
                if (item != null) LastMoviesid = item.id;

                if (LastMoviesid != "")
                    try
                    {
                        //Run Load More Api 
                        Task.Run(() => { Get_MoviesList_API(LastMoviesid); });
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