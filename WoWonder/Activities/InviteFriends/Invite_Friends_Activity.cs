using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.InviteFriends.Adapters;
using WoWonder.Helpers;
using Exception = Java.Lang.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.InviteFriends
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Invite_Friends_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Invite_Friends_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.Invite_Friends_Layout);

                //Get values
                InviteFriendsRecyler = FindViewById<RecyclerView>(Resource.Id.InviteFriends_Recylerview);

                //Set ToolBar
                var ToolBar = FindViewById<Toolbar>(Resource.Id.Searchtoolbar);
                ToolBar.Title = GetString(Resource.String.Lbl_Invite_Friends);

                SetSupportActionBar(ToolBar);

                SupportActionBar.SetDisplayShowCustomEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);

                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                    GetAllContacts();
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.ReadContacts) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.ReadPhoneNumbers) == Permission.Granted)
                        GetAllContacts();
                    else
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.ReadContacts,
                            Manifest.Permission.ReadPhoneNumbers
                        }, 208);
                }
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
                GC.Collect(0);
                base.OnResume();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void GetAllContacts()
        {
            try
            {
                var listContacts =
                    new ObservableCollection<IMethods.PhoneContactManager.UserContact>(IMethods.PhoneContactManager
                        .GetAllContacts());
                var OrderBydate = listContacts.OrderBy(a => a.UserDisplayName);

                //Set Adapter
                mLayoutManager = new LinearLayoutManager(this);
                InviteFriendsRecyler.SetLayoutManager(mLayoutManager);

                mAdapter = new InviteFriends_Adapter(this);
                mAdapter.mUsersPhoneContacts =
                    new ObservableCollection<IMethods.PhoneContactManager.UserContact>(OrderBydate);
                //Event
                mAdapter.ItemClick += MAdapterOnItemClick;

                InviteFriendsRecyler.SetAdapter(mAdapter);

                InviteSMSText = GetText(Resource.String.Lbl_InviteSMSText_1) + " " + Settings.Application_Name + " " +
                                GetText(Resource.String.Lbl_InviteSMSText_2);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        private void MAdapterOnItemClick(object sender, InviteFriends_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var Position = adapterClickEvents.Position;
                if (Position >= 0)
                {
                    var item = mAdapter.GetItem(Position);

                    Contact = item;
                    if (item != null)
                    {
                        // Check if we're running on Android 5.0 or higher
                        if ((int) Build.VERSION.SdkInt < 23)
                        {
                            IMethods.IApp.SendSMS(this, item.PhoneNumber, InviteSMSText);
                        }
                        else
                        {
                            //Check to see if any permission in our group is available, if one, then all are
                            if (CheckSelfPermission(Manifest.Permission.SendSms) == Permission.Granted)
                                IMethods.IApp.SendSMS(this, item.PhoneNumber, InviteSMSText);
                            else
                                RequestPermissions(new[]
                                {
                                    Manifest.Permission.SendSms,
                                    Manifest.Permission.BroadcastSms
                                }, 105);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 105)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        IMethods.IApp.SendSMS(this, Contact.PhoneNumber, InviteSMSText);
                    else
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                }
                else if (requestCode == 208)
                {
                    GetAllContacts();
                }
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
            catch (System.Exception exception)
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
            catch (System.Exception exception)
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
            catch (System.Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Variables Basic

        private RecyclerView InviteFriendsRecyler;
        private RecyclerView.LayoutManager mLayoutManager;
        public InviteFriends_Adapter mAdapter;

        public IMethods.PhoneContactManager.UserContact Contact = new IMethods.PhoneContactManager.UserContact();

        public string InviteSMSText = "";

        #endregion
    }
}