using System;
using Android.App;
using Android.Gms.Ads;
using Android.Support.Design.Widget;
using Android.Views;
using WoWonder.Helpers;

namespace WoWonder.Activities.DialogUser
{
    public class AdmobNative_Fragment : BottomSheetDialogFragment 
    {
        private BottomSheetBehavior.BottomSheetCallback mBottomSheetBehaviorCallback = new MyBottomSheetCallBack();
        private NativeExpressAdView native;
        public override void SetupDialog(Dialog dialog, int style)
        {
            try
            {
                base.SetupDialog(dialog, style);
                //Get the content View
                View contentView = View.Inflate(this.Context, Resource.Layout.AdmobNative_Layout, null);
                dialog.SetContentView(contentView);

                //Set the coordinator layout behavior
                CoordinatorLayout.LayoutParams Params =(CoordinatorLayout.LayoutParams) ((View) contentView.Parent).LayoutParameters;
                CoordinatorLayout.Behavior behavior = Params.Behavior;

                //Set callback
                if (behavior != null && behavior.GetType() == typeof(BottomSheetBehavior))
                {
                    ((BottomSheetBehavior) behavior).SetBottomSheetCallback(mBottomSheetBehaviorCallback);
                }

                string android_id = Android.Provider.Settings.Secure.GetString(this.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

                native = contentView.FindViewById<NativeExpressAdView>(Resource.Id.adViewNative);
                // Create an ad request.
                AdRequest.Builder adRequestBuilder = new AdRequest.Builder();

                // Optionally populate the ad request builder.
                adRequestBuilder.AddTestDevice(android_id);

                // Start loading the ad.
                native.LoadAd(adRequestBuilder.Build());

                native.Visibility = ViewStates.Visible;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();

                // Resume the NativeExpressAdView.
                native.Resume();
                native.Visibility = ViewStates.Visible;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public override void OnPause()
        {
            try
            {
                // Pause the NativeExpressAdView.
                native.Pause();
                native.Visibility = ViewStates.Gone;
                base.OnPause();
              
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public override void OnDestroy()
        {
            try
            {
                // Destroy the NativeExpressAdView.
                native.Destroy();
                native.Visibility = ViewStates.Gone;
                base.OnDestroy();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}