using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.AddPost.Adapters;
using WoWonder.Helpers;

namespace WoWonder.Activities.AddPost
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Feelings_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Feelings_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Feelings_Layout);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_Feeling);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                FeelingsRecylerView = FindViewById<RecyclerView>(Resource.Id.Recyler);

                FeelingsAdapter = new FeelingsAdapter(this);
                FeelingsRecylerView.SetLayoutManager(new GridLayoutManager(this, 3));
                FeelingsRecylerView.AddItemDecoration(new GridSpacingItemDecoration(3, 3, true));
                FeelingsRecylerView.SetAdapter(FeelingsAdapter);
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
                FeelingsAdapter.ItemClick += FeelingsAdapter_ItemClick;
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
                FeelingsAdapter.ItemClick -= FeelingsAdapter_ItemClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void FeelingsAdapter_ItemClick(object sender, FeelingsAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = FeelingsAdapter.GetItem(position);
                    if (item != null)
                    {
                        var resultIntent = new Intent();
                        resultIntent.PutExtra("Feelings", item.FeelingText);
                        SetResult(Result.Ok, resultIntent);
                        Finish();
                    }
                }
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
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Variables Basic

        public RecyclerView FeelingsRecylerView;
        public FeelingsAdapter FeelingsAdapter;

        #endregion
    }
}