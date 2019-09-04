using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.BlockedUsers;
using WoWonder.Helpers;

namespace WoWonder.Activities.InviteFriends.Adapters
{
    public class InviteFriends_Adapter : RecyclerView.Adapter
    {
        public int _position;
        public Context Activity_Context;

        public ObservableCollection<IMethods.PhoneContactManager.UserContact> mUsersPhoneContacts =
            new ObservableCollection<IMethods.PhoneContactManager.UserContact>();

        public InviteFriends_Adapter(Context context)
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
                if (mUsersPhoneContacts != null)
                    return mUsersPhoneContacts.Count;
                return 0;
            }
        }

        public event EventHandler<InviteFriends_AdapterClickEventArgs> ItemClick;
        public event EventHandler<InviteFriends_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Invite_Friends_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Invite_Friends_view, parent, false);
                var vh = new InviteFriends_AdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is InviteFriends_AdapterViewHolder holder)
                {
                    var item = mUsersPhoneContacts[_position];
                    if (item != null)
                    {
                        FontController.SetFont(holder.Txt_name, 1);
                        FontController.SetFont(holder.Txt_number, 3);
                        Initialize(holder, item);
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void Initialize(InviteFriends_AdapterViewHolder holder, IMethods.PhoneContactManager.UserContact users)
        {
            try
            {
                if (Settings.FlowDirection_RightToLeft)
                {
                    holder.RelativeLayout_main.LayoutDirection = LayoutDirection.Rtl;
                    holder.Txt_name.TextDirection = TextDirection.Rtl;
                    holder.Txt_number.TextDirection = TextDirection.Rtl;
                }

                string name = IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(mUsersPhoneContacts[_position].UserDisplayName));

                holder.Txt_name.Text = name;
                holder.Txt_number.Text = mUsersPhoneContacts[_position].PhoneNumber;

                var ImageTrancform = ImageService.Instance.LoadCompiledResource("no_profile_image.png");
                ImageTrancform.LoadingPlaceholder("no_profile_image.png", ImageSource.CompiledResource);
                ImageTrancform.ErrorPlaceholder("no_profile_image.png", ImageSource.CompiledResource);
                ImageTrancform.TransformPlaceholders(true);
                ImageTrancform.Transform(new CircleTransformation(5, "#ffffff"));

                ImageTrancform.Into(holder.ImageAvatar);
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

        // Function Users
        public void Add(IMethods.PhoneContactManager.UserContact User)
        {
            try
            {
                mUsersPhoneContacts.Add(User);
                NotifyItemInserted(mUsersPhoneContacts.IndexOf(mUsersPhoneContacts.Last()));
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public IMethods.PhoneContactManager.UserContact GetItem(int position)
        {
            return mUsersPhoneContacts[position];
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


        private void OnClick(InviteFriends_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(InviteFriends_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class InviteFriends_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public InviteFriends_AdapterViewHolder(View itemView, Action<InviteFriends_AdapterClickEventArgs> clickListener,
            Action<InviteFriends_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                RelativeLayout_main = (RelativeLayout) MainView.FindViewById(Resource.Id.main);
                Txt_name = (TextView) MainView.FindViewById(Resource.Id.name_Text);
                Txt_number = (TextView) MainView.FindViewById(Resource.Id.numberphone_Text);
                ImageAvatar = (ImageViewAsync) MainView.FindViewById(Resource.Id.Image_Avatar);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new InviteFriends_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new InviteFriends_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }

            itemView.Click += (sender, e) => clickListener(new InviteFriends_AdapterClickEventArgs
                {View = itemView, Position = AdapterPosition});
            itemView.LongClick += (sender, e) => longClickListener(new InviteFriends_AdapterClickEventArgs
                {View = itemView, Position = AdapterPosition});
        }

        #region Variables Basic

        public View MainView { get; }
        private BlockedUsers_Activity Activity_Context;

        public event EventHandler<int> ItemClick;
        public event EventHandler<int> ImageClick;

        public RelativeLayout RelativeLayout_main { get; }
        public TextView Txt_name { get; }
        public TextView Txt_number { get; }
        public ImageViewAsync ImageAvatar { get; }

        #endregion
    }

    public class InviteFriends_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}