using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
using WoWonder.Activities.Album.Adapters;
using WoWonder.Activities.UsersPages;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.Album
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class MyPhotosActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Albums_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Albums_Layout);

                var data = Intent.GetStringExtra("UserId") ?? "Data not available";
                if (data != "Data not available" && !string.IsNullOrEmpty(data)) S_UserId = data;

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    if (S_UserId == UserDetails.User_id)
                    {
                        ToolBar.Title = GetText(Resource.String.Lbl_MyImages);
                    }
                    else
                    {
                        ToolBar.Title = GetText(Resource.String.Lbl_YourImages);
                    }

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }


                ImagesRecylerView = (RecyclerView) FindViewById(Resource.Id.RecylerImages);

                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                photos_Empty = FindViewById<LinearLayout>(Resource.Id.Albums_LinerEmpty);

                Icon_photos = FindViewById<TextView>(Resource.Id.Albums_icon);
                IMethods.Set_TextViewIcon("2", Icon_photos, "\uf03e");

                ImagesRecylerView.Visibility = ViewStates.Visible;
                photos_Empty.Visibility = ViewStates.Gone;

                photosAdapter = new PhotosAdapter(this);

                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                    mLayoutManager = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
                    ImagesRecylerView.SetLayoutManager(mLayoutManager);
                }
                else
                {
                    mLayoutManager = new StaggeredGridLayoutManager(3, StaggeredGridLayoutManager.Vertical);
                    ImagesRecylerView.SetLayoutManager(mLayoutManager);
                }

                ImagesRecylerView.AddItemDecoration(new GridSpacingItemDecoration(2, 3, true));
                ImagesRecylerView.SetAdapter(photosAdapter);
                ImagesRecylerView.Visibility = ViewStates.Visible;

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
                photosAdapter.ItemClick += PhotosAdapterOnItemClick;
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
                photosAdapter.ItemClick -= PhotosAdapterOnItemClick;
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
                if (photosAdapter != null)
                {
                    if (S_UserId == UserDetails.User_id)
                    {
                        if (Classes.ListChachedData_Album.Count > 0)
                        {
                            photosAdapter.mMyAlbumsList = Classes.ListChachedData_Album;
                            photosAdapter.BindEnd();
                        }
                    }
                }
                 
                Get_AlbumUser_Api();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Get Photo API
        public async void Get_AlbumUser_Api(string offset = "")
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    RunOnUiThread(() => { swipeRefreshLayout.Refreshing = false; });

                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)
                        .Show();
                    ImagesRecylerView.Visibility = ViewStates.Visible;
                    photos_Empty.Visibility = ViewStates.Gone;
                }
                else
                {
                    var (Api_status, Respond) = await Client.Album.Get_User_Albums(S_UserId, "35", offset);
                    if (Api_status == 200)
                    {
                        if (Respond is Get_User_Albums_Object result)
                            RunOnUiThread(() =>
                            {
                                if (result.albums.Count <= 0)
                                {
                                    if (swipeRefreshLayout != null)
                                        swipeRefreshLayout.Refreshing = false;
                                }
                                else if (result.albums.Count > 0)
                                {
                                    //Bring new groups
                                    var listNew = result.albums.Where(c =>
                                            !photosAdapter.mMyAlbumsList.Select(fc => fc.group_id).Contains(c.group_id))
                                        .ToList();
                                    if (listNew.Count > 0)
                                    {
                                        //Results differ
                                        Classes.AddRange(photosAdapter.mMyAlbumsList, listNew);
                                        photosAdapter.BindEnd();
                                    }
                                    else
                                    {
                                        photosAdapter.mMyAlbumsList =
                                            new ObservableCollection<Get_User_Albums_Object.Album>(result.albums);
                                        photosAdapter.BindEnd();
                                    }
                                }
                            });
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
                RunOnUiThread(() =>
                {
                    if (photosAdapter.mMyAlbumsList.Count > 0)
                    {
                        ImagesRecylerView.Visibility = ViewStates.Visible;
                        photos_Empty.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        ImagesRecylerView.Visibility = ViewStates.Gone;
                        photos_Empty.Visibility = ViewStates.Visible;
                    }

                    swipeRefreshLayout.Refreshing = false;

                    //Set Event Scroll
                    if (OnMainScrolEvent == null)
                    {
                        var xamarinRecyclerViewOnScrollListener =
                            new XamarinRecyclerViewOnScrollListener(mLayoutManager, swipeRefreshLayout);
                        OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                        OnMainScrolEvent.LoadMoreEvent += MyAlbums_OnScroll_OnLoadMoreEvent;
                        ImagesRecylerView.AddOnScrollListener(OnMainScrolEvent);
                        ImagesRecylerView.AddOnScrollListener(new ScrollDownDetector());
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
                Get_AlbumUser_Api(offset);
            }
        }

        // Event Open IMG Using ImagePostViewer_Activity
        private void PhotosAdapterOnItemClick(object sender, GroupsAdapteClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = photosAdapter.GetItem(position);
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


        //Event Refresh Data Page
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                photosAdapter.Clear();
                Get_AlbumUser_Api();
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
                if (S_UserId == UserDetails.User_id)
                {
                    if (photosAdapter.mMyAlbumsList.Count > 0)
                        Classes.ListChachedData_Album = photosAdapter.mMyAlbumsList;
                } 

                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Variables Basic

        public StaggeredGridLayoutManager mLayoutManager;

        public static PhotosAdapter photosAdapter;
        public RecyclerView ImagesRecylerView;
        public SwipeRefreshLayout swipeRefreshLayout;

        private LinearLayout photos_Empty;
        private TextView Icon_photos;

        private string S_UserId = "";
        private string LastMyAlbumsid = "";

        public XamarinRecyclerViewOnScrollListener OnMainScrolEvent;

        #endregion

        #region Scroll

        //Event Scroll #MyAlbums
        private void MyAlbums_OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = photosAdapter.mMyAlbumsList.LastOrDefault();
                if (item != null) LastMyAlbumsid = item.id;

                if (LastMyAlbumsid != "")
                    try
                    {
                        //Run Load More Api 
                        Task.Run(() => { Get_AlbumUser_Api(LastMyAlbumsid); });
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
            public StaggeredGridLayoutManager LayoutManager;
            public SwipeRefreshLayout SwipeRefreshLayout;

            public XamarinRecyclerViewOnScrollListener(StaggeredGridLayoutManager layoutManager,
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

                    int[] firstVisibleItemPositions;

                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                        firstVisibleItemPositions = new int[2];
                    else
                        firstVisibleItemPositions = new int[3];

                    var firstVisibleItem =
                        ((StaggeredGridLayoutManager) recyclerView.GetLayoutManager()).FindFirstVisibleItemPositions(
                            firstVisibleItemPositions)[0];

                    var pastVisiblesItems = firstVisibleItem;
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