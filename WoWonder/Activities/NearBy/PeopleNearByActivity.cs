using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.Geolocator;
using SettingsConnecter;
using WoWonder.Activities.MyProfile;
using WoWonder.Activities.NearBy.Adapters;
using WoWonder.Activities.UserProfile;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.NearBy
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class PeopleNearByActivity : AppCompatActivity, ILocationListener
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

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.NearByPepole_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.NearByPepole_Layout);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_NearBy);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                //Get values
                NearByRecylerView = (RecyclerView) FindViewById(Resource.Id.Recyler);
                NearBy_Empty = FindViewById<LinearLayout>(Resource.Id.NearBy_LinerEmpty);
                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                Icon_NearBy = FindViewById<TextView>(Resource.Id.NearBy_icon);

                FloatingActionButtonView = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonView);

                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                IMethods.Set_TextViewIcon("2", Icon_NearBy, "\uf21d");
                Icon_NearBy.SetTextColor(Color.ParseColor(Settings.MainColor));

                //Set Adapter
                mLayoutManager = new GridLayoutManager(this, 2);
                NearByRecylerView.SetLayoutManager(mLayoutManager);
                NearByRecylerView.AddItemDecoration(new GridSpacingItemDecoration(2, 3, true));
                NearByAdapter = new NearByAdapter(this);
                NearByRecylerView.SetAdapter(NearByAdapter);
                NearByRecylerView.SetItemViewCacheSize(18);
                NearByRecylerView.GetLayoutManager().ItemPrefetchEnabled = true;
                NearByRecylerView.DrawingCacheEnabled = (true);
                NearByRecylerView.DrawingCacheQuality = DrawingCacheQuality.High;
                NearByRecylerView.Visibility = ViewStates.Visible;
                NearBy_Empty.Visibility = ViewStates.Gone;

                InitializeLocationManager();

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
                NearByAdapter.ItemClick += NearByAdapter_OnItemClick;
                FloatingActionButtonView.Click += Filter_OnClick;
                swipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;

                locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
                locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 400, 1, this);
                locationManager.RequestLocationUpdates(LocationManager.NetworkProvider, 400, 1, this);
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
                NearByAdapter.ItemClick -= NearByAdapter_OnItemClick;
                FloatingActionButtonView.Click -= Filter_OnClick;
                swipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;

                locationManager.RemoveUpdates(this);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Get_Data_local()
        {
            try
            {
                if (NearByAdapter != null)
                    if (Classes.ListChachedData_Nearby.Count > 0)
                    {
                        NearByAdapter.mNearByList = Classes.ListChachedData_Nearby;
                        NearByAdapter.BindEnd();
                    }

                GetFilter();

                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                    Get_NearByList_API();
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                        Get_NearByList_API();
                    else
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.AccessFineLocation,
                            Manifest.Permission.AccessCoarseLocation
                        }, 108);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void GetFilter()
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();

                var data = dbDatabase.GetNearByFilterById();
                if (data != null)
                {
                    Filter_gender = data.Gender.ToString();
                    Filter_Distance = data.DistanceValue.ToString();
                    Filter_status = data.Status.ToString();
                }
                else
                {
                    Filter_gender = "0";
                    Filter_Distance = "0";
                    Filter_status = "0";
                }

                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public bool ShowToast = true;
        public async void Get_NearByList_API(string offset = "")
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
                    XamarinRecyclerViewOnScrollListener.IsLoading = true;

                    await GetPosition();

                    var dictionary = new Dictionary<string, string>
                    {
                        {"limit", "25"},
                        {"offset", offset},
                        {"gender", Filter_gender},
                        {"keyword", ""},
                        {"status", Filter_status},
                        {"distance", Filter_Distance},
                        {"lat", UserDetails.Lat},
                        {"lng", UserDetails.Lng}
                    };


                    var (Api_status, Respond) = await Client.Nearby.Get_Nearby_Users(dictionary);
                    if (Api_status == 200)
                    {
                        if (Respond is Get_Nearby_Users_Object result)
                            RunOnUiThread(() =>
                            {
                                if (result.nearby_users.Length <= 0)
                                {
                                    if (swipeRefreshLayout.Refreshing)
                                        swipeRefreshLayout.Refreshing = false;

                                    if (NearByAdapter.mNearByList.Count > 0)
                                        Snackbar.Make(NearByRecylerView, GetText(Resource.String.Lbl_No_more_users),
                                            Snackbar.LengthLong).Show();
                                }
                                else if (result.nearby_users.Length > 0)
                                {
                                    if (NearByAdapter.mNearByList.Count <= 0)
                                    {
                                        NearByAdapter.mNearByList =
                                            new ObservableCollection<Get_Nearby_Users_Object.Nearby_Users>(
                                                result.nearby_users);
                                        NearByAdapter.BindEnd();
                                    }
                                    else
                                    {
                                        //Bring new item
                                        var listnew = result.nearby_users?.Where(c =>
                                                !NearByAdapter.mNearByList.Select(fc => fc.user_id).Contains(c.user_id))
                                            .ToList();
                                        if (listnew.Count > 0)
                                        {
                                            var lastCountItem = NearByAdapter.ItemCount;

                                            //Results differ
                                            Classes.AddRange(NearByAdapter.mNearByList, listnew);
                                            NearByAdapter.NotifyItemRangeInserted(lastCountItem, listnew.Count);
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
                }

                //Show Empty Page >> 
                //===============================================================
                RunOnUiThread(() =>
                {
                    if (NearByAdapter.mNearByList.Count > 0)
                    {
                        NearByRecylerView.Visibility = ViewStates.Visible;
                        NearBy_Empty.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        NearByRecylerView.Visibility = ViewStates.Gone;
                        NearBy_Empty.Visibility = ViewStates.Visible;
                    }

                    swipeRefreshLayout.Refreshing = false;

                    //Set Event Scroll
                    if (OnMainScrolEvent == null)
                    {
                        var xamarinRecyclerViewOnScrollListener =new XamarinRecyclerViewOnScrollListener(mLayoutManager, swipeRefreshLayout);
                        OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                        OnMainScrolEvent.LoadMoreEvent += NearBy_OnScroll_OnLoadMoreEvent;
                        NearByRecylerView.AddOnScrollListener(OnMainScrolEvent);
                        NearByRecylerView.AddOnScrollListener(new ScrollDownDetector());
                    }
                    else
                    {
                        XamarinRecyclerViewOnScrollListener.IsLoading = false;
                    }
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_NearByList_API(offset);
            }
        }

        private void NearByAdapter_OnItemClick(object sender, NearByAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = NearByAdapter.GetItem(position);
                    if (item != null)
                    {
                        Intent Int;
                        if (item.user_id != UserDetails.User_id)
                        {
                            Int = new Intent(this, typeof(User_Profile_Activity));
                            Int.PutExtra("UserId", item.user_id);
                            Int.PutExtra("UserType", "NearBy");
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
                var intent = new Intent(this, typeof(NearByFilter_Activity));
                StartActivityForResult(intent, 50000);
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

                if (requestCode == 50000 && resultCode == Result.Ok)
                {
                    var gender = data.GetStringExtra("Gender") ?? "Data not available";
                    if (gender != "Data not available" && !string.IsNullOrEmpty(gender))
                        Filter_gender = gender;

                    var distance = data.GetStringExtra("Distance") ?? "Data not available";
                    if (distance != "Data not available" && !string.IsNullOrEmpty(distance))
                        Filter_Distance = distance;

                    var status = data.GetStringExtra("Status") ?? "Data not available";
                    if (status != "Data not available" && !string.IsNullOrEmpty(status))
                        Filter_status = status;

                    NearByAdapter.Clear();

                    swipeRefreshLayout.Refreshing = true;

                    Get_NearByList_API();
                }
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

                if (requestCode == 108)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        Get_NearByList_API();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
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
                NearByAdapter.Clear();
                Get_NearByList_API();
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
                if (NearByAdapter.mNearByList.Count > 0)
                    Classes.ListChachedData_Nearby = NearByAdapter.mNearByList;

                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Variables Basic

        public RecyclerView NearByRecylerView;
        private GridLayoutManager mLayoutManager;
        public NearByAdapter NearByAdapter;

        private LinearLayout NearBy_Empty;
        private TextView Icon_NearBy;

        public SwipeRefreshLayout swipeRefreshLayout;

        private Location currentLocation;
        private LocationManager locationManager;
        private string locationProvider;
        private string _deviceAddress = "";
        private string LastNearByid = "";

        private string Filter_gender = "";
        private string Filter_Distance = "";
        private string Filter_status = "";


        private FloatingActionButton FloatingActionButtonView;

        public XamarinRecyclerViewOnScrollListener OnMainScrolEvent;

        #endregion

        #region Location

        //Get Position GPS Current Location
        public async Task<bool> GetPosition()
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                    if (CrossGeolocator.Current.IsGeolocationAvailable)
                    {
                        if (currentLocation == null)
                        {
                            //Toast.MakeText(this, "Can't determine the current address. Try again in a few minutes.", ToastLength.Short).Show();
                            //return false;
                        }

                        var address = await ReverseGeocodeCurrentLocation();
                        if (address != null) DisplayAddress(address);

                        return true;
                    }
                    //throw new Exception("Geolocation is turned off");
                    // Geolocation is turned off for the device.

                    Toast.MakeText(this, GetString(Resource.String.Lbl_turn_your_GPS), ToastLength.Short).Show();
                    return false;
                }

                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted &&
                    CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                {
                    if (CrossGeolocator.Current.IsGeolocationAvailable)
                    {
                        if (currentLocation == null)
                        {
                            //Toast.MakeText(this, "Can't determine the current address. Try again in a few minutes.", ToastLength.Short).Show();
                            //return false;
                        }

                        var address = await ReverseGeocodeCurrentLocation();
                        if (address != null) DisplayAddress(address);

                        return true;
                    }
                    //throw new Exception("Geolocation is turned off");
                    // Geolocation is turned off for the device.

                    Toast.MakeText(this, GetString(Resource.String.Lbl_turn_your_GPS), ToastLength.Short).Show();
                    return false;
                }

                RequestPermissions(new[]
                {
                    Manifest.Permission.AccessFineLocation,
                    Manifest.Permission.AccessCoarseLocation
                }, 108);

                return false;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return false;
            }
        }

        private async Task<Address> ReverseGeocodeCurrentLocation()
        {
            try
            {
                var geocoder = new Geocoder(this);
                var addressList =
                    await geocoder.GetFromLocationAsync(currentLocation.Latitude, currentLocation.Longitude, 10);

                var address = addressList.FirstOrDefault();
                return address;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        }

        private void DisplayAddress(Address address)
        {
            try
            {
                if (address != null)
                {
                    var deviceAddress = new StringBuilder();
                    for (var i = 0; i < address.MaxAddressLineIndex; i++)
                        deviceAddress.AppendLine(address.GetAddressLine(i));
                    // Remove the last comma from the end of the address.
                    _deviceAddress = deviceAddress.ToString();
                }
                else
                {
                    //Error Message  
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Error_DisplayAddress), ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void InitializeLocationManager()
        {
            try
            {
                locationManager = (LocationManager) GetSystemService(LocationService);
                var criteriaForLocationService = new Criteria
                {
                    Accuracy = Accuracy.Fine
                };
                var acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);
                if (acceptableLocationProviders.Any())
                    locationProvider = acceptableLocationProviders.First();
                else
                    locationProvider = string.Empty;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public async void OnLocationChanged(Location location)
        {
            try
            {
                currentLocation = location;
                if (currentLocation == null)
                {
                    //Error Message  
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Error_LocationChanged), ToastLength.Short).Show();
                }
                else
                {
                    UserDetails.Lat = currentLocation.Latitude.ToString();
                    UserDetails.Lng = currentLocation.Longitude.ToString();

                    var address = await ReverseGeocodeCurrentLocation();
                    DisplayAddress(address);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnProviderDisabled(string provider)
        {
            try
            {
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnProviderEnabled(string provider)
        {
            try
            {
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            try
            {
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion

        #region Scroll

        //Event Scroll #NearBy
        private void NearBy_OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = NearByAdapter.mNearByList.LastOrDefault();
                if (item != null) LastNearByid = item.user_id;

                if (LastNearByid != "" )
                    try
                    {
                        //Run Load More Api 
                        Task.Run(() => { Get_NearByList_API(LastNearByid); });
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

            public static bool IsLoading = false;
            public GridLayoutManager LayoutManager;
            public SwipeRefreshLayout SwipeRefreshLayout;

            public XamarinRecyclerViewOnScrollListener(GridLayoutManager layoutManager,
                SwipeRefreshLayout swipeRefreshLayout)
            {
                try
                {
                    IsLoading = false;
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
                    if (visibleItemCount + pastVisiblesItems + 4 >= totalItemCount)
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
                        {
                            ImageService.Instance
                                .SetPauseWork(true); // all image loading requests will be silently canceled
                        }
                        else if (recyclerView.ScrollState == (int) ScrollState.Fling)
                        {
                            ImageService.Instance.SetPauseWork(false);
                        }
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