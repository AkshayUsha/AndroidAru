using System;
using System.Collections.ObjectModel;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Adapters;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Group;
using WoWonder_API.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.Communities.Groups
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class CreateGroup_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.CreateGroup_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.CreateGroup_Layout);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_Create_New_Group);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                CategoriesRecyclerView = FindViewById<RecyclerView>(Resource.Id.CatRecyler);

                Txt_Create = FindViewById<TextView>(Resource.Id.toolbar_title);

                Txt_title = FindViewById<EditText>(Resource.Id.titleet);
                Txt_name = FindViewById<EditText>(Resource.Id.nameet);
                Txt_about = FindViewById<EditText>(Resource.Id.aboutet);

                //Get Data Categories Local
                var cat = new CategoriesController();
                cat.Get_Categories_Communities();
                if (CategoriesController.ListCatigories_Names.Count > 0)
                {
                    mLayoutManager = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Horizontal);

                    CategoriesRecyclerView.HasFixedSize = true;

                    CategoriesRecyclerView.SetLayoutManager(mLayoutManager);

                    CategoriesAdapter = new Categories_Adapter(this);
                    CategoriesAdapter.mCategoriesList =
                        new ObservableCollection<Classes.Catigories>(CategoriesController.ListCatigories_Names);
                    CategoriesRecyclerView.SetAdapter(CategoriesAdapter);
                    CategoriesRecyclerView.NestedScrollingEnabled = false;
                    CategoriesAdapter.BindEnd();

                    CategoriesRecyclerView.Visibility = ViewStates.Visible;
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
                base.OnResume();

                //Add Event
                Txt_Create.Click += CreatButton_OnClick;
                CategoriesAdapter.ItemClick += CategoriesAdapter_OnItemClick;
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
                Txt_Create.Click -= CreatButton_OnClick;
                CategoriesAdapter.ItemClick -= CategoriesAdapter_OnItemClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //function Creat New Group
        private async void CreatButton_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)
                        .Show();
                }
                else
                {
                    if (string.IsNullOrEmpty(Txt_name.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_name), ToastLength.Short).Show();
                    }
                    else if (string.IsNullOrEmpty(Txt_title.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_title), ToastLength.Short).Show();
                    }
                    else if (string.IsNullOrEmpty(Txt_about.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_about), ToastLength.Short).Show();
                    }
                    else if (string.IsNullOrEmpty(CategoryId))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_category), ToastLength.Short)
                            .Show();
                    }
                    else
                    {
                        //Show a progress
                        AndHUD.Shared.Show(this, "Loading...");

                        var (Api_status, Respond) = await Client.Group.Create_Group(Txt_name.Text, Txt_title.Text,
                            Txt_about.Text, CategoryId);
                        if (Api_status == 200)
                        {
                            if (Respond is Create_Group_Object result)
                                if (result.group_data != null)
                                {
                                    var item = result.group_data;
                                    //Insert group data to database
                                    var group = new DataTables.GroupsTB
                                    {
                                        Id = item.id,
                                        UserId = item.user_id,
                                        GroupName = item.group_name,
                                        GroupTitle = item.group_title,
                                        Avatar = item.avatar,
                                        Cover = item.cover,
                                        About = item.about,
                                        Category = item.category,
                                        Privacy = item.privacy,
                                        JoinPrivacy = item.join_privacy,
                                        Active = item.active,
                                        Registered = item.registered,
                                        GroupId = item.group_id,
                                        Url = item.url,
                                        Name = item.name,
                                        CategoryId = item.category_id,
                                        Type = item.type,
                                        Username = item.username
                                    };

                                    var dbDatabase = new SqLiteDatabase();
                                    dbDatabase.InsertRow(group);
                                    dbDatabase.Dispose();

                                    AndHUD.Shared.ShowSuccess(this);
                                }
                        }
                        else if (Api_status == 400)
                        {
                            if (Respond is Error_Object error)
                            {
                                var errortext = error._errors.Error_text;
                                //Show a Error 
                                AndHUD.Shared.ShowError(this, errortext, MaskType.Clear, TimeSpan.FromSeconds(2));

                                if (errortext.Contains("Invalid or expired access_token"))
                                    API_Request.Logout(this);
                            }
                        }
                        else if (Api_status == 404)
                        {
                            var error = Respond.ToString();
                            //Show a Error
                            AndHUD.Shared.ShowError(this, error, MaskType.Clear, TimeSpan.FromSeconds(2));
                        }

                        AndHUD.Shared.Dismiss(this);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        private void CategoriesAdapter_OnItemClick(object sender, Categories_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = CategoriesAdapter.GetItem(position);
                    if (item != null)
                    {
                        CategoriesAdapter.Click_Categories(item);
                        CategoryId = item.Catigories_Id;
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

        private RecyclerView CategoriesRecyclerView;
        private Categories_Adapter CategoriesAdapter;
        private StaggeredGridLayoutManager mLayoutManager;

        private TextView Txt_Create;

        private EditText Txt_title;
        private EditText Txt_name;
        private EditText Txt_about;

        public string CategoryId = "";

        #endregion
    }
}