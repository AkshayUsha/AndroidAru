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

namespace WoWonder.Activities.userProfile.Adapters
{
    public class UserFriendsAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public static ObservableCollection<Get_User_Data_Object.Followers> mAllUserFriendsList =
            new ObservableCollection<Get_User_Data_Object.Followers>(); // Followers Or Friends

        public ObservableCollection<Get_User_Data_Object.Followers> mUserFriendsList =
            new ObservableCollection<Get_User_Data_Object.Followers>(); // Followers Or Friends

        public UserFriendsAdapter(Context context)
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
                if (mUserFriendsList != null)
                    return mUserFriendsList.Count;
                return 0;
            }
        }

        public event EventHandler<UserFriendsAdapterClickEventArgs> ItemClick;
        public event EventHandler<UserFriendsAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_ImageCircle_Simple
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_ImageCircle_Simple, parent, false);
                var vh = new UserFriendsAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is UserFriendsAdapterViewHolder holder)
                {
                    var item = mUserFriendsList[_position];
                    if (item != null)
                    {
                        var AvatarSplit = item.avatar.Split('/').Last();
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
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
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
        public void Add(Get_User_Data_Object.Followers user)
        {
            try
            {
                var check = mUserFriendsList.FirstOrDefault(a => a.user_id == user.user_id);
                if (check == null)
                {
                    mUserFriendsList.Add(user);
                    NotifyItemInserted(mUserFriendsList.IndexOf(mUserFriendsList.Last()));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public Get_User_Data_Object.Followers GetItem(int position)
        {
            return mUserFriendsList[position];
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

        private void OnClick(UserFriendsAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(UserFriendsAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class UserFriendsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public UserFriendsAdapterViewHolder(View itemView, Action<UserFriendsAdapterClickEventArgs> clickListener,
            Action<UserFriendsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.ImageUsers);

                //Event
                itemView.Click += (sender, e) => clickListener(new UserFriendsAdapterClickEventArgs
                {
                    View = itemView,
                    Position = AdapterPosition
                });
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

    public class UserFriendsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}