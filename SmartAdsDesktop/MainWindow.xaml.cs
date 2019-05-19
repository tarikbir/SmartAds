using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Google.Cloud.Firestore;

namespace SmartAdsDesktop
{
    public partial class MainWindow : Window
    {
        FirestoreDb fs;

        public MainWindow()
        {
            InitializeComponent();
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "SmartAdsXamarin-251a2ecd6354.json");

            fs = FirestoreDb.Create("smartadsxamarin");
        }

        private async void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            btnSend.IsEnabled = false;
            WriteResult result;
            Google.Type.LatLng location = new Google.Type.LatLng();
            try
            {
                location.Latitude = Double.Parse(txtCompanyLatitude.Text);
                location.Longitude = Double.Parse(txtCompanyLongitude.Text);
                if (location.Latitude > 90 || location.Latitude < -90) throw new FormatException();
                if (location.Longitude > 180 || location.Longitude < -180) throw new FormatException();
                result = await AddCompany(txtCompanyName.Text, (cmbCompanyBusiness.SelectedItem as ComboBoxItem).Content.ToString(), location);
            }
            catch (FormatException)
            {
                MessageBox.Show("Error while parsing location information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch
            {
                MessageBox.Show("Unknown error occured.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally
            {
                btnSend.IsEnabled = true;
            }

            if (result != null)
            {
                MessageBox.Show("Company sent to the server at " + result.UpdateTime.ToDateTime().ToLocalTime() + ".", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void BtnSendCampaign_Click(object sender, RoutedEventArgs e)
        {
            btnSendCampaign.IsEnabled = false;
            WriteResult result;
            try
            {
                result = await AddCampaign(txtCampaignName.Text, txtCampaignDescription.Text, dpCampaignDeadline.SelectedDate.ToString(), (cmbCampaignCompany.SelectedItem as Company).ID);
            }
            catch
            {
                MessageBox.Show("Unknown error occured.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally
            {
                btnSendCampaign.IsEnabled = true;
            }

            if (result != null)
            {
                MessageBox.Show("Campaign sent to the server at " + result.UpdateTime.ToDateTime().ToLocalTime() + ".", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task<WriteResult> AddCompany(string companyName, string companyBusiness, Google.Type.LatLng companyLocation)
        {
            Google.Cloud.Firestore.DocumentReference docRef = fs.Collection("companies").Document();
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "CompanyName", companyName },
                { "CompanyBusiness", companyBusiness },
                { "CompanyLocation", companyLocation }
            };
            return await docRef.SetAsync(data);
        }

        private async Task<WriteResult> AddCampaign(string campaignName, string campaignDescription, string campaignDeadline, string companyID)
        {
            Google.Cloud.Firestore.DocumentReference docRef = fs.Collection("campaigns").Document();
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "CampaignName", campaignName },
                { "CampaignDescription", campaignDescription },
                { "CampaignDeadline", campaignDeadline },
                { "companyID", companyID }
            };
            return await docRef.SetAsync(data);
        }

        private void TxtCompanyLatitude_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex(@"[^0-9\-\.]+").IsMatch(e.Text);
        }

        private void TxtCompanyLongitude_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex(@"[^0-9\-\.]+").IsMatch(e.Text);
        }

        private async Task<List<Company>> ReadCompanies()
        {
            List<Company> companyList = new List<Company>();
            try
            {
                CollectionReference usersRef = fs.Collection("companies");
                QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    Company cmp = new Company();
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    cmp.CompanyName = documentDictionary["CompanyName"].ToString();
                    cmp.ID = document.Id;
                    companyList.Add(cmp);
                }
            }
            catch { return null; }
            return companyList;
        }

        private async void TabCampaign_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!cmbCampaignCompany.HasItems)
            {
                List<Company> companyList = await ReadCompanies();

                cmbCampaignCompany.ItemsSource = companyList;
            }
        }

        private void TabCompany_GotFocus(object sender, RoutedEventArgs e)
        {
            if (cmbCampaignCompany.HasItems)
            {
                cmbCampaignCompany.ItemsSource = null;
            }
        }
    }

    class Company
    {
        public string ID { get; set; }
        public string CompanyName { get; set; }

        public override string ToString()
        {
            return CompanyName;
        }
    }
}
