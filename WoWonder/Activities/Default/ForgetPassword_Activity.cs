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
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Requests;

namespace WoWonder.Activities.Default
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class ForgetPassword_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);


                //View view = MyContextWrapper.GetContentView(this, Settings.Lang, Resource.Layout.ForgetPassword_Layout);
                // Create your application here
                SetContentView(Resource.Layout.ForgetPassword_Layout);

                EmailEditext = FindViewById<EditText>(Resource.Id.emailfield);
                Btn_Send = FindViewById<Button>(Resource.Id.SendButton);
                progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

                Main_LinearLayout = FindViewById<LinearLayout>(Resource.Id.mainLinearLayout);

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
                Btn_Send.Click += BtnSendOnClick;
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
                Btn_Send.Click -= BtnSendOnClick;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private async void BtnSendOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!string.IsNullOrEmpty(EmailEditext.Text))
                {
                    if (IMethods.CheckConnectivity())
                    {
                        var check = IMethods.Fun_String.IsEmailValid(EmailEditext.Text);
                        if (!check)
                        {
                            IMethods.DialogPopup.InvokeAndShowDialog(this,
                                GetText(Resource.String.Lbl_VerificationFailed),
                                GetText(Resource.String.Lbl_IsEmailValid), GetText(Resource.String.Lbl_Ok));
                        }
                        else
                        {
                            progressBar.Visibility = ViewStates.Visible;
                            var (Api_status, Respond) = await Client.Global.Get_Reset_Password_Email(new Settings(), EmailEditext.Text);
                            if (Api_status == 200)
                            {
                                if (Respond is Get_Reset_Password_Email_Object result)
                                {
                                    progressBar.Visibility = ViewStates.Invisible;
                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_VerificationFailed),
                                        GetText(Resource.String.Lbl_Email_Has_Been_Send),
                                        GetText(Resource.String.Lbl_Ok));
                                }
                            }
                            else if (Api_status == 400)
                            {
                                var error = Respond as Error_Object;
                                if (error != null)
                                {
                                    var errortext = error._errors.Error_text;
                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_Security), errortext,
                                        GetText(Resource.String.Lbl_Ok));

                                    if (errortext.Contains("Invalid or expired access_token"))
                                        API_Request.Logout(this);
                                }

                                progressBar.Visibility = ViewStates.Invisible;
                            }
                            else if (Api_status == 404)
                            {
                                //var Error = Respond.ToString();
                                progressBar.Visibility = ViewStates.Invisible;
                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                    GetText(Resource.String.Lbl_Error_Login), GetText(Resource.String.Lbl_Ok));
                            }
                        }
                    }
                    else
                    {
                        progressBar.Visibility = ViewStates.Invisible;
                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed),
                            GetText(Resource.String.Lbl_something_went_wrong), GetText(Resource.String.Lbl_Ok));
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                progressBar.Visibility = ViewStates.Invisible;
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed),
                    exception.ToString(), GetText(Resource.String.Lbl_Ok));
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

        private Button Btn_Send;
        private EditText EmailEditext;
        private LinearLayout Main_LinearLayout;

        private ProgressBar progressBar;

        #endregion
    }
}