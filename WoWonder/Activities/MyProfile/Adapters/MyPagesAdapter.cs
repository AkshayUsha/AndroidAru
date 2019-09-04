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
    public class MyPagesAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_User_Data_Object.Liked_Pages> mAllMyPagesList =
            new ObservableCollection<Get_User_Data_Object.Liked_Pages>();


        public MyPagesAdapter(Context context)
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
                if (mAllMyPagesList != null)
                    return mAllMyPagesList.Count;
                return 0;
            }
        }

        public event EventHandler<MyPagesAdapterClickEventArgs> ItemClick;
        public event EventHandler<MyPagesAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_PageCircle_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_PageCircle_view, parent, false);
                var vh = new MyPagesAdapterViewHolder(itemView, OnClick, OnLongClick);
                return vh;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                _position = position;

                if (viewHolder is MyPagesAdapterViewHolder holder)
                {
                    var item = mAllMyPagesList[_position];
                    if (item != null)
                    {
                        //Dont Remove this code #####
                        FontController.SetFont(holder.Name, 1);
                        IMethods.Set_TextViewIcon("1", holder.IconPage, IonIcons_Fonts.IosFlag);
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

        public void Initialize(MyPagesAdapterViewHolder holder, Get_User_Data_Object.Liked_Pages item)
        {
            try
            { 
                var AvatarSplit = item.avatar.Split('/').Last();
                var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, AvatarSplit);
                if (getImage_Avatar != "File Dont Exists")
                {
                    if (holder.Image.Tag?.ToString() != "loaded")
                    {
                        ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", getImage_Avatar, 1);
                        holder.Image.Tag = "loaded";
                    }
                }
                else
                {
                    if (holder.Image.Tag?.ToString() != "loaded")
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage, item.avatar);
                        ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", item.avatar, 1);
                        holder.Image.Tag = "loaded";
                    }
                }

                var CoverSplit = item.cover.Split('/').Last();
                var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, CoverSplit);
                if (getImage_Cover != "File Dont Exists")
                {
                    //ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", getImage_Cover);
                }
                else
                {
                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage, item.cover);
                    // ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", result.user_data.cover);
                }

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(item.page_name));
                holder.Name.Text = name;

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
        public void Add(Get_User_Data_Object.Liked_Pages page)
        {
            try
            {
                var check = mAllMyPagesList.FirstOrDefault(a => a.page_id == page.page_id);
                if (check == null)
                {
                    mAllMyPagesList.Add(page);
                    NotifyItemInserted(mAllMyPagesList.IndexOf(mAllMyPagesList.Last()));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public Get_User_Data_Object.Liked_Pages GetItem(int position)
        {
            return mAllMyPagesList[position];
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

        private void OnClick(MyPagesAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(MyPagesAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class MyPagesAdapterViewHolder : RecyclerView.ViewHolder
    {
        public MyPagesAdapterViewHolder(View itemView, Action<MyPagesAdapterClickEventArgs> clickListener,
            Action<MyPagesAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.Image);
                Name = MainView.FindViewById<TextView>(Resource.Id.Name);
                IconPage = MainView.FindViewById<TextView>(Resource.Id.Icon);

                //Event
                itemView.Click += (sender, e) => clickListener(new MyPagesAdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new MyPagesAdapterClickEventArgs
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
        public TextView IconPage { get; set; }

        #endregion
    }

    public class MyPagesAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}