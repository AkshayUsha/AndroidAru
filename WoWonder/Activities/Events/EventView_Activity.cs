using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Com.Devs.ReadMoreOptionLib;
using FFImageLoading;
using FFImageLoading.Views;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.AddPost;
using WoWonder.Helpers;
using WoWonder.Helpers.HybirdView;
using WoWonder_API;
using WoWonder_API.Classes.Event;
using Client = WoWonder_API.Requests.Client;
using Exception = System.Exception;
using IMethods = WoWonder.Helpers.IMethods;

namespace WoWonder.Activities.Events
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class EventView_Activity : AppCompatActivity, IOnMapReadyCallback, ILocationListener,
        GoogleMap.IOnInfoWindowClickListener, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                if ((int) Build.VERSION.SdkInt >= 23)
                {
                    Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
                    Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                }

                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);
                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.EventView_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.EventView_Layout);

                News_Empty = (LinearLayout) FindViewById(Resource.Id.News_LinerEmpty);
                News_Icon = (TextView) FindViewById(Resource.Id.News_icon);
                Txt_News_Empty = (TextView) FindViewById(Resource.Id.Txt_LabelEmpty);
                Txt_News_start = (TextView) FindViewById(Resource.Id.Txt_LabelStart);
                Btn_Reload = (Button) FindViewById(Resource.Id.reloadPage_Button);
                IMethods.Set_TextViewIcon("2", News_Icon, "\uf119");
                News_Empty.Visibility = ViewStates.Gone;

                Txt_Name = FindViewById<TextView>(Resource.Id.tvName_ptwo);
                IconGoing = FindViewById<TextView>(Resource.Id.IconGoing);
                Txt_Going = FindViewById<TextView>(Resource.Id.GoingTextview);
                IconInterested = FindViewById<TextView>(Resource.Id.IconInterested);
                Txt_Interested = FindViewById<TextView>(Resource.Id.InterestedTextview);
                Txt_StartDate = FindViewById<TextView>(Resource.Id.txtStartDate);
                Txt_EndDate = FindViewById<TextView>(Resource.Id.txtEndDate);

                IconLocation = FindViewById<TextView>(Resource.Id.IconLocation);
                Txt_Location = FindViewById<TextView>(Resource.Id.LocationTextview);
                Txt_Description = FindViewById<TextView>(Resource.Id.tv_aboutdescUser);

                HybirdView = (WebView) FindViewById(Resource.Id.hybirdview);

                ImageEventCover = FindViewById<ImageViewAsync>(Resource.Id.EventCover);
                IconBack = FindViewById<ImageView>(Resource.Id.back);

                Btn_Go = FindViewById<Button>(Resource.Id.ButtonGoing);
                Btn_Intersted = FindViewById<Button>(Resource.Id.ButtonIntersted);

                FloatingActionButtonView = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonView);

                IMethods.Set_TextViewIcon("1", IconGoing, IonIcons_Fonts.IosPeople);
                IMethods.Set_TextViewIcon("1", IconInterested, IonIcons_Fonts.AndroidStar);
                IMethods.Set_TextViewIcon("1", IconLocation, IonIcons_Fonts.Location);

                var mapFragment = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
                mapFragment.GetMapAsync(this);

                //Set to event id to load the news feed
                HybridController = new HybirdViewController(this, HybirdView, null);

                Get_Data_Event();
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
                IconBack.Click += IconBack_OnClick;
                FloatingActionButtonView.Click += Btn_AddPsot_OnClick;
                Btn_Go.Click += BtnGo_OnClick;
                Btn_Intersted.Click += BtnIntersted_OnClick;

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
                IconBack.Click -= IconBack_OnClick;
                FloatingActionButtonView.Click -= Btn_AddPsot_OnClick;
                Btn_Go.Click -= BtnGo_OnClick;
                Btn_Intersted.Click -= BtnIntersted_OnClick;

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


        private void Get_Data_Event()
        {
            try
            {
                _Event_Data = JsonConvert.DeserializeObject<Get_Events_Object.Event>(Intent.GetStringExtra("EventView"));
                if (_Event_Data != null)
                {
                    var CoverSplit = _Event_Data.cover.Split('/').Last();
                    var getImage_Cover =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskEvent, CoverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(ImageEventCover, "ImagePlacholder.jpg", getImage_Cover);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskEvent,
                            _Event_Data.cover);
                        ImageServiceLoader.Load_Image(ImageEventCover, "ImagePlacholder.jpg", _Event_Data.cover);
                    }

                    Txt_Name.Text = _Event_Data.name;

                    Txt_Going.Text = _Event_Data.going_count + " " + GetText(Resource.String.Lbl_GoingPeople);
                    Txt_Interested.Text = _Event_Data.interested_count + " " +
                                          GetText(Resource.String.Lbl_InterestedPeople);
                    Txt_Location.Text = _Event_Data.location;

                    Txt_StartDate.Text = _Event_Data.start_date;
                    Txt_EndDate.Text = _Event_Data.end_date;

                    var Description =
                        IMethods.Fun_String.DecodeString(
                            IMethods.Fun_String.DecodeStringWithEnter(_Event_Data.description));

                    var readMoreOption = new ReadMoreOption.Builder(this)
                        .TextLength(250)
                        .MoreLabel(GetText(Resource.String.Lbl_ReadMore))
                        .LessLabel(GetText(Resource.String.Lbl_ReadLess))
                        .MoreLabelColor(Color.ParseColor(Settings.MainColor))
                        .LessLabelColor(Color.ParseColor(Settings.MainColor))
                        .LabelUnderLine(true)
                        .Build();
                    readMoreOption.AddReadMoreTo(Txt_Description, Description);

                    if (_Event_Data.is_going)
                    {
                        Btn_Go.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                        Btn_Go.SetTextColor(Color.ParseColor("#ffffff"));
                        Btn_Go.Text = GetText(Resource.String.Lbl_Going);
                        Btn_Go.Tag = "true";
                    }
                    else
                    {
                        Btn_Go.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                        Btn_Go.SetTextColor(Color.ParseColor(Settings.MainColor));
                        Btn_Go.Text = GetText(Resource.String.Lbl_Go);
                        Btn_Go.Tag = "false";
                    }

                    if (_Event_Data.is_interested)
                    {
                        Btn_Intersted.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                        Btn_Intersted.SetTextColor(Color.ParseColor("#ffffff"));
                        Btn_Intersted.Text = GetText(Resource.String.Lbl_Interested);
                        Btn_Intersted.Tag = "true";
                    }
                    else
                    {
                        Btn_Intersted.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                        Btn_Intersted.SetTextColor(Color.ParseColor(Settings.MainColor));
                        Btn_Intersted.Text = GetText(Resource.String.Lbl_Interested);
                        Btn_Intersted.Tag = "false";
                    }

                    //Set WebView and Load url to be rendered on WebView
                    if (!IMethods.CheckConnectivity())
                    {
                        HybirdView.Visibility = ViewStates.Gone;
                        News_Empty.Visibility = ViewStates.Visible;

                        Txt_News_Empty.Text = GetText(Resource.String.Lbl_Empty_News);
                        Txt_News_start.Text = GetText(Resource.String.Lbl_CheckYourInternetConnection);
                    }
                    else
                    {
                        HybirdView.Visibility = ViewStates.Visible;
                        News_Empty.Visibility = ViewStates.Gone;

                        if (Settings.ClearCachSystem)
                            HybridController.HybirdView.ClearCache(true);

                        if (Settings.FlowDirection_RightToLeft)
                            HybridController.LoadUrl(Current.URLS.UrlInstance.API_Get_News_Feed_Event + _Event_Data.id +
                                                     "&lang=arabic");
                        else
                            HybridController.LoadUrl(Current.URLS.UrlInstance.API_Get_News_Feed_Event + _Event_Data.id);
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

        #region  Variables Basic

        public GoogleMap Map;
        public double CurrentLongitude;
        public double CurrentLatitude;

        public LinearLayout News_Empty;
        public TextView News_Icon;
        public TextView Txt_News_Empty;
        public TextView Txt_News_start;
        private Button Btn_Reload;


        public TextView Txt_Name;

        public TextView IconGoing;
        public TextView Txt_Going;
        public TextView IconInterested;
        public TextView Txt_Interested;
        public TextView Txt_StartDate;
        public TextView Txt_EndDate;
        public TextView IconLocation;
        public TextView Txt_Location;
        public TextView Txt_Description;

        private FloatingActionButton FloatingActionButtonView;

        public ImageViewAsync ImageEventCover;
        private ImageView IconBack;

        private Button Btn_Go, Btn_Intersted;

        public WebView HybirdView;
        public HybirdViewController HybridController;

        private Get_Events_Object.Event _Event_Data;

        #endregion

        #region Event

        //Event Go
        private void BtnGo_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (Btn_Go.Tag.ToString() == "false")
                {
                    Btn_Go.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    Btn_Go.SetTextColor(Color.ParseColor("#ffffff"));
                    Btn_Go.Text = GetText(Resource.String.Lbl_Going);
                    Btn_Go.Tag = "true";
                }
                else
                {
                    Btn_Go.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    Btn_Go.SetTextColor(Color.ParseColor(Settings.MainColor));
                    Btn_Go.Text = GetText(Resource.String.Lbl_Go);
                    Btn_Go.Tag = "false";
                }                 
                var dataEvent = Event_Fragment.MEventAdapter?.mEventList?.FirstOrDefault(a => a.id == _Event_Data.id);
                if (dataEvent != null)
                {
                    dataEvent.is_going = Convert.ToBoolean(Btn_Go.Tag);
                }

                var result = Client.Event.Go_To_Event(_Event_Data.id).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Intersted
        private void BtnIntersted_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (Btn_Intersted.Tag.ToString() == "false")
                {
                    Btn_Intersted.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    Btn_Intersted.SetTextColor(Color.ParseColor("#ffffff"));
                    Btn_Intersted.Text = GetText(Resource.String.Lbl_Interested);
                    Btn_Intersted.Tag = "true";
                }
                else
                {
                    Btn_Intersted.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    Btn_Intersted.SetTextColor(Color.ParseColor(Settings.MainColor));
                    Btn_Intersted.Text = GetText(Resource.String.Lbl_Interested);
                    Btn_Intersted.Tag = "false";
                }


                var dataEvent = Event_Fragment.MEventAdapter?.mEventList?.FirstOrDefault(a => a.id == _Event_Data.id);
                if (dataEvent != null)
                {
                    dataEvent.is_interested = Convert.ToBoolean(Btn_Intersted.Tag);
                }

                var result = Client.Event.Interest_Event(_Event_Data.id).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Icon back
        private void IconBack_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                Finish();
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
                Int.PutExtra("PostId", _Event_Data.id);
                Int.PutExtra("isOwner", "SocialEvent");
                StartActivityForResult(Int, 2500);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion

        #region Location

        public async void OnMapReady(GoogleMap googleMap)
        {
            try
            {
                var loction = await API_Request.GetNameLocation(_Event_Data.location);
                if (loction.Count > 0)
                {
                    CurrentLatitude = loction[0].geometry.location.lat;
                    CurrentLongitude = loction[0].geometry.location.lng;
                }

                Map = googleMap;

                //Optional
                googleMap.UiSettings.MapToolbarEnabled = true;
                googleMap.UiSettings.ZoomControlsEnabled = false;
                googleMap.UiSettings.CompassEnabled = false;
                googleMap.MoveCamera(CameraUpdateFactory.ZoomIn());

                var makerOptions = new MarkerOptions();
                makerOptions.SetPosition(new LatLng(CurrentLatitude, CurrentLongitude));
                makerOptions.SetTitle(GetText(Resource.String.Lbl_EventPlace));

                Map.AddMarker(makerOptions);
                Map.SetOnInfoWindowClickListener(this); // Add event click on marker icon
                Map.MapType = GoogleMap.MapTypeNormal;

                var builder = CameraPosition.InvokeBuilder();
                builder.Target(new LatLng(CurrentLatitude, CurrentLongitude));
                var cameraPosition = builder.Zoom(16).Target(new LatLng(CurrentLatitude, CurrentLongitude)).Build();
                cameraPosition.Zoom = 16;

                var cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                Map.MoveCamera(cameraUpdate);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnLocationChanged(Location location)
        {
            try
            {
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnProviderDisabled(string provider)
        {
            try
            {
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnProviderEnabled(string provider)
        {
            try
            {
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            try
            {
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnInfoWindowClick(Marker marker)
        {
            try
            {
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
                if (IMethods.CheckConnectivity())
                {
                    if (HybirdView.Visibility != ViewStates.Visible)
                    {
                        HybirdView.Visibility = ViewStates.Visible;
                        News_Empty.Visibility = ViewStates.Gone;
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
                News_Empty.Visibility = ViewStates.Visible;

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