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
using WoWonder_API.Classes.Product;

namespace WoWonder.Activities.Market.Adapters
{
    public class MyProducts_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_Products_Object.Product> MyProductsList =
            new ObservableCollection<Get_Products_Object.Product>();

        public MyProducts_Adapter(Context context)
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
                if (MyProductsList != null)
                    return MyProductsList.Count;
                return 0;
            }
        }

        public event EventHandler<MyProducts_AdapterClickEventArgs> ItemClick;
        public event EventHandler<MyProducts_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Event_Cell
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Market_view, parent, false);
                var vh = new MyProducts_AdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is MyProducts_AdapterViewHolder holder)
                {
                    var item = MyProductsList[_position];
                    if (item != null) Initialize(holder, item);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Initialize(MyProducts_AdapterViewHolder holder, Get_Products_Object.Product item)
        {
            try
            {
                if (holder.Thumbnail.Tag?.ToString() != "loaded")
                {
                    ImageServiceLoader.Load_Image(holder.Thumbnail, "ImagePlacholder.jpg", item.images[0].image, 2);
                    holder.Thumbnail.Tag = "loaded";
                }

                var AvatarSplit = item.seller.avatar.Split('/').Last();
                var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                if (getImage_Avatar != "File Dont Exists")
                {
                    if (holder.Userprofilepic.Tag?.ToString() != "loaded")
                    {
                        ImageServiceLoader.Load_Image(holder.Userprofilepic, "no_profile_image.png", getImage_Avatar, 1);
                        holder.Userprofilepic.Tag = "loaded";
                    }
                }
                else
                {
                    if (holder.Userprofilepic.Tag?.ToString() != "loaded")
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, item.seller.avatar);
                        ImageServiceLoader.Load_Image(holder.Userprofilepic, "no_profile_image.png", item.seller.avatar, 1);
                        holder.Userprofilepic.Tag = "loaded";
                    }
                }

                IMethods.Set_TextViewIcon("1", holder.mappinIcon, IonIcons_Fonts.IosLocation);

                holder.Txt_Title.Text = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(item.name));

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(item.seller.name));
                holder.User_name.Text = name;

                if (!string.IsNullOrEmpty(item.location))
                    holder.LocationText.Text = item.location;
                else
                    holder.LocationText.Text = Activity_Context.GetText(Resource.String.Lbl_Unknown);

                holder.time.Text = item.time_text;

                holder.Txt_price.Text = item.price + " " + Settings.Market_curency;
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
        public void Add(Get_Products_Object.Product product)
        {
            try
            {
                var check = MyProductsList.FirstOrDefault(a => a.id == product.id);
                if (check == null)
                {
                    MyProductsList.Add(product);
                    NotifyItemInserted(MyProductsList.IndexOf(MyProductsList.Last()));
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
                MyProductsList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_Products_Object.Product GetItem(int position)
        {
            return MyProductsList[position];
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

        private void OnClick(MyProducts_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(MyProducts_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class MyProducts_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public MyProducts_AdapterViewHolder(View itemView, Action<MyProducts_AdapterClickEventArgs> clickListener,
            Action<MyProducts_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Thumbnail = MainView.FindViewById<ImageViewAsync>(Resource.Id.thumbnail);
                Txt_Title = MainView.FindViewById<TextView>(Resource.Id.titleTextView);
                mappinIcon = MainView.FindViewById<TextView>(Resource.Id.mappin);
                LocationText = MainView.FindViewById<TextView>(Resource.Id.LocationText);
                Userprofilepic = MainView.FindViewById<ImageViewAsync>(Resource.Id.userprofile_pic);
                User_name = MainView.FindViewById<TextView>(Resource.Id.User_name);
                time = MainView.FindViewById<TextView>(Resource.Id.card_dist);
                Txt_price = MainView.FindViewById<TextView>(Resource.Id.pricetext);

                //Event
                itemView.Click += (sender, e) => clickListener(new MyProducts_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new MyProducts_AdapterClickEventArgs
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
        public ImageViewAsync Thumbnail { get; set; }
        public TextView Txt_Title { get; set; }
        public TextView mappinIcon { get; set; }
        public ImageViewAsync Userprofilepic { get; set; }
        public TextView User_name { get; set; }
        public TextView time { get; set; }
        public TextView LocationText { get; set; }
        public TextView Txt_price { get; set; }

        #endregion
    }

    public class MyProducts_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}