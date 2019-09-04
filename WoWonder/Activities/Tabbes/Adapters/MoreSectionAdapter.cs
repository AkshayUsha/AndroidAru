using System;
using System.Collections.ObjectModel;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Helpers;

namespace WoWonder.Activities.Tabbes.Adapters
{
    public class SectionItem
    {
        public int ID { get; set; }
        public string SectionName { get; set; }
        public string Icon { get; set; }
        public Color IconColor { get; set; }
        public int BadgeCount { get; set; }
        public bool Badgevisibilty { get; set; }
    }

    public class MoreSectionAdapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;
        public ObservableCollection<SectionItem> SectionList = new ObservableCollection<SectionItem>();

        public MoreSectionAdapter(Context context)
        {
            Activity_Context = context;

            SectionList.Add(new SectionItem
            {
                ID = 1,
                SectionName = Activity_Context.GetText(Resource.String.Lbl_MyProfile),
                BadgeCount = 0,
                Badgevisibilty = false,
                Icon = IonIcons_Fonts.HappyOutline,
                IconColor = Color.ParseColor("#047cac")
            });
            if (Settings.Messenger_Integration)
                SectionList.Add(new SectionItem
                {
                    ID = 2,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_Messages),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.Chatbubble,
                    IconColor = Color.ParseColor("#03a9f4")
                });
            if (Settings.Show_UserContacts)
            {
                string name = "";
                if (Settings.ConnectivitySystem == "1") // Following
                    name = Activity_Context.GetText(Resource.String.Lbl_Following);
                else // Friend
                    name = Activity_Context.GetText(Resource.String.Lbl_Friends);

                SectionList.Add(new SectionItem
                {
                    ID = 3,
                    SectionName = name,
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.PersonStalker,
                    IconColor = Color.ParseColor("#d80073")
                });

            }
            if (Settings.Show_Album)
                SectionList.Add(new SectionItem
                {
                    ID = 4,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_Albums),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.Images,
                    IconColor = Color.ParseColor("#8bc34a")
                });
            if (Settings.Show_SavedPost)
                SectionList.Add(new SectionItem
                {
                    ID = 5,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_Saved_Posts),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.Bookmark,
                    IconColor = Color.ParseColor("#673ab7")
                });
            if (Settings.Show_Communitie_Groups)
                SectionList.Add(new SectionItem
                {
                    ID = 6,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_Groups),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.AndroidApps,
                    IconColor = Color.ParseColor("#03A9F4")
                });
            if (Settings.Show_Communities_Pages)
                SectionList.Add(new SectionItem
                {
                    ID = 7,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_Pages),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.Flag,
                    IconColor = Color.ParseColor("#f79f58")
                });
            if (Settings.Show_Articles)
                SectionList.Add(new SectionItem
                {
                    ID = 8,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_Blogs),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.IosBook,
                    IconColor = Color.ParseColor("#f35d4d")
                });
            if (Settings.Show_Market)
                SectionList.Add(new SectionItem
                {
                    ID = 9,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_Marketplace),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.Bag,
                    IconColor = Color.ParseColor("#7d8250")
                });
            if (Settings.Show_Events)
                SectionList.Add(new SectionItem
                {
                    ID = 10,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_Events),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.Calendar,
                    IconColor = Color.ParseColor("#f25e4e")
                });
            if (Settings.Show_NearBy)
                SectionList.Add(new SectionItem
                {
                    ID = 11,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_FindFriends),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.Location,
                    IconColor = Color.ParseColor("#b2c17c")
                });
            if (Settings.Show_Movies)
                SectionList.Add(new SectionItem
                {
                    ID = 12,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_Movies),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.FilmMarker,
                    IconColor = Color.ParseColor("#00695C")
                });
            //Settings Page
            if (Settings.Show_Settings_GeneralAccount)
                SectionList.Add(new SectionItem
                {
                    ID = 13,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_GeneralAccount),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.Settings,
                    IconColor = Color.ParseColor("#616161")
                });
            if (Settings.Show_Settings_Privacy)
                SectionList.Add(new SectionItem
                {
                    ID = 14,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_Privacy),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.Eye,
                    IconColor = Color.ParseColor("#616161")
                });
            if (Settings.Show_Settings_Notification)
                SectionList.Add(new SectionItem
                {
                    ID = 15,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_Notifcations),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.IosBell,
                    IconColor = Color.ParseColor("#616161")
                });
            if (Settings.Show_Settings_InviteFriends)
                SectionList.Add(new SectionItem
                {
                    ID = 16,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_Tell_Friends),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.Email,
                    IconColor = Color.ParseColor("#616161")
                });
            //if (Settings.Show_Settings_ClearCache)
            //{
            //    var firstWord = Settings.Application_Name.Substring(0, Settings.Application_Name.IndexOf(" ", StringComparison.Ordinal));
            //    SectionList.Add(new SectionItem
            //    {
            //        ID = 17,
            //        SectionName = this.Activity_Context.GetText(Resource.String.Lbl_CleanCashed),
            //        BadgeCount = 0,
            //        Badgevisibilty = false,
            //        Icon = IonIcons_Fonts.TrashA,
            //        IconColor = Color.ParseColor("#616161")
            //    });
            //}
            if (Settings.Show_Settings_Help_Support)
                SectionList.Add(new SectionItem
                {
                    ID = 18,
                    SectionName = Activity_Context.GetText(Resource.String.Lbl_Help_Support),
                    BadgeCount = 0,
                    Badgevisibilty = false,
                    Icon = IonIcons_Fonts.Help,
                    IconColor = Color.ParseColor("#616161")
                });
            SectionList.Add(new SectionItem
            {
                ID = 19,
                SectionName = Activity_Context.GetText(Resource.String.Lbl_Logout),
                BadgeCount = 0,
                Badgevisibilty = false,
                Icon = IonIcons_Fonts.LogOut,
                IconColor = Color.ParseColor("#d50000")
            });
        }

        public override int ItemCount
        {
            get
            {
                if (SectionList != null)
                    return SectionList.Count;
                return 0;
            }
        }

        public event EventHandler<MoreSectionAdapterClickEventArgs> ItemClick;
        public event EventHandler<MoreSectionAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> ChannelSubscribed_View
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_MoreSection_view, parent, false);
                var vh = new MoreSectionAdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is MoreSectionAdapterViewHolder holder)
                {
                    if (Settings.FlowDirection_RightToLeft)
                    {
                        holder.LinearLayout_image.LayoutDirection = LayoutDirection.Rtl;
                        holder.LinearLayout_main.LayoutDirection = LayoutDirection.Rtl;
                        holder.Name.LayoutDirection = LayoutDirection.Rtl;
                    }
                     
                    var item = SectionList[position];
                    if (item != null)
                    {
                        //Dont Remove this code #####
                        FontController.SetFont(holder.Name, 1);
                        //#####

                        IMethods.Set_TextViewIcon("1", holder.Icon, item.Icon);
                        holder.Icon.SetTextColor(item.IconColor);
                        holder.Name.Text = item.SectionName;

                        if (item.BadgeCount != 0)
                            holder.Badge.Text = item.BadgeCount.ToString();

                        if (item.Badgevisibilty)
                            holder.Badge.Visibility = ViewStates.Visible;
                        else
                            holder.Badge.Visibility = ViewStates.Invisible;
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public SectionItem GetItem(int position)
        {
            return SectionList[position];
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

        private void OnClick(MoreSectionAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(MoreSectionAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class MoreSectionAdapterViewHolder : RecyclerView.ViewHolder
    {
        public MoreSectionAdapterViewHolder(View itemView, Action<MoreSectionAdapterClickEventArgs> clickListener,Action<MoreSectionAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                LinearLayout_main = MainView.FindViewById<LinearLayout>(Resource.Id.main);
                LinearLayout_image = MainView.FindViewById<RelativeLayout>(Resource.Id.imagecontainer);

                Icon = MainView.FindViewById<TextView>(Resource.Id.Icon);
                Name = MainView.FindViewById<TextView>(Resource.Id.section_name);
                Badge = MainView.FindViewById<TextView>(Resource.Id.badge);

                itemView.Click += (sender, e) => clickListener(new MoreSectionAdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public View MainView { get; }

        public LinearLayout LinearLayout_main { get; set; }
        public RelativeLayout LinearLayout_image { get; set; }
        public TextView Icon { get; set; }
        public TextView Name { get; set; }
        public TextView Badge { get; set; }

        public event EventHandler<int> ItemClick;
    }

    public class MoreSectionAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}