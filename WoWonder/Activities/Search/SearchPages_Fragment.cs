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
using WoWonder.Activities.Communities.Pages;
using WoWonder.Activities.Search.Adapters;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;
using Fragment = Android.Support.V4.App.Fragment;

namespace WoWonder.Activities.Search
{
    public class SearchPages_Fragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                Context context = MyContextWrapper.Wrap(Context, Settings.Lang);
                inflater = (LayoutInflater) Activity.GetSystemService(Context.LayoutInflaterService);
                var view = inflater.Inflate(context.Resources.GetLayout(Resource.Layout.SearchPages_Layout), container,
                    false);

                SearchPage_Recyler = (RecyclerView) view.FindViewById(Resource.Id.searchPageRecyler);
                SearchPage_Empty = (LinearLayout) view.FindViewById(Resource.Id.searchPage_LinerEmpty);

                swipeRefreshLayout = (SwipeRefreshLayout) view.FindViewById(Resource.Id.searchPage_swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = false;
                swipeRefreshLayout.Enabled = false;

                mSearchLayoutManager = new LinearLayoutManager(Context);
                SearchPage_Recyler.SetLayoutManager(mSearchLayoutManager);
                mSaerchPageAdapter = new SaerchPage_Adapter(Context);
                mSaerchPageAdapter.mSearchPageList = new ObservableCollection<Get_Search_Object.Page>();
                SearchPage_Recyler.SetAdapter(mSaerchPageAdapter);

                Btn_SearchRandom = view.FindViewById<Button>(Resource.Id.SearchRandom_Button);
                Btn_SearchRandom.Click += BtnSearchRandomOnClick;

                SearchPage_Recyler.Visibility = ViewStates.Gone;
                SearchPage_Empty.Visibility = ViewStates.Visible;

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
                mSaerchPageAdapter.ItemClick += MSaerchPageAdapter_OnItemClick;
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
                mSaerchPageAdapter.ItemClick -= MSaerchPageAdapter_OnItemClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //profile page 
        private void MSaerchPageAdapter_OnItemClick(object sender, SaerchPage_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = mSaerchPageAdapter.GetItem(position);
                    if (item != null)
                    {
                        var Int = new Intent(Context, typeof(Page_ProfileActivity));
                        Int.PutExtra("SaerchPages", JsonConvert.SerializeObject(item));
                        Int.PutExtra("PagesType", "Saerch_Pages");
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

        public RecyclerView SearchPage_Recyler;
        public LinearLayout SearchPage_Empty;

        public LinearLayoutManager mSearchLayoutManager;
        public SaerchPage_Adapter mSaerchPageAdapter;

        private SwipeRefreshLayout swipeRefreshLayout;
        private TextView searchIcon;
        private Button Btn_SearchRandom;

        #endregion
    }
}