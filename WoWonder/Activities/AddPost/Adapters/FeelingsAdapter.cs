using System;
using System.Collections.ObjectModel;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using WoWonder.Helpers;

namespace WoWonder.Activities.AddPost.Adapters
{
    public class Feeling
    {
        public int ID { get; set; }
        public string FeelingText { get; set; }
        public string FeelingImageURL { get; set; }
        public bool Selected { get; set; } = false;
    }

    public class FeelingsAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;
        public ObservableCollection<Feeling> FeelingsList = new ObservableCollection<Feeling>();

        public FeelingsAdapter(Context context)
        {
            try
            {
                Activity_Context = context;

                FeelingsList.Add(new Feeling
                {
                    ID = 1, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Angry), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f621.png"
                });
                FeelingsList.Add(new Feeling
                {
                    ID = 2, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Funny), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f602.png"
                });
                FeelingsList.Add(new Feeling
                {
                    ID = 3, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Loved), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f60d.png"
                });
                FeelingsList.Add(new Feeling
                    {ID = 4, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Cool), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f60e.png"});
                FeelingsList.Add(new Feeling
                {
                    ID = 5, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Happy), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f603.png"
                });
                FeelingsList.Add(new Feeling
                {
                    ID = 6, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Tired), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f62b.png"
                });
                FeelingsList.Add(new Feeling
                {
                    ID = 7, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Sleepy), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f634.png"
                });
                FeelingsList.Add(new Feeling
                {
                    ID = 8, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Expressionless),FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f611.png"
                });
                FeelingsList.Add(new Feeling
                {
                    ID = 9, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Confused), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f615.png"
                });
                FeelingsList.Add(new Feeling
                {
                    ID = 10, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Shocked), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f631.png"
                });
                FeelingsList.Add(new Feeling
                {
                    ID = 11, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_VerySad),FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f62d.png"
                });
                FeelingsList.Add(new Feeling
                {
                    ID = 12, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Blessed), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f607.png"
                });
                FeelingsList.Add(new Feeling
                {
                    ID = 13, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Bored), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f610.png"
                });
                FeelingsList.Add(new Feeling
                {
                    ID = 14, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Broken), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f494.png"
                });
                FeelingsList.Add(new Feeling
                {
                    ID = 15, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Lovely), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/2665.png"
                });
                FeelingsList.Add(new Feeling
                {
                    ID = 16, FeelingText = this.Activity_Context.GetText(Resource.String.Lbl_Hot), FeelingImageURL = "https://abs.twimg.com/emoji/v1/72x72/1f60f.png"
                });
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
                if (FeelingsList != null)
                    return FeelingsList.Count;
                return 0;
            }
        }

        public event EventHandler<FeelingsAdapterClickEventArgs> ItemClick;
        public event EventHandler<FeelingsAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Feeling_View
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Feeling_View, parent, false);
                var vh = new FeelingsAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is FeelingsAdapterViewHolder holder)
                {
                    var item = FeelingsList[_position];
                    if (item != null)
                    {
                        FontController.SetFont(holder.FeelingName, 1);
                        holder.FeelingName.Text = item.FeelingText;

                        if (!string.IsNullOrEmpty(item.FeelingImageURL))
                        {
                            if (holder.Image.Tag?.ToString() != "loaded")
                            {
                                ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", item.FeelingImageURL, 2);
                                holder.Image.Tag = "loaded";
                            }
                        }
                            
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public Feeling GetItem(int position)
        {
            return FeelingsList[position];
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

        private void OnClick(FeelingsAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(FeelingsAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class FeelingsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public FeelingsAdapterViewHolder(View itemView, Action<FeelingsAdapterClickEventArgs> clickListener,Action<FeelingsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values         
                FeelingName = (TextView) MainView.FindViewById(Resource.Id.feelingName);
                Image = (ImageViewAsync) MainView.FindViewById(Resource.Id.Image);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new FeelingsAdapterClickEventArgs
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

        public TextView FeelingName { get; }
        public ImageViewAsync Image { get; }

        #endregion
    }

    public class FeelingsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}