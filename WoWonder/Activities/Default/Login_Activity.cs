using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Org.Json;
using SettingsConnecter;
using WoWonder.Activities.Tabbes;
using WoWonder.Helpers;
using WoWonder.Helpers.SocialLogins;
using WoWonder.Library.OneSignal;
using WoWonder.SQLite;
using WoWonder_API;
using WoWonder_API.Classes.Global;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using Client = WoWonder_API.Requests.Client;
using IMethods = WoWonder.Helpers.IMethods;
using Object = Java.Lang.Object;

namespace WoWonder.Activities.Default
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Login_Activity : AppCompatActivity, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback,
        GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener , IResultCallback
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                //View view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.Login_Layout);
                SetContentView(Resource.Layout.Login_Layout);

                regularTxt = Typeface.CreateFromAsset(Assets, "fonts/SF-UI-Display-Regular.ttf");
                semiboldTxt = Typeface.CreateFromAsset(Assets, "fonts/SF-UI-Display-Semibold.ttf");

                //declare layouts and editext
                mEditTextEmail = (EditText) FindViewById(Resource.Id.editTxtEmail);
                mEditTextPassword = (EditText) FindViewById(Resource.Id.editTxtPassword);

                mTextViewSignUp = (TextView) FindViewById(Resource.Id.tvSignUp); // Register
                mButtonViewSignIn = (Button) FindViewById(Resource.Id.SignInButton); // Login

                mTextViewForgotPwd = (TextView) FindViewById(Resource.Id.tvForgotPwd); // Forget password 
                mTextViewCreateAccount = (TextView) FindViewById(Resource.Id.tvCreateAccount);

                progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                progressBar.Visibility = ViewStates.Gone;
                mButtonViewSignIn.Visibility = ViewStates.Visible;

                //mTextViewSignIn.SetTypeface(semiboldTxt,TypefaceStyle.Bold);
                mTextViewCreateAccount.SetTypeface(regularTxt, TypefaceStyle.Normal);
                mTextViewForgotPwd.SetTypeface(regularTxt, TypefaceStyle.Normal);
                mTextViewSignUp.SetTypeface(regularTxt, TypefaceStyle.Normal);
                mEditTextEmail.SetTypeface(regularTxt, TypefaceStyle.Normal);
                mEditTextPassword.SetTypeface(regularTxt, TypefaceStyle.Normal);

                FacebookSdk.SdkInitialize(this);

                mprofileTracker = new FB_MyProfileTracker();
                mprofileTracker.mOnProfileChanged += MprofileTrackerOnM_OnProfileChanged;
                mprofileTracker.StartTracking();

                BtnFBLogin = FindViewById<LoginButton>(Resource.Id.fblogin_button);
                BtnFBLogin.SetReadPermissions(new List<string>
                {
                    "email",
                    "public_profile"
                });

                mFBCallManager = CallbackManagerFactory.Create();
                BtnFBLogin.RegisterCallback(mFBCallManager, this);


                //FB accessToken
                var accessToken = AccessToken.CurrentAccessToken;
                var isLoggedIn = accessToken != null && !accessToken.IsExpired;

                // Configure sign-in to request the user's ID, email address, and basic profile. ID and basic profile are included in DEFAULT_SIGN_IN.
                var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestIdToken(GoogleServices.ClientId)
                    .RequestScopes(new Scope(Scopes.Profile))
                    .RequestScopes(new Scope(Scopes.PlusLogin))
                    .RequestServerAuthCode(GoogleServices.ClientId)
                    .RequestProfile().RequestEmail().Build();

                // Build a GoogleApiClient with access to the Google Sign-In API and the options specified by gso.
                mGoogleApiClient = new GoogleApiClient.Builder(this, this, this)
                    .EnableAutoManage(this, this)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .Build();

                mGoogleSignIn = FindViewById<SignInButton>(Resource.Id.Googlelogin_button);
                mGoogleSignIn.Click += MGsignBtnOnClick;
                mGoogleSignIn.SetSize(SignInButton.SizeStandard);

                if (!Settings.Show_Facebook_Login)
                    BtnFBLogin.Visibility = ViewStates.Invisible;

                if (!Settings.Show_Google_Login)
                    mGoogleSignIn.Visibility = ViewStates.Invisible;

                IMethods.IApp.GetKeyHashesConfigured(this);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        protected override void OnStart()
        {
            try
            {
                base.OnStart(); 
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
                mButtonViewSignIn.Click += BtnLoginOnClick;
                mTextViewCreateAccount.Click += RegisterButton_Click;
                mTextViewSignUp.Click += RegisterButton_Click;
                mTextViewForgotPwd.Click += TxtForgetpassOnClick;
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
                mButtonViewSignIn.Click -= BtnLoginOnClick;
                mTextViewCreateAccount.Click -= RegisterButton_Click;
                mTextViewSignUp.Click -= RegisterButton_Click;
                mTextViewForgotPwd.Click -= TxtForgetpassOnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        protected override void OnStop()
        {
            try
            {
                base.OnStop();
                if (mGoogleApiClient.IsConnected) mGoogleApiClient.Disconnect();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Click Button Login
        private async void BtnLoginOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                        GetText(Resource.String.Lbl_CheckYourInternetConnection), GetText(Resource.String.Lbl_Ok));
                }
                else
                {
                    if (!string.IsNullOrEmpty(mEditTextEmail.Text) || !string.IsNullOrEmpty(mEditTextPassword.Text))
                    {
                        Current.CurrentInstance.SetWebsiteURL(st.WebsiteUrl, st.ServerKey);

                        progressBar.Visibility = ViewStates.Visible;
                        mButtonViewSignIn.Visibility = ViewStates.Gone;
 
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
                                if (Settings.ShowNotification) OneSignalNotification.RegisterNotificationDevice();
                            }

                            var (Api_status, Respond) = await Client.Global.Get_Auth(st, mEditTextEmail.Text,mEditTextPassword.Text, "UTC", UserDetails.Device_ID);
                            if (Api_status == 200)
                            {
                                if (Respond is Auth_Object auth)
                                {
                                    WoWonder_API.Client.WebsiteUrl = st.WebsiteUrl;
                                    Current.CurrentInstance.ServerKey = st.ServerKey;
                                    Current.CurrentInstance.Access_token = auth.access_token;

                                    UserDetails.Username = mEditTextEmail.Text;
                                    UserDetails.Full_name = mEditTextEmail.Text;
                                    UserDetails.Password = mEditTextPassword.Text;
                                    UserDetails.access_token = auth.access_token;
                                    UserDetails.User_id = auth.user_id;
                                    UserDetails.Status = "Active";
                                    UserDetails.Cookie = auth.access_token;
                                    UserDetails.Email = mEditTextEmail.Text;

                                    //Insert user data to database
                                    var user = new DataTables.LoginTB
                                    {
                                        UserID = UserDetails.User_id,
                                        access_token = UserDetails.access_token,
                                        Cookie = UserDetails.Cookie,
                                        Username = mEditTextEmail.Text,
                                        Password = mEditTextPassword.Text,
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
                                 
                                    progressBar.Visibility = ViewStates.Gone;
                                    mButtonViewSignIn.Visibility = ViewStates.Visible;
                                    Finish();
                                }
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
                                            GetText(Resource.String.Lbl_ErrorLogin_3), GetText(Resource.String.Lbl_Ok));
                                    else if (errorid == "4")
                                        IMethods.DialogPopup.InvokeAndShowDialog(this,
                                            GetText(Resource.String.Lbl_Security),
                                            GetText(Resource.String.Lbl_ErrorLogin_4), GetText(Resource.String.Lbl_Ok));
                                    else if (errorid == "5")
                                        IMethods.DialogPopup.InvokeAndShowDialog(this,
                                            GetText(Resource.String.Lbl_Security),
                                            GetText(Resource.String.Lbl_ErrorLogin_5), GetText(Resource.String.Lbl_Ok));
                                    else
                                        IMethods.DialogPopup.InvokeAndShowDialog(this,
                                            GetText(Resource.String.Lbl_Security), errortext,
                                            GetText(Resource.String.Lbl_Ok));
                                }

                                progressBar.Visibility = ViewStates.Gone;
                                mButtonViewSignIn.Visibility = ViewStates.Visible;
                            }
                            else if (Api_status == 404)
                            {
                                //var Error = Respond.ToString();
                                progressBar.Visibility = ViewStates.Gone;
                                mButtonViewSignIn.Visibility = ViewStates.Visible;
                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                    GetText(Resource.String.Lbl_Error_Login), GetText(Resource.String.Lbl_Ok));
                            }
                        }
                    }
                    else
                    {
                        progressBar.Visibility = ViewStates.Gone;
                        mButtonViewSignIn.Visibility = ViewStates.Visible;
                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                            GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                    }
                }
            }
            catch (Exception exception)
            {
                progressBar.Visibility = ViewStates.Gone;
                mButtonViewSignIn.Visibility = ViewStates.Visible;
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message,
                    GetText(Resource.String.Lbl_Ok));
                Crashes.TrackError(exception);
            }
        }

        //Click Button Register
        private void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(Register_Activity)));
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Click Forget Password
        private void TxtForgetpassOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                StartActivity(typeof(ForgetPassword_Activity));
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                Log.Debug("Login_Activity", "onActivityResult:" + requestCode + ":" + resultCode + ":" + data);
                if (requestCode == 0)
                {
                    var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                    HandleSignInResult(result);
                }
                else
                {
                    // Logins Facebook
                    mFBCallManager.OnActivityResult(requestCode, (int) resultCode, data);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,  Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 110)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        if (!mGoogleApiClient.IsConnecting)
                            ResolveSignInError();
                        else if (mGoogleApiClient.IsConnected) mGoogleApiClient.Disconnect();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                    }
                }
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
                mprofileTracker.StopTracking();
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #region Variables Basic

        private TextView mTextViewForgotPwd, mTextViewCreateAccount, mTextViewSignUp;
        private Typeface regularTxt, semiboldTxt;
        private EditText mEditTextEmail, mEditTextPassword;
        private Button mButtonViewSignIn;
        private ProgressBar progressBar;

        private LoginButton BtnFBLogin;
        private ICallbackManager mFBCallManager;
        private FB_MyProfileTracker mprofileTracker;

        public static GoogleApiClient mGoogleApiClient;
        private SignInButton mGoogleSignIn;
        private Settings st = new Settings();
        #endregion

        #region Social Logins

        private string FB_firstName, FB_lastName, FB_name, FB_email, FB_accessToken, FB_profileId;
        private string G_firstName, G_lastName, G_profileId;
        private string G_AccountName,G_AccountType,G_displayName,G_email,G_Img,G_Idtoken,G_accessToken,G_ServerCode;

        #region Facebook

        public void OnCancel()
        {
            try
            {
                progressBar.Visibility = ViewStates.Gone;
                mButtonViewSignIn.Visibility = ViewStates.Visible;

                SetResult(Result.Canceled);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void OnError(FacebookException error)
        {
            try
            {

                progressBar.Visibility = ViewStates.Gone;
                mButtonViewSignIn.Visibility = ViewStates.Visible;

                // Handle exception
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), error.Message, GetText(Resource.String.Lbl_Ok));

                SetResult(Result.Canceled);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void OnSuccess(Object result)
        {
            try
            {
                var loginResult = result as LoginResult;
                var id = AccessToken.CurrentAccessToken.UserId;

                progressBar.Visibility = ViewStates.Visible;
                mButtonViewSignIn.Visibility = ViewStates.Gone;

                SetResult(Result.Ok);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public async void OnCompleted(JSONObject json, GraphResponse response)
        {
            try
            {
                var data = json.ToString();
                var result = JsonConvert.DeserializeObject<FacebookResult>(data);
                FB_email = result.email;

                var accessToken = AccessToken.CurrentAccessToken;
                if (accessToken != null)
                {
                    FB_accessToken = accessToken.Token;

                    Current.CurrentInstance.SetWebsiteURL(st.WebsiteUrl, st.ServerKey);
                    
                    var settingsResult = await Current.GetWoWonder_Settings(st , "WoWonder_Native_Android");
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
                            if (Settings.ShowNotification) OneSignalNotification.RegisterNotificationDevice();
                        }
                         
                        var (Api_status, respond) = await Client.Global.Get_SocialLogin(st, FB_accessToken, "facebook");
                        if (Api_status == 200)
                        {
                            if (respond is Auth_Object auth)
                            {
                                WoWonder_API.Client.WebsiteUrl = st.WebsiteUrl;
                                Current.CurrentInstance.ServerKey = st.ServerKey;
                                Current.CurrentInstance.Access_token = auth.access_token;

                                UserDetails.Username = mEditTextEmail.Text;
                                UserDetails.Full_name = mEditTextEmail.Text;
                                UserDetails.Password = mEditTextPassword.Text;
                                UserDetails.access_token = auth.access_token;
                                UserDetails.User_id = auth.user_id;
                                UserDetails.Status = "Active";
                                UserDetails.Cookie = auth.access_token;
                                UserDetails.Email = mEditTextEmail.Text;

                                //Insert user data to database
                                var user = new DataTables.LoginTB
                                {
                                    UserID = UserDetails.User_id,
                                    access_token = UserDetails.access_token,
                                    Cookie = UserDetails.Cookie,
                                    Username = mEditTextEmail.Text,
                                    Password = mEditTextPassword.Text,
                                    Status = "Active",
                                    Lang = "",
                                    Device_ID = UserDetails.Device_ID,
                                };
                                Classes.DataUserLoginList.Add(user);

                                var dbDatabase = new SqLiteDatabase();
                                dbDatabase.InsertRow(user);
                                dbDatabase.Dispose();

                                StartActivity(new Intent(this, typeof(AppIntroWalkTroutPage)));
                            }
                        }
                        else if (Api_status == 400)
                        {
                            if (respond is Error_Object error)
                            {
                                var errortext = error._errors.Error_text;
                                //Toast.MakeText(this, errortext, ToastLength.Short).Show();

                                if (errortext.Contains("Invalid or expired access_token"))
                                    API_Request.Logout(this);
                            }
                        }
                        else if (Api_status == 404)
                        {
                            var error = respond.ToString();
                            //Toast.MakeText(this, error, ToastLength.Short).Show();
                        }

                        progressBar.Visibility = ViewStates.Gone;
                        mButtonViewSignIn.Visibility = ViewStates.Visible;
                        Finish();
                    }
                }

                //bool isLoggedIn = accessToken != null && !accessToken.IsExpired;
            }
            catch (Exception exception)
            {
                progressBar.Visibility = ViewStates.Gone;
                mButtonViewSignIn.Visibility = ViewStates.Visible;
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message, GetText(Resource.String.Lbl_Ok));
                Crashes.TrackError(exception);
            }
        }

        private void MprofileTrackerOnM_OnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {
            try
            {
                if (e.mProfile != null)
                    try
                    {
                        FB_firstName = e.mProfile.FirstName;
                        FB_lastName = e.mProfile.LastName;
                        FB_name = e.mProfile.Name;
                        FB_profileId = e.mProfile.Id;

                        var request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);
                        var parameters = new Bundle();
                        parameters.PutString("fields", "id,name,age_range,email");
                        request.Parameters = parameters;
                        request.ExecuteAsync();
                    }
                    catch (Java.Lang.Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                else
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Null_Data_User), ToastLength.Short).Show();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        #endregion

        //======================================================

        #region Google

        //Event Click login using google
        private void MGsignBtnOnClick(object sender, EventArgs e)
        {
            try
            {
                mGoogleApiClient.Connect();

                var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
                if (opr.IsDone)
                {
                    // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
                    // and the GoogleSignInResult will be available instantly.
                    Log.Debug("Login_Activity", "Got cached sign-in");
                    var result = opr.Get() as GoogleSignInResult;
                    HandleSignInResult(result);

                    //Auth.GoogleSignInApi.SignOut(mGoogleApiClient).SetResultCallback(this);
                }
                else
                {
                    // If the user has not previously signed in on this device or the sign-in has expired,
                    // this asynchronous branch will attempt to sign in the user silently.  Cross-device
                    // single sign-on will occur in this branch.
                    opr.SetResultCallback(new SignInResultCallback { Activity = this });
                }

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    if (!mGoogleApiClient.IsConnecting)
                        ResolveSignInError();
                    else if (mGoogleApiClient.IsConnected) mGoogleApiClient.Disconnect();
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.GetAccounts) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.UseCredentials) == Permission.Granted)
                    {
                        if (!mGoogleApiClient.IsConnecting)
                            ResolveSignInError();
                        else if (mGoogleApiClient.IsConnected) mGoogleApiClient.Disconnect();
                    }
                    else
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.GetAccounts,
                            Manifest.Permission.UseCredentials
                        }, 110);
                    }
                } 
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void HandleSignInResult(GoogleSignInResult result)
        {
            try
            { 
                Log.Debug("Login_Activity", "handleSignInResult:" + result.IsSuccess);
                if (result.IsSuccess)
                {
                    // Signed in successfully, show authenticated UI.
                    var acct = result.SignInAccount;
                    SetContentGoogle(acct);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void ResolveSignInError()
        {
            try
            {
                if (mGoogleApiClient.IsConnecting) return;

                var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
                StartActivityForResult(signInIntent, 0);
            }
            catch (IntentSender.SendIntentException io)
            {
                //The intent was cancelled before it was sent. Return to the default
                //state and attempt to connect to get an updated ConnectionResult
                Crashes.TrackError(io);
                mGoogleApiClient.Connect();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnConnected(Bundle connectionHint)
        {
            try
            {
                var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
                if (opr.IsDone)
                {
                    // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
                    // and the GoogleSignInResult will be available instantly.
                    Log.Debug("Login_Activity", "Got cached sign-in");
                    var result = opr.Get() as GoogleSignInResult;
                    HandleSignInResult(result);
                }
                else
                {
                    // If the user has not previously signed in on this device or the sign-in has expired,
                    // this asynchronous branch will attempt to sign in the user silently.  Cross-device
                    // single sign-on will occur in this branch.

                    opr.SetResultCallback(new SignInResultCallback {Activity = this});
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public async void SetContentGoogle(GoogleSignInAccount acct)
        {
            try
            {
                //Successful log in hooray!!
                if (acct != null)
                {
                    progressBar.Visibility = ViewStates.Visible;
                    mButtonViewSignIn.Visibility = ViewStates.Gone;

                    G_AccountName = acct.Account.Name;
                    G_AccountType = acct.Account.Type;
                    G_displayName = acct.DisplayName;
                    G_firstName = acct.GivenName;
                    G_lastName = acct.FamilyName;
                    G_profileId = acct.Id;
                    G_email = acct.Email;
                    G_Img = acct.PhotoUrl.Path;
                    G_Idtoken = acct.IdToken;
                    G_ServerCode = acct.ServerAuthCode;

                    var api = new GoogleAPI();
                    G_accessToken = await api.GetAccessTokenAsync(G_ServerCode);

                    if (!string.IsNullOrEmpty(G_accessToken))
                    {
                        Current.CurrentInstance.SetWebsiteURL(st.WebsiteUrl, st.ServerKey);

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
                                if (Settings.ShowNotification) OneSignalNotification.RegisterNotificationDevice();
                            }

                            string key = IMethods.IApp.GetValueFromManifest(this, "com.google.android.geo.API_KEY");
                            var (Api_status, respond) = await Client.Global.Get_SocialLogin(st, G_accessToken, "google", key);
                            if (Api_status == 200)
                            {
                                if (respond is Auth_Object auth)
                                {
                                    WoWonder_API.Client.WebsiteUrl = st.WebsiteUrl;
                                    Current.CurrentInstance.ServerKey = st.ServerKey;
                                    Current.CurrentInstance.Access_token = auth.access_token;

                                    UserDetails.Username = mEditTextEmail.Text;
                                    UserDetails.Full_name = mEditTextEmail.Text;
                                    UserDetails.Password = mEditTextPassword.Text;
                                    UserDetails.access_token = auth.access_token;
                                    UserDetails.User_id = auth.user_id;
                                    UserDetails.Status = "Active";
                                    UserDetails.Cookie = auth.access_token;
                                    UserDetails.Email = mEditTextEmail.Text;

                                    //Insert user data to database
                                    var user = new DataTables.LoginTB
                                    {
                                        UserID = UserDetails.User_id,
                                        access_token = UserDetails.access_token,
                                        Cookie = UserDetails.Cookie,
                                        Username = mEditTextEmail.Text,
                                        Password = mEditTextPassword.Text,
                                        Status = "Active",
                                        Lang = "",
                                        Device_ID = UserDetails.Device_ID,
                                    };
                                    Classes.DataUserLoginList.Add(user);

                                    var dbDatabase = new SqLiteDatabase();
                                    dbDatabase.InsertRow(user);
                                    dbDatabase.Dispose();

                                    StartActivity(new Intent(this, typeof(AppIntroWalkTroutPage)));
                                }
                            }
                            else if (Api_status == 400)
                            {
                                if (respond is Error_Object error)
                                {
                                    var errortext = error._errors.Error_text;
                                    //Toast.MakeText(this, errortext, ToastLength.Short).Show();

                                    if (errortext.Contains("Invalid or expired access_token"))
                                        API_Request.Logout(this);
                                }
                            }
                            else if (Api_status == 404)
                            {
                                var error = respond.ToString();
                                //Toast.MakeText(this, error, ToastLength.Short).Show();
                            }

                            progressBar.Visibility = ViewStates.Gone;
                            mButtonViewSignIn.Visibility = ViewStates.Visible;
                            Finish();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                progressBar.Visibility = ViewStates.Gone;
                mButtonViewSignIn.Visibility = ViewStates.Visible;
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message,GetText(Resource.String.Lbl_Ok));
                Crashes.TrackError(exception);
            }
        }

        public void OnConnectionSuspended(int cause)
        {
            try
            {
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            try
            {
                // An unresolvable error has occurred and Google APIs (including Sign-In) will not
                // be available.
                Log.Debug("Login_Activity", "onConnectionFailed:" + result);

                //The user has already clicked 'sign-in' so we attempt to resolve all
                //errors until the user is signed in, or the cancel
                ResolveSignInError();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

      

        public void OnResult(Object result)
        {
            try
            {
                 
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        #endregion

        //======================================================

        #endregion
    }
}