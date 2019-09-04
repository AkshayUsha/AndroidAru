using System;
using System.Collections.ObjectModel;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using WoWonder.Helpers;

namespace WoWonder.Activities.AddPost.Adapters
{
    public class GifAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;
        public ObservableCollection<GifGiphyClass.Datum> GifList = new ObservableCollection<GifGiphyClass.Datum>();

        public GifAdapter(Context context)
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
                if (GifList != null)
                    return GifList.Count;
                return 0;
            }
        }

        public event EventHandler<GifAdapterAdapterClickEventArgs> ItemClick;
        public event EventHandler<GifAdapterAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Gif_View
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Gif_View, parent, false);
                var vh = new GifAdapterViewHolder(itemView, OnClick, OnLongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return null;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                _position = position;
                if (viewHolder is GifAdapterViewHolder holder)
                {
                    var item = GifList[_position];
                    if (!string.IsNullOrEmpty(item?.images?.preview_gif.url))
                    {
                        if (holder.Image.Tag?.ToString() != "loaded")
                        {
                            ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", item.images.preview_gif.url);
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


        public void Clear()
        {
            try
            {
                GifList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public GifGiphyClass.Datum GetItem(int position)
        {
            return GifList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return base.GetItemId(position);
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
                return base.GetItemViewType(position);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return 0;
            }
        }

        private void OnClick(GifAdapterAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(GifAdapterAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class GifAdapterViewHolder : RecyclerView.ViewHolder
    {
        public GifAdapterViewHolder(View itemView, Action<GifAdapterAdapterClickEventArgs> clickListener,
            Action<GifAdapterAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;
                Image = (ImageViewAsync) MainView.FindViewById(Resource.Id.Image);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new GifAdapterAdapterClickEventArgs
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


        public ImageViewAsync Image { get; }

        #endregion
    }

    public class GifAdapterAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}