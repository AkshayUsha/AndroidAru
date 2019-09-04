using System;
using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.Formats;
using Android.Gms.Ads.Reward;
using Android.Widget;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
 
namespace WoWonder.Helpers
{
    public class AdsGoogle
    {
        //Interstitial >> 
        //==================================================

        #region Interstitial

        public class AdmobInterstitial
        {
            public InterstitialAd _ad;
            public void ShowAd(Context context)
            {
                try
                {
                    _ad = new InterstitialAd(context);
                    _ad.AdUnitId = Settings.Ad_Interstitial_Key;

                    var intlistener = new InterstitialAdListener(_ad);
                    intlistener.OnAdLoaded();
                    _ad.AdListener = intlistener;

                    var requestbuilder = new AdRequest.Builder();
                    _ad.LoadAd(requestbuilder.Build());
                }
                catch (Exception exception)
                {
                    Crashes.TrackError(exception);
                }
            }
        }

        public class InterstitialAdListener : AdListener
        {
            public InterstitialAd _ad;

            public InterstitialAdListener(InterstitialAd ad)
            {
                _ad = ad;
            }

            public override void OnAdLoaded()
            {
                base.OnAdLoaded();

                if (_ad.IsLoaded)
                    _ad.Show();
            }
        }

        private static int Count_Interstitial = 0;
        public static void Ad_Interstitial(Context context)
        {
            try
            {
                if (Settings.Show_ADMOB_Interstitial)
                {
                    if (Count_Interstitial == Settings.Show_ADMOB_Interstitial_Count)
                    {
                        Count_Interstitial = 0;
                        AdmobInterstitial ads = new AdmobInterstitial();
                        ads.ShowAd(context);
                    }
                    Count_Interstitial++;
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }


        #endregion

        //Rewarded Video >>
        //===================================================

        #region Rewarded

        public class AdmobRewardedVideo : AdListener, IRewardedVideoAdListener
        {
            public IRewardedVideoAd Rad;
            public void ShowAd(Context context)
            {
                try
                {
                    MobileAds.Initialize(context, Settings.Ad_RewardVideo_Key);

                    // Use an activity context to get the rewarded video instance.
                    Rad = MobileAds.GetRewardedVideoAdInstance(context);
                    Rad.RewardedVideoAdListener = this;

                    OnRewardedVideoAdLoaded();
                }
                catch (Exception exception)
                {
                    Crashes.TrackError(exception);
                }
            }

            public override void OnAdLoaded()
            {
                try
                {
                    base.OnAdLoaded();

                    OnRewardedVideoAdLoaded();

                    if (Rad.IsLoaded)
                        Rad.Show();
                }
                catch (Exception exception)
                {
                    Crashes.TrackError(exception);
                }
            }

            public void OnRewarded(IRewardItem reward)
            {
                //Toast.MakeText(Application.Context, "onRewarded! currency: " + reward.Type + "  amount: " + reward.Amount , ToastLength.Short).Show();

                if (Rad.IsLoaded)
                    Rad.Show();
            }

            public void OnRewardedVideoAdClosed()
            {
                OnRewardedVideoAdLoaded();
            }

            public void OnRewardedVideoAdFailedToLoad(int errorCode)
            {
                 Toast.MakeText(Application.Context, "No ads currently available", ToastLength.Short).Show();
            }

            public void OnRewardedVideoAdLeftApplication()
            {

            }

            public void OnRewardedVideoAdLoaded()
            {
                Rad.LoadAd(Settings.Ad_RewardVideo_Key, new AdRequest.Builder().Build());
                Rad.Show();
            }

            public void OnRewardedVideoAdOpened()
            {

            }

            public void OnRewardedVideoStarted()
            {

            }

            void IRewardedVideoAdListener.OnRewardedVideoAdClosed()
            {
                OnRewardedVideoAdClosed();
            }
        }


        private static int Count_RewardedVideo = 0;
        public static void Ad_RewardedVideo(Context context)
        {
            try
            {
                if (Settings.Show_ADMOB_RewardVideo)
                {
                    if (Count_RewardedVideo == Settings.Show_ADMOB_RewardedVideo_Count)
                    {
                        Count_RewardedVideo = 0;
                        AdmobRewardedVideo ads = new AdmobRewardedVideo();
                        ads.ShowAd(context);
                    }
                    Count_RewardedVideo++;
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #endregion
         
        //Native Ads Advanced
        //==================================================

        #region Native

        public class AdmobNative : AdListener, NativeAppInstallAd.IOnAppInstallAdLoadedListener, NativeContentAd.IOnContentAdLoadedListener
        {
            private AdLoader adLoader;
            public void ShowAd(Context context)
            {
                try
                {
                    //ca-app-pub-3940256099942544/2247696110
                    // Methods in the NativeAdOptions.Builder class can be
                    // used here to specify individual options settings.

                    VideoOptions videoOptions = new VideoOptions.Builder()
                        .SetStartMuted(false)
                        .Build();

                    NativeAdOptions adOptions = new NativeAdOptions.Builder()
                        .SetVideoOptions(videoOptions)
                        .Build();

                    adLoader = new AdLoader.Builder(context, Settings.Ad_Native_Key)
                        .ForAppInstallAd(this).ForContentAd(this).WithAdListener(this).WithNativeAdOptions(adOptions)
                        .Build();

                    var adRequest = new AdRequest.Builder().Build();
                    adLoader.LoadAd(adRequest);

                }
                catch (Exception exception)
                {
                    Crashes.TrackError(exception);
                }
            }

            public void OnAppInstallAdLoaded(NativeAppInstallAd ad)
            {
                try
                {
                    // Show the app install ad.

                    if (adLoader.IsLoading)
                    {
                        // The AdLoader is still loading ads.
                        // Expect more adLoaded or onAdFailedToLoad callbacks.
                    }
                    else
                    {
                        // The AdLoader has finished loading ads.
                    }
                }
                catch (Exception exception)
                {
                    Crashes.TrackError(exception);
                } 
            }

            public void OnContentAdLoaded(NativeContentAd ad)
            {
                try
                {
                    // Show the content ad.
                   
                }
                catch (Exception exception)
                {
                    Crashes.TrackError(exception);
                }
            }

            public override void OnAdFailedToLoad(int errorCode)
            {
                try
                {
                    // Handle the failure by logging, altering the UI, and so on.
                }
                catch (Exception exception)
                {
                    Crashes.TrackError(exception);
                }  
            } 
        }

        private static int Count_Native = 0;
        public static void Ad_Native(Context context)
        {
            try
            {
                if (Settings.Show_ADMOB_Native)
                {
                    if (Count_Native == Settings.Show_ADMOB_Native_Count)
                    {
                        Count_Native = 0;
                        AdmobNative ads = new AdmobNative();
                        ads.ShowAd(context);
                    }
                    Count_Native++;
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #endregion
    }
}