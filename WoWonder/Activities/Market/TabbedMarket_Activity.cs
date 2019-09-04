using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Adapters;
using WoWonder.Helpers;

namespace WoWonder.Activities.Market
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class TabbedMarket_Activity : AppCompatActivity
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
                    Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
                    Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                }

                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);


                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.EventMain_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.EventMain_Layout);

                var toolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolBar != null)
                {
                    if (Settings.Show_Toolbar_Market)
                    {
                        toolBar.Title = GetText(Resource.String.Lbl_Market);

                        SetSupportActionBar(toolBar);
                        SupportActionBar.SetDisplayShowCustomEnabled(true);
                        SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                        SupportActionBar.SetHomeButtonEnabled(true);
                        SupportActionBar.SetDisplayShowHomeEnabled(true);
                    }
                    else
                    {
                        toolBar.Title = " ";
                        SetSupportActionBar(toolBar);
                    }
                }

                viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
                tab_Layout = FindViewById<TabLayout>(Resource.Id.tabs);
                FloatingActionButtonView = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonView);

                viewPager.OffscreenPageLimit = 2;
                SetUpViewPager(viewPager);
                tab_Layout.SetupWithViewPager(viewPager);
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
                //ImageService.Instance.SetExitTasksEarly(false);
                //Add Event
                FloatingActionButtonView.Click += FloatingActionButtonViewOnClick;
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
                //ImageService.Instance.SetExitTasksEarly(true);
                //Close Event
                FloatingActionButtonView.Click -= FloatingActionButtonViewOnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void FloatingActionButtonViewOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                StartActivity(new Intent(this, typeof(CreateProduct_Activity)));
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
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
                
                base.OnDestroy();
                ImageService.Instance.InvalidateMemoryCache();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }
        protected override void JavaFinalize()
        {
            try
            {
                base.JavaFinalize();
                ImageService.Instance.InvalidateCacheEntryAsync("", FFImageLoading.Cache.CacheType.Memory);
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }



        #region Variables Basic

        public ViewPager viewPager;

        public Market_Fragment Market_Fragment_Tab;
        public static MyProducts_Fragment MyProducts_Fragment_Tab;
        public TabLayout tab_Layout;

        private FloatingActionButton FloatingActionButtonView;

        #endregion Variables Basic

        #region Set Tab

        private void SetUpViewPager(ViewPager viewPager)
        {
            try
            {
                MyProducts_Fragment_Tab = new MyProducts_Fragment();
                Market_Fragment_Tab = new Market_Fragment();

                var adapter = new Main_Tab_Adapter(SupportFragmentManager);
                adapter.AddFragment(Market_Fragment_Tab, GetText(Resource.String.Lbl_Market));
                adapter.AddFragment(MyProducts_Fragment_Tab, GetText(Resource.String.Lbl_MyProducts));

                viewPager.CurrentItem = 2;

                viewPager.PageScrolled += ViewPager_PageScrolled;
                viewPager.Adapter = adapter;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        private void ViewPager_PageScrolled(object sender, ViewPager.PageScrolledEventArgs page)
        {
            try
            {
                var p = page.Position;
                if (p == 0) // Market
                    Market_Fragment_Tab.Get_DataMarket_Api();
                else if (p == 1) // My Products
                    MyProducts_Fragment_Tab.Get_MyDataMarket_Api();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #endregion Set Tab
    }
}