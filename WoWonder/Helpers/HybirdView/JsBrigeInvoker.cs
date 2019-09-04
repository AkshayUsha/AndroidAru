//###############################################################
// Author >> Elin Doughouz
// Copyright (c) WoWonder 15/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using Android.App;
using Android.Content;
using Android.Webkit;
using Java.Interop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Android.Support.V4.Content;
using Android.Widget;
using AndroidHUD;
using Microsoft.AppCenter.Crashes;
using WoWonder.Activities.Communities.Groups;
using WoWonder.Activities.Communities.Pages;
using WoWonder.Activities.EditPost;
using WoWonder.Activities.MyProfile;
using WoWonder.Activities.PostData;
using WoWonder.Activities.UserProfile;
using WoWonder.Activities.UsersPages;


namespace WoWonder.Helpers.HybirdView
{
    public class JsBrigeInvoker : Java.Lang.Object, IDisposable
    { 
        public Activity ApplicationContext { get; set; }
        public string IdPage;

        public JsBrigeInvoker(Activity context, string idpage)
        {
            try
            {
                IdPage = idpage;
                ApplicationContext = context;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
        internal static string GenerateFunctionScript(string name)
        {
            return $"function {name}(str){{csharp(\"{{'action':'{name}','data':'\"+window.btoa(str)+\"'}}\");}}";
        }

        /// <summary>
        /// A delegate which takes valid javascript and returns the response from it, if the response is a string.
        /// </summary>
        /// <param name="js">The valid JS to inject</param>
        /// <returns>Any string response from the DOM or string.Empty</returns>
        public delegate Task<string> JavascriptInjectionRequestDelegate(string js);

      
       
        public delegate void JavascriptInjectionRequest(string data);

        internal event JavascriptInjectionRequestDelegate OnJavascriptInjectionRequest;


        [JavascriptInterface]
        [Export("invokeAction")]
        public virtual void InvokeAction(string eventobj)
        {
            try
            {
                if (!string.IsNullOrEmpty(eventobj))
                {
                    //OnJavascriptInjectionRequest?.Invoke(eventobj);

                    if (eventobj.Contains("type"))
                    {
                        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(eventobj);
                        string type = data["type"].ToString();

                        if (type == "user")
                        {
                            OpenUserProfile(data["profile_id"].ToString());
                        }
                        else if (type == "lightbox")
                        {
                            OpenImageLihtBox(data["image_url"].ToString());
                        }
                        else if (type == "mention")
                        {
                            OpenUserProfile(data["user_id"].ToString());
                        }
                        else if (type == "hashtag")
                        {
                            Openhashtag(data["tag"].ToString());
                        }
                        else if (type == "url")
                        {
                            OpenUrl(data["link"].ToString());
                        }
                        else if (type == "page")
                        {
                            OpenLikePage(data["profile_id"].ToString());
                        }
                        else if (type == "group")
                        {
                            OpenGroupPage(data["profile_id"].ToString());
                        }
                        else if (type == "post_wonders" || type == "post_likes")
                        {
                            OpenPostLikecount(data["post_id"].ToString(), type);
                        }
                        else if (type == "edit_post")
                        {
                            OpenEditPost(data["post_id"].ToString(), data["edit_text"].ToString());
                        }
                        else
                        {
                            OnJavascriptInjectionRequest?.Invoke(eventobj);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OpenUserProfile(string userid)
        {
            try
            {
                if (userid == IdPage)
                    return;

                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                Classes.UserContacts.User data = dbDatabase.Get_DataOneUser(userid);
                dbDatabase.Dispose();

                ApplicationContext.RunOnUiThread(() =>
                {
                    if (UserDetails.User_id == userid)
                    {
                        Intent Int = new Intent(ApplicationContext, typeof(MyProfile_Activity));
                        Int.PutExtra("UserId", userid);
                        ApplicationContext.StartActivity(Int);
                    }
                    else
                    {
                        Intent Int = new Intent(ApplicationContext, typeof(User_Profile_Activity));
                        Int.PutExtra("UserId", userid);
                        Int.PutExtra("UserType", "MyContacts");
                        if (data != null)
                            Int.PutExtra("UserItem", JsonConvert.SerializeObject(data));
                        
                        ApplicationContext.StartActivity(Int);
                    }
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void Openhashtag(string tag)
        {
            try
            {
                if (string.IsNullOrEmpty(tag))
                    return;

                ApplicationContext.RunOnUiThread(() =>
                {
                    var intent = new Intent(ApplicationContext, typeof(HyberdPostViewer_Activity));
                    intent.PutExtra("Type", "Hashtag");
                    intent.PutExtra("Id", tag);
                    intent.PutExtra("Title", tag);
                    ApplicationContext.StartActivity(intent);
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OpenUrl(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return;

                ApplicationContext.RunOnUiThread(() =>
                {
                    IMethods.IApp.OpenbrowserUrl(ApplicationContext, url);
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OpenLikePage(string pageid)
        {
            try
            {
                if (string.IsNullOrEmpty(pageid))
                    return;

                ApplicationContext.RunOnUiThread(() =>
                {
                    Intent Int = new Intent(ApplicationContext, typeof(Page_ProfileActivity));
                    Int.PutExtra("WebUserPages_ID", pageid);
                    Int.PutExtra("PagesType", "Liked_WebUserPages");
                    ApplicationContext.StartActivity(Int);
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OpenGroupPage(string groupid)
        {
            try
            {
                if (string.IsNullOrEmpty(groupid))
                    return;

                ApplicationContext.RunOnUiThread(() =>
                {
                    Intent Int = new Intent(ApplicationContext, typeof(Group_Profile_Activity));
                    Int.PutExtra("WebUserGroups_ID", groupid);
                    Int.PutExtra("GroupsType", "Joined_WebUserGroups");
                    ApplicationContext.StartActivity(Int);
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OpenPostLikecount(string postid ,string posttype)
        {
            try
            {
                if (string.IsNullOrEmpty(postid))
                    return;

                ApplicationContext.RunOnUiThread(() =>
                {
                    Intent Int = new Intent(ApplicationContext, typeof(PostData_Activity));
                    Int.PutExtra("PostId", postid);
                    Int.PutExtra("PostType", posttype);
                    ApplicationContext.StartActivity(Int);
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OpenEditPost(string postid, string edittext)
        {
            try
            {
                if (string.IsNullOrEmpty(postid))
                    return;

                ApplicationContext.RunOnUiThread(() =>
                {
                    Intent Int = new Intent(ApplicationContext, typeof(EditPost_Activity));
                    Int.PutExtra("PostId", postid);
                    Int.PutExtra("PostText", edittext);
                    ApplicationContext.StartActivityForResult(Int, 3500); 
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OpenImageLihtBox(string imageurl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageurl))
                    return;
                 
                ApplicationContext.RunOnUiThread(() =>
                { 
                    var fileName = imageurl.Split('/').Last();
 
                    var getImage = IMethods.MultiMedia.GetMediaFrom_Gallery(IMethods.IPath.FolderDcimPost, fileName);
                    if (getImage != "File Dont Exists")
                    {
                        Java.IO.File file2 = new Java.IO.File(getImage);
                        var photoURI = FileProvider.GetUriForFile(ApplicationContext, ApplicationContext.PackageName + ".fileprovider", file2);

                        Intent intent = new Intent(Intent.ActionPick);
                        intent.SetAction(Intent.ActionView);
                        intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                        intent.SetDataAndType(photoURI, "image/*");
                        ApplicationContext.StartActivity(intent);
                    }
                    else 
                    {
                        string Filename = imageurl.Split('/').Last();
                        string filePath = Path.Combine(IMethods.IPath.FolderDcimPost);
                        string MediaFile = filePath + "/" + Filename;

                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);

                        if (!File.Exists(MediaFile))
                        {
                            WebClient WebClient = new WebClient();
                            AndHUD.Shared.Show(ApplicationContext, ApplicationContext.GetText(Resource.String.Lbl_Loading));

                            WebClient.DownloadDataAsync(new System.Uri(imageurl));
                            WebClient.DownloadProgressChanged += (sender, args) =>
                            {
                                var progress = args.ProgressPercentage;
                                // holder.loadingProgressview.Progress = progress;
                                //Show a progress
                                AndHUD.Shared.Show(ApplicationContext, ApplicationContext.GetText(Resource.String.Lbl_Loading));
                            };
                            WebClient.DownloadDataCompleted += (s, e) =>
                            {
                                try
                                {
                                    File.WriteAllBytes(MediaFile, e.Result);

                                    var get_Image = IMethods.MultiMedia.GetMediaFrom_Gallery(IMethods.IPath.FolderDcimPost, fileName);
                                    if (get_Image != "File Dont Exists")
                                    {
                                        Java.IO.File file2 = new Java.IO.File(get_Image);

                                        Android.Net.Uri photoURI = FileProvider.GetUriForFile(ApplicationContext, ApplicationContext.PackageName + ".fileprovider", file2);

                                        Intent intent = new Intent(Intent.ActionPick);
                                        intent.SetAction(Intent.ActionView);
                                        intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                        intent.SetDataAndType(photoURI, "image/*");
                                        ApplicationContext.StartActivity(intent);
                                    }
                                    else
                                    {
                                        Toast.MakeText(ApplicationContext, ApplicationContext.GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long).Show();
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Crashes.TrackError(exception);
                                }

                                var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                                mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(MediaFile)));
                                ApplicationContext.SendBroadcast(mediaScanIntent);


                                AndHUD.Shared.Dismiss(ApplicationContext);

                            };
                        }                       
                    }
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public static async Task Post_Manager(string type, string postid)
        {
            try
            {
                var action = type == "edit_post" ? "edit" : "delete";

                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("post_id", postid),
                        new KeyValuePair<string, string>("s",UserDetails.access_token),
                        new KeyValuePair<string, string>("action", action),
                    });

                    var response = await client.PostAsync(WoWonder_API.Client.WebsiteUrl + "/app_api.php?application=phone&type=post_manager", formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["api_status"].ToString();
                    if (apiStatus == "200")
                    {
                        if (type == "edit_post")
                        {
                            action = "edit";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
    }
}