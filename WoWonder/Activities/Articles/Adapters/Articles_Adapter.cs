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
using Newtonsoft.Json;
using WoWonder.Activities.UserProfile;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;

namespace WoWonder.Activities.Articles.Adapters
{
    public class Articles_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_Users_Articles_Object.Article> ArticlesList =
            new ObservableCollection<Get_Users_Articles_Object.Article>();

        public Articles_Adapter(Context context)
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
                if (ArticlesList != null)
                    return ArticlesList.Count;
                return 0;
            }
        }

        public event EventHandler<Articles_AdapterClickEventArgs> ItemClick;
        public event EventHandler<Articles_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Article_View
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Article_View, parent, false);
                var vh = new Articles_AdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is Articles_AdapterViewHolder holder)
                {
                    var item = ArticlesList[_position];
                    if (item != null) Initialize(holder, item);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Initialize(Articles_AdapterViewHolder holder, Get_Users_Articles_Object.Article item)
        {
            try
            {
                var ArticlesSplit = item.thumbnail.Split('/').Last();
                var getImage_Articles = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskArticles, ArticlesSplit);
                if (getImage_Articles != "File Dont Exists")
                {
                    if (holder.Image.Tag?.ToString() != "loaded")
                    {
                        ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", item.thumbnail, 0, false);
                        holder.Image.Tag = "loaded";
                    }
                }
                else
                {
                    if (holder.Image.Tag?.ToString() != "loaded")
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskArticles, item.author.avatar);
                        ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", item.thumbnail, 0, false);
                        holder.Image.Tag = "loaded";
                    }
                }

                var AvatarSplit = item.author.avatar.Split('/').Last();
                var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                if (getImage_Avatar != "File Dont Exists")
                {
                    if (holder.UserImageProfile.Tag?.ToString() != "loaded")
                    {
                        ImageServiceLoader.Load_Image(holder.UserImageProfile, "no_profile_image.png", getImage_Avatar, 1);
                        holder.UserImageProfile.Tag = "loaded";
                    }
                }
                else
                {
                    if (holder.UserImageProfile.Tag?.ToString() != "loaded")
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, item.author.avatar);
                        ImageServiceLoader.Load_Image(holder.UserImageProfile, "no_profile_image.png", item.author.avatar, 1, false);
                        holder.UserImageProfile.Tag = "loaded";
                    }
                }

                holder.Category.SetBackgroundColor(Color.ParseColor(IMethods.Fun_String.RandomColor()));

                CategoriesController cat = new CategoriesController();
                string id = item.category_link.Split('/').Last();
                holder.Category.Text = cat.Get_Translate_Categories_Communities(id, item.category_name);
                 
                holder.Description.Text = item.description;
                holder.Title.Text = item.title;
                holder.Username.Text = item.author.name;
                holder.ViewMore.Text = Activity_Context.GetText(Resource.String.Lbl_ReadMore) + " >"; //READ MORE &gt; 
                holder.Time.Text = item.posted;
                
                if (!holder.UserItem.HasOnClickListeners)
                    holder.UserItem.Click += (sender, args) =>
                    {
                        try
                        {
                            var Int = new Intent(Activity_Context, typeof(User_Profile_Activity));
                            Int.PutExtra("UserId", item.author.user_id);
                            Int.PutExtra("UserType", "Articles");
                            Int.PutExtra("UserItem", JsonConvert.SerializeObject(item));
                            Activity_Context.StartActivity(Int);
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
        public void Add(Get_Users_Articles_Object.Article article)
        {
            try
            {
                var check = ArticlesList.FirstOrDefault(a => a.id == article.id);
                if (check == null)
                {
                    ArticlesList.Add(article);
                    NotifyItemInserted(ArticlesList.IndexOf(ArticlesList.Last()));
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
                ArticlesList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_Users_Articles_Object.Article GetItem(int position)
        {
            return ArticlesList[position];
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

        private void OnClick(Articles_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(Articles_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class Articles_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public Articles_AdapterViewHolder(View itemView, Action<Articles_AdapterClickEventArgs> clickListener,
            Action<Articles_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;


                UserItem = MainView.FindViewById<RelativeLayout>(Resource.Id.UserItem_Layout);

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.Image);
                Category = MainView.FindViewById<TextView>(Resource.Id.Category);
                Title = MainView.FindViewById<TextView>(Resource.Id.Title);
                Description = MainView.FindViewById<TextView>(Resource.Id.description);
                UserImageProfile = MainView.FindViewById<ImageViewAsync>(Resource.Id.UserImageProfile);
                Username = MainView.FindViewById<TextView>(Resource.Id.Username);
                Time = MainView.FindViewById<TextView>(Resource.Id.card_dist);
                ViewMore = MainView.FindViewById<TextView>(Resource.Id.View_more);

                //Event
                itemView.Click += (sender, e) => clickListener(new Articles_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new Articles_AdapterClickEventArgs
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
        public TextView Title { get; set; }
        public TextView Description { get; set; }
        public ImageViewAsync UserImageProfile { get; set; }
        public TextView Category { get; set; }
        public TextView Username { get; set; }
        public TextView Time { get; set; }
        public TextView ViewMore { get; set; }
        public RelativeLayout UserItem { get; set; }

        #endregion
    }

    public class Articles_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}