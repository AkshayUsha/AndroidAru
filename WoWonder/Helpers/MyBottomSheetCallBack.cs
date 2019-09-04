using System;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Views;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.DialogUser;

namespace WoWonder.Helpers
{
    public class MyBottomSheetCallBack : BottomSheetBehavior.BottomSheetCallback
    {
        public override void OnSlide(View bottomSheet, float slideOffset)
        {
            try
            {
                //Sliding 
                switch (slideOffset)
                {
                    case BottomSheetBehavior.StateCollapsed:
                        // Log.d(TAG, "State Collapsed");
                        break;
                    case BottomSheetBehavior.StateDragging:
                        // Log.d(TAG, "State Dragging");
                        break;
                    case BottomSheetBehavior.StateExpanded:
                        // Log.d(TAG, "State Expanded");
                        break;
                    case BottomSheetBehavior.StateHidden:
                        // Log.d(TAG, "State Hidden");
                        break;
                    case BottomSheetBehavior.StateSettling:
                        // Log.d(TAG, "State Settling");
                        break;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override void OnStateChanged(View bottomSheet, int newState)
        {
            try
            {
                //State changed
                if (newState == BottomSheetBehavior.StateHidden)
                {
                    Dispose();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
         
        private static int Count_Native = 0;

        public static void Ad_BottomSheet(FragmentManager fragmentManager)
        {
            try
            {
                if (Settings.Show_ADMOB_Native)
                {
                    if (Count_Native == Settings.Show_ADMOB_Native_Count)
                    {
                        Count_Native = 0;

                        //Show the Bottom Sheet Fragment
                        AdmobNative_Fragment bottomSheetDialogFragment = new AdmobNative_Fragment();
                        bottomSheetDialogFragment.Show(fragmentManager, "Bottom Sheet Dialog Fragment");
                    }
                    Count_Native++;
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

    }
}