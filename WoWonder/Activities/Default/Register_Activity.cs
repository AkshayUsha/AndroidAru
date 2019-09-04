using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.Activities.Tabbes;
using WoWonder.Helpers;
using WoWonder.Library.OneSignal;
using WoWonder.SQLite;
using WoWonder_API;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using Client = WoWonder_API.Requests.Client;
using IMethods = WoWonder.Helpers.IMethods;

namespace WoWonder.Activities.Default
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Register_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                //View view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Register_Layout);
                SetContentView(Resource.Layout.Register_Layout);

                try
                {
                    Window.SetBackgroundDrawableResource(Resource.Drawable.RegisterScreen);
                }
                catch (Exception exception)
                {
                    Crashes.TrackError(exception);
                }

                EmailEditext = FindViewById<EditText>(Resource.Id.emailfield);
                UsernameEditext = FindViewById<EditText>(Resource.Id.usernamefield);
                PasswordEditext = FindViewById<EditText>(Resource.Id.passwordfield);
                PasswordRepeatEditext = FindViewById<EditText>(Resource.Id.ConfirmPasswordfield);

                Main_LinearLayout = FindViewById<LinearLayout>(Resource.Id.mainLinearLayout);
                progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

                RegisterButton = FindViewById<Button>(Resource.Id.signUpButton);

                progressBar.Visibility = ViewStates.Invisible;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();

                //Add Event
                Main_LinearLayout.Click += MainLinearLayoutOnClick;
                RegisterButton.Click += RegisterButtonOnClick;
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
                Main_LinearLayout.Click -= MainLinearLayoutOnClick;
                RegisterButton.Click -= RegisterButtonOnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        private async void RegisterButtonOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    if (!string.IsNullOrEmpty(UsernameEditext.Text) || !string.IsNullOrEmpty(PasswordEditext.Text) ||
                        !string.IsNullOrEmpty(PasswordRepeatEditext.Text) || !string.IsNullOrEmpty(EmailEditext.Text))
                    {
                        Settings st = new Settings();

                        Current.CurrentInstance.SetWebsiteURL(st.WebsiteUrl, st.ServerKey);

                        var check = IMethods.Fun_String.IsEmailValid(EmailEditext.Text);
                        if (!check)
                        {
                            IMethods.DialogPopup.InvokeAndShowDialog(this,
                                GetText(Resource.String.Lbl_VerificationFailed),
                                GetText(Resource.String.Lbl_IsEmailValid), GetText(Resource.String.Lbl_Ok));
                        }
                        else
                        {
                            if (PasswordRepeatEditext.Text == PasswordEditext.Text)
                            {
                                progressBar.Visibility = ViewStates.Visible;

                                var settingsResult = await Current.GetWoWonder_Settings(st, "WoWonder_Native_Android");
                                if (settingsResult != null)
                                { 
                                    string PushID = "";
                                    try
                                    {
                                        PushID = settingsResult["push_id_2"].ToString();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                    if (OneSignalNotification.OneSignalAPP_ID == "")
                                    {
                                        OneSignalNotification.OneSignalAPP_ID = PushID;
                                        if (Settings.ShowNotification)
                                            OneSignalNotification.RegisterNotificationDevice();
                                    }

                                    var (Api_status, Respond) =
                                        await Client.Global.Get_Create_Account(UsernameEditext.Text,
                                            PasswordEditext.Text, PasswordRepeatEditext.Text, EmailEditext.Text, UserDetails.Device_ID);
                                    if (Api_status == 200)
                                    {
                                        if (Respond is Creat_Account_Object result)
                                        {
                                            WoWonder_API.Client.WebsiteUrl = st.WebsiteUrl;
                                            Current.CurrentInstance.ServerKey = st.ServerKey;
                                            Current.CurrentInstance.Access_token = result.access_token;

                                            UserDetails.Username = UsernameEditext.Text;
                                            UserDetails.Full_name = UsernameEditext.Text;
                                            UserDetails.Password = PasswordEditext.Text;
                                            UserDetails.access_token = result.access_token;
                                            UserDetails.User_id = result.user_id;
                                            UserDetails.Status = "Active";
                                            UserDetails.Cookie = result.access_token;
                                            UserDetails.Email = EmailEditext.Text;

                                            //Insert user data to database
                                            var user = new DataTables.LoginTB
                                            {
                                                UserID = UserDetails.User_id,
                                                access_token = UserDetails.access_token,
                                                Cookie = UserDetails.Cookie,
                                                Username = UsernameEditext.Text,
                                                Password = PasswordEditext.Text,
                                                Status = "Active",
                                                Lang = "",
                                                Device_ID = UserDetails.Device_ID,
                                            };

                                            Classes.DataUserLoginList.Add(user);

                                            var dbDatabase = new SqLiteDatabase();
                                            dbDatabase.InsertRow(user);
                                            dbDatabase.Dispose();

                                            if (Settings.Show_WalkTroutPage)
                                            {
                                                StartActivity(new Intent(this, typeof(AppIntroWalkTroutPage)));
                                            }
                                            else
                                            {
                                                StartActivity(new Intent(this, typeof(Tabbed_Main_Activity)));

                                                //Get data profile
                                                var data = API_Request.Get_MyProfileData_Api(this).ConfigureAwait(false);
                                            }
                                        }

                                        progressBar.Visibility = ViewStates.Invisible;
                                        Finish();
                                    }
                                    else if (Api_status == 400)
                                    {
                                        var error = Respond as Error_Object;
                                        if (error != null)
                                        {
                                            var errortext = error._errors.Error_text;

                                            var errorid = error._errors.Error_id;
                                            
                                            if (errorid == "3")
                                                IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                    GetText(Resource.String.Lbl_Security),
                                                    GetText(Resource.String.Lbl_ErrorRegister_3),
                                                    GetText(Resource.String.Lbl_Ok));
                                            else if (errorid == "4")
                                                IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                    GetText(Resource.String.Lbl_Security),
                                                    GetText(Resource.String.Lbl_ErrorRegister_4),
                                                    GetText(Resource.String.Lbl_Ok));
                                            else if (errorid == "5")
                                                IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                    GetText(Resource.String.Lbl_Security),
                                                    GetText(Resource.String.Lbl_something_went_wrong),
                                                    GetText(Resource.String.Lbl_Ok));
                                            else if (errorid == "6")
                                                IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                    GetText(Resource.String.Lbl_Security),
                                                    GetText(Resource.String.Lbl_ErrorRegister_6),
                                                    GetText(Resource.String.Lbl_Ok));
                                            else if (errorid == "7")
                                                IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                    GetText(Resource.String.Lbl_Security),
                                                    GetText(Resource.String.Lbl_ErrorRegister_7),
                                                    GetText(Resource.String.Lbl_Ok));
                                            else if (errorid == "8")
                                                IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                    GetText(Resource.String.Lbl_Security),
                                                    GetText(Resource.String.Lbl_ErrorRegister_8),
                                                    GetText(Resource.String.Lbl_Ok));
                                            else if (errorid == "9")
                                                IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                    GetText(Resource.String.Lbl_Security),
                                                    GetText(Resource.String.Lbl_ErrorRegister_9),
                                                    GetText(Resource.String.Lbl_Ok));
                                            else if (errorid == "10")
                                                IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                    GetText(Resource.String.Lbl_Security),
                                                    GetText(Resource.String.Lbl_ErrorRegister_10),
                                                    GetText(Resource.String.Lbl_Ok));
                                            else if (errorid == "11")
                                                IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                    GetText(Resource.String.Lbl_Security),
                                                    GetText(Resource.String.Lbl_ErrorRegister_11),
                                                    GetText(Resource.String.Lbl_Ok));
                                            else
                                                IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                    GetText(Resource.String.Lbl_Security), errortext,
                                                    GetText(Resource.String.Lbl_Ok));
                                        }

                                        progressBar.Visibility = ViewStates.Invisible;
                                    }
                                    else if (Api_status == 404)
                                    {
                                        //var Error = Respond.ToString();
                                        progressBar.Visibility = ViewStates.Invisible;
                                        IMethods.DialogPopup.InvokeAndShowDialog(this,
                                            GetText(Resource.String.Lbl_Security),
                                            GetText(Resource.String.Lbl_Error_Login), GetText(Resource.String.Lbl_Ok));
                                    }
                                }
                            }
                            else
                            {
                                progressBar.Visibility = ViewStates.Invisible;

                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                    GetText(Resource.String.Lbl_Error_Register_password),
                                    GetText(Resource.String.Lbl_Ok));
                            }
                        }
                    }
                    else
                    {
                        progressBar.Visibility = ViewStates.Invisible;

                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                            GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                    }
                }
                else
                {
                    progressBar.Visibility = ViewStates.Invisible;
                    //IMethods.DialogPopup.Hide_Loading_Dialog(this);
                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                        GetText(Resource.String.Lbl_CheckYourInternetConnection), GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                progressBar.Visibility = ViewStates.Invisible;
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message,
                    GetText(Resource.String.Lbl_Ok));
            }
        }

        private void MainLinearLayoutOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var inputManager = (InputMethodManager) GetSystemService(InputMethodService);
                inputManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.None);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
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

        private Button RegisterButton;
        private EditText EmailEditext;
        private EditText UsernameEditext;
        private EditText PasswordEditext;
        private EditText PasswordRepeatEditext;
        private LinearLayout Main_LinearLayout;

        private ProgressBar progressBar;

        #endregion
    }
}