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
using WoWonder_API.Classes.Page;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.Communities.Pages
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class EditInfoPage_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.EditInfoPage_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.EditInfoPage_Layout);

                var pagesType = Intent.GetStringExtra("PagesType") ?? "Data not available";
                if (pagesType != "Data not available" && !string.IsNullOrEmpty(pagesType)) Pages_Type = pagesType;

                var id = Intent.GetStringExtra("PagesId") ?? "Data not available";
                if (id != "Data not available" && !string.IsNullOrEmpty(id)) Pages_Id = id;

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_Update_Data_Page);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }


                CategoriesRecyclerView = FindViewById<RecyclerView>(Resource.Id.CatRecyler);

                Txt_Save = FindViewById<TextView>(Resource.Id.toolbar_title);

                Txt_title = FindViewById<EditText>(Resource.Id.titleet);
                Txt_name = FindViewById<EditText>(Resource.Id.nameet);
                Txt_about = FindViewById<EditText>(Resource.Id.aboutet);

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

                Get_Data_Page();
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
                Txt_Save.Click -= SaveDataButton_OnClick;
                CategoriesAdapter.ItemClick -= CategoriesAdapterOnItemClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //Get Data Page and set Categories
        private void Get_Data_Page()
        {
            try
            {
                if (Pages_Type == "Liked_UserPages")
                {
                    _LikedPages_Data =
                        JsonConvert.DeserializeObject<Get_User_Data_Object.Liked_Pages>(
                            Intent.GetStringExtra("UserPages"));
                    if (_LikedPages_Data != null)
                    {
                        Txt_title.Text = _LikedPages_Data.page_title;
                        Txt_name.Text = _LikedPages_Data.username;
                        Txt_about.Text =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_LikedPages_Data.about));

                        CategoryId = _LikedPages_Data.page_category;
                        //set value list Categories
                        CategoriesAdapter.Set_Categories(CategoryId, "EditInfoPage");
                    }
                }
                else if (Pages_Type == "Liked_MyPages")
                {
                    _MyPages_Data =
                        JsonConvert.DeserializeObject<Get_Community_Object.Page>(Intent.GetStringExtra("MyPages"));
                    if (_MyPages_Data != null)
                    {
                        Txt_title.Text = _MyPages_Data.PageTitle;
                        Txt_name.Text = _MyPages_Data.Username;
                        Txt_about.Text =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_MyPages_Data.About));

                        CategoryId = _MyPages_Data.PageCategory;
                        //set value list Categories
                        CategoriesAdapter.Set_Categories(CategoryId, "EditInfoPage");
                    }
                }
                else if (Pages_Type == "Saerch_Pages")
                {
                    _SearchPages_Data =
                        JsonConvert.DeserializeObject<Get_Search_Object.Page>(Intent.GetStringExtra("SaerchPages"));
                    if (_SearchPages_Data != null)
                    {
                        Txt_title.Text = _SearchPages_Data.PageTitle;
                        Txt_name.Text = _SearchPages_Data.Username;
                        Txt_about.Text =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_SearchPages_Data.About));

                        CategoryId = _SearchPages_Data.PageCategory;
                        //set value list Categories
                        CategoriesAdapter.Set_Categories(CategoryId, "EditInfoPage");
                    }
                }
                else if (Pages_Type == "Page_Data")
                {
                    _Page_Data =
                        JsonConvert.DeserializeObject<Get_Page_Data_Object.Page_Data>(
                            Intent.GetStringExtra("PageData"));
                    if (_Page_Data != null)
                    {
                        Txt_title.Text = _Page_Data.page_title;
                        Txt_name.Text = _Page_Data.username;
                        Txt_about.Text =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_Page_Data.about));

                        CategoryId = _Page_Data.page_category;
                        //set value list Categories
                        CategoriesAdapter.Set_Categories(CategoryId, "EditInfoPage");
                    }
                }
                else if (Pages_Type == "Liked_PromotedPages")
                {
                    _PromotedPages_Data =
                        JsonConvert.DeserializeObject<Get_General_Data_Object.Promoted_Pages>(
                            Intent.GetStringExtra("PromotedPages"));
                    if (_PromotedPages_Data != null)
                    {
                        Txt_title.Text = _PromotedPages_Data.page_title;
                        Txt_name.Text = _PromotedPages_Data.username;
                        Txt_about.Text =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_PromotedPages_Data.about));

                        CategoryId = _PromotedPages_Data.page_category;
                        //set value list Categories
                        CategoriesAdapter.Set_Categories(CategoryId, "EditInfoPage");
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Save Data Page
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

                    //dictionary.Add("page_id", Pages_Id);
                    dictionary.Add("page_name", Txt_name.Text);
                    dictionary.Add("page_title", Txt_title.Text);
                    dictionary.Add("page_description", Txt_about.Text);
                    dictionary.Add("page_category", CategoryId);

                    var (Api_status, Respond) = await Client.Page.Update_Page_Data(Pages_Id, dictionary);
                    if (Api_status == 200)
                    {
                        if (Respond is Update_Group_Data_Object result)
                        {
                            var cat = CategoriesController.ListCatigories_Names.FirstOrDefault(a =>
                                a.Catigories_Id == CategoryId);
                            if (cat != null) CategoryText = cat.Catigories_Name;

                            // Update Page in DB 
                            var dbDatabase = new SqLiteDatabase();
                            if (Pages_Type == "Liked_UserPages")
                            {
                                var item = _LikedPages_Data;
                                var data = new DataTables.PageTB
                                {
                                    PageId = item.page_id,
                                    UserId = item.user_id,
                                    PageName = item.page_name,
                                    PageTitle = item.page_title,
                                    PageDescription = item.page_description,
                                    Avatar = item.avatar,
                                    Cover = item.cover,
                                    PageCategory = item.page_category,
                                    Website = item.website,
                                    Facebook = item.facebook,
                                    Google = item.google,
                                    Vk = item.vk,
                                    Twitter = item.twitter,
                                    Linkedin = item.linkedin,
                                    Company = item.company,
                                    Phone = item.phone,
                                    Address = item.address,
                                    CallActionType = item.call_action_type,
                                    CallActionTypeUrl = item.call_action_type_url,
                                    BackgroundImage = item.background_image,
                                    BackgroundImageStatus = item.background_image_status,
                                    Instgram = item.instgram,
                                    Youtube = item.youtube,
                                    Verified = item.verified,
                                    Registered = item.registered,
                                    Boosted = item.boosted,
                                    About = item.about,
                                    Id = item.id,
                                    Type = item.type,
                                    Url = item.url,
                                    Name = item.name,
                                    // Rating = item.rating,
                                    Category = item.category,
                                    IsPageOnwer = Convert.ToString(item.is_page_onwer),
                                    Username = item.username
                                };
                                dbDatabase.UpdateRow(data);
                            }
                            else if (Pages_Type == "Liked_MyPages")
                            {
                                var item = _MyPages_Data;
                                var data = new DataTables.PageTB
                                {
                                    PageId = item.PageId,
                                    UserId = item.UserId,
                                    PageName = item.PageName,
                                    PageTitle = item.PageTitle,
                                    PageDescription = item.PageDescription,
                                    Avatar = item.Avatar,
                                    Cover = item.Cover,
                                    PageCategory = item.PageCategory,
                                    Website = item.Website,
                                    Facebook = item.Facebook,
                                    Google = item.Google,
                                    Vk = item.Vk,
                                    Twitter = item.Twitter,
                                    Linkedin = item.Linkedin,
                                    Company = item.Company,
                                    Phone = item.Phone,
                                    Address = item.Address,
                                    CallActionType = item.CallActionType,
                                    CallActionTypeUrl = item.CallActionTypeUrl,
                                    BackgroundImage = item.BackgroundImage,
                                    BackgroundImageStatus = item.BackgroundImageStatus,
                                    Instgram = item.Instgram,
                                    Youtube = item.Youtube,
                                    Verified = item.Verified,
                                    Registered = item.Registered,
                                    Boosted = item.Boosted,
                                    About = item.About,
                                    Id = item.Id,
                                    Type = item.Type,
                                    Url = item.Url,
                                    Name = item.Name,
                                    //Rating = item.Rating,
                                    Category = item.Category,
                                    IsPageOnwer = Convert.ToString(item.IsPageOnwer),
                                    Username = item.Username
                                };
                                dbDatabase.UpdateRow(data);
                            }
                            else if (Pages_Type == "Saerch_Pages")
                            {
                                var item = _SearchPages_Data;
                                var data = new DataTables.PageTB
                                {
                                    PageId = item.PageId,
                                    UserId = item.UserId,
                                    PageName = item.PageName,
                                    PageTitle = item.PageTitle,
                                    PageDescription = item.PageDescription,
                                    Avatar = item.Avatar,
                                    Cover = item.Cover,
                                    PageCategory = item.PageCategory,
                                    Website = item.Website,
                                    Facebook = item.Facebook,
                                    Google = item.Google,
                                    Vk = item.Vk,
                                    Twitter = item.Twitter,
                                    Linkedin = item.Linkedin,
                                    Company = item.Company,
                                    Phone = item.Phone,
                                    Address = item.Address,
                                    CallActionType = item.CallActionType,
                                    CallActionTypeUrl = item.CallActionTypeUrl,
                                    BackgroundImage = item.BackgroundImage,
                                    BackgroundImageStatus = item.BackgroundImageStatus,
                                    Instgram = item.Instgram,
                                    Youtube = item.Youtube,
                                    Verified = item.Verified,
                                    Registered = item.Registered,
                                    Boosted = item.Boosted,
                                    About = item.About,
                                    Id = item.Id,
                                    Type = item.Type,
                                    Url = item.Url,
                                    Name = item.Name,
                                    //Rating = item.Rating,
                                    Category = item.Category,
                                    IsPageOnwer = Convert.ToString(item.IsPageOnwer),
                                    Username = item.Username
                                };
                                dbDatabase.UpdateRow(data);
                            }
                            else if (Pages_Type == "Page_Data")
                            {
                                var item = _Page_Data;
                                var data = new DataTables.PageTB
                                {
                                    PageId = item.page_id,
                                    UserId = item.user_id,
                                    PageName = item.page_name,
                                    PageTitle = item.page_title,
                                    PageDescription = item.page_description,
                                    Avatar = item.avatar,
                                    Cover = item.cover,
                                    PageCategory = item.page_category,
                                    Website = item.website,
                                    Facebook = item.facebook,
                                    Google = item.google,
                                    Vk = item.vk,
                                    Twitter = item.twitter,
                                    Linkedin = item.linkedin,
                                    Company = item.company,
                                    Phone = item.phone,
                                    Address = item.address,
                                    CallActionType = item.call_action_type,
                                    CallActionTypeUrl = item.call_action_type_url,
                                    BackgroundImage = item.background_image,
                                    //BackgroundImageStatus = item.BackgroundImageStatus,
                                    Instgram = item.instgram,
                                    Youtube = item.youtube,
                                    Verified = item.verified,
                                    Registered = item.registered,
                                    Boosted = item.boosted,
                                    About = item.about,
                                    //Id = item.id,
                                    //Type = item.t,
                                    Url = item.url,
                                    Name = item.name,
                                    //Rating = item.rating,
                                    Category = item.category,
                                    IsPageOnwer = Convert.ToString(item.is_page_onwer),
                                    Username = item.username
                                };
                                dbDatabase.UpdateRow(data);
                            }

                            dbDatabase.Dispose();

                            AndHUD.Shared.ShowSuccess(this, GetText(Resource.String.Lbl_YourPageWasUpdated),
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

        public string PagesPrivacy = "";
        public string CategoryId = "";
        public string CategoryText = "";

        public string Pages_Type = "";
        public string Pages_Id = "";

        private Get_User_Data_Object.Liked_Pages _LikedPages_Data;
        private Get_Community_Object.Page _MyPages_Data;
        private Get_Search_Object.Page _SearchPages_Data;
        private Get_Page_Data_Object.Page_Data _Page_Data;
        private Get_General_Data_Object.Promoted_Pages _PromotedPages_Data;

        #endregion
    }
}