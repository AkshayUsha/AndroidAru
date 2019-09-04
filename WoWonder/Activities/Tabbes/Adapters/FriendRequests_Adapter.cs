using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using WoWonder.Activities.FriendRequest;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;

namespace WoWonder.Activities.Tabbes.Adapters
{
    public class FriendRequests_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_General_Data_Object.Friend_Requests> mFriendRequestsList =
            new ObservableCollection<Get_General_Data_Object.Friend_Requests>();

        public FriendRequests_Adapter(Context context)
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
                if (mFriendRequestsList != null)
                    return mFriendRequestsList.Count;
                return 0;
            }
        }

        public event EventHandler<FriendRequests_AdapterClickEventArgs> ItemClick;
        public event EventHandler<FriendRequests_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> BlockedUsers_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.BlockedUsers_view, parent, false);
                var vh = new FriendRequests_AdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is FriendRequests_AdapterViewHolder holder)
                {
                    var item = mFriendRequestsList[_position];
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

        public void Initialize(FriendRequests_AdapterViewHolder holder, Get_General_Data_Object.Friend_Requests users)
        {
            try
            {
                var AvatarSplit = users.avatar.Split('/').Last();
                var getImage_Avatar =
                    IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
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
                holder.Txt_Lastseen.Text = IMethods.ITime.TimeAgo(int.Parse(users.lastseen_unix_time));
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

        // Function 
        public void Add(Get_General_Data_Object.Friend_Requests user)
        {
            try
            {
                var check = mFriendRequestsList.FirstOrDefault(a => a.user_id == user.user_id);
                if (check == null)
                {
                    mFriendRequestsList.Add(user);
                    NotifyItemInserted(mFriendRequestsList.IndexOf(mFriendRequestsList.Last()));
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
                mFriendRequestsList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void Remove(Get_General_Data_Object.Friend_Requests users)
        {
            try
            {
                var Index = mFriendRequestsList.IndexOf(
                    mFriendRequestsList.FirstOrDefault(a => a.user_id == users.user_id));
                if (Index != -1)
                {
                    mFriendRequestsList.Remove(users);
                    NotifyItemRemoved(Index);
                    NotifyItemRangeRemoved(0, ItemCount);
                }

                if (mFriendRequestsList.Count == 0)
                {
                    FriendRequest_Activity.FriendRequestRecyler.Visibility = ViewStates.Gone;
                    FriendRequest_Activity.FriendRequest_Empty.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_General_Data_Object.Friend_Requests GetItem(int position)
        {
            return mFriendRequestsList[position];
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

        private void OnClick(FriendRequests_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(FriendRequests_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class FriendRequests_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public FriendRequests_AdapterViewHolder(View itemView,
            Action<FriendRequests_AdapterClickEventArgs> clickListener,
            Action<FriendRequests_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Txt_Username = (TextView) MainView.FindViewById(Resource.Id.Txt_Username);
                Txt_Lastseen = (TextView) MainView.FindViewById(Resource.Id.Txt_LastSeen);
                ImageAvatar = (ImageViewAsync) MainView.FindViewById(Resource.Id.Image_Avatar);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new FriendRequests_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new FriendRequests_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #region Variables Basic

        public View MainView { get; }

        public event EventHandler<int> ItemClick;

        public TextView Txt_Username { get; }
        public TextView Txt_Lastseen { get; }
        public ImageViewAsync ImageAvatar { get; }

        #endregion
    }

    public class FriendRequests_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}