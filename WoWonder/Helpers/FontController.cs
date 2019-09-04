using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Java.Lang;
using String = System.String;

namespace WoWonder.Helpers
{
    public class FontController 
    {
        public TextView TextviewResizable;
        public int MaxLine;
        public string ExpandText;
        public bool ViewMore;

        public static void SetFont(TextView textView ,int type)
        {
            Typeface regularTxt = Typeface.CreateFromAsset(textView.Context.Assets, "fonts/SF-UI-Display-Regular.ttf");
            Typeface semiboldTxt = Typeface.CreateFromAsset(textView.Context.Assets, "fonts/SF-UI-Display-Semibold.ttf");
            Typeface semiMediumTxt = Typeface.CreateFromAsset(textView.Context.Assets, "fonts/SF-UI-Display-Medium.ttf");

            if (type == 1)
            {
                textView.SetTypeface(regularTxt, TypefaceStyle.Normal);
            }
            else if(type == 2)
            {
                textView.SetTypeface(semiboldTxt, TypefaceStyle.Bold);
            }
            else 
            {
                textView.SetTypeface(semiMediumTxt, TypefaceStyle.Normal);
            }
        }

       

    }
}