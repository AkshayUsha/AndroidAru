using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder_API;

namespace WoWonder.Activities.SettingsPreferences.Support
{
    public class Settings_Support_PrefsFragment : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        public Settings_Support_PrefsFragment(Activity context)
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

                // Create your fragment here
                AddPreferencesFromResource(Resource.Xml.SettingsPrefs_Help_Support);

                WowTime_Main_Settings.Shared_Data = PreferenceManager.SharedPreferences;

                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                Help_Pref = FindPreference("help_key");
                ReportProblem_Pref = FindPreference("Report_key");
                AboutApp_Pref = FindPreference("About_key");
                PrivacyPolicy_Pref = FindPreference("PrivacyPolicy_key");
                TermsOfUse_Pref = FindPreference("TermsOfUse_key");

                //Delete Preference
                var mCategory_Support = (PreferenceCategory) FindPreference("SectionSupport_key");
                if (!Settings.Show_Settings_Help)
                    mCategory_Support.RemovePreference(Help_Pref);

                if (!Settings.Show_Settings_ReportProblem)
                    mCategory_Support.RemovePreference(ReportProblem_Pref);

                var mCategory_About = (PreferenceCategory) FindPreference("SectionAbout_key");
                if (!Settings.Show_Settings_About)
                    mCategory_About.RemovePreference(AboutApp_Pref);

                if (!Settings.Show_Settings_PrivacyPolicy)
                    mCategory_About.RemovePreference(PrivacyPolicy_Pref);

                if (!Settings.Show_Settings_TermsOfUse)
                    mCategory_About.RemovePreference(TermsOfUse_Pref);
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
                Help_Pref.PreferenceClick += HelpPref_OnPreferenceClick;
                ReportProblem_Pref.PreferenceClick += ReportProblemPref_OnPreferenceClick;
                AboutApp_Pref.PreferenceClick += AboutAppPref_OnPreferenceClick;
                PrivacyPolicy_Pref.PreferenceClick += PrivacyPolicyPref_OnPreferenceClick;
                TermsOfUse_Pref.PreferenceClick += TermsOfUsePref_OnPreferenceClick;
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
                Help_Pref.PreferenceClick -= HelpPref_OnPreferenceClick;
                ReportProblem_Pref.PreferenceClick -= ReportProblemPref_OnPreferenceClick;
                AboutApp_Pref.PreferenceClick -= AboutAppPref_OnPreferenceClick;
                PrivacyPolicy_Pref.PreferenceClick -= PrivacyPolicyPref_OnPreferenceClick;
                TermsOfUse_Pref.PreferenceClick -= TermsOfUsePref_OnPreferenceClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //Terms Of Use
        private void TermsOfUsePref_OnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(SupportWebView_Activity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/terms/terms");
                intent.PutExtra("Type",this.ActivityContext.GetString(Resource.String.Terms_of_service));
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Privacy Policy
        private void PrivacyPolicyPref_OnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(SupportWebView_Activity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/terms/privacy-policy");
                intent.PutExtra("Type", this.ActivityContext.GetString(Resource.String.Privacy_Policy));
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //About Us
        private void AboutAppPref_OnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(SupportWebView_Activity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/terms/about-us");
                intent.PutExtra("Type", this.ActivityContext.GetString(Resource.String.Lbl_About_App));
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Report a Problem
        private void ReportProblemPref_OnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(SupportWebView_Activity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/contact-us");
                intent.PutExtra("Type", this.ActivityContext.GetString(Resource.String.Lbl_Report_Problem));
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Help
        private void HelpPref_OnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(SupportWebView_Activity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/contact-us");
                intent.PutExtra("Type", this.ActivityContext.GetString(Resource.String.Lbl_Help));
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #region Variables Basic

        private Preference Help_Pref;
        private Preference ReportProblem_Pref;
        private Preference AboutApp_Pref;
        private Preference PrivacyPolicy_Pref;
        private Preference TermsOfUse_Pref;

        private readonly Activity ActivityContext;

        #endregion
    }
}