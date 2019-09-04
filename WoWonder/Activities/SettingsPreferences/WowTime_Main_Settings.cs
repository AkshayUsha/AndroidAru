using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Preferences;
using Java.Util;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;

namespace WoWonder.Activities.SettingsPreferences
{
    public class WowTime_Main_Settings
    {
        public static ISharedPreferences Shared_Data;
        public static string Local_Language = "";

        public static async void Init()
        {
            try
            {
                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                var ss = await dbDatabase.CheckTablesStatus();
                dbDatabase.OpenConnection();
                dbDatabase.Get_MyProfile_CredentialList();

                Shared_Data = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
                var data = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                if (data != null)
                {
                    Shared_Data.Edit().PutString("whocanfollow_key", data.follow_privacy).Commit();
                    Shared_Data.Edit().PutString("whocanMessage_key", data.message_privacy).Commit();
                    Shared_Data.Edit().PutString("whoCanSeeMyfriends_key", data.friend_privacy).Commit();
                    Shared_Data.Edit().PutString("whoCanPostOnMyTimeline_key", data.post_privacy).Commit();
                    Shared_Data.Edit().PutString("whoCanSeeMyBirthday_key", data.birth_privacy).Commit();
                    Shared_Data.Edit().PutString("ConfirmRequestFollows_key", data.confirm_followers).Commit();
                    Shared_Data.Edit().PutString("ShowMyActivities_key", data.show_activities_privacy).Commit();
                    Shared_Data.Edit().PutString("Status_key", data.status).Commit();
                    Shared_Data.Edit().PutString("ShareMyLocation_key", data.share_my_location).Commit();
                }

                SetDefaultSettings(); 
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public static void SetDefaultSettings()
        {
            try
            {
                //Shared_Data.Edit().PutString("Lang_key", "Auto").Commit();
                if (Settings.Lang != "")
                {
                    if (Settings.Lang == "ar")
                    {
                        Shared_Data.Edit().PutString("Lang_key", "ar").Commit();
                        Settings.Lang = "ar";
                        Settings.FlowDirection_RightToLeft = true;
                    }
                    else
                    {
                        Shared_Data.Edit().PutString("Lang_key", Settings.Lang).Commit();
                        Settings.FlowDirection_RightToLeft = false;
                    }
                }
                else
                {
                    Settings.FlowDirection_RightToLeft = false;

                    var Lang = Shared_Data.GetString("Lang_key", Settings.Lang);
                    if (Lang == "ar")
                    {
                        Shared_Data.Edit().PutString("Lang_key", "ar").Commit();
                        Settings.Lang = "ar";
                        Settings.FlowDirection_RightToLeft = true;
                    }
                    else if (Lang == "Auto")
                    {
                        Shared_Data.Edit().PutString("Lang_key", "Auto").Commit();
                    }
                    else
                    {
                        Shared_Data.Edit().PutString("Lang_key", Lang).Commit();
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public static void SetApplicationLang(Context context, string lang)
        {
            try
            { 
                var config = new Configuration();
                Settings.Lang = lang;

                if (string.IsNullOrEmpty(lang))
                {
                    if (lang == "Auto" || lang == "")
                    {
                        config.Locale = Locale.Default;
                        Local_Language = config.Locale.Language;
                    }
                    else
                    {
                        config.Locale = Locale.Default = new Locale(lang);
                    }

                    if (config.Locale.Language.Contains("ar"))
                    {
                        Settings.Lang = "ar";
                        Settings.FlowDirection_RightToLeft = true;
                    }
                    else
                    {
                        Settings.FlowDirection_RightToLeft = false;
                    }
                }
                else
                {
                    config.Locale = Locale.Default = new Locale(lang);
                    context.Resources.Configuration.Locale = Locale.Default = new Locale(lang);
                    Shared_Data.Edit().PutString("Lang_key", lang).Commit();

                    if (lang.Contains("ar"))
                    {
                        Settings.Lang = "ar";
                        Settings.FlowDirection_RightToLeft = true;
                    }
                    else 
                    {
                        Settings.Lang = lang;
                    }
                }

                //Shared_Data.Edit().PutString("Lang_key", lang).Commit();
                //context.Resources.UpdateConfiguration(config, context.Resources.DisplayMetrics);

                SetDefaultSettings();


                MyContextWrapper.Wrap(context, Settings.Lang);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }
    }
}