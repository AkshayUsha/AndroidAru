using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;
using WoWonder_API.Requests;

namespace WoWonder.Activities.SettingsPreferences.Privacy
{
    public class Settings_Privacy_PrefsFragment : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        public Settings_Privacy_PrefsFragment(Activity context)
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
                var dataUser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                if (key.Equals("whocanfollow_key"))
                {
                    try
                    {
                        var valueAsText = Privacy_CanFollow_Pref.Entry;
                        if (!string.IsNullOrEmpty(valueAsText))
                        {
                            if (dataUser != null)
                            {
                                WowTime_Main_Settings.Shared_Data.Edit()
                                    .PutString("whocanfollow_key", dataUser.follow_privacy).Commit();
                                Privacy_CanFollow_Pref.SetValueIndex(int.Parse(dataUser.follow_privacy));

                                S_CanFollow_Pref = dataUser.follow_privacy;
                                if (S_CanFollow_Pref == "0")
                                    Privacy_CanFollow_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_Everyone);
                                else
                                    Privacy_CanFollow_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_People_i_Follow);
                            }
                        }
                        else
                        {
                            S_CanFollow_Pref = Privacy_CanFollow_Pref.Value;
                            Privacy_CanFollow_Pref.Summary = S_CanFollow_Pref;
                            Privacy_CanFollow_Pref.SetValueIndex(dataUser != null ? int.Parse(dataUser?.message_privacy): 0);
                        }
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }
                }
                else if (key.Equals("whocanMessage_key"))
                {
                    try
                    {
                        var valueAsText = Privacy_CanMessage_Pref.Entry;
                        if (!string.IsNullOrEmpty(valueAsText))
                        {
                            if (dataUser != null)
                            {
                                WowTime_Main_Settings.Shared_Data.Edit().PutString("whocanMessage_key", dataUser.message_privacy).Commit();
                                Privacy_CanMessage_Pref.SetValueIndex(int.Parse(dataUser.message_privacy));

                                S_CanMessage_Pref = dataUser.message_privacy;
                                if (S_CanMessage_Pref == "0")
                                    Privacy_CanMessage_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_Everyone);
                                else
                                    Privacy_CanMessage_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_People_i_Follow);
                            }
                        }
                        else
                        {
                            S_CanMessage_Pref = Privacy_CanMessage_Pref.Value;
                            Privacy_CanMessage_Pref.Summary = S_CanMessage_Pref;
                            Privacy_CanMessage_Pref.SetValueIndex(dataUser != null
                                ? int.Parse(dataUser?.message_privacy)
                                : 0);
                        }
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }
                }
                else if (key.Equals("whoCanSeeMyfriends_key"))
                {
                    try
                    {
                        var valueAsText = Privacy_CanSeeMyfriends_Pref.Entry;
                        if (!string.IsNullOrEmpty(valueAsText))
                        {
                            if (dataUser != null)
                            {
                                WowTime_Main_Settings.Shared_Data.Edit()
                                    .PutString("whoCanSeeMyfriends_key", dataUser.friend_privacy).Commit();
                                Privacy_CanSeeMyfriends_Pref.SetValueIndex(int.Parse(dataUser.friend_privacy));

                                S_CanSeeMyfriends_Pref = dataUser.friend_privacy;
                                if (S_CanSeeMyfriends_Pref == "0")
                                    Privacy_CanSeeMyfriends_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_Everyone);
                                else if (S_CanSeeMyfriends_Pref == "1")
                                    Privacy_CanSeeMyfriends_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_People_i_Follow);
                                else if (S_CanSeeMyfriends_Pref == "2")
                                    Privacy_CanSeeMyfriends_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_People_Follow_Me);
                                else
                                    Privacy_CanSeeMyfriends_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_No_body);
                            }
                        }
                        else
                        {
                            S_CanSeeMyfriends_Pref = Privacy_CanSeeMyfriends_Pref.Value;
                            Privacy_CanSeeMyfriends_Pref.Summary = S_CanSeeMyfriends_Pref;
                            Privacy_CanSeeMyfriends_Pref.SetValueIndex(dataUser != null
                                ? int.Parse(dataUser?.friend_privacy)
                                : 0);
                        }
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }
                }
                else if (key.Equals("whoCanPostOnMyTimeline_key"))
                {
                    try
                    {
                        var valueAsText = Privacy_CanPostOnMyTimeline_Pref.Entry;
                        if (!string.IsNullOrEmpty(valueAsText))
                        {
                            if (dataUser != null)
                            {
                                WowTime_Main_Settings.Shared_Data.Edit()
                                    .PutString("whoCanPostOnMyTimeline_key", dataUser.post_privacy).Commit();
                                Privacy_CanPostOnMyTimeline_Pref.SetValueIndex(int.Parse(dataUser.post_privacy));

                                S_CanPostOnMyTimeline_Pref = dataUser.post_privacy;

                                if (dataUser.post_privacy.Contains("everyone"))
                                {
                                    Privacy_CanPostOnMyTimeline_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_Everyone);
                                }
                                else if (dataUser.post_privacy.Contains("ifollow"))
                                {
                                    Privacy_CanPostOnMyTimeline_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_People_i_Follow);
                                }
                                else if (dataUser.post_privacy.Contains("me"))
                                {
                                    //PostPrivacyButton.Text = this.GetString(Resource.String.Lbl_People_Follow_Me);
                                }
                                else
                                {
                                    Privacy_CanPostOnMyTimeline_Pref.Summary = ActivityContext.GetString(Resource.String.Lbl_No_body);
                                }
                            }
                        }
                        else
                        {
                            S_CanPostOnMyTimeline_Pref = Privacy_CanPostOnMyTimeline_Pref.Value;
                            Privacy_CanPostOnMyTimeline_Pref.Summary = S_CanPostOnMyTimeline_Pref;

                            if (dataUser != null && dataUser.post_privacy.Contains("everyone"))
                                Privacy_CanPostOnMyTimeline_Pref.SetValueIndex(0);
                            else if (dataUser != null && dataUser.post_privacy.Contains("ifollow"))
                                Privacy_CanPostOnMyTimeline_Pref.SetValueIndex(1);
                            else
                                Privacy_CanPostOnMyTimeline_Pref.SetValueIndex(0);
                        }
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }
                }
                else if (key.Equals("whoCanSeeMyBirthday_key"))
                {
                    try
                    {
                        var valueAsText = Privacy_CanSeeMyBirthday_Pref.Entry;
                        if (!string.IsNullOrEmpty(valueAsText))
                        {
                            if (dataUser != null)
                            {
                                WowTime_Main_Settings.Shared_Data.Edit()
                                    .PutString("whoCanSeeMyBirthday_key", dataUser.birth_privacy).Commit();
                                Privacy_CanSeeMyBirthday_Pref.SetValueIndex(int.Parse(dataUser.birth_privacy));

                                S_CanSeeMyBirthday_Pref = dataUser.birth_privacy;
                                if (S_CanSeeMyBirthday_Pref == "0")
                                    Privacy_CanSeeMyBirthday_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_Everyone);
                                else if (S_CanSeeMyBirthday_Pref == "1")
                                    Privacy_CanSeeMyBirthday_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_People_i_Follow);
                                else
                                    Privacy_CanSeeMyBirthday_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_No_body);
                            }
                        }
                        else
                        {
                            S_CanSeeMyBirthday_Pref = Privacy_CanSeeMyBirthday_Pref.Value;
                            Privacy_CanSeeMyBirthday_Pref.Summary = S_CanSeeMyBirthday_Pref;
                            Privacy_CanSeeMyBirthday_Pref.SetValueIndex(dataUser != null
                                ? int.Parse(dataUser?.birth_privacy)
                                : 0);
                        }
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }
                }
                else if (key.Equals("ConfirmRequestFollows_key"))
                {
                    try
                    {
                        var Getvalue = WowTime_Main_Settings.Shared_Data.GetBoolean("ConfirmRequestFollows_key", true);
                        Privacy_ConfirmRequestFollows_Pref.Checked = Getvalue; 
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }
                }
                else if (key.Equals("ShowMyActivities_key"))
                {
                    try
                    {
                        var Getvalue = WowTime_Main_Settings.Shared_Data.GetBoolean("ShowMyActivities_key", true);
                        Privacy_ShowMyActivities_Pref.Checked = Getvalue;
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }
                }
                else if (key.Equals("Status_key"))
                {
                    try
                    {
                        var valueAsText = Privacy_Status_Pref.Entry;
                        if (!string.IsNullOrEmpty(valueAsText))
                        {
                            if (dataUser != null)
                            {
                                WowTime_Main_Settings.Shared_Data.Edit().PutString("Status_key", dataUser.status)
                                    .Commit();
                                Privacy_Status_Pref.SetValueIndex(int.Parse(dataUser.status));

                                S_Status_Pref = dataUser.status;
                                if (S_Status_Pref == "0")
                                    Privacy_Status_Pref.Summary = ActivityContext.GetString(Resource.String.Lbl_Online);
                                else
                                    Privacy_Status_Pref.Summary =
                                        ActivityContext.GetString(Resource.String.Lbl_Offline);
                            }
                        }
                        else
                        {
                            S_Status_Pref = Privacy_Status_Pref.Value;
                            Privacy_Status_Pref.Summary = S_Status_Pref;
                            Privacy_Status_Pref.SetValueIndex(dataUser != null ? int.Parse(dataUser?.status) : 0);
                        }
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }
                }
                else if (key.Equals("ShareMyLocation_key"))
                {
                    try
                    {
                        var Getvalue = WowTime_Main_Settings.Shared_Data.GetBoolean("ShareMyLocation_key", true);
                        Privacy_ShareMyLocation_Pref.Checked = Getvalue;
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }
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
                AddPreferencesFromResource(Resource.Xml.SettingsPrefs_Privacy);

                WowTime_Main_Settings.Shared_Data = PreferenceManager.SharedPreferences;

                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                Privacy_CanFollow_Pref = (ListPreference) FindPreference("whocanfollow_key");
                Privacy_CanMessage_Pref = (ListPreference) FindPreference("whocanMessage_key");
                Privacy_CanSeeMyfriends_Pref = (ListPreference) FindPreference("whoCanSeeMyfriends_key");
                Privacy_CanPostOnMyTimeline_Pref = (ListPreference) FindPreference("whoCanPostOnMyTimeline_key");
                Privacy_CanSeeMyBirthday_Pref = (ListPreference) FindPreference("whoCanSeeMyBirthday_key");
                Privacy_ConfirmRequestFollows_Pref = (SwitchPreference) FindPreference("ConfirmRequestFollows_key");
                Privacy_ShowMyActivities_Pref = (SwitchPreference) FindPreference("ShowMyActivities_key");
                Privacy_Status_Pref = (ListPreference) FindPreference("Status_key");
                Privacy_ShareMyLocation_Pref = (SwitchPreference) FindPreference("ShareMyLocation_key");

                if (Classes.MyProfileList.Count == 0)
                {
                    SqLiteDatabase sdDatabase = new SqLiteDatabase();
                    sdDatabase.Get_MyProfile_CredentialList();
                    sdDatabase.Dispose();
                }

                //Update Preferences data on Load
                OnSharedPreferenceChanged(WowTime_Main_Settings.Shared_Data, "whocanfollow_key");
                OnSharedPreferenceChanged(WowTime_Main_Settings.Shared_Data, "whocanMessage_key");
                OnSharedPreferenceChanged(WowTime_Main_Settings.Shared_Data, "whoCanSeeMyfriends_key");
                OnSharedPreferenceChanged(WowTime_Main_Settings.Shared_Data, "whoCanPostOnMyTimeline_key");
                OnSharedPreferenceChanged(WowTime_Main_Settings.Shared_Data, "whoCanSeeMyBirthday_key");
                OnSharedPreferenceChanged(WowTime_Main_Settings.Shared_Data, "ConfirmRequestFollows_key");
                OnSharedPreferenceChanged(WowTime_Main_Settings.Shared_Data, "ShowMyActivities_key");
                OnSharedPreferenceChanged(WowTime_Main_Settings.Shared_Data, "Status_key");
                OnSharedPreferenceChanged(WowTime_Main_Settings.Shared_Data, "ShareMyLocation_key");
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
                Privacy_CanFollow_Pref.PreferenceChange += PrivacyCanFollowPref_OnPreferenceChange;
                Privacy_CanMessage_Pref.PreferenceChange += PrivacyCanMessagePref_OnPreferenceChange;
                Privacy_CanSeeMyfriends_Pref.PreferenceChange += PrivacyCanSeeMyfriendsPref_OnPreferenceChange;
                Privacy_CanPostOnMyTimeline_Pref.PreferenceChange += PrivacyCanPostOnMyTimelinePref_OnPreferenceChange;
                Privacy_CanSeeMyBirthday_Pref.PreferenceChange += PrivacyCanSeeMyBirthdayPref_OnPreferenceChange;
                Privacy_ConfirmRequestFollows_Pref.PreferenceChange += PrivacyConfirmRequestFollowsPref_OnPreferenceChange;
                Privacy_ShowMyActivities_Pref.PreferenceChange += PrivacyShowMyActivitiesPref_OnPreferenceChange;
                Privacy_Status_Pref.PreferenceChange += PrivacyStatusPref_OnPreferenceChange;
                Privacy_ShareMyLocation_Pref.PreferenceChange += PrivacyShareMyLocationPref_OnPreferenceChange;
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
                Privacy_CanFollow_Pref.PreferenceChange -= PrivacyCanFollowPref_OnPreferenceChange;
                Privacy_CanMessage_Pref.PreferenceChange -= PrivacyCanMessagePref_OnPreferenceChange;
                Privacy_CanSeeMyfriends_Pref.PreferenceChange -= PrivacyCanSeeMyfriendsPref_OnPreferenceChange;
                Privacy_CanPostOnMyTimeline_Pref.PreferenceChange -= PrivacyCanPostOnMyTimelinePref_OnPreferenceChange;
                Privacy_CanSeeMyBirthday_Pref.PreferenceChange -= PrivacyCanSeeMyBirthdayPref_OnPreferenceChange;
                Privacy_ConfirmRequestFollows_Pref.PreferenceChange -=PrivacyConfirmRequestFollowsPref_OnPreferenceChange;
                Privacy_ShowMyActivities_Pref.PreferenceChange -= PrivacyShowMyActivitiesPref_OnPreferenceChange;
                Privacy_Status_Pref.PreferenceChange -= PrivacyStatusPref_OnPreferenceChange;
                Privacy_ShareMyLocation_Pref.PreferenceChange -= PrivacyShareMyLocationPref_OnPreferenceChange;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Who can follow me
        private void PrivacyCanFollowPref_OnPreferenceChange(object sender,Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Handled)
                {
                    var etp = (ListPreference) sender;
                    var value = eventArgs.NewValue.ToString();
                    var valueAsText = etp.GetEntries()[int.Parse(value)];
                    etp.Summary = valueAsText;

                    S_CanFollow_Pref = value;

                    if (IMethods.CheckConnectivity())
                    {
                        var datauser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                        if (datauser != null) datauser.follow_privacy = S_CanFollow_Pref;

                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"follow_privacy", S_CanFollow_Pref}
                        };

                        var data = Client.Global.Update_User_Data(st ,dataPrivacy).ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext,
                                ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection),
                                ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Who can message me
        private void PrivacyCanMessagePref_OnPreferenceChange(object sender,
            Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Handled)
                {
                    var etp = (ListPreference) sender;
                    var value = eventArgs.NewValue.ToString();
                    var valueAsText = etp.GetEntries()[int.Parse(value)];
                    etp.Summary = valueAsText;

                    S_CanMessage_Pref = value;

                    if (IMethods.CheckConnectivity())
                    {
                        var datauser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                        if (datauser != null) datauser.message_privacy = S_CanMessage_Pref;

                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"message_privacy", S_CanMessage_Pref}
                        };

                        var data = Client.Global.Update_User_Data(st ,dataPrivacy).ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext,
                                ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection),
                                ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Who can see my friends
        private void PrivacyCanSeeMyfriendsPref_OnPreferenceChange(object sender,
            Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Handled)
                {
                    var etp = (ListPreference) sender;
                    var value = eventArgs.NewValue.ToString();
                    var valueAsText = etp.GetEntries()[int.Parse(value)];
                    etp.Summary = valueAsText;

                    S_CanSeeMyfriends_Pref = value;

                    if (IMethods.CheckConnectivity())
                    {
                        var datauser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                        if (datauser != null) datauser.friend_privacy = S_CanSeeMyfriends_Pref;

                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"friend_privacy", S_CanSeeMyfriends_Pref}
                        };

                        var data = Client.Global.Update_User_Data(st ,dataPrivacy).ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext,
                                ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection),
                                ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Who can post on my timeline
        private void PrivacyCanPostOnMyTimelinePref_OnPreferenceChange(object sender,
            Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Handled)
                {
                    var etp = (ListPreference) sender;
                    var value = eventArgs.NewValue.ToString();
                    var valueAsText = etp.GetEntries()[int.Parse(value)];
                    etp.Summary = valueAsText;

                    S_CanPostOnMyTimeline_Pref = value;

                    if (IMethods.CheckConnectivity())
                    {
                        var datauser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                        if (datauser != null) datauser.post_privacy = S_CanPostOnMyTimeline_Pref;


                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"post_privacy", S_CanPostOnMyTimeline_Pref}
                        };

                        var data = Client.Global.Update_User_Data(st ,dataPrivacy).ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext,
                                ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection),
                                ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Who can see my birthday
        private void PrivacyCanSeeMyBirthdayPref_OnPreferenceChange(object sender,
            Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Handled)
                {
                    var etp = (ListPreference) sender;
                    var value = eventArgs.NewValue.ToString();
                    var valueAsText = etp.GetEntries()[int.Parse(value)];
                    etp.Summary = valueAsText;

                    S_CanSeeMyBirthday_Pref = value;


                    if (IMethods.CheckConnectivity())
                    {
                        var datauser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                        if (datauser != null) datauser.birth_privacy = S_CanSeeMyBirthday_Pref;

                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"birth_privacy", S_CanSeeMyBirthday_Pref}
                        };

                        var data = Client.Global.Update_User_Data(st ,dataPrivacy).ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext,
                                ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection),
                                ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Confirm request when someone follows you
        private void PrivacyConfirmRequestFollowsPref_OnPreferenceChange(object sender,Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Handled)
                {
                    var datauser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);

                    var etp = (SwitchPreference)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (etp.Checked)
                    {
                        S_ConfirmRequestFollows_Pref = "1";
                        if (datauser != null) datauser.confirm_followers = "1";
                    }
                    else
                    {
                        S_ConfirmRequestFollows_Pref = "0";
                        if (datauser != null) datauser.confirm_followers = "0";
                    }
                     
                    if (IMethods.CheckConnectivity())
                    {
                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"confirm_followers", S_ConfirmRequestFollows_Pref}
                        };
                       
                        var data = Client.Global.Update_User_Data(st ,dataPrivacy).ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext,
                                ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //Show my activities
        private void PrivacyShowMyActivitiesPref_OnPreferenceChange(object sender,
            Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Handled)
                {
                    var datauser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                    var etp = (SwitchPreference) sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (etp.Checked)
                    {
                        if (datauser != null) datauser.show_activities_privacy = "1";
                        S_ShowMyActivities_Pref = "1";
                    }
                    else
                    {
                        if (datauser != null) datauser.show_activities_privacy = "0";
                        S_ShowMyActivities_Pref = "0";
                    }

                    if (IMethods.CheckConnectivity())
                    {
                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"show_activities_privacy", S_ShowMyActivities_Pref}
                        };

                        var data = Client.Global.Update_User_Data(st ,dataPrivacy).ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext,
                                ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection),
                                ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Status
        private void PrivacyStatusPref_OnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Handled)
                {
                    var etp = (ListPreference) sender;
                    var value = eventArgs.NewValue.ToString();
                    var valueAsText = etp.GetEntries()[int.Parse(value)];
                    etp.Summary = valueAsText;

                    S_Status_Pref = value;

                    if (IMethods.CheckConnectivity())
                    {
                        var datauser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                        if (datauser != null) datauser.status = S_Status_Pref;

                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"status", S_Status_Pref}
                        };

                        var data = Client.Global.Update_User_Data(st ,dataPrivacy).ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext,
                                ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection),
                                ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Share my location with public
        private void PrivacyShareMyLocationPref_OnPreferenceChange(object sender,
            Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Handled)
                {
                    var datauser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                    var etp = (SwitchPreference) sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (etp.Checked)
                    {
                        if (datauser != null) datauser.share_my_location = "1";
                        S_ShareMyLocation_Pref = "1";
                    }
                    else
                    {
                        if (datauser != null) datauser.share_my_location = "0";
                        S_ShareMyLocation_Pref = "0";
                    }

                   
                    if (datauser != null) datauser.share_my_location = S_ShareMyLocation_Pref;

                    if (IMethods.CheckConnectivity())
                    { 
                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"share_my_location", S_ShareMyLocation_Pref}
                        };

                        var data = Client.Global.Update_User_Data(st,dataPrivacy).ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext,
                                ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection),
                                ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #region Variables Basic

        private ListPreference Privacy_CanFollow_Pref;
        private ListPreference Privacy_CanMessage_Pref;
        private ListPreference Privacy_CanSeeMyfriends_Pref;
        private ListPreference Privacy_CanPostOnMyTimeline_Pref;
        private ListPreference Privacy_CanSeeMyBirthday_Pref;
        private SwitchPreference Privacy_ConfirmRequestFollows_Pref;
        private SwitchPreference Privacy_ShowMyActivities_Pref;
        private ListPreference Privacy_Status_Pref;
        private SwitchPreference Privacy_ShareMyLocation_Pref;
        private Settings st = new Settings();
        public string S_CanFollow_Pref = "0";
        public string S_CanMessage_Pref = "0";
        public string S_CanSeeMyfriends_Pref = "0";
        public string S_CanPostOnMyTimeline_Pref = "0";
        public string S_CanSeeMyBirthday_Pref = "0";
        public string S_ConfirmRequestFollows_Pref = "0";
        public string S_ShowMyLastSeen_Pref = "0";
        public string S_ShowMyActivities_Pref = "0";
        public string S_Status_Pref = "0";
        public string S_ShareMyLocation_Pref = "0";

        private readonly Activity ActivityContext;

        #endregion
    }
}