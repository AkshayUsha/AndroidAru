using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;

namespace WoWonder.Activities.PostData.Adapters
{
    public class LikedUsers_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_Post_Data_Object.PostLikedUsers> mPostlikedList =
            new ObservableCollection<Get_Post_Data_Object.PostLikedUsers>();


        public LikedUsers_Adapter(Context context)
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
                if (mPostlikedList != null)
                    return mPostlikedList.Count;
                return 0;
            }
        }

        public event EventHandler<LikedUsers_AdapterClickEventArgs> ItemClick;
        public event EventHandler<LikedUsers_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_HContact_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_HContact_view, parent, false);
                var vh = new LikedUsers_AdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is LikedUsers_AdapterViewHolder holder)
                {
                    var item = mPostlikedList[_position];
                    if (item != null)
                    {
                        //Dont Remove this code #####
                        FontController.SetFont(holder.Name, 1);
                        FontController.SetFont(holder.About, 3);
                        holder.Button.Visibility = ViewStates.Gone;
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

        public void Initialize(LikedUsers_AdapterViewHolder holder, Get_Post_Data_Object.PostLikedUsers users)
        {
            try
            {
                var AvatarSplit = users.Avatar.Split('/').Last();
                var getImage_Avatar =
                    IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
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
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, users.Avatar);
                        ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", users.Avatar, 1);
                        holder.Image.Tag = "loaded";
                    }
                }

                var CoverSplit = users.Cover.Split('/').Last();
                var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, CoverSplit);
                if (getImage_Cover == "File Dont Exists")
                {
                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, users.Cover);
                }

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(users.Name));
                holder.Name.Text = IMethods.Fun_String.SubStringCutOf(name, 25);

                var dataabout = IMethods.Fun_String.StringNullRemover(users.About);
                if (dataabout != "Empty")
                {
                    var about = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                    holder.About.Text = IMethods.Fun_String.SubStringCutOf(about, 40);
                }
                else
                {
                    var about = Activity_Context.GetText(Resource.String.Lbl_DefaultAbout) + " " +
                                Settings.Application_Name;
                    holder.About.Text = IMethods.Fun_String.SubStringCutOf(about, 40);
                }
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
        public void Add(Get_Post_Data_Object.PostLikedUsers user)
        {
            try
            {
                var check = mPostlikedList.FirstOrDefault(a => a.UserId == user.UserId);
                if (check == null)
                {
                    mPostlikedList.Add(user);
                    NotifyItemInserted(mPostlikedList.IndexOf(mPostlikedList.Last()));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public Get_Post_Data_Object.PostLikedUsers GetItem(int position)
        {
            return mPostlikedList[position];
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

        private void OnClick(LikedUsers_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(LikedUsers_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class LikedUsers_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public LikedUsers_AdapterViewHolder(View itemView, Action<LikedUsers_AdapterClickEventArgs> clickListener,
            Action<LikedUsers_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.card_pro_pic);
                Name = MainView.FindViewById<TextView>(Resource.Id.card_name);
                About = MainView.FindViewById<TextView>(Resource.Id.card_dist);
                Button = MainView.FindViewById<Button>(Resource.Id.cont);

                //Event
                itemView.Click += (sender, e) => clickListener(new LikedUsers_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new LikedUsers_AdapterClickEventArgs
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
        public TextView About { get; set; }
        public Button Button { get; set; }

        #endregion
    }

    public class LikedUsers_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}