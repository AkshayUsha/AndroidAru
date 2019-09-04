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
    public class ProPages_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_General_Data_Object.Promoted_Pages> mProPagesList =
            new ObservableCollection<Get_General_Data_Object.Promoted_Pages>();

        public ProPages_Adapter(Context context)
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
                if (mProPagesList != null)
                    return mProPagesList.Count;
                return 0;
            }
        }

        public event EventHandler<ProPages_AdapterClickEventArgs> ItemClick;
        public event EventHandler<ProPages_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_PageCircle_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_PageCircle_view, parent, false);
                var vh = new ProPages_AdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is ProPages_AdapterViewHolder holder)
                {
                    var item = mProPagesList[_position];
                    if (item != null)
                    {
                        //Dont Remove this code #####
                        FontController.SetFont(holder.Name, 1);
                        IMethods.Set_TextViewIcon("1", holder.IconPage, IonIcons_Fonts.IosFlag);


                        if (holder.Image.Tag?.ToString() != "loaded")
                        {
                            ImageServiceLoader.Load_Image(holder.Image, "ImagePlacholder.jpg", item.avatar, 1);
                            holder.Image.Tag = "loaded";
                        }

                        string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(item.page_name));
                        holder.Name.Text = name;
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
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
        public void Add(Get_General_Data_Object.Promoted_Pages user)
        {
            try
            {
                var check = mProPagesList.FirstOrDefault(a => a.user_id == user.user_id);
                if (check == null)
                {
                    mProPagesList.Add(user);
                    NotifyItemInserted(mProPagesList.IndexOf(mProPagesList.Last()));
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
                mProPagesList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_General_Data_Object.Promoted_Pages GetItem(int position)
        {
            return mProPagesList[position];
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

        private void OnClick(ProPages_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(ProPages_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class ProPages_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public ProPages_AdapterViewHolder(View itemView, Action<ProPages_AdapterClickEventArgs> clickListener,
            Action<ProPages_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.Image);
                Name = MainView.FindViewById<TextView>(Resource.Id.Name);
                IconPage = MainView.FindViewById<TextView>(Resource.Id.Icon);


                //Create an Event
                itemView.Click += (sender, e) => clickListener(new ProPages_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new ProPages_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
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
        public TextView IconPage { get; set; }

        #endregion
    }

    public class ProPages_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}