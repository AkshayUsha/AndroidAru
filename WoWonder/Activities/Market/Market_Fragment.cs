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
using Android.Views.Animations;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.Market.Adapters;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Product;
using WoWonder_API.Requests;

namespace WoWonder.Activities.Market
{
    public class Market_Fragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                var view = MyContextWrapper.GetContentView(Context, Settings.Lang, Resource.Layout.Market_Fragment);
                if (view == null) view = inflater.Inflate(Resource.Layout.Market_Fragment, container, false);

                MainRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.Recyler);
                Market_Empty = view.FindViewById<LinearLayout>(Resource.Id.Market_LinerEmpty);

                IconEmpty = view.FindViewById<TextView>(Resource.Id.Market_icon);
                IMethods.Set_TextViewIcon("2", IconEmpty, "\uf07a");

                Btn_AddProduct = view.FindViewById<Button>(Resource.Id.AddProduct_Button);

                swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                MainRecyclerView.Visibility = ViewStates.Visible;
                Market_Empty.Visibility = ViewStates.Gone;

                MMarketAdapter = new MarketAdapter(Activity);
                mLayoutManager = new GridLayoutManager(Activity, 2);
                MainRecyclerView.SetLayoutManager(mLayoutManager);
                MainRecyclerView.AddItemDecoration(new GridSpacingItemDecoration(2, 10, true));
                var animation = AnimationUtils.LoadAnimation(Activity, Resource.Animation.slideUpAnim);
                MainRecyclerView.StartAnimation(animation);
                MMarketAdapter.MarketList = new ObservableCollection<Get_Products_Object.Product>();
                MainRecyclerView.SetAdapter(MMarketAdapter);

                MainRecyclerView.HasFixedSize = (true);
                MainRecyclerView.SetItemViewCacheSize(5);
                MainRecyclerView.GetLayoutManager().ItemPrefetchEnabled = true;
                MainRecyclerView.DrawingCacheEnabled = (true);
                

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
                Btn_AddProduct.Click += BtnAddProductOnClick;
                MMarketAdapter.ItemClick += MMarketAdapter_OnItemClick;
                swipeRefreshLayout.Refresh += Market_SwipeRefreshLayoutOnRefresh;
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
                Btn_AddProduct.Click -= BtnAddProductOnClick;
                MMarketAdapter.ItemClick -= MMarketAdapter_OnItemClick;
                swipeRefreshLayout.Refresh -= Market_SwipeRefreshLayoutOnRefresh;
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
                if (MMarketAdapter != null)
                    if (Classes.ListChachedData_Product.Count > 0)
                    {
                        MMarketAdapter.MarketList = Classes.ListChachedData_Product;
                        MMarketAdapter.BindEnd();
                    }

                Get_DataMarket_Api();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //Get Data Market Using API
        public async void Get_DataMarket_Api(string offset = "")
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Activity.RunOnUiThread(() => { swipeRefreshLayout.Refreshing = false; });
                    Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection),
                        ToastLength.Short).Show();
                }
                else
                {
                    var (Api_status, Respond) = await Client.Market.Get_Products("", "35", offset);
                    if (Api_status == 200)
                    {
                        if (Respond is Get_Products_Object result)
                            Activity.RunOnUiThread(() =>
                            {
                                if (result.products.Length <= 0)
                                {
                                    if (swipeRefreshLayout.Refreshing)
                                        swipeRefreshLayout.Refreshing = false;
                                }
                                else if (result.products.Length > 0)
                                {
                                    if (MMarketAdapter.MarketList.Count <= 0)
                                    {
                                        MMarketAdapter.MarketList =
                                            new ObservableCollection<Get_Products_Object.Product>(result.products);
                                        MMarketAdapter.BindEnd();

                                        var animation = AnimationUtils.LoadAnimation(Activity,
                                            Resource.Animation.slideUpAnim);
                                        MainRecyclerView.StartAnimation(animation);
                                    }
                                    else
                                    {
                                        //Bring new item
                                        //var exceptList = result.products?.ToList().Except(MMarketAdapter?.MarketList).ToList();

                                        var listnew = result.products?.Where(c =>
                                            !MMarketAdapter.MarketList.Select(fc => fc.id).Contains(c.id)).ToList();
                                        if (listnew.Count > 0)
                                        {
                                            var lastCountItem = MMarketAdapter.ItemCount;

                                            //Results differ
                                            Classes.AddRange(MMarketAdapter.MarketList, listnew);
                                            MMarketAdapter.NotifyItemRangeInserted(lastCountItem, listnew.Count);
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
                            //Toast.MakeText(this.Activity, errortext, ToastLength.Short).Show();

                            if (errortext.Contains("Invalid or expired access_token"))
                                API_Request.Logout(Activity);
                        }
                    }
                    else if (Api_status == 404)
                    {
                        var error = Respond.ToString();
                        //Toast.MakeText(this.Activity, error, ToastLength.Short).Show();
                    }
                }

                //Show Empty Page >> 
                //===============================================================
                Activity.RunOnUiThread(() =>
                {
                    swipeRefreshLayout.Refreshing = false;

                    if (MMarketAdapter.MarketList.Count > 0)
                    {
                        if (MainRecyclerView.Visibility == ViewStates.Gone)
                            MainRecyclerView.Visibility = ViewStates.Visible;

                        if (Market_Empty.Visibility == ViewStates.Visible)
                            Market_Empty.Visibility = ViewStates.Gone;
                    }
                    else if (MMarketAdapter.MarketList.Count <= 0)
                    {
                        MainRecyclerView.Visibility = ViewStates.Gone;
                        Market_Empty.Visibility = ViewStates.Visible;
                    }

                    //Set Event Scroll
                    if (MarketMainScrolEvent == null)
                    {
                        var xamarinRecyclerViewOnScrollListener =
                            new XamarinRecyclerViewOnScrollListener(mLayoutManager, swipeRefreshLayout);
                        MarketMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                        MarketMainScrolEvent.LoadMoreEvent += Market_FragmentOnScroll_OnLoadMoreEvent;
                        MainRecyclerView.AddOnScrollListener(MarketMainScrolEvent);
                        MainRecyclerView.AddOnScrollListener(new ScrollDownDetector());
                    }
                    else
                    {
                        MarketMainScrolEvent.IsLoading = false;
                    }
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_DataMarket_Api(offset);
            }
        }

        //Event Add New Product  >> CreateProduct_Activity
        private void BtnAddProductOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var intent = new Intent(Context, typeof(CreateProduct_Activity));
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Go to see data Product >> Open ProductView_Activity
        private void MMarketAdapter_OnItemClick(object sender, MarketAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var Position = adapterClickEvents.Position;
                if (Position >= 0)
                {
                    var item = MMarketAdapter.GetItem(Position);
                    if (item != null)
                    {
                        var Int = new Intent(Context, typeof(ProductView_Activity));
                        Int.PutExtra("ProductView", JsonConvert.SerializeObject(item));
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
        private void Market_SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                MMarketAdapter.Clear();
                Get_DataMarket_Api("");
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
                if (MMarketAdapter.MarketList.Count > 0)
                    Classes.ListChachedData_Product = MMarketAdapter.MarketList;

                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region  Variables Basic

        public LinearLayout Market_Empty;
        public RecyclerView MainRecyclerView;

        public GridLayoutManager mLayoutManager;

        public MarketAdapter MMarketAdapter;

        private TextView IconEmpty;
        private Button Btn_AddProduct;

        public SwipeRefreshLayout swipeRefreshLayout;

        public XamarinRecyclerViewOnScrollListener MarketMainScrolEvent;
        private string Lastproductid = "";

        #endregion

        #region Scroll

        //Event Scroll #Market
        private void Market_FragmentOnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = MMarketAdapter.MarketList.LastOrDefault();
                if (item != null) Lastproductid = item.id;

                if (Lastproductid != "")
                    try
                    {
                        //Run Load More Api
                        Task.Run(() => { Get_DataMarket_Api(Lastproductid); });
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
            public bool ReadyForAction;

            public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
            {
                try
                {
                    base.OnScrollStateChanged(recyclerView, newState);

                    if (newState == RecyclerView.ScrollStateDragging)
                        ImageService.Instance.SetPauseWork(true);
                    else if (newState == RecyclerView.ScrollStateIdle)
                        ImageService.Instance.SetPauseWork(false);
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

                    if (ReadyForAction && dy > 0)
                    {
                        //The scroll direction is down
                        ReadyForAction = false;
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