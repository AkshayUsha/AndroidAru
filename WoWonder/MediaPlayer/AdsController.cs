using Android.Content;
using Com.Google.Android.Exoplayer2.Source.Ads;
using Java.IO;
using Java.Lang;

namespace WoWonder.MediaPlayer
{
    public class AdsController : Java.Lang.Object, IAdsLoaderEventListener
    {

        public static Context MainActivity;

        public AdsController(Context activty)
        {
            MainActivity = activty;
        }


        public void Dispose()
        {

        }

        public void OnAdClicked()
        {

        }

        public void OnAdLoadError(IOException p0)
        {

        }

        public void OnAdPlaybackState(AdPlaybackState p0)
        {

        }

        public void OnAdTapped()
        {

        }

        public void OnInternalAdLoadError(RuntimeException p0)
        {

        }
    }

}