using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Android.Content;
using System;
using static SmartAds.Common;

namespace SmartAds
{
    [Activity(Label = "CreateActivity")]
    public class CreateActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FirebaseAuthenticator firebaseAuth = new FirebaseAuthenticator();
            SetContentView(2130968604);

            TextView txtEmail = FindViewById<TextView>(Resource.Id.input_email);
            TextView txtPass = FindViewById<TextView>(Resource.Id.input_password);

            Button btnCreate = FindViewById<Button>(Resource.Id.btn_signup);
            btnCreate.Click += async (sender, e) => {
                LocalUser user = await firebaseAuth.CreateUserWithEmailPassword(txtEmail.Text, txtPass.Text);
                if (!String.IsNullOrEmpty(user.Token))
                {
                    ShowToast(this, "Successfully created account!", ToastLength.Short);
                    Intent nextActivity = new Intent(this, typeof(CampaignsActivity));
                    StartActivity(nextActivity);
                }
                else
                {
                    ShowToast(this, "Cannot register user!", ToastLength.Short);
                }
            };
            TextView lnkBack = FindViewById<TextView>(Resource.Id.link_login);

            lnkBack.Click += (sender, e) => {
                OnBackPressed();
            };
        }
    }
}