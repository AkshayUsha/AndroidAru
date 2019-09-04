using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Gms.Common;
using Android.Gms.Location.Places.UI;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AT.Markushi.UI;
using Com.Theartofdev.Edmodo.Cropper;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Adapters;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Product;
using WoWonder_API.Requests;
using File = Java.IO.File;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.Market
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class CreateProduct_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.CreateProduct_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.CreateProduct_Layout);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_CreateNewProduct);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                CategoriesRecyclerView = FindViewById<RecyclerView>(Resource.Id.CatRecyler);

                Txt_Add = FindViewById<TextView>(Resource.Id.toolbar_title);

                Image_Item = FindViewById<ImageViewAsync>(Resource.Id.Image);
                Image_delete = FindViewById<CircleButton>(Resource.Id.ImageCircle);
                Btn_AddPhoto = FindViewById<Button>(Resource.Id.btn_AddPhoto);
                Txt_name = FindViewById<EditText>(Resource.Id.nameet);
                Txt_price = FindViewById<EditText>(Resource.Id.priceet);
                Txt_Location = FindViewById<EditText>(Resource.Id.Locationet);
                Txt_about = FindViewById<EditText>(Resource.Id.aboutet);

                RB_New = FindViewById<RadioButton>(Resource.Id.rad_New);
                RB_Used = FindViewById<RadioButton>(Resource.Id.rad_Used);


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

                var imageTrancform = ImageService.Instance.LoadCompiledResource("Grey_Offline.jpg");
                imageTrancform.LoadingPlaceholder("Grey_Offline.jpg", ImageSource.CompiledResource);
                imageTrancform.ErrorPlaceholder("Grey_Offline.jpg", ImageSource.CompiledResource);
                imageTrancform.TransformPlaceholders(true);
                imageTrancform.Transform(new RoundedTransformation(30));
                imageTrancform.FadeAnimation(false);
                imageTrancform.Into(Image_Item);

                //Show Ads
                mAdView = FindViewById<AdView>(Resource.Id.adView);
                if (Settings.Show_ADMOB_Banner)
                {
                    mAdView.Visibility = ViewStates.Visible;
                    var adRequest = new AdRequest.Builder().Build();
                    mAdView.LoadAd(adRequest);
                }
                else
                {
                    mAdView.Pause();
                    mAdView.Visibility = ViewStates.Invisible;
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

                if (mAdView != null) mAdView.Resume();

                //Add Event
                Image_delete.Click += ImageDelete_OnClick;
                Btn_AddPhoto.Click += BtnAddPhoto_OnClick;
                RB_New.CheckedChange += RbNew_OnCheckedChange;
                RB_Used.CheckedChange += RbUsed_OnCheckedChange;
                Txt_Add.Click += AddButton_OnClick;
                Txt_Location.FocusChange += TxtLocation_OnFocusChange;
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
                if (mAdView != null) mAdView.Pause();

                base.OnPause();

                //Close Event
                Image_delete.Click -= ImageDelete_OnClick;
                Btn_AddPhoto.Click -= BtnAddPhoto_OnClick;
                RB_New.CheckedChange -= RbNew_OnCheckedChange;
                RB_Used.CheckedChange -= RbUsed_OnCheckedChange;
                Txt_Add.Click -= AddButton_OnClick;
                Txt_Location.FocusChange -= TxtLocation_OnFocusChange;
                CategoriesAdapter.ItemClick -= CategoriesAdapter_OnItemClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void TxtLocation_OnFocusChange(object sender, View.FocusChangeEventArgs focusChangeEventArgs)
        {
            try
            {
                if (focusChangeEventArgs.HasFocus)
                {
                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                    {
                        try
                        {
                            var builder = new PlacePicker.IntentBuilder();
                            StartActivityForResult(builder.Build(this), 4);
                        }
                        catch (GooglePlayServicesRepairableException exception)
                        {
                            Crashes.TrackError(exception);
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Not_Available), ToastLength.Short)
                                .Show();
                        }
                        catch (GooglePlayServicesNotAvailableException exception)
                        {
                            Crashes.TrackError(exception);
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Not_Available), ToastLength.Short)
                                .Show();
                        }
                        catch (Exception exception)
                        {
                            Crashes.TrackError(exception);
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Exception), ToastLength.Short)
                                .Show();
                        }
                    }
                    else
                    {
                        if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted &&
                            CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                            try
                            {
                                var builder = new PlacePicker.IntentBuilder();
                                StartActivityForResult(builder.Build(this),4);
                            }
                            catch (GooglePlayServicesRepairableException exception)
                            {
                                Crashes.TrackError(exception);
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Not_Available),
                                    ToastLength.Short).Show();
                            }
                            catch (GooglePlayServicesNotAvailableException exception)
                            {
                                Crashes.TrackError(exception);
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Not_Available),
                                    ToastLength.Short).Show();
                            }
                            catch (Exception exception)
                            {
                                Crashes.TrackError(exception);
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Exception), ToastLength.Short)
                                    .Show();
                            }
                        else
                            RequestPermissions(new[]
                            {
                                Manifest.Permission.AccessFineLocation,
                                Manifest.Permission.AccessCoarseLocation
                            }, 4);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private async void AddButton_OnClick(object sender, EventArgs eventArgs)
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
                        return;
                    }

                    if (string.IsNullOrEmpty(Txt_price.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_price), ToastLength.Short).Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(Txt_Location.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_Location), ToastLength.Short)
                            .Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(Txt_about.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_about), ToastLength.Short).Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(ProductPathImage))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_Image), ToastLength.Short)
                            .Show();
                    }
                    else
                    {
                        //Show a progress
                        AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading) + "...");

                        var (Api_status, Respond) = await Client.Market.Create_Product(Txt_name.Text, Txt_about.Text,Txt_Location.Text, Txt_price.Text, CategoryId, ProductType, ProductPathImage);
                        if (Api_status == 200)
                        {
                            if (Respond is Create_Product_Object result)
                            {
                                // Add new item to my Product list                                
                                var image = new Get_Products_Object.Image
                                {
                                    id = "", product_id = "", image = ProductPathImage, image_org = ""
                                };
                                var list = new List<Get_Products_Object.Image> {image};

                                TabbedMarket_Activity.MyProducts_Fragment_Tab?.MMyProductsAdapter?.MyProductsList
                                    ?.Insert(0, new Get_Products_Object.Product
                                    {
                                        name = Txt_name.Text,
                                        user_id = UserDetails.User_id,
                                        id = result.product_id.ToString(),
                                        location = Txt_Location.Text,
                                        description = Txt_about.Text,
                                        category = CategoryId,
                                        images = new List<Get_Products_Object.Image>(list),
                                        price = Txt_price.Text,
                                        type = ProductType
                                    });

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

        // Event add photo Product
        private void BtnAddPhoto_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!Directory.Exists(IMethods.IPath.FolderDcimMarket))
                    Directory.CreateDirectory(IMethods.IPath.FolderDcimMarket);

                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                    //Open Image 
                    var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimMarket,
                        IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Builder()
                        .SetInitialCropWindowPaddingRatio(0)
                        .SetAutoZoomEnabled(true)
                        .SetMaxZoom(4)
                        .SetGuidelines(CropImageView.Guidelines.On)
                        .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                        .SetOutputUri(myUri).Start(this);
                }
                else
                {
                    if (CropImage.IsExplicitCameraPermissionRequired(this))
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.Camera,
                            Manifest.Permission.ReadExternalStorage
                        }, CropImage.PickImagePermissionsRequestCode);
                    }
                    else
                    {
                        //Open Image 
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimMarket,
                            IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                            .SetOutputUri(myUri).Start(this);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Permission
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == CropImage.PickImagePermissionsRequestCode)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        //Open Image 
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimMarket,
                            IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                            .SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                    }
                }
                else if (requestCode == CropImage.CameraCapturePermissionsRequestCode)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        CropImage.StartPickImageActivity(this);
                    else
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                }
                else if (requestCode == 101)
                {
                    try
                    {
                        var builder = new PlacePicker.IntentBuilder();
                        StartActivityForResult(builder.Build(this), 4);
                    }
                    catch (GooglePlayServicesRepairableException exception)
                    {
                        Crashes.TrackError(exception);
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Not_Available), ToastLength.Short)
                            .Show();
                    }
                    catch (GooglePlayServicesNotAvailableException exception)
                    {
                        Crashes.TrackError(exception);
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Not_Available), ToastLength.Short)
                            .Show();
                    }
                    catch (Exception exception)
                    {
                        Crashes.TrackError(exception);
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Google_Exception), ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                //If its from Camera or Gallary
                if (requestCode == CropImage.CropImageActivityRequestCode)
                {
                    var result = CropImage.GetActivityResult(data);

                    if (resultCode == Result.Ok)
                    {
                        var imageUri = CropImage.GetPickImageResultUri(this, data);

                        if (result.IsSuccessful)
                        {
                            var resultUri = result.Uri;

                            if (!string.IsNullOrEmpty(resultUri.Path))
                            {
                                ProductPathImage = resultUri.Path;

                                var file = Uri.FromFile(new File(ProductPathImage));
                                var imageTrancform = ImageService.Instance.LoadFile(file.Path);
                                imageTrancform.LoadingPlaceholder("ImagePlacholder.jpg", ImageSource.CompiledResource);
                                imageTrancform.ErrorPlaceholder("ImagePlacholder.jpg", ImageSource.CompiledResource);
                                imageTrancform.TransformPlaceholders(true);
                                imageTrancform.Transform(new RoundedTransformation(30));
                                imageTrancform.FadeAnimation(false);
                                imageTrancform.Into(Image_Item);
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong),
                                    ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long)
                                .Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long)
                            .Show();
                    }
                }
                else if (requestCode == CropImage.CropImageActivityResultErrorCode)
                {
                    var result = CropImage.GetActivityResult(data);
                    Exception error = result.Error;
                }
                else if (requestCode == 4 && resultCode == Result.Ok)
                {
                    GetPlaceFromPicker(data);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Toast.MakeText(this, GetText(Resource.String.Lbl_Error_path_image), ToastLength.Short).Show();
            }
        }

        //Event delete image 
        private void ImageDelete_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var imageTrancform = ImageService.Instance.LoadCompiledResource("Grey_Offline.jpg");
                imageTrancform.LoadingPlaceholder("ImagePlacholder.jpg", ImageSource.CompiledResource);
                imageTrancform.ErrorPlaceholder("ImagePlacholder.jpg", ImageSource.CompiledResource);
                imageTrancform.TransformPlaceholders(true);
                imageTrancform.Transform(new RoundedTransformation(30));
                imageTrancform.FadeAnimation(false);
                imageTrancform.Into(Image_Item);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        private void GetPlaceFromPicker(Intent data)
        {
            try
            {
                var placePicked = PlacePicker.GetPlace(this, data);

                if (!string.IsNullOrEmpty(PlaceText))
                    PlaceText = string.Empty;

                PlaceText = placePicked?.AddressFormatted?.ToString();
                Txt_Location.Text = PlaceText;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void RbUsed_OnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            try
            {
                var isChecked = RB_Used.Checked;
                if (isChecked)
                {
                    RB_New.Checked = false;
                    ProductType = "1";
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void RbNew_OnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            try
            {
                var isChecked = RB_New.Checked;
                if (isChecked)
                {
                    RB_Used.Checked = false;
                    ProductType = "0";
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
        public Categories_Adapter CategoriesAdapter;
        private StaggeredGridLayoutManager mLayoutManager;

        private TextView Txt_Add;

        private ImageViewAsync Image_Item;
        private CircleButton Image_delete;

        private Button Btn_AddPhoto;

        private EditText Txt_name;
        private EditText Txt_price;
        private EditText Txt_Location;
        private EditText Txt_about;

        private RadioButton RB_New;
        private RadioButton RB_Used;

        public string CategoryId = "";
        public string ProductType = "";
        public string ProductPathImage = "";
        public string PlaceText;

        private AdView mAdView;

        #endregion
    }
}