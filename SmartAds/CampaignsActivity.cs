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
        private LocationManager LocationManager;
        private static Uri endpoint = new Uri("https://smartadsxamarin.firebaseapp.com/getCampaigns");
        HttpClient httpClient;
        private bool locationCheck = true;
        private ListView listView;
        private TextView FilterText;
        private string filter;
        private TextView ThresholdText;
        private int threshold;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ShowToast(this, "Successfully logged in.", ToastLength.Short);
            httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(7) };
            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation, Manifest.Permission.Internet }, 10);
            if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Android.Content.PM.Permission.Granted)
            {
                LocationManager = (LocationManager)GetSystemService(LocationService);
                LocationManager.RequestLocationUpdates(LocationManager.GpsProvider, 0, 0, this);
            }

            SetContentView(2130968602);

            FilterText = FindViewById<TextView>(Resource.Id.text_filter);
            ThresholdText = FindViewById<TextView>(Resource.Id.text_threshold);
            listView = FindViewById<ListView>(Resource.Id.campaign_list);
            ChangeFilter("All");
            FilterText.Click += FilterText_Click;
            ChangeThreshold(500);
            ThresholdText.Click += ThresholdText_Click;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public async void OnLocationChanged(Location location)
        {
            if (locationCheck)
            {
                locationCheck = false;
                List<Campaign> camp = await GetResponseFromRequest(new Request() { filter = filter, threshold = threshold, lat = location.Latitude, lng = location.Longitude });
                Log.Debug("OnLocationChanged","Got campaigns response.");
                if (camp.Count > 0)
                {
                    CampaignListAdapter arrayAdapter = new CampaignListAdapter(this, camp);
                    listView.Adapter = arrayAdapter;
                }

                locationCheck = true;
            }
        }

        public void OnProviderDisabled(string provider)
        {
            Log.Debug("Location", "Provider disabled.");
        }

        public void OnProviderEnabled(string provider)
        {
            Log.Debug("Location", "Provider enabled.");
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Log.Debug("Location", "Provider status changed.");
        }

        private async Task<List<Campaign>> GetResponseFromRequest(Request req)
        {
            List<Campaign> camp = new List<Campaign>(); ;
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

        private void ChangeFilter(string newFilter)
        {
            filter = newFilter;
            FilterText.Text = GetString(Resource.String.filter) + filter;
        }

        private void ChangeThreshold(int newThreshold)
        {
            threshold = newThreshold;
            ThresholdText.Text = GetString(Resource.String.threshold) + threshold.ToString();
        }


        private void ThresholdText_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Change threshold...");

            EditText input = new EditText(this);

            input.SetRawInputType(Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberVariationNormal);
            input.TextAlignment = TextAlignment.TextStart;
            builder.SetView(input);

            builder.SetPositiveButton("SET", (s, dialogEvent) => {
                Int32.TryParse(input.Text, out int n);
                ChangeThreshold(n);
            });
            builder.SetNegativeButton("CANCEL", (s, dialogEvent) => {
                (s as Dialog).Dismiss();
            });

            builder.Show();
        }

        private void FilterText_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Change filter...");

            EditText input = new EditText(this);

            input.SetRawInputType(Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationNormal);
            input.TextAlignment = TextAlignment.TextStart;
            builder.SetView(input);

            builder.SetPositiveButton("SET", (s, dialogEvent) => {
                ChangeFilter(input.Text);
            });
            builder.SetNegativeButton("CANCEL", (s, dialogEvent) => {
                (s as Dialog).Dismiss();
            });

            builder.Show();
        }
    }
}