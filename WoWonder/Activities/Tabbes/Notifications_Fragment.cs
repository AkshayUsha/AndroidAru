using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.Communities.Groups;
using WoWonder.Activities.Communities.Pages;
using WoWonder.Activities.MyProfile;
using WoWonder.Activities.Tabbes.Adapters;
using WoWonder.Activities.UserProfile;
using WoWonder.Activities.UsersPages;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Requests;
using Fragment = Android.Support.V4.App.Fragment;

namespace WoWonder.Activities.Tabbes
{
    public class Notifications_Fragment : Fragment
    {
        public Tabbed_Main_Activity ContextTab;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                var view = MyContextWrapper.GetContentView(Context, Settings.Lang,
                    Resource.Layout.Tab_Notifications_Layout);
                if (view == null) view = inflater.Inflate(Resource.Layout.Tab_Notifications_Layout, container, false);

                Layout_main = (LinearLayout) view.FindViewById(Resource.Id.main);

                Notifcation_Recyler = (RecyclerView) view.FindViewById(Resource.Id.NotifcationRecyler);
                notifications_Empty = (LinearLayout) view.FindViewById(Resource.Id.notifications_LinerEmpty);

                Notify_LayoutManager = new LinearLayoutManager(Activity);
                Notifcation_Recyler.SetLayoutManager(Notify_LayoutManager);
                NotifyAdapter = new Notifications_Adapter(Context);
                NotifyAdapter.mNotificationsList = new ObservableCollection<Get_General_Data_Object.Notification>();
                Notifcation_Recyler.SetAdapter(NotifyAdapter);

                Notifcation_Recyler.Visibility = ViewStates.Visible;
                notifications_Empty.Visibility = ViewStates.Gone;

                //============================= Hash tag Users ==================================

                HashtagUserAdapter = new HashtagUser_Adapter(Context);
                HashtagUserAdapter.mHashtagList = new ObservableCollection<Get_General_Data_Object.Trending_Hashtag>();

               ContextTab = ((Tabbed_Main_Activity) Context);



                //============================= Promoted Pages ==================================

                ProPagesAdapter = new ProPages_Adapter(Context);
                ProPagesAdapter.mProPagesList = new ObservableCollection<Get_General_Data_Object.Promoted_Pages>();

                //============================= Requests Users ==================================

                FriendRequestsAdapter = new FriendRequests_Adapter(Context);
                FriendRequestsAdapter.mFriendRequestsList = new ObservableCollection<Get_General_Data_Object.Friend_Requests>();

                //===============================================================

                notificationsIcon = (TextView) view.FindViewById(Resource.Id.notifications_icon);
                IMethods.Set_TextViewIcon("1", notificationsIcon, IonIcons_Fonts.AndroidNotifications);

                swipeRefreshLayout = (SwipeRefreshLayout) view.FindViewById(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                if (!Settings.SetTabOnButton)
                {
                    var parasms = (RelativeLayout.LayoutParams) swipeRefreshLayout.LayoutParameters;
                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                        parasms.TopMargin = 80;
                    else
                        parasms.TopMargin = 120;

                    Notifcation_Recyler.LayoutParameters = parasms;
                    Notifcation_Recyler.SetPadding(0, 0, 0, 0);
                }

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
                NotifyAdapter.ItemClick += NotifyAdapter_OnItemClick;
                ContextTab.ProUsersAdapter.ItemClick += ProUsersAdapter_OnItemClick;
                ProPagesAdapter.ItemClick += ProPagesAdapter_OnItemClick;

                swipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public override void OnPause()
        {
            try
            {
                base.OnPause();

                //Close Event
                NotifyAdapter.ItemClick -= NotifyAdapter_OnItemClick;
                ContextTab.ProUsersAdapter.ItemClick -= ProUsersAdapter_OnItemClick;
                ProPagesAdapter.ItemClick -= ProPagesAdapter_OnItemClick;

                swipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Get General Data Using Api >> notifications , pro_users , promoted_pages , trending_hashtag
        public async Task<(string, string,string)> Get_GeneralData_Api(bool seenNotifications)
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    swipeRefreshLayout.Refreshing = false;
                    Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection),
                        ToastLength.Short).Show();
                }
                else
                {
                    // Get General Data : notifications , pro_users , promoted_pages , trending_hashtag
                    //==========================================
                    var (api_status, respond) = await Client.Global.Get_General_Data(seenNotifications, UserDetails.Device_ID);
                    if (api_status == 200)
                    {
                        if (respond is Get_General_Data_Object result)
                        {
                            this.Activity.RunOnUiThread(() =>
                            {
                                // Notifications
                                if (result.notifications.Length > 0)
                                {
                                    if (NotifyAdapter.mNotificationsList.Count > 0)
                                    {
                                        //Bring new users
                                        var listnew = result.notifications.Where(c => !NotifyAdapter.mNotificationsList.Select(fc => fc.notifier_id).Contains(c.notifier_id)).ToList();
                                        if (listnew.Count > 0)
                                        {
                                            //Results differ
                                            Classes.AddRange(NotifyAdapter.mNotificationsList, listnew);
                                            NotifyAdapter.BindEnd();
                                        }
                                    }
                                    else
                                    {
                                        NotifyAdapter.mNotificationsList =  new ObservableCollection<Get_General_Data_Object.Notification>( result.notifications);
                                        NotifyAdapter.BindEnd();
                                    }
                                }
                               
                                // Friend Requests
                                if (result.friend_requests.Length > 0)
                                {
                                    FriendRequestsAdapter.mFriendRequestsList = new ObservableCollection<Get_General_Data_Object.Friend_Requests>(result.friend_requests);

                                    ContextTab.Trending_Tab.layout_FriendRequest.Visibility = ViewStates.Visible;
                                    try
                                    {
                                        for (var i = 0; i < 4; i++)
                                            if (i == 0)
                                                ImageServiceLoader.Load_Image(ContextTab.Trending_Tab.FriendRequestimage1,
                                                    "no_profile_image.png",
                                                    FriendRequestsAdapter.mFriendRequestsList[i].avatar, 1);
                                            else if (i == 1)
                                                ImageServiceLoader.Load_Image(ContextTab.Trending_Tab.FriendRequestimage2,
                                                    "no_profile_image.png",
                                                    FriendRequestsAdapter.mFriendRequestsList[i].avatar, 1);
                                            else if (i == 2)
                                                ImageServiceLoader.Load_Image(ContextTab.Trending_Tab.FriendRequestimage3,
                                                    "no_profile_image.png",
                                                    FriendRequestsAdapter.mFriendRequestsList[i].avatar, 1);
                                    }
                                    catch (Exception e)
                                    {
                                        Crashes.TrackError(e);
                                    }
                                }
                                else
                                {
                                    ContextTab.Trending_Tab.layout_FriendRequest.Visibility = ViewStates.Gone;
                                }

                                if (Settings.Show_ProUsers_Members)
                                {
                                    // Pro Users
                                    if (result.pro_users.Length > 0)
                                    {
                                        int countList = ContextTab.ProUsersAdapter.mProUsersList.Count();

                                        if (countList > 0)
                                        {
                                            foreach (var item in result.pro_users)
                                            {
                                                var check = ContextTab.ProUsersAdapter.mProUsersList.FirstOrDefault( a => a.user_id == item.user_id);
                                                if (check == null)
                                                    ContextTab.ProUsersAdapter.mProUsersList.Add(item);
                                            }

                                            ContextTab.ProUsersAdapter.NotifyItemRangeChanged(countList, ContextTab.ProUsersAdapter.mProUsersList.Count-1);
                                            ContextTab.Trending_Tab.ProRecylerView.ScrollToPosition(ContextTab.ProUsersAdapter.mProUsersList.Count - 1);
                                            if(ContextTab.ProUsersAdapter.mProUsersList.Count() >=40)
                                            {
                                                try
                                                {
                                                    ContextTab.ProUsersAdapter.mProUsersList.RemoveAt(32);
                                                    ContextTab.ProUsersAdapter.mProUsersList.RemoveAt(33);
                                                    ContextTab.ProUsersAdapter.mProUsersList.RemoveAt(34);
                                                    ContextTab.ProUsersAdapter.mProUsersList.RemoveAt(35);
                                                    ContextTab.ProUsersAdapter.mProUsersList.RemoveAt(36);
                                                    ContextTab.ProUsersAdapter.mProUsersList.RemoveAt(37);
                                                    ContextTab.ProUsersAdapter.mProUsersList.RemoveAt(38);
                                                    ContextTab.ProUsersAdapter.mProUsersList.RemoveAt(39);
                                                    ContextTab.ProUsersAdapter.mProUsersList.RemoveAt(40);

                                                    ContextTab.ProUsersAdapter.NotifyItemRangeRemoved(0, 8);
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                    throw;
                                                }
                                              
                                            }
                                        }
                                        else
                                        {
                                            ContextTab.ProUsersAdapter.mProUsersList =new ObservableCollection<Get_General_Data_Object.Pro_Users>(result.pro_users);
                                           
                                        }
                                       

                                        if (ContextTab.Trending_Tab.layout_SuggestionProUsers.Visibility != ViewStates.Visible)
                                            ContextTab.Trending_Tab.layout_SuggestionProUsers.Visibility = ViewStates.Visible;
                                    }
                                    else
                                    {
                                        if (ContextTab.Trending_Tab.layout_SuggestionProUsers.Visibility != ViewStates.Gone)
                                            ContextTab.Trending_Tab.layout_SuggestionProUsers.Visibility = ViewStates.Gone;
                                    }
                                }
                                else
                                {
                                    ContextTab.Trending_Tab.layout_SuggestionProUsers.Visibility = ViewStates.Gone;
                                }


                                if (Settings.Show_Promoted_Pages)
                                {
                                    // Pro Pages
                                    if (result.promoted_pages.Length > 0)
                                    {
                                        ProPagesAdapter.mProPagesList =
                                            new ObservableCollection<Get_General_Data_Object.Promoted_Pages>(
                                                result.promoted_pages);
                                        ProPagesAdapter.BindEnd();
                                        ContextTab.Trending_Tab.PageRecylerView.SetLayoutManager(
                                            new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false));
                                        ContextTab.Trending_Tab.PageRecylerView.SetAdapter(ProPagesAdapter);

                                        ContextTab.Trending_Tab.layout_SuggestionPromotedPage.Visibility = ViewStates.Visible;
                                    }
                                    else
                                    {
                                        ContextTab.Trending_Tab.layout_SuggestionPromotedPage.Visibility = ViewStates.Gone;
                                    }
                                }
                                else
                                {
                                    ContextTab.Trending_Tab.layout_SuggestionPromotedPage.Visibility = ViewStates.Gone;
                                }

                                if (Settings.Show_Trending_Hashtags)
                                    if (result.trending_hashtag.Length > 0)
                                        HashtagUserAdapter.mHashtagList =
                                            new ObservableCollection<Get_General_Data_Object.Trending_Hashtag>(
                                                result.trending_hashtag);

                                if (swipeRefreshLayout != null && swipeRefreshLayout.Refreshing)
                                    swipeRefreshLayout.Refreshing = false;

                                //Show Empty Page
                                if (NotifyAdapter.mNotificationsList.Count > 0)
                                {
                                    Notifcation_Recyler.Visibility = ViewStates.Visible;
                                    notifications_Empty.Visibility = ViewStates.Gone;
                                }
                                else
                                {
                                    Notifcation_Recyler.Visibility = ViewStates.Gone;
                                    notifications_Empty.Visibility = ViewStates.Visible;
                                } 
                            });
                            return (result.new_notifications_count, result.new_friend_requests_count, result.count_new_messages);
                        }
                    }
                    else if (api_status == 400)
                    {
                        if (respond is Error_Object error)
                        {
                            var errortext = error._errors.Error_text;
                            //Toast.MakeText(this.Context, errortext, ToastLength.Short).Show();

                            if (errortext.Contains("Invalid or expired access_token"))
                                API_Request.Logout(Activity);
                        }

                        if (swipeRefreshLayout != null && swipeRefreshLayout.Refreshing)
                            swipeRefreshLayout.Refreshing = false;


                        return ("", "", "");
                    }
                    else if (api_status == 404)
                    {
                        var error = respond.ToString();
                        //Toast.MakeText(this.Context, error, ToastLength.Short).Show();

                        if (swipeRefreshLayout != null && swipeRefreshLayout.Refreshing)
                            swipeRefreshLayout.Refreshing = false;

                        return ("", "" , "");
                    }
                }

                this.Activity.RunOnUiThread(() =>
                {
                    //Show Empty Page
                    if (NotifyAdapter.mNotificationsList.Count > 0)
                    {
                        Notifcation_Recyler.Visibility = ViewStates.Visible;
                        notifications_Empty.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        Notifcation_Recyler.Visibility = ViewStates.Gone;
                        notifications_Empty.Visibility = ViewStates.Visible;
                    }

                    if (swipeRefreshLayout != null && swipeRefreshLayout.Refreshing)
                        swipeRefreshLayout.Refreshing = false;
                });
                  

                return ("", "", "");
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                await Get_GeneralData_Api(seenNotifications);
                return ("", "", "");
            }
        }


        //Event Click Item Notifications and Open Activity by type 
        private void NotifyAdapter_OnItemClick(object sender, Notifications_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = NotifyAdapter.GetItem(position);
                    if (item != null)
                    {
                        if (item.type == "following" || item.type == "visited_profile" ||
                            item.type == "accepted_request")
                        {
                            Intent Int;
                            if (item.notifier.user_id != UserDetails.User_id)
                            {
                                Int = new Intent(this.Context, typeof(User_Profile_Activity));
                                Int.PutExtra("UserId", item.notifier.user_id);
                                Int.PutExtra("UserType", "Notify");
                                Int.PutExtra("UserItem", JsonConvert.SerializeObject(item));
                            }
                            else
                            {
                                Int = new Intent(this.Context, typeof(MyProfile_Activity));
                                Int.PutExtra("UserId", item.notifier.user_id);
                            }

                            StartActivity(Int); 
                        }
                        else if (item.type == "liked_page" || item.type == "invited_page" ||
                                 item.type == "accepted_invite")
                        {
                            var Int = new Intent(Context, typeof(Page_ProfileActivity));
                            Int.PutExtra("NotifyPages", JsonConvert.SerializeObject(item));
                            Int.PutExtra("PagesType", "Liked_NotifyPages");
                            StartActivity(Int);
                        }
                        else if (item.type == "joined_group" || item.type == "accepted_join_request" ||
                                 item.type == "added_you_to_group")
                        {
                            var Int = new Intent(Context, typeof(Group_Profile_Activity));
                            Int.PutExtra("NotifyGroups", JsonConvert.SerializeObject(item));
                            Int.PutExtra("GroupsType", "Joined_NotifyGroups");
                            StartActivity(Int);
                        }
                        else if (item.type == "comment" || item.type == "wondered_post" ||
                                 item.type == "wondered_comment" || item.type == "reaction" ||
                                 item.type == "wondered_reply_comment" || item.type == "comment_mention" ||
                                 item.type == "comment_reply_mention" ||
                                 item.type == "liked_post" || item.type == "liked_comment" ||
                                 item.type == "liked_reply_comment" || item.type == "post_mention" ||
                                 item.type == "share_post" || item.type == "comment_reply" ||
                                 item.type == "also_replied" || item.type == "profile_wall_post")
                        {
                            var Int = new Intent(Context, typeof(HyberdPostViewer_Activity));
                            Int.PutExtra("Id", item.post_id);
                            Int.PutExtra("Type", "Post");
                            Int.PutExtra("Title", this.Context.GetString(Resource.String.Lbl_Post));
                            StartActivity(Int);
                        }
                        else if (item.type == "going_event")
                        {
                            var Int = new Intent(Context, typeof(HyberdPostViewer_Activity));
                            Int.PutExtra("Id", item.event_id);
                            Int.PutExtra("Type", "Event");
                            Int.PutExtra("Title", this.Context.GetString(Resource.String.Lbl_ViewEvent));
                            StartActivity(Int);
                        }
                        else if (item.type == "viewed_story")
                        {
                            var Int = new Intent(Context, typeof(HyberdPostViewer_Activity));
                            Int.PutExtra("Type", "Viewed");
                            Int.PutExtra("Id", item.url);
                            Int.PutExtra("Title", this.Context.GetString(Resource.String.Lbl_Story));
                            StartActivity(Int);
                        }
                        else
                        {
                            var Int = new Intent(Context, typeof(HyberdPostViewer_Activity));
                            Int.PutExtra("Id", item.post_id);
                            Int.PutExtra("Type", "Post");
                            Int.PutExtra("Title", this.Context.GetString(Resource.String.Lbl_Post));
                            StartActivity(Int);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        // Event Promoted Pages => Using Get_General_Data_Object.Promoted_Pages => Open Page_ProfileActivity
        private void ProPagesAdapter_OnItemClick(object sender, ProPages_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = ProPagesAdapter.GetItem(position);
                    if (item != null)
                    {
                        var Int = new Intent(Context, typeof(Page_ProfileActivity));
                        Int.PutExtra("PromotedPages", JsonConvert.SerializeObject(item));
                        Int.PutExtra("PagesType", "Liked_PromotedPages");
                        StartActivity(Int);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Open Profile User Using User_Profile_Activity
        private void ProUsersAdapter_OnItemClick(object sender, ProUsers_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = ContextTab.ProUsersAdapter.GetItem(position);
                    if (item != null)
                    {
                        Intent Int;
                        if (item.user_id != UserDetails.User_id)
                        {
                            Int = new Intent(Context, typeof(User_Profile_Activity));
                            Int.PutExtra("UserId", item.user_id);
                            Int.PutExtra("UserType", "ProUsers");
                            Int.PutExtra("UserItem", JsonConvert.SerializeObject(item));
                        }
                        else
                        {
                            Int = new Intent(Context, typeof(MyProfile_Activity));
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


        //Event Refresh Data Page
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                NotifyAdapter.Clear();
                ContextTab.ProUsersAdapter.Clear();
                ProPagesAdapter.Clear();

                Get_GeneralData_Api(true).ConfigureAwait(false);
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
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Variables Basic

        private LinearLayout Layout_main;
        public RecyclerView Notifcation_Recyler;
        public LinearLayout notifications_Empty;
        public RecyclerView.LayoutManager Notify_LayoutManager;

        public static Notifications_Adapter NotifyAdapter;
        public static HashtagUser_Adapter HashtagUserAdapter;
       
        public static ProPages_Adapter ProPagesAdapter;
        public static FriendRequests_Adapter FriendRequestsAdapter;

        public TextView notificationsIcon;
        public SwipeRefreshLayout swipeRefreshLayout;

        #endregion
    }
}