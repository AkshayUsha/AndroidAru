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
    public class MyProducts_Fragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                var view = MyContextWrapper.GetContentView(Context, Settings.Lang, Resource.Layout.Market_Fragment);
                if (view == null) view = inflater.Inflate(Resource.Layout.Market_Fragment, container, false);

                MainRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.Recyler);
                MyProducts_Empty = view.FindViewById<LinearLayout>(Resource.Id.Market_LinerEmpty);

                IconEmpty = view.FindViewById<TextView>(Resource.Id.Market_icon);
                IMethods.Set_TextViewIcon("2", IconEmpty, "\uf07a");

                Btn_AddProduct = view.FindViewById<Button>(Resource.Id.AddProduct_Button);

                swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = true;

                MainRecyclerView.Visibility = ViewStates.Visible;
                MyProducts_Empty.Visibility = ViewStates.Gone;

                MMyProductsAdapter = new MyProducts_Adapter(Activity);
                mLayoutManager = new GridLayoutManager(Activity, 2);
                MainRecyclerView.SetLayoutManager(mLayoutManager);
                MainRecyclerView.AddItemDecoration(new GridSpacingItemDecoration(2, 10, true));
                var animation = AnimationUtils.LoadAnimation(Activity, Resource.Animation.slideUpAnim);
                MainRecyclerView.StartAnimation(animation);
                MMyProductsAdapter.MyProductsList = new ObservableCollection<Get_Products_Object.Product>();
                MainRecyclerView.SetAdapter(MMyProductsAdapter);

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
                MMyProductsAdapter.ItemClick += MMyProductsAdapter_OnItemClick;
                swipeRefreshLayout.Refresh += MyProducts_SwipeRefreshLayoutOnRefresh;
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
                MMyProductsAdapter.ItemClick -= MMyProductsAdapter_OnItemClick;
                swipeRefreshLayout.Refresh -= MyProducts_SwipeRefreshLayoutOnRefresh;
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
                if (MMyProductsAdapter != null)
                    if (Classes.ListChachedData_MyProduct.Count > 0)
                    {
                        MMyProductsAdapter.MyProductsList = Classes.ListChachedData_MyProduct;
                        MMyProductsAdapter.BindEnd();
                    }

                Get_MyDataMarket_Api();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //Get Data Market Using API >> My Products
        public async void Get_MyDataMarket_Api(string offset = "")
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection),
                        ToastLength.Short).Show();
                }
                else
                {
                    var (Api_status, Respond) = await Client.Market.Get_Products(UserDetails.User_id, "35", offset);
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
                                    if (MMyProductsAdapter.MyProductsList.Count <= 0)
                                    {
                                        MMyProductsAdapter.MyProductsList =
                                            new ObservableCollection<Get_Products_Object.Product>(result.products);
                                        MMyProductsAdapter.BindEnd();

                                        var animation = AnimationUtils.LoadAnimation(Activity,
                                            Resource.Animation.slideUpAnim);
                                        MainRecyclerView.StartAnimation(animation);
                                    }
                                    else
                                    {
                                        //Bring new item
                                        var listnew = result.products?.Where(c =>
                                                !MMyProductsAdapter.MyProductsList.Select(fc => fc.id).Contains(c.id))
                                            .ToList();
                                        if (listnew.Count > 0)
                                        {
                                            var lastCountItem = MMyProductsAdapter.ItemCount;

                                            //Results differ
                                            Classes.AddRange(MMyProductsAdapter.MyProductsList, listnew);
                                            MMyProductsAdapter.NotifyItemRangeInserted(lastCountItem, listnew.Count);
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
                    if (MMyProductsAdapter.MyProductsList.Count > 0)
                    {
                        if (MainRecyclerView.Visibility == ViewStates.Gone)
                            MainRecyclerView.Visibility = ViewStates.Visible;

                        if (MyProducts_Empty.Visibility == ViewStates.Visible)
                            MyProducts_Empty.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        MainRecyclerView.Visibility = ViewStates.Gone;
                        MyProducts_Empty.Visibility = ViewStates.Visible;
                    }

                    swipeRefreshLayout.Refreshing = false;

                    //Set Event Scroll
                    if (MarketMyProductScrolEvent == null)
                    {
                        var xamarinRecyclerViewOnScrollListener =
                            new XamarinRecyclerViewOnScrollListener(mLayoutManager, swipeRefreshLayout);
                        MarketMyProductScrolEvent = xamarinRecyclerViewOnScrollListener;
                        MarketMyProductScrolEvent.LoadMoreEvent += MyProductsOnScroll_OnLoadMoreEvent;
                        MainRecyclerView.AddOnScrollListener(MarketMyProductScrolEvent);
                        MainRecyclerView.AddOnScrollListener(new ScrollDownDetector());
                    }
                    else
                    {
                        MarketMyProductScrolEvent.IsLoading = false;
                    }
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_MyDataMarket_Api(offset);
            }
        }

        //Event Go to see data Product >> Open ProductView_Activity
        private void MMyProductsAdapter_OnItemClick(object sender, MyProducts_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var Position = adapterClickEvents.Position;
                if (Position >= 0)
                {
                    var item = MMyProductsAdapter.GetItem(Position);
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

        //Event Refresh Data Page
        private void MyProducts_SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                MMyProductsAdapter.Clear();
                Get_MyDataMarket_Api();
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
                if (MMyProductsAdapter.MyProductsList.Count > 0)
                    Classes.ListChachedData_MyProduct = MMyProductsAdapter.MyProductsList;

                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region  Variables Basic

        public LinearLayout MyProducts_Empty;
        public RecyclerView MainRecyclerView;
        public GridLayoutManager mLayoutManager;

        public MyProducts_Adapter MMyProductsAdapter;

        private TextView IconEmpty;
        private Button Btn_AddProduct;

        public SwipeRefreshLayout swipeRefreshLayout;

        public XamarinRecyclerViewOnScrollListener MarketMyProductScrolEvent;
        private string LastMyproductid = "";

        #endregion

        #region Scroll

        //Event Scroll #My_Product
        private void MyProductsOnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = MMyProductsAdapter.MyProductsList.LastOrDefault();
                if (item != null) LastMyproductid = item.id;

                if (LastMyproductid != "")
                    try
                    {
                        //Run Load More Api
                        Task.Run(() => { Get_MyDataMarket_Api(LastMyproductid); });
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