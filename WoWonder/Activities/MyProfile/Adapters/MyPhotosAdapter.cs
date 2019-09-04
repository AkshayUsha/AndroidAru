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
    public class MyPhotosAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_User_Albums_Object.Album> mAllMyAlbumsList =
            new ObservableCollection<Get_User_Albums_Object.Album>();

        public ObservableCollection<Get_User_Albums_Object.Album> mMyAlbumsList =
            new ObservableCollection<Get_User_Albums_Object.Album>();


        public MyPhotosAdapter(Context context)
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
                if (mMyAlbumsList != null)
                    return mMyAlbumsList.Count;
                return 0;
            }
        }

        public event EventHandler<MyPhotosAdapterClickEventArgs> ItemClick;
        public event EventHandler<MyPhotosAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Image_Simple
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Image_Simple, parent, false);
                var vh = new MyPhotosAdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is MyPhotosAdapterViewHolder holder)
                {
                    var item = mMyAlbumsList[_position];
                    if (item != null)
                    {
                        if (holder.Image.Tag?.ToString() != "loaded")
                        {
                            IMethods.Load_Image_From_Url_Normally(holder.Image, item.postFile_full);
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
                mMyAlbumsList.Add(Album);
                NotifyItemInserted(mMyAlbumsList.IndexOf(mMyAlbumsList.Last()));
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
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

        private void OnClick(MyPhotosAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(MyPhotosAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class MyPhotosAdapterViewHolder : RecyclerView.ViewHolder
    {
        public MyPhotosAdapterViewHolder(View itemView, Action<MyPhotosAdapterClickEventArgs> clickListener,
            Action<MyPhotosAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.ImageUserAlbums);

                //Event
                itemView.Click += (sender, e) => clickListener(new MyPhotosAdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new MyPhotosAdapterClickEventArgs
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

    public class MyPhotosAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}