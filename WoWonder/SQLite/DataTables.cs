using SQLite;
using WoWonder_API.Classes.Global;


namespace WoWonder.SQLite
{
    public class DataTables
    {
        public class LoginTB
        {
            [PrimaryKey, AutoIncrement] public int AutoIDLogin { get; set; }

            public string UserID { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string access_token { get; set; }
            public string Cookie { get; set; }
            public string Email { get; set; }
            public string Status { get; set; }
            public string Lang { get; set; }
            public string Device_ID { get; set; }
        }

        public class SettingsTB : Settings_Object
        {
            [PrimaryKey, AutoIncrement] public int AutoIDSettings { get; set; }



        }

        public class CatigoriesTB
        {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }

            public string Catigories_Id { get; set; }
            public string Catigories_Name { get; set; }
        }


        public class MyContactsTB
        {
            [PrimaryKey, AutoIncrement] public int ID { get; set; }

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
            public string sidebar_data { get; set; }
            public string last_avatar_mod { get; set; }
            public string last_cover_mod { get; set; }
            public string avatar_full { get; set; }
            public string url { get; set; }
            public string name { get; set; }
            public string following_data { get; set; }
            public string followers_data { get; set; }
            public string likes_data { get; set; }
            public string groups_data { get; set; }
            public string album_data { get; set; }
            public string lastseen_unix_time { get; set; }
            public string lastseen_status { get; set; }
            public string family_member { get; set; }
            public string lastseen_time_text { get; set; }
            public string user_platform { get; set; }
            public string is_following { get; set; }

            //Details
            public string de_post_count { get; set; }
            public string de_album_count { get; set; }
            public string de_following_count { get; set; }
            public string de_followers_count { get; set; }
            public string de_groups_count { get; set; }
            public string de_likes_count { get; set; }
        }

        public class MyFollowersTB
        {
            [PrimaryKey, AutoIncrement] public int AutoIDMyFollowers { get; set; }

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
            public string sidebar_data { get; set; }
            public string last_avatar_mod { get; set; }
            public string last_cover_mod { get; set; }
            public string avatar_full { get; set; }
            public string url { get; set; }
            public string name { get; set; }
            public string following_data { get; set; }
            public string followers_data { get; set; }
            public string likes_data { get; set; }
            public string groups_data { get; set; }
            public string album_data { get; set; }
            public string lastseen_unix_time { get; set; }
            public string lastseen_status { get; set; }
            public string family_member { get; set; }
            public string lastseen_time_text { get; set; }
            public string user_platform { get; set; }
            public int is_following { get; set; }

            //Details
            public string de_post_count { get; set; }
            public string de_album_count { get; set; }
            public string de_following_count { get; set; }
            public string de_followers_count { get; set; }
            public string de_groups_count { get; set; }
            public string de_likes_count { get; set; }
        }

        public class BlockedUsersTB
        {
            [PrimaryKey, AutoIncrement] public int AutoIDBlockedUsers { get; set; }

            public string user_id { get; set; }
            public string username { get; set; }
            public string email { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string avatar { get; set; }
            public string cover { get; set; }
            public string background_image { get; set; }
            public string relationship_id { get; set; }
            public string address { get; set; }
            public string working { get; set; }
            public string working_link { get; set; }
            public string about { get; set; }
            public string school { get; set; }
            public string gender { get; set; }
            public string birthday { get; set; }
            public string country_id { get; set; }
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
            public string emailNotification { get; set; }
            public string e_liked { get; set; }
            public string e_wondered { get; set; }
            public string e_shared { get; set; }
            public string e_followed { get; set; }
            public string e_commented { get; set; }
            public string e_visited { get; set; }
            public string e_liked_page { get; set; }
            public string e_mentioned { get; set; }
            public string e_joined_group { get; set; }
            public string e_accepted { get; set; }
            public string e_profile_wall_post { get; set; }
            public string e_sentme_msg { get; set; }
            public string e_last_notif { get; set; }
            public string status { get; set; }
            public string active { get; set; }
            public string admin { get; set; }
            public string registered { get; set; }
            public string phone_number { get; set; }
            public string is_pro { get; set; }
            public string pro_type { get; set; }
            public string timezone { get; set; }
            public string referrer { get; set; }
            public string balance { get; set; }
            public string paypal_email { get; set; }
            public string notifications_sound { get; set; }
            public string order_posts_by { get; set; }
            public string device_id { get; set; }
            public string web_device_id { get; set; }
            public string wallet { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
            public string last_location_update { get; set; }
            public string share_my_location { get; set; }
            public string last_data_update { get; set; }
            public string last_avatar_mod { get; set; }
            public string last_cover_mod { get; set; }
            public string avatar_full { get; set; }
            public string url { get; set; }
            public string name { get; set; }
            public string lastseen_unix_time { get; set; }

            public string lastseen_status { get; set; }

            //Details
            public string de_post_count { get; set; }
            public string de_album_count { get; set; }
            public string de_following_count { get; set; }
            public string de_followers_count { get; set; }
            public string de_groups_count { get; set; }
            public string de_likes_count { get; set; }
        }

        public class MyProfileTB
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIDMyProfile { get; set; }

            public string user_id { get; set; }
            public string username { get; set; }
            public string email { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string avatar { get; set; }
            public string cover { get; set; }
            public string background_image { get; set; }
            public string relationship_id { get; set; }
            public string address { get; set; }
            public string working { get; set; }
            public string working_link { get; set; }
            public string about { get; set; }
            public string school { get; set; }
            public string gender { get; set; }
            public string birthday { get; set; }
            public string country_id { get; set; }
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
            public string emailNotification { get; set; }
            public string e_liked { get; set; }
            public string e_wondered { get; set; }
            public string e_shared { get; set; }
            public string e_followed { get; set; }
            public string e_commented { get; set; }
            public string e_visited { get; set; }
            public string e_liked_page { get; set; }
            public string e_mentioned { get; set; }
            public string e_joined_group { get; set; }
            public string e_accepted { get; set; }
            public string e_profile_wall_post { get; set; }
            public string e_sentme_msg { get; set; }
            public string e_last_notif { get; set; }
            public string status { get; set; }
            public string active { get; set; }
            public string admin { get; set; }
            public string registered { get; set; }
            public string phone_number { get; set; }
            public string is_pro { get; set; }
            public string pro_type { get; set; }
            public string timezone { get; set; }
            public string referrer { get; set; }
            public string balance { get; set; }
            public string paypal_email { get; set; }
            public string notifications_sound { get; set; }
            public string order_posts_by { get; set; }
            public string device_id { get; set; }
            public string web_device_id { get; set; }
            public string wallet { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
            public string last_location_update { get; set; }
            public string share_my_location { get; set; }
            public string last_data_update { get; set; }
            public string last_avatar_mod { get; set; }
            public string last_cover_mod { get; set; }
            public string avatar_full { get; set; }
            public string url { get; set; }
            public string name { get; set; }
            public string lastseen_unix_time { get; set; }
            public string lastseen_status { get; set; }
            public string is_following { get; set; }
            public string can_follow { get; set; }
            public string is_following_me { get; set; }
            public string gender_text { get; set; }
            public string lastseen_time_text { get; set; }

            public string is_blocked { get; set; }

            //Details
            public string de_post_count { get; set; }
            public string de_album_count { get; set; }
            public string de_following_count { get; set; }
            public string de_followers_count { get; set; }
            public string de_groups_count { get; set; }
            public string de_likes_count { get; set; }
        }

        public class GroupsTB
        {
            [PrimaryKey, AutoIncrement] public int AutoIDGroups { get; set; }

            public string Id { get; set; }
            public string UserId { get; set; }
            public string GroupName { get; set; }
            public string GroupTitle { get; set; }
            public string Avatar { get; set; }
            public string Cover { get; set; }
            public string About { get; set; }
            public string Category { get; set; }
            public string Privacy { get; set; }
            public string JoinPrivacy { get; set; }
            public string Active { get; set; }
            public string Registered { get; set; }
            public string GroupId { get; set; }
            public string Url { get; set; }
            public string Name { get; set; }
            public string CategoryId { get; set; }
            public string Type { get; set; }
            public string Username { get; set; }
        }

        public class PageTB
        {
            [PrimaryKey, AutoIncrement] public int AutoIDPage { get; set; }

            public string PageId { get; set; }
            public string UserId { get; set; }
            public string PageName { get; set; }
            public string PageTitle { get; set; }
            public string PageDescription { get; set; }
            public string Avatar { get; set; }
            public string Cover { get; set; }
            public string PageCategory { get; set; }
            public string Website { get; set; }
            public string Facebook { get; set; }
            public string Google { get; set; }
            public string Vk { get; set; }
            public string Twitter { get; set; }
            public string Linkedin { get; set; }
            public string Company { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string CallActionType { get; set; }
            public string CallActionTypeUrl { get; set; }
            public string BackgroundImage { get; set; }
            public string BackgroundImageStatus { get; set; }
            public string Instgram { get; set; }
            public string Youtube { get; set; }
            public string Verified { get; set; }
            public string Active { get; set; }
            public string Registered { get; set; }
            public string Boosted { get; set; }
            public string About { get; set; }
            public string Id { get; set; }
            public string Type { get; set; }
            public string Url { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public string IsPageOnwer { get; set; }
            public string Username { get; set; }
        }

        public class SearchFilterTB
        {
            [PrimaryKey, AutoIncrement] public int AutoIDSearchFilter { get; set; }

            public string UserId { get; set; }
            public int Gender { get; set; }
            public int ProfilePicture { get; set; }
            public int Status { get; set; }
        }

        public class WatchOfflineVideosTB
        {
            [PrimaryKey, AutoIncrement] public int AutoIDWatchOfflineVideos { get; set; }





            public string id { get; set; }

            public string name { get; set; }

            public string genre { get; set; }

            public string stars { get; set; }

            public string producer { get; set; }

            public string country { get; set; }

            public string release { get; set; }

            public string quality { get; set; }

            public string duration { get; set; }

            public string description { get; set; }
            public string cover { get; set; }

            public string source { get; set; }

            public string iframe { get; set; }
            public string video { get; set; }
            public string views { get; set; }

            public string url { get; set; }
            public string Video_Name { get; set; }
            public string Video_SavedPath { get; set; }
        }

        public class NearByFilterTB
        {

            [PrimaryKey, AutoIncrement] public int AutoIDNearByFilter { get; set; }




            public string UserId { get; set; }
            public int DistanceValue { get; set; }
            public int Gender { get; set; }
            public int Status { get; set; }

        }

        public class NotificationTB
        {
            [PrimaryKey, AutoIncrement] public int AutoIDNotification { get; set; }




            public string id { get; set; }
            public string notifier_id { get; set; }
            public string recipient_id { get; set; }
            public string post_id { get; set; }
            public string page_id { get; set; }
            public string group_id { get; set; }
            public string event_id { get; set; }
            public string thread_id { get; set; }
            public string seen_pop { get; set; }
            public string type { get; set; }
            public string type2 { get; set; }
            public string text { get; set; }
            public string url { get; set; }
            public string full_link { get; set; }
            public string seen { get; set; }
            public string sent_push { get; set; }
            public string time { get; set; }
            public string ajax_url { get; set; }
            public string type_text { get; set; }
            public string icon { get; set; }
            public string time_text_string { get; set; }
            public string time_text { get; set; }

            //Notifier
            public string n_user_id { get; set; }
            public string n_username { get; set; }
            public string n_email { get; set; }
            public string n_first_name { get; set; }
            public string n_last_name { get; set; }
            public string n_avatar { get; set; }
            public string n_cover { get; set; }
            public string n_background_image { get; set; }
            public string n_relationship_id { get; set; }
            public string n_address { get; set; }
            public string n_working { get; set; }
            public string n_working_link { get; set; }
            public string n_about { get; set; }
            public string n_school { get; set; }
            public string n_gender { get; set; }
            public string n_birthday { get; set; }
            public string n_country_id { get; set; }
            public string n_website { get; set; }
            public string n_facebook { get; set; }
            public string n_google { get; set; }
            public string n_twitter { get; set; }
            public string n_linkedin { get; set; }
            public string n_youtube { get; set; }
            public string n_vk { get; set; }
            public string n_instagram { get; set; }
            public string n_language { get; set; }
            public string n_ip_address { get; set; }
            public string n_follow_privacy { get; set; }
            public string n_friend_privacy { get; set; }
            public string n_post_privacy { get; set; }
            public string n_message_privacy { get; set; }
            public string n_confirm_followers { get; set; }
            public string n_show_activities_privacy { get; set; }
            public string n_birth_privacy { get; set; }
            public string n_visit_privacy { get; set; }
            public string n_verified { get; set; }
            public string n_lastseen { get; set; }
            public string n_emailNotification { get; set; }
            public string n_e_liked { get; set; }
            public string n_e_wondered { get; set; }
            public string n_e_shared { get; set; }
            public string n_e_followed { get; set; }
            public string n_e_commented { get; set; }
            public string n_e_visited { get; set; }
            public string n_e_liked_page { get; set; }
            public string n_e_mentioned { get; set; }
            public string n_e_joined_group { get; set; }
            public string n_e_accepted { get; set; }
            public string n_e_profile_wall_post { get; set; }
            public string n_e_sentme_msg { get; set; }
            public string n_e_last_notif { get; set; }
            public string n_status { get; set; }
            public string n_active { get; set; }
            public string n_admin { get; set; }
            public string n_registered { get; set; }
            public string n_phone_number { get; set; }
            public string n_is_pro { get; set; }
            public string n_pro_type { get; set; }
            public string n_timezone { get; set; }
            public string n_referrer { get; set; }
            public string n_balance { get; set; }
            public string n_paypal_email { get; set; }
            public string n_notifications_sound { get; set; }
            public string n_order_posts_by { get; set; }
            public string n_device_id { get; set; }
            public string n_web_device_id { get; set; }
            public string n_wallet { get; set; }
            public string n_lat { get; set; }
            public string n_lng { get; set; }
            public string n_last_location_update { get; set; }
            public string n_share_my_location { get; set; }

            public string n_last_data_update { get; set; }

            //Details      
            public string n_de_post_count { get; set; }
            public string n_de_album_count { get; set; }
            public string n_de_following_count { get; set; }
            public string n_de_followers_count { get; set; }
            public string n_de_groups_count { get; set; }
            public string n_de_likes_count { get; set; }
            public string n_last_avatar_mod { get; set; }
            public string n_last_cover_mod { get; set; }
            public string n_avatar_full { get; set; }
            public string n_url { get; set; }
            public string n_name { get; set; }
            public string n_lastseen_unix_time { get; set; }
            public string n_lastseen_status { get; set; }
        }
    }
}