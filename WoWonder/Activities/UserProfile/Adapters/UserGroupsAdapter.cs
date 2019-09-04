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

namespace WoWonder.Activities.userProfile.Adapters
{
    public class UserGroupsAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_User_Data_Object.Joined_Groups> mAllUserGroupsList =
            new ObservableCollection<Get_User_Data_Object.Joined_Groups>();

        public ObservableCollection<Get_User_Data_Object.Joined_Groups> mUserGroupsList =
            new ObservableCollection<Get_User_Data_Object.Joined_Groups>();

        public UserGroupsAdapter(Context context)
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
                if (mUserGroupsList != null)
                    return mUserGroupsList.Count;
                return 0;
            }
        }

        public event EventHandler<UserGroupsAdapterClickEventArgs> ItemClick;
        public event EventHandler<UserGroupsAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Group_Simple
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Group_Simple, parent, false);
                var vh = new UserGroupsAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is UserGroupsAdapterViewHolder holder)
                {
                    var item = mUserGroupsList[_position];
                    if (item != null)
                    {
                        var AvatarSplit = item.avatar.Split('/').Last();
                        var getImage_Avatar =
                            IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskGroup, AvatarSplit);
                        if (getImage_Avatar != "File Dont Exists")
                        {
                            if (holder.Image.Tag?.ToString() != "loaded")
                            {
                                ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", item.avatar);
                                holder.Image.Tag = "loaded";
                            }
                        }
                        else
                        {
                            if (holder.Image.Tag?.ToString() != "loaded")
                            {
                                IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskGroup, item.avatar);
                                ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", item.avatar);
                                holder.Image.Tag = "loaded";
                            }
                        }

                        var CoverSplit = item.cover.Split('/').Last();
                        var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskGroup, CoverSplit);
                        if (getImage_Cover != "File Dont Exists")
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskGroup, item.cover);
                        }

                        string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(item.name));
                        holder.Nmae.Text = IMethods.Fun_String.SubStringCutOf(name, 20);
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

        // Function Video
        public void Add(Get_User_Data_Object.Joined_Groups groups)
        {
            try
            {
                var check = mUserGroupsList.FirstOrDefault(a => a.group_id == groups.group_id);
                if (check == null)
                {
                    mUserGroupsList.Add(groups);
                    NotifyItemInserted(mUserGroupsList.IndexOf(mUserGroupsList.Last()));
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
                mUserGroupsList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_User_Data_Object.Joined_Groups GetItem(int position)
        {
            return mUserGroupsList[position];
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

        private void OnClick(UserGroupsAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(UserGroupsAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class UserGroupsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public UserGroupsAdapterViewHolder(View itemView, Action<UserGroupsAdapterClickEventArgs> clickListener,
            Action<UserGroupsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.ImageGroup);
                Nmae = MainView.FindViewById<TextView>(Resource.Id.Group_titile);

                //Event
                itemView.Click += (sender, e) => clickListener(new UserGroupsAdapterClickEventArgs
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
        public TextView Nmae { get; set; }

        #endregion
    }

    public class UserGroupsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}