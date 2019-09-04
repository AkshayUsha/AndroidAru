using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.BlockedUsers;
using WoWonder.Activities.InviteFriends;
using WoWonder.Activities.MyProfile;
using WoWonder.Activities.userProfile.Adapters;
using WoWonder.Activities.UserContacts.Adapters;
using WoWonder.Activities.UserProfile;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.UserContacts
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class UserContacts_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);
                 
                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Contacts_Main_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Contacts_Main_Layout);

                var ContactsType = Intent.GetStringExtra("ContactsType") ?? "Data not available";
                if (ContactsType != "Data not available" && !string.IsNullOrEmpty(ContactsType))
                    Type_Contacts = ContactsType;

                ContactsRecyler = FindViewById<RecyclerView>(Resource.Id.Recyler);
                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                Contacts_Empty = FindViewById<LinearLayout>(Resource.Id.Contacts_LinerEmpty);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    if (Settings.ConnectivitySystem == "1") // Following
                        ToolBar.Title = GetText(Resource.String.Lbl_Followers);
                    else // Friend
                        ToolBar.Title = GetText(Resource.String.Lbl_Friends);
                     
                    ToolBar.SetSubtitleTextColor(Color.White);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);

                Icon_UserContacts = FindViewById<TextView>(Resource.Id.Contactsusers_icon);
                Btn_SearchRandom = FindViewById<Button>(Resource.Id.SearchRandom_Button);
                Btn_SearchRandom.Visibility = ViewStates.Gone;

                IMethods.Set_TextViewIcon("1", Icon_UserContacts, IonIcons_Fonts.IosPeopleOutline);
                Icon_UserContacts.SetTextColor(Color.ParseColor(Settings.MainColor));

                Contacts_Empty.Visibility = ViewStates.Gone;
                ContactsRecyler.Visibility = ViewStates.Visible;

                Get_UsersContact();

                AdsGoogle.Ad_Interstitial(this);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void Get_UsersContact()
        {
            try
            {
                //swipeRefreshLayout.Refreshing = true;
                //swipeRefreshLayout.Enabled = true;

                if (Type_Contacts == "UserProfile")
                {
                    var list = new JavaList<Get_User_Data_Object.Followers>(UserFriendsAdapter.mAllUserFriendsList?.OrderBy(a => a.name));
                    if (list?.Count > 0)
                    {
                        //Set Adapter
                        ContactsLayoutManager = new LinearLayoutManager(this);
                        ContactsRecyler.SetLayoutManager(ContactsLayoutManager);
                        UserContactsAdapter = new UserContacts_Adapter(this, list, ContactsRecyler);
                        ContactsRecyler.SetAdapter(UserContactsAdapter);
                        UserContactsAdapter.ItemClick += ContactAdapter_OnItemClick;
                        UserContactsAdapter.BindEnd();

                        if (UserContactsAdapter.mUsersContactsList.Count > 0)
                        {
                            Contacts_Empty.Visibility = ViewStates.Gone;
                            ContactsRecyler.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            Contacts_Empty.Visibility = ViewStates.Visible;
                            ContactsRecyler.Visibility = ViewStates.Gone;
                        }
                    }
                    else
                    {
                        Contacts_Empty.Visibility = ViewStates.Visible;
                        ContactsRecyler.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    Contacts_Empty.Visibility = ViewStates.Visible;
                    ContactsRecyler.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void ContactAdapter_OnItemClick(object sender, UserContacts_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var Position = adapterClickEvents.Position;
                if (Position >= 0)
                {
                    var item = UserContactsAdapter.GetItem(Position);
                    if (item != null)
                    {
                        Intent Int;
                        if (item.user_id != UserDetails.User_id)
                        {
                            Int = new Intent(this, typeof(User_Profile_Activity));
                            Int.PutExtra("UserId", item.user_id);
                            Int.PutExtra("UserType", "MyFollowers");
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

        public UserContacts_Adapter UserContactsAdapter;
        private RecyclerView ContactsRecyler;
        private RecyclerView.LayoutManager ContactsLayoutManager;
        private SwipeRefreshLayout swipeRefreshLayout;
        private LinearLayout Contacts_Empty;
        private Button Btn_SearchRandom;
        private TextView Icon_UserContacts;

        public SearchView _SearchView;

        private Get_User_Data_Object.Followers _follower;
        private string Type_Contacts = "";
        public static string Aftercontact = "0";

        #endregion

        #region Menu 

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.User_Search_Menu, menu);

            var item = menu.FindItem(Resource.Id.searchUserBar);
            var searchItem = MenuItemCompat.GetActionView(item);

            _SearchView = searchItem.JavaCast<SearchView>();
            _SearchView.SetIconifiedByDefault(true);
            _SearchView.QueryTextChange += _SearchView_OnTextChange;
            _SearchView.QueryTextSubmit += _SearchView_OnTextSubmit;

            var refresh = menu.FindItem(Resource.Id.menue_refresh);
            refresh.SetVisible(false);

            return base.OnCreateOptionsMenu(menu);
        }

        private void _SearchView_OnTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            try
            {
                UserContactsAdapter.Filter.InvokeFilter(e.Query);
                e.Handled = true;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        private void _SearchView_OnTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            try
            {
                UserContactsAdapter.Filter.InvokeFilter(e.NewText);
                ContactsRecyler.Invalidate();
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

                case Resource.Id.menue_invite_friend:
                    invite_friend_OnClick();
                    break;

                //case Resource.Id.menue_refresh:
                //    refresh_OnClick();
                //    break;

                case Resource.Id.menue_blockList:
                    blockList_OnClick();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void blockList_OnClick()
        {
            try
            {
                var Intent = new Intent(this, typeof(BlockedUsers_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void invite_friend_OnClick()
        {
            try
            {
                var Intent = new Intent(this, typeof(Invite_Friends_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion
    }
}