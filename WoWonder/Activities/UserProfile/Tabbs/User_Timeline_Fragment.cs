using System;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers.HybirdView;
using WoWonder_API;

namespace WoWonder.Activities.userProfile.Tabbs
{
    public class User_Timeline_Fragment : Fragment
    {
        public WebView HybirdView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.User_Timeline_Fragment, container, false);

            HybirdView = (WebView) view.FindViewById(Resource.Id.hybirdview);

            var hybridController = new HybirdViewController(Activity, HybirdView, null);
            hybridController.LoadUrl(Client.WebsiteUrl + "/app_api.php?application=phone&type=set_c&c=9860088665e4c26a03d7efc3bdca2e61ac134710a8b033e0c059833bc1355aea5ca2be7d47128206fce34b6aef091b6fb2032870279690f8");


            //hybridController.JavascriptInterface.OnJavascriptInjectionRequest += OnJavascriptInjectionRequest;
            hybridController.DefaultClient.OnPageEventFinished += WoDefaultClient_OnPageEventFinished;

            return view;
        }

        private void WoDefaultClient_OnPageEventFinished(WebView view, string url)
        {
            try
            {
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void OnJavascriptInjectionRequest(string data)
        {
            //Click events comes here 
        }
    }
}