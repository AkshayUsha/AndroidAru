using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;

namespace WoWonder.Activities.Tabbes.Adapters
{
    public class LastActivities_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Activities_Object.Activity> mLastActivitiesList =
            new ObservableCollection<Activities_Object.Activity>();

        public LastActivities_Adapter(Context context)
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
                if (mLastActivitiesList != null)
                    return mLastActivitiesList.Count;
                return 0;
            }
        }

        public event EventHandler<LastActivities_AdapterClickEventArgs> ItemClick;
        public event EventHandler<LastActivities_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_LastActivities_View
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_LastActivities_View, parent, false);
                var vh = new LastActivities_AdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is LastActivities_AdapterViewHolder holder)
                {
                    var item = mLastActivitiesList[_position];
                    if (item != null)
                    {
                        //Dont Remove this code #####
                        FontController.SetFont(holder.Username, 1);
                        FontController.SetFont(holder.Activities_event, 1);
                        FontController.SetFont(holder.Time, 3);
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

        public void Initialize(LastActivities_AdapterViewHolder holder, Activities_Object.Activity item)
        {
            try
            {
                var AvatarSplit = item.Activator.Avatar.Split('/').Last();
                var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                if (getImage_Avatar != "File Dont Exists")
                {
                    if (holder.ActivitiesImage.Tag?.ToString() != "loaded")
                    {
                        ImageServiceLoader.Load_Image(holder.ActivitiesImage, "no_profile_image.png", getImage_Avatar, 5);
                        holder.ActivitiesImage.Tag = "loaded";
                    }
                }
                else
                {
                    if (holder.ActivitiesImage.Tag?.ToString() != "loaded")
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage,item.Activator.Avatar);
                        ImageServiceLoader.Load_Image(holder.ActivitiesImage, "no_profile_image.png", item.Activator.Avatar,5);
                        holder.ActivitiesImage.Tag = "loaded";
                    }
                }

                if (item.ActivityType.Contains("wondered_post"))
                    IMethods.Set_TextViewIcon("1", holder.Icon, IonIcons_Fonts.InformationCircled);
                else if (item.ActivityType.Contains("liked_post"))
                    IMethods.Set_TextViewIcon("1", holder.Icon, IonIcons_Fonts.Thumbsup);
                else if (item.ActivityType.Contains("commented_post"))
                    IMethods.Set_TextViewIcon("1", holder.Icon, IonIcons_Fonts.Chatboxes);
                else
                    IMethods.Set_TextViewIcon("1", holder.Icon, IonIcons_Fonts.AndroidNotifications);

                string replace = "";
                if (item.ActivityText.Contains("commented on"))
                {
                    replace = item.ActivityText.Replace("commented on", this.Activity_Context.GetString(Resource.String.Lbl_CommentedOn))
                        .Replace("post", this.Activity_Context.GetString(Resource.String.Lbl_Post));
                }
                else if (item.ActivityText.Contains("reacted to"))
                {
                    replace = item.ActivityText.Replace("reacted to", this.Activity_Context.GetString(Resource.String.Lbl_ReactedTo))
                        .Replace("post", this.Activity_Context.GetString(Resource.String.Lbl_Post));
                }
                else if (item.ActivityText.Contains("started following"))
                {
                    replace = item.ActivityText.Replace("started following", this.Activity_Context.GetString(Resource.String.Lbl_StartedFollowing));
                }
                else if (item.ActivityText.Contains("become friends with"))
                {
                    replace = item.ActivityText.Replace("become friends with", this.Activity_Context.GetString(Resource.String.Lbl_BecomeFriendsWith));
                }
                else if (item.ActivityText.Contains("is following"))
                {
                    replace = item.ActivityText.Replace("is following", this.Activity_Context.GetString(Resource.String.Lbl_IsFollowing));
                }
                else if (item.ActivityText.Contains("liked"))
                {
                    replace = item.ActivityText.Replace("liked", this.Activity_Context.GetString(Resource.String.Btn_Liked))
                        .Replace("post", this.Activity_Context.GetString(Resource.String.Lbl_Post));
                }
                else if (item.ActivityText.Contains("wondered"))
                {
                    replace = item.ActivityText.Replace("wondered", this.Activity_Context.GetString(Resource.String.Lbl_wondered))
                        .Replace("post", this.Activity_Context.GetString(Resource.String.Lbl_Post));
                }
                else if (item.ActivityText.Contains("disliked"))
                {
                    replace = item.ActivityText.Replace("disliked", this.Activity_Context.GetString(Resource.String.Lbl_disliked))
                        .Replace("post", this.Activity_Context.GetString(Resource.String.Lbl_Post));
                }
                else if (item.ActivityText.Contains("shared"))
                {
                    replace = item.ActivityText.Replace("shared", this.Activity_Context.GetString(Resource.String.Lbl_shared))
                        .Replace("post", this.Activity_Context.GetString(Resource.String.Lbl_Post));
                }

                holder.Activities_event.Text = !string.IsNullOrEmpty(replace) ? replace : item.ActivityText;
                 
                // holder.Username.Text = item.Activator.Name; 
                holder.Username.Visibility = ViewStates.Gone;

                holder.Time.Text = IMethods.ITime.TimeAgo(int.Parse(item.Time));
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

        // Function 
        public void Add(Activities_Object.Activity item)
        {
            try
            {
                var check = mLastActivitiesList.FirstOrDefault(a => a.Id == item.Id);
                if (check == null)
                {
                    mLastActivitiesList.Add(item);
                    NotifyItemInserted(mLastActivitiesList.IndexOf(mLastActivitiesList.Last()));
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
                mLastActivitiesList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        public Activities_Object.Activity GetItem(int position)
        {
            return mLastActivitiesList[position];
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


        private void OnClick(LastActivities_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(LastActivities_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class LastActivities_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public LastActivities_AdapterViewHolder(View itemView,
            Action<LastActivities_AdapterClickEventArgs> clickListener,
            Action<LastActivities_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                ActivitiesImage = (ImageViewAsync) MainView.FindViewById(Resource.Id.Image);
                Username = MainView.FindViewById<TextView>(Resource.Id.LastActivitiesUserName);
                Activities_event = MainView.FindViewById<TextView>(Resource.Id.LastActivitiesText);
                Icon = MainView.FindViewById<TextView>(Resource.Id.LastActivitiesIcon);
                Time = MainView.FindViewById<TextView>(Resource.Id.Time);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new LastActivities_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new LastActivities_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #region Variables Basic

        public View MainView { get; set; }

        public ImageViewAsync ActivitiesImage { get; set; }
        public TextView Username { get; set; }
        public TextView Activities_event { get; set; }
        public TextView Icon { get; set; }
        public TextView Time { get; set; }

        #endregion
    }

    public class LastActivities_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}