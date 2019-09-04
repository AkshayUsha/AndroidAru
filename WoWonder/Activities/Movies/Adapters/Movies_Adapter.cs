using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using Plugin.Share;
using Plugin.Share.Abstractions;
using WoWonder.Helpers;
using WoWonder_API.Classes.Movies;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;

namespace WoWonder.Activities.Movies.Adapters
{
    public class Movies_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_Movies_Object.Movie> mMoviesList =
            new ObservableCollection<Get_Movies_Object.Movie>();

        public Movies_Adapter(Context context)
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
                if (mMoviesList != null)
                    return mMoviesList.Count;
                return 0;
            }
        }

        public event EventHandler<Movies_AdapterClickEventArgs> ItemClick;
        public event EventHandler<Movies_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Video_View
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Video_View, parent, false);
                var vh = new Movies_AdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is Movies_AdapterViewHolder holder)
                {
                    var item = mMoviesList[_position];
                    if (item != null)
                    {
                        FontController.SetFont(holder.Txt_Title, 1);
                        FontController.SetFont(holder.Txt_Description, 3);
                        FontController.SetFont(holder.Txt_ViewsCount, 3);
                        Initialize(holder, item);
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Initialize(Movies_AdapterViewHolder holder, Get_Movies_Object.Movie movie)
        {
            try
            {
                var CoverSplit = movie.cover.Split('/').Last();
                var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskMovie, CoverSplit);
                if (getImage_Cover != "File Dont Exists")
                {
                    if (holder.VideoImage.Tag?.ToString() != "loaded")
                    {
                        ImageServiceLoader.Load_Image(holder.VideoImage, "ImagePlacholder.jpg", getImage_Cover);
                        holder.VideoImage.Tag= "loaded";
                    }
                }
                else
                {
                    if (holder.VideoImage.Tag?.ToString() != "loaded")
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskMovie, movie.cover);
                        ImageServiceLoader.Load_Image(holder.VideoImage, "ImagePlacholder.jpg", movie.cover);
                        holder.VideoImage.Tag = "loaded";
                    }
                }

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(movie.name));
                holder.Txt_Title.Text = name;
                holder.Txt_Description.Text = IMethods.Fun_String.SubStringCutOf(movie.description, 50);
                holder.Txt_duration.Text = movie.duration + " " + Activity_Context.GetText(Resource.String.Lbl_Min);
                holder.Txt_ViewsCount.Text = movie.views + " " + Activity_Context.GetText(Resource.String.Lbl_Views);

                IMethods.Set_TextViewIcon("1", holder.MenueView, IonIcons_Fonts.AndroidMoreVertical);

                if (!holder.MenueView.HasOnClickListeners)
                    holder.MenueView.Click +=  (sender, args) => 
                    {
                        try
                        {
                            var ctw = new ContextThemeWrapper(Activity_Context, Resource.Style.PopupMenuStyle);
                            var popup = new PopupMenu(ctw, holder.MenueView);
                            popup.MenuInflater.Inflate(Resource.Menu.MoreCommunities_NotEdit_Menu, popup.Menu);
                            popup.Show();
                            popup.MenuItemClick += (o, eventArgs) =>
                            {
                                var Id = eventArgs.Item.ItemId;
                                switch (Id)
                                {
                                    case Resource.Id.menu_CopeLink:
                                        OnCopeLink_Button_Click(movie);
                                        break;

                                    case Resource.Id.menu_Share:
                                        OnShare_Button_Click(movie);
                                        break;
                                }
                            };
                        }
                        catch (Exception e)
                        {
                            Crashes.TrackError(e);
                        }
                    };
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Menu >> Cope Link
        private void OnCopeLink_Button_Click(Get_Movies_Object.Movie movie)
        {
            try
            {
                var clipboardManager = (ClipboardManager) Activity_Context.GetSystemService(Context.ClipboardService);

                var clipData = ClipData.NewPlainText("text", movie.url);
                clipboardManager.PrimaryClip = clipData;

                Toast.MakeText(Activity_Context, Activity_Context.GetText(Resource.String.Lbl_Copied),
                    ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Menu >> Share
        private async void OnShare_Button_Click(Get_Movies_Object.Movie movie)
        {
            try
            {
                //Share Plugin same as video
                if (!CrossShare.IsSupported) return;

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = movie.name,
                    Text = movie.description,
                    Url = movie.url
                });
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

        // Function movie
        public void Add(Get_Movies_Object.Movie movie)
        {
            try
            {
                var check = mMoviesList.FirstOrDefault(a => a.id == movie.id);
                if (check == null)
                {
                    mMoviesList.Add(movie);
                    NotifyItemInserted(mMoviesList.IndexOf(mMoviesList.Last()));
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
                mMoviesList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public Get_Movies_Object.Movie GetItem(int position)
        {
            return mMoviesList[position];
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

        private void OnClick(Movies_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(Movies_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class Movies_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public Movies_AdapterViewHolder(View itemView, Action<Movies_AdapterClickEventArgs> clickListener,
            Action<Movies_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                VideoImage = (ImageViewAsync) MainView.FindViewById(Resource.Id.Imagevideo);
                Txt_duration = MainView.FindViewById<TextView>(Resource.Id.duration);
                Txt_Title = MainView.FindViewById<TextView>(Resource.Id.Title);
                Txt_Description = MainView.FindViewById<TextView>(Resource.Id.description);
                Txt_ViewsCount = MainView.FindViewById<TextView>(Resource.Id.Views_Count);
                MenueView = MainView.FindViewById<TextView>(Resource.Id.videoMenue);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new Movies_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new Movies_AdapterClickEventArgs
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

        public ImageViewAsync VideoImage { get; set; }
        public TextView Txt_duration { get; set; }
        public TextView Txt_Title { get; set; }
        public TextView Txt_Description { get; set; }
        public TextView Txt_ViewsCount { get; set; }
        public TextView MenueView { get; set; }

        #endregion
    }

    public class Movies_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}