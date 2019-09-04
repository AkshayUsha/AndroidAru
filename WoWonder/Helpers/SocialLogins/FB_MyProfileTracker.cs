using System;
using Microsoft.AppCenter.Crashes;
using Xamarin.Facebook;
using Exception = System.Exception;


namespace WoWonder.Helpers.SocialLogins
{
    public class FB_MyProfileTracker : ProfileTracker
    {
        public event EventHandler<OnProfileChangedEventArgs> mOnProfileChanged;

        protected override void OnCurrentProfileChanged(Profile oldProfile, Profile currentProfile)
        {
            try
            {
                mOnProfileChanged?.Invoke(this, new OnProfileChangedEventArgs(currentProfile));
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }  
        }
    }

    public class OnProfileChangedEventArgs : EventArgs
    {
        public Profile mProfile;
        public OnProfileChangedEventArgs(Profile profile)
        {
            try
            {
                mProfile = profile;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }           
        }
        //Extract or delete HTML tags based on their name or whether or not they contain some attributes or content with the HTML editor pro online program.
    }
}