using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.Communities.Groups;
using WoWonder.Activities.Communities.Pages;
using WoWonder.Activities.Market;
using WoWonder.Helpers;

namespace WoWonder.Adapters
{
    public class Categories_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Classes.Catigories>
            mCategoriesList = new ObservableCollection<Classes.Catigories>();

        public Categories_Adapter(Context context)
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
                if (mCategoriesList != null)
                    return mCategoriesList.Count;
                return 0;
            }
        }

        public event EventHandler<Categories_AdapterClickEventArgs> ItemClick;
        public event EventHandler<Categories_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Categories_View
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Categories_View, parent, false);
                var vh = new Categories_AdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is Categories_AdapterViewHolder holder)
                {
                    var item = mCategoriesList[_position];
                    if (item != null)
                    {
                        //Dont Remove this code #####
                        FontController.SetFont(holder.Button, 1);
                        //#####

                        if (item.Catigories_Color == Settings.MainColor)
                            holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                        else
                            holder.Button.SetBackgroundResource(Resource.Drawable.Categories_button);

                        holder.Button.SetTextColor(Color.ParseColor(item.Catigories_Color));
                        holder.Button.Text = item.Catigories_Name;
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

        public void Click_Categories(Classes.Catigories cat)
        {
            try
            {
                var check = mCategoriesList.Where(a => a.Catigories_Color == Settings.MainColor).ToList();
                if (check.Count > 0)
                    foreach (var all in check)
                        all.Catigories_Color = "#212121";

                var click = mCategoriesList.FirstOrDefault(a => a.Catigories_Id == cat.Catigories_Id);
                if (click != null) click.Catigories_Color = Settings.MainColor;

                NotifyDataSetChanged();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Set_Categories(string Catigories_Id, string type_Catigories)
        {
            try
            {
                var click = mCategoriesList.FirstOrDefault(a => a.Catigories_Id == Catigories_Id);
                if (click != null)
                {
                    click.Catigories_Color = Settings.MainColor;

                    //Scroll Down >> 
                    if (type_Catigories == "EditInfoGroup")
                    {
                        EditInfoGroup_Activity.CategoriesRecyclerView.ScrollToPosition(mCategoriesList.IndexOf(click));
                    }
                    else if (type_Catigories == "EditInfoPage")
                    {
                        EditInfoPage_Activity.CategoriesRecyclerView.ScrollToPosition(mCategoriesList.IndexOf(click));
                    }
                    else if (type_Catigories == "CreateProduct")
                    {
                        CreateProduct_Activity.CategoriesRecyclerView.ScrollToPosition(mCategoriesList.IndexOf(click));
                    }
                }

                NotifyDataSetChanged();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        // Function 
        public void Add(Classes.Catigories cat)
        {
            try
            {
                var check = mCategoriesList.FirstOrDefault(a => a.Catigories_Id == cat.Catigories_Id);
                if (check == null)
                {
                    mCategoriesList.Add(cat);
                    NotifyItemInserted(mCategoriesList.IndexOf(mCategoriesList.Last()));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public Classes.Catigories GetItem(int position)
        {
            return mCategoriesList[position];
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

        private void OnClick(Categories_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(Categories_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class Categories_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public Categories_AdapterViewHolder(View itemView, Action<Categories_AdapterClickEventArgs> clickListener,
            Action<Categories_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Button = MainView.FindViewById<Button>(Resource.Id.cont);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new Categories_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new Categories_AdapterClickEventArgs
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

    public class Categories_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}