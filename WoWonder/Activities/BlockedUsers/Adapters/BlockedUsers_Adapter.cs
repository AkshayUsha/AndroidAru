using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;

namespace WoWonder.Activities.BlockedUsers.Adapters
{
    public class BlockedUsers_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_Blocked_Users_Object.Blocked_Users> mBlockedUsersList =
            new ObservableCollection<Get_Blocked_Users_Object.Blocked_Users>();

        public BlockedUsers_Adapter(Context context)
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
                if (mBlockedUsersList != null)
                    return mBlockedUsersList.Count;
                return 0;
            }
        }

        public event EventHandler<BlockedUsers_AdapterClickEventArgs> ItemClick;
        public event EventHandler<BlockedUsers_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> BlockedUsers_view
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.BlockedUsers_view, parent, false);
                var vh = new BlockedUsers_AdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is BlockedUsers_AdapterViewHolder holder)
                {
                    var item = mBlockedUsersList[_position];
                    if (item != null)
                    {
                        FontController.SetFont(holder.Txt_Username, 1);
                        FontController.SetFont(holder.Txt_Lastseen, 3);
                        Initialize(holder, item);
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Initialize(BlockedUsers_AdapterViewHolder holder, Get_Blocked_Users_Object.Blocked_Users users)
        {
            try
            {
                var AvatarSplit = users.avatar.Split('/').Last();
                var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                if (getImage_Avatar != "File Dont Exists")
                {
                    if (holder.ImageAvatar.Tag?.ToString() != "loaded")
                    {
                        ImageServiceLoader.Load_Image(holder.ImageAvatar, "no_profile_image.png", getImage_Avatar, 1);
                        holder.ImageAvatar.Tag = "loaded";
                    }
                }
                else
                {
                    if (holder.ImageAvatar.Tag?.ToString() != "loaded")
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, users.avatar);
                        ImageServiceLoader.Load_Image(holder.ImageAvatar, "no_profile_image.png", users.avatar, 1);
                        holder.ImageAvatar.Tag = "loaded";
                    }
                }

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(users.name));

                holder.Txt_Username.Text = name;

                string lastSeen = Activity_Context.GetText(Resource.String.Lbl_Last_seen);

                var time = users.lastseen_time_text;
                if (time.Contains("hours ago"))
                    holder.Txt_Lastseen.Text = lastSeen + " " + time.Replace("hours ago", Activity_Context.GetText(Resource.String.Lbl_hours));
                else if (time.Contains("days ago"))
                    holder.Txt_Lastseen.Text = lastSeen + " " + time.Replace("days ago",Activity_Context.GetText(Resource.String.Lbl_days));
                else if (time.Contains("month ago"))            
                    holder.Txt_Lastseen.Text = lastSeen + " " + time.Replace("month ago",Activity_Context.GetText(Resource.String.Lbl_month));
                else if (time.Contains("months ago"))           
                    holder.Txt_Lastseen.Text = lastSeen + " " + time.Replace("months ago",Activity_Context.GetText(Resource.String.Lbl_month));
                else if (time.Contains("day ago"))              
                    holder.Txt_Lastseen.Text = lastSeen + " " + time.Replace("day ago",Activity_Context.GetText(Resource.String.Lbl_days));
                else if (time.Contains("minutes ago"))          
                    holder.Txt_Lastseen.Text = lastSeen + " " + time.Replace("minutes ago",Activity_Context.GetText(Resource.String.Lbl_minutes));
                else if (time.Contains("minute ago"))           
                    holder.Txt_Lastseen.Text = lastSeen + " " + time.Replace("minute ago",Activity_Context.GetText(Resource.String.Lbl_minutes));
                else if (time.Contains("seconds ago"))          
                    holder.Txt_Lastseen.Text = lastSeen + " " + time.Replace("seconds ago",Activity_Context.GetText(Resource.String.Lbl_seconds));
                else if (time.Contains("hour ago"))             
                    holder.Txt_Lastseen.Text = lastSeen + " " + time.Replace("hour ago",Activity_Context.GetText(Resource.String.Lbl_hours));
                else
                    holder.Txt_Lastseen.Text = lastSeen + " " + time;
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
        public void Add(Get_Blocked_Users_Object.Blocked_Users user)
        {
            try
            {
                var check = mBlockedUsersList.FirstOrDefault(a => a.user_id == user.user_id);
                if (check == null)
                {
                    mBlockedUsersList.Add(user);
                    NotifyItemInserted(mBlockedUsersList.IndexOf(mBlockedUsersList.Last()));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Insert(Get_Blocked_Users_Object.Blocked_Users users)
        {
            try
            {
                mBlockedUsersList.Insert(0, users);
                NotifyItemInserted(mBlockedUsersList.IndexOf(mBlockedUsersList.First()));
                NotifyItemRangeInserted(0, mBlockedUsersList.Count);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void Move(Get_Blocked_Users_Object.Blocked_Users users)
        {
            try
            {
                var data = mBlockedUsersList.FirstOrDefault(a => a.user_id == users.user_id);
                var index = mBlockedUsersList.IndexOf(data);
                if (index > -1)
                {
                    mBlockedUsersList.Move(index, 0);
                    NotifyItemMoved(index, 0);
                    NotifyItemChanged(0);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void Clear()
        {
            try
            {
                mBlockedUsersList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void Remove(Get_Blocked_Users_Object.Blocked_Users users)
        {
            try
            {
                var Index = mBlockedUsersList.IndexOf(
                    mBlockedUsersList.FirstOrDefault(a => a.user_id == users.user_id));
                if (Index != -1)
                {
                    mBlockedUsersList.Remove(users);
                    NotifyItemRemoved(Index);
                    NotifyItemRangeRemoved(0, ItemCount);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_Blocked_Users_Object.Blocked_Users GetItem(int position)
        {
            return mBlockedUsersList[position];
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

        private void OnClick(BlockedUsers_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(BlockedUsers_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class BlockedUsers_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public BlockedUsers_AdapterViewHolder(View itemView, Action<BlockedUsers_AdapterClickEventArgs> clickListener,
            Action<BlockedUsers_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                RelativeLayout_main = (RelativeLayout) MainView.FindViewById(Resource.Id.main);

                Txt_Username = (TextView) MainView.FindViewById(Resource.Id.Txt_Username);
                Txt_Lastseen = (TextView) MainView.FindViewById(Resource.Id.Txt_LastSeen);
                ImageAvatar = (ImageViewAsync) MainView.FindViewById(Resource.Id.Image_Avatar);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new BlockedUsers_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new BlockedUsers_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #region Variables Basic

        public View MainView { get; }
        private BlockedUsers_Activity Activity_Context;

        public event EventHandler<int> ItemClick;

        public RelativeLayout RelativeLayout_main { get; set; }
        public TextView Txt_Username { get; }
        public TextView Txt_Lastseen { get; }
        public ImageViewAsync ImageAvatar { get; }

        #endregion
    }

    public class BlockedUsers_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}