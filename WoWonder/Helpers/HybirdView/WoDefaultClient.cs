using System;
using Android.App;
using Android.Graphics;
using Android.Net.Http;
using Android.Webkit;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;

namespace WoWonder.Helpers.HybirdView
{
    public class WoDefaultClient : WebViewClient
    {
        public static int ErrorBadUrl { get; } = -12;
        public static int ErrorConnect { get; } = -6;
        public static int ErrorFailedSslHandshake { get; } = -11;
        public static int ErrorFile { get; } = -13;
        public static int ErrorFileNotFound { get; } = -14;
        public static int ErrorHostLookup { get; } = -2;
        public static int ErrorIo { get; } = -7;
        public static int ErrorProxyAuthentication { get; } = -9;
        public static int ErrorTimeout { get; } = -8;
        public static int ErrorTooManyRequests { get; } = -15;
        public static int ErrorUnknown { get; } = -1;
        public static int ErrorUnsafeResource { get; } = -16;
        public static int ErrorUnsupportedAuthScheme { get; } = -3;
        public static int SafeBrowsingThreatMalware { get; } = 1;
        public static int SafeBrowsingThreatPhishing { get; } = 2;
        public static int SafeBrowsingThreatUnknown { get; } = 0;
        public static int SafeBrowsingThreatUnwantedSoftware { get; } = 3;
      

        public delegate void WoPageStarted(WebView view, string url, Bitmap favicon);
        public delegate void WoPageFinished(WebView view, string url);
        public delegate void WoPageReceivedError(WebView view, IWebResourceRequest request, WebResourceError error, string textError);
        public delegate void WoPageReceivedHttpError(WebView view, IWebResourceRequest request, WebResourceResponse errorResponse);
        public delegate void WoPageReceivedSslError(WebView view, SslErrorHandler handler, SslError error);
        public delegate void WoPageLoadResource(WebView view, string url);
        public delegate void WoShouldOverrideUrlLoading(WebView view, IWebResourceRequest request);
        public delegate void WoPageShouldInterceptRequest(WebView view, IWebResourceRequest request);

        //Public Events
        internal event WoPageStarted OnPageEventStarted;
        internal event WoPageFinished OnPageEventFinished;
        internal event WoPageReceivedError OnPageEventReceivedError;
        internal event WoPageShouldInterceptRequest OnPageEventShouldInterceptRequest;
        internal event WoPageReceivedHttpError OnPageEventReceivedHttpError;
        internal event WoPageReceivedSslError OnPageEventReceivedSslError;
        internal event WoPageLoadResource OnPageEventLoadResource;
        internal event WoShouldOverrideUrlLoading OnPageEventShouldOverrideUrlLoading;

        public HybirdViewController HybridController;

        public WoDefaultClient(HybirdViewController controller)
        {
            try
            {
                HybridController = controller;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            try
            {
                base.OnPageStarted(view, url, favicon);
                 
                OnPageEventStarted?.Invoke(view, url, favicon);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override void OnPageFinished(WebView view, string url)
        {
            try
            {
                base.OnPageFinished(view, url);
                view.EvaluateJavascript("function csharp(data){bridge.invokeAction(data);}", HybridController.JavascriptCallback);
                view.EvaluateJavascript("eval", HybridController.JavascriptCallback);
  
                OnPageEventFinished?.Invoke(view, url);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override void OnReceivedError(WebView view, IWebResourceRequest request, WebResourceError error)
        {
            try
            { 
                base.OnReceivedError(view, request, error);
                 
                string TextError = Application.Context.GetString(Resource.String.Lbl_Error_Code) + " ";
                
                switch (error.ErrorCode)
                {
                    case ClientError.BadUrl:
                        TextError = ErrorBadUrl.ToString();
                        break;
                    case ClientError.Connect:
                        TextError +=  ErrorConnect.ToString();
                        break;
                    case ClientError.FailedSslHandshake:
                        TextError +=  ErrorFailedSslHandshake.ToString();
                        break;
                    case ClientError.File:
                        TextError +=  ErrorFile.ToString();
                        break;
                    case ClientError.FileNotFound:
                        TextError +=  ErrorFileNotFound.ToString();
                        break;
                    case ClientError.HostLookup:
                        TextError +=  ErrorHostLookup.ToString();
                        break;
                    case ClientError.ProxyAuthentication:
                        TextError +=  ErrorProxyAuthentication.ToString();
                        break;
                    case ClientError.Timeout:
                        TextError +=  ErrorTimeout.ToString();
                        break;
                    case ClientError.TooManyRequests:
                        TextError +=  ErrorTooManyRequests.ToString();
                        break;
                    case ClientError.Unknown:
                        TextError +=  ErrorUnknown.ToString();
                        break;
                    case ClientError.UnsafeResource:
                        TextError +=  ErrorUnsafeResource.ToString();
                        break;
                    case ClientError.UnsupportedScheme:
                        TextError +=  ErrorUnsupportedAuthScheme.ToString();
                        break;
                    case ClientError.Io:
                        TextError +=  ErrorIo.ToString();
                        break;
                }

                OnPageEventReceivedError?.Invoke(view, request, error, TextError);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override void OnReceivedHttpError(WebView view, IWebResourceRequest request, WebResourceResponse errorResponse)
        {
            try
            {
                base.OnReceivedHttpError(view, request, errorResponse);
                OnPageEventReceivedHttpError?.Invoke(view, request, errorResponse);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override void OnReceivedSslError(WebView view, SslErrorHandler handler, SslError error)
        {
            try
            {
                base.OnReceivedSslError(view, handler, error);
                OnPageEventReceivedSslError?.Invoke(view, handler, error);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            try
            {
                OnPageEventShouldInterceptRequest?.Invoke(view, request);
                if (Settings.UseCachResourceLoad)
                {
                    WebResourceResponse respond = LoadFromAssetCach(request.Url);
                    if (respond != null)
                        return respond;
                }

                return base.ShouldInterceptRequest(view, request);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return base.ShouldInterceptRequest(view, request);
            }
        }

        public override void OnLoadResource(WebView view, string url)
        {
            try
            {
                base.OnLoadResource(view, url);
                OnPageEventLoadResource?.Invoke(view, url);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            try
            {
                OnPageEventShouldOverrideUrlLoading?.Invoke(view, request);
               
                if (!request.Url.Path.Contains("get_news_feed") || !request.Url.Path.Contains("terms"))
                {
                    return false;
                } 
                else
                {
                    base.ShouldOverrideUrlLoading(view, request);
                    return true;
                } 
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return base.ShouldOverrideUrlLoading(view, request);
            }
        }

        public WebResourceResponse LoadFromAssetCach(Android.Net.Uri url)
        {
            try
            {
                if (url.ToString().Contains("recorder.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/recorder.js"));
                }
                else if (url.ToString().Contains("script.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/script.js"));
                }
                else if (url.ToString().Contains("speed.min.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/speed.min.js"));
                }
                else if (url.ToString().Contains("speed-i18n.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/speed-i18n.js"));
                }
                else if (url.ToString().Contains("stats.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/stats.js"));
                }
                else if (url.ToString().Contains("twilio-video.min.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/twilio-video.min.js"));
                }
                else if (url.ToString().Contains("util.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/util.js"));
                }
                else if (url.ToString().Contains("ads.min.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/ads.min.js"));
                }
                else if (url.ToString().Contains("ads-i18n.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/ads-i18n.js"));
                }
                else if (url.ToString().Contains("ads-vast-vpaid.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/ads-vast-vpaid.js"));
                }
                else if (url.ToString().Contains("ads-vast-vpaid.min.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/ads-vast-vpaid.min.js"));
                }
                else if (url.ToString().Contains("autocomplete.jquery.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/autocomplete.jquery.js"));
                }
                else if (url.ToString().Contains("automention.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/automention.js"));
                }
                else if (url.ToString().Contains("bootstrap.min.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/bootstrap.min.js"));
                }
                else if (url.ToString().Contains("common.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/common.js"));
                }
                else if (url.ToString().Contains("controls.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/controls.js"));
                }
                else if (url.ToString().Contains("cookieconsent.min.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/cookieconsent.min.js"));
                }
                else if (url.ToString().Contains("guessLanguage.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/guessLanguage.js"));
                }
                else if (url.ToString().Contains("init.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/init.js"));
                }
                else if (url.ToString().Contains("jquery.form.min.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/jquery.form.min.js"));
                }
                else if (url.ToString().Contains("jquery.limit-1.2.source.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/jquery.limit-1.2.source.js"));
                }
                else if (url.ToString().Contains("jquery.magnific-popup.min.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/jquery.magnific-popup.min.js"));
                }
                else if (url.ToString().Contains("jquery.ui.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/jquery.ui.js"));
                }
                else if (url.ToString().Contains("jquery-3.1.1.min.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/jquery-3.1.1.min.js"));
                }
                else if (url.ToString().Contains("jump-forward.min.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/jump-forward.min.js"));
                }
                else if (url.ToString().Contains("mediaelement-and-player.min.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/mediaelement-and-player.min.js"));
                }
                else if (url.ToString().Contains("places_impl.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/places_impl.js"));
                }
                else if (url.ToString().Contains("rcrop.min.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/rcrop.min.js"));
                }
                else if (url.ToString().Contains("readmore.min.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/readmore.min.js"));
                }
                else if (url.ToString().Contains("record.js"))
                {
                    return new WebResourceResponse("text/javascript", "utf-8",
                        Application.Context.Assets.Open("Cache/record.js"));
                }
                else if (url.ToString().Contains("ads.min.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/ads.min.css"));
                }
                else if (url.ToString().Contains("bootsrap-social.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/bootsrap-social.css"));
                }
                else if (url.ToString().Contains("bootstrap.min.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/bootstrap.min.css"));
                }
                else if (url.ToString().Contains("bootstrap-tagsinput.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/bootstrap-tagsinput.css"));
                }
                else if (url.ToString().Contains("cookieconsent.min.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/cookieconsent.min.css"));
                }
                else if (url.ToString().Contains("font-awesome.min.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/font-awesome.min.css"));
                }
                else if (url.ToString().Contains("jquery.ui.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/jquery.ui.css"));
                }
                else if (url.ToString().Contains("jump-forward.min.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/jump-forward.min.css"));
                }
                else if (url.ToString().Contains("magnific-popup.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/magnific-popup.css"));
                }
                else if (url.ToString().Contains("mediaelementplayer.min.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/mediaelementplayer.min.css"));
                }
                else if (url.ToString().Contains("speed.min.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/speed.min.css"));
                }
                else if (url.ToString().Contains("style.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/style.css"));
                }
                else if (url.ToString().Contains("style_ltr.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/style_ltr.css"));
                }
                else if (url.ToString().Contains("twemoji-awesome.min.css"))
                {
                    return new WebResourceResponse("text/css", "utf-8",
                        Application.Context.Assets.Open("Cache/twemoji-awesome.min.css"));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        }
    }
}