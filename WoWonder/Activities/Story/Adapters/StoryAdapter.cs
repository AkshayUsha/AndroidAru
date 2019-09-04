using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using Refractored.Controls;
using SettingsConnecter;
using WoWonder.Helpers;
using WoWonder_API.Classes.Story;

namespace WoWonder.Activities.Story.Adapters
{
    public class StoryAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_Stories_Object.Story> mStorylList = new ObservableCollection<Get_Stories_Object.Story>();

        public StoryAdapter(Context context)
        {
            Activity_Context = context;
        }

        public override int ItemCount
        {
            get
            {
                if (mStorylList != null)
                    return mStorylList.Count;
                return 0;
            }
        }

        public event EventHandler<StoryAdapterClickEventArgs> ItemClick;
        public event EventHandler<StoryAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Story_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Story_view, parent, false);
                var vh = new StoryAdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is StoryAdapterViewHolder holder)
                {
                    var item = mStorylList[_position];
                    if (item != null)
                    {
                        //Dont Remove this code #####
                        FontController.SetFont(holder.Name, 1);
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

        public void Initialize(StoryAdapterViewHolder holder, Get_Stories_Object.Story Story)
        {
            try
            { 
                var StorySplit = Story.thumbnail.Split('/').Last();
                var getImage_Story = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskStory, StorySplit);
                if (getImage_Story != "File Dont Exists")
                {
                    if (holder.Image.Tag?.ToString() != "loaded")
                    {
                        ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", getImage_Story, 1, true, 10);
                        holder.Image.Tag = "loaded";
                    }
                }
                else
                {
                    if (holder.Image.Tag?.ToString() != "loaded")
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskStory, Story.thumbnail);
                        ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", Story.thumbnail, 1, true, 10);
                        holder.Image.Tag = "loaded";
                    }
                }

                holder.Circleindicator.BorderColor = Color.ParseColor(Settings.MainColor);

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(Story.user_data.name));
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

        // Function Video
        public void Add(Get_Stories_Object.Story Story)
        {
            try
            {
                var check = mStorylList.FirstOrDefault(a => a.id == Story.id);
                if (check == null)
                {
                    mStorylList.Add(Story);
                    NotifyItemInserted(mStorylList.IndexOf(mStorylList.Last()));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Remove(Get_Stories_Object.Story Story)
        {
            try
            {
                var Index = mStorylList.IndexOf(mStorylList.FirstOrDefault(a => a.id == Story.id));
                if (Index != -1)
                {
                    mStorylList.Remove(Story);
                    NotifyItemRemoved(Index);
                    NotifyItemRangeRemoved(0, ItemCount);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void Clear()
        {
            try
            {
                mStorylList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void Update()
        {
            try
            {
                var data_Stories = mStorylList.Where(a => a.Profile_indicator.Contains(Settings.Story_Read_Color))
                    .ToList();
                if (data_Stories.Count > 0)
                {
                    var cheker = data_Stories.FirstOrDefault(a => a.Profile_indicator == Settings.Story_Read_Color);
                    NotifyItemChanged(data_Stories.IndexOf(cheker));
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_Stories_Object.Story GetItem(int position)
        {
            return mStorylList[position];
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

        private void OnClick(StoryAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(StoryAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class StoryAdapterViewHolder : RecyclerView.ViewHolder
    {
        public StoryAdapterViewHolder(View itemView, Action<StoryAdapterClickEventArgs> clickListener,
            Action<StoryAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.userProfileImage);
                Name = MainView.FindViewById<TextView>(Resource.Id.Txt_Username);
                Circleindicator = MainView.FindViewById<CircleImageView>(Resource.Id.profile_indicator);

                //Event
                itemView.Click += (sender, e) => clickListener(new StoryAdapterClickEventArgs
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
        public CircleImageView Circleindicator { get; set; }

        #endregion
    }

    public class StoryAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}