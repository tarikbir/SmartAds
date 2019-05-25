const functions = require('firebase-functions');

const firebase = require('firebase-admin')
const serviceAccount = require('./service_account.json');

firebase.initializeApp({
    credential: firebase.credential.cert(serviceAccount),
    databaseURL: "https://${process.env.GCLOUD_PROJECT}.firebaseio.com"
});

exports.getCampaigns = functions.region('europe-west1').https.onRequest((req, res) => {
    let reqJson = req.body;
    let lat = reqJson.lat;
    let lng = reqJson.lng;
    let threshold = reqJson.threshold;

    let queryCmp = firebase.firestore().collection('companies');

    if (reqJson.filter != 'All') {
        queryCmp = queryCmp.where('CompanyBusiness', '==', reqJson.filter);
    }

    console.log('LOG: Query generated.');
    
    queryCmp.get().then((snapshot) => {
        console.log('LOG: Reached the server...');

        let allCampaigns = [];
        snapshot.forEach((cmp) => {
            let data = cmp.data();
            let loc = data.CompanyLocation;
            let distance = haversine(lat, lng, loc._latitude, loc._longitude);
            if (distance <= threshold) {
                let companyName = data.CompanyName;
                if (data.Campaigns) {
                    data.Campaigns.forEach((campaign) => {
                        let camp = {
                            CampaignName: campaign.Name,
                            CampaignDeadline: campaign.Deadline,
                            CampaignDescription: campaign.Description,
                            CampaignCompanyName: companyName,
                            CampaignDistance: distance
                        }
                        allCampaigns.push(camp);
                    });
                }
            }
        });
        res.send(allCampaigns.sort((a, b) => {
            return parseFloat(a.CampaignDistance) - parseFloat(b.CampaignDistance);
        }));
    }).catch((err) => {
        console.log('LOG: Error while getting data.', err);
    });
});

getCompany = function (id) {
    return firebase.firestore().collection('companies').doc(id).get();
};

haversine = function (lat1, lng1, lat2, lng2) {
    toRad = function (number) {
        return number * Math.PI / 180;
    }

    var R = 6371; // Diameter of earth in km
    var x1 = lat2 - lat1;
    var dLat = toRad(x1);
    var x2 = lng2 - lng1;
    var dLon = toRad(x2);
    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
        Math.cos(toRad(lat1)) * Math.cos(toRad(lat2)) *
        Math.sin(dLon / 2) * Math.sin(dLon / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c;

    return d;
}
