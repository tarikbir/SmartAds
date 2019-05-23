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
using static SmartAds.Common;

namespace SmartAds
{
    [Activity(Label = "ForgetActivity")]
    public class ForgotActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FirebaseAuthenticator firebaseAuth = new FirebaseAuthenticator();
            SetContentView(2130968619);

            TextView txtEmail = FindViewById<TextView>(Resource.Id.input_email);

            Button btnForgot = FindViewById<Button>(Resource.Id.btn_forgot);
            btnForgot.Click += async (sender, e) => {
                bool result = await firebaseAuth.SendPasswordReset(txtEmail.Text);
                if (result)
                {
                    ShowToast(this, "Sent e-mail to " + txtEmail.Text + "!", ToastLength.Short);
                    btnForgot.Enabled = false;
                }
                else
                {
                    ShowToast(this, "Cannot reach servers!", ToastLength.Short);
                }
            };
        }
    }
}