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
using WoWonder.Helpers;

namespace WoWonder.Activities.MyProfile.Adapters
{
    public class SocialItem
    {
        public int ID { get; set; }
        public string SocialName { get; set; }
        public string SocialIcon { get; set; }
        public Color IconColor { get; set; }
        public string SocialLinkName { get; set; }
        public bool Checkvisibilty { get; set; }
    }

    public class SocialLinks_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;
        public ObservableCollection<SocialItem> SocialList = new ObservableCollection<SocialItem>();


        public SocialLinks_Adapter(Context context)
        {
            try
            {
                Activity_Context = context;

                SocialList.Add(new SocialItem
                {
                    ID = 1,
                    SocialName = Activity_Context.GetText(Resource.String.Lbl_Facebook),
                    SocialLinkName = "",
                    Checkvisibilty = false,
                    SocialIcon = IonIcons_Fonts.SocialFacebook,
                    IconColor = Color.ParseColor("#3b5999")
                });

                SocialList.Add(new SocialItem
                {
                    ID = 2,
                    SocialName = Activity_Context.GetText(Resource.String.Lbl_Twitter),
                    SocialLinkName = "",
                    Checkvisibilty = false,
                    SocialIcon = IonIcons_Fonts.SocialTwitter,
                    IconColor = Color.ParseColor("#55acee")
                });

                SocialList.Add(new SocialItem
                {
                    ID = 3,
                    SocialName = Activity_Context.GetText(Resource.String.Lbl_GooglePlus) + "+",
                    SocialLinkName = "",
                    Checkvisibilty = false,
                    SocialIcon = IonIcons_Fonts.SocialGoogle,
                    IconColor = Color.ParseColor("#dd4b39")
                });

                SocialList.Add(new SocialItem
                {
                    ID = 4,
                    SocialName = Activity_Context.GetText(Resource.String.Lbl_Vk),
                    SocialLinkName = "",
                    Checkvisibilty = false,
                    SocialIcon = "\uf189",
                    IconColor = Color.ParseColor("#4c75a3")
                });

                SocialList.Add(new SocialItem
                {
                    ID = 5,
                    SocialName = Activity_Context.GetText(Resource.String.Lbl_Linkedin),
                    SocialLinkName = "",
                    Checkvisibilty = false,
                    SocialIcon = IonIcons_Fonts.SocialLinkedin,
                    IconColor = Color.ParseColor("#0077B5")
                });

                SocialList.Add(new SocialItem
                {
                    ID = 6,
                    SocialName = Activity_Context.GetText(Resource.String.Lbl_Instagram),
                    SocialLinkName = "",
                    Checkvisibilty = false,
                    SocialIcon = IonIcons_Fonts.HappyOutline,
                    IconColor = Color.ParseColor("#e4405f")
                });

                SocialList.Add(new SocialItem
                {
                    ID = 7,
                    SocialName = Activity_Context.GetText(Resource.String.Lbl_YouTube),
                    SocialLinkName = "",
                    Checkvisibilty = false,
                    SocialIcon = IonIcons_Fonts.SocialYoutube,
                    IconColor = Color.ParseColor("#cd201f")
                });
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
                if (SocialList != null)
                    return SocialList.Count;
                return 0;
            }
        }

        public event EventHandler<SocialLinks_AdapterClickEventArgs> ItemClick;
        public event EventHandler<SocialLinks_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> ChannelSubscribed_View
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_SocialLinks_View, parent, false);
                var vh = new SocialLinks_AdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is SocialLinks_AdapterViewHolder holder)
                {
                    var item = SocialList[position];
                    if (item != null)
                    {
                        //Dont Remove this code #####
                        FontController.SetFont(holder.NameSocial, 1);
                        FontController.SetFont(holder.NameLink, 1);
                        //#####

                        string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(item.SocialName));
                        holder.NameSocial.Text = IMethods.Fun_String.SubStringCutOf(name, 20);

                        if (item.ID == 4)
                            IMethods.Set_TextViewIcon("3", holder.IconSocial, item.SocialIcon);
                        else
                            IMethods.Set_TextViewIcon("1", holder.IconSocial, item.SocialIcon);

                        holder.IconSocial.SetTextColor(item.IconColor);

                        if (item.Checkvisibilty)
                        {
                            IMethods.Set_TextViewIcon("1", holder.IconCheck, IonIcons_Fonts.Checkmark);
                            holder.IconCheck.SetTextColor(Color.ParseColor(Settings.MainColor));

                            holder.NameLink.Text = item.SocialLinkName;
                            holder.NameLink.SetTextColor(Color.ParseColor(Settings.MainColor));

                            holder.Layout_Checkvisibilty.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            holder.Layout_Checkvisibilty.Visibility = ViewStates.Invisible;
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

        public void Update(SocialItem item, string linkName)
        {
            try
            {
                var data = SocialList.FirstOrDefault(a => a.ID == item.ID);
                if (data != null)
                {
                    if (!string.IsNullOrEmpty(linkName))
                    {
                        data.SocialLinkName = linkName;
                        data.Checkvisibilty = true;
                    }
                    else
                    {
                        data.Checkvisibilty = false;
                    }

                    NotifyItemChanged(SocialList.IndexOf(data));
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public SocialItem GetItem(int position)
        {
            return SocialList[position];
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

        private void OnClick(SocialLinks_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(SocialLinks_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class SocialLinks_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public SocialLinks_AdapterViewHolder(View itemView, Action<SocialLinks_AdapterClickEventArgs> clickListener,
            Action<SocialLinks_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                IconSocial = MainView.FindViewById<TextView>(Resource.Id.Social_Icon);
                NameSocial = MainView.FindViewById<TextView>(Resource.Id.Social_name);
                IconCheck = MainView.FindViewById<TextView>(Resource.Id.Icon_Check);
                NameLink = MainView.FindViewById<TextView>(Resource.Id.Link_name);

                Layout_Checkvisibilty = MainView.FindViewById<RelativeLayout>(Resource.Id.icon_container);

                itemView.Click += (sender, e) => clickListener(new SocialLinks_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new SocialLinks_AdapterClickEventArgs
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

        public TextView IconSocial { get; set; }
        public TextView IconCheck { get; set; }
        public TextView NameSocial { get; set; }
        public TextView NameLink { get; set; }
        public RelativeLayout Layout_Checkvisibilty { get; set; }

        #endregion
    }

    public class SocialLinks_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}