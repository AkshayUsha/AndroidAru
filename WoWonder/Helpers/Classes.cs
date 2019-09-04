using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FFImageLoading.Work;
using Microsoft.AppCenter.Crashes;
using WoWonder.SQLite;
using WoWonder_API.Classes.Event;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Movies;
using WoWonder_API.Classes.Product;
using WoWonder_API.Classes.Story;
using WoWonder_API.Classes.User;


namespace WoWonder.Helpers
{
    public class UserDetails
    {
        //############# DONT'T MODIFY HERE #############
        //Auto Session bindable objects 
        //*********************************************************
        public static string access_token = "";
        public static string User_id = "";
        public static string Username = "";
        public static string Full_name = "";
        public static string Password = "";
        public static string Email = "";
        public static string Cookie = "";
        public static string Status = "";
        public static string avatar = "";
        public static string cover = "";
        public static string Device_ID = "";
        public static string Lang = "";
        public static string Lat = "";
        public static string Lng = "";
        public static bool NotificationPopup { get; set; } = true;

        public static Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        public string Time = unixTimestamp.ToString();
    }

    public class Classes
    {
        //############# DONT'T MODIFY HERE #############
        //List Items Declaration 
        //*********************************************************
        public static ObservableCollection<DataTables.LoginTB> DataUserLoginList = new ObservableCollection<DataTables.LoginTB>();
       
        public static ObservableCollection<Get_User_Data_Object.User_Data> MyProfileList = new ObservableCollection<Get_User_Data_Object.User_Data>();

        public static ObservableCollection<Get_User_Albums_Object.Album> ListChachedData_Album = new ObservableCollection<Get_User_Albums_Object.Album>();
        public static ObservableCollection<Get_Users_Articles_Object.Article> ListChachedData_Article = new ObservableCollection<Get_Users_Articles_Object.Article>();
        public static ObservableCollection<Get_Events_Object.Event> ListChachedData_Event = new ObservableCollection<Get_Events_Object.Event>();
        public static ObservableCollection<Get_MyEvent_object.My_Events> ListChachedData_MyEvents = new ObservableCollection<Get_MyEvent_object.My_Events>();
        public static ObservableCollection<Get_Products_Object.Product> ListChachedData_Product = new ObservableCollection<Get_Products_Object.Product>();
        public static ObservableCollection<Get_Products_Object.Product> ListChachedData_MyProduct = new ObservableCollection<Get_Products_Object.Product>();
        public static ObservableCollection<Get_Movies_Object.Movie> ListChachedData_Movie = new ObservableCollection<Get_Movies_Object.Movie>();
        public static ObservableCollection<Get_Nearby_Users_Object.Nearby_Users> ListChachedData_Nearby = new ObservableCollection<Get_Nearby_Users_Object.Nearby_Users>();
        public static Dictionary<List<Get_Stories_Object.Story>, string> StoryList = new Dictionary<List<Get_Stories_Object.Story>, string>();


        public static void ClearAllList()
        {
            try
            {
                DataUserLoginList.Clear();
                MyProfileList.Clear();
                ListChachedData_Album.Clear();
                ListChachedData_Article.Clear();
                ListChachedData_Event.Clear();
                ListChachedData_MyEvents.Clear();
                ListChachedData_Product.Clear();
                ListChachedData_MyProduct.Clear();
                ListChachedData_Movie.Clear();
                ListChachedData_Nearby.Clear();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
        
        public static void AddRange<T>(ObservableCollection<T> collection, IEnumerable<T> items)
        {
            try
            {
                items.ToList().ForEach(collection.Add);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
         
        public static List<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            var list = new List<List<T>>();

            for (int i = 0; i < locations.Count; i += nSize)
            {
                list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }

            return list;
        }

        public static IEnumerable<T> TakeLast<T>(IEnumerable<T> source, int n)
        {
            var enumerable = source as T[] ?? source.ToArray();

            return enumerable.Skip(Math.Max(0, enumerable.Count() - n));
        }


        //Classes 
        //*********************************************************

        public class UserContacts
        {
            public class Rootobject
            {
                public int api_status { get; set; }
                public string api_text { get; set; }
                public string api_version { get; set; }
                public string theme_url { get; set; }
                public User[] users { get; set; }
                // public object[] online { get; set; }
            }

            public class User
            {
                public string user_id { get; set; }
                public string username { get; set; }
                public string email { get; set; }
                public string first_name { get; set; }
                public string last_name { get; set; }
                public string avatar { get; set; }
                public string cover { get; set; }
                public string relationship_id { get; set; }
                public string address { get; set; }
                public string working { get; set; }
                public string working_link { get; set; }
                public string about { get; set; }
                public string school { get; set; }
                public string gender { get; set; }
                public string birthday { get; set; }
                public string website { get; set; }
                public string facebook { get; set; }
                public string google { get; set; }
                public string twitter { get; set; }
                public string linkedin { get; set; }
                public string youtube { get; set; }
                public string vk { get; set; }
                public string instagram { get; set; }
                public string language { get; set; }
                public string ip_address { get; set; }
                public string follow_privacy { get; set; }
                public string friend_privacy { get; set; }
                public string post_privacy { get; set; }
                public string message_privacy { get; set; }
                public string confirm_followers { get; set; }
                public string show_activities_privacy { get; set; }
                public string birth_privacy { get; set; }
                public string visit_privacy { get; set; }
                public string verified { get; set; }
                public string lastseen { get; set; }
                public string showlastseen { get; set; }
                public string e_sentme_msg { get; set; }
                public string e_last_notif { get; set; }
                public string status { get; set; }
                public string active { get; set; }
                public string admin { get; set; }
                public string registered { get; set; }
                public string phone_number { get; set; }
                public string is_pro { get; set; }
                public string pro_type { get; set; }
                public string joined { get; set; }
                public string timezone { get; set; }
                public string referrer { get; set; }
                public string balance { get; set; }
                public string paypal_email { get; set; }
                public string notifications_sound { get; set; }
                public string order_posts_by { get; set; }
                public string social_login { get; set; }
                public string device_id { get; set; }
                public string web_device_id { get; set; }
                public string wallet { get; set; }
                public string lat { get; set; }
                public string lng { get; set; }
                public string last_location_update { get; set; }
                public string share_my_location { get; set; }
                public string last_data_update { get; set; }
                public Details details { get; set; }
                public string sidebar_data { get; set; }
                public string last_avatar_mod { get; set; }
                public string last_cover_mod { get; set; }
                public string avatar_full { get; set; }
                public string url { get; set; }
                public string name { get; set; }
                // public object following_data { get; set; }
                // public string[] followers_data { get; set; }
                // public object likes_data { get; set; }
                // public object groups_data { get; set; }
                // public string album_data { get; set; }
                public string lastseen_unix_time { get; set; }
                public string lastseen_status { get; set; }
                public string family_member { get; set; }
                public string lastseen_time_text { get; set; }
                public string user_platform { get; set; }
                public string is_following { get; set; }
            }
        }

        public class UserChat : UserContacts.User
        {

        }

        public class UploadFile
        {
            public UploadFile()
            {
                ContentType = "application/octet-stream";
            }
            public string Name { get; set; }
            public string Filename { get; set; }
            public string ContentType { get; set; }
            public List<Stream> Stream { get; set; }
        }

        public class UploadFileImage
        {
            public UploadFileImage()
            {
                ContentType = "application/Mime-stream";
            }
            public string Name { get; set; }
            public string Filename { get; set; }
            public string ContentType { get; set; }
            public Stream Stream { get; set; }
        }

        public class Storyitems
        {
            public string Label { get; set; }
            public ImageSource Image { get; set; }
            public Stream ImageStream { get; set; }
            public string ImageFullPath { get; set; }
        }
         
        public class SerachFilterItems
        {
            public string PrivacyLabel { get; set; }
            public string value { get; set; }
            public string Checkicon { get; set; }
            public string CheckiconColor { get; set; }
        }

        public class PostType
        {
            public int ID { get; set; }
            public string TypeText { get; set; }
            public int Image { get; set; }
            public string ImageColor { get; set; }
        }

        public class Gifgiphy
        {
            public string G_id { get; set; }
            public string G_fixed_height_small_width { get; set; }
            public string G_fixed_height_small_height { get; set; }
            public string G_fixed_height_small_url { get; set; }
            public string G_fixed_height_small_mp4 { get; set; }
            public string G_fixed_height_small_webp { get; set; }
            public string G_original_url { get; set; } //sent

            //style
            public string G_Progressbar_Visibility { get; set; }

            public string G_Bar_load_gifs_Visibility { get; set; }
            public string G_btn_ExitGifs_remove { get; set; }
        }

        public class Catigories
        {
            public string Catigories_Id { get; set; }
            public string Catigories_Name { get; set; }
            public string Catigories_Color { get; set; }
        }


        public class Geocoding
        {
            public class AddressComponent
            {
                public string long_name { get; set; }
                public string short_name { get; set; }
                public List<string> types { get; set; }
            }

            public class Location
            {
                public double lat { get; set; }
                public double lng { get; set; }
            }

            public class Northeast
            {
                public double lat { get; set; }
                public double lng { get; set; }
            }

            public class Southwest
            {
                public double lat { get; set; }
                public double lng { get; set; }
            }

            public class Viewport
            {
                public Northeast northeast { get; set; }
                public Southwest southwest { get; set; }
            }

            public class Geometry
            {
                public Location location { get; set; }
                public string location_type { get; set; }
                public Viewport viewport { get; set; }
            }

            public class Result
            {
                public List<AddressComponent> address_components { get; set; }
                public string formatted_address { get; set; }
                public Geometry geometry { get; set; }
                public string place_id { get; set; }
                public List<string> types { get; set; }
            }

            public class GeoClass
            {
                public List<Result> results { get; set; }
                public string status { get; set; }
            }
        }
       
    }
}





