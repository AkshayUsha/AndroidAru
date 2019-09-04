using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using Refractored.Controls;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;

namespace WoWonder.Activities.Tabbes.Adapters
{
    public class ProUsers_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_General_Data_Object.Pro_Users> mProUsersList =
            new ObservableCollection<Get_General_Data_Object.Pro_Users>();

        public ProUsers_Adapter(Context context)
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
                if (mProUsersList != null)
                    return mProUsersList.Count;
                return 0;
            }
        }

        public event EventHandler<ProUsers_AdapterClickEventArgs> ItemClick;
        public event EventHandler<ProUsers_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Pro_Users_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Pro_Users_view, parent, false);
                var vh = new ProUsers_AdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is ProUsers_AdapterViewHolder holder)
                {
                    var item = mProUsersList[_position];
                    if (item != null)
                    {
                        if (holder.UserImage.Tag?.ToString() != "loaded")
                        {
                            ImageServiceLoader.Load_Image(holder.UserImage, "no_profile_image.png", item.avatar, 1);
                            holder.UserImage.Tag = "loaded";
                        }
                       
                        if (item.pro_type == "1") //  STAR
                        {
                            if (holder.CircleImageView.Tag?.ToString() != "true")
                            {
                                holder.CircleImageView.SetImageResource(Resource.Drawable.DarkGreen_Color);
                                IMethods.Set_TextViewIcon("1", holder.Icon, IonIcons_Fonts.AndroidStar);
                                holder.CircleImageView.Tag = "true";
                            }
                            
                        }
                        else if (item.pro_type == "2") // HOT
                        {
                            if (holder.CircleImageView.Tag?.ToString() != "true")
                            {
                                holder.CircleImageView.Tag = "true";
                                holder.CircleImageView.SetImageResource(Resource.Drawable.DarkOrange_Color);
                                IMethods.Set_TextViewIcon("1", holder.Icon, IonIcons_Fonts.Flame);
                            }
                               
                        }
                        else if (item.pro_type == "3") // ULTIMA
                        {
                            if (holder.CircleImageView.Tag?.ToString() != "true")
                            {
                                holder.CircleImageView.Tag = "true";
                                holder.CircleImageView.SetImageResource(Resource.Drawable.Red_Color);
                                IMethods.Set_TextViewIcon("1", holder.Icon, IonIcons_Fonts.Flash);
                            }
                             
                        }
                        else if (item.pro_type == "4") // VIP
                        {
                            if (holder.Icon.Tag?.ToString() != "true")
                            {
                                holder.Icon.Tag = "true";
                                IMethods.Set_TextViewIcon("3", holder.Icon, "\uf135");
                            }
                              
                        }
                        else
                        {
                            if (holder.Icon.Tag?.ToString() != "true")
                            {
                                holder.Icon.Tag = "true";
                                IMethods.Set_TextViewIcon("3", holder.Icon, "\uf135");
                            }
                        }
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
        public void Add(Get_General_Data_Object.Pro_Users user)
        {
            try
            {
                var check = mProUsersList.FirstOrDefault(a => a.user_id == user.user_id);
                if (check == null)
                {
                    mProUsersList.Add(user);
                    NotifyItemInserted(mProUsersList.IndexOf(mProUsersList.Last()));
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
                mProUsersList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_General_Data_Object.Pro_Users GetItem(int position)
        {
            return mProUsersList[position];
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

        private void OnClick(ProUsers_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(ProUsers_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class ProUsers_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public ProUsers_AdapterViewHolder(View itemView, Action<ProUsers_AdapterClickEventArgs> clickListener,
            Action<ProUsers_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Icon = MainView.FindViewById<TextView>(Resource.Id.Iconpro);
                UserImage = MainView.FindViewById<ImageViewAsync>(Resource.Id.Image);
                CircleImageView = MainView.FindViewById<CircleImageView>(Resource.Id.ImageCircle);


                //Create an Event
                itemView.Click += (sender, e) => clickListener(new ProUsers_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new ProUsers_AdapterClickEventArgs
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


        public ImageViewAsync UserImage { get; set; }
        public CircleImageView CircleImageView { get; set; }
        public TextView Icon { get; set; }

        #endregion
    }

    public class ProUsers_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}