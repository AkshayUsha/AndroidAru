using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;
using WoWonder_API.Requests;
using Exception = System.Exception;

namespace WoWonder.Activities.MyContacts.Adapters
{
    public class MyContacts_Adapter : RecyclerView.Adapter
    {
        public static RecyclerView Recylercontrol;
        private readonly JavaList<Classes.UserContacts.User> CurrentList;

        public int _position;
        public Context Activity_Context;
        public JavaList<Classes.UserContacts.User> mMyContactsList = new JavaList<Classes.UserContacts.User>();

        public MyContacts_Adapter(Context activity, JavaList<Classes.UserContacts.User> MyContactsList,
            RecyclerView _Recylercontrol)
        {
            try
            {
                CurrentList = MyContactsList;
                mMyContactsList = MyContactsList;

                Activity_Context = activity;
                Recylercontrol = _Recylercontrol;
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
                try
                {
                    if (mMyContactsList == null || mMyContactsList.Count <= 0)
                        return 0;
                    return mMyContactsList.Count;
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                    return 0;
                }
            }
        }

        public Filter Filter => FilterHelper.NewInstance(CurrentList, this);

        public event EventHandler<MyContacts_AdapterClickEventArgs> ItemClick;
        public event EventHandler<MyContacts_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_HContact_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_HContact_view, parent, false);
                var vh = new MyContacts_AdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is MyContacts_AdapterViewHolder holder)
                {
                    var item = mMyContactsList[_position];
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

        public void Initialize(MyContacts_AdapterViewHolder holder, Classes.UserContacts.User users)
        {
            try
            {
                var AvatarSplit = users.avatar.Split('/').Last();
                var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                if (getImage_Avatar != "File Dont Exists")
                {
                    ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", getImage_Avatar, 1);
                }
                else
                {
                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, users.avatar);
                    ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", users.avatar, 1);
                }

                var CoverSplit = users.cover.Split('/').Last();
                var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, CoverSplit);
                if (getImage_Cover == "File Dont Exists")
                {
                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, users.cover);
                }

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(users.name));
                holder.Name.Text = IMethods.Fun_String.SubStringCutOf(name, 25);

                var dataabout = IMethods.Fun_String.StringNullRemover(users.about);
                if (dataabout != "Empty")
                {
                    var about = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(dataabout));
                    holder.About.Text = IMethods.Fun_String.SubStringCutOf(about, 25);
                }
                else
                {
                    var about = Activity_Context.GetText(Resource.String.Lbl_DefaultAbout) + " " +
                                Settings.Application_Name;
                    holder.About.Text = IMethods.Fun_String.SubStringCutOf(about, 25);
                }

                //Set All My Contacts  
                if (users.is_following == "1") // My Friend
                {
                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                    if (Settings.ConnectivitySystem == "1") // Following
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Following);
                    else // Friend
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Friends);

                    holder.Button.Tag = "friends";
                }
                else if (users.is_following == "2") // Request
                {
                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    holder.Button.SetTextColor(Color.ParseColor("#444444"));
                    holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Request);
                    holder.Button.Tag = "Request";
                }
                else if (users.is_following == "0") //Not Friend
                {
                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    holder.Button.SetTextColor(Color.ParseColor(Settings.MainColor));
                    if (Settings.ConnectivitySystem == "1") // Following
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Follow);
                    else // Friend
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_AddFriends);
                    holder.Button.Tag = "false";

                    var dbDatabase = new SqLiteDatabase();
                    dbDatabase.Delete_UsersContact(users.user_id);
                    dbDatabase.Dispose();
                }
                else
                {
                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                    if (Settings.ConnectivitySystem == "1") // Following
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Following);
                    else // Friend
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Friends);

                    users.is_following = "1";
                }

                holder.Button.Tag = "true";

                if (!holder.Button.HasOnClickListeners)
                    holder.Button.Click += (sender, args) =>
                    {
                        try
                        {
                            if (!IMethods.CheckConnectivity())
                            {
                                Toast.MakeText(Activity_Context,
                                    Activity_Context.GetString(Resource.String.Lbl_CheckYourInternetConnection),
                                    ToastLength.Short).Show();
                            }
                            else
                            {
                                if (holder.Button.Tag.ToString() == "false")
                                {
                                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                                    holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                                    if (Settings.ConnectivitySystem == "1") // Following
                                    {
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Following);
                                        holder.Button.Tag = "true";
                                    }
                                    else // Request Friend 
                                    {
                                        holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                                        holder.Button.SetTextColor(Color.ParseColor("#444444"));
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Request);
                                        holder.Button.Tag = "Request";
                                    }
                                }
                                else
                                {
                                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                                    holder.Button.SetTextColor(Color.ParseColor(Settings.MainColor));
                                    if (Settings.ConnectivitySystem == "1") // Following
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Follow);
                                    else // Friend
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_AddFriends);
                                    holder.Button.Tag = "false";

                                    var dbDatabase = new SqLiteDatabase();
                                    dbDatabase.Delete_UsersContact(users.user_id);
                                    dbDatabase.Dispose();
                                }

                                var result = Client.Global.Follow_User(users.user_id).ConfigureAwait(false);
                            }
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
        public void Add(Classes.UserContacts.User userFollower)
        {
            try
            {
                var check = mMyContactsList.FirstOrDefault(a => a.user_id == userFollower.user_id);
                if (check == null)
                {
                    mMyContactsList.Add(userFollower);
                    NotifyItemInserted(mMyContactsList.IndexOf(mMyContactsList.Last()));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void AddRange<T>(JavaList<T> collection, IEnumerable<T> items)
        {
            try
            {
                items.ToList().ForEach(collection.Add);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void Clear()
        {
            try
            {
                mMyContactsList?.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void Remove(Classes.UserContacts.User users)
        {
            try
            {
                var Index = mMyContactsList.IndexOf(mMyContactsList.FirstOrDefault(a => a.user_id == users.user_id));
                if (Index != -1)
                {
                    mMyContactsList.Remove(users);
                    NotifyItemRemoved(Index);
                    NotifyItemRangeRemoved(0, ItemCount);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Classes.UserContacts.User GetItem(int position)
        {
            return mMyContactsList[position];
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

        public void SetContacts(JavaList<Classes.UserContacts.User> filtereddata)
        {
            mMyContactsList = filtereddata;
        }


        private void OnClick(MyContacts_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(MyContacts_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class MyContacts_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public MyContacts_AdapterViewHolder(View itemView, Action<MyContacts_AdapterClickEventArgs> clickListener,
            Action<MyContacts_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.card_pro_pic);
                Name = MainView.FindViewById<TextView>(Resource.Id.card_name);
                About = MainView.FindViewById<TextView>(Resource.Id.card_dist);
                Button = MainView.FindViewById<Button>(Resource.Id.cont);

                //Event
                itemView.Click += (sender, e) => clickListener(new MyContacts_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new MyContacts_AdapterClickEventArgs
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
        public TextView About { get; set; }
        public Button Button { get; set; }

        #endregion
    }

    public class MyContacts_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }

    public class FilterHelper : Filter
    {
        private static JavaList<Classes.UserContacts.User> currentList;

        private static MyContacts_Adapter AdapterMain;
        private readonly IFilterable _adapter;

        public static FilterHelper NewInstance(JavaList<Classes.UserContacts.User> CurrentList,
            MyContacts_Adapter MAdapter)
        {
            AdapterMain = MAdapter;
            currentList = CurrentList;

            return new FilterHelper();
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            try
            {
                var filterResults = new FilterResults();
                var foundFilters = new JavaList<Classes.UserContacts.User>();

                if (constraint != null && constraint.Length() > 0)
                {
                    for (var i = 0; i < currentList.Size(); i++)
                    {
                        var Data = currentList[i];
                        var query = constraint.ToString().ToUpper();

                        if (Data.name.ToUpper().Contains(query)) foundFilters.Add(Data);
                    }

                    filterResults.Count = foundFilters.Count;
                    filterResults.Values = foundFilters;

                    return filterResults;
                }

                filterResults.Count = currentList.Count;
                filterResults.Values = currentList;
                return filterResults;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                var filterResults = new FilterResults();
                filterResults.Count = currentList.Count;
                filterResults.Values = currentList;
                return filterResults;
            }
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            try
            {
                AdapterMain.SetContacts((JavaList<Classes.UserContacts.User>) results.Values);
                AdapterMain.NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
    }
}