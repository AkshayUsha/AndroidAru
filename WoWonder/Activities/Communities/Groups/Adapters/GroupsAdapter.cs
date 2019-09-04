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
using SettingsConnecter;
using WoWonder.Helpers;
using WoWonder_API.Classes.Group;

namespace WoWonder.Activities.Communities.Groups.Adapters
{
    public class GroupsAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_Community_Object.Group> mGroupsList =
            new ObservableCollection<Get_Community_Object.Group>();

        public GroupsAdapter(Context context)
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
                if (mGroupsList != null)
                    return mGroupsList.Count;
                return 0;
            }
        }

        public event EventHandler<GroupsAdapteClickEventArgs> ItemClick;
        public event EventHandler<GroupsAdapteClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_GroupCircle_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_GroupCircle_view, parent, false);
                var vh = new GroupsAdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is GroupsAdapterViewHolder holder)
                {
                    var item = mGroupsList[_position];
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

        public void Initialize(GroupsAdapterViewHolder holder, Get_Community_Object.Group item)
        {
            try
            { 
                var AvatarSplit = item.Avatar.Split('/').Last();
                var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskGroup, AvatarSplit);
                if (getImage_Avatar != "File Dont Exists")
                {
                    if (holder.Image.Tag?.ToString() != "loaded")
                    {
                        ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", getImage_Avatar, 1);
                        holder.Image.Tag = "loaded";
                    }
                }
                else
                {
                    if (holder.Image.Tag?.ToString() != "loaded")
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskGroup, item.Avatar);
                        ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", item.Avatar, 1);
                        holder.Image.Tag = "loaded";
                    }
                }

                IMethods.Set_TextViewIcon("1", holder.IconGroup, IonIcons_Fonts.PersonStalker);

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(item.GroupName));
                holder.Name.Text = IMethods.Fun_String.SubStringCutOf(name, 14);
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
        public void Add(Get_Community_Object.Group group)
        {
            try
            {
                var check = mGroupsList.FirstOrDefault(a => a.GroupId == group.GroupId);
                if (check == null)
                {
                    mGroupsList.Add(group);
                    NotifyItemInserted(mGroupsList.IndexOf(mGroupsList.Last()));
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
                mGroupsList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_Community_Object.Group GetItem(int position)
        {
            return mGroupsList[position];
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

        private void OnClick(GroupsAdapteClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(GroupsAdapteClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class GroupsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public GroupsAdapterViewHolder(View itemView, Action<GroupsAdapteClickEventArgs> clickListener,
            Action<GroupsAdapteClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.Image);
                Name = MainView.FindViewById<TextView>(Resource.Id.groupName);
                IconGroup = MainView.FindViewById<TextView>(Resource.Id.IconGroup);
                //Event
                itemView.Click += (sender, e) => clickListener(new GroupsAdapteClickEventArgs
                    {View = itemView, Position = AdapterPosition});
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (Button.Tag.ToString() == "false")
            {
                Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                Button.SetTextColor(Color.ParseColor("#ffffff"));
                Button.Text = "Following";
                Button.Tag = "true";
            }
            else
            {
                Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                Button.SetTextColor(Color.ParseColor(Settings.MainColor));
                Button.Text = "Follow";
                Button.Tag = "false";
            }
        }

        #region Variables Basic

        public View MainView { get; }

        public event EventHandler<int> ItemClick;
        public ImageViewAsync Image { get; set; }
        public TextView Name { get; set; }
        public TextView IconGroup { get; set; }


        public Button Button { get; set; }

        #endregion
    }

    public class GroupsAdapteClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}