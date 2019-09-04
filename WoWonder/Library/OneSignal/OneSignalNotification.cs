using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Widget;
using Com.OneSignal.Abstractions;
using Com.OneSignal.Android;
using Org.Json;
using SettingsConnecter;
using WoWonder.Activities.Tabbes;
using OSNotification = Com.OneSignal.Abstractions.OSNotification;
using OSNotificationPayload = Com.OneSignal.Abstractions.OSNotificationPayload;
using WoWonder.Helpers;

namespace WoWonder.Library.OneSignal
{
    public class OneSignalNotification
    {
        //Force your app to Register notifcation derictly without loading it from server (For Best Result)

        //push_id_2 : http://prntscr.com/l6d4al
        public static string OneSignalAPP_ID = "0eded64f-651e-472b-ac5a-69ecd785a94f";
        public static string userid;

        public static void RegisterNotificationDevice()
        {
            try
            {
                if (UserDetails.NotificationPopup)
                {
                    if (OneSignalAPP_ID != "")
                    {
                        Com.OneSignal.OneSignal.Current.StartInit(OneSignalAPP_ID)
                            .InFocusDisplaying(OSInFocusDisplayOption.Notification)
                            .HandleNotificationReceived(HandleNotificationReceived)
                            .HandleNotificationOpened(HandleNotificationOpened)
                            .EndInit();
                        Com.OneSignal.OneSignal.Current.IdsAvailable(IdsAvailable);
                        Com.OneSignal.OneSignal.Current.RegisterForPushNotifications();

                        Settings.ShowNotification = true;
                    }
                }
                else
                {
                    Un_RegisterNotificationDevice();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void Un_RegisterNotificationDevice()
        {
            try
            {
                Com.OneSignal.OneSignal.Current.SetSubscription(false);
                Settings.ShowNotification = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void IdsAvailable(string userID, string pushToken)
        {
            try
            {
                UserDetails.Device_ID = userID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);  
            }
        }

        private static void HandleNotificationReceived(OSNotification notification)
        {
            try
            {

                OSNotificationPayload payload = notification.payload;
                Dictionary<string, object> additionalData = payload.additionalData;



                //if (Settings.Enable_Audio_Video_Call)
                //{
                //    if (additionalData.ContainsKey("room_name") && Settings.Use_Agora_Library)
                //    {

                //        string room_name = additionalData["room_name"].ToString();
                //        string Call_type = additionalData["call_type"].ToString();
                //        string Call_id = additionalData["call_id"].ToString();
                //        string From_id = additionalData["from_id"].ToString();
                //        string to_id = additionalData["to_id"].ToString();



                //        Toast.MakeText(Application.Context, "GotNotification", ToastLength.Long).Show();
                //        var datauser = Classes.UserList.FirstOrDefault(a => a.user_id == From_id);
                //        if (datauser != null)
                //        {

                //            string AvatarSplit = datauser.profile_picture.Split('/').Last();
                //            var GetImg = IMethods.MultiMedia.GetMediaFrom_Disk("Images", AvatarSplit);

                //            if (datauser.profile_picture != null)
                //            {
                //                if (GetImg == "File Dont Exists")
                //                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync("Images", datauser.profile_picture);
                //                GetImg = datauser.profile_picture;
                //            }

                //            Intent intent = new Intent(Application.Context, typeof(AgoraAudioCallActivity));

                //            if (Call_type == "audio")
                //            {
                //                intent.PutExtra("UserID", From_id);
                //                intent.PutExtra("avatar", GetImg);
                //                intent.PutExtra("name", datauser.profile_picture);
                //                intent.PutExtra("CallID", Call_id);
                //                intent.PutExtra("room_name", room_name);
                //                intent.PutExtra("type", "Agora_audio_call_recieve");
                //                NotificationManagerClass.StartinCommingCall(intent, GetImg, "Voice Call", datauser.name + " is calling you", notification.androidNotificationId); //Allen
                //            }
                //            else
                //            {
                //                intent = new Intent(Application.Context, typeof(AgoraVideoCallActivity));
                //                intent.PutExtra("UserID", From_id);
                //                intent.PutExtra("avatar", GetImg);
                //                intent.PutExtra("name", datauser.profile_picture);
                //                intent.PutExtra("CallID", Call_id);
                //                intent.PutExtra("room_name", room_name);
                //                intent.PutExtra("type", "Agora_video_call_recieve");
                //                NotificationManagerClass.StartinCommingCall(intent, GetImg, "Video Call", datauser.name + " is calling you", notification.androidNotificationId); //Allen

                //            }
                //        }
                //        else
                //        {

                //        }



                //    }
                //    else if (additionalData.ContainsKey("access_token") && Settings.Use_Twilio_Library)
                //    {

                //    }
                //}

                string message = payload.body;

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show(); //Allen
                Console.WriteLine(ex);
            }
        }

        private static void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
            try
            {
                OSNotificationPayload payload = result.notification.payload;
                Dictionary<string, object> additionalData = payload.additionalData;
                string message = payload.body;
                string actionID = result.action.actionID;

                if (additionalData != null)
                {
                    foreach (var item in additionalData)
                    {
                        if (item.Key == "user_id")
                        {
                            userid = item.Value.ToString();
                        }
                    }

                    Intent intent = new Intent(Application.Context, typeof(Tabbed_Main_Activity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    intent.AddFlags(ActivityFlags.SingleTop);
                    intent.SetAction(Intent.ActionView);
                    Application.Context.StartActivity(intent);

                    if (additionalData.ContainsKey("discount"))
                    {
                        // Take user to your store..

                    }
                }
                if (actionID != null)
                {
                    // actionSelected equals the id on the button the user pressed.
                    // actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present. 


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public class NotificationExtenderServiceHandeler : NotificationExtenderService, NotificationCompat.IExtender
    {
        protected override void OnHandleIntent(Intent intent)
        {

        }

        protected override bool OnNotificationProcessing(OSNotificationReceivedResult p0)
        {
            OverrideSettings overrideSettings = new OverrideSettings();
            overrideSettings.Extender = new NotificationCompat.CarExtender();

            Com.OneSignal.Android.OSNotificationPayload payload = p0.Payload;
            JSONObject additionalData = payload.AdditionalData;

            if (additionalData.Has("room_name"))
            {
                string room_name = additionalData.Get("room_name").ToString();
                string Call_type = additionalData.Get("call_type").ToString();
                string Call_id = additionalData.Get("call_id").ToString();
                string From_id = additionalData.Get("from_id").ToString();
                string to_id = additionalData.Get("to_id").ToString();

                return false;
            }
            else
            {
                return true;
            }
        }

        public NotificationCompat.Builder Extend(NotificationCompat.Builder builder)
        {
            return builder;
        }
    }

}