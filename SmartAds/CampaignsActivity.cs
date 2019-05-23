using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using static SmartAds.Common;

namespace SmartAds
{
    [Activity(Label = "CampaignsActivity")]
    public class CampaignsActivity : Activity, ILocationListener
    {
        LocationManager LocationManager;
        static Uri endpoint = new Uri("https://smartadsxamarin.firebaseapp.com/getCampaigns");
        bool locationCheck = true;

        public async void OnLocationChanged(Location location)
        {
            if (locationCheck)
            {
                locationCheck = false;
                List<Campaign> camp = await GetResponseFromRequest(new Request() { filter="All", threshold=9999, lat= location.Latitude, lng= location.Longitude});
                Log.Debug("OnLocationChanged","Got campaigns.");
                TextView text = FindViewById<TextView>(Resource.Id.link_login);
                if (camp.Count > 1)
                    text.Text = camp.FirstOrDefault().ToString();

                locationCheck = true;
            }
        }

        public void OnProviderDisabled(string provider)
        {
            Log.Debug("Location", "disable.");
        }

        public void OnProviderEnabled(string provider)
        {
            Log.Debug("Location", "enable.");
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Log.Debug("Location", "status.");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ShowToast(this, "Successfully logged in.", ToastLength.Short);

            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation, Manifest.Permission.Internet }, 10);
            if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Android.Content.PM.Permission.Granted)
            {
                LocationManager = (LocationManager)GetSystemService(LocationService);
                LocationManager.RequestLocationUpdates(LocationManager.GpsProvider, 0, 0, this);
            }

            SetContentView(2130968602);
        }

        private async Task<List<Campaign>> GetResponseFromRequest(Request req)
        {
            List<Campaign> camp = new List<Campaign>(); ;
            HttpClient httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(7) };
            var jsonSerializer = JsonConvert.SerializeObject(req);
            var content = new StringContent(jsonSerializer, Encoding.UTF8, "application/json");
            try
            {
                httpClient.CancelPendingRequests();
                Log.Debug("GetResponseFromRequest", "Ready to send POST request.");
                var resp = await httpClient.PostAsync(endpoint, content);
                if (resp.IsSuccessStatusCode)
                {
                    string respContent = await resp.Content.ReadAsStringAsync();
                    camp = JsonConvert.DeserializeObject<List<Campaign>>(respContent);
                }
            }
            catch(Exception e)
            {
                Log.Debug("GetResponseFromRequest", "Error: " + e.Message);
            }

            return camp;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}