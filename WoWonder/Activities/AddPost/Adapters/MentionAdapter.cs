using System;
using System.Linq;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;

namespace WoWonder.Activities.AddPost.Adapters
{
    public class Mention : Classes.UserContacts.User
    {
        public bool Selected { set; get; }
    }

    public class MentionAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;
        public JavaList<Mention> MentionList = new JavaList<Mention>();

        public MentionAdapter(Context context, JavaList<Mention> MyContactsList)
        {
            try
            {
                Activity_Context = context;
                MentionList = MyContactsList;
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
                if (MentionList != null)
                    return MentionList.Count;
                return 0;
            }
        }

        public event EventHandler<MentionAdapterClickEventArgs> ItemClick;
        public event EventHandler<MentionAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Mention_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Mention_view, parent, false);
                var vh = new MentionAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is MentionAdapterViewHolder holder)
                {
                    var item = MentionList[position];
                    if (item != null)
                    {
                        FontController.SetFont(holder.Name, 1);
                        FontController.SetFont(holder.About, 3);

                        holder.CheckBox.Checked = item.Selected;
                        holder.CheckBox.SetOnCheckedChangeListener(null);

                        if (!holder.MainView.HasOnClickListeners)
                            holder.MainView.Click += (sender, args) =>
                            {
                                if (holder.CheckBox.Checked)
                                {
                                    holder.CheckBox.Checked = false;
                                    item.Selected = false;
                                    NotifyItemChanged(position);
                                }
                                else
                                {
                                    holder.CheckBox.Checked = true;
                                    item.Selected = true;
                                    NotifyItemChanged(position);
                                }
                            };

                        var AvatarSplit = item.avatar.Split('/').Last();
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
                                IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, item.avatar);
                                ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", item.avatar, 1);
                                holder.Image.Tag = "loaded";
                            }
                            
                        }

                        var CoverSplit = item.cover.Split('/').Last();
                        var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, CoverSplit);
                        if (getImage_Cover == "File Dont Exists")
                        {
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, item.cover);
                        }

                        string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(item.name));
                        holder.Name.Text = name;

                        var dataabout = IMethods.Fun_String.StringNullRemover(item.about);
                        if (dataabout != "Empty")
                        {
                            var about = IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                            holder.About.Text = IMethods.Fun_String.SubStringCutOf(about, 25);
                        }
                        else
                        {
                            var about = Activity_Context.GetText(Resource.String.Lbl_DefaultAbout) + " " +
                                        Settings.Application_Name;
                            holder.About.Text = IMethods.Fun_String.SubStringCutOf(about, 25);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public Mention GetItem(int position)
        {
            return MentionList[position];
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


        private void OnClick(MentionAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(MentionAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class MentionAdapterViewHolder : RecyclerView.ViewHolder
    {
        public MentionAdapterViewHolder(View itemView, Action<MentionAdapterClickEventArgs> clickListener,
            Action<MentionAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.card_pro_pic);
                Name = MainView.FindViewById<TextView>(Resource.Id.card_name);
                About = MainView.FindViewById<TextView>(Resource.Id.card_dist);
                CheckBox = MainView.FindViewById<CheckBox>(Resource.Id.cont);


                //Event
                //itemView.Click += (sender, e) => clickListener(new MentionAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                //itemView.LongClick += (sender, e) => longClickListener(new MentionAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #region Variables Basic

        public View MainView { get; }

        public event EventHandler<int> ItemClick;
        public ImageViewAsync Image { get; set; }

        public TextView Name { get; set; }
        public TextView About { get; set; }
        public CheckBox CheckBox { get; set; }

        #endregion
    }

    public class MentionAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public Mention Mention { get; set; }
    }
}