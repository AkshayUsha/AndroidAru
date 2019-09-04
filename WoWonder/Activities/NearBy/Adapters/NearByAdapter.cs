using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using Refractored.Controls;
using SettingsConnecter;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;

namespace WoWonder.Activities.NearBy.Adapters
{
    public class NearByAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_Nearby_Users_Object.Nearby_Users> mNearByList =
            new ObservableCollection<Get_Nearby_Users_Object.Nearby_Users>();

        public NearByAdapter(Context context)
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
                if (mNearByList != null)
                    return mNearByList.Count;
                return 0;
            }
        }

        public event EventHandler<NearByAdapterClickEventArgs> ItemClick;

        public event EventHandler<NearByAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_NearBy_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_NearBy_view, parent, false);
                var vh = new NearByAdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is NearByAdapterViewHolder holder)
                {
                    var item = mNearByList[_position];
                    if (item != null)
                    {
                        //Dont Remove this code #####
                        FontController.SetFont(holder.Name, 1);
                        FontController.SetFont(holder.LastTimeOnline, 3);
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

        public void Initialize(NearByAdapterViewHolder holder, Get_Nearby_Users_Object.Nearby_Users users)
        {
            try
            {
                if (holder.Image.Tag?.ToString() != "loaded")
                {
                    var AvatarSplit = users.avatar.Split('/').Last();
                    var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", getImage_Avatar, 1);
                        holder.Image.Tag = "loaded";
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, users.avatar);
                        ImageServiceLoader.Load_Image(holder.Image, "no_profile_image.png", users.avatar, 1);
                    }
                    holder.Image.Tag = "loaded";
                }

              
                //Online Or offline
                if (users.lastseen_status == "on")
                {
                    //Online
                    if (holder.ImageOnline.Tag?.ToString() != "true")
                    {
                        holder.ImageOnline.Tag = "true";
                        holder.ImageOnline.SetImageResource(Resource.Drawable.Green_Color);
                    }

                    if (holder.LastTimeOnline.Tag?.ToString() != "true")
                    {
                        holder.LastTimeOnline.Tag = "true";
                        holder.LastTimeOnline.Text = Activity_Context.GetString(Resource.String.Lbl_Online);
                    }
                }
                else
                {
                    if (holder.ImageOnline.Tag?.ToString() != "true")
                    {
                        holder.ImageOnline.Tag = "true";
                        holder.ImageOnline.SetImageResource(Resource.Drawable.Grey_Offline);
                    }

                    if (holder.LastTimeOnline.Tag?.ToString() != "true")
                    {
                        holder.LastTimeOnline.Tag = "true";
                        holder.LastTimeOnline.Text = IMethods.ITime.TimeAgo(int.Parse(users.lastseen_unix_time));
                    }
                }

                if (holder.Name.Tag?.ToString() != "true")
                {
                    holder.Name.Tag = "true";
                    string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(users.name));
                    holder.Name.Text = IMethods.Fun_String.SubStringCutOf(name, 14);
                }

                if (users.is_following == "yes" || users.is_following == "Yes") // My Friend
                {
                    if (holder.Button.Tag?.ToString() != "friends")
                    {
                        holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                        holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                        if (Settings.ConnectivitySystem == "1") // Following
                            holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Following);
                        else // Friend
                            holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Friends);

                        holder.Button.Tag = "friends";
                    }
                }
                else //Not Friend
                {
                    if (holder.Button.Tag?.ToString() != "false")
                    {
                        holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                        holder.Button.SetTextColor(Color.ParseColor(Settings.MainColor));
                        if (Settings.ConnectivitySystem == "1") // Following
                            holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Follow);
                        else // Friend
                            holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_AddFriends);
                        holder.Button.Tag = "false";
                    }

                    
                }

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
        public void Add(Get_Nearby_Users_Object.Nearby_Users user)
        {
            try
            {
                var check = mNearByList.FirstOrDefault(a => a.user_id == user.user_id);
                if (check == null)
                {
                    mNearByList.Add(user);
                    NotifyItemInserted(mNearByList.IndexOf(mNearByList.Last()));
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
                mNearByList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_Nearby_Users_Object.Nearby_Users GetItem(int position)
        {
            return mNearByList[position];
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

        private void OnClick(NearByAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(NearByAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class NearByAdapterViewHolder : RecyclerView.ViewHolder
    {
        public NearByAdapterViewHolder(View itemView, Action<NearByAdapterClickEventArgs> clickListener,
            Action<NearByAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageViewAsync>(Resource.Id.people_profile_sos);
                ImageOnline = MainView.FindViewById<CircleImageView>(Resource.Id.ImageLastseen);
                Name = MainView.FindViewById<TextView>(Resource.Id.people_profile_name);
                LastTimeOnline = MainView.FindViewById<TextView>(Resource.Id.people_profile_time);
                Button = MainView.FindViewById<Button>(Resource.Id.btn_follow_people);

                //Event
                itemView.Click += (sender, e) => clickListener(new NearByAdapterClickEventArgs
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

        public ImageViewAsync Image { get; set; }
        public CircleImageView ImageOnline { get; set; }

        public TextView Name { get; set; }
        public TextView LastTimeOnline { get; set; }
        public Button Button { get; set; }

        #endregion Variables Basic
    }

    public class NearByAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}