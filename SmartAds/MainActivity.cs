using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using System;
using Android.Content;
using static SmartAds.Common;

namespace SmartAds
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FirebaseAuthenticator firebaseAuth = new FirebaseAuthenticator();
            SetContentView(Resource.Layout.activity_main);

            TextView txtEmail = FindViewById<TextView>(Resource.Id.input_email);
            TextView txtPass = FindViewById<TextView>(Resource.Id.input_password);

            Button btnLogin = FindViewById<Button>(Resource.Id.btn_login);
            btnLogin.Click += async (sender, e) => {
                LocalUser user = await firebaseAuth.LoginWithEmailPassword(txtEmail.Text, txtPass.Text);
                if (!String.IsNullOrEmpty(user.Token))
                {
                    Intent nextActivity = new Intent(this, typeof(CampaignsActivity));
                    StartActivity(nextActivity);
                }
                else
                {
                    ShowToast(this, "Wrong e-mail or password!", ToastLength.Short);
                }
            };
            TextView lnkSignup = FindViewById<TextView>(Resource.Id.link_signup);

            lnkSignup.Click += (sender, e) => {
                Intent nextActivity = new Intent(this, typeof(CreateActivity));
                StartActivity(nextActivity);
            };

            TextView lnkForgot = FindViewById<TextView>(Resource.Id.link_forgot);
            lnkForgot.Click += (sender, e) => {
                Intent nextActivity = new Intent(this, typeof(ForgotActivity));
                StartActivity(nextActivity);
            };
        }
    }
}