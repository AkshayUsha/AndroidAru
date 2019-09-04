using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Gigamole.Navigationtabbar.Ntb;
using Com.Irozon.Sneaker.Interfaces;
using Com.Sothree.Slidinguppanel;
using Com.Theartofdev.Edmodo.Cropper;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using WoWonder.Activities.AddPost;
using WoWonder.Activities.Search;
using WoWonder.Activities.Story;
using WoWonder.Activities.Tabbes.Adapters;
using WoWonder.Adapters;
using WoWonder.Helpers;
using WoWonder.MediaPlayer;
using WoWonder.Services;
using File = Java.IO.File;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using SearchView = Android.Support.V7.Widget.SearchView;
using Settings = SettingsConnecter.Settings;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.Tabbes
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges =
        ConfigChanges.Keyboard | ConfigChanges.Orientation | ConfigChanges.KeyboardHidden |
        ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize |
        ConfigChanges.UiMode | ConfigChanges.Locale)]
    public class Tabbed_Main_Activity : AppCompatActivity, IOnSneakerClickListener, ViewPager.IOnPageChangeListener,
        View.IOnClickListener, View.IOnFocusChangeListener
    {

        public  ProUsers_Adapter ProUsersAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                //Window.SetFlags(WindowManagerFlags.HardwareAccelerated, WindowManagerFlags.HardwareAccelerated);
                base.OnCreate(savedInstanceState);

                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                }
                else
                {
                    //Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
                    Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                }

                IMethods.IApp.FullScreenApp(this);


                //Create your application here
                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Tabbed_Main_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Tabbed_Main_Layout);


                //App Center Errors
                //AppCenter.Start("adb501d0-8566-4078-8ae6-30fb14639509", typeof(Analytics), typeof(Crashes));

                navigationTabBar = FindViewById<NavigationTabBar>(Resource.Id.ntb_horizontal);

                viewPager = FindViewById<ViewPager>(Resource.Id.vp_horizontal_ntb);
                Tabadapter = new Main_Tab_Adapter(SupportFragmentManager);
                SearchViewBar = FindViewById<SearchView>(Resource.Id.searchView);
                if (SearchViewBar != null)
                {
                    SearchViewBar.SetIconifiedByDefault(false);
                    SearchViewBar.SetOnClickListener(this);
                    SearchViewBar.SetOnSearchClickListener(this);
                    SearchViewBar.SetOnQueryTextFocusChangeListener(this);
                }

                FloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonView);
                FloatingActionButton.Visibility = ViewStates.Invisible;

                Btn_story = FindViewById<ImageButton>(Resource.Id.storybutton);

                if (!Directory.Exists(IMethods.IPath.FolderDcimStory))
                    Directory.CreateDirectory(IMethods.IPath.FolderDcimStory);

                if (Settings.Show_Story)
                    Btn_story.Visibility = ViewStates.Visible;
                else
                    Btn_story.Visibility = ViewStates.Gone;

                Models = new JavaList<NavigationTabBar.Model>();
                AddFargmentsTabbs();


                JobRescheduble = new JobRescheduble(this, 10000);
                JobRescheduble.StartJob();

                Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);

                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                    {
                    }
                    else
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.Camera,
                            Manifest.Permission.AccessFineLocation,
                            Manifest.Permission.AccessCoarseLocation
                        }, 101);
                    }
                }
                 
                //IMethods.IApp.GetKeyHashesConfigured(this);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                SearchViewBar.ClearFocus();

                //Add Event
                if (Settings.Show_Story)
                    Btn_story.Click += CreateStories_OnClick;

                FloatingActionButton.Click += Btn_AddPsot_OnClick;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();

                //Close Event
                if (Settings.Show_Story)
                    Btn_story.Click -= CreateStories_OnClick;

                FloatingActionButton.Click -= Btn_AddPsot_OnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == CropImage.PickImagePermissionsRequestCode)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        //Open Image 
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimStory,
                            IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                            .SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                    }
                }
                else if (requestCode == CropImage.CameraCapturePermissionsRequestCode)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        CropImage.StartPickImageActivity(this);
                    else
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                }
                else if (requestCode == 101)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                    }
                }
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

                //If its from Camera or Gallery
                if (requestCode == CropImage.CropImageActivityRequestCode)
                {
                    var result = CropImage.GetActivityResult(data);

                    if (resultCode == Result.Ok)
                    {
                        var imageUri = CropImage.GetPickImageResultUri(this, data);

                        if (result.IsSuccessful)
                        {
                            var resultUri = result.Uri;

                            if (!string.IsNullOrEmpty(resultUri.Path))
                            {
                                var intent = new Intent(this, typeof(AddStory_Activity));
                                intent.PutExtra("Uri", resultUri.Path);
                                intent.PutExtra("Type", "image");
                                StartActivity(intent);
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong),
                                    ToastLength.Long).Show();
                            }
                        }
                    }
                }
                else if (requestCode == CropImage.CropImageActivityResultErrorCode)
                {
                    var result = CropImage.GetActivityResult(data);
                    Exception error = result.Error;
                }
                else if (requestCode == 100 && resultCode == Result.Ok)
                {
                    var FullPath = IMethods.MultiMedia.GetRealVideoPathFromURI(data.Data);
                    if (!string.IsNullOrEmpty(FullPath))
                    {
                        var intent = new Intent(this, typeof(AddStory_Activity));
                        intent.PutExtra("Uri", FullPath);
                        intent.PutExtra("Type", "video");
                        StartActivity(intent);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long)
                            .Show();
                    }
                }
                else if (requestCode == 2 && resultCode == Result.Ok)
                {
                    SearchViewBar.Focusable = false;
                    SearchViewBar.FocusableInTouchMode = false;
                    SearchViewBar.ClearFocus();
                }
                else if (requestCode == 2500)
                {
                    News_Feed_Tab.HybridController.EvaluateJavascript("Wo_GetNewPosts();");
                }
                else if (requestCode == 3500)
                {
                    string ID = data.GetStringExtra("PostId");
                    string Text = data.GetStringExtra("PostText");

                    string JavaCode = "$('#post-' + " + ID + ").find('#edit-post').attr('onclick', '{*type*:*edit_post*,*post_id*:*" + ID + "*,*edit_text*:*" + Text + "*}');";
                    string Decode = JavaCode.Replace("*", "&quot;");

                    News_Feed_Tab.HybridController.EvaluateJavascript(Decode);
                    News_Feed_Tab.HybridController.EvaluateJavascript("$('#post-' + " + ID + ").find('.post-description p').html('" + Text + "');");
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Open page add post
        private void Btn_AddPsot_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var Int = new Intent(this, typeof(AddPost_Activity));
                Int.PutExtra("Type", "Normal");
                Int.PutExtra("PostId", UserDetails.User_id);
                Int.PutExtra("isOwner", "Normal");
                StartActivityForResult(Int, 2500);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
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

        public JobRescheduble JobRescheduble;

        private static ViewPager viewPager;
        public News_Feed_Fragment News_Feed_Tab;
        public Notifications_Fragment Notifications_Tab;
        public Trending_Fragment Trending_Tab;
        public More_Fragment More_Tab;

        private static NavigationTabBar navigationTabBar;
        private static Main_Tab_Adapter Tabadapter;

        private static bool FirstRun;
        private static Player_Events Player_litsener;

        private SlidingUpPanelLayout SlidingUpPanel;
        public JavaList<NavigationTabBar.Model> Models;
        private Timer timer;
        private static FragmentManager frgmanager;
        public static string insertTab = "No";

        public FloatingActionButton FloatingActionButton;
        public SearchView SearchViewBar;
        public ImageButton Btn_story;

        public string search_key = "";

        #endregion

        #region Create Stories

        //Menu Create Stories >>  Image , Video
        private void CreateStories_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var ctw = new ContextThemeWrapper(this, Resource.Style.PopupMenuStyle);
                var popup = new PopupMenu(ctw, Btn_story);
                popup.MenuInflater.Inflate(Resource.Menu.CreateStories_Menu, popup.Menu);
                popup.Show();
                popup.MenuItemClick += (o, e) =>
                {
                    try
                    {
                        var Id = e.Item.ItemId;
                        switch (Id)
                        {
                            case Resource.Id.menu_Image:
                                OnImage_Button_Click();
                                break;

                            case Resource.Id.menu_Video:
                                OnVideo_Button_Click();
                                break;
                        }
                    }
                    catch (Exception exception)
                    {
                        Crashes.TrackError(exception);
                    }
                };
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Add Video
        private void OnVideo_Button_Click()
        {
            try
            {
                var intent = new Intent(Intent.ActionPick, MediaStore.Video.Media.ExternalContentUri);
                intent.SetType("video/*");
                StartActivityForResult(intent, 100);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Add Image
        private void OnImage_Button_Click()
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                    //Open Image 
                    var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimStory,
                        IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Builder()
                        .SetInitialCropWindowPaddingRatio(0)
                        .SetAutoZoomEnabled(true)
                        .SetMaxZoom(4)
                        .SetGuidelines(CropImageView.Guidelines.On)
                        .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                        .SetOutputUri(myUri).Start(this);
                }
                else
                {
                    if (CropImage.IsExplicitCameraPermissionRequired(this))
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.Camera,
                            Manifest.Permission.ReadExternalStorage
                        }, CropImage.PickImagePermissionsRequestCode);
                    }
                    else
                    {
                        //Open Image 
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimStory,
                            IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                            .SetOutputUri(myUri).Start(this);
                    }
                }
            }
            catch (Exception exe)
            {
                Crashes.TrackError(exe);
            }
        }

        #endregion

        #region Search View

        public void OnClick(View v)
        {
            if (v.Id == SearchViewBar.Id)
            {
                //Hide keyboard programmatically in MonoDroid

                var inputManager = (InputMethodManager) GetSystemService(InputMethodService);
                inputManager.HideSoftInputFromWindow(SearchViewBar.WindowToken, HideSoftInputFlags.None);

                SearchViewBar.ClearFocus();

                var intent = new Intent(this, typeof(Search_Tabbed_Activity));
                intent.PutExtra("Key", "");
                StartActivity(intent);
            }
        }

        public void OnFocusChange(View v, bool hasFocus)
        {
            try
            {
                if (v.Id == SearchViewBar.Id && hasFocus)
                {
                    var intent = new Intent(this, typeof(Search_Tabbed_Activity));
                    intent.PutExtra("Key", "");
                    StartActivity(intent);
                }

                SearchViewBar.ClearFocus();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion

        #region Set Tab

        public void SetTabStyle()
        {
            try
            {
                Models.Clear();

                if (Models != null && Models.Count <= 0)
                {
                    Models.Add(new NavigationTabBar.Model.Builder(
                            ContextCompat.GetDrawable(this, Resource.Drawable.ic_tab_home), Color.ParseColor("#ffffff"))
                        .Title(GetText(Resource.String.Lbl_News_Feed)).Build());
                    Models.Add(new NavigationTabBar.Model.Builder(
                        ContextCompat.GetDrawable(this, Resource.Drawable.ic_action_notification),
                        Color.ParseColor("#ffffff")).Title(GetText(Resource.String.Lbl_Notifcations)).Build());
                    Models.Add(new NavigationTabBar.Model.Builder(
                        ContextCompat.GetDrawable(this, Resource.Drawable.ic_action_trending),
                        Color.ParseColor("#ffffff")).Title(GetText(Resource.String.Lbl_Trending)).Build());
                    Models.Add(new NavigationTabBar.Model.Builder(
                            ContextCompat.GetDrawable(this, Resource.Drawable.ic_tab_more), Color.ParseColor("#ffffff"))
                        .Title(GetText(Resource.String.Lbl_More)).Build());

                    if (Settings.SetTabColoredTheme)
                    {
                        Models.First(a => a.Title == GetText(Resource.String.Lbl_News_Feed)).Color =
                            Color.ParseColor(Settings.TabColoredColor);
                        Models.First(a => a.Title == GetText(Resource.String.Lbl_Notifcations)).Color =
                            Color.ParseColor(Settings.TabColoredColor);
                        Models.First(a => a.Title == GetText(Resource.String.Lbl_Trending)).Color =
                            Color.ParseColor(Settings.TabColoredColor);
                        Models.First(a => a.Title == GetText(Resource.String.Lbl_More)).Color =
                            Color.ParseColor(Settings.TabColoredColor);

                        navigationTabBar.BgColor = Color.ParseColor(Settings.MainColor);
                        navigationTabBar.ActiveColor = Color.White;
                        navigationTabBar.InactiveColor = Color.White;
                    }
                    else if (Settings.SetTabDarkTheme)
                    {
                        Models.First(a => a.Title == GetText(Resource.String.Lbl_News_Feed)).Color =
                            Color.ParseColor("#444444");
                        Models.First(a => a.Title == GetText(Resource.String.Lbl_Notifcations)).Color =
                            Color.ParseColor("#444444");
                        Models.First(a => a.Title == GetText(Resource.String.Lbl_Trending)).Color =
                            Color.ParseColor("#444444");
                        Models.First(a => a.Title == GetText(Resource.String.Lbl_More)).Color =
                            Color.ParseColor("#444444");

                        navigationTabBar.BgColor = Color.ParseColor("#282828");
                        navigationTabBar.ActiveColor = Color.White;
                        navigationTabBar.InactiveColor = Color.White;
                    }

                    navigationTabBar.Models = Models;
                    navigationTabBar.SetViewPager(viewPager, 0);
                    navigationTabBar.IsScaled = false;
                    navigationTabBar.IconSizeFraction = (float) 0.450;
                    //navigationTabBar.SetBadgePosition(NavigationTabBar.BadgePosition.Center);
                    if (Settings.SetTabIsTitledWithText)
                    {
                        navigationTabBar.SetTitleMode(NavigationTabBar.TitleMode.All);
                        navigationTabBar.IsTitled = true;
                    }

                    if (!Settings.SetTabOnButton)
                    {
                        //var eee = NavigationTabBar.BadgeGravity.TopIndex = 2;
                        // navigationTabBar.SetBadgeGravity( );
                        navigationTabBar.SetBadgePosition(NavigationTabBar.BadgePosition.Center);

                        var parasms = (CoordinatorLayout.LayoutParams) navigationTabBar.LayoutParameters;
                        parasms.Gravity = (int) GravityFlags.Top;

                        // Check if we're running on Android 5.0 or higher
                        if ((int) Build.VERSION.SdkInt < 23)
                            parasms.TopMargin = 70;
                        else
                            parasms.TopMargin = 115;

                        navigationTabBar.LayoutParameters = parasms;

                        var parasms2 = (CoordinatorLayout.LayoutParams) FloatingActionButton.LayoutParameters;

                        // Check if we're running on Android 5.0 or higher
                        if ((int) Build.VERSION.SdkInt < 23)
                            parasms2.BottomMargin = 48;
                        else
                            parasms2.BottomMargin = 68;

                        FloatingActionButton.LayoutParameters = parasms2;
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        private void ViewPager_PageScrolled(object sender, ViewPager.PageScrolledEventArgs e)
        {
            try
            {
                var page = e.Position;
                if (page == 0) // News_Feed_Tab
                {
                    if (FloatingActionButton.Visibility == ViewStates.Invisible)
                        FloatingActionButton.Visibility = ViewStates.Visible;
                }
                else if (page == 1) // Notifications_Tab
                {
                    if (FloatingActionButton.Visibility == ViewStates.Visible)
                        FloatingActionButton.Visibility = ViewStates.Invisible;
                }
                else if (page == 2) // Trending_Tab
                {
                    if (FloatingActionButton.Visibility == ViewStates.Visible)
                        FloatingActionButton.Visibility = ViewStates.Invisible;

                }
                else if (page == 3) // More_Tab
                {
                    if (FloatingActionButton.Visibility == ViewStates.Visible)
                        FloatingActionButton.Visibility = ViewStates.Invisible;
                }
                else
                {
                    if (FloatingActionButton.Visibility == ViewStates.Visible)
                        FloatingActionButton.Visibility = ViewStates.Invisible;
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void AddFargmentsTabbs()
        {
            try
            {
                Tabadapter.ClaerFragment();

                frgmanager = SupportFragmentManager;

                News_Feed_Tab = new News_Feed_Fragment();
                Notifications_Tab = new Notifications_Fragment();
                Trending_Tab = new Trending_Fragment();
                More_Tab = new More_Fragment();

                if (Tabadapter != null && Tabadapter.Count <= 0)
                {
                    Tabadapter.AddFragment(News_Feed_Tab, GetText(Resource.String.Lbl_News_Feed));
                    Tabadapter.AddFragment(Notifications_Tab, GetText(Resource.String.Lbl_Notifcations));
                    Tabadapter.AddFragment(Trending_Tab, GetText(Resource.String.Lbl_Trending));
                    Tabadapter.AddFragment(More_Tab, GetText(Resource.String.Lbl_More));
                    viewPager.CurrentItem = 3;
                    viewPager.OffscreenPageLimit = Tabadapter.Count;
                    viewPager.Adapter = Tabadapter;
                    viewPager.PageScrolled += ViewPager_PageScrolled;
                    viewPager.AddOnPageChangeListener(this);
                    SetTabStyle();

                    navigationTabBar.SetZ(5);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void OnPageScrollStateChanged(int state)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            try
            {
                if (position == 0) // News_Feed_Tab
                {
                    News_Feed_Tab.ScrollView.ScrollTo(0, 0);

                  
                }
                else if (position == 1) // Notifications_Tab
                {
                   
                }
                else if (position == 2) // Trending_Tab
                {
                   
                }
                else if (position == 3) // More_Tab
                {
                    var cat = new CategoriesController();
                    cat.Get_Categories_Communities(); 
                }
                else
                {
                    
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            } 
        }

        public int Page_position;

        public void OnPageSelected(int position)
        {
            try
            {
                if (position >= 0)
                {
                    Page_position = position;

                    if (position == 0) // News_Feed_Tab
                    {
                        if (Settings.Show_ADMOB_Native)
                            MyBottomSheetCallBack.Ad_BottomSheet(News_Feed_Tab.FragmentManager);
                    }
                    else if (position == 1) // Notifications_Tab
                    {
                        var dataTab = Models.FirstOrDefault(a => a.Title == GetText(Resource.String.Lbl_Notifcations));
                        dataTab?.HideBadge();

                        if (News_Feed_Fragment.Is_RunApi)
                            Notifications_Tab.Get_GeneralData_Api(false).ConfigureAwait(false);

                        if (News_Feed_Tab.swipeRefreshLayout.Refreshing)
                            News_Feed_Tab.swipeRefreshLayout.Refreshing = false;

                        if (Settings.Show_ADMOB_Native)
                            MyBottomSheetCallBack.Ad_BottomSheet(Notifications_Tab.FragmentManager);
                    }
                    else if (position == 2) // Trending_Tab
                    {
                        var dataTab = Models.FirstOrDefault(a => a.Title == GetText(Resource.String.Lbl_Trending));
                        dataTab?.HideBadge();

                       ProUsersAdapter.NotifyDataSetChanged();

                        if (Settings.Show_LastActivities)
                             Task.Run(() => { Trending_Tab.Get_Activities(); });
                       

                        if (Settings.Show_ADMOB_Native)
                            MyBottomSheetCallBack.Ad_BottomSheet(Trending_Tab.FragmentManager);
                    }
                    else if (position == 3) // More_Tab
                    {
                        if (Settings.Show_ADMOB_Native)
                            MyBottomSheetCallBack.Ad_BottomSheet(More_Tab.FragmentManager);
                    }
                    else 
                    {

                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void OnSneakerClick(View view)
        {
        }

        #endregion
    }
}