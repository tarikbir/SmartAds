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
        private double latestLat;
        private double latestLng;
        private static Uri endpoint = new Uri("https://smartadsxamarin.firebaseapp.com/getCampaigns");
        private HttpClient httpClient;
        private bool locationCheck = true;
        private ListView listView;
        private TextView FilterText;
        private string filter;
        private TextView ThresholdText;
        private int threshold;
        private TextView GpsBrokenText;
        private bool isGpsBrokenTextClicked;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ShowToast(this, "Successfully logged in.", ToastLength.Short);
            httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(7) };
            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation, Manifest.Permission.Internet }, 10);
            SetContentView(2130968602);
            InitializeContent();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Android.Content.PM.Permission.Granted) InitializeLocationManager();
        }

        private void InitializeContent()
        {
            FilterText = FindViewById<TextView>(Resource.Id.text_filter);
            ThresholdText = FindViewById<TextView>(Resource.Id.text_threshold);
            listView = FindViewById<ListView>(Resource.Id.campaign_list);
            GpsBrokenText = FindViewById<TextView>(Resource.Id.text_brokengps);
            isGpsBrokenTextClicked = false;
            filter = "All";
            FilterText.Text = GetString(Resource.String.filter) + filter;
            FilterText.Click += FilterText_Click;
            threshold = 80;
            ThresholdText.Text = GetString(Resource.String.threshold) + threshold.ToString();
            ThresholdText.Click += ThresholdText_Click;
            GpsBrokenText.Click += GpsBrokenText_Click;
            latestLat = 0;
            latestLng = 0;
        }

        private void InitializeLocationManager()
        {
            LocationManager = (LocationManager)GetSystemService(LocationService);
            LocationManager.RequestLocationUpdates(LocationManager.GpsProvider, 0, 0, this);
        }

        public void OnLocationChanged(Location location)
        {
            LocationChangeCallback(location.Latitude, location.Longitude);
        }

        private async void LocationChangeCallback(double lat, double lng)
        {
            if (locationCheck)
            {
                locationCheck = false;
                latestLat = lat;
                latestLng = lng;
                List<Campaign> camp = await GetResponseFromRequest(new Request() { filter = filter, threshold = threshold, lat = lat, lng = lng });
                Log.Debug("OnLocationChanged", "Got campaigns response.");
                if (camp.Count > 0)
                {
                    CampaignListAdapter arrayAdapter = new CampaignListAdapter(this, camp);
                    listView.Adapter = arrayAdapter;
                }
                else
                {
                    listView.Adapter = null;
                }
                locationCheck = true;
            }
        }

        private void LocationChangeCallback()
        {
            Location l = LocationManager.GetLastKnownLocation(LocationManager.GpsProvider);
            LocationChangeCallback(l.Latitude, l.Longitude);
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
            if (LocationManager != null) LocationChangeCallback();
            else LocationChangeCallback(latestLat, latestLng);
        }

        private void ChangeThreshold(int newThreshold)
        {
            threshold = newThreshold;
            ThresholdText.Text = GetString(Resource.String.threshold) + threshold.ToString();
            if (LocationManager != null) LocationChangeCallback();
            else LocationChangeCallback(latestLat, latestLng);
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

        private void GpsBrokenText_Click(object sender, EventArgs e)
        {
            if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Android.Content.PM.Permission.Granted)
            {
                if (!isGpsBrokenTextClicked)
                {
                    ShowToast(this, "Location services are disabled.", ToastLength.Short);
                    LocationManager.RemoveUpdates(this);
                    LocationManager.Dispose();
                    LocationManager = null;
                    isGpsBrokenTextClicked = true;
                    GpsBrokenText.Text = "Click here to re-enable location services.";
                }
                else
                {
                    ShowToast(this, "Location services are enabled.", ToastLength.Short);
                    InitializeLocationManager();
                    isGpsBrokenTextClicked = false;
                    GpsBrokenText.Text = GetString(Resource.String.brokengps);
                    return;
                }
            }
            else
            {
                GpsBrokenText.Text = "Click here to search again!";
            }
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Manual search");

            LinearLayout layout = new LinearLayout(this);
            layout.Orientation = Orientation.Vertical;

            EditText inputLat = new EditText(this);
            EditText inputLng = new EditText(this);

            layout.AddView(inputLat);
            layout.AddView(inputLng);

            inputLat.SetRawInputType(Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagDecimal);
            inputLng.SetRawInputType(Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagDecimal);
            inputLat.TextAlignment = TextAlignment.TextStart;
            inputLng.TextAlignment = TextAlignment.TextStart;
            builder.SetView(layout);

            builder.SetPositiveButton("SET", (s, dialogEvent) => {
                if (Double.TryParse(inputLat.Text, out double lat)) (s as Dialog).Dismiss();
                if (Double.TryParse(inputLng.Text, out double lng)) (s as Dialog).Dismiss();
                LocationChangeCallback(lat, lng);
            });
            builder.SetNegativeButton("CANCEL", (s, dialogEvent) => {
                (s as Dialog).Dismiss();
            });

            builder.Show();
        }
    }
}