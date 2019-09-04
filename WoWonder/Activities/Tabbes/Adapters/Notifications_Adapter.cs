using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;

namespace WoWonder.Activities.Tabbes.Adapters
{
    public class Notifications_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<Get_General_Data_Object.Notification> mNotificationsList =
            new ObservableCollection<Get_General_Data_Object.Notification>();

        public Notifications_Adapter(Context context)
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
                if (mNotificationsList != null)
                    return mNotificationsList.Count;
                return 0;
            }
        }

        public event EventHandler<Notifications_AdapterClickEventArgs> ItemClick;
        public event EventHandler<Notifications_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Notifications_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_Notifications_view, parent, false);
                var vh = new Notifications_AdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is Notifications_AdapterViewHolder holder)
                {
                    var item = mNotificationsList[_position];
                    if (item != null)
                    {
                        //Dont Remove this code #####
                        FontController.SetFont(holder.UserName_Notfy, 1);
                        FontController.SetFont(holder.Text_Notfy, 3);
                        //#####

                        Initialize(holder, item);

                        if (Settings.FlowDirection_RightToLeft)
                        {
                            holder.Layout_main.LayoutDirection = LayoutDirection.Rtl;
                            holder.Image_User.LayoutDirection = LayoutDirection.Rtl;
                            holder.Image.LayoutDirection = LayoutDirection.Rtl;
                            holder.Icon_Notfy.LayoutDirection = LayoutDirection.Rtl;
                            holder.UserName_Notfy.LayoutDirection = LayoutDirection.Rtl;
                            holder.Text_Notfy.LayoutDirection = LayoutDirection.Rtl;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Initialize(Notifications_AdapterViewHolder holder, Get_General_Data_Object.Notification notfy)
        {
            try
            {
                var AvatarSplit = notfy.notifier.avatar.Split('/').Last();
                var getImage_Avatar =
                    IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                if (getImage_Avatar != "File Dont Exists")
                {
                    if (holder.Image_User.Tag?.ToString() != "loaded")
                    {
                        ImageServiceLoader.Load_Image(holder.Image_User, "no_profile_image.png", getImage_Avatar, 1);
                        holder.Image_User.Tag = "loaded";
                    }
                }
                else
                {
                    if (holder.Image_User.Tag?.ToString() != "loaded")
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage,
                            notfy.notifier.avatar);
                        ImageServiceLoader.Load_Image(holder.Image_User, "no_profile_image.png", notfy.notifier.avatar, 1);
                        holder.Image_User.Tag = "loaded";
                    }
                }

                AddIconFonts(holder, notfy.type, notfy.icon);

                var drawable = TextDrawable.TextDrawable.TextDrawbleBuilder.BeginConfig().FontSize(30).EndConfig()
                    .BuildRound("", Color.ParseColor(GetColorFonts(notfy.type, notfy.icon)));
                holder.Image.SetImageDrawable(drawable);

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(notfy.notifier.name));

                holder.UserName_Notfy.Text = name;
                holder.Text_Notfy.Text = notfy.type_text;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public static string AddIconFonts(Notifications_AdapterViewHolder holder, string Type, string icon)
        {
            try
            {
                var Type_icon = "";

                if (Type == "following")
                {
                    Type_icon = "person-add";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.PersonAdd);
                    return Type_icon;
                }

                if (Type == "comment" || Type == "comment_reply" || Type == "also_replied")
                {
                    Type_icon = "chatboxes";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.IosChatboxes);
                    return Type_icon;
                }

                if (Type == "liked_post" || Type == "liked_comment" || Type == "liked_reply_comment" ||
                    Type == "liked_page")
                {
                    Type_icon = "ThumbUp";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Thumbsup);
                    return Type_icon;
                }

                if (Type == "wondered_post" || Type == "wondered_comment" || Type == "wondered_reply_comment" ||
                    Type == "exclamation-circle")
                {
                    Type_icon = "Information";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Information);
                    return Type_icon;
                }

                if (Type == "comment_mention" || Type == "comment_reply_mention")
                {
                    Type_icon = "Tag";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Pricetag);
                    return Type_icon;
                }

                if (Type == "post_mention")
                {
                    Type_icon = "at";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.At);
                    return Type_icon;
                }

                if (Type == "share_post")
                {
                    Type_icon = "Share";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.AndroidShareAlt);
                    return Type_icon;
                }

                if (Type == "profile_wall_post")
                {
                    Type_icon = "image";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Image);
                    return Type_icon;
                }

                if (Type == "visited_profile")
                {
                    Type_icon = "Eye";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Eye);
                    return Type_icon;
                }

                if (Type == "joined_group" || Type == "accepted_invite" || Type == "accepted_request" ||
                    Type == "accepted_join_request")
                {
                    Type_icon = "Check";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Checkmark);
                    return Type_icon;
                }

                if (Type == "invited_page")
                {
                    Type_icon = "Flag";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Flag);
                    return Type_icon;
                }

                if (Type == "added_you_to_group")
                {
                    Type_icon = "Adjust";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.AndroidAdd);
                    return Type_icon;
                }

                if (Type == "requested_to_join_group")
                {
                    Type_icon = "Clock";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.IosTimerOutline);
                    return Type_icon;
                }

                if (Type == "thumbs-down")
                {
                    Type_icon = "ThumbDown";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Thumbsdown);
                    return Type_icon;
                }

                if (Type == "going_event")
                {
                    Type_icon = "Calendar";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Calendar);
                    return Type_icon; 
                }

                if (Type == "viewed_story")
                {
                    Type_icon = "Calendar";
                    IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Aperture);
                    return Type_icon;
                }

                if (Type == "reaction")
                {
                    if (icon == "like")
                    {
                        Type_icon = "ThumbUp";
                        IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Thumbsup);
                        return Type_icon;
                    }
                    else if (icon == "haha")
                    {
                        Type_icon = "Happy";
                        IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Happy);
                        return Type_icon;
                    }
                    else if (icon == "love")
                    {
                        Type_icon = "Heart";
                        IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Heart);
                        return Type_icon;
                    }
                    else if (icon == "wow")
                    {
                        Type_icon = "Information";
                        IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.Information);
                        return Type_icon;
                    }
                    else if (icon == "sad")
                    {
                        Type_icon = "SadOutline";
                        IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.SadOutline);
                        return Type_icon;
                    }
                    else if(icon == "angry")
                    {
                        Type_icon = "SadOutline";
                        IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.SocialFreebsdDevil);
                        return Type_icon;
                    }
                    else
                    {
                        Type_icon = "ArrowRightDropCircleOutline";
                        IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.AndroidNotifications);
                        return Type_icon;
                    }
                }


                Type_icon = "ArrowRightDropCircleOutline";
                IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.AndroidNotifications);
                return Type_icon;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                IMethods.Set_TextViewIcon("1", holder.Icon_Notfy, IonIcons_Fonts.AndroidNotifications);
                return "";
            }
        }

        public static string GetColorFonts(string Type, string icon)
        {
            try
            {
                var Icon_Color_FO = "#424242";

                if (Type == "following")
                {
                    Icon_Color_FO = "#F50057";
                    return Icon_Color_FO;
                }

                if (Type == "comment" || Type == "comment_reply" || Type == "also_replied")
                {
                    Icon_Color_FO = Settings.MainColor;
                    return Icon_Color_FO;
                }

                if (Type == "liked_post" || Type == "liked_comment" || Type == "liked_reply_comment")
                {
                    Icon_Color_FO = Settings.MainColor;
                    return Icon_Color_FO;
                }

                if (Type == "wondered_post" || Type == "wondered_comment" || Type == "wondered_reply_comment" ||
                    Type == "exclamation-circle")
                {
                    Icon_Color_FO = "#FFA500";
                    return Icon_Color_FO;
                }

                if (Type == "comment_mention" || Type == "comment_reply_mention")
                {
                    Icon_Color_FO = "#B20000";

                    return Icon_Color_FO;
                }

                if (Type == "post_mention")
                {
                    Icon_Color_FO = "#B20000";
                    return Icon_Color_FO;
                }

                if (Type == "share_post")
                {
                    Icon_Color_FO = "#2F2FFF";
                    return Icon_Color_FO;
                }

                if (Type == "profile_wall_post")
                {
                    Icon_Color_FO = "#006064";
                    return Icon_Color_FO;
                }

                if (Type == "visited_profile")
                {
                    Icon_Color_FO = "#328432";
                    return Icon_Color_FO;
                }

                if (Type == "liked_page")
                {
                    Icon_Color_FO = "#2F2FFF";
                    return Icon_Color_FO;
                }

                if (Type == "joined_group" || Type == "accepted_invite" || Type == "accepted_request")
                {
                    Icon_Color_FO = "#2F2FFF";
                    return Icon_Color_FO;
                }

                if (Type == "invited_page")
                {
                    Icon_Color_FO = "#B20000";
                    return Icon_Color_FO;
                }

                if (Type == "accepted_join_request")
                {
                    Icon_Color_FO = "#2F2FFF";
                    return Icon_Color_FO;
                }

                if (Type == "added_you_to_group")
                {
                    Icon_Color_FO = "#311B92";
                    return Icon_Color_FO;
                }

                if (Type == "requested_to_join_group")
                {
                    Icon_Color_FO = Settings.MainColor;
                    return Icon_Color_FO;
                }

                if (Type == "thumbs-down")
                {
                    Icon_Color_FO = Settings.MainColor;
                    return Icon_Color_FO;
                }

                if (Type == "going_event")
                {
                    Icon_Color_FO = "#33691E";
                    return Icon_Color_FO;
                }
                
                if (Type == "viewed_story")
                {
                    Icon_Color_FO = "#D81B60";
                    return Icon_Color_FO;
                }

                if (Type == "reaction")
                {
                    if (icon == "like")
                    {
                        Icon_Color_FO = Settings.MainColor;
                        return Icon_Color_FO;
                    }
                    else if (icon == "haha")
                    {
                        Icon_Color_FO = "#0277BD";
                        return Icon_Color_FO; 
                    }
                    else if (icon == "love")
                    {
                        Icon_Color_FO = "#d50000";
                        return Icon_Color_FO; 
                    }
                    else if (icon == "wow")
                    {
                        Icon_Color_FO = "#FBC02D";
                        return Icon_Color_FO; 
                    }
                    else if (icon == "sad")
                    {
                        Icon_Color_FO = "#455A64";
                        return Icon_Color_FO; 
                    }
                    else if (icon == "angry")
                    {
                        Icon_Color_FO = "#BF360C";
                        return Icon_Color_FO; 
                    }
                    else
                    {
                        Icon_Color_FO = "#424242";
                        return Icon_Color_FO;
                    }
                }
                
                Icon_Color_FO = "#424242";
                return Icon_Color_FO;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return "#424242";
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

        // Function Users
        public void Add(Get_General_Data_Object.Notification notfy)
        {
            try
            {
                var check = mNotificationsList.FirstOrDefault(a => a.notifier_id == notfy.notifier_id);
                if (check == null)
                {
                    mNotificationsList.Add(notfy);
                    NotifyItemInserted(mNotificationsList.IndexOf(mNotificationsList.Last()));
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
                mNotificationsList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void Insert(Get_General_Data_Object.Notification users)
        {
            try
            {
                mNotificationsList.Insert(0, users);
                NotifyItemInserted(mNotificationsList.IndexOf(mNotificationsList.First()));
                NotifyItemRangeInserted(0, mNotificationsList.Count);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Get_General_Data_Object.Notification GetItem(int position)
        {
            return mNotificationsList[position];
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

        private void OnClick(Notifications_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(Notifications_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class Notifications_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public Notifications_AdapterViewHolder(View itemView, Action<Notifications_AdapterClickEventArgs> clickListener,
            Action<Notifications_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Layout_main = (LinearLayout) MainView.FindViewById(Resource.Id.main);

                //Get values
                Image_User = (ImageViewAsync) MainView.FindViewById(Resource.Id.ImageUser);
                Image = MainView.FindViewById<ImageView>(Resource.Id.image_view);
                Icon_Notfy = (TextView) MainView.FindViewById(Resource.Id.IconNotifications);
                UserName_Notfy = (TextView) MainView.FindViewById(Resource.Id.NotificationsName);
                Text_Notfy = (TextView) MainView.FindViewById(Resource.Id.NotificationsText);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new Notifications_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new Notifications_AdapterClickEventArgs
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

        public LinearLayout Layout_main;
        public ImageViewAsync Image_User { get; set; }
        public ImageView Image { get; set; }
        public TextView Icon_Notfy { get; set; }
        public TextView UserName_Notfy { get; set; }
        public TextView Text_Notfy { get; set; }

        #endregion
    }

    public class Notifications_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}