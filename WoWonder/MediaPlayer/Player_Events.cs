using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using Microsoft.AppCenter.Crashes;
using Object = Java.Lang.Object;

namespace WoWonder.MediaPlayer
{
    public class Player_Events : Java.Lang.Object, IPlayerEventListener, PlaybackControlView.IVisibilityListener
    {
        public Activity ActContext;
        public ProgressBar Loadingprogress_bar;
        public ImageButton videoPlayButton;
        public ImageButton videoResumeButton;

        public Player_Events(Activity Act, PlaybackControlView controlView)
        {
            try
            {
                ActContext = Act;

                if (controlView != null)
                {
                    videoPlayButton = controlView.FindViewById<ImageButton>(Resource.Id.exo_play);
                    videoResumeButton = controlView.FindViewById<ImageButton>(Resource.Id.exo_pause);
                    Loadingprogress_bar = ActContext.FindViewById<ProgressBar>(Resource.Id.progress_bar);
                }

                
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
         
        }

        public void OnLoadingChanged(bool p0)
        {

        }

        public void OnPlaybackParametersChanged(PlaybackParameters p0)
        {

        }

        public void OnPlayerError(ExoPlaybackException p0)
        {

        }

        public void OnPlayerStateChanged(bool playWhenReady, int playbackState)
        {
            try
            {
                if (videoResumeButton == null || videoPlayButton == null || Loadingprogress_bar== null)
                    return;

                if (playbackState == Player.StateEnded)
                {
                    if (playWhenReady == false)
                    {
                        videoResumeButton.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        videoResumeButton.Visibility = ViewStates.Gone;
                        videoPlayButton.Visibility = ViewStates.Visible;
                    }

                    Loadingprogress_bar.Visibility = ViewStates.Invisible;
                }
                else if (playbackState == Player.StateReady)
                {
                    if (playWhenReady == false)
                    {
                       videoResumeButton.Visibility = ViewStates.Gone;
                       videoPlayButton.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        videoResumeButton.Visibility = ViewStates.Visible;
                    }

                   Loadingprogress_bar.Visibility = ViewStates.Invisible;
                }
                else if (playbackState == Player.StateBuffering)
                {
                    Loadingprogress_bar.Visibility = ViewStates.Visible;
                    videoResumeButton.Visibility = ViewStates.Invisible;
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void OnPositionDiscontinuity(int p0)
        {

        }

        public void OnRepeatModeChanged(int p0)
        {

        }

        public void OnSeekProcessed()
        {

        }

        public void OnShuffleModeEnabledChanged(bool p0)
        {

        }

        public void OnTimelineChanged(Timeline p0, Object p1, int p2)
        {

        }

        public void OnTracksChanged(TrackGroupArray p0, TrackSelectionArray p1)
        {

        }

        public void OnVisibilityChange(int p0)
        {

        }



    }
}