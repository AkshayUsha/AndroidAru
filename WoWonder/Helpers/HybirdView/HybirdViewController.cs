//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) WoWOnder 15/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using Color = Android.Graphics.Color;

namespace WoWonder.Helpers.HybirdView
{
    public class HybirdViewController
    {
        public WebView HybirdView { get; set; }
        public Activity ApplicationContext { get; set; }

        public readonly JavascriptValueCallbacker JavascriptCallback;
        public JsBrigeInvoker JavascriptInterface;
        public WoDefaultClient DefaultClient;
        public WoChromeClient ChromeClient;

        public HybirdViewController(Activity context, WebView hview, string idpage ="")
        {
            try
            {
               
                ApplicationContext = context;
                HybirdView = hview;

                JavascriptCallback = new JavascriptValueCallbacker(HybirdView);
                JavascriptInterface = new JsBrigeInvoker(ApplicationContext, idpage);
                DefaultClient = new WoDefaultClient(this);
                ChromeClient = new WoChromeClient();

                //HybirdView.Settings.UseWideViewPort = true;
                HybirdView.Settings.JavaScriptEnabled = true;
                HybirdView.Settings.DomStorageEnabled = true;
                HybirdView.Settings.AllowFileAccess = true;
                HybirdView.Settings.JavaScriptCanOpenWindowsAutomatically = true;

                HybirdView.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.NarrowColumns);
                HybirdView.AddJavascriptInterface(JavascriptInterface, "bridge");
                HybirdView.SetWebViewClient(DefaultClient);
                HybirdView.SetWebChromeClient(ChromeClient);

                HybirdView.Settings.CacheMode = CacheModes.Default;
                HybirdView.Settings.SetEnableSmoothTransition(true);
               
                HybirdView.SetDownloadListener(new DownloadListener(ApplicationContext));
                HybirdView.SetBackgroundColor(Color.Argb(1, 0, 0, 0));

                if (Settings.EnableCachSystem)
                {
                    HybirdView.Settings.SetAppCacheMaxSize(8 * 1024 * 1024);
                    HybirdView.Settings.SetAppCachePath(ApplicationContext.CacheDir.AbsolutePath);
                    HybirdView.Settings.SetAppCacheEnabled(true);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void LoadUrl(string url)
        {
            try
            {
                HybirdView.LoadUrl(url);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void EvaluateJavascript(string name)
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                {
                    HybirdView.EvaluateJavascript(name, JavascriptCallback);
                }
                else
                {
                    HybirdView.LoadUrl(name);
                } 
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        public void UseRenderPriorityFastPostLoad()
        {
            try
            {
                HybirdView.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
                HybirdView.Settings.SetAppCacheEnabled(true);

                HybirdView.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.NarrowColumns);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                    HybirdView.SetLayerType(LayerType.Hardware, null);
                else
                    HybirdView.SetLayerType(LayerType.Software, null);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

       

        public class DownloadListener : Java.Lang.Object, IDownloadListener
        {
            public Activity Context;
            public DownloadListener(Activity applicationContext)
            {
                Context = applicationContext;
                
            }

            public void OnDownloadStart(string url, string userAgent, string contentDisposition, string mimetype, long contentLength)
            {

                Intent i = new Intent(Intent.ActionView);
                i.SetData(Android.Net.Uri.Parse(url));
                Context.StartActivity(i);

            }
        }
    }
}