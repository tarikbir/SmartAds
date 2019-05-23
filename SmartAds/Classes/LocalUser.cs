using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;

namespace SmartAds
{
    public class LocalUser
    {
        public bool Result { get; set; }
        public Firebase.Auth.IAuthResult AuthResult { get; set; }
        public string Token { get; set; }

        public LocalUser(IAuthResult authResult, string token)
        {
            AuthResult = authResult;
            Token = token;
        }

        public LocalUser()
        { }
    }
}