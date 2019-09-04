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
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace WoWonder.Activities.Events
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class EventMain_Activity : AppCompatActivity
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
                    toolBar.Title = GetText(Resource.String.Lbl_Events);

                    SetSupportActionBar(toolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                   
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

                //Add Event
                FloatingActionButtonView.Click += BtnCreateEventsOnClick;
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
                FloatingActionButtonView.Click -= BtnCreateEventsOnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //Event Create New Event 
        private void BtnCreateEventsOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                StartActivity(new Intent(this, typeof(CreateEvent_Activity)));
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
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #region Variables Basic

        public ViewPager viewPager;

        public FragmentManager frgmanager;
        public Event_Fragment Event_Fragment_Tab;
        public MyEvent_Fragment MyEvent_Fragment_Tab;
        public TabLayout tab_Layout;

        private FloatingActionButton FloatingActionButtonView;

        #endregion
         
        #region Set Tap

        private void SetUpViewPager(ViewPager viewPager)
        {
            try
            {
                Event_Fragment_Tab = new Event_Fragment();
                MyEvent_Fragment_Tab = new MyEvent_Fragment();

                var adapter = new Main_Tab_Adapter(SupportFragmentManager);
                adapter.AddFragment(Event_Fragment_Tab, GetText(Resource.String.Lbl_All_Events));
                adapter.AddFragment(MyEvent_Fragment_Tab, GetText(Resource.String.Lbl_My_Events));

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
                if (p == 0)
                    Event_Fragment_Tab.Get_Event_Api();
                else if (p == 1) MyEvent_Fragment_Tab.Get_MyEvent_Api();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #endregion
    }
}