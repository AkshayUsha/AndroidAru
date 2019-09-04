using System;
using System.Globalization;
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using Java.Util;
using Microsoft.AppCenter.Crashes;
using WoWonder.Activities.SettingsPreferences;


namespace WoWonder.Helpers
{
    public class MyContextWrapper : ContextWrapper
    {
        private Context _context;
        private static string Language = "";

        protected MyContextWrapper(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public MyContextWrapper(Context context) : base(context)
        {
            _context = context;
        }
        
        public static ContextWrapper Wrap(Context context, string language )
        {
            try
            {
                Language = language;

                Configuration config = context.Resources.Configuration;
                Locale sysLocale = null;

                sysLocale = config.Locales.Get(0);

                if (!language.Equals("") && !sysLocale.Language.Equals(language))
                {
                    sysLocale = new Locale(language);
                    Locale.Default = sysLocale;
                }
                CultureInfo myCulture = new CultureInfo(language);
                CultureInfo.DefaultThreadCurrentCulture = myCulture;
                config.SetLocale(sysLocale);

                var ss = context.Resources.Configuration.Locale;

                WowTime_Main_Settings.Shared_Data.Edit().PutString("Lang_key", language).Commit();

                //context = context.CreateConfigurationContext(config);
                context.Resources.UpdateConfiguration(config, null);

                return new MyContextWrapper(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new MyContextWrapper(context);
            } 
        }
        
        public static View GetContentView(Context context, string language , int layout)
        {
            try
            {
                Context newContext = Wrap(context, language);
                View view = null;
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                view = inflater.Inflate(newContext.Resources.GetLayout(layout), null);

                return view;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            } 
        }
    }
}