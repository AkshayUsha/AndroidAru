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
    public class UserPhotosAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_User_Albums_Object.Album> mAllUserAlbumsList =
            new ObservableCollection<Get_User_Albums_Object.Album>();

        public ObservableCollection<Get_User_Albums_Object.Album> mUserAlbumsList =
            new ObservableCollection<Get_User_Albums_Object.Album>();

        public UserPhotosAdapter(Context context)
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
                if (mUserAlbumsList != null)
                    return mUserAlbumsList.Count;
                return 0;
            }
        }

        public event EventHandler<UserPhotosAdapterClickEventArgs> ItemClick;
        public event EventHandler<UserPhotosAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Image_Simple
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Image_Simple, parent, false);
                var vh = new UserPhotosAdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is UserPhotosAdapterViewHolder holder)
                {
                    var item = mUserAlbumsList[_position];
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
                mUserAlbumsList.Add(Album);
                NotifyItemInserted(mUserAlbumsList.IndexOf(mUserAlbumsList.Last()));
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public Get_User_Albums_Object.Album GetItem(int position)
        {
            return mUserAlbumsList[position];
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

        private void OnClick(UserPhotosAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(UserPhotosAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class UserPhotosAdapterViewHolder : RecyclerView.ViewHolder
    {
        public UserPhotosAdapterViewHolder(View itemView, Action<UserPhotosAdapterClickEventArgs> clickListener,
            Action<UserPhotosAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.ImageUserAlbums);

                //Event
                itemView.Click += (sender, e) => clickListener(new UserPhotosAdapterClickEventArgs
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

    public class UserPhotosAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}