using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using SQLite;
using SQLiteNetExtensions.Extensions;
using WoWonder;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Group;
using WoWonder_API.Classes.Movies;
using WoWonder_API.Classes.User;
using Exception = System.Exception;


public class SqLiteDatabase : IDisposable
{
    //############# DON'T MODIFY HERE #############
    public static string Folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    public static string PathCombine = Path.Combine(Folder, "WowonderSocial.db");
    public SQLiteConnection Connection;

    //Open Connection in Database
    //*********************************************************

    #region Connection

    public void OpenConnection()
    {
        try
        {
            Connection = new SQLiteConnection(PathCombine);
            string path = Connection.DatabasePath;
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }
     
    public async Task<bool> CheckTablesStatus()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                OpenConnection();

                Connection?.CreateTable<DataTables.LoginTB>();
                Connection?.CreateTable<DataTables.MyContactsTB>();
                Connection?.CreateTable<DataTables.MyFollowersTB>();
                Connection?.CreateTable<DataTables.BlockedUsersTB>();
                Connection?.CreateTable<DataTables.MyProfileTB>();

                Connection?.CreateTable<DataTables.GroupsTB>();
                Connection?.CreateTable<DataTables.PageTB>();
                Connection?.CreateTable<DataTables.CatigoriesTB>();
               
                Connection?.CreateTable<DataTables.SearchFilterTB>();
                Connection?.CreateTable<DataTables.NearByFilterTB>();
                Connection?.CreateTable<DataTables.WatchOfflineVideosTB>();
                Connection?.Dispose();
                Connection?.Close();

                return true;
            }
          
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            //throw new Exception(e.Message);
            return false;
        } 
    }

    //Close Connection in Database
    public void Dispose()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                Connection?.Dispose();
                Connection?.Close();
                GC.SuppressFinalize(this);
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
           // throw new Exception(e.Message);
        }
    }

    public void DeleteDatabase()
    {
        try
        {
            DirectoryInfo di = new DirectoryInfo(Folder);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            //throw new Exception(e.Message);
        }
    }


    public void ClearAll()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                Connection.DeleteAll<DataTables.LoginTB>();
                Connection.DeleteAll<DataTables.MyContactsTB>();
                Connection.DeleteAll<DataTables.MyFollowersTB>();
                Connection.DeleteAll<DataTables.BlockedUsersTB>();
                Connection.DeleteAll<DataTables.GroupsTB>();
                Connection.DeleteAll<DataTables.PageTB>();
                Connection.DeleteAll<DataTables.CatigoriesTB>();
                Connection.DeleteAll<DataTables.MyProfileTB>();
                Connection.DeleteAll<DataTables.SearchFilterTB>();
                Connection.DeleteAll<DataTables.NearByFilterTB>();
                Connection.DeleteAll<DataTables.WatchOfflineVideosTB>();
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
            //throw new Exception(exception.Message);
        }
    }

    //Delete table 
    public void DropAll()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                Connection.DropTable<DataTables.LoginTB>();
                Connection.DropTable<DataTables.MyContactsTB>();
                Connection.DropTable<DataTables.MyFollowersTB>();
                Connection.DropTable<DataTables.BlockedUsersTB>();
                Connection.DropTable<DataTables.GroupsTB>();
                Connection.DropTable<DataTables.PageTB>();
                Connection.DropTable<DataTables.CatigoriesTB>();
                Connection.DropTable<DataTables.MyProfileTB>();
                Connection.DropTable<DataTables.SearchFilterTB>();
                Connection.DropTable<DataTables.NearByFilterTB>();
                Connection.DropTable<DataTables.WatchOfflineVideosTB>();
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
           // throw new Exception(exception.Message);
        }
    }

    #endregion

    //########################## End SQLite_Entity ##########################

    //Start SQL_Commander >>  General 
    //*********************************************************

    #region General

    public void InsertRow(object row)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                Connection.Insert(row);
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }

    public void UpdateRow(object row)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                Connection.Update(row);
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }

    public void DeleteRow(object row)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                Connection.Delete(row);
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }

    public void InsertListOfRows(List<object> row)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                Connection.InsertAll(row);
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }

    #endregion

    //Start SQL_Commander >>  Custom 
    //*********************************************************

    #region Login

    //Get data Login
    public DataTables.LoginTB Get_data_Login_Credentials()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var dataUser = Connection.Table<DataTables.LoginTB>().FirstOrDefault();
                if (dataUser != null)
                {
                    UserDetails.Username = dataUser.Username;
                    UserDetails.Full_name = dataUser.Username;
                    UserDetails.Password = dataUser.Password;
                    UserDetails.access_token = dataUser.access_token;
                    UserDetails.User_id = dataUser.UserID;
                    UserDetails.Status = dataUser.Status;
                    UserDetails.Cookie = dataUser.Cookie;
                    UserDetails.Email = dataUser.Email;
                    Settings.Lang = dataUser.Lang;
                    UserDetails.Device_ID = dataUser.Device_ID;

                    Current.CurrentInstance.Access_token = dataUser.access_token;
                    Classes.DataUserLoginList.Add(dataUser);

                    return dataUser;
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
            return null;
        }
    }

    //Delete data Login
    public void Delete_Login_Credentials()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var data = Connection.Table<DataTables.LoginTB>().FirstOrDefault(c => c.Status == "Active");
                if (data != null)
                {
                    Connection.Delete(data);
                }
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }

    //Clear All data Login
    public void Clear_Login_Credentials()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                Connection.DeleteAll<DataTables.LoginTB>();
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }

    #endregion

    #region My Contacts >> Following

    //Insert data To My Contact Table
    public void Insert_Or_Replace_MyContactTable(ObservableCollection<Classes.UserContacts.User> usersContactList)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var result = Connection.Table<DataTables.MyContactsTB>().ToList();
                var list = usersContactList.Select(user => new DataTables.MyContactsTB
                {
                    user_id = user.user_id,
                    username = user.username,
                    email = user.email,
                    first_name = user.first_name,
                    last_name = user.last_name,
                    avatar = user.avatar,
                    cover = user.cover,
                    relationship_id = user.relationship_id,
                    lastseen_time_text = user.lastseen_time_text,
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
                    user_platform = user.user_platform,
                    de_post_count = user.details.post_count,
                    de_album_count = user.details.album_count,
                    de_following_count = user.details.following_count,
                    de_followers_count = user.details.followers_count,
                    de_groups_count = user.details.groups_count,
                    de_likes_count = user.details.likes_count,
                    is_following = user.is_following,
                }).ToList();

                //var deleteItemlist = result.Where(c => !list.Select(fc => fc.user_id).Contains(c.user_id)).ToList();
                //if (deleteItemlist.Count > 0)
                //{
                //    //Connection.DeleteAll(deleteItemlist);
                //}

                //Bring new  
                var newItemList = list.Where(c => !result.Select(fc => fc.user_id).Contains(c.user_id)).ToList();
                if (newItemList.Count > 0)
                {
                    Connection.InsertAll(newItemList);
                }
                else
                {
                    Connection.UpdateAll(list);
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }

    // Get data To My Contact Table
    public ObservableCollection<Classes.UserContacts.User> Get_MyContact(int id, int nSize)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {               
                var query = Connection.Table<DataTables.MyContactsTB>().Where(w => w.ID >= id).OrderBy(q => q.ID).Take(nSize).ToList();
                if (query.Count > 0)
                {
                    var list = query.Select(user => new Classes.UserContacts.User
                    {
                        user_id = user.user_id,
                        username = user.username,
                        email = user.email,
                        first_name = user.first_name,
                        last_name = user.last_name,
                        avatar = user.avatar,
                        cover = user.cover,
                        relationship_id = user.relationship_id,
                        lastseen_time_text = user.lastseen_time_text,
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
                        user_platform = user.user_platform,
                        is_following = user.is_following,
                        details = new Details()
                        {
                            post_count = user.de_post_count,
                            album_count = user.de_album_count,
                            following_count = user.de_following_count,
                            followers_count = user.de_followers_count,
                            groups_count = user.de_groups_count,
                            likes_count = user.de_likes_count,
                        }
                    }).ToList();

                    if (list.Count > 0)
                    {
                        return new ObservableCollection<Classes.UserContacts.User>(list);
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
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    // Get data One user To My Contact Table
    public Classes.UserContacts.User Get_DataOneUser(string userId)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var user = Connection.Table<DataTables.MyContactsTB>().FirstOrDefault(c => c.user_id == userId);
                if (user != null)
                {
                    Classes.UserContacts.User item = new Classes.UserContacts.User()
                    {
                        user_id = user.user_id,
                        username = user.username,
                        email = user.email,
                        first_name = user.first_name,
                        last_name = user.last_name,
                        avatar = user.avatar,
                        cover = user.cover,
                        relationship_id = user.relationship_id,
                        lastseen_time_text = user.lastseen_time_text,
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
                        user_platform = user.user_platform,
                        is_following = user.is_following,
                        details = new Details()
                        {
                            post_count = user.de_post_count,
                            album_count = user.de_album_count,
                            following_count = user.de_following_count,
                            followers_count = user.de_followers_count,
                            groups_count = user.de_groups_count,
                            likes_count = user.de_likes_count,
                        }
                    };
                    return item;
                }
                else
                {
                    var userFollowers = Connection.Table<DataTables.MyFollowersTB>().FirstOrDefault(c => c.user_id == userId);
                    if (userFollowers != null)
                    {
                        Classes.UserContacts.User item = new Classes.UserContacts.User()
                        {
                            user_id = userFollowers.user_id,
                            username = userFollowers.username,
                            email = userFollowers.email,
                            first_name = userFollowers.first_name,
                            last_name = userFollowers.last_name,
                            avatar = userFollowers.avatar,
                            cover = userFollowers.cover,
                            relationship_id = userFollowers.relationship_id,
                            lastseen_time_text = userFollowers.lastseen_time_text,
                            address = userFollowers.address,
                            working = userFollowers.working,
                            working_link = userFollowers.working_link,
                            about = userFollowers.about,
                            school = userFollowers.school,
                            gender = userFollowers.gender,
                            birthday = userFollowers.birthday,
                            website = userFollowers.website,
                            facebook = userFollowers.facebook,
                            google = userFollowers.google,
                            twitter = userFollowers.twitter,
                            linkedin = userFollowers.linkedin,
                            youtube = userFollowers.youtube,
                            vk = userFollowers.vk,
                            instagram = userFollowers.instagram,
                            language = userFollowers.language,
                            ip_address = userFollowers.ip_address,
                            follow_privacy = userFollowers.follow_privacy,
                            friend_privacy = userFollowers.friend_privacy,
                            post_privacy = userFollowers.post_privacy,
                            message_privacy = userFollowers.message_privacy,
                            confirm_followers = userFollowers.confirm_followers,
                            show_activities_privacy = userFollowers.show_activities_privacy,
                            birth_privacy = userFollowers.birth_privacy,
                            visit_privacy = userFollowers.visit_privacy,
                            lastseen = userFollowers.lastseen,
                            showlastseen = userFollowers.showlastseen,
                            e_sentme_msg = userFollowers.e_sentme_msg,
                            e_last_notif = userFollowers.e_last_notif,
                            status = userFollowers.status,
                            active = userFollowers.active,
                            admin = userFollowers.admin,
                            registered = userFollowers.registered,
                            phone_number = userFollowers.phone_number,
                            is_pro = userFollowers.is_pro,
                            pro_type = userFollowers.pro_type,
                            joined = userFollowers.joined,
                            timezone = userFollowers.timezone,
                            referrer = userFollowers.referrer,
                            balance = userFollowers.balance,
                            paypal_email = userFollowers.paypal_email,
                            notifications_sound = userFollowers.notifications_sound,
                            order_posts_by = userFollowers.order_posts_by,
                            social_login = userFollowers.social_login,
                            device_id = userFollowers.device_id,
                            web_device_id = userFollowers.web_device_id,
                            wallet = userFollowers.wallet,
                            lat = userFollowers.lat,
                            lng = userFollowers.lng,
                            last_location_update = userFollowers.last_location_update,
                            share_my_location = userFollowers.share_my_location,
                            url = userFollowers.url,
                            name = userFollowers.name,
                            lastseen_unix_time = userFollowers.lastseen_unix_time,
                            user_platform = userFollowers.user_platform,
                            is_following = userFollowers.is_following.ToString(),
                            details = new Details()
                            {
                                post_count = userFollowers.de_post_count,
                                album_count = userFollowers.de_album_count,
                                following_count = userFollowers.de_following_count,
                                followers_count = userFollowers.de_followers_count,
                                groups_count = userFollowers.de_groups_count,
                                likes_count = userFollowers.de_likes_count,
                            }
                        };
                        return item;
                    }

                    return null;
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    //Remove data To My Contact Table
    public void Delete_UsersContact(string userId)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var user = Connection.Table<DataTables.MyContactsTB>().FirstOrDefault(c => c.user_id == userId);
                if (user != null)
                {
                    Connection.Delete(user);
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }

    #endregion

    #region My Followers

    //Insert data To My Followers Table
    public void Insert_Or_Replace_MyFollowersTable(ObservableCollection<Get_User_Data_Object.Followers> usersContactList)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var result = Connection.Table<DataTables.MyFollowersTB>().ToList();
                var list = usersContactList.Select(user => new DataTables.MyFollowersTB()
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
                    is_following = user.is_following,
                    //user_platform = user.user_platform,
                    de_post_count = user.details.post_count,
                    de_album_count = user.details.album_count,
                    de_following_count = user.details.following_count,
                    de_followers_count = user.details.followers_count,
                    de_groups_count = user.details.groups_count,
                    de_likes_count = user.details.likes_count,
                  
                }).ToList();

                var deleteItemlist = result.Where(c => !list.Select(fc => fc.user_id).Contains(c.user_id)).ToList();
                if (deleteItemlist.Count > 0)
                {
                    Connection.DeleteAll(deleteItemlist);
                }

                //Bring new  
                var newItemList = list.Where(c => !result.Select(fc => fc.user_id).Contains(c.user_id)).ToList();
                if (newItemList.Count > 0)
                {
                    Connection.InsertAll(newItemList);
                }
                else
                {
                    Connection.UpdateAll(list);
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }

    // Get data To My Followers Table
    public ObservableCollection<Get_User_Data_Object.Followers> Get_MyFollowers()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var result = Connection.Table<DataTables.MyFollowersTB>().ToList();
                if (result.Count > 0)
                {
                    var list = result.Select(user => new Get_User_Data_Object.Followers
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
                        is_following = user.is_following,
                        //user_platform = user.user_platform,
                        details = new Details()
                        {
                            post_count = user.de_post_count,
                            album_count = user.de_album_count,
                            following_count = user.de_following_count,
                            followers_count = user.de_followers_count,
                            groups_count = user.de_groups_count,
                            likes_count = user.de_likes_count,
                        }
                    }).ToList();

                    if (list.Count > 0)
                    {
                        return new ObservableCollection<Get_User_Data_Object.Followers>(list);
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
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    // Get data One user To My Followers Table
    public Get_User_Data_Object.Followers Get_DataOneUserFollowers(string userId)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var user = Connection.Table<DataTables.MyFollowersTB>().FirstOrDefault(c => c.user_id == userId);
                if (user != null)
                {
                    Get_User_Data_Object.Followers item = new Get_User_Data_Object.Followers()
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
                        is_following = user.is_following,
                        details = new Details()
                        {
                            post_count = user.de_post_count,
                            album_count = user.de_album_count,
                            following_count = user.de_following_count,
                            followers_count = user.de_followers_count,
                            groups_count = user.de_groups_count,
                            likes_count = user.de_likes_count,
                        }
                    };
                    return item;
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    //Remove data To My Followers Table
    public void Delete_UsersFollowers(string userId)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var user = Connection.Table<DataTables.MyContactsTB>().FirstOrDefault(c => c.user_id == userId);
                if (user != null)
                {
                    Connection.Delete(user);
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }

    #endregion

    #region Blocked Users

    //Insert data To Blocked Users Table
    public void Insert_Or_Replace_BlockedUsersTable(ObservableCollection<Get_Blocked_Users_Object.Blocked_Users> blockedUsersList)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var result = Connection.Table<DataTables.BlockedUsersTB>().ToList();
                var list = blockedUsersList.Select(user => new DataTables.BlockedUsersTB
                {
                    user_id = user.user_id,
                    username = user.username,
                    email = user.email,
                    first_name = user.first_name,
                    last_name = user.last_name,
                    avatar = user.avatar,
                    cover = user.cover,
                    background_image = user.background_image,
                    relationship_id = user.relationship_id,
                    address = user.address,
                    working = user.working,
                    working_link = user.working_link,
                    about = user.about,
                    school = user.school,
                    gender = user.gender,
                    birthday = user.birthday,
                    country_id = user.country_id,
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
                    verified = user.verified,
                    lastseen = user.lastseen,
                    emailNotification = user.emailNotification,
                    e_liked = user.e_liked,
                    e_wondered = user.e_wondered,
                    e_shared = user.e_shared,
                    e_followed = user.e_followed,
                    e_commented = user.e_commented,
                    e_visited = user.e_visited,
                    e_liked_page = user.e_liked_page,
                    e_mentioned = user.e_mentioned,
                    e_joined_group = user.e_joined_group,
                    e_accepted = user.e_accepted,
                    e_profile_wall_post = user.e_profile_wall_post,
                    e_sentme_msg = user.e_sentme_msg,
                    e_last_notif = user.e_last_notif,
                    status = user.status,
                    active = user.active,
                    admin = user.admin,
                    registered = user.registered,
                    phone_number = user.phone_number,
                    is_pro = user.is_pro,
                    pro_type = user.pro_type,
                    timezone = user.timezone,
                    referrer = user.referrer,
                    balance = user.balance,
                    paypal_email = user.paypal_email,
                    notifications_sound = user.notifications_sound,
                    order_posts_by = user.order_posts_by,
                    device_id = user.device_id,
                    web_device_id = user.web_device_id,
                    wallet = user.wallet,
                    lat = user.lat,
                    lng = user.lng,
                    last_location_update = user.last_location_update,
                    share_my_location = user.share_my_location,
                    last_data_update = user.last_data_update,
                    last_avatar_mod = user.last_avatar_mod,
                    last_cover_mod = user.last_cover_mod,
                    avatar_full = user.avatar_full,
                    url = user.url,
                    name = user.name,
                    lastseen_unix_time = user.lastseen_unix_time,
                    lastseen_status = user.lastseen_status,
                    de_post_count = user.details.post_count,
                    de_album_count = user.details.album_count,
                    de_following_count = user.details.following_count,
                    de_followers_count = user.details.followers_count,
                    de_groups_count = user.details.groups_count,
                    de_likes_count = user.details.likes_count,
                }).ToList();

                var deleteItemlist = result.Where(c => !list.Select(fc => fc.user_id).Contains(c.user_id)).ToList();
                if (deleteItemlist.Count > 0)
                {
                    Connection.DeleteAll(deleteItemlist);
                }

                //Bring new  
                var newItemList = list.Where(c => !result.Select(fc => fc.user_id).Contains(c.user_id)).ToList();
                if (newItemList.Count > 0)
                {
                    Connection.InsertAll(newItemList);
                }
                else
                {
                    Connection.UpdateAll(list);
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }

    public ObservableCollection<Get_Blocked_Users_Object.Blocked_Users> Get_Blocked_Users()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var result = Connection.Table<DataTables.BlockedUsersTB>().ToList();
                if (result.Count > 0)
                {
                    var list = result.Select(user => new Get_Blocked_Users_Object.Blocked_Users
                    {
                        user_id = user.user_id,
                        username = user.username,
                        email = user.email,
                        first_name = user.first_name,
                        last_name = user.last_name,
                        avatar = user.avatar,
                        cover = user.cover,
                        background_image = user.background_image,
                        relationship_id = user.relationship_id,
                        address = user.address,
                        working = user.working,
                        working_link = user.working_link,
                        about = user.about,
                        school = user.school,
                        gender = user.gender,
                        birthday = user.birthday,
                        country_id = user.country_id,
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
                        verified = user.verified,
                        lastseen = user.lastseen,
                        emailNotification = user.emailNotification,
                        e_liked = user.e_liked,
                        e_wondered = user.e_wondered,
                        e_shared = user.e_shared,
                        e_followed = user.e_followed,
                        e_commented = user.e_commented,
                        e_visited = user.e_visited,
                        e_liked_page = user.e_liked_page,
                        e_mentioned = user.e_mentioned,
                        e_joined_group = user.e_joined_group,
                        e_accepted = user.e_accepted,
                        e_profile_wall_post = user.e_profile_wall_post,
                        e_sentme_msg = user.e_sentme_msg,
                        e_last_notif = user.e_last_notif,
                        status = user.status,
                        active = user.active,
                        admin = user.admin,
                        registered = user.registered,
                        phone_number = user.phone_number,
                        is_pro = user.is_pro,
                        pro_type = user.pro_type,
                        timezone = user.timezone,
                        referrer = user.referrer,
                        balance = user.balance,
                        paypal_email = user.paypal_email,
                        notifications_sound = user.notifications_sound,
                        order_posts_by = user.order_posts_by,
                        device_id = user.device_id,
                        web_device_id = user.web_device_id,
                        wallet = user.wallet,
                        lat = user.lat,
                        lng = user.lng,
                        last_location_update = user.last_location_update,
                        share_my_location = user.share_my_location,
                        last_data_update = user.last_data_update,
                        last_avatar_mod = user.last_avatar_mod,
                        last_cover_mod = user.last_cover_mod,
                        avatar_full = user.avatar_full,
                        url = user.url,
                        name = user.name,
                        lastseen_unix_time = user.lastseen_unix_time,
                        lastseen_status = user.lastseen_status,
                        details = new Details()
                        {
                            post_count = user.de_post_count,
                            album_count = user.de_album_count,
                            following_count = user.de_following_count,
                            followers_count = user.de_followers_count,
                            groups_count = user.de_groups_count,
                            likes_count = user.de_likes_count,
                        }
                    }).ToList();

                    return new ObservableCollection<Get_Blocked_Users_Object.Blocked_Users>(list);
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    #endregion

    #region Manage Groups

    public void InsertOrReplace_ManageGroupsTable(ObservableCollection<Get_User_Data_Object.Joined_Groups> manageGroupsList)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var result = Connection.Table<DataTables.GroupsTB>().ToList();
                var list = manageGroupsList.Select(group => new DataTables.GroupsTB
                {
                    Id = group.id,
                    UserId = group.user_id,
                    GroupName = group.group_name,
                    GroupTitle = group.group_title,
                    Avatar = group.avatar,
                    Cover = group.cover,
                    About = group.about,
                    Category = group.category,
                    Privacy = group.privacy,
                    JoinPrivacy = group.join_privacy,
                    Active = group.active,
                    Registered = group.registered,
                    GroupId = group.group_id,
                    Url = group.url,
                    Name = group.name,
                    CategoryId = group.category_id,
                    Type = group.type,
                    Username = group.username
                }).ToList();

                var manage = result.Where(a => a.UserId == UserDetails.User_id).ToList();
                if (manage.Count > 0)
                {
                    var deleteItemlist = manage.Where(c => !list.Select(fc => fc.GroupId).Contains(c.GroupId)).ToList();
                    if (deleteItemlist.Count > 0)
                    {
                        Connection.DeleteAll(deleteItemlist);
                    }
                }

                //Bring new  
                var newItemList = list.Where(c => !result.Select(fc => fc.GroupId).Contains(c.GroupId)).ToList();
                if (newItemList.Count > 0)
                {
                    Connection.InsertAll(newItemList);
                }
                else
                {
                    Connection.UpdateAll(list);
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }

    public ObservableCollection<Get_User_Data_Object.Joined_Groups> GetAll_ManageGroups()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var result = Connection.Table<DataTables.GroupsTB>().Where(a => a.UserId == UserDetails.User_id).ToList();
                if (result.Count > 0)
                {
                    var list = result.Select(group => new Get_User_Data_Object.Joined_Groups
                    {
                        id = group.Id,
                        user_id = group.UserId,
                        group_name = group.GroupName,
                        group_title = group.GroupTitle,
                        avatar = group.Avatar,
                        cover = group.Cover,
                        about = group.About,
                        category = group.Category,
                        privacy = group.Privacy,
                        join_privacy = group.JoinPrivacy,
                        active = group.Active,
                        registered = group.Registered,
                        group_id = group.GroupId,
                        url = group.Url,
                        name = group.Name,
                        category_id = group.CategoryId,
                        type = group.Type,
                        username = group.Username
                    }).ToList();

                    return new ObservableCollection<Get_User_Data_Object.Joined_Groups>(list);
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    #endregion

    #region Communities Groups

    public void Insert_Or_Replace_GroupsTable(ObservableCollection<Get_Community_Object.Group> groupsList)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var result = Connection.Table<DataTables.GroupsTB>().ToList();
                var list = groupsList.Select(group => new DataTables.GroupsTB()
                {
                    Id = group.Id,
                    UserId = group.UserId,
                    GroupName = group.GroupName,
                    GroupTitle = group.GroupTitle,
                    Avatar = group.Avatar,
                    Cover = group.Cover,
                    About = group.About,
                    Category = group.Category,
                    Privacy = group.Privacy,
                    JoinPrivacy = group.JoinPrivacy,
                    Active = group.Active,
                    Registered = group.Registered,
                    GroupId = group.GroupId,
                    Url = group.Url,
                    Name = group.Name,
                    CategoryId = group.CategoryId,
                    Type = group.Type,
                    Username = group.Username
                }).ToList();

                var communities = result.Where(a => a.UserId != UserDetails.User_id).ToList();
                if (communities.Count > 0)
                {
                    var deleteItemlist = communities.Where(c => !list.Select(fc => fc.GroupId).Contains(c.GroupId)).ToList();
                    if (deleteItemlist.Count > 0)
                    {
                        Connection.DeleteAll(deleteItemlist);
                    }
                }

                //Bring new  
                var newItemList = list.Where(c => !result.Select(fc => fc.GroupId).Contains(c.GroupId)).ToList();
                if (newItemList.Count > 0)
                {
                    Connection.InsertAll(newItemList);
                }
                else
                {
                    Connection.UpdateAll(list);
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }

    public ObservableCollection<Get_Community_Object.Group> Get_Groups()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var result = Connection.Table<DataTables.GroupsTB>().Where(a => a.UserId != UserDetails.User_id).ToList();
                if (result.Count > 0)
                {
                    var list = result.Select(group => new Get_Community_Object.Group
                    {
                        Id = group.Id,
                        UserId = group.UserId,
                        GroupName = group.GroupName,
                        GroupTitle = group.GroupTitle,
                        Avatar = group.Avatar,
                        Cover = group.Cover,
                        About = group.About,
                        Category = group.Category,
                        Privacy = group.Privacy,
                        JoinPrivacy = group.JoinPrivacy,
                        Active = group.Active,
                        Registered = group.Registered,
                        GroupId = group.GroupId,
                        Url = group.Url,
                        Name = group.Name,
                        CategoryId = group.CategoryId,
                        Type = group.Type,
                        Username = group.Username
                    }).ToList();

                    return new ObservableCollection<Get_Community_Object.Group>(list);
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    public void Insert_Or_Delete_OneGroupsTable(string groupId, object item)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var group = Connection.Table<DataTables.GroupsTB>().FirstOrDefault(c => c.GroupId == groupId);
                if (group != null)
                {
                    Connection.Delete(group);
                }
                else
                {
                    Connection.Insert(item);
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }

    public Get_Community_Object.Group Get_ItemIsOwner_Groups(string groupId)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var group = Connection.Table<DataTables.GroupsTB>().FirstOrDefault(a => a.GroupId == groupId && a.UserId == UserDetails.User_id);
                if (group != null)
                {
                    var data = new Get_Community_Object.Group
                    {
                        Id = group.Id,
                        UserId = group.UserId,
                        GroupName = group.GroupName,
                        GroupTitle = group.GroupTitle,
                        Avatar = group.Avatar,
                        Cover = group.Cover,
                        About = group.About,
                        Category = group.Category,
                        Privacy = group.Privacy,
                        JoinPrivacy = group.JoinPrivacy,
                        Active = group.Active,
                        Registered = group.Registered,
                        GroupId = group.GroupId,
                        Url = group.Url,
                        Name = group.Name,
                        CategoryId = group.CategoryId,
                        Type = group.Type,
                        Username = group.Username
                    };

                    return data;
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    #endregion

    #region Manage Pages 

    public void InsertOrReplace_ManagePagesTable(ObservableCollection<Get_User_Data_Object.Liked_Pages> managepagesList)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var result = Connection.Table<DataTables.PageTB>().ToList();
                var list = managepagesList.Select(page => new DataTables.PageTB
                {
                    PageId = page.page_id,
                    UserId = page.user_id,
                    PageName = page.page_name,
                    PageTitle = page.page_title,
                    PageDescription = page.page_description,
                    Avatar = page.avatar,
                    Cover = page.cover,
                    PageCategory = page.page_category,
                    Website = page.website,
                    Facebook = page.facebook,
                    Google = page.google,
                    Vk = page.vk,
                    Twitter = page.twitter,
                    Linkedin = page.linkedin,
                    Company = page.company,
                    Phone = page.phone,
                    Address = page.address,
                    CallActionType = page.call_action_type,
                    CallActionTypeUrl = page.call_action_type_url,
                    BackgroundImage = page.background_image,
                    BackgroundImageStatus = page.background_image_status,
                    Instgram = page.instgram,
                    Youtube = page.youtube,
                    Verified = page.verified,
                    Registered = page.registered,
                    Boosted = page.boosted,
                    About = page.about,
                    Id = page.id,
                    Type = page.type,
                    Url = page.url,
                    Name = page.name,
                    //Rating = page.rating,
                    Category = page.category,
                    IsPageOnwer = Convert.ToString(page.is_page_onwer), 
                    Username = page.username,
                }).ToList();

                var manage = result.Where(a => a.UserId == UserDetails.User_id).ToList();
                if (manage.Count > 0)
                {
                    var deleteItemlist = manage.Where(c => !list.Select(fc => fc.PageId).Contains(c.PageId)).ToList();
                    if (deleteItemlist.Count > 0)
                    {
                        Connection.DeleteAll(deleteItemlist);
                    }
                }

                //Bring new  
                var newItemList = list.Where(c => !result.Select(fc => fc.PageId).Contains(c.PageId)).ToList();
                if (newItemList.Count > 0)
                {
                    Connection.InsertAll(newItemList);
                }
                else
                {
                    Connection.UpdateAll(list);
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }

    public ObservableCollection<Get_User_Data_Object.Liked_Pages> GetAll_ManagePages()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var result = Connection.Table<DataTables.PageTB>().Where(a => a.UserId == UserDetails.User_id).ToList();
                if (result.Count > 0)
                {
                    var list = result.Select(page => new Get_User_Data_Object.Liked_Pages
                    {
                        page_id = page.PageId,
                        user_id = page.UserId,
                        page_name = page.PageName,
                        page_title = page.PageTitle,
                        page_description = page.PageDescription,
                        avatar = page.Avatar,
                        cover = page.Cover,
                        page_category = page.PageCategory,
                        website = page.Website,
                        facebook = page.Facebook,
                        google = page.Google,
                        vk = page.Vk,
                        twitter = page.Twitter,
                        linkedin = page.Linkedin,
                        company = page.Company,
                        phone = page.Phone,
                        address = page.Address,
                        call_action_type = page.CallActionType,
                        call_action_type_url = page.CallActionTypeUrl,
                        background_image = page.BackgroundImage,
                        background_image_status = page.BackgroundImageStatus,
                        instgram = page.Instgram,
                        youtube = page.Youtube,
                        verified = page.Verified,
                        registered = page.Registered,
                        boosted = page.Boosted,
                        about = page.About,
                        id = page.Id,
                        type = page.Type,
                        url = page.Url,
                        name = page.Name,
                        //rating = page.Rating,
                        category = page.Category,
                        is_page_onwer = Convert.ToBoolean(page.IsPageOnwer),
                        username = page.Username,
                    }).ToList();

                    return new ObservableCollection<Get_User_Data_Object.Liked_Pages>(list);
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    #endregion

    #region Communities Pages and liked 

    public void Insert_Or_Replace_PagesTable(ObservableCollection<Get_Community_Object.Page> pagesList)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {

                var result = Connection.Table<DataTables.PageTB>().ToList();
                var list = pagesList.Select(page => new DataTables.PageTB
                {
                    PageId = page.PageId,
                    UserId = page.UserId,
                    PageName = page.PageName,
                    PageTitle = page.PageTitle,
                    PageDescription = page.PageDescription,
                    Avatar = page.Avatar,
                    Cover = page.Cover,
                    PageCategory = page.PageCategory,
                    Website = page.Website,
                    Facebook = page.Facebook,
                    Google = page.Google,
                    Vk = page.Vk,
                    Twitter = page.Twitter,
                    Linkedin = page.Linkedin,
                    Company = page.Company,
                    Phone = page.Phone,
                    Address = page.Address,
                    CallActionType = page.CallActionType,
                    CallActionTypeUrl = page.CallActionTypeUrl,
                    BackgroundImage = page.BackgroundImage,
                    BackgroundImageStatus = page.BackgroundImageStatus,
                    Instgram = page.Instgram,
                    Youtube = page.Youtube,
                    Verified = page.Verified,
                    Registered = page.Registered,
                    Boosted = page.Boosted,
                    About = page.About,
                    Id = page.Id,
                    Type = page.Type,
                    Url = page.Url,
                    Name = page.Name,
                    //Rating = page.Rating,
                    Category = page.Category,
                    IsPageOnwer = page.IsPageOnwer.ToString(),
                    Username = page.Username,
                }).ToList();

                var communities = result.Where(a => a.UserId != UserDetails.User_id).ToList();
                if (communities.Count > 0)
                {
                    var deleteItemlist = communities.Where(c => !list.Select(fc => fc.PageId).Contains(c.PageId)).ToList();
                    if (deleteItemlist.Count > 0)
                    {
                        Connection.DeleteAll(deleteItemlist);
                    }
                }

                //Bring new  
                var newItemList = list.Where(c => !result.Select(fc => fc.PageId).Contains(c.PageId)).ToList();
                if (newItemList.Count > 0)
                {
                    Connection.InsertAll(newItemList);
                }
                else
                {
                    Connection.UpdateAll(list);
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }

    public ObservableCollection<Get_Community_Object.Page> Get_Pages()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var result = Connection.Table<DataTables.PageTB>().Where(a => a.UserId != UserDetails.User_id).ToList();
                if (result.Count > 0)
                {
                    var list = result.Select(page => new Get_Community_Object.Page
                    {
                        PageId = page.PageId,
                        UserId = page.UserId,
                        PageName = page.PageName,
                        PageTitle = page.PageTitle,
                        PageDescription = page.PageDescription,
                        Avatar = page.Avatar,
                        Cover = page.Cover,
                        PageCategory = page.PageCategory,
                        Website = page.Website,
                        Facebook = page.Facebook,
                        Google = page.Google,
                        Vk = page.Vk,
                        Twitter = page.Twitter,
                        Linkedin = page.Linkedin,
                        Company = page.Company,
                        Phone = page.Phone,
                        Address = page.Address,
                        CallActionType = page.CallActionType,
                        CallActionTypeUrl = page.CallActionTypeUrl,
                        BackgroundImage = page.BackgroundImage,
                        BackgroundImageStatus = page.BackgroundImageStatus,
                        Instgram = page.Instgram,
                        Youtube = page.Youtube,
                        Verified = page.Verified,
                        Registered = page.Registered,
                        Boosted = page.Boosted,
                        About = page.About,
                        Id = page.Id,
                        Type = page.Type,
                        Url = page.Url,
                        Name = page.Name,
                        //Rating = page.Rating,
                        Category = page.Category,
                        IsPageOnwer = Convert.ToBoolean(page.IsPageOnwer),
                        Username = page.Username,
                    }).ToList();

                    return new ObservableCollection<Get_Community_Object.Page>(list);

                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    public void Insert_Or_Delete_OnePagesTable(string pageId, object item)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var group = Connection.Table<DataTables.PageTB>().FirstOrDefault(c => c.PageId == pageId);
                if (group != null)
                {
                    Connection.Delete(group);
                }
                else
                {
                    Connection.Insert(item);
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }

    public Get_Community_Object.Page Get_ItemIsOwner_Page(string pageId)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var page = Connection.Table<DataTables.PageTB>().FirstOrDefault(a => a.PageId == pageId && a.UserId == UserDetails.User_id);
                if (page != null)
                {
                    var data = new Get_Community_Object.Page
                    {
                        PageId = page.PageId,
                        UserId = page.UserId,
                        PageName = page.PageName,
                        PageTitle = page.PageTitle,
                        PageDescription = page.PageDescription,
                        Avatar = page.Avatar,
                        Cover = page.Cover,
                        PageCategory = page.PageCategory,
                        Website = page.Website,
                        Facebook = page.Facebook,
                        Google = page.Google,
                        Vk = page.Vk,
                        Twitter = page.Twitter,
                        Linkedin = page.Linkedin,
                        Company = page.Company,
                        Phone = page.Phone,
                        Address = page.Address,
                        CallActionType = page.CallActionType,
                        CallActionTypeUrl = page.CallActionTypeUrl,
                        BackgroundImage = page.BackgroundImage,
                        BackgroundImageStatus = page.BackgroundImageStatus,
                        Instgram = page.Instgram,
                        Youtube = page.Youtube,
                        Verified = page.Verified,
                        Registered = page.Registered,
                        Boosted = page.Boosted,
                        About = page.About,
                        Id = page.Id,
                        Type = page.Type,
                        Url = page.Url,
                        Name = page.Name,
                        //Rating = page.Rating,
                        Category = page.Category,
                        IsPageOnwer = Convert.ToBoolean(page.IsPageOnwer),
                        Username = page.Username,
                    };

                    return data;
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    #endregion

    #region My Profile

    //Insert Or Update data My Profile Table
    public void Insert_Or_Update_To_MyProfileTable(Get_User_Data_Object.User_Data user)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var data = Connection.Table<DataTables.MyProfileTB>().FirstOrDefault(a => a.user_id == user.user_id);
                if (data != null)
                {
                    data.user_id = user.user_id;
                    data.username = user.username;
                    data.email = user.email;
                    data.first_name = user.first_name;
                    data.last_name = user.last_name;
                    data.avatar = user.avatar;
                    data.cover = user.cover;
                    data.background_image = user.background_image;
                    data.relationship_id = user.relationship_id;
                    data.address = user.address;
                    data.working = user.working;
                    data.working_link = user.working_link;
                    data.about = user.about;
                    data.school = user.school;
                    data.gender = user.gender;
                    data.birthday = user.birthday;
                    data.country_id = user.country_id;
                    data.website = user.website;
                    data.facebook = user.facebook;
                    data.google = user.google;
                    data.twitter = user.twitter;
                    data.linkedin = user.linkedin;
                    data.youtube = user.youtube;
                    data.vk = user.vk;
                    data.instagram = user.instagram;
                    data.language = user.language;
                    data.ip_address = user.ip_address;
                    data.follow_privacy = user.follow_privacy;
                    data.friend_privacy = user.friend_privacy;
                    data.post_privacy = user.post_privacy;
                    data.message_privacy = user.message_privacy;
                    data.confirm_followers = user.confirm_followers;
                    data.show_activities_privacy = user.show_activities_privacy;
                    data.birth_privacy = user.birth_privacy;
                    data.visit_privacy = user.visit_privacy;
                    data.lastseen = user.lastseen;
                    data.emailNotification = user.emailNotification;
                    data.e_liked = user.e_liked;
                    data.e_wondered = user.e_wondered;
                    data.e_shared = user.e_shared;
                    data.e_followed = user.e_followed;
                    data.e_commented = user.e_commented;
                    data.e_visited = user.e_visited;
                    data.e_liked_page = user.e_liked_page;
                    data.e_mentioned = user.e_mentioned;
                    data.e_joined_group = user.e_joined_group;
                    data.e_accepted = user.e_accepted;
                    data.e_profile_wall_post = user.e_profile_wall_post;
                    data.e_sentme_msg = user.e_sentme_msg;
                    data.e_last_notif = user.e_last_notif;
                    data.status = user.status;
                    data.active = user.active;
                    data.admin = user.admin;
                    data.registered = user.registered;
                    data.phone_number = user.phone_number;
                    data.is_pro = user.is_pro;
                    data.pro_type = user.pro_type;
                    data.timezone = user.timezone;
                    data.referrer = user.referrer;
                    data.balance = user.balance;
                    data.paypal_email = user.paypal_email;
                    data.notifications_sound = user.notifications_sound;
                    data.order_posts_by = user.order_posts_by;
                    data.device_id = user.device_id;
                    data.web_device_id = user.web_device_id;
                    data.wallet = user.wallet;
                    data.lat = user.lat;
                    data.lng = user.lng;
                    data.last_location_update = user.last_location_update;
                    data.share_my_location = user.share_my_location;
                    data.last_data_update = user.last_data_update;
                    data.last_avatar_mod = user.last_avatar_mod;
                    data.last_cover_mod = user.last_cover_mod;
                    data.avatar_full = user.avatar_full;
                    data.url = user.url;
                    data.name = user.name;
                    data.lastseen_unix_time = user.lastseen_unix_time;
                    data.lastseen_status = user.lastseen_status;
                    data.is_following = user.is_following.ToString();
                    data.can_follow = user.can_follow.ToString();
                    data.is_following_me = user.is_following_me.ToString();
                    data.gender_text = user.gender_text;
                    data.lastseen_status = user.lastseen_status;
                    data.lastseen_time_text = user.lastseen_time_text;
                    data.is_blocked = user.is_blocked.ToString();
                    data.de_post_count = user.details.post_count;
                    data.de_album_count = user.details.album_count;
                    data.de_following_count = user.details.following_count;
                    data.de_followers_count = user.details.followers_count;
                    data.de_groups_count = user.details.groups_count;
                    data.de_likes_count = user.details.likes_count;

                    UserDetails.avatar = user.avatar;
                    UserDetails.cover = user.cover;
                    UserDetails.Username = user.username;
                    UserDetails.Full_name = user.name;
                    UserDetails.Email = user.email;

                    Connection.Update(data);
                }
                else
                {
                    DataTables.MyProfileTB udb = new DataTables.MyProfileTB
                    {
                        user_id = user.user_id,
                        username = user.username,
                        email = user.email,
                        first_name = user.first_name,
                        last_name = user.last_name,
                        avatar = user.avatar,
                        cover = user.cover,
                        background_image = user.background_image,
                        relationship_id = user.relationship_id,
                        address = user.address,
                        working = user.working,
                        working_link = user.working_link,
                        about = user.about,
                        school = user.school,
                        gender = user.gender,
                        birthday = user.birthday,
                        country_id = user.country_id,
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
                        emailNotification = user.emailNotification,
                        e_liked = user.e_liked,
                        e_wondered = user.e_wondered,
                        e_shared = user.e_shared,
                        e_followed = user.e_followed,
                        e_commented = user.e_commented,
                        e_visited = user.e_visited,
                        e_liked_page = user.e_liked_page,
                        e_mentioned = user.e_mentioned,
                        e_joined_group = user.e_joined_group,
                        e_accepted = user.e_accepted,
                        e_profile_wall_post = user.e_profile_wall_post,
                        e_sentme_msg = user.e_sentme_msg,
                        e_last_notif = user.e_last_notif,
                        status = user.status,
                        active = user.active,
                        admin = user.admin,
                        registered = user.registered,
                        phone_number = user.phone_number,
                        is_pro = user.is_pro,
                        pro_type = user.pro_type,
                        timezone = user.timezone,
                        referrer = user.referrer,
                        balance = user.balance,
                        paypal_email = user.paypal_email,
                        notifications_sound = user.notifications_sound,
                        order_posts_by = user.order_posts_by,
                        device_id = user.device_id,
                        web_device_id = user.web_device_id,
                        wallet = user.wallet,
                        lat = user.lat,
                        lng = user.lng,
                        last_location_update = user.last_location_update,
                        share_my_location = user.share_my_location,
                        last_data_update = user.last_data_update,
                        last_avatar_mod = user.last_avatar_mod,
                        last_cover_mod = user.last_cover_mod,
                        avatar_full = user.avatar_full,
                        url = user.url,
                        name = user.name,
                        lastseen_unix_time = user.lastseen_unix_time,
                        lastseen_status = user.lastseen_status,
                        is_following = user.is_following.ToString(),
                        can_follow = user.can_follow.ToString(),
                        is_following_me = user.is_following_me.ToString(),
                        gender_text = user.gender_text,
                        lastseen_time_text = user.lastseen_time_text,
                        is_blocked = user.is_blocked.ToString(),
                        de_post_count = user.details.post_count,
                        de_album_count = user.details.album_count,
                        de_following_count = user.details.following_count,
                        de_followers_count = user.details.followers_count,
                        de_groups_count = user.details.groups_count,
                        de_likes_count = user.details.likes_count,
                    };

                    UserDetails.avatar = udb.avatar;
                    UserDetails.cover = udb.cover;
                    UserDetails.Username = udb.username;
                    UserDetails.Full_name = udb.name;
                    UserDetails.Email = udb.email;

                    //Insert 
                    Connection.Insert(udb);
                }

                Classes.MyProfileList = new ObservableCollection<Get_User_Data_Object.User_Data>();
                if (Classes.MyProfileList.Count > 0)
                {
                    Classes.MyProfileList.Clear();
                    Classes.MyProfileList.Add(user);
                }
                else
                {
                    Classes.MyProfileList.Add(user);
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }

    // Get data To My Profile Table
    public ObservableCollection<Get_User_Data_Object.User_Data> Get_MyProfile_CredentialList()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var listdata = new ObservableCollection<Get_User_Data_Object.User_Data>();
                var user = Connection.Table<DataTables.MyProfileTB>().FirstOrDefault(a => a.user_id == UserDetails.User_id);
                if (user != null)
                {
                    Get_User_Data_Object.User_Data data = new Get_User_Data_Object.User_Data()
                    {
                        user_id = user.user_id,
                        username = user.username,
                        email = user.email,
                        first_name = user.first_name,
                        last_name = user.last_name,
                        avatar = user.avatar,
                        cover = user.cover,
                        background_image = user.background_image,
                        relationship_id = user.relationship_id,
                        address = user.address,
                        working = user.working,
                        working_link = user.working_link,
                        about = user.about,
                        school = user.school,
                        gender = user.gender,
                        birthday = user.birthday,
                        country_id = user.country_id,
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
                        emailNotification = user.emailNotification,
                        e_liked = user.e_liked,
                        e_wondered = user.e_wondered,
                        e_shared = user.e_shared,
                        e_followed = user.e_followed,
                        e_commented = user.e_commented,
                        e_visited = user.e_visited,
                        e_liked_page = user.e_liked_page,
                        e_mentioned = user.e_mentioned,
                        e_joined_group = user.e_joined_group,
                        e_accepted = user.e_accepted,
                        e_profile_wall_post = user.e_profile_wall_post,
                        e_sentme_msg = user.e_sentme_msg,
                        e_last_notif = user.e_last_notif,
                        status = user.status,
                        active = user.active,
                        admin = user.admin,
                        registered = user.registered,
                        phone_number = user.phone_number,
                        is_pro = user.is_pro,
                        pro_type = user.pro_type,
                        timezone = user.timezone,
                        referrer = user.referrer,
                        balance = user.balance,
                        paypal_email = user.paypal_email,
                        notifications_sound = user.notifications_sound,
                        order_posts_by = user.order_posts_by,
                        device_id = user.device_id,
                        web_device_id = user.web_device_id,
                        wallet = user.wallet,
                        lat = user.lat,
                        lng = user.lng,
                        last_location_update = user.last_location_update,
                        share_my_location = user.share_my_location,
                        last_data_update = user.last_data_update,
                        last_avatar_mod = user.last_avatar_mod,
                        last_cover_mod = user.last_cover_mod,
                        avatar_full = user.avatar_full,
                        url = user.url,
                        name = user.name,
                        lastseen_unix_time = user.lastseen_unix_time,
                        lastseen_status = user.lastseen_status,
                        is_following = Convert.ToInt32(user.is_following),
                        can_follow = Convert.ToInt32(user.can_follow),
                        is_following_me = Convert.ToInt32(user.is_following_me),
                        gender_text = user.gender_text,
                        lastseen_time_text = user.lastseen_time_text,
                        is_blocked = Convert.ToBoolean(user.is_blocked),
                        details = new Details()
                        {
                            post_count = user.de_post_count,
                            album_count = user.de_album_count,
                            following_count = user.de_following_count,
                            followers_count = user.de_followers_count,
                            groups_count = user.de_groups_count,
                            likes_count = user.de_likes_count,
                        }
                    };

                    listdata.Add(data);

                    UserDetails.Username = data.name;
                    UserDetails.Full_name = data.name;
                    UserDetails.avatar = data.avatar;
                    UserDetails.cover = data.cover;

                    Classes.MyProfileList = new ObservableCollection<Get_User_Data_Object.User_Data>();
                    Classes.MyProfileList.Clear();
                    Classes.MyProfileList = listdata;

                    return listdata;
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    #endregion

    #region Search Filter 

    public void InsertOrUpdate_SearchFilter(DataTables.SearchFilterTB dataFilter)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var data = Connection.Table<DataTables.SearchFilterTB>().FirstOrDefault(c => c.UserId == dataFilter.UserId);
                if (data == null)
                {
                    Connection.Insert(dataFilter);
                }
                else
                {
                    data.UserId = dataFilter.UserId;
                    data.ProfilePicture = dataFilter.ProfilePicture;
                    data.Gender = dataFilter.Gender;
                    data.Status = dataFilter.Status; 
                    Connection.Update(data);
                }
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }

    public DataTables.SearchFilterTB GetSearchFilterById()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var data = Connection.Table<DataTables.SearchFilterTB>().FirstOrDefault(c => c.UserId == UserDetails.User_id);
                if (data != null)
                    return data;
                else
                    return null;
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    #endregion

    #region Near By Filter 

    public void InsertOrUpdate_NearByFilter(DataTables.NearByFilterTB dataFilter)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var data = Connection.Table<DataTables.NearByFilterTB>().FirstOrDefault(c => c.UserId == dataFilter.UserId);
                if (data == null)
                {
                    Connection.Insert(dataFilter);
                }
                else
                {
                    data.UserId = dataFilter.UserId;
                    data.DistanceValue = dataFilter.DistanceValue;
                    data.Gender = dataFilter.Gender;
                    data.Status = dataFilter.Status;

                    Connection.Update(data);
                }
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }

    public DataTables.NearByFilterTB GetNearByFilterById()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var data = Connection.Table<DataTables.NearByFilterTB>().FirstOrDefault(c => c.UserId == UserDetails.User_id);
                if (data != null)
                    return data;
                else
                    return null;
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            return null;
        }
    }

    #endregion

    #region WatchOffline Videos

    //Insert WatchOffline Videos
    public void Insert_WatchOfflineVideos(Get_Movies_Object.Movie video)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                if (video != null)
                {
                    var select = Connection.Table<DataTables.WatchOfflineVideosTB>().FirstOrDefault(a => a.id == video.id);
                    if (select == null)
                    {
                        DataTables.WatchOfflineVideosTB watchOffline = new DataTables.WatchOfflineVideosTB()
                        {
                            id = video.id,
                            name = video.name,
                            cover = video.cover,
                            description = video.description,
                            country = video.country,
                            duration = video.duration,
                            genre = video.genre,
                            iframe = video.iframe,
                            quality = video.quality,
                            producer = video.producer,
                            release = video.release,
                            source = video.source,
                            stars = video.stars,
                            url = video.url,
                            video = video.video,
                            views = video.views,
                        };

                        Connection.Insert(watchOffline);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }

    //Remove WatchOffline Videos
    public void Remove_WatchOfflineVideos(string watchOfflineVideos_id)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                if (!string.IsNullOrEmpty(watchOfflineVideos_id))
                {
                    var select = Connection.Table<DataTables.WatchOfflineVideosTB>().FirstOrDefault(a => a.id == watchOfflineVideos_id);
                    if (select != null)
                    {
                        Connection.Delete(select);
                    }
                }
            }
               
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }

    //Get WatchOffline Videos
    public ObservableCollection<DataTables.WatchOfflineVideosTB> Get_WatchOfflineVideos()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var select = Connection.Table<DataTables.WatchOfflineVideosTB>().ToList().OrderByDescending(a => a.AutoIDWatchOfflineVideos);
                if (select.Count() > 0)
                {
                    return new ObservableCollection<DataTables.WatchOfflineVideosTB>(select);
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
            return null;
        }
    }

    //Get WatchOffline Videos
    public Get_Movies_Object.Movie Get_WatchOfflineVideos_ById(string id)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var video = Connection.Table<DataTables.WatchOfflineVideosTB>().FirstOrDefault(a => a.id == id);
                if (video != null)
                {
                    Get_Movies_Object.Movie watchOffline = new Get_Movies_Object.Movie()
                    {
                        id = video.id,
                        name = video.name,
                        cover = video.cover,
                        description = video.description,
                        country = video.country,
                        duration = video.duration,
                        genre = video.genre,
                        iframe = video.iframe,
                        quality = video.quality,
                        producer = video.producer,
                        release = video.release,
                        source = video.source,
                        stars = video.stars,
                        url = video.url,
                        video = video.video,
                        views = video.views,
                    };

                    return watchOffline;
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
            return null;
        }
    }

    public DataTables.WatchOfflineVideosTB Update_WatchOfflineVideos(string videoId, string videoPath)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var select = Connection.Table<DataTables.WatchOfflineVideosTB>().FirstOrDefault(a => a.id == videoId);
                if (select != null)
                {
                    select.Video_Name = videoId + ".mp4";
                    select.Video_SavedPath = videoPath;

                    Connection.Update(select);

                    return select;
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
            return null;
        }
    }

    #endregion

    #region Catigories

    //Insert data Categories
    public void Insert_Categories(ObservableCollection<DataTables.CatigoriesTB> ListData)
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                Connection.InsertAll(ListData);
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }

    //Get List Categories 
    public ObservableCollection<Classes.Catigories> Get_CategoriesList()
    {
        try
        {
            using (Connection = new SQLiteConnection(PathCombine))
            {
                var result = Connection.Table<DataTables.CatigoriesTB>().ToList();
                if (result?.Count > 0)
                {
                    var list = result.Select(cat => new Classes.Catigories
                    {
                        Catigories_Id = cat.Catigories_Id,
                        Catigories_Name = cat.Catigories_Name,
                        Catigories_Color = "#212121"
                    }).ToList();

                    return new ObservableCollection<Classes.Catigories>(list);
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception exception)
        {
            Crashes.TrackError(exception);
            return null;
        }
    }
     
    #endregion
}