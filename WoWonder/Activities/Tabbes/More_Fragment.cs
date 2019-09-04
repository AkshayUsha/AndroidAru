using System;
using AFollestad.MaterialDialogs;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.Album;
using WoWonder.Activities.Articles;
using WoWonder.Activities.Communities.Groups;
using WoWonder.Activities.Communities.Pages;
using WoWonder.Activities.Events;
using WoWonder.Activities.Market;
using WoWonder.Activities.Movies;
using WoWonder.Activities.MyContacts;
using WoWonder.Activities.MyProfile;
using WoWonder.Activities.NearBy;
using WoWonder.Activities.SettingsPreferences.General;
using WoWonder.Activities.SettingsPreferences.Notification;
using WoWonder.Activities.SettingsPreferences.Privacy;
using WoWonder.Activities.SettingsPreferences.Support;
using WoWonder.Activities.SettingsPreferences.TellFriend;
using WoWonder.Activities.Tabbes.Adapters;
using WoWonder.Activities.UsersPages;
using WoWonder.Helpers;
using Exception = System.Exception;
using Fragment = Android.Support.V4.App.Fragment;

namespace WoWonder.Activities.Tabbes
{
    public class More_Fragment : Fragment, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        public void OnSelection(MaterialDialog p0, View p1, int p2, ICharSequence p3)
        {
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                    {
                        Toast.MakeText(Activity, Activity.GetText(Resource.String.Lbl_You_will_be_logged),ToastLength.Long).Show();
                        API_Request.Logout(Activity);
                    }
                    else
                    {
                        if (Activity.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted && Activity.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                        {
                            Toast.MakeText(Activity, Activity.GetText(Resource.String.Lbl_You_will_be_logged), ToastLength.Long).Show();
                            API_Request.Logout(Activity);
                        }
                        else
                            RequestPermissions(new[]
                            {
                                Manifest.Permission.ReadExternalStorage,
                                Manifest.Permission.WriteExternalStorage
                            }, 101);
                    }
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                var view = MyContextWrapper.GetContentView(Context, Settings.Lang, Resource.Layout.Tab_More_Layout);
                if (view == null) view = inflater.Inflate(Resource.Layout.Tab_More_Layout, container, false);

                MoreRecylerView = (RecyclerView) view.FindViewById(Resource.Id.Recyler);

                MoreRecylerView.SetLayoutManager(new LinearLayoutManager(Activity));
                MoreSectionAdapter = new MoreSectionAdapter(Activity);
                MoreRecylerView.SetAdapter(MoreSectionAdapter);
                MoreRecylerView.NestedScrollingEnabled = true;

                if (!Settings.SetTabOnButton)
                {
                    var parasms = (LinearLayout.LayoutParams) MoreRecylerView.LayoutParameters;
                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                        parasms.TopMargin = 130;
                    else
                        parasms.TopMargin = 270;

                    MoreRecylerView.LayoutParameters = parasms;
                    MoreRecylerView.SetPadding(0, 0, 0, 0);
                }

                return view;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return null;
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();

                //Add Event
                MoreSectionAdapter.ItemClick += MoreSection_OnItemClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                base.OnPause();

                //Close Event
                MoreSectionAdapter.ItemClick -= MoreSection_OnItemClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Open Intent Activity
        private void MoreSection_OnItemClick(object sender, MoreSectionAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = MoreSectionAdapter.GetItem(position);
                    if (item != null)
                    {
                        if (item.ID == 1) // My Profile
                        {
                            var intent = new Intent(Context, typeof(MyProfile_Activity));
                            StartActivity(intent);
                        }
                        else if (item.ID == 2) // Messages
                        {
                            IMethods.IApp.OpenApp_BypackageName(Context, Settings.Messenger_Package_Name);
                        }
                        else if (item.ID == 3) // Contacts
                        {
                            var intent = new Intent(Context, typeof(MyContacts_Activity));
                            intent.PutExtra("ContactsType", "Following");
                            StartActivity(intent);
                        }
                        else if (item.ID == 4) // Albums
                        {
                            var intent = new Intent(Context, typeof(MyPhotosActivity));
                            intent.PutExtra("UserId", UserDetails.User_id);
                            StartActivity(intent);
                        }
                        else if (item.ID == 5) // Saved Posts
                        {
                            var intent = new Intent(Context, typeof(HyberdPostViewer_Activity));
                            intent.PutExtra("Type", "Saved Post");
                            intent.PutExtra("Id", UserDetails.User_id);
                            intent.PutExtra("Title", this.Context.GetString(Resource.String.Lbl_Saved_Posts));
                            StartActivity(intent);
                        }
                        else if (item.ID == 6) // Groups
                        {
                            var intent = new Intent(Context, typeof(Groups_Activity));
                            intent.PutExtra("GroupsType", "Manage_MyGroups");
                            intent.PutExtra("UserID", UserDetails.User_id);
                            StartActivity(intent);
                        }
                        else if (item.ID == 7) // Pages
                        {
                            var intent = new Intent(Context, typeof(Pages_Activity));
                            intent.PutExtra("PagesType", "Manage_MyPages");
                            intent.PutExtra("UserID", UserDetails.User_id);
                            StartActivity(intent);
                        }
                        else if (item.ID == 8) // Blogs
                        {
                            StartActivity(new Intent(Context, typeof(ArticlesActivity)));
                        }
                        else if (item.ID == 9) // Market
                        {
                            StartActivity(new Intent(Context, typeof(TabbedMarket_Activity)));
                        }
                        else if (item.ID == 10) // Events
                        {
                            var intent = new Intent(Context, typeof(EventMain_Activity));
                            StartActivity(intent);
                        }
                        else if (item.ID == 11) // Find Friends
                        {
                            var intent = new Intent(Context, typeof(PeopleNearByActivity));
                            StartActivity(intent);
                        }
                        else if (item.ID == 12) // Movies
                        {
                            var intent = new Intent(Context, typeof(Movies_Activity));
                            StartActivity(intent);
                        }
                        //Settings Page
                        else if (item.ID == 13) // General Account
                        {
                            var Intent = new Intent(Context, typeof(GeneralAccount_Activity));
                            StartActivity(Intent);
                        }
                        else if (item.ID == 14) // Privacy
                        {
                            var Intent = new Intent(Context, typeof(Privacy_Activity));
                            StartActivity(Intent);
                        }
                        else if (item.ID == 15) // Notification
                        {
                            var Intent = new Intent(Context, typeof(MessegeNotification_Activity));
                            StartActivity(Intent);
                        }
                        else if (item.ID == 16) // Tell a Friends
                        {
                            var Intent = new Intent(Context, typeof(TellFriend_Activity));
                            StartActivity(Intent);
                        }
                        else if (item.ID == 17) // clear Cache
                        {
                        }
                        else if (item.ID == 18) // Help & Support
                        {
                            var Intent = new Intent(Context, typeof(Support_Activity));
                            StartActivity(Intent);
                        }
                        else if (item.ID == 19) // Logout
                        {
                            var dialog = new MaterialDialog.Builder(Context);

                            dialog.Title(Resource.String.Lbl_Warning);
                            dialog.Content(Context.GetText(Resource.String.Lbl_Are_you_logout));
                            dialog.PositiveText(Context.GetText(Resource.String.Lbl_Ok)).OnPositive(this);
                            dialog.NegativeText(Context.GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                            dialog.AlwaysCallSingleChoiceCallback();
                            dialog.ItemsCallback(this).Build().Show();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }


        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 101)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        API_Request.Logout(Activity);
                    else
                        Toast.MakeText(Activity, Activity.GetText(Resource.String.Lbl_Permission_is_denailed),
                            ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
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

        public override void OnDestroy()
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

        #region  Variables Basic

        public MoreSectionAdapter MoreSectionAdapter;
        public RecyclerView MoreRecylerView;

        #endregion
    }
}