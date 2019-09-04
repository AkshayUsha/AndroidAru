using System;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Microsoft.AppCenter.Crashes;
using WoWonder.Activities.Default;
using Exception = Java.Lang.Exception;
using Object = Java.Lang.Object;

namespace WoWonder.Helpers.SocialLogins
{
    public class SignInResultCallback : Object, IResultCallback
    {
        public Login_Activity Activity { get; set; }

        public void OnResult(Object result)
        {
            try
            {
                var googleSignInResult = result as GoogleSignInResult;
                Activity.HandleSignInResult(googleSignInResult);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        } 
    }
}