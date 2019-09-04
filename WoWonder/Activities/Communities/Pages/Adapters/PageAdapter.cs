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
using WoWonder.SQLite;
using WoWonder_API.Classes.Group;
using WoWonder_API.Requests;

namespace WoWonder.Activities.Communities.Pages.Adapters
{
    public class PageAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_Community_Object.Page> mPageList =new ObservableCollection<Get_Community_Object.Page>();

        public PageAdapter(Context context)
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
                if (mPageList != null)
                    return mPageList.Count;
                return 0;
            }
        }

        public event EventHandler<PageAdapterClickEventArgs> ItemClick;
        public event EventHandler<PageAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_HPage_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_HPage_view, parent, false);
                var vh = new PageAdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is PageAdapterViewHolder holder)
                {
                    var item = mPageList[_position];
                    if (item != null)
                    {
                        //Dont Remove this code #####
                        FontController.SetFont(holder.Name, 1);
                        FontController.SetFont(holder.About, 3);
                        FontController.SetFont(holder.Button, 1);
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


        public void Initialize(PageAdapterViewHolder holder, Get_Community_Object.Page item)
        {
            try
            { 
                var AvatarSplit = item.Avatar.Split('/').Last();
                var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, AvatarSplit);
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
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage, item.Avatar);
                        ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", item.Avatar, 1);
                        holder.Image.Tag = "loaded";
                    }
                }

                var CoverSplit = item.Cover.Split('/').Last();
                var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskPage, CoverSplit);
                if (getImage_Cover == "File Dont Exists")
                {
                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskPage, item.Cover);
                }

                CategoriesController cat = new CategoriesController();
                holder.About.Text = cat.Get_Translate_Categories_Communities(item.PageCategory, item.Category);

                var drawable = TextDrawable.TextDrawable.TextDrawbleBuilder.BeginConfig().FontSize(30).EndConfig().BuildRound("", Color.ParseColor("#BF360C"));
                holder.ImageView.SetImageDrawable(drawable);

                IMethods.Set_TextViewIcon("1", holder.IconGroup, IonIcons_Fonts.IosFlag);

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(item.PageName));
                holder.Name.Text = IMethods.Fun_String.SubStringCutOf(name, 14);

                //Set style Btn Like page 
                holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                holder.Button.Text = Activity_Context.GetText(Resource.String.Btn_Unlike);
                holder.Button.Tag = "true";

                if (!holder.Button.HasOnClickListeners)
                    holder.Button.Click += delegate
                    {
                        if (holder.Button.Tag.ToString() == "false")
                        {
                            holder.Button.SetBackgroundResource(Resource.Drawable
                                .follow_button_profile_friends_pressed);
                            holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                            holder.Button.Text = Activity_Context.GetText(Resource.String.Btn_Unlike);
                            holder.Button.Tag = "true";
                        }
                        else
                        {
                            holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                            holder.Button.SetTextColor(Color.ParseColor(Settings.MainColor));
                            holder.Button.Text = Activity_Context.GetText(Resource.String.Btn_Like);
                            holder.Button.Tag = "false";
                        }

                        // Add Page Or Remove in DB 
                        var dbDatabase = new SqLiteDatabase();
                        var data = new DataTables.PageTB
                        {
                            PageId = item.PageId,
                            UserId = item.UserId,
                            PageName = item.PageName,
                            PageTitle = item.PageTitle,
                            PageDescription = item.PageDescription,
                            Avatar = item.Avatar,
                            Cover = item.Cover,
                            PageCategory = item.PageCategory,
                            Website = item.Website,
                            Facebook = item.Facebook,
                            Google = item.Google,
                            Vk = item.Vk,
                            Twitter = item.Twitter,
                            Linkedin = item.Linkedin,
                            Company = item.Company,
                            Phone = item.Phone,
                            Address = item.Address,
                            CallActionType = item.CallActionType,
                            CallActionTypeUrl = item.CallActionTypeUrl,
                            BackgroundImage = item.BackgroundImage,
                            BackgroundImageStatus = item.BackgroundImageStatus,
                            Instgram = item.Instgram,
                            Youtube = item.Youtube,
                            Verified = item.Verified,
                            Registered = item.Registered,
                            Boosted = item.Boosted,
                            About = item.About,
                            Id = item.Id,
                            Type = item.Type,
                            Url = item.Url,
                            Name = item.Name,
                            //Rating = item.Rating,
                            Category = item.Category,
                            IsPageOnwer = Convert.ToString(item.IsPageOnwer),
                            Username = item.Username
                        };
                        dbDatabase.Insert_Or_Delete_OnePagesTable(item.PageId, data);

                        var result = Client.Page.Like_Page(item.PageId).ConfigureAwait(false);

                        dbDatabase.Dispose();
                    };
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
        public void Add(Get_Community_Object.Page page)
        {
            try
            {
                var check = mPageList.FirstOrDefault(a => a.PageId == page.PageId);
                if (check == null)
                {
                    mPageList.Add(page);
                    NotifyItemInserted(mPageList.IndexOf(mPageList.Last()));
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
                mPageList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_Community_Object.Page GetItem(int position)
        {
            return mPageList[position];
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

        private void OnClick(PageAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(PageAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class PageAdapterViewHolder : RecyclerView.ViewHolder
    {
        public PageAdapterViewHolder(View itemView, Action<PageAdapterClickEventArgs> clickListener,
            Action<PageAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.Image);
                Name = MainView.FindViewById<TextView>(Resource.Id.card_name);
                About = MainView.FindViewById<TextView>(Resource.Id.card_dist);
                Button = MainView.FindViewById<Button>(Resource.Id.cont);
                IconGroup = MainView.FindViewById<TextView>(Resource.Id.Icon);
                ImageView = MainView.FindViewById<ImageView>(Resource.Id.image_view);

                //Event
                itemView.Click += (sender, e) => clickListener(new PageAdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new PageAdapterClickEventArgs
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
        public TextView IconGroup { get; set; }
        public ImageView ImageView { get; set; }

        #endregion
    }

    public class PageAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}