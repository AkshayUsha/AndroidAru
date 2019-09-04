using System;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Ima;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Upstream.Cache;
using Com.Google.Android.Exoplayer2.Util;
using Com.Luseen.Autolinklibrary;
using Java.Lang;
using Java.Net;
using Microsoft.AppCenter.Crashes;
using Plugin.Share;
using Plugin.Share.Abstractions;
using SettingsConnecter;
using WoWonder.Activities.Videos;
using WoWonder.MediaPlayer;
using WoWonder_API.Classes.Movies;
using Exception = System.Exception;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;


namespace WoWonder.Helpers
{
    public class VideoController : Java.Lang.Object, View.IOnClickListener, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private Activity ActivityContext { get; set; }
        private string ActivityName { get; set; }

        private Player Factory;
        private IDataSourceFactory DefaultDatamediaFactory;
        private static SimpleExoPlayer Player { get; set; }

        private ImaAdsLoader ImaAdsLoader;
        private AdsController ImAdsController;
        private Player_Events PlayerLitsener;
        private static PlayerView FullscreenplayerView;

        private PlayerView simpleExoPlayerView;
        private FrameLayout playerframeLyout;
        private TextView QualityiconView;
        private TextView ViewsiconView;
        private TextView ShareiconView;
        private TextView MoreiconView;
        private TextView ShowMoreDiscriptioniconView;
        private LinearLayout videoDescriptionLayout;
        private Button retryButton;
        private FrameLayout MainvideoFrameLayout;
        public ImageView Download_icon;
        private ImageView Exo_back_button;
        private LinearLayout Exo_topLayout;
        private PlaybackControlView controlView;
        private ProgressBar Loadingprogress_bar;
        private ImageButton videoPlayButton;
        private ImageButton videoResumeButton;

        private ImageView mFullScreenIcon;
        private FrameLayout mFullScreenButton;
        private ImageView ShareIcon;
        private FrameLayout Menue_button;

        private LinearLayout Share_Button;
        private LinearLayout More_Button;

        //Dragble video player info
        private TextView Video_Titile;

        private TextView Video_QualityTextView;
        private TextView Video_ViewsNumber;
        private TextView Video_videoDate;
        private AutoLinkTextView Video_videoDescription;
        private TextView Video_videoCategory;
        private TextView Video_Stars;
        private TextView Video_Tag;

        private static ViewGroup adOverlayViewGroup;
        private static IMediaSource videoSource;
        private static DefaultBandwidthMeter BANDWIDTH_METER = new DefaultBandwidthMeter();
        private static Java.Net.CookieManager DEFAULT_COOKIE_MANAGER;
        private static Handler mainHandler;
        private static ExctractorMediaListener eventLogger;

        private static int resumeWindow;
        private static long resumePosition;

        private TextSanitizer TextSanitizerAutoLink;
        private static VideoDownloadAsyncControler VideoControler;

        public Get_Movies_Object.Movie Videodata;

        #endregion

        public VideoController(Activity activity, string activtyName)
        {
            try
            {
                DEFAULT_COOKIE_MANAGER = new CookieManager();
                DEFAULT_COOKIE_MANAGER.SetCookiePolicy(CookiePolicy.AcceptOriginalServer);

                ActivityName = activtyName;
                ActivityContext = activity;

                Initialize();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Initialize()
        {
            try
            {
                PlayerLitsener = new Player_Events(ActivityContext, controlView);

                if (ActivityName != "FullScreen")
                {
                    simpleExoPlayerView = ActivityContext.FindViewById<PlayerView>(Resource.Id.player_view);
                    simpleExoPlayerView.SetControllerVisibilityListener(PlayerLitsener);
                    simpleExoPlayerView.RequestFocus();

                    //Player initialize
                    controlView = simpleExoPlayerView.FindViewById<PlaybackControlView>(Resource.Id.exo_controller);
                    PlayerLitsener = new Player_Events(ActivityContext, controlView);

                    Exo_topLayout = controlView.FindViewById<LinearLayout>(Resource.Id.topLayout);
                    Exo_back_button = controlView.FindViewById<ImageView>(Resource.Id.backIcon);
                    Download_icon = controlView.FindViewById<ImageView>(Resource.Id.Download_icon);
                    mFullScreenIcon = controlView.FindViewById<ImageView>(Resource.Id.exo_fullscreen_icon);
                    mFullScreenButton = controlView.FindViewById<FrameLayout>(Resource.Id.exo_fullscreen_button);
                    ShareIcon = controlView.FindViewById<ImageView>(Resource.Id.share_icon);
                    Menue_button = controlView.FindViewById<FrameLayout>(Resource.Id.exo_menue_button);
                    videoPlayButton = controlView.FindViewById<ImageButton>(Resource.Id.exo_play);
                    videoResumeButton = controlView.FindViewById<ImageButton>(Resource.Id.exo_pause);

                    MainvideoFrameLayout = ActivityContext.FindViewById<FrameLayout>(Resource.Id.root);
                    MainvideoFrameLayout.SetOnClickListener(this);

                    Loadingprogress_bar = ActivityContext.FindViewById<ProgressBar>(Resource.Id.progress_bar);

                    QualityiconView = ActivityContext.FindViewById<TextView>(Resource.Id.Qualityicon);
                    ViewsiconView = ActivityContext.FindViewById<TextView>(Resource.Id.Viewsicon);
                    ShareiconView = ActivityContext.FindViewById<TextView>(Resource.Id.Shareicon);
                    MoreiconView = ActivityContext.FindViewById<TextView>(Resource.Id.Moreicon);
                    ShowMoreDiscriptioniconView =ActivityContext.FindViewById<TextView>(Resource.Id.video_ShowDiscription);
                    videoDescriptionLayout =ActivityContext.FindViewById<LinearLayout>(Resource.Id.videoDescriptionLayout);
                   

                    Share_Button = ActivityContext.FindViewById<LinearLayout>(Resource.Id.ShareButton);
                    Share_Button.Click += ShareIcon_Click;

                    More_Button = ActivityContext.FindViewById<LinearLayout>(Resource.Id.moreButton);
                    More_Button.Click += MoreButton_OnClick;

                    Video_Titile = ActivityContext.FindViewById<TextView>(Resource.Id.video_Titile);
                    Video_QualityTextView = ActivityContext.FindViewById<TextView>(Resource.Id.QualityTextView);
                    Video_ViewsNumber = ActivityContext.FindViewById<TextView>(Resource.Id.ViewsNumber);
                    Video_videoDate = ActivityContext.FindViewById<TextView>(Resource.Id.videoDate);
                    Video_videoDescription =ActivityContext.FindViewById<AutoLinkTextView>(Resource.Id.videoDescriptionTextview);
                    Video_videoCategory = ActivityContext.FindViewById<TextView>(Resource.Id.videoCategorytextview);

                    Video_Stars = ActivityContext.FindViewById<TextView>(Resource.Id.videoStarstextview);
                    Video_Tag   = ActivityContext.FindViewById<TextView>(Resource.Id.videoTagtextview);

                    TextSanitizerAutoLink = new TextSanitizer(Video_videoDescription, ActivityContext);

                    IMethods.Set_TextViewIcon("1", QualityiconView, IonIcons_Fonts.RibbonA);
                    IMethods.Set_TextViewIcon("1", ViewsiconView, IonIcons_Fonts.Eye);
                    IMethods.Set_TextViewIcon("1", ShareiconView, IonIcons_Fonts.ReplyAll);
                    IMethods.Set_TextViewIcon("1", MoreiconView, IonIcons_Fonts.PlusCircled);
                    IMethods.Set_TextViewIcon("1", ShowMoreDiscriptioniconView, IonIcons_Fonts.ArrowDownB);
                     
                    ShowMoreDiscriptioniconView.Visibility = ViewStates.Gone;

                    videoDescriptionLayout.Visibility = ViewStates.Visible;

                    if (!mFullScreenButton.HasOnClickListeners)
                        mFullScreenButton.SetOnClickListener(this);

                    if (!Exo_back_button.HasOnClickListeners)
                    {
                        Exo_back_button.Click += BackIcon_Click;
                        Download_icon.Click += Download_icon_Click;
                        ShareIcon.Click += ShareIcon_Click;
                        //Menue_button.Click += Menue_button_Click;

                        Menue_button.Visibility = ViewStates.Gone;
                    } 
                }
                else
                {
                    FullscreenplayerView = ActivityContext.FindViewById<PlayerView>(Resource.Id.player_view2);
                    controlView = FullscreenplayerView.FindViewById<PlaybackControlView>(Resource.Id.exo_controller);
                    PlayerLitsener = new Player_Events(ActivityContext, controlView);

                    Exo_topLayout = controlView.FindViewById<LinearLayout>(Resource.Id.topLayout);
                    Exo_back_button = controlView.FindViewById<ImageView>(Resource.Id.backIcon);
                    Download_icon = controlView.FindViewById<ImageView>(Resource.Id.Download_icon);
                    mFullScreenIcon = controlView.FindViewById<ImageView>(Resource.Id.exo_fullscreen_icon);
                    mFullScreenButton = controlView.FindViewById<FrameLayout>(Resource.Id.exo_fullscreen_button);
                    ShareIcon = controlView.FindViewById<ImageView>(Resource.Id.share_icon);
                    Menue_button = controlView.FindViewById<FrameLayout>(Resource.Id.exo_menue_button);
                    videoPlayButton = controlView.FindViewById<ImageButton>(Resource.Id.exo_play);
                    videoResumeButton = controlView.FindViewById<ImageButton>(Resource.Id.exo_pause);

                    if (!mFullScreenButton.HasOnClickListeners)
                        mFullScreenButton.SetOnClickListener(this);

                    if (!Exo_back_button.HasOnClickListeners)
                    {
                        Exo_back_button.Click += BackIcon_Click;
                        Download_icon.Click += Download_icon_Click;
                        ShareIcon.Click += ShareIcon_Click;
                        //Menue_button.Click += Menue_button_Click;

                        Menue_button.Visibility = ViewStates.Gone;
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        
        public void PlayVideo(string videoUrL, Get_Movies_Object.Movie videoObject)
        {
            try
            {
                if (videoObject != null)
                {
                    Videodata = videoObject;

                    LoadVideo_Data(videoObject);

                    ReleaseVideo();

                    if (Settings.Allow_Offline_Download == false)
                        Download_icon.Visibility = ViewStates.Gone;

                    mFullScreenIcon.SetImageDrawable(ActivityContext.GetDrawable(Resource.Drawable.ic_action_ic_fullscreen_expand));

                    Android.Net.Uri videoURL;
                    if (!string.IsNullOrEmpty(videoUrL))
                    {
                        videoURL = Android.Net.Uri.Parse(videoUrL);
                    }
                    else
                    {
                        videoURL = Android.Net.Uri.Parse(Videodata.source);
                    }

                    mainHandler = new Handler();

                    AdaptiveTrackSelection.Factory videoTrackSelectionFactory =new AdaptiveTrackSelection.Factory(BANDWIDTH_METER);
                    DefaultTrackSelector trackSelector = new DefaultTrackSelector(videoTrackSelectionFactory);

                    Player = ExoPlayerFactory.NewSimpleInstance(ActivityContext, trackSelector);
                    DefaultDatamediaFactory = new DefaultDataSourceFactory(ActivityContext,Util.GetUserAgent(ActivityContext, Settings.Application_Name), BANDWIDTH_METER);

                    // Produces DataSource instances through which media data is loaded.
                    ExtractorMediaSource DefaultSource = new ExtractorMediaSource(videoURL, DefaultDatamediaFactory,new DefaultExtractorsFactory(), mainHandler, eventLogger);

                    videoSource = null;

                    //Set Interactive Media Ads 
                    if (Player_Settings.ShowInteractiveMediaAds)
                        videoSource = CreateAdsMediaSource(DefaultSource, Player_Settings.IMAdsUri);

                    if (simpleExoPlayerView == null)
                        Initialize();

                    //Set Cache Media Load
                    if (Player_Settings.EnableOfflineMode)
                    {
                        videoSource = videoSource == null? CreateCacheMediaSource(DefaultSource, videoURL): CreateCacheMediaSource(videoSource, videoURL);
                        if (videoSource != null)
                        {
                            Download_icon.SetImageDrawable(ActivityContext.GetDrawable(Resource.Drawable.ic_checked_red));
                            Download_icon.Tag = "Downloaded";

                            simpleExoPlayerView.Player = Player;
                            Player.Prepare(videoSource);
                            Player.AddListener(PlayerLitsener);
                            Player.PlayWhenReady = true;

                            bool haveResumePosition = resumeWindow != C.IndexUnset;
                            if (haveResumePosition)
                                Player.SeekTo(resumeWindow, resumePosition);

                            return;
                        }
                    }

                    if (videoSource == null)
                    {
                        if (!string.IsNullOrEmpty(videoUrL))
                        {
                            if (videoUrL.Contains("youtube") || videoUrL.Contains("Youtube") || videoUrL.Contains("youtu"))
                            {
                                Task.Run(async () =>
                                {
                                    var newurl = await VideoInfoRetriever.GetEmbededVideo(Videodata.source);
                                    videoSource = CreateDefaultMediaSource(Android.Net.Uri.Parse(newurl));
                                });
                            }
                            else
                            {
                                videoSource = CreateDefaultMediaSource(Android.Net.Uri.Parse(videoUrL));

                                simpleExoPlayerView.Player = Player;
                                Player.Prepare(videoSource);
                                Player.AddListener(PlayerLitsener);
                                Player.PlayWhenReady = true;

                                bool haveResumePosition = resumeWindow != C.IndexUnset;
                                if (haveResumePosition)
                                    Player.SeekTo(resumeWindow, resumePosition);
                            }
                        }
                    }
                    else
                    {
                        simpleExoPlayerView.Player = Player;
                        Player.Prepare(videoSource);
                        Player.AddListener(PlayerLitsener);
                        Player.PlayWhenReady = true;

                        bool haveResumePosition = resumeWindow != C.IndexUnset;
                        if (haveResumePosition)
                            Player.SeekTo(resumeWindow, resumePosition);
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void ReleaseVideo()
        {
            try
            {
                if (Player != null)
                {
                    Player?.Release();
                    Player = null;

                    //GC Collecter
                    GC.Collect();
                }

                if (Download_icon.Tag.ToString() != "false")
                {
                    Download_icon.SetImageDrawable(ActivityContext.GetDrawable(Resource.Drawable.ic_action_download));
                    Download_icon.Tag = "false";
                }
                
                videoDescriptionLayout.Visibility = ViewStates.Visible;
                
                if (Videodata != null)
                {
                    Video_Titile.Text = Videodata.name;
                }

                simpleExoPlayerView.Player = null;

             }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void SetStopvideo()
        {
            try
            {
                if (simpleExoPlayerView.Player != null)
                {
                    if (simpleExoPlayerView.Player.PlaybackState == Com.Google.Android.Exoplayer2.Player.StateReady)
                    {
                        simpleExoPlayerView.Player.PlayWhenReady = false;
                    }

                    //GC Collecter
                    GC.Collect();
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void LoadVideo_Data(Get_Movies_Object.Movie videoObject)
        {
            try
            {
                if (ActivityName != "Viewer_Video")
                {
                    Exo_back_button.SetImageResource(Resource.Drawable.ic_action_arrow_down_sign);
                    Exo_back_button.Tag = "Open";
                }

                if (videoObject != null)
                {
                    Video_Titile.Text = videoObject.name;

                    Video_QualityTextView.Text = videoObject.quality.ToUpperInvariant(); 
                    Video_ViewsNumber.Text = videoObject.views + " " + ActivityContext.GetText(Resource.String.Lbl_Views);
                    Video_videoDate.Text = ActivityContext.GetText(Resource.String.Lbl_Published_on) + " " + videoObject.release;
                    Video_videoDescription.Text = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(videoObject.description));
                    Video_videoCategory.Text = videoObject.genre;
                    Video_Stars.Text = videoObject.stars;
                    Video_Tag.Text = videoObject.producer;
                    
                    TextSanitizerAutoLink.Load(videoObject.description);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Video player

        public IMediaSource CreateCacheMediaSource(IMediaSource videoSource, Android.Net.Uri videoUrL)
        {
            try
            {
                if (Player_Settings.EnableOfflineMode)
                {
                    //Set the video for offline mode 
                    if (!string.IsNullOrEmpty(Videodata.id))
                    {
                        string file = VideoDownloadAsyncControler.GetDownloadedDiskVideoUri(Videodata.id);

                        SimpleCache cache = new SimpleCache(ActivityContext.CacheDir,new LeastRecentlyUsedCacheEvictor(1024 * 1024 * 10));
                        CacheDataSourceFactory cacheDataSource =new CacheDataSourceFactory(cache, DefaultDatamediaFactory);

                        if (file != null)
                        {
                            videoUrL = Android.Net.Uri.Parse(file);
                            videoSource = new ExtractorMediaSource(videoUrL, cacheDataSource,new DefaultExtractorsFactory(), mainHandler, eventLogger);
                            return videoSource;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return null;
            }
        }

        public IMediaSource CreateAdsMediaSource(IMediaSource mediaSource, Android.Net.Uri adTagUri)
        {
            if (ImaAdsLoader == null)
            {
                ImaAdsLoader = new ImaAdsLoader(ActivityContext, adTagUri);
                adOverlayViewGroup = new FrameLayout(ActivityContext);
                simpleExoPlayerView.OverlayFrameLayout.AddView(adOverlayViewGroup);
                AdsController cont = new AdsController(ActivityContext);
            }
            return new ImaAdsMediaSource(mediaSource, DefaultDatamediaFactory, ImaAdsLoader, adOverlayViewGroup);
        }

        public IMediaSource CreateDefaultMediaSource(Android.Net.Uri videoUrL)
        {
            try
            {
                if (videoUrL != null)
                {
                    ExtractorMediaSource defaultSource = new ExtractorMediaSource(videoUrL, DefaultDatamediaFactory,new DefaultExtractorsFactory(), mainHandler, eventLogger);

                    this.ActivityContext.RunOnUiThread(() =>
                    {
                        if (simpleExoPlayerView != null)
                        {
                            simpleExoPlayerView.Player = Player;
                            Player.Prepare(videoSource);
                            Player.AddListener(PlayerLitsener);
                            Player.PlayWhenReady = true;
                        }
                        else
                        {
                            Initialize();
                            simpleExoPlayerView.Player = Player;
                            Player.Prepare(videoSource);
                            Player.AddListener(PlayerLitsener);
                            Player.PlayWhenReady = true;
                        }

                        bool haveResumePosition = resumeWindow != C.IndexUnset;
                        if (haveResumePosition)
                        {
                            Player.SeekTo(resumeWindow, resumePosition);
                        }
                    });
                    return defaultSource;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return null;
            }
        }

        public void OnClick(View v)
        {
            try
            {
                if (v.Id == mFullScreenIcon.Id || v.Id == mFullScreenButton.Id)
                {
                    InitFullscreenDialog();
                }

            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void ReleaseAdsLoader()
        {
            try
            {
                if (ImaAdsLoader != null)
                {
                    ImaAdsLoader.Release();
                    ImaAdsLoader = null;
                    simpleExoPlayerView.OverlayFrameLayout.RemoveAllViews();
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void OnPlayerStateChanged(bool playWhenReady, int playbackState)
        {
            try
            {
                if (playbackState == Com.Google.Android.Exoplayer2.Player.StateEnded)
                {
                    if (playWhenReady == false)
                    {
                        videoResumeButton.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        videoResumeButton.Visibility = ViewStates.Gone;
                        videoPlayButton.Visibility = ViewStates.Visible;

                    }

                    Loadingprogress_bar.Visibility = ViewStates.Invisible;
                }
                else if (playbackState == Com.Google.Android.Exoplayer2.Player.StateReady)
                {
                    if (playWhenReady == false)
                    {
                        videoResumeButton.Visibility = ViewStates.Gone;
                        videoPlayButton.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        videoResumeButton.Visibility = ViewStates.Visible;
                    }

                    Loadingprogress_bar.Visibility = ViewStates.Invisible;
                }
                else if (playbackState == Com.Google.Android.Exoplayer2.Player.StateBuffering)
                {
                    Loadingprogress_bar.Visibility = ViewStates.Visible;
                    videoResumeButton.Visibility = ViewStates.Invisible;
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void RestartPlayAfterShrinkScreen()
        {
            try
            {
                simpleExoPlayerView.Player = null;
                simpleExoPlayerView.Player = Player;
                simpleExoPlayerView.Player.PlayWhenReady = true;
                mFullScreenIcon.SetImageDrawable(
                    ActivityContext.GetDrawable(Resource.Drawable.ic_action_ic_fullscreen_expand));
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void PlayFullScreen()
        {
            try
            {
                if (FullscreenplayerView != null)
                {
                    Player?.AddListener(PlayerLitsener);
                    FullscreenplayerView.Player = Player;
                    FullscreenplayerView.Player.PlayWhenReady = true;
                    mFullScreenIcon.SetImageDrawable(ActivityContext.GetDrawable(Resource.Drawable.ic_action_ic_fullscreen_skrink));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #endregion

        #region Event 
         
        //Full Screen
        public void InitFullscreenDialog(string action = "Open")
        {
            try
            {
                if (ActivityName != "FullScreen" && action == "Open")
                {
                    Intent intent = new Intent(ActivityContext, typeof(FullScreenVideoActivity));
                    intent.PutExtra("Downloaded", Download_icon.Tag.ToString());
                    ActivityContext.StartActivityForResult(intent, 2000);
                }
                else
                {
                    Intent intent = new Intent();
                    ActivityContext.SetResult(Result.Ok, intent);
                    ActivityContext.Finish();
                }

            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }
         
        //Menu More >>  
        private void MoreButton_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                ContextThemeWrapper ctw = new ContextThemeWrapper(ActivityContext, Resource.Style.PopupMenuStyle);
                PopupMenu popup = new PopupMenu(ctw, More_Button);
                popup.MenuInflater.Inflate(Resource.Menu.MoreCommunities_NotEdit_Menu, popup.Menu);
                popup.Show();
                popup.MenuItemClick += (o, popup_eventArgs) =>
                {
                    try
                    {
                        var Id = popup_eventArgs.Item.ItemId;
                        switch (Id)
                        {
                            case Resource.Id.menu_CopeLink:
                                OnMenu_CopeLink_Click(Videodata);
                                break;

                            case Resource.Id.menu_Share:
                                OnMenu_ShareIcon_Click(Videodata);
                                break;
                        }
                    }
                    catch (Exception exception)
                    {
                        Crashes.TrackError(exception);
                    }
                };
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Menu >> Share
        public async void OnMenu_ShareIcon_Click(Get_Movies_Object.Movie video)
        {
            try
            {
                //Share Plugin same as flame
                if (!CrossShare.IsSupported)
                {
                    return;
                }

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = Videodata.name,
                    Text = Videodata.description,
                    Url = Videodata.url
                });
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Event Menu >> Cope Link
        public void OnMenu_CopeLink_Click(Get_Movies_Object.Movie video)
        {
            try
            {
                var clipboardManager = (ClipboardManager)ActivityContext.GetSystemService(Context.ClipboardService);

                ClipData clipData = ClipData.NewPlainText("text", video.url);
                clipboardManager.PrimaryClip = clipData;

                Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Copied), ToastLength.Short).Show();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Share
        private  void ShareIcon_Click(object sender, EventArgs e)
        {
            try
            {
                OnMenu_ShareIcon_Click(Videodata);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Download
        private void Download_icon_Click(object sender, EventArgs e)
        {
            try
            {
                if (Download_icon.Tag.ToString() == "false")
                {
                    Download_icon.SetImageDrawable(ActivityContext.GetDrawable(Resource.Drawable.ic_action_download_stop));
                    Download_icon.Tag = "true";

                    string urlVideo = string.Empty;
                    if (Videodata.source.Contains("youtube") || Videodata.source.Contains("Youtube") || Videodata.source.Contains("youtu"))
                    {
                        urlVideo = VideoInfoRetriever.VideoDownloadstring;
                        if (!string.IsNullOrEmpty(urlVideo))
                        {
                            VideoControler = new VideoDownloadAsyncControler(urlVideo, Videodata.source, ActivityContext);
                            if (!VideoControler.CheckDownloadLinkIfExits())
                                VideoControler.StartDownloadManager(Videodata.name, Videodata);
                        }
                        else
                        {
                            IMethods.DialogPopup.InvokeAndShowDialog(ActivityContext, ActivityContext.GetString(Resource.String.Lbl_Error), ActivityContext.GetString(Resource.String.Lbl_You_can_not_Download_video), ActivityContext.GetString(Resource.String.Lbl_Ok));
                        }
                    }
                    else
                    {
                        urlVideo = Videodata.source;

                        VideoControler = new VideoDownloadAsyncControler(urlVideo, Videodata.id, ActivityContext);
                        if (!VideoControler.CheckDownloadLinkIfExits())
                            VideoControler.StartDownloadManager(Videodata.name, Videodata);
                    }
                }
                else if (Download_icon.Tag.ToString() == "Downloaded")
                {
                    try
                    {
                        AlertDialog.Builder builder = new AlertDialog.Builder(ActivityContext);
                        builder.SetTitle(ActivityContext.GetString(Resource.String.Lbl_Delete_video));
                        builder.SetMessage(ActivityContext.GetString(Resource.String.Lbl_Do_You_want_to_remove_video));

                        builder.SetPositiveButton(ActivityContext.GetString(Resource.String.Lbl_Yes), delegate (object o, DialogClickEventArgs args)
                        {
                            try
                            {
                                VideoDownloadAsyncControler.RemoveDiskVideoFile(Videodata.id + ".mp4");
                                Download_icon.SetImageDrawable(ActivityContext.GetDrawable(Resource.Drawable.ic_action_download));
                                Download_icon.Tag = "false";
                            }
                            catch (Exception exception)
                            {
                                Crashes.TrackError(exception);
                            }
                        });

                        builder.SetNegativeButton(ActivityContext.GetString(Resource.String.Lbl_No), delegate (object o, DialogClickEventArgs args)
                        {

                        });

                        var alert = builder.Create();
                        alert.Show();
                    }
                    catch (System.Exception exception)
                    {
                        Crashes.TrackError(exception);
                    }
                }
                else
                {
                    Download_icon.SetImageDrawable(ActivityContext.GetDrawable(Resource.Drawable.ic_action_download));
                    Download_icon.Tag = "false";
                    VideoControler.StopDownloadManager();
                }
            }
            catch (System.Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Back
        public void BackIcon_Click(object sender, EventArgs e)
        {
            try
            {
                if (ActivityName == "FullScreen")
                {
                    Intent intent = new Intent();
                    ActivityContext.SetResult(Result.Ok, intent);
                    ActivityContext.Finish();
                }
                else if (ActivityName == "Viewer_Video")
                {
                    ReleaseVideo();
                    ActivityContext.Finish();
                }
                SetStopvideo();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #endregion

        public void OnSelection(MaterialDialog p0, View p1, int p2, ICharSequence p3)
        {

        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {

        }
    }
}