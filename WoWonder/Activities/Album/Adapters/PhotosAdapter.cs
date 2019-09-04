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

namespace WoWonder.Activities.Album.Adapters
{
    public class PhotosAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_User_Albums_Object.Album> mMyAlbumsList =
            new ObservableCollection<Get_User_Albums_Object.Album>();

        public PhotosAdapter(Context context)
        {
            Activity_Context = context;
        }

        public override int ItemCount
        {
            get
            {
                if (mMyAlbumsList != null)
                    return mMyAlbumsList.Count;
                return 0;
            }
        }

        public event EventHandler<GroupsAdapteClickEventArgs> ItemClick;
        public event EventHandler<GroupsAdapteClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_ImageAlbum_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_ImageAlbum_view, parent, false);
                var vh = new PhotosAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is PhotosAdapterViewHolder holder)
                {
                    var item = mMyAlbumsList[_position];
                    if (item != null)
                    {
                        if (holder.Image.Tag?.ToString() != "loaded")
                        {
                            ImageServiceLoader.Load_ImageResampled(holder.Image, "Grey_Offline.jpg", item.postFile_full, 2,false, 5, true);
                            holder.Image.Tag = "loaded";
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
        public void Add(Get_User_Albums_Object.Album Album)
        {
            try
            {
                var check = mMyAlbumsList.FirstOrDefault(a => a.post_id == Album.post_id);
                if (check == null)
                {
                    mMyAlbumsList.Add(Album);
                    NotifyItemInserted(mMyAlbumsList.IndexOf(mMyAlbumsList.Last()));
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
                mMyAlbumsList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_User_Albums_Object.Album GetItem(int position)
        {
            return mMyAlbumsList[position];
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

        private void OnClick(GroupsAdapteClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(GroupsAdapteClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class PhotosAdapterViewHolder : RecyclerView.ViewHolder
    {
        public PhotosAdapterViewHolder(View itemView, Action<GroupsAdapteClickEventArgs> clickListener,
            Action<GroupsAdapteClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.Image);

                //Event
                itemView.Click += (sender, e) => clickListener(new GroupsAdapteClickEventArgs
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

    public class GroupsAdapteClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}