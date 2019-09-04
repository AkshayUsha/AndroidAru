using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;
using Com.Luseen.Autolinklibrary;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities;
using WoWonder.Activities.Videos;

namespace WoWonder.Helpers
{
    public class TextSanitizer
    {
        public AutoLinkTextView AutoLinkTextView;
        public Activity _activity;

        public TextSanitizer(AutoLinkTextView linkTextView , Activity activity )
        {
            try
            {
                AutoLinkTextView = linkTextView;
                _activity = activity;
                AutoLinkTextView.AutoLinkOnClick += AutoLinkTextViewOnAutoLinkOnClick;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }
        
        public void Load(string text)
        {
            try
            {
                AutoLinkTextView.AddAutoLinkMode(AutoLinkMode.ModePhone, AutoLinkMode.ModeEmail, AutoLinkMode.ModeHashtag, AutoLinkMode.ModeUrl, AutoLinkMode.ModeMention,AutoLinkMode.ModeCustom);
                AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(_activity, Resource.Color.AutoLinkText_ModePhone_color));
                AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(_activity, Resource.Color.AutoLinkText_ModeEmail_color));
                AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(_activity, Resource.Color.AutoLinkText_ModeHashtag_color));
                AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(_activity, Resource.Color.AutoLinkText_ModeUrl_color));
                AutoLinkTextView.SetMentionModeColor(Color.ParseColor(Settings.MainColor));
                var textt = text.Split('/');
                if (textt.Count() > 1)
                {
                   
                    AutoLinkTextView.SetCustomModeColor(ContextCompat.GetColor(_activity, Resource.Color.AutoLinkText_ModeUrl_color));

                    AutoLinkTextView.SetCustomRegex(@"\b("+ textt.LastOrDefault() + @")\b");
                }

               string LastString =  text.Replace(" /", " ");
                if (!string.IsNullOrEmpty(LastString))
                   AutoLinkTextView.SetAutoLinkText(LastString);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void AutoLinkTextViewOnAutoLinkOnClick(object sender, AutoLinkOnClickEventArgs autoLinkOnClickEventArgs)
        {
            try
            {
                var typetext = IMethods.Fun_String.Check_Regex(autoLinkOnClickEventArgs.P1);
                if (typetext == "Email")
                {
                    IMethods.IApp.SendEmail(Application.Context, autoLinkOnClickEventArgs.P1);
                    return;
                }
                else if (typetext == "Website")
                {
                    String url = autoLinkOnClickEventArgs.P1;
                    if (!autoLinkOnClickEventArgs.P1.Contains("http"))
                    {
                        url = "http://" + autoLinkOnClickEventArgs.P1;
                    }
                   
                    var intent = new Intent(Application.Context, typeof(LocalWebView_Activity));
                    intent.PutExtra("URL", url);
                    intent.PutExtra("Type", url);
                    _activity.StartActivity(intent);
                    return;

                }
                else if (typetext == "Hashtag")
                {
                   
                    return;
                }
                else if (typetext == "Mention")
                {
                   
                    return;
                }
                else if (typetext == "Number")
                {
                    IMethods.IApp.SaveContacts(_activity, autoLinkOnClickEventArgs.P1, "", "2");
                    return;
                }
                else
                {
                    return;
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }
    }
}