using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Microsoft.AppCenter.Crashes;
using Plugin.Share;
using Plugin.Share.Abstractions;
using SettingsConnecter;
using WoWonder.Activities.InviteFriends;

namespace WoWonder.Activities.SettingsPreferences.TellFriend
{
    public class TellFriend_PrefsFragment : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        public TellFriend_PrefsFragment(Activity context)
        {
            try
            {
                ActivityContext = context;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //On Change 
        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            try
            {
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

                AddPreferencesFromResource(Resource.Xml.SettingsPrefs_TellFriend);

                WowTime_Main_Settings.Shared_Data = PreferenceManager.SharedPreferences;

                InviteFriends_Pref = FindPreference("InviteFriends_key");
                Share_Pref = FindPreference("Share_key");
                MyAffiliates_Pref = FindPreference("MyAffiliates_key");


                //Delete Preference
                var mCategory_Invite = (PreferenceCategory) FindPreference("SectionInvite_key");
                if (!Settings.Show_Settings_InviteFriends)
                    mCategory_Invite.RemovePreference(InviteFriends_Pref);

                if (!Settings.Show_Settings_Share)
                    mCategory_Invite.RemovePreference(Share_Pref);

                if (!Settings.Show_Settings_MyAffiliates)
                    mCategory_Invite.RemovePreference(MyAffiliates_Pref);

                InviteSMSText = GetText(Resource.String.Lbl_InviteSMSText_1) + " " + Settings.Application_Name + " " +
                                GetText(Resource.String.Lbl_InviteSMSText_2);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();
                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                //Add event to Preferences
                InviteFriends_Pref.PreferenceClick += InviteFriendsPref_OnPreferenceClick;
                Share_Pref.PreferenceClick += SharePref_OnPreferenceClick;
                MyAffiliates_Pref.PreferenceClick += MyAffiliatesPref_OnPreferenceClick;
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

                //Add event to Preferences
                InviteFriends_Pref.PreferenceClick -= InviteFriendsPref_OnPreferenceClick;
                Share_Pref.PreferenceClick -= SharePref_OnPreferenceClick;
                MyAffiliates_Pref.PreferenceClick -= MyAffiliatesPref_OnPreferenceClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //Share App with your friends using Url This App in Market Google play 
        private async void SharePref_OnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                //Share Plugin same as flame
                if (!CrossShare.IsSupported) return;

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = Settings.Application_Name,
                    Text = InviteSMSText,
                    Url = "http://play.google.com/store/apps/details?id=" + ActivityContext.PackageName
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Invite Friends
        private void InviteFriendsPref_OnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var Intent = new Intent(Application.Context, typeof(Invite_Friends_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //My Affiliates
        private void MyAffiliatesPref_OnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var Intent = new Intent(Application.Context, typeof(MyAffiliates_Activity));
                StartActivity(Intent);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Variables Basic

        private Preference InviteFriends_Pref;
        private Preference Share_Pref;
        private Preference MyAffiliates_Pref;

        public string InviteSMSText = "";

        private readonly Activity ActivityContext;

        #endregion
    }
}