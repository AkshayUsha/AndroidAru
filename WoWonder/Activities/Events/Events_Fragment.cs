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
    public class Event_Fragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                var view = MyContextWrapper.GetContentView(Context, Settings.Lang,
                    Resource.Layout.Event_Fragment_Layout);
                if (view == null) view = inflater.Inflate(Resource.Layout.Event_Fragment_Layout, container, false);

                MainRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.Recyler);
                Events_Empty = view.FindViewById<LinearLayout>(Resource.Id.Events_LinerEmpty);

                EventsIcon = view.FindViewById<TextView>(Resource.Id.Events_icon);

                IMethods.Set_TextViewIcon("2", EventsIcon, "\uf073");

                Btn_CreateEvents = view.FindViewById<Button>(Resource.Id.CreateEvents_Button);
                swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                MLayoutManager = new LinearLayoutManager(Activity);
                MainRecyclerView.SetLayoutManager(MLayoutManager);
                MEventAdapter = new Event_Adapter(Activity);
                MEventAdapter.mEventList = new ObservableCollection<Get_Events_Object.Event>();
                MainRecyclerView.SetAdapter(MEventAdapter);

                MainRecyclerView.Visibility = ViewStates.Visible;
                Events_Empty.Visibility = ViewStates.Gone;

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
                MEventAdapter.ItemClick += MAdapterOnItemClick;
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
                MEventAdapter.ItemClick -= MAdapterOnItemClick;
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
                if (MEventAdapter != null)
                    if (Classes.ListChachedData_Event.Count > 0)
                    {
                        MEventAdapter.mEventList = Classes.ListChachedData_Event;
                        MEventAdapter.BindEnd();
                    }

                Get_Event_Api();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Get Data Event Using API
        public async void Get_Event_Api(string offset = "")
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
                    var (api_status, respond) = await Client.Event.Get_Events("20", offset, "");
                    if (api_status == 200)
                    {
                        if (respond is Get_Events_Object result)
                            Activity.RunOnUiThread(() =>
                            {
                                if (result.events.Length <= 0)
                                {
                                    if (swipeRefreshLayout != null)
                                        swipeRefreshLayout.Refreshing = false;
                                }
                                else if (result.events.Length > 0)
                                {
                                    if (MEventAdapter.mEventList.Count <= 0)
                                    {
                                        MEventAdapter.mEventList =
                                            new ObservableCollection<Get_Events_Object.Event>(result.events);
                                        MEventAdapter.BindEnd();
                                    }
                                    else
                                    {
                                        //Bring new item
                                        var listnew = result.events?.Where(c =>
                                            !MEventAdapter.mEventList.Select(fc => fc.id).Contains(c.id)).ToList();
                                        if (listnew.Count > 0)
                                        {
                                            var lastCountItem = MEventAdapter.ItemCount;

                                            //Results differ
                                            Classes.AddRange(MEventAdapter.mEventList, listnew);
                                            MEventAdapter.NotifyItemRangeInserted(lastCountItem, listnew.Count);
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
                    if (MEventAdapter.mEventList.Count > 0)
                    {
                        MainRecyclerView.Visibility = ViewStates.Visible;
                        Events_Empty.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        MainRecyclerView.Visibility = ViewStates.Gone;
                        Events_Empty.Visibility = ViewStates.Visible;
                    }

                    if (swipeRefreshLayout != null)
                        swipeRefreshLayout.Refreshing = false;

                    //Set Event Scroll
                    if (OnEventMainScrolEvent == null)
                    {
                        var xamarinRecyclerViewOnScrollListener =
                            new XamarinRecyclerViewOnScrollListener(MLayoutManager, swipeRefreshLayout);
                        OnEventMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                        OnEventMainScrolEvent.LoadMoreEvent += EventOnScroll_OnLoadMoreEvent;
                        MainRecyclerView.AddOnScrollListener(OnEventMainScrolEvent);
                        MainRecyclerView.AddOnScrollListener(new ScrollDownDetector());
                    }
                    else
                    {
                        OnEventMainScrolEvent.IsLoading = false;
                    }
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_Event_Api(offset);
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

        //Event Go to see data Event >> Open EventView_Activity
        private void MAdapterOnItemClick(object sender, Event_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = MEventAdapter.GetItem(position);
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

        //Event Refresh Data Page
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                MEventAdapter.Clear();
                Get_Event_Api();
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
                if (MEventAdapter.mEventList.Count > 0)
                    Classes.ListChachedData_Event = MEventAdapter.mEventList;

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
        public static Event_Adapter MEventAdapter;

        public RecyclerView MainRecyclerView;
        public LinearLayout Events_Empty;

        private TextView EventsIcon;
        private Button Btn_CreateEvents;
        public SwipeRefreshLayout swipeRefreshLayout;

        public XamarinRecyclerViewOnScrollListener OnEventMainScrolEvent;

        public string LastEventid = "";

        #endregion
         
        #region Scroll

        //Event Scroll #Event
        private void EventOnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = MEventAdapter.mEventList.LastOrDefault();
                if (item != null) LastEventid = item.id;

                if (LastEventid != "")
                    try
                    {
                        //Run Load More Api 
                        Task.Run(() => { Get_Event_Api(LastEventid); });
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

                    var pastVisitableItems = LayoutManager.FindFirstVisibleItemPosition();
                    if (visibleItemCount + pastVisitableItems + 8 >= totalItemCount)
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