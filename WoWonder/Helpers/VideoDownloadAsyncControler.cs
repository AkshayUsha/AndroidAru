using System;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Widget;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.Videos;
using WoWonder_API.Classes.Movies;

namespace WoWonder.Helpers
{

    public class VideoDownloadAsyncControler
    {
        public DownloadManager Downloadmanager;
        public DownloadManager.Request Request;

        public string FilePath = Android.OS.Environment.ExternalStorageDirectory + "/" + Settings.Application_Name + "/Video/Movie/";
        public string Filename;
        public long DownloadID;
        public static Get_Movies_Object.Movie Video;
        public static Context ContextActivty;

        public VideoDownloadAsyncControler(string url, string filename, Context contextActivty)
        {
            try
            {
                if (!Directory.Exists(FilePath))
                    Directory.CreateDirectory(FilePath);

                if (!filename.Contains(".mp4") || !filename.Contains(".Mp4") || !filename.Contains(".MP4"))
                {
                    Filename = filename + ".mp4";
                }

                ContextActivty = contextActivty;

                Downloadmanager = (DownloadManager)Application.Context.GetSystemService(Context.DownloadService);
                Request = new DownloadManager.Request(Android.Net.Uri.Parse(url));
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void StartDownloadManager(string title, Get_Movies_Object.Movie video)
        {
            try
            {
                if (video != null && !string.IsNullOrEmpty(title))
                {
                    Video = video;
                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    dbDatabase.Insert_WatchOfflineVideos(Video);
                    dbDatabase.Dispose();

                    Request.SetTitle(title);
                    Request.SetAllowedNetworkTypes(DownloadNetwork.Mobile | DownloadNetwork.Wifi);
                    Request.SetDestinationInExternalPublicDir("/" + Settings.Application_Name + "/Video/Movie/", Filename);
                    Request.SetNotificationVisibility(DownloadVisibility.Visible);
                    Request.SetAllowedOverRoaming(true);
                    Request.SetVisibleInDownloadsUi(true);
                    DownloadID = Downloadmanager.Enqueue(Request);

                    OnDownloadComplete onDownloadComplete = new OnDownloadComplete();
                    Application.Context.ApplicationContext.RegisterReceiver(onDownloadComplete, new IntentFilter(DownloadManager.ActionDownloadComplete));
                }
                else
                {
                    Toast.MakeText(ContextActivty, Application.Context.GetText(Resource.String.Lbl_Download_faileds), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void StopDownloadManager()
        {
            try
            {
                Downloadmanager.Remove(DownloadID);
                RemoveDiskVideoFile(Filename);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public static bool RemoveDiskVideoFile(string filename)
        {
            try
            {
                string Path = Android.OS.Environment.ExternalStorageDirectory + "/" + Settings.Application_Name + "/Video/Movie/" + filename;
                if (File.Exists(Path))
                {
                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    dbDatabase.Remove_WatchOfflineVideos(Video.id);
                    dbDatabase.Dispose();

                    File.Delete(Path);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return false;
            }
        }

        public bool CheckDownloadLinkIfExits()
        {
            try
            {
                if (File.Exists(FilePath + Filename))
                    return true;

                return false;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return false;
            }
        }

        public static string GetDownloadedDiskVideoUri(string url)
        {
            try
            {
                string filename = url.Split('/').Last();

                var fullpath = "file://" + Android.Net.Uri.Parse(Android.OS.Environment.ExternalStorageDirectory + "/" + Settings.Application_Name + "/Video/Movie/" + filename + ".mp4");
                if (File.Exists(fullpath))
                    return fullpath;

                var fullpath2 = Android.OS.Environment.ExternalStorageDirectory + "/" + Settings.Application_Name + "/Video/Movie/" + filename + ".mp4";
                if (File.Exists(fullpath2))
                    return fullpath2;


                var fullpath3 = Android.OS.Environment.ExternalStorageDirectory + "/" + Settings.Application_Name + "/Video/Movie/" + filename + ".mp4";
                if (File.Exists(fullpath3))
                    return fullpath3;

                return null;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return null;
            }
        }

        [BroadcastReceiver()]
        [IntentFilter(new[] { DownloadManager.ActionDownloadComplete })]
        public class OnDownloadComplete : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                try
                {
                    if (intent.Action == DownloadManager.ActionDownloadComplete)
                    {
                        DownloadManager downloadManagerExcuter = (DownloadManager)Application.Context.GetSystemService(Context.DownloadService);
                        long downloadId = intent.GetLongExtra(DownloadManager.ExtraDownloadId, -1);
                        DownloadManager.Query query = new DownloadManager.Query();
                        query.SetFilterById(downloadId);
                        ICursor c = downloadManagerExcuter.InvokeQuery(query);

                        if (c.MoveToFirst())
                        {
                            SqLiteDatabase dbDatabase = new SqLiteDatabase();

                            int columnIndex = c.GetColumnIndex(DownloadManager.ColumnStatus);
                            if (c.GetInt(columnIndex) == (int)DownloadStatus.Successful)
                            {
                                string downloadedPath = c.GetString(c.GetColumnIndex(DownloadManager.ColumnLocalUri));

                                ActivityManager.RunningAppProcessInfo appProcessInfo = new ActivityManager.RunningAppProcessInfo();
                                ActivityManager.GetMyMemoryState(appProcessInfo);
                                if (appProcessInfo.Importance == Importance.Foreground || appProcessInfo.Importance == Importance.Background)
                                {

                                    dbDatabase.Update_WatchOfflineVideos(Video.id, downloadedPath);
                                    var videoViewer = ContextActivty as Video_Viewer_Activity;
                                    var fullScreen = ContextActivty as FullScreenVideoActivity;

                                    if (videoViewer != null)
                                    {
                                        if (videoViewer.VideoActionsController.Videodata.id == Video.id)
                                        {
                                            videoViewer.VideoActionsController.Download_icon.SetImageDrawable(Application.Context.GetDrawable(Resource.Drawable.ic_checked_red));
                                            videoViewer.VideoActionsController.Download_icon.Tag = "Downloaded";
                                        }
                                    }
                                    else if (fullScreen != null)
                                    {
                                        if (fullScreen.VideoActionsController.Videodata.id == Video.id)
                                        {
                                            fullScreen.VideoActionsController.Download_icon.SetImageDrawable(Application.Context.GetDrawable(Resource.Drawable.ic_checked_red));
                                            fullScreen.VideoActionsController.Download_icon.Tag = "Downloaded";
                                        }
                                    }
                                }
                                else
                                {
                                    dbDatabase.Update_WatchOfflineVideos(Video.id, downloadedPath);
                                }
                            }
                            dbDatabase.Dispose();
                        }
                    }
                }
                catch (Exception exception)
                {
                    Crashes.TrackError(exception);
                }
            }
        }
    }
}