using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Requests;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.EditPost
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class EditPost_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.EditPost_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.AddPost_Layout);

                var datapost = Intent.GetStringExtra("PostText") ?? "Data not available";
                if (datapost != "Data not available" && !string.IsNullOrEmpty(datapost))
                    PostText = datapost;

                var dataid = Intent.GetStringExtra("PostId") ?? "Data not available";
                if (dataid != "Data not available" && !string.IsNullOrEmpty(dataid))
                    IdPost = dataid;

                TopToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (TopToolBar != null)
                {
                    TopToolBar.Title = GetText(Resource.String.Lbl_EditPost);

                    SetSupportActionBar(TopToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                Txt_AddPost = FindViewById<TextView>(Resource.Id.toolbar_title);
                Txt_ContentPost = FindViewById<EditText>(Resource.Id.editTxtEmail);
               
                Txt_ContentPost.Text = PostText;

            
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
                Txt_AddPost.Click += EditPost_OnClick;
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
                Txt_AddPost.Click -= EditPost_OnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Edit 
        private async void EditPost_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)
                        .Show();
                }
                else
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading) + "...");

                    var (Api_status, Respond) = await Client.Global.Post_Actions(IdPost, "edit", Txt_ContentPost.Text);
                    if (Api_status == 200)
                    {
                        if (Respond is Post_Actions_Object result)
                        {
                            if (result.Action.Contains(""))
                            {
                                Toast.MakeText(this, result.Action, ToastLength.Short).Show();
                                AndHUD.Shared.Dismiss(this);

                                // put the String to pass back into an Intent and close this activity
                                var resultIntent = new Intent();
                                resultIntent.PutExtra("PostId", IdPost);
                                resultIntent.PutExtra("PostText", Txt_ContentPost.Text);
                                SetResult(Result.Ok, resultIntent);
                                Finish();
                            }
                            else
                            {
                                //Show a Error image with a message
                                AndHUD.Shared.ShowError(this, result.Action, MaskType.Clear, TimeSpan.FromSeconds(2));
                            }
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

                    AndHUD.Shared.Dismiss(this);
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

        private Toolbar TopToolBar;
        private TextView Txt_AddPost;

        private EditText Txt_ContentPost;

        private string PostText = "";
        private string IdPost = "";

        #endregion

    }
}