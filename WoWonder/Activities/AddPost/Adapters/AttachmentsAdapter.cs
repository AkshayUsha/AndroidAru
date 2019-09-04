using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using WoWonder.Helpers;

namespace WoWonder.Activities.AddPost.Adapters
{
    public class Attachments
    {
        public int ID { get; set; } = 0;
        public string TypeAttachment { get; set; }
        public string FileUrl { get; set; }
        public string FileSimple { get; set; }
        public Stream FileStream { get; set; }
    }

    public class AttachmentsAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;
        public ObservableCollection<Attachments> AttachemntsList = new ObservableCollection<Attachments>();

        public AttachmentsAdapter(Context context)
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
                if (AttachemntsList != null)
                    return AttachemntsList.Count;
                return 0;
            }
        }

        public event EventHandler<AttachmentsAdapterClickEventArgs> ItemClick;
        public event EventHandler<AttachmentsAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Attachment_View
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Attachment_View, parent, false);
                var vh = new AttachmentsAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is AttachmentsAdapterViewHolder holder)
                {
                    var item = AttachemntsList[_position];
                    if (item != null)
                    {
                        if (item.TypeAttachment == "postPhotos")
                        {
                            holder.AttachType.Visibility = ViewStates.Gone;
                            ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", item.FileSimple, 2);
                        }
                        else if (item.TypeAttachment == "postVideo")
                        {
                            holder.AttachType.Visibility = ViewStates.Visible;
                            ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", item.FileSimple, 2);
                        }
                        else if (item.TypeAttachment == "postMusic" || item.TypeAttachment == "postFile")
                        {
                            holder.AttachType.Visibility = ViewStates.Gone;
                            ImageServiceLoader.Load_Image(holder.Image, item.FileSimple, item.FileSimple, 2);
                        }
                        else
                        {
                            holder.AttachType.Visibility = ViewStates.Gone;
                            ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", item.FileSimple, 2);
                        }

                        holder.Image_delete.Click += delegate { Remove(item); };
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        // Function 
        public void Add(Attachments item)
        {
            try
            {
                AttachemntsList.Add(item);
                NotifyItemInserted(AttachemntsList.IndexOf(AttachemntsList.Last()));
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Remove(Attachments item)
        {
            try
            {
                var Index = AttachemntsList.IndexOf(AttachemntsList.FirstOrDefault(a => a.ID == item.ID));
                if (Index != -1)
                {
                    AttachemntsList.Remove(item);
                    NotifyItemRemoved(Index);
                    NotifyItemRangeRemoved(0, ItemCount);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }


        public void RemoveAll()
        {
            try
            {
                AttachemntsList.Clear();
                NotifyDataSetChanged();
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

        public Attachments GetItem(int position)
        {
            return AttachemntsList[position];
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

        private void OnClick(AttachmentsAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(AttachmentsAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class AttachmentsAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; set; }
        public event EventHandler<int> ItemClick;

        public ImageView AttachType { get; set; }
        public ImageViewAsync Image { get; set; }
        public CircleButton Image_delete { get; set; }

        #endregion

        public AttachmentsAdapterViewHolder(View itemView, Action<AttachmentsAdapterClickEventArgs> clickListener,Action<AttachmentsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values         
                AttachType = (ImageView) MainView.FindViewById(Resource.Id.AttachType);
                Image = (ImageViewAsync) MainView.FindViewById(Resource.Id.Image);

                Image_delete = MainView.FindViewById<CircleButton>(Resource.Id.ImageCircle);


                //Create an Event
                itemView.Click += (sender, e) => clickListener(new AttachmentsAdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

    }

    public class AttachmentsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}