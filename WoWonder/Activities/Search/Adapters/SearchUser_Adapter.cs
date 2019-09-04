using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;

namespace WoWonder.Activities.Search.Adapters
{
    public class SearchUser_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_Search_Object.User> mSearchUserList =
            new ObservableCollection<Get_Search_Object.User>();

        public SearchUser_Adapter(Context context)
        {
            try
            {
                Activity_Context = context;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override int ItemCount
        {
            get
            {
                if (mSearchUserList != null)
                    return mSearchUserList.Count;
                return 0;
            }
        }

        public event EventHandler<SearchUser_AdapterClickEventArgs> ItemClick;
        public event EventHandler<SearchUser_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_HContact_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_HContact_view, parent, false);
                var vh = new SearchUser_AdapterViewHolder(itemView, OnClick, OnLongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                _position = position;
                if (viewHolder is SearchUser_AdapterViewHolder holder)
                {
                    var item = mSearchUserList[_position];
                    if (item != null)
                    {
                        //Dont Remove this code #####
                        FontController.SetFont(holder.Name, 1);
                        FontController.SetFont(holder.About, 3);
                        FontController.SetFont(holder.Button, 1);
                        //#####
                        Initialize(holder, item);
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Initialize(SearchUser_AdapterViewHolder holder, Get_Search_Object.User item)
        {
            try
            {
                var AvatarSplit = item.Avatar.Split('/').Last();
                var getImage_Avatar =
                    IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                if (getImage_Avatar != "File Dont Exists")
                {
                    if (holder.Image.Tag?.ToString() != "loaded")
                    {
                        ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", getImage_Avatar, 1);
                        holder.Image.Tag = "loaded";
                    }
                }
                else
                {
                    if (holder.Image.Tag?.ToString() != "loaded")
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, item.Avatar);
                        ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", item.Avatar, 1);
                        holder.Image.Tag = "loaded";
                    }
                }

                var CoverSplit = item.Cover.Split('/').Last();
                var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, CoverSplit);
                if (getImage_Cover != "File Dont Exists")
                {
                    //ImageServiceLoader.Load_Image(CoverImage, "ImagePlacholder.jpg", getImage_Cover);
                }
                else
                {
                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, item.Cover);
                    //ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", users.cover);
                }
                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(item.Name));
                holder.Name.Text = IMethods.Fun_String.SubStringCutOf(name, 14);

                var time = IMethods.ITime.TimeAgo(int.Parse(item.LastseenUnixTime));
                if (time.Contains("hours ago"))
                    holder.About.Text = Activity_Context.GetText(Resource.String.Lbl_Last_seen) + " " +
                                        time.Replace("hours ago", Activity_Context.GetText(Resource.String.Lbl_hours));
                else if (time.Contains("days ago"))
                    holder.About.Text = Activity_Context.GetText(Resource.String.Lbl_Last_seen) + " " +
                                        time.Replace("days ago", Activity_Context.GetText(Resource.String.Lbl_days));
                else if (time.Contains("month ago"))
                    holder.About.Text = Activity_Context.GetText(Resource.String.Lbl_Last_seen) + " " +
                                        time.Replace("month ago", Activity_Context.GetText(Resource.String.Lbl_month));
                else if (time.Contains("months ago"))
                    holder.About.Text = Activity_Context.GetText(Resource.String.Lbl_Last_seen) + " " +
                                        time.Replace("months ago", Activity_Context.GetText(Resource.String.Lbl_month));
                else if (time.Contains("day ago"))
                    holder.About.Text = Activity_Context.GetText(Resource.String.Lbl_Last_seen) + " " +
                                        time.Replace("day ago", Activity_Context.GetText(Resource.String.Lbl_days));
                else if (time.Contains("minutes ago"))
                    holder.About.Text = Activity_Context.GetText(Resource.String.Lbl_Last_seen) + " " +
                                        time.Replace("minutes ago",
                                            Activity_Context.GetText(Resource.String.Lbl_minutes));
                else if (time.Contains("minute ago"))
                    holder.About.Text = Activity_Context.GetText(Resource.String.Lbl_Last_seen) + " " +
                                        time.Replace("minute ago",
                                            Activity_Context.GetText(Resource.String.Lbl_minutes));
                else if (time.Contains("seconds ago"))
                    holder.About.Text = Activity_Context.GetText(Resource.String.Lbl_Last_seen) + " " +
                                        time.Replace("seconds ago",
                                            Activity_Context.GetText(Resource.String.Lbl_seconds));
                else if (time.Contains("hour ago"))
                    holder.About.Text = Activity_Context.GetText(Resource.String.Lbl_Last_seen) + " " +
                                        time.Replace("hour ago", Activity_Context.GetText(Resource.String.Lbl_hours));
                else
                    holder.About.Text = Activity_Context.GetText(Resource.String.Lbl_Last_seen) + " " + time;

                if (item.is_following == "1") // My Friend
                {
                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                    if (Settings.ConnectivitySystem == "1") // Following
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Following);
                    else // Friend
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Friends);

                    holder.Button.Tag = "friends";
                }
                else if (item.is_following == "2") // Request
                {
                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    holder.Button.SetTextColor(Color.ParseColor("#444444"));
                    holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Request);
                    holder.Button.Tag = "Request";
                }
                else if (item.is_following == "0") //Not Friend
                {
                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    holder.Button.SetTextColor(Color.ParseColor(Settings.MainColor));
                    if (Settings.ConnectivitySystem == "1") // Following
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Follow);
                    else // Friend
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_AddFriends);
                    holder.Button.Tag = "false";

                    var dbDatabase = new SqLiteDatabase();
                    dbDatabase.Delete_UsersContact(item.UserId);
                    dbDatabase.Dispose();
                }
                else
                {
                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                    if (Settings.ConnectivitySystem == "1") // Following
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Following);
                    else // Friend
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Friends);
                }

                if (!holder.Button.HasOnClickListeners)
                    holder.Button.Click +=  (sender, args) => 
                    {
                        try
                        {
                            if (!IMethods.CheckConnectivity())
                            {
                                Toast.MakeText(Activity_Context,Activity_Context.GetString(Resource.String.Lbl_CheckYourInternetConnection),ToastLength.Short).Show();
                            }
                            else
                            {
                                if (holder.Button.Tag.ToString() == "false")
                                {
                                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                                    holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                                    if (Settings.ConnectivitySystem == "1") // Following
                                    {
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Following);
                                        holder.Button.Tag = "true";
                                    }
                                    else // Request Friend 
                                    {
                                        holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                                        holder.Button.SetTextColor(Color.ParseColor("#444444"));
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Request);
                                        holder.Button.Tag = "Request";
                                    }
                                }
                                else
                                {
                                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                                    holder.Button.SetTextColor(Color.ParseColor(Settings.MainColor));
                                    if (Settings.ConnectivitySystem == "1") // Following
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Follow);
                                    else // Friend
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_AddFriends);
                                    holder.Button.Tag = "false";

                                    var dbDatabase = new SqLiteDatabase();
                                    dbDatabase.Delete_UsersContact(item.UserId);
                                    dbDatabase.Dispose();
                                }

                                var result = Client.Global.Follow_User(item.UserId).ConfigureAwait(false);
                            }
                        }
                        catch (Exception e)
                        {
                            Crashes.TrackError(e);
                        }
                    };
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void BindEnd()
        {
            try
            {
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        // Function Users
        public void Add(Get_Search_Object.User user)
        {
            try
            {
                var check = mSearchUserList.FirstOrDefault(a => a.UserId == user.UserId);
                if (check == null)
                {
                    mSearchUserList.Add(user);
                    NotifyItemInserted(mSearchUserList.IndexOf(mSearchUserList.Last()));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Clear()
        {
            try
            {
                mSearchUserList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_Search_Object.User GetItem(int position)
        {
            return mSearchUserList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return 0;
            }
        }


        private void OnClick(SearchUser_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(SearchUser_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class SearchUser_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public SearchUser_AdapterViewHolder(View itemView, Action<SearchUser_AdapterClickEventArgs> clickListener,
            Action<SearchUser_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.card_pro_pic);
                Name = MainView.FindViewById<TextView>(Resource.Id.card_name);
                About = MainView.FindViewById<TextView>(Resource.Id.card_dist);
                Button = MainView.FindViewById<Button>(Resource.Id.cont);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new SearchUser_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new SearchUser_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Variables Basic

        public View MainView { get; }

        public event EventHandler<int> ItemClick;
        public ImageViewAsync Image { get; set; }

        public TextView Name { get; set; }
        public TextView About { get; set; }
        public Button Button { get; set; }

        #endregion
    }

    public class SearchUser_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}