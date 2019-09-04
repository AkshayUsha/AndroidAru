using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using Plugin.Share;
using Plugin.Share.Abstractions;
using WoWonder.Helpers;
using WoWonder_API.Classes.Event;

namespace WoWonder.Activities.Events.Adapters
{
    public class MyEvent_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_MyEvent_object.My_Events> mMyEventList =
            new ObservableCollection<Get_MyEvent_object.My_Events>();


        public MyEvent_Adapter(Context context)
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
                if (mMyEventList != null)
                    return mMyEventList.Count;
                return 0;
            }
        }

        public event EventHandler<MyEvent_AdapterClickEventArgs> ItemClick;
        public event EventHandler<MyEvent_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Event_Cell
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Event_Cell, parent, false);
                var vh = new MyEvent_AdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is MyEvent_AdapterViewHolder holder)
                {
                    var item = mMyEventList[_position];
                    if (item != null) Initialize(holder, item);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }


        public void Initialize(MyEvent_AdapterViewHolder holder, Get_MyEvent_object.My_Events item)
        {
            try
            {
                var CoverSplit = item.cover.Split('/').Last();
                var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskEvent, CoverSplit);
                if (getImage_Cover != "File Dont Exists")
                {
                    if (holder.Image.Tag?.ToString() != "loaded")
                    {
                        ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", getImage_Cover, 3);
                        holder.Image.Tag = "loaded";
                    }
                }
                else
                {
                    if (holder.Image.Tag?.ToString() != "loaded")
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskEvent, item.cover);
                        ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", item.cover, 3);
                        holder.Image.Tag = "loaded";
                    }
                }

                IMethods.Set_TextViewIcon("1", holder.event_place_icon, IonIcons_Fonts.IosLocation);
                IMethods.Set_TextViewIcon("1", holder.event_time_icon, IonIcons_Fonts.IosClockOutline);
                IMethods.Set_TextViewIcon("1", holder.event_going_icon, IonIcons_Fonts.PersonStalker);
                IMethods.Set_TextViewIcon("1", holder.event_intersted_icon, IonIcons_Fonts.Star);

                holder.Txt_event_titile.Text = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(IMethods.Fun_String.SubStringCutOf(item.name, 30)));
                holder.Txt_event_Place.Text = item.location;
                //holder.Txt_event_going.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(item.going_count));
                //holder.Txt_event_intersted.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(item.interested_count));
                holder.Txt_event_time.Text = item.start_date;

                if (!holder.Btn_event_share.HasOnClickListeners)
                    holder.Btn_event_share.Click += async delegate
                    {
                        try
                        {
                            //Share Plugin same as video
                            if (!CrossShare.IsSupported) return;

                            await CrossShare.Current.Share(new ShareMessage
                            {
                                Title = item.name,
                                Text = item.description,
                                Url = item.url
                            });
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
        public void Add(Get_MyEvent_object.My_Events Event)
        {
            try
            {
                var check = mMyEventList.FirstOrDefault(a => a.id == Event.id);
                if (check == null)
                {
                    mMyEventList.Add(Event);
                    NotifyItemInserted(mMyEventList.IndexOf(mMyEventList.Last()));
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
                mMyEventList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_MyEvent_object.My_Events GetItem(int position)
        {
            return mMyEventList[position];
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

        private void OnClick(MyEvent_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(MyEvent_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class MyEvent_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public MyEvent_AdapterViewHolder(View itemView, Action<MyEvent_AdapterClickEventArgs> clickListener,
            Action<MyEvent_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                RelativeLayout_main = (RelativeLayout) MainView.FindViewById(Resource.Id.main);

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.Image);
                Txt_event_titile = MainView.FindViewById<TextView>(Resource.Id.event_titile);
                Txt_event_Place = MainView.FindViewById<TextView>(Resource.Id.event_Place);
                Txt_event_time = MainView.FindViewById<TextView>(Resource.Id.event_time);
                event_time_icon = MainView.FindViewById<TextView>(Resource.Id.event_time_icon);
                event_place_icon = MainView.FindViewById<TextView>(Resource.Id.event_place_icon);
                Txt_event_going = MainView.FindViewById<TextView>(Resource.Id.event_going);
                event_going_icon = MainView.FindViewById<TextView>(Resource.Id.event_going_icon);
                event_intersted_icon = MainView.FindViewById<TextView>(Resource.Id.event_intersted_icon);
                Txt_event_intersted = MainView.FindViewById<TextView>(Resource.Id.event_intersted);
                Btn_event_share = MainView.FindViewById<CircleButton>(Resource.Id.event_share);

                //Event
                itemView.Click += (sender, e) => clickListener(new MyEvent_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new MyEvent_AdapterClickEventArgs
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

        public RelativeLayout RelativeLayout_main { get; set; }
        public ImageViewAsync Image { get; set; }

        public TextView Txt_event_titile { get; set; }
        public TextView Txt_event_Place { get; set; }
        public TextView Txt_event_going { get; set; }
        public TextView Txt_event_intersted { get; set; }
        public TextView Txt_event_time { get; set; }

        public TextView event_place_icon { get; set; }
        public TextView event_time_icon { get; set; }
        public TextView event_going_icon { get; set; }
        public TextView event_intersted_icon { get; set; }

        public CircleButton Btn_event_share { set; get; }

        #endregion
    }

    public class MyEvent_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}