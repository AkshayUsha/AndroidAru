//##############################################

//Cᴏᴘʏʀɪɢʜᴛ 2018 DᴏᴜɢʜᴏᴜᴢLɪɢʜᴛ Codecanyon Item 19703216
//Elin Doughouz >> https://www.facebook.com/Elindoughous
//====================================================

namespace SettingsConnecter
{
    public class Settings
    {
        public Settings()
        {
            //DoughouzCheker version 3.0 
            Certificate = Costumer.Certificate;
            WebsiteUrl = "https://demo.wowonder.com/";
            ServerKey = "b37bb1cd53d0bc21c13c1644e98a58ca";
            PurchaseCode = "c99e1a62-e18e-40c7-b749-55c2bf60f5fc";
            ItemNumber = "19703216";
        }

        //Main Settings >>>>>
        //*********************************************************
        public string WebsiteUrl;
        public string ServerKey;
        public string PurchaseCode;
        public string Certificate;
        public string ItemNumber;

        public string Version = "2.3";
        public static string Application_Name = "WoWonder Timeline";

        // Friend system = 0 , follow system = 1
        public static string ConnectivitySystem = "1";
         
        public static bool WonderSystem = false;

        public static string Market_curency = "$"; //The currency used in the store

        //Main Colors >>
        //*********************************************************
        public static string MainColor = "#a84849";
        public static string Story_Read_Color = "#808080";

        //Language Settings >>
        //*********************************************************
        public static bool FlowDirection_RightToLeft = false;
        public static string Lang { set; get; } = ""; //Default language

        //Type connection >>
        //*********************************************************
        public static bool UseSocketClient = false; //For next update versions 

        //Notification Settings >>
        //*********************************************************
        public static bool ShowNotification = true;

        //Offline Watched Videos >>  
        //*********************************************************
        public static bool Allow_Offline_Download = true;

        // Walkthrough Settings >>
        //*********************************************************
        public static bool Show_WalkTroutPage = true;
        
        public static bool Walkthrough_SetFlowAnimation = false;
        public static bool Walkthrough_SetZoomAnimation = false;
        public static bool Walkthrough_SetSlideOverAnimation = false;
        public static bool Walkthrough_SetDepthAnimation = true;
        public static bool Walkthrough_SetFadeAnimation = false;

        //Main Messenger settings
        //*********************************************************
        public static bool Messenger_Integration = true;
        public static string Messenger_Package_Name = "com.wowonderandroid.messenger"; //APK name on Google Play

        //ADMOB >> Please add the code ad in the Here and Strings.xml 
        //*********************************************************
        public static bool Show_ADMOB_Banner = false;
        public static bool Show_ADMOB_Interstitial = false;
        public static bool Show_ADMOB_RewardVideo = false;
        public static bool Show_ADMOB_Native = false;

        public static string Ad_App_ID = "ca-app-pub-5135691635931982~1668785995";
        public static string Ad_Unit_ID = "ca-app-pub-5135691635931982/1369403093";
        public static string Ad_Interstitial_Key = "ca-app-pub-5135691635931982/3584502890";
        public static string Ad_RewardVideo_Key = "ca-app-pub-5135691635931982/2518408206";
        public static string Ad_Native_Key = "ca-app-pub-3940256099942544/2247696110";

        //Three times after entering the ad is displayed
        public static int Show_ADMOB_Interstitial_Count = 3;
        public static int Show_ADMOB_RewardedVideo_Count = 3;
        public static int Show_ADMOB_Native_Count = 3;

        //*********************************************************

        //Set Theme Welcome Pages 
        //*********************************************************
        //Types >> Gradient or Video or Image
        public static string BackgroundScreenWelcomeType = "Image";

        //Set Theme Full Screen App
        //*********************************************************
        public static bool EnableFullScreenApp = false;
         
        //Set Toolbar >> Market , Events
        //*********************************************************
        public static bool Show_Toolbar_Market = true;
        public static bool Show_Toolbar_Events = true;

        //Social Logins >>
        //If you want login with facebook or google you should change id key in the String.xml file or AndroidManifest.xml
        //Facebook >> ../values/Strings.xml .. line 15 - 16 
        //Google >> ../Properties/AndroidManifest.xml .. line 49
        //*********************************************************
        public static bool Show_Facebook_Login = false;
        public static bool Show_Google_Login = false;

        //########################### 

        //Main Slider settings
        //*********************************************************
        public static bool Show_Album = true;
        public static bool Show_Articles = true;
        public static bool Show_Communitie_Groups = true;
        public static bool Show_Communities_Pages = true;
        public static bool Show_Events = true;
        public static bool Show_Market = true;
        public static bool Show_Movies = true;
        public static bool Show_NearBy = true;
        public static bool Show_Story = true;
        public static bool Show_SavedPost = true;
        public static bool Show_UserContacts = true; // Follow or Friend System

        //UsersPages
        public static bool Show_ProUsers_Members = true;
        public static bool Show_Promoted_Pages = true;
        public static bool Show_Trending_Hashtags = true;
        public static bool Show_LastActivities = true;

        //Add Post
        public static bool Show_Galery_Image = true;
        public static bool Show_Galery_Video = true;
        public static bool Show_Mention = true;
        public static bool Show_Location = true;
        public static bool Show_Feeling_Activity = true;
        public static bool Show_Feeling = true;
        public static bool Show_Activity = false; //For next update version 
        public static bool Show_Listening = true;
        public static bool Show_Playing = true;
        public static bool Show_Watching = true;
        public static bool Show_Traveling = true;
        public static bool Show_Camera = true;
        public static bool Show_Gif = true;
        public static bool Show_File = true;
        public static bool Show_Music = true;

        //Settings Page >> General Account
        public static bool Show_Settings_GeneralAccount = true;

        public static bool Show_Settings_Account = true;
        public static bool Show_Settings_SocialLinks = true;
        public static bool Show_Settings_Password = true;
        public static bool Show_Settings_BlockedUsers = true;
        public static bool Show_Settings_DeleteAccount = true;

        //Settings Page >> Privacy
        public static bool Show_Settings_Privacy = true;
        public static bool Show_Settings_Notification = true;

        //Settings Page >> Tell a Friends 
        public static bool Show_Settings_InviteFriends = true;


        public static bool Show_Settings_Share = true;
        public static bool Show_Settings_MyAffiliates = true;

        //Settings Page >> Help && Support
        public static bool Show_Settings_Help_Support = true;

        public static bool Show_Settings_Help = true;
        public static bool Show_Settings_ReportProblem = true;
        public static bool Show_Settings_About = true;
        public static bool Show_Settings_PrivacyPolicy = true;
        public static bool Show_Settings_TermsOfUse = true;

        //CategoriesVideoList
        //*********************************************************
        //You can change the value of the name in the file
        // ../values/Strings.xml >> Categories Communities Or Market Local Custom
        //If you are adding a new category you must add it in:
        // ../values/Strings.xml 

        public static bool CategoriesComunities_Local = false;

        //Set Theme Tab
        //*********************************************************
        public static bool SetTabColoredTheme = false;
        public static bool SetTabDarkTheme = false;

        public static string TabColoredColor = MainColor;
        public static bool SetTabOnButton = false;
        public static bool SetTabIsTitledWithText = false;

        //Bypass Web Errors  
        //*********************************************************
        public static bool TurnTrustFailureOn_WebException = false;
        public static bool TurnSecurityProtocolType3072On = false;
        public static int RefreshWebSeconds = 10000;

        //Show custom error reporting page
        public static bool Show_Error_HybirdView = true;

        //*********************************************************
        public static bool RenderPriorityFastPostLoad = false;
        public static bool UseCachResourceLoad = false;

        public static bool LoadCachedHybirdViewOnFirstLoad = false;
        public static bool EnableCachSystem = true;
        public static bool ClearCachSystem = true;
    }
}
