using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.AddPost.Adapters;
using WoWonder.Helpers;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.AddPost
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Mention_Activity : AppCompatActivity
    {
        public static MentionAdapter MentionsAdapter;

        public TextView Actionbutton;
        public RecyclerView MentionRecylerView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.MentionMain_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.MentionMain_Layout);

                MentionRecylerView = FindViewById<RecyclerView>(Resource.Id.Recyler);

                Actionbutton = FindViewById<TextView>(Resource.Id.actionbutton);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_MentionsFriend);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }


                LoadContacts();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();

                //Add Event
                Actionbutton.Click += Actionbutton_Click;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();

                //Close Event
                Actionbutton.Click -= Actionbutton_Click;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        private void Actionbutton_Click(object sender, EventArgs e)
        {
            try
            {
                var resultIntent = new Intent();
                resultIntent.PutExtra("Mentions", MentionsAdapter.MentionList.Where(a => a.Selected).ToString());
                SetResult(Result.Ok, resultIntent);
                Finish();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void LoadContacts()
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();
                var localList = dbDatabase.Get_MyContact(0, 25);
                if (localList != null)
                {
                    var list = localList.Select(user => new Mention
                    {
                        user_id = user.user_id,
                        username = user.username,
                        email = user.email,
                        first_name = user.first_name,
                        last_name = user.last_name,
                        avatar = user.avatar,
                        cover = user.cover,
                        relationship_id = user.relationship_id,
                        lastseen_time_text = user.lastseen_time_text,
                        address = user.address,
                        working = user.working,
                        working_link = user.working_link,
                        about = user.about,
                        school = user.school,
                        gender = user.gender,
                        birthday = user.birthday,
                        website = user.website,
                        facebook = user.facebook,
                        google = user.google,
                        twitter = user.twitter,
                        linkedin = user.linkedin,
                        youtube = user.youtube,
                        vk = user.vk,
                        instagram = user.instagram,
                        language = user.language,
                        ip_address = user.ip_address,
                        follow_privacy = user.follow_privacy,
                        friend_privacy = user.friend_privacy,
                        post_privacy = user.post_privacy,
                        message_privacy = user.message_privacy,
                        confirm_followers = user.confirm_followers,
                        show_activities_privacy = user.show_activities_privacy,
                        birth_privacy = user.birth_privacy,
                        visit_privacy = user.visit_privacy,
                        lastseen = user.lastseen,
                        showlastseen = user.showlastseen,
                        e_sentme_msg = user.e_sentme_msg,
                        e_last_notif = user.e_last_notif,
                        status = user.status,
                        active = user.active,
                        admin = user.admin,
                        registered = user.registered,
                        phone_number = user.phone_number,
                        is_pro = user.is_pro,
                        pro_type = user.pro_type,
                        joined = user.joined,
                        timezone = user.timezone,
                        referrer = user.referrer,
                        balance = user.balance,
                        paypal_email = user.paypal_email,
                        notifications_sound = user.notifications_sound,
                        order_posts_by = user.order_posts_by,
                        social_login = user.social_login,
                        device_id = user.device_id,
                        web_device_id = user.web_device_id,
                        wallet = user.wallet,
                        lat = user.lat,
                        lng = user.lng,
                        last_location_update = user.last_location_update,
                        share_my_location = user.share_my_location,
                        url = user.url,
                        name = user.name,
                        lastseen_unix_time = user.lastseen_unix_time,
                        user_platform = user.user_platform,
                        Selected = false
                    }).ToList();

                    MentionsAdapter = new MentionAdapter(this, new JavaList<Mention>(list));
                    MentionRecylerView.SetLayoutManager(new LinearLayoutManager(this));
                    MentionRecylerView.SetAdapter(MentionsAdapter);

                    if (MentionsAdapter.MentionList.Count > 0)
                    {
                        MentionsAdapter.NotifyDataSetChanged();

                        //Contacts_Empty.Visibility = ViewStates.Gone;
                        //ContactsRecyler.Visibility = ViewStates.Visible;
                    }
                }

                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }
    }
}