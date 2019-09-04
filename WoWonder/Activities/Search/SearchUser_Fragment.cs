using System;
using System.Collections.ObjectModel;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.MyProfile;
using WoWonder.Activities.Search.Adapters;
using WoWonder.Activities.UserProfile;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;
using Fragment = Android.Support.V4.App.Fragment;

namespace WoWonder.Activities.Search
{
    public class SearchUser_Fragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                Context context = MyContextWrapper.Wrap(Context, Settings.Lang);
                inflater = (LayoutInflater) Activity.GetSystemService(Context.LayoutInflaterService);
                var view = inflater.Inflate(context.Resources.GetLayout(Resource.Layout.SearchUsers_Layout), container,
                    false);

                Search_Recyler = (RecyclerView) view.FindViewById(Resource.Id.searchRecyler);
                Search_Empty = (LinearLayout) view.FindViewById(Resource.Id.Search_LinerEmpty);

                swipeRefreshLayout = (SwipeRefreshLayout) view.FindViewById(Resource.Id.search_swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = false;
                swipeRefreshLayout.Enabled = false;

                mSearchLayoutManager = new LinearLayoutManager(Context);
                Search_Recyler.SetLayoutManager(mSearchLayoutManager);
                mSearchAdapter = new SearchUser_Adapter(Context);
                mSearchAdapter.mSearchUserList = new ObservableCollection<Get_Search_Object.User>();
                Search_Recyler.SetAdapter(mSearchAdapter);

                Btn_SearchRandom = view.FindViewById<Button>(Resource.Id.SearchRandom_Button);

                Search_Recyler.Visibility = ViewStates.Gone;
                Search_Empty.Visibility = ViewStates.Visible;

                //Show Ads
                mAdView = view.FindViewById<AdView>(Resource.Id.adView);
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
                if (mAdView != null) mAdView.Resume();
                //Add Event
                Btn_SearchRandom.Click += BtnSearchRandomOnClick;
                mSearchAdapter.ItemClick += MSearchAdapterOnItemClick;
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
                if (mAdView != null) mAdView.Pause();

                base.OnPause();

                //Close Event
                Btn_SearchRandom.Click -= BtnSearchRandomOnClick;
                mSearchAdapter.ItemClick -= MSearchAdapterOnItemClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void MSearchAdapterOnItemClick(object sender, SearchUser_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = mSearchAdapter.GetItem(position);
                    if (item != null)
                    {
                        Intent Int;
                        if (item.UserId != UserDetails.User_id)
                        {
                            Int = new Intent(this.Context, typeof(User_Profile_Activity));
                            Int.PutExtra("UserId", item.UserId);
                            Int.PutExtra("UserType", "Search");
                            Int.PutExtra("UserItem", JsonConvert.SerializeObject(item));
                        }
                        else
                        {
                            Int = new Intent(this.Context, typeof(MyProfile_Activity));
                            Int.PutExtra("UserId", item.UserId);
                        }
                    
                        StartActivity(Int);
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        private void BtnSearchRandomOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                ((Search_Tabbed_Activity) Activity).GetSearch_Result("");
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

        public RecyclerView Search_Recyler;
        public LinearLayout Search_Empty;

        public LinearLayoutManager mSearchLayoutManager;
        public SearchUser_Adapter mSearchAdapter;

        private SwipeRefreshLayout swipeRefreshLayout;
        private TextView searchIcon;
        private Button Btn_SearchRandom;

        private AdView mAdView;

        #endregion
    }
}