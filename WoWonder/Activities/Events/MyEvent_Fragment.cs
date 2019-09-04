using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.Events.Adapters;
using WoWonder.Helpers;
using WoWonder_API.Classes.Event;
using WoWonder_API.Classes.Global;
using WoWonder_API.Requests;

namespace WoWonder.Activities.Events
{
    public class MyEvent_Fragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                var view = MyContextWrapper.GetContentView(Context, Settings.Lang,
                    Resource.Layout.Event_Fragment_Layout);
                if (view == null) view = inflater.Inflate(Resource.Layout.Event_Fragment_Layout, container, false);

                MyEventRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.Recyler);
                MyEvent_Empty = view.FindViewById<LinearLayout>(Resource.Id.Events_LinerEmpty);

                MyEventIcon = view.FindViewById<TextView>(Resource.Id.Events_icon);

                IMethods.Set_TextViewIcon("2", MyEventIcon, "\uf073");

                Btn_CreateEvents = view.FindViewById<Button>(Resource.Id.CreateEvents_Button);

                swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                MLayoutManager = new LinearLayoutManager(Activity);
                MyEventRecyclerView.SetLayoutManager(MLayoutManager);
                MyEventAdapter = new MyEvent_Adapter(Activity);
                MyEventRecyclerView.SetAdapter(MyEventAdapter);

                MyEventRecyclerView.Visibility = ViewStates.Visible;
                MyEvent_Empty.Visibility = ViewStates.Gone;


                Get_Data_local();

                return view;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();

                //Add Event
                Btn_CreateEvents.Click += BtnCreateEventsOnClick;
                MyEventAdapter.ItemClick += MyEventAdapterOnItemClick;
                swipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                base.OnPause();

                //Close Event
                Btn_CreateEvents.Click -= BtnCreateEventsOnClick;
                MyEventAdapter.ItemClick -= MyEventAdapterOnItemClick;
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
                if (MyEventAdapter != null)
                    if (Classes.ListChachedData_MyEvents.Count > 0)
                    {
                        MyEventAdapter.mMyEventList = Classes.ListChachedData_MyEvents;
                        MyEventAdapter.BindEnd();
                    }

                Get_MyEvent_Api();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Get Data My Event Using API
        public async void Get_MyEvent_Api(string offset = "")
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Activity.RunOnUiThread(() =>
                    {
                        if (swipeRefreshLayout != null)
                            swipeRefreshLayout.Refreshing = false;
                    });
                    Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection),
                        ToastLength.Short).Show();
                }
                else
                {
                    var (api_status, respond) = await Client.Event.Get_MyEvents("20", "", offset);
                    if (api_status == 200)
                    {
                        if (respond is Get_MyEvent_object result)
                            Activity.RunOnUiThread(() =>
                            {
                                if (result.my_events.Length <= 0)
                                {
                                    if (swipeRefreshLayout != null)
                                        swipeRefreshLayout.Refreshing = false;
                                }
                                else if (result.my_events.Length > 0)
                                {
                                    if (MyEventAdapter.mMyEventList.Count <= 0)
                                    {
                                        MyEventAdapter.mMyEventList =
                                            new ObservableCollection<Get_MyEvent_object.My_Events>(result.my_events);
                                        MyEventAdapter.BindEnd();
                                    }
                                    else
                                    {
                                        //Bring new item
                                        var listNew = result.my_events?.Where(c =>
                                            !MyEventAdapter.mMyEventList.Select(fc => fc.id).Contains(c.id)).ToList();
                                        if (listNew.Count > 0)
                                        {
                                            var lastCountItem = MyEventAdapter.ItemCount;

                                            //Results differ
                                            Classes.AddRange(MyEventAdapter.mMyEventList, listNew);
                                            MyEventAdapter.NotifyItemRangeInserted(lastCountItem, listNew.Count);
                                        }

                                        if (swipeRefreshLayout != null)
                                            swipeRefreshLayout.Refreshing = false;
                                    }
                                }
                            });
                    }
                    else if (api_status == 400)
                    {
                        if (respond is Error_Object error)
                        {
                            var errortext = error._errors.Error_text;
                            //Toast.MakeText(this.Activity, errortext, ToastLength.Short).Show();

                            if (errortext.Contains("Invalid or expired access_token"))
                                API_Request.Logout(Activity);
                        }
                    }
                    else if (api_status == 404)
                    {
                        var error = respond.ToString();
                        //Toast.MakeText(this.Activity, error, ToastLength.Short).Show();
                    }
                }

                //Show Empty Page >> 
                //===============================================================
                Activity.RunOnUiThread(() =>
                {
                    if (MyEventAdapter.mMyEventList.Count > 0)
                    {
                        MyEventRecyclerView.Visibility = ViewStates.Visible;
                        MyEvent_Empty.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        MyEventRecyclerView.Visibility = ViewStates.Gone;
                        MyEvent_Empty.Visibility = ViewStates.Visible;
                    }

                    if (swipeRefreshLayout != null)
                        swipeRefreshLayout.Refreshing = false;

                    //Set Event Scroll
                    if (OnMyEventScrolEvent == null)
                    {
                        var xamarinRecyclerViewOnScrollListener =
                            new XamarinRecyclerViewOnScrollListener(MLayoutManager, swipeRefreshLayout);
                        OnMyEventScrolEvent = xamarinRecyclerViewOnScrollListener;
                        OnMyEventScrolEvent.LoadMoreEvent += MyEventOnScroll_OnLoadMoreEvent;
                        MyEventRecyclerView.AddOnScrollListener(OnMyEventScrolEvent);
                        MyEventRecyclerView.AddOnScrollListener(new ScrollDownDetector());
                    }
                    else
                    {
                        OnMyEventScrolEvent.IsLoading = false;
                    }
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_MyEvent_Api(offset);
            }
        }

        //Event Go to see data Event >> Open EventView_Activity
        private void MyEventAdapterOnItemClick(object sender, MyEvent_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = MyEventAdapter.GetItem(position);
                    if (item != null)
                    {
                        var Int = new Intent(Context, typeof(EventView_Activity));
                        Int.PutExtra("EventView", JsonConvert.SerializeObject(item));
                        StartActivity(Int);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Create New Event >> Open CreateEvent_Activity  
        private void BtnCreateEventsOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var Int = new Intent(Context, typeof(CreateEvent_Activity));
                StartActivity(Int);
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
                MyEventAdapter.Clear();
                Get_MyEvent_Api();
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

        public override void OnDestroy()
        {
            try
            {
                if (MyEventAdapter.mMyEventList.Count > 0)
                    Classes.ListChachedData_MyEvents = MyEventAdapter.mMyEventList;

                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Variables Basic

        public LinearLayoutManager MLayoutManager;
        public static MyEvent_Adapter MyEventAdapter;

        public RecyclerView MyEventRecyclerView;
        public LinearLayout MyEvent_Empty;

        private TextView MyEventIcon;
        private Button Btn_CreateEvents;
        public SwipeRefreshLayout swipeRefreshLayout;

        public XamarinRecyclerViewOnScrollListener OnMyEventScrolEvent;

        public string LastMyEventid = "";

        #endregion

        #region Scroll

        //Event Scroll #My_Event
        private void MyEventOnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = MyEventAdapter.mMyEventList.LastOrDefault();
                if (item != null) LastMyEventid = item.id;

                if (LastMyEventid != "")
                    try
                    {
                        //Run Load More Api 
                        Task.Run(() => { Get_MyEvent_Api(LastMyEventid); });
                    }
                    catch (Exception exception)
                    {
                        Crashes.TrackError(exception);
                    }

                if (swipeRefreshLayout != null)
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