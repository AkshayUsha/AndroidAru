using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter.Crashes;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;

namespace WoWonder.Activities.Tabbes.Adapters
{
    public class HashtagUser_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_General_Data_Object.Trending_Hashtag> mHashtagList =
            new ObservableCollection<Get_General_Data_Object.Trending_Hashtag>();

        public HashtagUser_Adapter(Context context)
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
                if (mHashtagList != null)
                    return mHashtagList.Count;
                return 0;
            }
        }

        public event EventHandler<HashtagUser_AdapterClickEventArgs> ItemClick;
        public event EventHandler<HashtagUser_AdapterClickEventArgs> ItemLongClick;


        // Create new views (invoked by the layout manager) 
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_HLastSearch_View
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_HLastSearch_View, parent, false);
                var vh = new HashtagUser_AdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is HashtagUser_AdapterViewHolder holder)
                {
                    var item = mHashtagList[_position];
                    if (item != null)
                    {
                        //Dont Remove this code #####
                        FontController.SetFont(holder.Button, 1);
                        //#####
                        holder.Button.Text = "#" + item.tag;
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
        public void Add(Get_General_Data_Object.Trending_Hashtag hash)
        {
            try
            {
                var check = mHashtagList.FirstOrDefault(a => a.id == hash.id);
                if (check == null)
                {
                    mHashtagList.Add(hash);
                    NotifyItemInserted(mHashtagList.IndexOf(mHashtagList.Last()));
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
                mHashtagList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_General_Data_Object.Trending_Hashtag GetItem(int position)
        {
            return mHashtagList[position];
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

        private void OnClick(HashtagUser_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(HashtagUser_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class HashtagUser_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public HashtagUser_AdapterViewHolder(View itemView, Action<HashtagUser_AdapterClickEventArgs> clickListener,
            Action<HashtagUser_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Button = MainView.FindViewById<Button>(Resource.Id.cont);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new HashtagUser_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new HashtagUser_AdapterClickEventArgs
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

        public Button Button { get; set; }

        #endregion
    }

    public class HashtagUser_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}