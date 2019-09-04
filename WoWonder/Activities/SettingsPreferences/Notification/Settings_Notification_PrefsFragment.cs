using System;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Microsoft.AppCenter.Crashes;
using WoWonder.Library.OneSignal;

namespace WoWonder.Activities.SettingsPreferences.Notification
{
    public class Settings_Notification_PrefsFragment : PreferenceFragment,
        ISharedPreferencesOnSharedPreferenceChangeListener
    {
        private SwitchPreference Notifcation_Popup_Pref;

        //On Change
        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            try
            {
                if (key.Equals("notifications_key"))
                {
                    var Getvalue = WowTime_Main_Settings.Shared_Data.GetBoolean("notifications_key", true);
                    Notifcation_Popup_Pref.Checked = Getvalue;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

      
        public override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Create your fragment here
                AddPreferencesFromResource(Resource.Xml.SettingsPrefs_Notification);

                WowTime_Main_Settings.Shared_Data = PreferenceManager.SharedPreferences;

                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                Notifcation_Popup_Pref = (SwitchPreference) FindPreference("notifications_key");


                //Update Preferences data on Load
                OnSharedPreferenceChanged(WowTime_Main_Settings.Shared_Data, "notifications_key");
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();

                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                //Add OnChange event to Preferences
                Notifcation_Popup_Pref.PreferenceChange += NotificationPopupPref_OnPreferenceChange;
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
                PreferenceScreen.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);

                //Close OnChange event to Preferences
                Notifcation_Popup_Pref.PreferenceChange -= NotificationPopupPref_OnPreferenceChange;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Notification >> Popup 
        private void NotificationPopupPref_OnPreferenceChange(object sender,
            Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Handled)
                {
                    var etp = (SwitchPreference) sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (etp.Checked)
                        OneSignalNotification.RegisterNotificationDevice();
                    else
                        OneSignalNotification.Un_RegisterNotificationDevice();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
    }
}