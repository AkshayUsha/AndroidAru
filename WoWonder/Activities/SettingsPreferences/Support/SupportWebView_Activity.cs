using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using FFImageLoading;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.AddPost;
using WoWonder.Helpers;
using WoWonder.Helpers.HybirdView;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.SettingsPreferences.Support
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class SupportWebView_Activity : AppCompatActivity, MaterialDialog.IListCallback,
        MaterialDialog.ISingleButtonCallback
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang,
                    Resource.Layout.Settings_SupportWebView_layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Settings_SupportWebView_layout);

                var data = Intent.GetStringExtra("URL") ?? "Data not available";
                if (data != "Data not available" && !string.IsNullOrEmpty(data)) Url = data;

                var Type = Intent.GetStringExtra("Type") ?? "Data not available";
                if (Type != "Data not available" && !string.IsNullOrEmpty(data)) Type_Url = Type;

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = Type_Url;

                    SetSupportActionBar(ToolBar);

                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                HybirdView = FindViewById<WebView>(Resource.Id.SupportWebView);
                swipeRefreshLayout = (SwipeRefreshLayout) FindViewById(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);

                Support_Empty = FindViewById<LinearLayout>(Resource.Id.Support_LinerEmpty);
                Support_Emptyicon = FindViewById<TextView>(Resource.Id.Support_icon);
                Txt_News_Empty = (TextView) FindViewById(Resource.Id.Txt_LabelEmpty);
                Txt_News_start = (TextView) FindViewById(Resource.Id.Txt_LabelStart);
                Btn_Reload = (Button) FindViewById(Resource.Id.reloadPage_Button);
                IMethods.Set_TextViewIcon("2", Support_Emptyicon, "\uf119");
                Support_Empty.Visibility = ViewStates.Gone;

                IMethods.Set_TextViewIcon("1", Support_Emptyicon, IonIcons_Fonts.Videocamera);
                Support_Emptyicon.SetTextColor(Color.ParseColor(Settings.MainColor));

                //Set WebView and Load url to be rendered on WebView
                HybridController = new HybirdViewController(this, HybirdView);

                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)
                        .Show();

                    HybirdView.Visibility = ViewStates.Gone;
                    Support_Empty.Visibility = ViewStates.Visible;
                }
                else
                {
                    HybirdView.Visibility = ViewStates.Visible;
                    Support_Empty.Visibility = ViewStates.Gone;

                    swipeRefreshLayout.Refreshing = true;
                    swipeRefreshLayout.Enabled = true;

                    if (Settings.ClearCachSystem)
                        HybridController.HybirdView.ClearCache(true);


                    switch (Settings.Lang)
                    {
                        case "en":
                            HybridController.LoadUrl(Url + "&lang=english");
                            break;
                        case "ar":
                            HybridController.LoadUrl(Url + "&lang=arabic");
                            Settings.FlowDirection_RightToLeft = true;
                            break;
                        case "de":
                            HybridController.LoadUrl(Url + "&lang=german");
                            break;
                        case "el":
                            HybridController.LoadUrl(Url + "&lang=greek");
                            break;
                        case "es":
                            HybridController.LoadUrl(Url + "&lang=spanish");
                            break;
                        case "fr":
                            HybridController.LoadUrl(Url + "&lang=french");
                            break;
                        case "it":
                            HybridController.LoadUrl(Url + "&lang=italian");
                            break;
                        case "ja":
                            HybridController.LoadUrl(Url + "&lang=japanese");
                            break;
                        case "nl":
                            HybridController.LoadUrl(Url + "&lang=dutch");
                            break;
                        case "pt":
                            HybridController.LoadUrl(Url + "&lang=portuguese");
                            break;
                        case "ro":
                            HybridController.LoadUrl(Url + "&lang=romanian");
                            break;
                        case "ru":
                            HybridController.LoadUrl(Url + "&lang=russian");
                            break;
                        case "sq":
                            HybridController.LoadUrl(Url + "&lang=albanian");
                            break;
                        case "sr":
                            HybridController.LoadUrl(Url + "&lang=serbian");
                            break;
                        case "tr":
                            HybridController.LoadUrl(Url + "&lang=turkish");
                            break;
                        default:
                            HybridController.LoadUrl(Url);
                            break;
                    }
                }
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
                Btn_Reload.Click += BtnReload_OnClick;
                HybridController.JavascriptInterface.OnJavascriptInjectionRequest += OnJavascriptInjectionRequest;
                HybridController.DefaultClient.OnPageEventFinished += WoDefaultClient_OnPageEventFinished;
                if (Settings.Show_Error_HybirdView)
                    HybridController.DefaultClient.OnPageEventReceivedError += DefaultClientOnOnPageEventReceivedError;
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
                Btn_Reload.Click -= BtnReload_OnClick;
                HybridController.JavascriptInterface.OnJavascriptInjectionRequest -= OnJavascriptInjectionRequest;
                HybridController.DefaultClient.OnPageEventFinished -= WoDefaultClient_OnPageEventFinished;
                if (Settings.Show_Error_HybirdView)
                    HybridController.DefaultClient.OnPageEventReceivedError -= DefaultClientOnOnPageEventReceivedError;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == 2500)
                {
                    HybridController.EvaluateJavascript("Wo_GetNewPosts();");
                }
                else if (requestCode == 3500)
                {
                    string ID = data.GetStringExtra("PostId");
                    string Text = data.GetStringExtra("PostText");

                    string JavaCode = "$('#post-' + " + ID + ").find('#edit-post').attr('onclick', '{*type*:*edit_post*,*post_id*:*" + ID + "*,*edit_text*:*" + Text + "*}');";
                    string Decode = JavaCode.Replace("*", "&quot;");

                    HybridController.EvaluateJavascript(Decode);
                    HybridController.EvaluateJavascript("$('#post-' + " + ID + ").find('.post-description p').html('" + Text + "');");
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

        public static SwipeRefreshLayout swipeRefreshLayout;
        public static WebView HybirdView;

        public HybirdViewController HybridController;
        public static LinearLayout Support_Empty;
        public static TextView Support_Emptyicon;
        public TextView Txt_News_Empty;
        public TextView Txt_News_start;
        private Button Btn_Reload;

        private string Url = "";
        private string Type_Url = "";

        #endregion

        #region HybridController

        private Task<string> OnJavascriptInjectionRequest(string eventobj)
        {
            try
            {
                if (!string.IsNullOrEmpty(eventobj))
                    if (eventobj.Contains("type"))
                    {
                        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(eventobj);
                        var type = data["type"].ToString();

                        if (type == "delete_post")
                            RunOnUiThread(() =>
                            {
                                try
                                {
                                    var DialogList = new MaterialDialog.Builder(this);
                                    DialogList.Tag(data["post_id"].ToString());
                                    DialogList.Title(GetText(Resource.String.Lbl_Title_DeletePost));
                                    DialogList.Content(GetText(Resource.String.Lbl_Content_DeletePost));
                                    DialogList.NegativeText(GetText(Resource.String.Lbl_Cancel));
                                    DialogList.OnNegative(this);
                                    DialogList.PositiveText(GetText(Resource.String.Lbl_Delete));
                                    DialogList.OnPositive(this);
                                    DialogList.Build().Show();
                                }
                                catch (Exception exception)
                                {
                                    Crashes.TrackError(exception);
                                }
                            });
                        else if (type == "publisher-box")
                            RunOnUiThread(() =>
                            {
                                var Int = new Intent(this, typeof(AddPost_Activity));
                                Int.PutExtra("Type", "Normal");
                                Int.PutExtra("PostId", UserDetails.User_id);
                                Int.PutExtra("isOwner", "Normal");
                                StartActivityForResult(Int, 2500);
                            });
                        else
                            return null;
                    }

                return null;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        }

        private void WoDefaultClient_OnPageEventFinished(WebView view, string url)
        {
            try
            {
                swipeRefreshLayout.Refreshing = false;
                swipeRefreshLayout.Enabled = false;

                HybridController.EvaluateJavascript("$('.header-container').hide();");
                HybridController.EvaluateJavascript("$('.footer-wrapper').hide();");
                HybridController.EvaluateJavascript("$('.content-container').css('margin-top', '0');");
                HybridController.EvaluateJavascript("$('.wo_about_wrapper_parent').css('top', '0');");


                if (IMethods.CheckConnectivity())
                {
                    if (HybirdView.Visibility != ViewStates.Visible)
                    {
                        HybirdView.Visibility = ViewStates.Visible;
                        Support_Empty.Visibility = ViewStates.Gone;
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void DefaultClientOnOnPageEventReceivedError(WebView view, IWebResourceRequest request,
            WebResourceError error, string textError)
        {
            try
            {
                HybirdView.Visibility = ViewStates.Gone;
                Support_Empty.Visibility = ViewStates.Visible;

                Txt_News_Empty.Text = textError;
                Txt_News_start.Text = error.Description;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Reload Page
        private void BtnReload_OnClick(object sender, EventArgs e)
        {
            try
            {
                HybirdView.Reload();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void OnSelection(MaterialDialog p0, View p1, int p2, ICharSequence p3)
        {
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (!string.IsNullOrEmpty(p0.Tag.ToString()))
                {
                    if (p1 == DialogAction.Positive)
                    {
                        RunOnUiThread(() =>
                        {
                            var id = p0.Tag.ToString();
                            //Fire Javascript Event
                            HybridController.EvaluateJavascript(
                                "$('#post-' + " + id + ").slideUp(200, function () { $(this).remove();}); ");
                            p0.Dismiss();
                        });

                        //Delete Post from database
                        JsBrigeInvoker.Post_Manager("delete_post", p0.Tag.ToString()).ConfigureAwait(false);
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

        #endregion
    }
}