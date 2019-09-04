using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Devs.ReadMoreOptionLib;
using FFImageLoading;
using FFImageLoading.Views;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using SettingsConnecter;
using WoWonder.Activities.MyProfile;
using WoWonder.Activities.UserProfile;
using WoWonder.Helpers;
using WoWonder_API.Classes.Product;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;

namespace WoWonder.Activities.Market
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class ProductView_Activity : AppCompatActivity
    {
      

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);


                var view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.ProductView_Layout);
                if (view != null)
                    SetContentView(view);
                else
                    SetContentView(Resource.Layout.ProductView_Layout);

                imgBackground = (ImageViewAsync) FindViewById(Resource.Id.imgBackground);
                Txt_Product_Title = (TextView) FindViewById(Resource.Id.tv_product_title);
                Txt_Product_Price = (TextView) FindViewById(Resource.Id.tv_price);
                Txt_Product_BoleanNew = (TextView) FindViewById(Resource.Id.BoleanNew);
                Txt_Product_BoleanInStock = (TextView) FindViewById(Resource.Id.BoleanInStock);
                Txt_Product_Description = (TextView) FindViewById(Resource.Id.tv_description);
                Txt_Product_Location = (TextView) FindViewById(Resource.Id.tv_Location);
                Txt_Product_CardName = (TextView) FindViewById(Resource.Id.card_name);
                Txt_Product_Time = (TextView) FindViewById(Resource.Id.card_dist);

                Btn_Contact = (Button) FindViewById(Resource.Id.cont);
                rlContainer = (RelativeLayout) FindViewById(Resource.Id.rl_container);
                llProductDetails = (LinearLayout) FindViewById(Resource.Id.ll_product_details);
                cvProductDetails = (CardView) FindViewById(Resource.Id.cv_product_details);

                UserImageAvatar = (ImageViewAsync) FindViewById(Resource.Id.card_pro_pic);
                ImageMore = (ImageView) FindViewById(Resource.Id.Image_more);
                IconBack = (ImageView) FindViewById(Resource.Id.iv_back);
                 

                Get_Data_Product();
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
                Btn_Contact.Click += BtnContactOnClick;
                ImageMore.Click += ImageMoreOnClick;
                IconBack.Click += IconBackOnClick;
                 
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
                Btn_Contact.Click -= BtnContactOnClick;
                ImageMore.Click -= ImageMoreOnClick;
                IconBack.Click -= IconBackOnClick;
               
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        private void Get_Data_Product()
        {
            try
            {
                _Product_Data =
                    JsonConvert.DeserializeObject<Get_Products_Object.Product>(Intent.GetStringExtra("ProductView"));
                if (_Product_Data != null)
                {
                    ImageServiceLoader.Load_Image(imgBackground, "ImagePlacholder.jpg", _Product_Data.images[0].image,
                        2, false, 5);

                    var AvatarSplit = _Product_Data.seller.avatar.Split('/').Last();
                    var getImage_Avatar =
                        IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                        ImageServiceLoader.Load_Image(UserImageAvatar, "no_profile_image.png", getImage_Avatar, 1,
                            false, 5);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage,
                            _Product_Data.seller.avatar);
                        ImageServiceLoader.Load_Image(UserImageAvatar, "no_profile_image.png",
                            _Product_Data.seller.avatar, 1, false, 5);
                    }

                    Txt_Product_Title.Text =
                        IMethods.Fun_String.DecodeString(IMethods.Fun_String.DecodeStringWithEnter(_Product_Data.name));
                    Txt_Product_Price.Text = _Product_Data.price + " " + Settings.Market_curency;

                    var readMoreOption = new ReadMoreOption.Builder(this)
                        .TextLength(200)
                        .MoreLabel(GetText(Resource.String.Lbl_ReadMore))
                        .LessLabel(GetText(Resource.String.Lbl_ReadLess))
                        .MoreLabelColor(Color.ParseColor(Settings.MainColor))
                        .LessLabelColor(Color.ParseColor(Settings.MainColor))
                        .LabelUnderLine(true)
                        .Build();

                    if (IMethods.Fun_String.StringNullRemover(_Product_Data.description) != "Empty")
                    {
                        var description =
                            IMethods.Fun_String.DecodeString(
                                IMethods.Fun_String.DecodeStringWithEnter(_Product_Data.description));
                        readMoreOption.AddReadMoreTo(Txt_Product_Description, description);
                    }
                    else
                    {
                        readMoreOption.AddReadMoreTo(Txt_Product_Description, GetText(Resource.String.Lbl_Empty));
                    }

                    Txt_Product_Location.Text = _Product_Data.location;
                    Txt_Product_CardName.Text = IMethods.Fun_String.SubStringCutOf(_Product_Data.seller.name, 14);
                    Txt_Product_Time.Text = _Product_Data.time_text;

                    if (_Product_Data.type == "0") // New
                        Txt_Product_BoleanNew.Visibility = ViewStates.Visible;
                    else // Used
                        Txt_Product_BoleanNew.Visibility = ViewStates.Gone;

                    if (_Product_Data.status == "0") // Status InStock
                        Txt_Product_BoleanInStock.Visibility = ViewStates.Visible;
                    else
                        Txt_Product_BoleanInStock.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        // Event Open User Profile
        private void BtnContactOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                Intent Int;
                if (_Product_Data.user_id != UserDetails.User_id)
                {
                    Int = new Intent(this, typeof(User_Profile_Activity));
                    Int.PutExtra("UserType", "ProductView");
                    Int.PutExtra("UserItem", JsonConvert.SerializeObject(_Product_Data));
                    Int.PutExtra("UserId", _Product_Data.user_id);
                }
                else
                {
                    Int = new Intent(this, typeof(MyProfile_Activity));
                }

                StartActivity(Int);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        //Event Back
        private void IconBackOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                Finish();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event More >> Show Menu (CopeLink , Share)
        private void ImageMoreOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var ctw = new ContextThemeWrapper(this, Resource.Style.PopupMenuStyle);
                var popup = new PopupMenu(ctw, ImageMore);
                popup.MenuInflater.Inflate(Resource.Menu.MoreCommunities_NotEdit_Menu, popup.Menu);
                popup.Show();
                popup.MenuItemClick += (o, e) =>
                {
                    try
                    {
                        var Id = e.Item.ItemId;
                        switch (Id)
                        {
                            case Resource.Id.menu_CopeLink:
                                OnCopyLink_Button_Click();
                                break;

                            case Resource.Id.menu_Share:
                                OnShare_Button_Click();
                                break;
                        }
                    }
                    catch (Exception exception)
                    {
                        Crashes.TrackError(exception);
                    }
                };
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Menu >> Copy Link
        private void OnCopyLink_Button_Click()
        {
            try
            {
                var clipboardManager = (ClipboardManager) GetSystemService(ClipboardService);

                var clipData = ClipData.NewPlainText("text", _Product_Data.url);
                clipboardManager.PrimaryClip = clipData;

                Toast.MakeText(this, GetText(Resource.String.Lbl_Copied), ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Event Menu >> Share
        private async void OnShare_Button_Click()
        {
            try
            {
                //Share Plugin same as video
                if (!CrossShare.IsSupported) return;

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = IMethods.Fun_String.DecodeString(
                        IMethods.Fun_String.DecodeStringWithEnter(_Product_Data.name)),
                    Text = IMethods.Fun_String.DecodeString(
                        IMethods.Fun_String.DecodeStringWithEnter(_Product_Data.description)),
                    Url = _Product_Data.url
                });
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
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
          
        private CardView cvProductDetails;
        private LinearLayout llProductDetails;
        private RelativeLayout rlContainer;


        private ImageView IconBack;

        private TextView Txt_Product_Title;
        private TextView Txt_Product_Price;
        private TextView Txt_Product_BoleanNew;
        private TextView Txt_Product_BoleanInStock;
        private TextView Txt_Product_Description;
        private TextView Txt_Product_Location;
        private TextView Txt_Product_CardName;
        private TextView Txt_Product_Time;

        private ImageView ImageMore;
        private Button Btn_Contact;

        private ImageViewAsync imgBackground;
        private ImageViewAsync UserImageAvatar;

        private Get_Products_Object.Product _Product_Data;

        #endregion
    }
}