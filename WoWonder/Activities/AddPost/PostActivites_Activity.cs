using System;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using FFImageLoading;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.AddPost.Adapters;
using WoWonder.Helpers;
using Exception = System.Exception;

namespace WoWonder.Activities.AddPost
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class PostActivites_Activity : AppCompatActivity, MaterialDialog.ISingleButtonCallback,
        MaterialDialog.IInputCallback
    {
        public void OnInput(MaterialDialog p0, ICharSequence p1)
        {
            try
            {
                if (TypeDialog == "Listening")
                {
                    if (p1.Length() > 0)
                    {
                        var strName = p1.ToString();

                        var resultIntent = new Intent();
                        resultIntent.PutExtra("ActivitiesType", "listening");
                        resultIntent.PutExtra("ActivitiesText", strName);
                        SetResult(Result.Ok, resultIntent);
                        Finish();
                    }
                }
                else if (TypeDialog == "Playing")
                {
                    if (p1.Length() > 0)
                    {
                        var strName = p1.ToString();

                        var resultIntent = new Intent();
                        resultIntent.PutExtra("ActivitiesType", "playing");
                        resultIntent.PutExtra("ActivitiesText", strName);
                        SetResult(Result.Ok, resultIntent);
                        Finish();
                    }
                }
                else if (TypeDialog == "Watching")
                {
                    if (p1.Length() > 0)
                    {
                        var strName = p1.ToString();

                        var resultIntent = new Intent();
                        resultIntent.PutExtra("ActivitiesType", "watching");
                        resultIntent.PutExtra("ActivitiesText", strName);
                        SetResult(Result.Ok, resultIntent);
                        Finish();
                    }
                }
                else if (TypeDialog == "Traveling")
                {
                    if (p1.Length() > 0)
                    {
                        var strName = p1.ToString();

                        var resultIntent = new Intent();
                        resultIntent.PutExtra("ActivitiesType", "traveling");
                        resultIntent.PutExtra("ActivitiesText", strName);
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

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (TypeDialog == "PostPrivacy")
                {
                    if (p1 == DialogAction.Positive) p0.Dismiss();
                }
                else if (TypeDialog == "PostBack")
                {
                    if (p1 == DialogAction.Positive)
                    {
                        p0.Dismiss();
                        Finish();
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
                else
                {
                    if (p1 == DialogAction.Positive)
                    {
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.PostActivites_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.PostActivites_Layout);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_What_Are_You_Doing);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                PostActivitesRecyler = FindViewById<RecyclerView>(Resource.Id.Recyler);

                PostActivitesRecyler.SetLayoutManager(new LinearLayoutManager(this));
                ActivitesAdapter = new PostActivites_Adapter(this);
                PostActivitesRecyler.SetAdapter(ActivitesAdapter);
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
                ActivitesAdapter.ItemClick += ActivitesAdapter_OnItemClick;
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
                ActivitesAdapter.ItemClick -= ActivitesAdapter_OnItemClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void ActivitesAdapter_OnItemClick(object sender, PostActivites_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = ActivitesAdapter.GetItem(position);
                    if (item != null)
                    {
                        if (item.ID == 1) // Listening to
                        {
                            TypeDialog = "Listening";

                            var dialog = new MaterialDialog.Builder(this);

                            dialog.Title(Resource.String.Lbl_What_Are_You_Doing);
                            dialog.Input(Resource.String.Lbl_Comment_Hint_Listening, 0, false, this);
                            dialog.InputType(InputTypes.TextFlagImeMultiLine);
                            dialog.PositiveText(GetText(Resource.String.Lbl_Submit)).OnPositive(this);
                            dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                            dialog.Build().Show();
                            dialog.AlwaysCallSingleChoiceCallback();
                        }
                        else if (item.ID == 2) // Playing
                        {
                            TypeDialog = "Playing";

                            var dialog = new MaterialDialog.Builder(this);

                            dialog.Title(Resource.String.Lbl_What_Are_You_Doing);
                            dialog.Input(Resource.String.Lbl_Comment_Hint_Playing, 0, false, this);
                            dialog.InputType(InputTypes.TextFlagImeMultiLine);
                            dialog.PositiveText(GetText(Resource.String.Lbl_Submit)).OnPositive(this);
                            dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                            dialog.Build().Show();
                            dialog.AlwaysCallSingleChoiceCallback();
                        }
                        else if (item.ID == 3) // Watching
                        {
                            TypeDialog = "Watching";

                            var dialog = new MaterialDialog.Builder(this);

                            dialog.Title(Resource.String.Lbl_What_Are_You_Doing);
                            dialog.Input(Resource.String.Lbl_Comment_Hint_Watching, 0, false, this);
                            dialog.InputType(InputTypes.TextFlagImeMultiLine);
                            dialog.PositiveText(GetText(Resource.String.Lbl_Submit)).OnPositive(this);
                            dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                            dialog.Build().Show();
                            dialog.AlwaysCallSingleChoiceCallback();
                        }
                        else if (item.ID == 4) // Traveling
                        {
                            TypeDialog = "Traveling";

                            var dialog = new MaterialDialog.Builder(this);

                            dialog.Title(Resource.String.Lbl_What_Are_You_Doing);
                            dialog.Input(Resource.String.Lbl_Comment_Hint_Traveling, 0, false, this);
                            dialog.InputType(InputTypes.TextFlagImeMultiLine);
                            dialog.PositiveText(GetText(Resource.String.Lbl_Submit)).OnPositive(this);
                            dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                            dialog.Build().Show();
                            dialog.AlwaysCallSingleChoiceCallback();
                        }
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

        public RecyclerView PostActivitesRecyler;
        private PostActivites_Adapter ActivitesAdapter;

        public string TypeDialog = "";

        #endregion
    }
}