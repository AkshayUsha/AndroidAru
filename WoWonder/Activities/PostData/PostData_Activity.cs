using System;
using System.Collections.ObjectModel;
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
using WoWonder.Activities.MyProfile;
using WoWonder.Activities.PostData.Adapters;
using WoWonder.Activities.UserProfile;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.PostData
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class PostData_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.PostData_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.PostData_Layout);

                var dataid = Intent.GetStringExtra("PostId") ?? "Data not available";
                if (dataid != "Data not available" && !string.IsNullOrEmpty(dataid)) IdPost = dataid;

                var datatype = Intent.GetStringExtra("PostType") ?? "Data not available";
                if (datatype != "Data not available" && !string.IsNullOrEmpty(dataid)) TypePost = datatype;

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    if (TypePost == "post_likes")
                        ToolBar.Title = GetText(Resource.String.Lbl_PostLikes);
                    else if (TypePost == "post_wonders") ToolBar.Title = GetText(Resource.String.Lbl_PostWonders);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                DataPost_Recycler = FindViewById<RecyclerView>(Resource.Id.DataPostRecylerview);
                DataPost_Empty = FindViewById<LinearLayout>(Resource.Id.DataPost_LinerEmpty);

                mLayoutManager = new LinearLayoutManager(this);
                DataPost_Recycler.SetLayoutManager(mLayoutManager);

                LikedUsersAdapter = new LikedUsers_Adapter(this);
                LikedUsersAdapter.mPostlikedList = new ObservableCollection<Get_Post_Data_Object.PostLikedUsers>();

                WonderedAdapter = new Wondered_Adapter(this);
                WonderedAdapter.mPostWonderedList = new ObservableCollection<Get_Post_Data_Object.PostWonderedUsers>();

                Icon_DataPost = FindViewById<TextView>(Resource.Id.DataPost_icon);
                Txt_Empty = FindViewById<TextView>(Resource.Id.Txt_LabelEmpty);

                DataPost_Recycler.Visibility = ViewStates.Visible;
                DataPost_Empty.Visibility = ViewStates.Gone;

                Get_PostData_API();
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
                LikedUsersAdapter.ItemClick += LikedUsersAdapter_OnItemClick;
                WonderedAdapter.ItemClick += WonderedAdapter_OnItemClick;
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
                LikedUsersAdapter.ItemClick -= LikedUsersAdapter_OnItemClick;
                WonderedAdapter.ItemClick -= WonderedAdapter_OnItemClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Api
        public async void Get_PostData_API()
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)
                        .Show();
                    DataPost_Recycler.Visibility = ViewStates.Visible;
                    DataPost_Empty.Visibility = ViewStates.Gone;
                }
                else
                {
                    if (TypePost == "post_likes")
                    {
                        var (Api_status, Respond) = await Client.Global.Get_Post_Data(IdPost, "post_liked_users");
                        if (Api_status == 200)
                        {
                            if (Respond is Get_Post_Data_Object result)
                                if (result.post_liked_users.Length > 0)
                                {
                                    DataPost_Recycler.SetAdapter(LikedUsersAdapter);

                                    LikedUsersAdapter.mPostlikedList =
                                        new ObservableCollection<Get_Post_Data_Object.PostLikedUsers>(
                                            result.post_liked_users);
                                    LikedUsersAdapter.BindEnd();
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
                    else if (TypePost == "post_wonders")
                    {
                        var (Api_status, Respond) = await Client.Global.Get_Post_Data(IdPost, "post_wondered_users");
                        if (Api_status == 200)
                        {
                            if (Respond is Get_Post_Data_Object result)
                                if (result.post_wondered_users.Length > 0)
                                {
                                    DataPost_Recycler.SetAdapter(WonderedAdapter);

                                    WonderedAdapter.mPostWonderedList =
                                        new ObservableCollection<Get_Post_Data_Object.PostWonderedUsers>(
                                            result.post_wondered_users);
                                    WonderedAdapter.BindEnd();
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

                    if (TypePost == "post_likes")
                    {
                        //Show Empty Page
                        if (LikedUsersAdapter.mPostlikedList.Count > 0)
                        {
                            DataPost_Recycler.Visibility = ViewStates.Visible;
                            DataPost_Empty.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            DataPost_Recycler.Visibility = ViewStates.Gone;
                            DataPost_Empty.Visibility = ViewStates.Visible;

                            IMethods.Set_TextViewIcon("1", Icon_DataPost, IonIcons_Fonts.Thumbsup);

                            Txt_Empty.Text = GetText(Resource.String.Lbl_Empty_PostLikes);
                        }
                    }
                    else if (TypePost == "post_wonders")
                    {
                        //Show Empty Page
                        if (LikedUsersAdapter.mPostlikedList.Count > 0)
                        {
                            DataPost_Recycler.Visibility = ViewStates.Visible;
                            DataPost_Empty.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            DataPost_Recycler.Visibility = ViewStates.Gone;
                            DataPost_Empty.Visibility = ViewStates.Visible;

                            IMethods.Set_TextViewIcon("1", Icon_DataPost, IonIcons_Fonts.IosInformationOutline);

                            Txt_Empty.Text = GetText(Resource.String.Lbl_Empty_PostWonders);
                        }
                    }

                    swipeRefreshLayout.Refreshing = false;
                    swipeRefreshLayout.Enabled = false;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event #Liked Users >> Open User Profile 
        private void LikedUsersAdapter_OnItemClick(object sender, LikedUsers_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = LikedUsersAdapter.GetItem(position);
                    if (item != null)
                    {
                        if (UserDetails.User_id == item.UserId)
                        {
                            var Int = new Intent(ApplicationContext, typeof(MyProfile_Activity));
                            Int.PutExtra("UserId", item.UserId);
                            StartActivity(Int);
                        }
                        else
                        {
                            var Int = new Intent(this, typeof(User_Profile_Activity));
                            Int.PutExtra("UserId", item.UserId);
                            Int.PutExtra("UserType", "LikedUsers");
                            Int.PutExtra("UserItem", JsonConvert.SerializeObject(item));
                            StartActivity(Int);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event #Wondered Users >> Open User Profile 
        private void WonderedAdapter_OnItemClick(object sender, Wondered_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = WonderedAdapter.GetItem(position);
                    if (item != null)
                    {
                        if (UserDetails.User_id == item.UserId)
                        {
                            var Int = new Intent(ApplicationContext, typeof(MyProfile_Activity));
                            Int.PutExtra("UserId", item.UserId);
                            StartActivity(Int);
                        }
                        else
                        {
                            var Int = new Intent(this, typeof(User_Profile_Activity));
                            Int.PutExtra("UserId", item.UserId);
                            Int.PutExtra("UserType", "WonderedUsers");
                            Int.PutExtra("UserItem", JsonConvert.SerializeObject(item));
                            StartActivity(Int);
                        }
                    }
                }
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
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Variables Basic

        private SwipeRefreshLayout swipeRefreshLayout;

        private RecyclerView.LayoutManager mLayoutManager;

        private RecyclerView DataPost_Recycler;
        private LinearLayout DataPost_Empty;

        private LikedUsers_Adapter LikedUsersAdapter;
        private Wondered_Adapter WonderedAdapter;

        private TextView Icon_DataPost;
        private TextView Txt_Empty;

        public string IdPost = "";
        public string TypePost = "";

        #endregion
    }
}