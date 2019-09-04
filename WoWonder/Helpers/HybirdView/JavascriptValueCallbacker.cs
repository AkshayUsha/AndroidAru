//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) WoWOnder 15/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Android.Webkit;
using Microsoft.AppCenter.Crashes;
using Object = Java.Lang.Object;

namespace WoWonder.Helpers.HybirdView
{
    public class JavascriptValueCallbacker  : Object, IValueCallback
    {
        public WebView HybirdView;
        public bool FirstInject;
        public Object Value { get; set; }

        public JavascriptValueCallbacker(WebView hview)
        {
            try
            {
                
                HybirdView = hview;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void OnReceiveValue(Object value)
        {
            try
            {
                if (value is Java.Lang.String)
                {
                    // Unescape that damn Unicode Java bull.
                    var response = Regex.Replace(value.ToString(), @"\\[Uu]([0-9A-Fa-f]{4})", m => char.ToString((char)ushort.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier)));
                    response = Regex.Unescape(response);

                    if (response.Equals("\"null\""))
                        response = null;

                    else if (response.StartsWith("\"") && response.EndsWith("\""))
                        response = response.Substring(1, response.Length - 2);

                    Value = response;

                    if(response == "<html><head></head><body></body></html>")
                        return;

                    if (!string.IsNullOrEmpty(response) && response.Contains("<html><head>") && FirstInject == false)
                    {
                        FirstInject = true;
                        SaveWebsiteCachContent(response);
                    }
                       

                }
                else
                {
                    Value = null;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }


        public void SaveWebsiteCachContent(string result)
        {
            try
            {
                if (FirstInject)
                {
                    string file = IMethods.IPath.FolderDiskPost + "news_feed.html";

                    if (File.Exists(file))
                        File.Delete(file);

                    using (FileStream fs = new FileStream(file, FileMode.Create))
                    {
                        using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                        {
                            w.WriteLine(result);
                            w.Close();
                        }
                    }
                    FirstInject = false;
                }
              
            }
            catch (Exception e)
            {
                FirstInject = false;
                Crashes.TrackError(e);
            }
        }
    }
}