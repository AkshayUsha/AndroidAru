using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.MyProfile.Adapters;
using WoWonder.Helpers;
using WoWonder_API.Requests;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.MyProfile
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class EditSocialLinks_Activity : AppCompatActivity, MaterialDialog.ISingleButtonCallback,
        MaterialDialog.IInputCallback
    {
        public void OnInput(MaterialDialog p0, ICharSequence p1)
        {
            try
            {
                if (p1.Length() > 0)
                {
                    var strName = p1.ToString();

                    if (IMethods.CheckConnectivity())
                    {
                        if (Socialitem != null)
                        {
                            SocialAdapter.Update(Socialitem, strName);

                            var dataPrivacy = new Dictionary<string, string>();

                            if (Socialitem.ID == 1)
                                dataPrivacy.Add("facebook", strName);

                            if (Socialitem.ID == 2)
                                dataPrivacy.Add("twitter", strName);

                            if (Socialitem.ID == 3)
                                dataPrivacy.Add("google", strName);

                            if (Socialitem.ID == 4)
                                dataPrivacy.Add("vk", strName);

                            if (Socialitem.ID == 5)
                                dataPrivacy.Add("linkedin", strName);

                            if (Socialitem.ID == 6)
                                dataPrivacy.Add("instagram", strName);

                            if (Socialitem.ID == 7)
                                dataPrivacy.Add("youtube", strName);

                            var data = Client.Global.Update_User_Data(new Settings(), dataPrivacy).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)
                            .Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_The_name_can_not_be_blank), ToastLength.Short)
                        .Show();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.EditSocialLinks_layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.EditSocialLinks_layout);

                var toolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolBar != null)
                {
                    toolBar.Title = GetText(Resource.String.Lbl_SocialLinks);

                    SetSupportActionBar(toolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                SocialRecyclerView = FindViewById<RecyclerView>(Resource.Id.SocialRecyler);

                SocialRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
                SocialAdapter = new SocialLinks_Adapter(this);
                SocialRecyclerView.SetAdapter(SocialAdapter);

                Get_Data_User();
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
                SocialAdapter.ItemClick += SocialAdapter_OnItemClick;
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
                SocialAdapter.ItemClick -= SocialAdapter_OnItemClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        private void Get_Data_User()
        {
            try
            {
                var local = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                if (local != null)
                    foreach (var item in SocialAdapter.SocialList)
                        if (item.ID == 1) //Facebook
                        {
                            if (!string.IsNullOrEmpty(local.facebook))
                            {
                                item.SocialLinkName = local.facebook;
                                item.Checkvisibilty = true;
                            }
                            else
                            {
                                item.Checkvisibilty = false;
                            }
                        }
                        else if (item.ID == 2) // Twitter
                        {
                            if (!string.IsNullOrEmpty(local.twitter))
                            {
                                item.SocialLinkName = local.twitter;
                                item.Checkvisibilty = true;
                            }
                            else
                            {
                                item.Checkvisibilty = false;
                            }
                        }
                        else if (item.ID == 3) // Google
                        {
                            if (!string.IsNullOrEmpty(local.google))
                            {
                                item.SocialLinkName = local.google;
                                item.Checkvisibilty = true;
                            }
                            else
                            {
                                item.Checkvisibilty = false;
                            }
                        }
                        else if (item.ID == 4) // Vkontakte
                        {
                            if (!string.IsNullOrEmpty(local.vk))
                            {
                                item.SocialLinkName = local.vk;
                                item.Checkvisibilty = true;
                            }
                            else
                            {
                                item.Checkvisibilty = false;
                            }
                        }
                        else if (item.ID == 5) // Linkedin
                        {
                            if (!string.IsNullOrEmpty(local.linkedin))
                            {
                                item.SocialLinkName = local.linkedin;
                                item.Checkvisibilty = true;
                            }
                            else
                            {
                                item.Checkvisibilty = false;
                            }
                        }
                        else if (item.ID == 6) // Instagram
                        {
                            if (!string.IsNullOrEmpty(local.instagram))
                            {
                                item.SocialLinkName = local.instagram;
                                item.Checkvisibilty = true;
                            }
                            else
                            {
                                item.Checkvisibilty = false;
                            }
                        }
                        else if (item.ID == 7) // YouTube
                        {
                            if (!string.IsNullOrEmpty(local.youtube))
                            {
                                item.SocialLinkName = local.youtube;
                                item.Checkvisibilty = true;
                            }
                            else
                            {
                                item.Checkvisibilty = false;
                            }
                        }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void SocialAdapter_OnItemClick(object sender, SocialLinks_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = SocialAdapter.GetItem(position);
                    if (item != null)
                    {
                        Socialitem = item;

                        var dialog = new MaterialDialog.Builder(this);

                        dialog.Title(item.SocialName);
                        dialog.Input(Resource.String.Lbl_Enter_your_link, 0, false, this);

                        dialog.InputType(InputTypes.TextFlagImeMultiLine);
                        dialog.PositiveText(GetText(Resource.String.Lbl_Save)).OnPositive(this);
                        dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                        dialog.Build().Show();
                        dialog.AlwaysCallSingleChoiceCallback();
                    }
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

        #region Variables Basic

        private RecyclerView SocialRecyclerView;
        private SocialLinks_Adapter SocialAdapter;

        private SocialItem Socialitem;

        #endregion
    }
}