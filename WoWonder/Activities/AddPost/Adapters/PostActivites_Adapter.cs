using System;
using System.Collections.ObjectModel;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;

namespace WoWonder.Activities.AddPost.Adapters
{
    public class PostActivites_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Classes.PostType> PostActivitesTypeList =
            new ObservableCollection<Classes.PostType>();

        public PostActivites_Adapter(Context context)
        {
            try
            {
                Activity_Context = context;

                if (Settings.Show_Listening)
                    PostActivitesTypeList.Add(new Classes.PostType
                    {
                        ID = 1,
                        TypeText = Activity_Context.GetText(Resource.String.Lbl_ListeningTo),
                        Image = Resource.Drawable.ic_attach_listening,
                        ImageColor = ""
                    });
                if (Settings.Show_Playing)
                    PostActivitesTypeList.Add(new Classes.PostType
                    {
                        ID = 2,
                        TypeText = Activity_Context.GetText(Resource.String.Lbl_Playing),
                        Image = Resource.Drawable.ic_attach_playing,
                        ImageColor = ""
                    });
                if (Settings.Show_Watching)
                    PostActivitesTypeList.Add(new Classes.PostType
                    {
                        ID = 3,
                        TypeText = Activity_Context.GetText(Resource.String.Lbl_Watching),
                        Image = Resource.Drawable.ic_attach_watching,
                        ImageColor = ""
                    });
                if (Settings.Show_Traveling)
                    PostActivitesTypeList.Add(new Classes.PostType
                    {
                        ID = 4,
                        TypeText = Activity_Context.GetText(Resource.String.Lbl_Traveling),
                        Image = Resource.Drawable.ic_attach_traveling,
                        ImageColor = ""
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
                if (PostActivitesTypeList != null)
                    return PostActivitesTypeList.Count;
                return 0;
            }
        }

        public event EventHandler<PostActivites_AdapterClickEventArgs> ItemClick;
        public event EventHandler<PostActivites_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_AddPost_View
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_AddPost_View, parent, false);
                var vh = new PostActivites_AdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is PostActivites_AdapterViewHolder holder)
                {
                    var item = PostActivitesTypeList[_position];
                    if (item != null)
                    {
                        FontController.SetFont(holder.PostTypeText, 1);
                        holder.PostTypeText.Text = item.TypeText;
                        holder.PostImageIcon.SetImageResource(item.Image);

                        if (!string.IsNullOrEmpty(item.ImageColor))
                            holder.PostImageIcon.SetColorFilter(Color.ParseColor(item.ImageColor));
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public Classes.PostType GetItem(int position)
        {
            return PostActivitesTypeList[position];
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

        private void OnClick(PostActivites_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(PostActivites_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class PostActivites_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public PostActivites_AdapterViewHolder(View itemView, Action<PostActivites_AdapterClickEventArgs> clickListener,
            Action<PostActivites_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values         
                PostTypeText = (TextView) MainView.FindViewById(Resource.Id.type_name);
                PostImageIcon = (ImageView) MainView.FindViewById(Resource.Id.Iconimage);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new PostActivites_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new PostActivites_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #region Variables Basic

        public View MainView { get; }

        public TextView PostTypeText { get; }
        public ImageView PostImageIcon { get; }

        #endregion
    }

    public class PostActivites_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}