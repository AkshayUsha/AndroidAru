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
    public class MarketAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_Products_Object.Product> MarketList =
            new ObservableCollection<Get_Products_Object.Product>();

        public MarketAdapter(Context context)
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
                if (MarketList != null)
                    return MarketList.Count;
                return 0;
            }
        }

        public event EventHandler<MarketAdapterClickEventArgs> ItemClick;
        public event EventHandler<MarketAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Event_Cell
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Market_view, parent, false);
                var vh = new MarketAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is MarketAdapterViewHolder holder)
                {
                    var item = MarketList[_position];
                    if (item != null) Initialize(holder, item);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Initialize(MarketAdapterViewHolder holder, Get_Products_Object.Product item)
        {
            try
            {
                if (holder.Thumbnail.Tag?.ToString() != "loaded")
                {
                    ImageServiceLoader.Load_Image(holder.Thumbnail, "ImagePlacholder.jpg", item.images[0].image, 2);
                    holder.Thumbnail.Tag = "loaded";
                }


                if (holder.Userprofilepic.Tag?.ToString() != "loaded")
                {
                    var AvatarSplit = item.seller.avatar.Split('/').Last();
                    var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(holder.Userprofilepic, "no_profile_image.png", getImage_Avatar, 1);
                        holder.Userprofilepic.Tag = "loaded";
                    }
                    else
                    {
                       
                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, item.seller.avatar);
                            ImageServiceLoader.Load_Image(holder.Userprofilepic, "no_profile_image.png", item.seller.avatar, 1);
                            holder.Userprofilepic.Tag = "loaded";
                       
                    }
                    
                }

                if (holder.mappinIcon.Text != IonIcons_Fonts.IosLocation)
                    IMethods.Set_TextViewIcon("1", holder.mappinIcon, IonIcons_Fonts.IosLocation);

                if (holder.Title.Tag?.ToString() != "true")
                {
                    holder.Title.Tag="true";
                    holder.Title.Text = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(item.name));
                }

                if (holder.User_name.Tag?.ToString() != "true")
                {
                    holder.User_name.Tag = "true";
                    string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(item.seller.name));
                    holder.User_name.Text = name;
                }

                if (holder.time.Tag?.ToString() != "true")
                {
                    holder.time.Tag = "true";
                    holder.time.Text = item.time_text;
                }


                if (holder.Txt_price.Tag?.ToString() != "true")
                {
                    holder.Txt_price.Tag = "true";
                    holder.Txt_price.Text = item.price + " " + Settings.Market_curency;
                }


                if (holder.LocationText.Tag?.ToString() != "true")
                {
                   
                    if (!string.IsNullOrEmpty(item.location))
                        holder.LocationText.Text = item.location;
                    else
                        holder.LocationText.Text = Activity_Context.GetText(Resource.String.Lbl_Unknown);

                    holder.LocationText.Tag = "true";
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
        public void Add(Get_Products_Object.Product product)
        {
            try
            {
                var check = MarketList.FirstOrDefault(a => a.id == product.id);
                if (check == null)
                {
                    MarketList.Add(product);
                    NotifyItemInserted(MarketList.IndexOf(MarketList.Last()));
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
                MarketList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_Products_Object.Product GetItem(int position)
        {
            return MarketList[position];
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

        private void OnClick(MarketAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(MarketAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class MarketAdapterViewHolder : RecyclerView.ViewHolder
    {
        public MarketAdapterViewHolder(View itemView, Action<MarketAdapterClickEventArgs> clickListener,
            Action<MarketAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Thumbnail = MainView.FindViewById<ImageViewAsync>(Resource.Id.thumbnail);
                Title = MainView.FindViewById<TextView>(Resource.Id.titleTextView);
                mappinIcon = MainView.FindViewById<TextView>(Resource.Id.mappin);
                LocationText = MainView.FindViewById<TextView>(Resource.Id.LocationText);
                Userprofilepic = MainView.FindViewById<ImageViewAsync>(Resource.Id.userprofile_pic);
                User_name = MainView.FindViewById<TextView>(Resource.Id.User_name);
                time = MainView.FindViewById<TextView>(Resource.Id.card_dist);
                Txt_price = MainView.FindViewById<TextView>(Resource.Id.pricetext);

                //Event
                itemView.Click += (sender, e) => clickListener(new MarketAdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new MarketAdapterClickEventArgs
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
        public TextView Title { get; set; }
        public TextView mappinIcon { get; set; }
        public ImageViewAsync Userprofilepic { get; set; }
        public TextView User_name { get; set; }
        public TextView time { get; set; }
        public TextView LocationText { get; set; }
        public TextView Txt_price { get; set; }

        #endregion
    }

    public class MarketAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}