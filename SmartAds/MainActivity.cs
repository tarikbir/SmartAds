using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using System;
using Android.Content;
using static SmartAds.Common;
using Firebase.Auth;

namespace SmartAds
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static FirebaseUser user;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FirebaseAuthenticator firebaseAuth = new FirebaseAuthenticator();
            SetContentView(Resource.Layout.activity_main);

            TextView txtEmail = FindViewById<TextView>(Resource.Id.input_email);
            TextView txtPass = FindViewById<TextView>(Resource.Id.input_password);
            Button btnLogin = FindViewById<Button>(Resource.Id.btn_login);

            btnLogin.Click += async (sender, e) => {
                if (user == null) user = await firebaseAuth.LoginWithEmailPassword(txtEmail.Text, txtPass.Text);
                if (user != null)
                {
                    ChangeIntent(typeof(CampaignsActivity));
                }
                else
                {
                    ShowToast(this, "Wrong e-mail or password!", ToastLength.Short);
                }
            };
            TextView lnkSignup = FindViewById<TextView>(Resource.Id.link_signup);

            lnkSignup.Click += (sender, e) => {
                ChangeIntent(typeof(CreateActivity));
            };

            TextView lnkForgot = FindViewById<TextView>(Resource.Id.link_forgot);
            lnkForgot.Click += (sender, e) => {
                ChangeIntent(typeof(ForgotActivity));
            };

            user = FirebaseAuth.Instance.CurrentUser;
            if (user != null)
            {
                ChangeIntent(typeof(CampaignsActivity));
            }
        }

        private void ChangeIntent(Type type)
        {
            Intent nextActivity = new Intent(this, type);
            StartActivity(nextActivity);
        }
    }
}