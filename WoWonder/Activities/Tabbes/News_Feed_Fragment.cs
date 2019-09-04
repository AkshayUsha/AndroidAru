using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using AFollestad.MaterialDialogs;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using FFImageLoading;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.AddPost;
using WoWonder.Activities.Story;
using WoWonder.Activities.Story.Adapters;
using WoWonder.Helpers;
using WoWonder.Helpers.HybirdView;
using WoWonder_API;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Story;
using Client = WoWonder_API.Requests.Client;
using Exception = System.Exception;
using Fragment = Android.Support.V4.App.Fragment;
using IMethods = WoWonder.Helpers.IMethods;

namespace WoWonder.Activities.Tabbes
{
    public class News_Feed_Fragment : Fragment, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                var view = MyContextWrapper.GetContentView(Context, Settings.Lang, Resource.Layout.Tab_News_Feed_Layout);
                if (view == null) view = inflater.Inflate(Resource.Layout.Tab_News_Feed_Layout, container, false);

                coordinatorLayout = (CoordinatorLayout)view.FindViewById(Resource.Id.coordinator_Layout);
                StoryRecylerView = (RecyclerView)view.FindViewById(Resource.Id.Recyler);
                HybirdView = (WebView)view.FindViewById(Resource.Id.hybirdview);
                swipeRefreshLayout = (SwipeRefreshLayout)view.FindViewById(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                ScrollView = (NestedScrollView)view.FindViewById(Resource.Id.scroll_View);

                Stories_Layout = (LinearLayout)view.FindViewById(Resource.Id.StoriesLayout);

                News_Empty = (LinearLayout)view.FindViewById(Resource.Id.News_LinerEmpty);
                News_Icon = (TextView)view.FindViewById(Resource.Id.News_icon);
                Txt_News_Empty = (TextView)view.FindViewById(Resource.Id.Txt_LabelEmpty);
                Txt_News_start = (TextView)view.FindViewById(Resource.Id.Txt_LabelStart);
                Btn_Reload = (Button)view.FindViewById(Resource.Id.reloadPage_Button);
                IMethods.Set_TextViewIcon("2", News_Icon, "\uf119");
                News_Empty.Visibility = ViewStates.Gone;

                StoryRecylerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal,
                    false));
                StoryAdapter = new StoryAdapter(Activity);
                StoryRecylerView.SetAdapter(StoryAdapter);
                StoryRecylerView.NestedScrollingEnabled = false;

                if (!Settings.SetTabOnButton)
                {
                    var parasms = (CoordinatorLayout.LayoutParams)swipeRefreshLayout.LayoutParameters;
                    parasms.Gravity = (int)GravityFlags.Top;

                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                        parasms.TopMargin = 100;
                    else
                        parasms.TopMargin = 225;

                    swipeRefreshLayout.LayoutParameters = parasms;
                }

                HybridController = new HybirdViewController(Activity, HybirdView);

                //Set WebView and Load url to be rendered on WebView
                if (!IMethods.CheckConnectivity())
                {
                    if (Settings.EnableCachSystem)
                    {
                        if (File.Exists(IMethods.IPath.FolderDiskPost + "news_feed.html"))
                        {
                            LoadCach = true;
                            HybridController.HybirdView.Settings.CacheMode = CacheModes.CacheElseNetwork;
                            var html = File.ReadAllText(IMethods.IPath.FolderDiskPost + "news_feed.html");
                            //HybridController.HybirdView.LoadData(html, "text/html", null);
                            HybridController.HybirdView.LoadDataWithBaseURL(WoWonder_API.Client.WebsiteUrl, html, "text/html", "utf-8", null);
                        }
                        else
                        {
                            HybirdView.Visibility = ViewStates.Gone;
                            News_Empty.Visibility = ViewStates.Visible;
                            swipeRefreshLayout.Refreshing = false;

                            Txt_News_Empty.Text = GetText(Resource.String.Lbl_Empty_News);
                            Txt_News_start.Text = GetText(Resource.String.Lbl_CheckYourInternetConnection);
                        }
                    }
                    else
                    {
                        HybirdView.Visibility = ViewStates.Gone;
                        News_Empty.Visibility = ViewStates.Visible;
                        swipeRefreshLayout.Refreshing = false;

                        Txt_News_Empty.Text = GetText(Resource.String.Lbl_Empty_News);
                        Txt_News_start.Text = GetText(Resource.String.Lbl_CheckYourInternetConnection);
                    }
                }
                else
                {
                    if (Settings.ClearCachSystem)
                        HybridController.HybirdView.ClearCache(true);

                    HybirdView.Visibility = ViewStates.Visible;
                    News_Empty.Visibility = ViewStates.Gone;

                    if (Settings.LoadCachedHybirdViewOnFirstLoad)
                    {
                        if (File.Exists(IMethods.IPath.FolderDiskPost + "news_feed.html"))
                        {
                            LoadCach = true;
                            var html = File.ReadAllText(IMethods.IPath.FolderDiskPost + "news_feed.html");
                            //HybridController.HybirdView.LoadData(html, "text/html", null);
                            HybridController.HybirdView.LoadDataWithBaseURL(WoWonder_API.Client.WebsiteUrl + "/get_news_feed", html, "text/html", "utf-8", null);
                        }
                        else
                        {
                            switch (Settings.Lang)
                            {
                                case "en":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=english" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                case "ar":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=arabic" + "&type=set_c&c=" + UserDetails.Cookie);
                                    Settings.FlowDirection_RightToLeft = true;
                                    break;
                                case "de":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=german" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                case "el":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=greek" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                case "es":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=spanish" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                case "fr":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=french" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                case "it":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=italian" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                case "ja":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=japanese" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                case "nl":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=dutch" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                case "pt":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=portuguese" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                case "ro":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=romanian" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                case "ru":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=russian" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                case "sq":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=albanian" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                case "sr":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=serbian" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                case "tr":
                                    HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=turkish" + "&type=set_c&c=" + UserDetails.Cookie);
                                    break;
                                default:
                                    HybridController.LoadUrl(Current.URLS.UrlInstance.API_Get_News_Feed_Cookie + UserDetails.Cookie);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (Settings.Lang)
                        {
                            case "en":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=english" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "ar":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=arabic" + "&type=set_c&c=" + UserDetails.Cookie);
                                Settings.FlowDirection_RightToLeft = true;
                                break;
                            case "de":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=german" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "el":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=greek" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "es":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=spanish" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "fr":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=french" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "it":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=italian" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "ja":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=japanese" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "nl":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=dutch" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "pt":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=portuguese" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "ro":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=romanian" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "ru":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=russian" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "sq":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=albanian" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "sr":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=serbian" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "tr":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=turkish" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            default:
                                HybridController.LoadUrl(Current.URLS.UrlInstance.API_Get_News_Feed_Cookie + UserDetails.Cookie);
                                break;
                        }
                    }
                }

                if (Settings.Show_Story)
                {
                    Stories_Layout.Visibility = ViewStates.Visible;
                    GetStory_Api();
                }
                else
                {
                    Stories_Layout.Visibility = ViewStates.Gone;
                    Get_Notifications();
                }

                return view;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                HybridController.EvaluateJavascript("Wo_GetNewPosts();");
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();

                //Add Event
                Btn_Reload.Click += BtnReload_OnClick;
                StoryAdapter.ItemClick += StoryAdapter_OnItemClick;
                swipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
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

        public override void OnPause()
        {
            try
            {
                base.OnPause();

                //Close Event
                Btn_Reload.Click -= BtnReload_OnClick;
                StoryAdapter.ItemClick -= StoryAdapter_OnItemClick;
                swipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
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

        //Event Refresh Data Page
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    if (Settings.Show_Story)
                    {
                        StoryAdapter.Clear();
                        Stories_Layout.Visibility = ViewStates.Visible;
                        GetStory_Api();
                    }
                    else
                    {
                        Stories_Layout.Visibility = ViewStates.Gone;
                        Get_Notifications();
                    }

                    switch (Settings.Lang)
                    {
                        case "en":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=english" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        case "ar":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=arabic" + "&type=set_c&c=" + UserDetails.Cookie);
                            Settings.FlowDirection_RightToLeft = true;
                            break;
                        case "de":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=german" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        case "el":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=greek" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        case "es":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=spanish" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        case "fr":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=french" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        case "it":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=italian" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        case "ja":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=japanese" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        case "nl":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=dutch" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        case "pt":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=portuguese" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        case "ro":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=romanian" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        case "ru":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=russian" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        case "sq":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=albanian" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        case "sr":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=serbian" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        case "tr":
                            HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=turkish" + "&type=set_c&c=" + UserDetails.Cookie);
                            break;
                        default:
                            HybridController.LoadUrl(Current.URLS.UrlInstance.API_Get_News_Feed_Cookie + UserDetails.Cookie);
                            break;
                    }
                }
                else
                {
                    Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
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

        public static StoryAdapter StoryAdapter;
        public RecyclerView StoryRecylerView;

        public NestedScrollView ScrollView;
        public WebView HybirdView;
        public HybirdViewController HybridController;
        public SwipeRefreshLayout swipeRefreshLayout;
        public CoordinatorLayout coordinatorLayout;
        public LinearLayout Stories_Layout;

        public LinearLayout News_Empty;
        public TextView News_Icon;
        public TextView Txt_News_Empty;
        public TextView Txt_News_start;
        private Button Btn_Reload;

        //Run anther Api
        public static bool Is_RunApi;
        public static bool LoadCach;

        public Timer timer;
        #endregion

        #region Get Story

        public async void GetStory_Api()
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                }
                else
                {
                    var (apiStatus, respond) = await Client.Story.Get_Stories();
                    if (apiStatus == 200)
                    {
                        if (respond is Get_Stories_Object result)
                        {
                            if (result.stories.Length > 0)
                            {
                                Classes.StoryList = new Dictionary<List<Get_Stories_Object.Story>, string>(); // Key ListData , Value : user_id 
                                Classes.StoryList.Clear();


                                foreach (var story in result.stories)
                                {
                                    List<Get_Stories_Object.Story> listOfStories = new List<Get_Stories_Object.Story>();
                                    var checkUser = StoryAdapter.mStorylList.FirstOrDefault(a => a.user_id == story.user_id);
                                    if (checkUser != null)
                                    {
                                        if (Classes.StoryList == null)
                                            continue;

                                        var checkUserExits = Classes.StoryList.FirstOrDefault(a => a.Value == checkUser.user_id);
                                        if (checkUserExits.Value == null)
                                        {
                                            var ch = checkUserExits.Key.FirstOrDefault(a => a.id == checkUser.id);
                                            if (ch == null)
                                            {
                                                listOfStories.Add(story);
                                                Classes.StoryList.Add(listOfStories, story.user_id);
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in Classes.StoryList.Keys.ToList())
                                            {
                                                string userId = item.FirstOrDefault(a => a.user_id == checkUser.user_id)?.user_id;
                                                if (checkUserExits.Value == userId)
                                                {
                                                    var ch = item.FirstOrDefault(a => a.id == story.id);
                                                    if (ch == null)
                                                    {
                                                        item.Add(story);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        StoryAdapter.Add(story);

                                        listOfStories.Clear();

                                        if (Classes.StoryList == null)
                                            continue;

                                        var checkUserExits = Classes.StoryList.FirstOrDefault(a => a.Value == story.user_id);
                                        if (checkUserExits.Value == null)
                                        {
                                            listOfStories.Add(story);
                                            Classes.StoryList.Add(listOfStories, story.user_id);
                                        }
                                    }
                                }

                                this.Activity.RunOnUiThread(() =>
                                {
                                    StoryAdapter.BindEnd();
                                });
                            }
                        }
                    }
                    else if (apiStatus == 400)
                    {
                        if (respond is Error_Object error)
                        {
                            var errortext = error._errors.Error_text;
                            //Toast.MakeText(this.Context, errortext, ToastLength.Short).Show();

                            if (errortext.Contains("Invalid or expired access_token"))
                                API_Request.Logout(Activity);
                        }
                    }
                    else if (apiStatus == 404)
                    {
                        var error = respond.ToString();
                        //Toast.MakeText(this.Context, error, ToastLength.Short).Show();
                    }
                }

                Get_Notifications();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void StoryAdapter_OnItemClick(object sender, StoryAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = StoryAdapter.GetItem(position);
                    if (item != null)
                    {
                        StoryAdapter.Update();

                        var Int = new Intent(Context, typeof(View_Story_Activity));
                        Int.PutExtra("Story", JsonConvert.SerializeObject(item));
                        Int.PutExtra("Story_Position", position.ToString());
                        StartActivity(Int);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

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
                            Activity.RunOnUiThread(() =>
                            {
                                try
                                {
                                    var DialogList = new MaterialDialog.Builder(Context);
                                    DialogList.Tag(data["post_id"].ToString());
                                    DialogList.Title(Context.GetText(Resource.String.Lbl_Title_DeletePost));
                                    DialogList.Content(Context.GetText(Resource.String.Lbl_Content_DeletePost));
                                    DialogList.NegativeText(Context.GetText(Resource.String.Lbl_Cancel));
                                    DialogList.OnNegative(this);
                                    DialogList.PositiveText(Context.GetText(Resource.String.Lbl_Delete));
                                    DialogList.OnPositive(this);
                                    DialogList.Build().Show();
                                }
                                catch (Exception exception)
                                {
                                    Crashes.TrackError(exception);
                                }
                            });
                        else if (type == "publisher-box")
                            Activity.RunOnUiThread(() =>
                            {
                                var Int = new Intent(Activity, typeof(AddPost_Activity));
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

                Is_RunApi = true;

                //Get My Data
                var data = API_Request.Get_MyProfileData_Api(Activity).ConfigureAwait(false);

                var dbDatabase = new SqLiteDatabase();
                dbDatabase.Get_MyProfile_CredentialList();
                dbDatabase.Dispose();

                var cat = new CategoriesController();
                cat.Get_Categories_Communities();

                if (IMethods.CheckConnectivity())
                {
                    if (Settings.EnableCachSystem)
                        HybridController.EvaluateJavascript(!LoadCach
                            ? "(function() { return ('<html>'+document.getElementsByTagName('html')[0].innerHTML+'</html>'); })();"
                            : "Wo_GetNewPosts();");

                    if (LoadCach)
                    {
                        switch (Settings.Lang)
                        {
                            case "en":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=english" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "ar":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=arabic" + "&type=set_c&c=" + UserDetails.Cookie);
                                Settings.FlowDirection_RightToLeft = true;
                                break;
                            case "de":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=german" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "el":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=greek" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "es":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=spanish" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "fr":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=french" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "it":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=italian" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "ja":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=japanese" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "nl":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=dutch" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "pt":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=portuguese" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "ro":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=romanian" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "ru":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=russian" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "sq":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=albanian" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "sr":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=serbian" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            case "tr":
                                HybridController.LoadUrl(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone" + "&lang=turkish" + "&type=set_c&c=" + UserDetails.Cookie);
                                break;
                            default:
                                HybridController.LoadUrl(Current.URLS.UrlInstance.API_Get_News_Feed_Cookie + UserDetails.Cookie);
                                break;
                        }
                    }

                    if (HybirdView.Visibility != ViewStates.Visible)
                    {
                        HybirdView.Visibility = ViewStates.Visible;
                        News_Empty.Visibility = ViewStates.Gone;
                    }

                    LoadCach = false;
                }

                //Refresh Web Timer
                Timer timer = new Timer();
                if (!timer.Enabled)
                {
                    timer.Interval = Settings.RefreshWebSeconds;
                    timer.Elapsed += TimerOnElapsed;
                    timer.Enabled = true;
                    timer.Start();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public async void Get_Notifications()
        {
            try
            {
                var (countNotifications, countFriend, countMessages) = await ((Tabbed_Main_Activity)Context).Notifications_Tab.Get_GeneralData_Api(false).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(countNotifications) && countNotifications != "0")
                    Activity.RunOnUiThread(() =>
                    {
                        var dataTab = ((Tabbed_Main_Activity)Context).Models.FirstOrDefault(a => a.Title == GetText(Resource.String.Lbl_Notifcations));
                        if (dataTab != null)
                        {
                            dataTab.BadgeTitle = countNotifications;
                            dataTab.UpdateBadgeTitle(countNotifications);
                            dataTab.ShowBadge();
                        }
                    });

                if (!string.IsNullOrEmpty(countFriend) && countFriend != "0")
                    Activity.RunOnUiThread(() =>
                    {
                        var dataTab =
                            ((Tabbed_Main_Activity)Context).Models.FirstOrDefault(a =>
                                a.Title == GetText(Resource.String.Lbl_Trending));
                        if (dataTab != null)
                        {
                            dataTab.BadgeTitle = countFriend;
                            dataTab.UpdateBadgeTitle(countFriend);
                            dataTab.ShowBadge();
                        }
                    });


                if (Settings.Messenger_Integration)
                {
                    if (!string.IsNullOrEmpty(countMessages) && countMessages != "0")
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            var listMore = ((Tabbed_Main_Activity)Context).More_Tab.MoreSectionAdapter.SectionList;
                            if (listMore != null)
                            {
                                var dataTab = listMore.FirstOrDefault(a => a.ID == 2);
                                if (dataTab != null)
                                {
                                    dataTab.BadgeCount = int.Parse(countMessages);
                                    dataTab.Badgevisibilty = true;
                                    dataTab.IconColor = Color.ParseColor("#b71c1c");

                                    ((Tabbed_Main_Activity)Context).More_Tab.MoreSectionAdapter.NotifyItemChanged(listMore.IndexOf(dataTab));
                                }
                            }
                        });
                    }
                    else
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            var listMore = ((Tabbed_Main_Activity)Context).More_Tab.MoreSectionAdapter.SectionList;
                            if (listMore != null)
                            {
                                var dataTab = listMore.FirstOrDefault(a => a.ID == 2);
                                if (dataTab != null)
                                {
                                    dataTab.BadgeCount = 0;
                                    dataTab.Badgevisibilty = false;
                                    dataTab.IconColor = Color.ParseColor("#03a9f4");

                                    ((Tabbed_Main_Activity)Context).More_Tab.MoreSectionAdapter.NotifyItemChanged(listMore.IndexOf(dataTab));
                                }
                            }
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void DefaultClientOnOnPageEventReceivedError(WebView view, IWebResourceRequest request, WebResourceError error, string textError)
        {
            try
            {
                if (!LoadCach)
                {
                    HybirdView.Visibility = ViewStates.Gone;
                    News_Empty.Visibility = ViewStates.Visible;

                    Txt_News_Empty.Text = textError;
                    Txt_News_start.Text = error.Description;
                }
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
                        Activity.RunOnUiThread(() =>
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
