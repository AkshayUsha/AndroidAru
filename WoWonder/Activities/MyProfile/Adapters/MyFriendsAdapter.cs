using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;

namespace WoWonder.Activities.MyProfile.Adapters
{
    public class MyFriendsAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_User_Data_Object.Following> mAllMyFriendsList =
            new ObservableCollection<Get_User_Data_Object.Following>(); // Following Or Friends

        public ObservableCollection<Get_User_Data_Object.Following> mMyFriendsList =
            new ObservableCollection<Get_User_Data_Object.Following>(); // Following Or Friends

        public MyFriendsAdapter(Context context)
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
                if (mMyFriendsList != null)
                    return mMyFriendsList.Count;
                return 0;
            }
        }

        public event EventHandler<MyFriendsAdapterClickEventArgs> ItemClick;
        public event EventHandler<MyFriendsAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_ImageCircle_Simple
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_ImageCircle_Simple, parent, false);
                var vh = new MyFriendsAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is MyFriendsAdapterViewHolder holder)
                {
                    var item = mMyFriendsList[_position];
                    if (item != null) Initialize(holder, item);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Initialize(MyFriendsAdapterViewHolder holder, Get_User_Data_Object.Following item)
        {
            try
            {
                var AvatarSplit = item.avatar.Split('/').Last();
                var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
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
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, item.avatar);
                        ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", item.avatar, 1);
                        holder.Image.Tag = "loaded";
                    }
                }

                var CoverSplit = item.cover.Split('/').Last();
                var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, CoverSplit);
                if (getImage_Cover == "File Dont Exists")
                {
                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, item.cover);
                } 
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

        // Function Video
        public void Add(Get_User_Data_Object.Following user)
        {
            try
            {
                var check = mMyFriendsList.FirstOrDefault(a => a.user_id == user.user_id);
                if (check == null)
                {
                    mMyFriendsList.Add(user);
                    NotifyItemInserted(mMyFriendsList.IndexOf(mMyFriendsList.Last()));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public Get_User_Data_Object.Following GetItem(int position)
        {
            return mMyFriendsList[position];
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

        private void OnClick(MyFriendsAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(MyFriendsAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class MyFriendsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public MyFriendsAdapterViewHolder(View itemView, Action<MyFriendsAdapterClickEventArgs> clickListener,
            Action<MyFriendsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.ImageUsers);

                //Event
                itemView.Click += (sender, e) => clickListener(new MyFriendsAdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new MyFriendsAdapterClickEventArgs
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

        #endregion
    }

    public class MyFriendsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}