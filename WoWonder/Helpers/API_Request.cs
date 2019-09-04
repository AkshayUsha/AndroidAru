using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Common.Apis;
using Android.Widget;
using Microsoft.AppCenter.Crashes;
using MimeTypes.Core;
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Activities.AddPost.Adapters;
using WoWonder.Activities.Default;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.Tabbes;
using WoWonder_API;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Story;
using WoWonder_API.Classes.User;
using Client = WoWonder_API.Requests.Client;


namespace WoWonder.Helpers
{
    public class API_Request  
    {
        public static string API_Get_users_friends = WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone&type=get_user_list_info";
        public static string API_Add_new_post = WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone&type=new_post";

        public static bool is_Friend = false;

        public static async Task<(int, dynamic)> Get_users_friends_Async(string Aftercontact)
        {
            try
            {
                
                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("user_profile_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                        new KeyValuePair<string, string>("after_user_id", Aftercontact),
                        new KeyValuePair<string, string>("list_type", "all"),
                    });

                    var response = await client.PostAsync(API_Get_users_friends, formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Classes.UserContacts.Rootobject>(json);
                    if (data.api_status == 200)
                    {
                        if (data.users.Length > 0)
                        {
                            //Classes.UserContactsList = new ObservableCollection<Classes.UserContacts.User>(data.users);

                            if (is_Friend)
                            {
                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                //Insert Or Replace To Database
                                dbDatabase.Insert_Or_Replace_MyContactTable(new ObservableCollection<Classes.UserContacts.User>(data.users));
                                dbDatabase.Dispose();

                                is_Friend = false;
                            }
                        }

                        return (200, data);
                    }
                    else
                    {
                        var error = JsonConvert.DeserializeObject<Error_Object>(json);
                        return (404, error);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error : " + e.Message);
                Crashes.TrackError(e);
                return (400, e.Message);
            }
        }
         
        public static async Task<ObservableCollection<GifGiphyClass.Datum>> Search_Gifs_Web(string search_key, string offset)
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    return null;
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(Current.URLS.UrlInstance.API_Get_Search_Gifs + search_key);
                        response.EnsureSuccessStatusCode();
                        string json = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<GifGiphyClass.RootObject>(json);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            if (data.meta.status == 200)
                            {
                                return new ObservableCollection<GifGiphyClass.Datum>(data.data);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        }

        public static async Task<List<Classes.Geocoding.Result>> GetNameLocation(string address)
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    return null;
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}";
                        var response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        string json = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<Classes.Geocoding.GeoClass>(json);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            if (data.status == "OK")
                            {
                                return new List<Classes.Geocoding.Result>(data.results);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        }

        public static async Task<bool> AddNewPost_Async(string postId, string postPage, string textContent, string privacy, string feelingtype, string feeling, string loction, ObservableCollection<Attachments> postAttachments)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var multi = new MultipartFormDataContent(Guid.NewGuid().ToString())
                    {
                        { new StringContent(textContent), $"\"postText\"" },
                        { new StringContent(privacy), $"\"postPrivacy\"" },
                        { new StringContent(UserDetails.access_token), $"\"s\""},
                        { new StringContent(UserDetails.User_id), $"\"user_id\"" },
                    };

                    if (!string.IsNullOrEmpty(feelingtype))
                        multi.Add(new StringContent(feelingtype), $"\"feeling_type\"");

                    if (!string.IsNullOrEmpty(feeling))
                        multi.Add(new StringContent(feeling), $"\"feeling\"");

                    if (!string.IsNullOrEmpty(loction))
                        multi.Add(new StringContent(loction), $"\"postMap\"");

                    if (postPage == "SocialGroup")
                        multi.Add(new StringContent(postId), $"\"group_id\"");
                    else if (postPage == "SocialPage")
                        multi.Add(new StringContent(postId), $"\"page_id\"");
                    else if (postPage == "SocialEvent")
                        multi.Add(new StringContent(postId), $"\"event_id\"");

                    if (postAttachments.Count > 0)
                    {
                        foreach (var attachment in postAttachments)
                        {
                            FileStream fs = File.OpenRead(attachment.FileUrl);
                            StreamContent scontent = new StreamContent(fs);
                            scontent.Headers.ContentType = new MediaTypeHeaderValue(MimeTypeMap.GetMimeType(attachment.FileUrl?.Split('.').LastOrDefault()));
                            scontent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                Name = attachment.TypeAttachment,
                                FileName = attachment.FileUrl?.Split('\\').LastOrDefault()?.Split('/').LastOrDefault()
                            };

                            multi.Add(scontent);
                        }
                    }

                    HttpResponseMessage response = await client.PostAsync(API_Add_new_post, multi).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["api_status"].ToString();
                    if (apiStatus == "200")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return false;
            }
        }

        public static async Task GetStory_Api()
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    (int Api_status, var Respond) = await Client.Story.Get_Stories();
                    if (Api_status == 200)
                    {
                        if (Respond is Get_Stories_Object result)
                        {
                            if (result.stories.Length > 0)
                            {
                                //StoryAdapter.mStorylList = new ObservableCollection<Get_Stories_Object.Story>(result.stories);
                                //StoryAdapter.BindEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public static async Task Get_MyProfileData_Api(Activity context)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    (int api_status, var respond) = await Client.Global.Get_User_Data(new Settings(), UserDetails.User_id);
                    if (api_status == 200)
                    {
                        if (respond is Get_User_Data_Object result)
                        {
                            context.RunOnUiThread(() =>
                            {
                                SqLiteDatabase dbDatabase = new SqLiteDatabase();

                                // user_data
                                if (result.user_data != null)
                                {
                                    //Insert Or Update All data user_data
                                    dbDatabase.Insert_Or_Update_To_MyProfileTable(result.user_data);

                                    if (result.joined_groups.Length > 0)
                                    {
                                        //Insert Or Update All data Groups
                                        dbDatabase.InsertOrReplace_ManageGroupsTable(new ObservableCollection<Get_User_Data_Object.Joined_Groups>(result.joined_groups));
                                    }

                                    if (result.followers.Length > 0)
                                    {
                                        //Insert Or Update All data Groups
                                        dbDatabase.Insert_Or_Replace_MyFollowersTable(new ObservableCollection<Get_User_Data_Object.Followers>(result.followers));
                                    }

                                    if (result.following.Length > 0)
                                    {
                                        var list = result.following.Select(user => new Classes.UserContacts.User()
                                        {
                                            user_id = user.user_id,
                                            username = user.username,
                                            email = user.email,
                                            first_name = user.first_name,
                                            last_name = user.last_name,
                                            avatar = user.avatar,
                                            cover = user.cover,
                                            relationship_id = user.relationship_id,
                                            //lastseen_time_text = user.lastseen_time_text,
                                            address = user.address,
                                            working = user.working,
                                            working_link = user.working_link,
                                            about = user.about,
                                            school = user.school,
                                            gender = user.gender,
                                            birthday = user.birthday,
                                            website = user.website,
                                            facebook = user.facebook,
                                            google = user.google,
                                            twitter = user.twitter,
                                            linkedin = user.linkedin,
                                            youtube = user.youtube,
                                            vk = user.vk,
                                            instagram = user.instagram,
                                            language = user.language,
                                            ip_address = user.ip_address,
                                            follow_privacy = user.follow_privacy,
                                            friend_privacy = user.friend_privacy,
                                            post_privacy = user.post_privacy,
                                            message_privacy = user.message_privacy,
                                            confirm_followers = user.confirm_followers,
                                            show_activities_privacy = user.show_activities_privacy,
                                            birth_privacy = user.birth_privacy,
                                            visit_privacy = user.visit_privacy,
                                            lastseen = user.lastseen,
                                            showlastseen = user.showlastseen,
                                            e_sentme_msg = user.e_sentme_msg,
                                            e_last_notif = user.e_last_notif,
                                            status = user.status,
                                            active = user.active,
                                            admin = user.admin,
                                            registered = user.registered,
                                            phone_number = user.phone_number,
                                            is_pro = user.is_pro,
                                            pro_type = user.pro_type,
                                            joined = user.joined,
                                            timezone = user.timezone,
                                            referrer = user.referrer,
                                            balance = user.balance,
                                            paypal_email = user.paypal_email,
                                            notifications_sound = user.notifications_sound,
                                            order_posts_by = user.order_posts_by,
                                            social_login = user.social_login,
                                            device_id = user.device_id,
                                            web_device_id = user.web_device_id,
                                            wallet = user.wallet,
                                            lat = user.lat,
                                            lng = user.lng,
                                            last_location_update = user.last_location_update,
                                            share_my_location = user.share_my_location,
                                            url = user.url,
                                            name = user.name,
                                            lastseen_unix_time = user.lastseen_unix_time,
                                            //user_platform = user.user_platform,
                                            details = new Details()
                                            {
                                                post_count = user.details.post_count,
                                                album_count = user.details.album_count,
                                                following_count = user.details.following_count,
                                                followers_count = user.details.followers_count,
                                                groups_count = user.details.groups_count,
                                                likes_count = user.details.likes_count,
                                            }
                                        }).ToList();

                                        //Insert Or Update All data Groups
                                        dbDatabase.Insert_Or_Replace_MyContactTable(new ObservableCollection<Classes.UserContacts.User>(list));
                                    }

                                    if (result.liked_pages.Length > 0)
                                    {
                                        //Insert Or Update All data Pages
                                        dbDatabase.InsertOrReplace_ManagePagesTable(new ObservableCollection<Get_User_Data_Object.Liked_Pages>(result.liked_pages));
                                    }

                                    dbDatabase.Dispose();
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
              await Get_MyProfileData_Api(context).ConfigureAwait(false);
            }
        }
        
        public static bool RunLogout = false;
 
        public static async void Logout(Activity context)
        {
            try
            {
                if (RunLogout == false)
                {
                    RunLogout = true;
                    
                    await RemoveData("Logout");
                     
                    context.RunOnUiThread(async () =>
                    { 
                        IMethods.IPath.DeleteAll_MyFolderDisk();

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.ClearAll();
                        dbDatabase.DropAll();

                        Classes.ClearAllList();

                        Java.Lang.Runtime.GetRuntime().RunFinalization();
                        Java.Lang.Runtime.GetRuntime().Gc();
                        TrimCache(context);
                         
                        var ss = await dbDatabase.CheckTablesStatus();
                        dbDatabase.Dispose();

                        Intent intent = new Intent(context, typeof(First_Activity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        context.StartActivity(intent);
                        context.FinishAffinity();

                        ((Tabbed_Main_Activity)context).JobRescheduble.StopJob();
                    });

                    WowTime_Main_Settings.Shared_Data.Edit().Clear().Commit();

                    RunLogout = false;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public static void TrimCache(Activity context)
        {
            try
            {
                Java.IO.File dir = context.CacheDir;
                if (dir != null && dir.IsDirectory)
                {
                    DeleteDir(dir);
                }

                context.DeleteDatabase("WowonderSocial.db");
                context.DeleteDatabase(SqLiteDatabase.PathCombine);
            }
            catch (Exception e)
            {
                // TODO: handle exception
                Crashes.TrackError(e);
            }
        }

        public static bool DeleteDir(Java.IO.File dir)
        {
            try
            {
                if (dir != null && dir.IsDirectory)
                {
                    string[] children = dir.List();
                    foreach (string child in children)
                    {
                        bool success = DeleteDir(new Java.IO.File(dir, child));
                        if (!success)
                        {
                            return false;
                        }
                    }
                }

                // The directory is now empty so delete it
                return dir.Delete();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return false;
            }
        }

        public static async Task RemoveData(string type)
        {
            try
            {
                if (type == "Logout")
                {
                    if (IMethods.CheckConnectivity())
                    {
                        var data = await Client.Global.Get_Delete_Token(); 
                    }
                }
                else if (type == "Delete")
                {
                    IMethods.IPath.DeleteAll_MyFolder();

                    if (IMethods.CheckConnectivity())
                    {
                        var data = await Client.Global.Delete_User(UserDetails.Password);
                    }
                }
                 
                try
                {
                    if (Login_Activity.mGoogleApiClient != null)
                    {
                        await Auth.GoogleSignInApi.SignOut(Login_Activity.mGoogleApiClient);
                    }
                }
                catch (Exception exception)
                {
                    Crashes.TrackError(exception);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public async Task<string> GetWebsiteContent(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        using (HttpContent content = response.Content)
                        {
                            string result = await content.ReadAsStringAsync();

                            return result;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        } 
    }
}