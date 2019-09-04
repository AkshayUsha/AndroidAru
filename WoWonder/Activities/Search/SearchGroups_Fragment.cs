using System;
using System.Collections.ObjectModel;
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
using WoWonder.Activities.Search.Adapters;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;
using Fragment = Android.Support.V4.App.Fragment;

namespace WoWonder.Activities.Search
{
    public class SearchGroups_Fragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                Context context = MyContextWrapper.Wrap(Context, Settings.Lang);
                inflater = (LayoutInflater) Activity.GetSystemService(Context.LayoutInflaterService);
                var view = inflater.Inflate(context.Resources.GetLayout(Resource.Layout.SearchGroups_Layout), container,
                    false);

                SearchGroup_Recyler = (RecyclerView) view.FindViewById(Resource.Id.searchGroupRecyler);
                SearchGroup_Empty = (LinearLayout) view.FindViewById(Resource.Id.searchGroup_LinerEmpty);

                swipeRefreshLayout = (SwipeRefreshLayout) view.FindViewById(Resource.Id.searchGroup_swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = false;
                swipeRefreshLayout.Enabled = false;

                mSearchLayoutManager = new LinearLayoutManager(Context);
                SearchGroup_Recyler.SetLayoutManager(mSearchLayoutManager);
                mSaerchGroupAdapter = new SearchGroup_Adapter(Context);
                mSaerchGroupAdapter.mSearchGroupList = new ObservableCollection<Get_Search_Object.Group>();
                SearchGroup_Recyler.SetAdapter(mSaerchGroupAdapter);

                Btn_SearchRandom = view.FindViewById<Button>(Resource.Id.SearchRandom_Button);

                SearchGroup_Recyler.Visibility = ViewStates.Gone;
                SearchGroup_Empty.Visibility = ViewStates.Visible;

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
                mSaerchGroupAdapter.ItemClick += MSaerchGroupAdapter_OnItemClick;
                Btn_SearchRandom.Click += BtnSearchRandomOnClick;
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
                mSaerchGroupAdapter.ItemClick -= MSaerchGroupAdapter_OnItemClick;
                Btn_SearchRandom.Click -= BtnSearchRandomOnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //profile Group 
        private void MSaerchGroupAdapter_OnItemClick(object sender,
            SearchGroup_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = mSaerchGroupAdapter.GetItem(position);
                    if (item != null)
                    {
                        var Int = new Intent(Context, typeof(Group_Profile_Activity));
                        Int.PutExtra("SearchGroups", JsonConvert.SerializeObject(item));
                        Int.PutExtra("GroupsType", "Search_Groups");
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

        public RecyclerView SearchGroup_Recyler;
        public LinearLayout SearchGroup_Empty;

        public LinearLayoutManager mSearchLayoutManager;
        public SearchGroup_Adapter mSaerchGroupAdapter;

        private SwipeRefreshLayout swipeRefreshLayout;
        private TextView searchIcon;
        private Button Btn_SearchRandom;

        #endregion
    }
}