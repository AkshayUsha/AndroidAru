using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.FriendRequest;
using WoWonder.Activities.Tabbes.Adapters;
using WoWonder.Activities.UsersPages;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Requests;
using Fragment = Android.Support.V4.App.Fragment;

namespace WoWonder.Activities.Tabbes
{
    public class Trending_Fragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                var view = MyContextWrapper.GetContentView(Context, Settings.Lang, Resource.Layout.Tab_Trending_Layout);
                if (view == null) view = inflater.Inflate(Resource.Layout.Tab_Trending_Layout, container, false);

                ProRecylerView = (RecyclerView) view.FindViewById(Resource.Id.proRecyler);
                PageRecylerView = (RecyclerView) view.FindViewById(Resource.Id.pagerecyler);
                LastActivitiesRecyclerView = (RecyclerView) view.FindViewById(Resource.Id.lastactivitiesRecyler);
                NestedScroller = (NestedScrollView) view.FindViewById(Resource.Id.nestedScrollView);
                FriendRequestimage1 = (ImageViewAsync) view.FindViewById(Resource.Id.image_page_1);
                FriendRequestimage2 = (ImageViewAsync) view.FindViewById(Resource.Id.image_page_2);
                FriendRequestimage3 = (ImageViewAsync) view.FindViewById(Resource.Id.image_page_3);

                layout_SuggestionProUsers = (LinearLayout) view.FindViewById(Resource.Id.layout_suggestion_Friends);
                layout_SuggestionlLastactivities =
                    (LinearLayout) view.FindViewById(Resource.Id.layout_suggestion_lastactivities);
                layout_FriendRequest = (RelativeLayout) view.FindViewById(Resource.Id.layout_friend_Request);
                layout_SuggestionPromotedPage =
                    (LinearLayout) view.FindViewById(Resource.Id.layout_suggestion_PromotedPage);

                ProRecylerView.NestedScrollingEnabled = false;
                PageRecylerView.NestedScrollingEnabled = false;

                ((Tabbed_Main_Activity)Context).ProUsersAdapter = new ProUsers_Adapter(Context);
                ((Tabbed_Main_Activity)Context).ProUsersAdapter.mProUsersList = new ObservableCollection<Get_General_Data_Object.Pro_Users>();
                ProRecylerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false));
                ProRecylerView.SetAdapter(((Tabbed_Main_Activity)Context).ProUsersAdapter);
                ProRecylerView.SetItemViewCacheSize(18);
                ProRecylerView.GetLayoutManager().ItemPrefetchEnabled = true;
                ProRecylerView.DrawingCacheEnabled = (true);
                ProRecylerView.DrawingCacheQuality = DrawingCacheQuality.High;
                //============================= Last Activities Users ==================================

                LastActivitiesAdapter = new LastActivities_Adapter(Context);
                LastActivitiesAdapter.mLastActivitiesList = new ObservableCollection<Activities_Object.Activity>();
                LastActivitiesRecyclerView.NestedScrollingEnabled = false;
                LastActivitiesRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
                LastActivitiesRecyclerView.SetAdapter(LastActivitiesAdapter);
                LastActivitiesRecyclerView.SetItemViewCacheSize(18);
                LastActivitiesRecyclerView.GetLayoutManager().ItemPrefetchEnabled = true;
                LastActivitiesRecyclerView.DrawingCacheEnabled = (true);
                LastActivitiesRecyclerView.DrawingCacheQuality = DrawingCacheQuality.High;

                if (!Settings.Show_LastActivities)
                {
                    LastActivitiesRecyclerView.Visibility = ViewStates.Gone;
                    layout_SuggestionlLastactivities.Visibility = ViewStates.Gone;
                }

                if (!Settings.SetTabOnButton)
                {
                    var parasms = (LinearLayout.LayoutParams) NestedScroller.LayoutParameters;

                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                        parasms.TopMargin = 120;
                    else
                        parasms.TopMargin = 225;

                    NestedScroller.LayoutParameters = parasms;
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

                //Add Event
                layout_FriendRequest.Click += LayoutFriendRequest_OnClick;
                LastActivitiesAdapter.ItemClick += LastActivitiesAdapter_OnItemClick;
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
                layout_FriendRequest.Click -= LayoutFriendRequest_OnClick;
                LastActivitiesAdapter.ItemClick -= LastActivitiesAdapter_OnItemClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Api latest activities
        public async void Get_Activities()
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(Context, GetString(Resource.String.Lbl_CheckYourInternetConnection),
                        ToastLength.Short).Show();
                    LastActivitiesRecyclerView.Visibility = ViewStates.Gone;
                    layout_SuggestionlLastactivities.Visibility = ViewStates.Gone;
                }
                else
                {
                    var (Api_status, Respond) = await Client.Global.Get_Activities();
                    if (Api_status == 200)
                    {
                        if (Respond is Activities_Object result)
                        {
                            this.Activity.RunOnUiThread(() =>
                            {
                                if (result.Activities.Count > 0)
                                {
                                    //Bring new groups
                                    var listnew = result.Activities.Where(c =>
                                            !LastActivitiesAdapter.mLastActivitiesList.Select(fc => fc.Id).Contains(c.Id))
                                        .ToList();
                                    if (listnew.Count > 0)
                                    {
                                        //Results differ
                                        Classes.AddRange(LastActivitiesAdapter.mLastActivitiesList, listnew);
                                        LastActivitiesAdapter.BindEnd();
                                    }
                                    else
                                    {
                                        LastActivitiesAdapter.mLastActivitiesList =
                                            new ObservableCollection<Activities_Object.Activity>(result.Activities);
                                        LastActivitiesAdapter.BindEnd();
                                    }
                                }
                                else
                                {
                                    LastActivitiesAdapter.mLastActivitiesList =
                                        new ObservableCollection<Activities_Object.Activity>(result.Activities);
                                    LastActivitiesAdapter.BindEnd();
                                }
                            }); 
                        }
                    }
                    else if (Api_status == 400)
                    {
                        if (Respond is Error_Object error)
                        {
                            var errortext = error._errors.Error_text;
                            //Toast.MakeText(this.Context, errortext, ToastLength.Short).Show();

                            if (errortext.Contains("Invalid or expired access_token"))
                                API_Request.Logout(Activity);
                        }
                    }
                    else if (Api_status == 404)
                    {
                        var error = Respond.ToString();
                        //Toast.MakeText(this.Context, error, ToastLength.Short).Show();
                    }

                    this.Activity.RunOnUiThread(() =>
                    {
                        if (LastActivitiesAdapter.mLastActivitiesList.Count > 0)
                        {
                            LastActivitiesRecyclerView.Visibility = ViewStates.Visible;
                            layout_SuggestionlLastactivities.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            LastActivitiesRecyclerView.Visibility = ViewStates.Gone;
                            layout_SuggestionlLastactivities.Visibility = ViewStates.Gone;
                        }
                    }); 
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_Activities();
            }
        }

        //Event Click Item last Activities and Open HyberdPostViewer_Activity >> Post
        private void LastActivitiesAdapter_OnItemClick(object sender,
            LastActivities_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = LastActivitiesAdapter.GetItem(position);
                    if (item != null)
                    {
                        var Int = new Intent(Context, typeof(HyberdPostViewer_Activity));
                        Int.PutExtra("Id", item.PostId);
                        Int.PutExtra("Type", "Post");
                        Int.PutExtra("Title", this.Context.GetString(Resource.String.Lbl_Post));
                        StartActivity(Int);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        // Event Show all Friend Request 
        private void LayoutFriendRequest_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (Notifications_Fragment.FriendRequestsAdapter?.mFriendRequestsList.Count > 0)
                {
                    var Int = new Intent(Context, typeof(FriendRequest_Activity));
                    StartActivity(Int);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
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

        public RecyclerView ProRecylerView;
        public RecyclerView PageRecylerView;
        public RecyclerView LastActivitiesRecyclerView;

        public ImageViewAsync FriendRequestimage1;
        public ImageViewAsync FriendRequestimage2;
        public ImageViewAsync FriendRequestimage3;

        public RelativeLayout layout_FriendRequest;
        public LinearLayout layout_SuggestionProUsers;
        public LinearLayout layout_SuggestionPromotedPage;
        public LinearLayout layout_SuggestionlLastactivities;

        public LastActivities_Adapter LastActivitiesAdapter;
         
        public NestedScrollView NestedScroller;

        #endregion
    }
}