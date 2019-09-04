using Android.Gms.Common.Apis;
using Java.Lang;
using WoWonder.Activities.Default;

namespace WoWonder.Helpers.SocialLogins
{
    public class SignOutResultCallback : Object, IResultCallback
    {
        public Login_Activity Activity { get; set; }

        public void OnResult(Object result)
        {
            //Activity.UpdateUI(false);
        }
    }
}