using System;
using System.Collections.ObjectModel;
using System.Linq;
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
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.BlockedUsers.Adapters;
using WoWonder.Activities.DialogUser;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.BlockedUsers
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class BlockedUsers_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.BlockedUsers_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.BlockedUsers_Layout);

                //Get values
                BlockedUsersRecyler = FindViewById<RecyclerView>(Resource.Id.BlockRecylerview);
                BlockedUsers_Empty = FindViewById<LinearLayout>(Resource.Id.Block_LinerEmpty);
                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                Icon_blockedusers = FindViewById<TextView>(Resource.Id.blockedusers_icon);

                BlockedUsersRecyler.Visibility = ViewStates.Visible;
                BlockedUsers_Empty.Visibility = ViewStates.Gone;

                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                IMethods.Set_TextViewIcon("2", Icon_blockedusers, "\uf235");
                Icon_blockedusers.SetTextColor(Color.ParseColor(Settings.MainColor));

                var ToolBar = FindViewById<Toolbar>(Resource.Id.Searchtoolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetString(Resource.String.Lbl_BlockedUsers);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                //Set Adapter
                mLayoutManager = new LinearLayoutManager(this);
                BlockedUsersRecyler.SetLayoutManager(mLayoutManager);
                mAdapter = new BlockedUsers_Adapter(this);
                BlockedUsersRecyler.SetAdapter(mAdapter);

                Get_BlockedList();

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
                mAdapter.ItemClick += MAdapterOnItemClick;
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
                mAdapter.ItemClick -= MAdapterOnItemClick;
                swipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        public void Get_BlockedList()
        {
            try
            {
                swipeRefreshLayout.Refreshing = true;

                //Get All User Block From Database 
                var dbDatabase = new SqLiteDatabase();
                var localList = dbDatabase.Get_Blocked_Users();
                if (localList != null)
                {
                    mAdapter.mBlockedUsersList =
                        new ObservableCollection<Get_Blocked_Users_Object.Blocked_Users>(localList);
                    mAdapter.BindEnd();
                }

                dbDatabase.Dispose();

                Get_BlockedList_API();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public async void Get_BlockedList_API()
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
                    var (Api_status, Respond) = await Client.Global.Get_Blocked_Users();
                    if (Api_status == 200)
                    {
                        if (Respond is Get_Blocked_Users_Object result)
                        {
                            if (result.blocked_users.Length <= 0)
                            {
                                swipeRefreshLayout.Refreshing = false;
                                return;
                            }

                            var dbDatabase = new SqLiteDatabase();
                            if (mAdapter.mBlockedUsersList.Count > 0)
                            {
                                //Bring new users
                                var listnew = result.blocked_users.Where(c =>
                                    !mAdapter.mBlockedUsersList.Select(fc => fc.user_id).Contains(c.user_id)).ToList();
                                if (listnew.Count > 0)
                                {
                                    //Results differ
                                    Classes.AddRange(mAdapter.mBlockedUsersList, listnew);

                                    //Insert Or Replace Just New Data To Database
                                    dbDatabase.Insert_Or_Replace_BlockedUsersTable(
                                        new ObservableCollection<Get_Blocked_Users_Object.Blocked_Users>(listnew));
                                }
                            }
                            else
                            {
                                mAdapter.mBlockedUsersList =
                                    new ObservableCollection<Get_Blocked_Users_Object.Blocked_Users>(
                                        result.blocked_users);
                                mAdapter.BindEnd();

                                //Insert Or Replace Data To Database
                                dbDatabase.Insert_Or_Replace_BlockedUsersTable(mAdapter.mBlockedUsersList);
                            }

                            dbDatabase.Dispose();
                        }
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
                if (mAdapter.mBlockedUsersList.Count > 0)
                {
                    BlockedUsersRecyler.Visibility = ViewStates.Visible;
                    BlockedUsers_Empty.Visibility = ViewStates.Gone;
                }
                else
                {
                    BlockedUsersRecyler.Visibility = ViewStates.Gone;
                    BlockedUsers_Empty.Visibility = ViewStates.Visible;
                }

                swipeRefreshLayout.Refreshing = false;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_BlockedList_API();
            }
        }

        private void MAdapterOnItemClick(object sender, BlockedUsers_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var Position = adapterClickEvents.Position;
                if (Position >= 0)
                {
                    var item = mAdapter.GetItem(Position);
                    if (item != null)
                    {
                        //Pull up dialog
                        var transaction = FragmentManager.BeginTransaction();
                        var UserDialog = new Dialog_BlockUser(item.user_id, item);
                        UserDialog.Show(transaction, "dialog fragment");
                        UserDialog._OnBlockUserUpComplete += UserDialogOnOnBlockUserUpComplete;
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
                mAdapter.Clear();
                Get_BlockedList_API();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        private void UserDialogOnOnBlockUserUpComplete(object sender,
            Dialog_BlockUser.OnBlockUserUp_EventArgs onBlockUserUpEventArgs)
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

        private RecyclerView BlockedUsersRecyler;
        private RecyclerView.LayoutManager mLayoutManager;
        public static BlockedUsers_Adapter mAdapter;

        private LinearLayout BlockedUsers_Empty;
        private TextView Icon_blockedusers;

        public SwipeRefreshLayout swipeRefreshLayout;

        #endregion
    }
}