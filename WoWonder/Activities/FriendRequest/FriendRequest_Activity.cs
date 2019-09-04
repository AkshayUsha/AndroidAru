using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
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
using WoWonder.Activities.DialogUser;
using WoWonder.Activities.Tabbes;
using WoWonder.Activities.Tabbes.Adapters;
using WoWonder.Helpers;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.FriendRequest
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class FriendRequest_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);
                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.FriendRequest_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.FriendRequest_Layout);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.Searchtoolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetString(Resource.String.Lbl_FriendRequest);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                //Get values
                FriendRequestRecyler = FindViewById<RecyclerView>(Resource.Id.FriendRequestRecylerview);
                FriendRequest_Empty = FindViewById<LinearLayout>(Resource.Id.FriendRequest_LinerEmpty);
                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                Icon_FriendRequest = FindViewById<TextView>(Resource.Id.FriendRequest_icon);

                FriendRequestRecyler.Visibility = ViewStates.Visible;
                FriendRequest_Empty.Visibility = ViewStates.Gone;

                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                IMethods.Set_TextViewIcon("1", Icon_FriendRequest, IonIcons_Fonts.PersonAdd);
                Icon_FriendRequest.SetTextColor(Color.ParseColor(Settings.MainColor));

                //Set Adapter
                mLayoutManager = new LinearLayoutManager(this);
                FriendRequestRecyler.SetLayoutManager(mLayoutManager);
                Notifications_Fragment.FriendRequestsAdapter.ItemClick += FriendRequestsAdapter_OnItemClick;
                FriendRequestRecyler.SetAdapter(Notifications_Fragment.FriendRequestsAdapter);
                Notifications_Fragment.FriendRequestsAdapter.BindEnd();

                //Show Ads
                mAdView = FindViewById<AdView>(Resource.Id.adView);
                if (Settings.Show_ADMOB_Banner)
                {
                    mAdView.Visibility = ViewStates.Visible;
                    var adRequest = new AdRequest.Builder().Build();
                    mAdView.LoadAd(adRequest);
                }
                else
                {
                    mAdView.Pause();
                    mAdView.Visibility = ViewStates.Invisible;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void FriendRequestsAdapter_OnItemClick(object sender,
            FriendRequests_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var Position = adapterClickEvents.Position;
                if (Position >= 0)
                {
                    var item = Notifications_Fragment.FriendRequestsAdapter.GetItem(Position);
                    if (item != null)
                    {
                        //Pull up dialog
                        var transaction = FragmentManager.BeginTransaction();
                        var UserDialog = new Dialog_FriendRequests(item.user_id, item);
                        UserDialog.Show(transaction, "dialog fragment");
                        UserDialog._OnUserUpComplete += UserDialogOnOnUserUpComplete;
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void UserDialogOnOnUserUpComplete(object sender,
            Dialog_FriendRequests.OnFriendRequestsUp_EventArgs onFriendRequestsUpEventArgs)
        {
            try
            {
                var th = new Thread(ActLikeARequest);
                th.Start();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void ActLikeARequest()
        {
            var x = Resource.Animation.slide_right;
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
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Variables Basic

        public static RecyclerView FriendRequestRecyler;
        private RecyclerView.LayoutManager mLayoutManager;

        public static LinearLayout FriendRequest_Empty;
        private TextView Icon_FriendRequest;

        public SwipeRefreshLayout swipeRefreshLayout;

        private AdView mAdView;

        #endregion
    }
}