using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.BlockedUsers;
using WoWonder.Activities.MyProfile;
using WoWonder.Activities.Tabbes;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Exception = System.Exception;

namespace WoWonder.Activities.SettingsPreferences.General
{
    public class GeneralAccount_PrefsFragment : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener,
        MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        public GeneralAccount_PrefsFragment(Activity context)
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

        public void OnSelection(MaterialDialog p0, View p1, int p2, ICharSequence p3)
        {
        }

        

        //On Change 
        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            try
            {
                var dataUser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);

                if (key.Equals("about_me_key"))
                {
                    // Set summary to be the user-description for the selected value
                    var etp = (EditTextPreference) FindPreference("about_me_key");
                    if (dataUser != null)
                    {
                        WowTime_Main_Settings.Shared_Data.Edit().PutString("about_me_key", dataUser.about).Commit();

                        S_About = dataUser.about;

                        if (S_About != "Empty")
                        {
                            etp.EditText.Text = S_About;
                            etp.Text = S_About;
                        }
                        else
                        {
                            etp.EditText.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " +
                                                Settings.Application_Name;
                            etp.Text = GetText(Resource.String.Lbl_DefaultAbout) + " " + Settings.Application_Name;
                        }
                    }

                    var Getvalue = WowTime_Main_Settings.Shared_Data.GetString("about_me_key", S_About);
                    etp.EditText.Text = Getvalue;
                    etp.Summary = Getvalue;
                }
                else if (key.Equals("Lang_key"))
                {
                    var valueAsText = Lang_Pref.Entry;
                    if (!string.IsNullOrEmpty(valueAsText))
                    {
                        Settings.FlowDirection_RightToLeft = false;
                        if (valueAsText.ToLower().Contains("english"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "en").Commit();
                            Lang_Pref.SetValueIndex(1);
                        }
                        else if (valueAsText.ToLower().Contains("arabic"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "ar").Commit();
                            Lang_Pref.SetValueIndex(2);
                            Settings.FlowDirection_RightToLeft = true;
                        }
                        else if (valueAsText.ToLower().Contains("german"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "de").Commit();
                            Lang_Pref.SetValueIndex(3);
                        }
                        else if (valueAsText.ToLower().Contains("greek"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "el").Commit();
                            Lang_Pref.SetValueIndex(4);
                        }
                        else if (valueAsText.ToLower().Contains("spanish"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "es").Commit();
                            Lang_Pref.SetValueIndex(5);
                        }
                        else if (valueAsText.ToLower().Contains("french"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "fr").Commit();
                            Lang_Pref.SetValueIndex(6);
                        }
                        else if (valueAsText.ToLower().Contains("italian"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "it").Commit();
                            Lang_Pref.SetValueIndex(7);
                        }
                        else if (valueAsText.ToLower().Contains("japanese"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "ja").Commit();
                            Lang_Pref.SetValueIndex(8);
                        }
                        else if (valueAsText.ToLower().Contains("dutch"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "nl").Commit();
                            Lang_Pref.SetValueIndex(9);
                        }
                        else if (valueAsText.ToLower().Contains("portuguese"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "pt").Commit();
                            Lang_Pref.SetValueIndex(10);
                        }
                        else if (valueAsText.ToLower().Contains("romanian"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "ro").Commit();
                            Lang_Pref.SetValueIndex(11);
                        }
                        else if (valueAsText.ToLower().Contains("russian"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "ru").Commit();
                            Lang_Pref.SetValueIndex(12);
                        }
                        else if (valueAsText.ToLower().Contains("russian"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "ru").Commit();
                            Lang_Pref.SetValueIndex(13);
                        }
                        else if (valueAsText.ToLower().Contains("albanian"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "sq").Commit();
                            Lang_Pref.SetValueIndex(14);
                        }
                        else if (valueAsText.ToLower().Contains("serbian"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "sr").Commit();
                            Lang_Pref.SetValueIndex(15);
                        }
                        else if (valueAsText.ToLower().Contains("turkish"))
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "tr").Commit();
                            Lang_Pref.SetValueIndex(16);
                        }
                        else
                        {
                            WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "Auto").Commit();
                            Lang_Pref.SetValueIndex(0);
                        }
                    }
                    else
                    {
                        WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", "Auto").Commit();
                        Lang_Pref.SetValueIndex(0);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                    var Intent = new Intent(ActivityContext, typeof(DeleteAccount_Activity));
                    StartActivity(Intent);

                    ((Tabbed_Main_Activity) ActivityContext).JobRescheduble.StopJob();
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
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

                AddPreferencesFromResource(Resource.Xml.SettingsPrefs_GeneralAccount);

                WowTime_Main_Settings.Shared_Data = PreferenceManager.SharedPreferences;

                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                EditProfile_Pref = FindPreference("editprofile_key");
                AboutMe_Pref = (EditTextPreference) FindPreference("about_me_key");
                EditAccount_Pref = FindPreference("editAccount_key");
                EditSocialLinks_Pref = FindPreference("editSocialLinks_key");
                EditPassword_Pref = FindPreference("editpassword_key");
                BlockedUsers_Pref = FindPreference("blocked_key");
                DeleteAccount_Pref = FindPreference("deleteaccount_key");

                Lang_Pref = (ListPreference) FindPreference("Lang_key");

                //Update Preferences data on Load
                OnSharedPreferenceChanged(WowTime_Main_Settings.Shared_Data, "about_me_key");
                OnSharedPreferenceChanged(WowTime_Main_Settings.Shared_Data, "Lang_key");

                //Delete Preference
                var mCategory_Account = (PreferenceCategory) FindPreference("SectionAccount_key");
                if (!Settings.Show_Settings_Account)
                    mCategory_Account.RemovePreference(EditAccount_Pref);

                if (!Settings.Show_Settings_SocialLinks)
                    mCategory_Account.RemovePreference(EditSocialLinks_Pref);

                if (!Settings.Show_Settings_Password)
                    mCategory_Account.RemovePreference(EditPassword_Pref);

                if (!Settings.Show_Settings_BlockedUsers)
                    mCategory_Account.RemovePreference(BlockedUsers_Pref);

                if (!Settings.Show_Settings_DeleteAccount)
                    mCategory_Account.RemovePreference(DeleteAccount_Pref);
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

                //Add OnChange event to Preferences
                AboutMe_Pref.PreferenceChange += AboutMePref_OnPreferenceChange;
                Lang_Pref.PreferenceChange += LangPref_OnPreferenceChange;

                EditProfile_Pref.PreferenceClick += EditProfilePref_OnPreferenceClick;
                EditAccount_Pref.PreferenceClick += EditAccountPrefOnPreferenceClick;
                EditSocialLinks_Pref.PreferenceClick += EditSocialLinksPref_OnPreferenceClick;
                EditPassword_Pref.PreferenceClick += EditPasswordPref_OnPreferenceClick;
                BlockedUsers_Pref.PreferenceClick += BlockedUsersPref_OnPreferenceClick;
                DeleteAccount_Pref.PreferenceClick += DeleteAccountPref_OnPreferenceClick;
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
                AboutMe_Pref.PreferenceChange -= AboutMePref_OnPreferenceChange;
                Lang_Pref.PreferenceChange -= LangPref_OnPreferenceChange;

                EditProfile_Pref.PreferenceClick -= EditProfilePref_OnPreferenceClick;
                EditAccount_Pref.PreferenceClick -= EditAccountPrefOnPreferenceClick;
                EditSocialLinks_Pref.PreferenceClick -= EditSocialLinksPref_OnPreferenceClick;
                EditPassword_Pref.PreferenceClick -= EditPasswordPref_OnPreferenceClick;
                BlockedUsers_Pref.PreferenceClick -= BlockedUsersPref_OnPreferenceClick;
                DeleteAccount_Pref.PreferenceClick -= DeleteAccountPref_OnPreferenceClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Lang
        private void LangPref_OnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Handled)
                {
                    var etp = (ListPreference) sender;
                    var value = eventArgs.NewValue;

                    Settings.Lang = value.ToString();

                    WowTime_Main_Settings.SetApplicationLang(Activity, Settings.Lang);

                    Toast.MakeText(ActivityContext, GetText(Resource.String.Lbl_Application_Restart), ToastLength.Long).Show();

                    var intent = new Intent(Activity, typeof(SpalshScreen_Activity));
                    intent.AddCategory(Intent.CategoryHome);
                    intent.SetAction(Intent.ActionMain);
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    Activity.StartActivity(intent);
                    Activity.FinishAffinity();

                    Settings.Lang = value.ToString();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //About me 
        private async void AboutMePref_OnPreferenceChange(object sender,
            Preference.PreferenceChangeEventArgs preferenceChangeEventArgs)
        {
            try
            {
                var etp = (EditTextPreference) sender;
                var value = etp.EditText.Text;
                etp.Summary = value;

                var datauser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                if (datauser != null)
                {
                    datauser.about = etp.EditText.Text;
                    S_About = etp.EditText.Text;
                }

                if (IMethods.CheckConnectivity())
                {
                    var dictionary_profile = new Dictionary<string, string>
                    {
                        {"about", S_About}
                    };
                    Settings st = new Settings();
                    var (Api_status, Respond) = await Client.Global.Update_User_Data(st,dictionary_profile);
                    if (Api_status == 200)
                    {
                        if (Respond is Update_User_Data_Object result)
                        {
                            if (result.message.Contains("updated"))
                            {
                                Toast.MakeText(ActivityContext, result.message, ToastLength.Short).Show();
                                AndHUD.Shared.Dismiss(ActivityContext);
                            }
                            else
                            {
                                //Show a Error image with a message
                                AndHUD.Shared.ShowError(ActivityContext, result.message, MaskType.Clear,
                                    TimeSpan.FromSeconds(2));
                            }
                        }
                    }
                    else if (Api_status == 400)
                    {
                        if (Respond is Error_Object error)
                        {
                            var errortext = error._errors.Error_text;
                            Toast.MakeText(ActivityContext, errortext, ToastLength.Short).Show();

                            if (errortext.Contains("Invalid or expired access_token"))
                                API_Request.Logout(Activity);
                        }
                    }
                    else if (Api_status == 404)
                    {
                        var error = Respond.ToString();
                        Toast.MakeText(ActivityContext, error, ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(ActivityContext,
                            ActivityContext.GetString(Resource.String.Lbl_CheckYourInternetConnection),
                            ToastLength.Short)
                        .Show();
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Edit Profile
        private void EditProfilePref_OnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var Intent = new Intent(ActivityContext, typeof(EditMyProfile_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Edit Account
        private void EditAccountPrefOnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var Intent = new Intent(ActivityContext, typeof(MyAccount_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Edit Social Links
        private void EditSocialLinksPref_OnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var Intent = new Intent(ActivityContext, typeof(EditSocialLinks_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Edit Password
        private void EditPasswordPref_OnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var Intent = new Intent(ActivityContext, typeof(Password_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Block users
        private void BlockedUsersPref_OnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var Intent = new Intent(ActivityContext, typeof(BlockedUsers_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Delete Account  
        private void DeleteAccountPref_OnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var dialog = new MaterialDialog.Builder(ActivityContext);

                dialog.Title(Resource.String.Lbl_Warning);
                dialog.Content(ActivityContext.GetText(Resource.String.Lbl_Are_you_DeleteAccount) + " " + Settings.Application_Name);
                dialog.PositiveText(ActivityContext.GetText(Resource.String.Lbl_Ok)).OnPositive(this);
                dialog.NegativeText(ActivityContext.GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialog.ItemsCallback(this).Build().Show();
                dialog.AlwaysCallSingleChoiceCallback();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #region Variables Basic

        private Preference EditProfile_Pref;
        private EditTextPreference AboutMe_Pref;
        private Preference EditAccount_Pref;
        private Preference EditSocialLinks_Pref;
        private Preference EditPassword_Pref;
        private Preference BlockedUsers_Pref;
        private Preference DeleteAccount_Pref;

        private ListPreference Lang_Pref;
        private string S_About = "";

        private readonly Activity ActivityContext;

        #endregion
    }
}