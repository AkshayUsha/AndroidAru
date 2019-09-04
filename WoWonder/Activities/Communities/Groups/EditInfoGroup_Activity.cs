using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using Newtonsoft.Json;
using SettingsConnecter;
using WoWonder.Adapters;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Group;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.Communities.Groups
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class EditInfoGroup_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.EditInfoGroup_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.EditInfoGroup_Layout);

                var groupsType = Intent.GetStringExtra("GroupsType") ?? "Data not available";
                if (groupsType != "Data not available" && !string.IsNullOrEmpty(groupsType)) Groups_Type = groupsType;

                var id = Intent.GetStringExtra("GroupsId") ?? "Data not available";
                if (id != "Data not available" && !string.IsNullOrEmpty(id)) Groups_Id = id;

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                ToolBar.Title = GetText(Resource.String.Lbl_Update_DataGroup);

                SetSupportActionBar(ToolBar);
                SupportActionBar.SetDisplayShowCustomEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);


                CategoriesRecyclerView = FindViewById<RecyclerView>(Resource.Id.CatRecyler);

                Txt_Save = FindViewById<TextView>(Resource.Id.toolbar_title);

                Txt_title = FindViewById<EditText>(Resource.Id.titleet);
                Txt_name = FindViewById<EditText>(Resource.Id.nameet);
                Txt_about = FindViewById<EditText>(Resource.Id.aboutet);

                RB_Public = FindViewById<RadioButton>(Resource.Id.rad_Public);
                RB_Private = FindViewById<RadioButton>(Resource.Id.rad_Private);

                //GEt Data Categories Local
                var cat = new CategoriesController();
                cat.Get_Categories_Communities();
                if (CategoriesController.ListCatigories_Names.Count > 0)
                {
                    CategoriesRecyclerView.SetLayoutManager(
                        new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Horizontal));
                    CategoriesAdapter = new Categories_Adapter(this);
                    CategoriesAdapter.mCategoriesList =
                        new ObservableCollection<Classes.Catigories>(CategoriesController.ListCatigories_Names);
                    CategoriesRecyclerView.SetAdapter(CategoriesAdapter);
                    CategoriesRecyclerView.NestedScrollingEnabled = false;
                    CategoriesAdapter.BindEnd();

                    CategoriesRecyclerView.Visibility = ViewStates.Visible;
                }

                Get_Data_Group();
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
                RB_Public.CheckedChange += RbPublic_OnCheckedChange;
                RB_Private.CheckedChange += RbPrivate_OnCheckedChange;
                Txt_Save.Click += SaveDataButton_OnClick;
                CategoriesAdapter.ItemClick += CategoriesAdapterOnItemClick;
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
                RB_Public.CheckedChange -= RbPublic_OnCheckedChange;
                RB_Private.CheckedChange -= RbPrivate_OnCheckedChange;
                Txt_Save.Click -= SaveDataButton_OnClick;
                CategoriesAdapter.ItemClick -= CategoriesAdapterOnItemClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //Get Data Group and set Categories
        private void Get_Data_Group()
        {
            try
            {
                if (Groups_Type == "Joined_UserGroups")
                {
                    _JoinedGroups_Data =
                        JsonConvert.DeserializeObject<Get_User_Data_Object.Joined_Groups>(
                            Intent.GetStringExtra("UserGroups"));
                    if (_JoinedGroups_Data != null)
                    {
                        Txt_title.Text = _JoinedGroups_Data.group_title;
                        Txt_name.Text = _JoinedGroups_Data.username;
                        Txt_about.Text =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_JoinedGroups_Data.about));

                        if (_JoinedGroups_Data.privacy == "0")
                        {
                            RB_Private.Checked = true;
                            RB_Public.Checked = false;
                        }
                        else
                        {
                            RB_Private.Checked = false;
                            RB_Public.Checked = true;
                        }

                        GroupPrivacy = _JoinedGroups_Data.privacy;
                        CategoryId = _JoinedGroups_Data.category_id;
                        //set value list Categories
                        CategoriesAdapter.Set_Categories(CategoryId, "EditInfoGroup");
                    }
                }
                else if (Groups_Type == "Joined_MyGroups")
                {
                    _MyGroups_Data =
                        JsonConvert.DeserializeObject<Get_Community_Object.Group>(Intent.GetStringExtra("MyGroups"));
                    if (_MyGroups_Data != null)
                    {
                        Txt_title.Text = _MyGroups_Data.GroupName;
                        Txt_name.Text = _MyGroups_Data.Username;
                        Txt_about.Text =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_MyGroups_Data.About));

                        if (_MyGroups_Data.Privacy == "0")
                        {
                            RB_Private.Checked = true;
                            RB_Public.Checked = false;
                        }
                        else
                        {
                            RB_Private.Checked = false;
                            RB_Public.Checked = true;
                        }

                        GroupPrivacy = _MyGroups_Data.Privacy;
                        CategoryId = _MyGroups_Data.CategoryId;
                        //set value list Categories
                        CategoriesAdapter.Set_Categories(CategoryId, "EditInfoGroup");
                    }
                }
                else if (Groups_Type == "Search_Groups")
                {
                    _SearchGroups_Data =
                        JsonConvert.DeserializeObject<Get_Search_Object.Group>(Intent.GetStringExtra("SearchGroups"));
                    if (_SearchGroups_Data != null)
                    {
                        Txt_title.Text = _SearchGroups_Data.GroupName;
                        Txt_name.Text = _SearchGroups_Data.Username;
                        Txt_about.Text =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_SearchGroups_Data.About));

                        if (_SearchGroups_Data.Privacy == "0")
                        {
                            RB_Private.Checked = true;
                            RB_Public.Checked = false;
                        }
                        else
                        {
                            RB_Private.Checked = false;
                            RB_Public.Checked = true;
                        }

                        GroupPrivacy = _SearchGroups_Data.Privacy;
                        CategoryId = _SearchGroups_Data.CategoryId;
                        //set value list Categories
                        CategoriesAdapter.Set_Categories(CategoryId, "EditInfoGroup");
                    }
                }
                else if (Groups_Type == "Group_Data")
                {
                    _Group_Data =
                        JsonConvert.DeserializeObject<Get_Group_Data_Object.Group_Data>(
                            Intent.GetStringExtra("GroupData"));
                    if (_Group_Data != null)
                    {
                        Txt_title.Text = _Group_Data.group_title;
                        Txt_name.Text = _Group_Data.username;
                        Txt_about.Text =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_Group_Data.about));

                        if (_Group_Data.privacy == "0")
                        {
                            RB_Private.Checked = true;
                            RB_Public.Checked = false;
                        }
                        else
                        {
                            RB_Private.Checked = false;
                            RB_Public.Checked = true;
                        }

                        GroupPrivacy = _Group_Data.privacy;
                        CategoryId = _Group_Data.category_id;
                        //set value list Categories
                        CategoriesAdapter.Set_Categories(CategoryId, "EditInfoGroup");
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Save Data Group
        private async void SaveDataButton_OnClick(object sender, EventArgs eventArgs)
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
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading) + "...");

                    var dictionary = new Dictionary<string, string>();

                    //dictionary.Add("group_id", Groups_Id);
                    dictionary.Add("group_name", Txt_name.Text);
                    dictionary.Add("group_title", Txt_title.Text);
                    dictionary.Add("about", Txt_about.Text);
                    dictionary.Add("category", CategoryId);
                    dictionary.Add("privacy", GroupPrivacy);

                    var (Api_status, Respond) = await Client.Group.Update_Group_Data(Groups_Id, dictionary);
                    if (Api_status == 200)
                    {
                        if (Respond is Update_Group_Data_Object result)
                        {
                            var cat = CategoriesController.ListCatigories_Names.FirstOrDefault(a =>
                                a.Catigories_Id == CategoryId);
                            if (cat != null) CategoryText = cat.Catigories_Name;

                            // Update Group in DB 
                            var dbDatabase = new SqLiteDatabase();
                            if (Groups_Type == "Joined_UserGroups")
                            {
                                var item = _JoinedGroups_Data;
                                var data = new DataTables.GroupsTB
                                {
                                    Id = item.id,
                                    UserId = item.user_id,
                                    GroupName = Txt_name.Text,
                                    GroupTitle = Txt_title.Text,
                                    Avatar = item.avatar,
                                    Cover = item.cover,
                                    About = Txt_about.Text,
                                    Category = CategoryText,
                                    Privacy = item.privacy,
                                    JoinPrivacy = item.join_privacy,
                                    Active = item.active,
                                    Registered = item.registered,
                                    GroupId = item.group_id,
                                    Url = item.url,
                                    Name = Txt_name.Text,
                                    CategoryId = CategoryId,
                                    Type = item.type,
                                    Username = Txt_name.Text
                                };
                                dbDatabase.UpdateRow(data);
                            }
                            else if (Groups_Type == "Joined_MyGroups")
                            {
                                var item = _MyGroups_Data;
                                var data = new DataTables.GroupsTB
                                {
                                    Id = item.Id,
                                    UserId = item.UserId,
                                    GroupName = Txt_name.Text,
                                    GroupTitle = Txt_title.Text,
                                    Avatar = item.Avatar,
                                    Cover = item.Cover,
                                    About = Txt_about.Text,
                                    Category = CategoryText,
                                    Privacy = item.Privacy,
                                    JoinPrivacy = item.JoinPrivacy,
                                    Active = item.Active,
                                    Registered = item.Registered,
                                    GroupId = item.GroupId,
                                    Url = item.Url,
                                    Name = Txt_name.Text,
                                    CategoryId = CategoryId,
                                    Type = item.Type,
                                    Username = Txt_name.Text
                                };
                                dbDatabase.UpdateRow(data);
                            }
                            else if (Groups_Type == "Search_Groups")
                            {
                                var item = _SearchGroups_Data;
                                var data = new DataTables.GroupsTB
                                {
                                    Id = item.Id,
                                    UserId = item.UserId,
                                    GroupName = Txt_name.Text,
                                    GroupTitle = Txt_title.Text,
                                    Avatar = item.Avatar,
                                    Cover = item.Cover,
                                    About = Txt_about.Text,
                                    Category = CategoryText,
                                    Privacy = item.Privacy,
                                    JoinPrivacy = item.JoinPrivacy,
                                    Active = item.Active,
                                    Registered = item.Registered,
                                    GroupId = item.GroupId,
                                    Url = item.Url,
                                    Name = Txt_name.Text,
                                    CategoryId = CategoryId,
                                    Type = item.Type,
                                    Username = Txt_name.Text
                                };
                                dbDatabase.UpdateRow(data);
                            }
                            else if (Groups_Type == "Group_Data")
                            {
                                var item = _Group_Data;
                                var data = new DataTables.GroupsTB
                                {
                                    //Id = item.id,
                                    UserId = item.user_id,
                                    GroupName = Txt_name.Text,
                                    GroupTitle = Txt_title.Text,
                                    Avatar = item.avatar,
                                    Cover = item.cover,
                                    About = Txt_about.Text,
                                    Category = CategoryText,
                                    Privacy = item.privacy,
                                    JoinPrivacy = item.join_privacy,
                                    Active = item.active,
                                    Registered = item.registered,
                                    GroupId = item.group_id,
                                    Url = item.url,
                                    Name = Txt_name.Text,
                                    CategoryId = CategoryId,
                                    //Type = item.type,
                                    Username = Txt_name.Text
                                };
                                dbDatabase.UpdateRow(data);
                            }

                            dbDatabase.Dispose();

                            AndHUD.Shared.ShowSuccess(this, GetText(Resource.String.Lbl_YourGroupWasUpdated),
                                MaskType.Clear, TimeSpan.FromSeconds(2));
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
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void RbPrivate_OnCheckedChange(object sender,
            CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            try
            {
                var isChecked = RB_Private.Checked;
                if (isChecked)
                {
                    RB_Public.Checked = false;
                    GroupPrivacy = "0";
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void RbPublic_OnCheckedChange(object sender,
            CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            try
            {
                var isChecked = RB_Public.Checked;
                if (isChecked)
                {
                    RB_Private.Checked = false;
                    GroupPrivacy = "1";
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void CategoriesAdapterOnItemClick(object sender, Categories_AdapterClickEventArgs adapterClickEvents)
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

        public static RecyclerView CategoriesRecyclerView;
        private Categories_Adapter CategoriesAdapter;

        private TextView Txt_Save;

        private EditText Txt_title;
        private EditText Txt_name;
        private EditText Txt_about;

        private RadioButton RB_Public;
        private RadioButton RB_Private;

        public string GroupPrivacy = "";
        public string CategoryId = "";
        public string CategoryText = "";

        public string Groups_Type = "";
        public string Groups_Id = "";

        private Get_User_Data_Object.Joined_Groups _JoinedGroups_Data;
        private Get_Community_Object.Group _MyGroups_Data;
        private Get_Search_Object.Group _SearchGroups_Data;
        private Get_Group_Data_Object.Group_Data _Group_Data;

        #endregion
    }
}