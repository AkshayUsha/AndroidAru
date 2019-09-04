//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) WoWonder 15/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using Android.Webkit;
namespace WoWonder.Helpers.HybirdView
{
   public class WoChromeClient : WebChromeClient
    {
        public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
        {
            base.OnGeolocationPermissionsShowPrompt(origin, callback);
        }
    }
}