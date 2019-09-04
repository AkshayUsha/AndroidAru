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
    public class MainPostAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;
        public ObservableCollection<Classes.PostType> PostTypeList = new ObservableCollection<Classes.PostType>();

        public MainPostAdapter(Context context)
        {
            try
            {
                Activity_Context = context;
                if (Settings.Show_Galery_Image)
                    PostTypeList.Add(new Classes.PostType
                    {
                        ID = 1,
                        TypeText = Activity_Context.GetText(Resource.String.Lbl_ImageGallery),
                        Image = Resource.Drawable.imageAttach,
                        ImageColor = "#00b200"
                    });
                if (Settings.Show_Galery_Video)
                    PostTypeList.Add(new Classes.PostType
                    {
                        ID = 2,
                        TypeText = Activity_Context.GetText(Resource.String.Lbl_VideoGallery),
                        Image = Resource.Drawable.ic_Attach_video,
                        ImageColor = "#D81B60"
                    });

                if (Settings.Show_Mention)
                    PostTypeList.Add(new Classes.PostType
                    {
                        ID = 3,
                        TypeText = Activity_Context.GetText(Resource.String.Lbl_MentionContact),
                        Image = Resource.Drawable.ic__Attach_tag,
                        ImageColor = ""
                    });

                if (Settings.Show_Location)
                {
                    var name = Activity_Context.GetText(Resource.String.Lbl_Location) + "/" +
                               Activity_Context.GetText(Resource.String.Lbl_Place);
                    PostTypeList.Add(new Classes.PostType
                    {
                        ID = 4,
                        TypeText = name,
                        Image = Resource.Drawable.ic__Attach_marker,
                        ImageColor = ""
                    });
                }

                if (Settings.Show_Feeling_Activity)
                {
                    var name = Activity_Context.GetText(Resource.String.Lbl_Feeling) + "/" +
                               Activity_Context.GetText(Resource.String.Lbl_Activity);

                    PostTypeList.Add(new Classes.PostType
                    {
                        ID = 5,
                        TypeText = name,
                        Image = Resource.Drawable.ic__Attach_happy,
                        ImageColor = ""
                    });
                }

                //if (Settings.Show_Activity)
                //{
                //    PostTypeList.Add(new Classes.PostType
                //    {
                //        ID = 6,
                //        TypeText = "Activity",
                //        Image = Resource.Drawable.ic_attach_Activites,
                //        ImageColor = ""
                //    });
                //}
                if (Settings.Show_Camera)
                    PostTypeList.Add(new Classes.PostType
                    {
                        ID = 6,
                        TypeText = Activity_Context.GetText(Resource.String.Lbl_Camera),
                        Image = Resource.Drawable.ic__Attach_video,
                        ImageColor = ""
                    });
                if (Settings.Show_Gif)
                    PostTypeList.Add(new Classes.PostType
                    {
                        ID = 7,
                        TypeText = Activity_Context.GetText(Resource.String.Lbl_Gif),
                        Image = Resource.Drawable.ic__Attach_gif,
                        ImageColor = ""
                    });
                if (Settings.Show_File)
                    PostTypeList.Add(new Classes.PostType
                    {
                        ID = 8,
                        TypeText = Activity_Context.GetText(Resource.String.Lbl_File),
                        Image = Resource.Drawable.ic_attach_file,
                        ImageColor = ""
                    });
                if (Settings.Show_Music)
                    PostTypeList.Add(new Classes.PostType
                    {
                        ID = 9,
                        TypeText = Activity_Context.GetText(Resource.String.Lbl_Music),
                        Image = Resource.Drawable.ic_attach_music,
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
                if (PostTypeList != null)
                    return PostTypeList.Count;
                return 0;
            }
        }

        public event EventHandler<MainPostAdapterClickEventArgs> ItemClick;
        public event EventHandler<MainPostAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_AddPost_View
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_AddPost_View, parent, false);
                var vh = new MainPostAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is MainPostAdapterViewHolder holder)
                {
                    var item = PostTypeList[_position];
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
            return PostTypeList[position];
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

        private void OnClick(MainPostAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(MainPostAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class MainPostAdapterViewHolder : RecyclerView.ViewHolder
    {
        public MainPostAdapterViewHolder(View itemView, Action<MainPostAdapterClickEventArgs> clickListener,
            Action<MainPostAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values         
                PostTypeText = (TextView) MainView.FindViewById(Resource.Id.type_name);
                PostImageIcon = (ImageView) MainView.FindViewById(Resource.Id.Iconimage);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new MainPostAdapterClickEventArgs
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

    public class MainPostAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}