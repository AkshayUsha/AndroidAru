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

namespace WoWonder.Activities.MyProfile.Adapters
{
    public class MyGroupsAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_User_Data_Object.Joined_Groups> mAllMyGroupsList =
            new ObservableCollection<Get_User_Data_Object.Joined_Groups>();

        public ObservableCollection<Get_User_Data_Object.Joined_Groups> mMyGroupsList =
            new ObservableCollection<Get_User_Data_Object.Joined_Groups>();


        public MyGroupsAdapter(Context context)
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
                if (mMyGroupsList != null)
                    return mMyGroupsList.Count;
                return 0;
            }
        }

        public event EventHandler<MyGroupsAdapterClickEventArgs> ItemClick;
        public event EventHandler<MyGroupsAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Group_Simple
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Group_Simple, parent, false);
                var vh = new MyGroupsAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is MyGroupsAdapterViewHolder holder)
                {
                    var item = mMyGroupsList[_position];
                    if (item != null) Initialize(holder, item);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Initialize(MyGroupsAdapterViewHolder holder, Get_User_Data_Object.Joined_Groups item)
        {
            try
            {
                var AvatarSplit = item.avatar.Split('/').Last();
                var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskGroup, AvatarSplit);
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
                if (getImage_Cover == "File Dont Exists")
                {
                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskGroup, item.cover);
                }

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(item.name));
                holder.Nmae.Text = IMethods.Fun_String.SubStringCutOf(name, 20);
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
        public void Add(Get_User_Data_Object.Joined_Groups groups)
        {
            try
            {
                var check = mMyGroupsList.FirstOrDefault(a => a.group_id == groups.group_id);
                if (check == null)
                {
                    mMyGroupsList.Add(groups);
                    NotifyItemInserted(mMyGroupsList.IndexOf(mMyGroupsList.Last()));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public Get_User_Data_Object.Joined_Groups GetItem(int position)
        {
            return mMyGroupsList[position];
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

        private void OnClick(MyGroupsAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(MyGroupsAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class MyGroupsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public MyGroupsAdapterViewHolder(View itemView, Action<MyGroupsAdapterClickEventArgs> clickListener,
            Action<MyGroupsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.ImageGroup);
                Nmae = MainView.FindViewById<TextView>(Resource.Id.Group_titile);

                //Event
                itemView.Click += (sender, e) => clickListener(new MyGroupsAdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new MyGroupsAdapterClickEventArgs
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

    public class MyGroupsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}