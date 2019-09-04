using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using Plugin.Share;
using Plugin.Share.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using WoWonder.Helpers;
using WoWonder_API.Classes.Event;

namespace WoWonder.Activities.Events.Adapters
{
    public class Event_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_Events_Object.Event> mEventList =
            new ObservableCollection<Get_Events_Object.Event>();

        public Event_Adapter(Context context)
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
                if (mEventList != null)
                    return mEventList.Count;
                return 0;
            }
        }

        public event EventHandler<Event_AdapterClickEventArgs> ItemClick;

        public event EventHandler<Event_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Event_Cell
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Event_Cell, parent, false);
                var vh = new Event_AdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is Event_AdapterViewHolder holder)
                {
                    var item = mEventList[_position];
                    if (item != null) Initialize(holder, item);
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Initialize(Event_AdapterViewHolder holder, Get_Events_Object.Event item)
        {
            try
            {
                if (holder.Image.Tag?.ToString() != "loaded")
                {
                    var CoverSplit = item.cover.Split('/').Last();
                    var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskEvent, CoverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", getImage_Cover);
                        holder.Image.Tag = "loaded";
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskEvent, item.cover);
                        ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", item.cover);
                        holder.Image.Tag = "loaded";
                    }
                }

                if (holder.event_place_icon.Text != IonIcons_Fonts.IosLocation)
                    IMethods.Set_TextViewIcon("1", holder.event_place_icon, IonIcons_Fonts.IosLocation);
                if (holder.event_time_icon.Text != IonIcons_Fonts.IosClockOutline)
                    IMethods.Set_TextViewIcon("1", holder.event_time_icon, IonIcons_Fonts.IosClockOutline);
                if (holder.event_going_icon.Text != IonIcons_Fonts.PersonStalker)
                    IMethods.Set_TextViewIcon("1", holder.event_going_icon, IonIcons_Fonts.PersonStalker);
                if (holder.event_intersted_icon.Text != IonIcons_Fonts.Star)
                    IMethods.Set_TextViewIcon("1", holder.event_intersted_icon, IonIcons_Fonts.Star);

                if (holder.Txt_event_titile.Tag?.ToString() != "true")
                {
                    holder.Txt_event_titile.Tag = "true";
                    holder.Txt_event_titile.Text = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(IMethods.Fun_String.SubStringCutOf(item.name, 30)));
                }

                if (holder.Txt_event_Place.Tag?.ToString() != "true")
                {
                    holder.Txt_event_Place.Tag = "true";
                    holder.Txt_event_Place.Text = item.location;
                }

                if (holder.Txt_event_going.Tag?.ToString() != "true")
                {
                    holder.Txt_event_going.Tag = "true";
                    holder.Txt_event_going.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(item.going_count));
                }

                if (holder.Txt_event_intersted.Tag?.ToString() != "true")
                {
                    holder.Txt_event_intersted.Tag = "true";
                    holder.Txt_event_intersted.Text = IMethods.Fun_String.FormatPriceValue(int.Parse(item.interested_count));
                }

                if (holder.Txt_event_time.Tag?.ToString() != "true")
                {
                    holder.Txt_event_time.Tag = "true";
                    holder.Txt_event_time.Text = item.start_date;
                }

                if (!holder.Btn_event_share.HasOnClickListeners)
                    holder.Btn_event_share.Click += async (sender, args) =>
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
        public void Add(Get_Events_Object.Event Event)
        {
            try
            {
                var check = mEventList.FirstOrDefault(a => a.id == Event.id);
                if (check == null)
                {
                    mEventList.Add(Event);
                    NotifyItemInserted(mEventList.IndexOf(mEventList.Last()));
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
                mEventList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_Events_Object.Event GetItem(int position)
        {
            return mEventList[position];
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

        private void OnClick(Event_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(Event_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class Event_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public Event_AdapterViewHolder(View itemView, Action<Event_AdapterClickEventArgs> clickListener,
            Action<Event_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                RelativeLayout_main = (RelativeLayout)MainView.FindViewById(Resource.Id.main);

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
                itemView.Click += (sender, e) => clickListener(new Event_AdapterClickEventArgs
                { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new Event_AdapterClickEventArgs
                { View = itemView, Position = AdapterPosition });
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

        #endregion Variables Basic
    }

    public class Event_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}