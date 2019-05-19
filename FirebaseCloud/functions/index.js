// The Cloud Functions for Firebase SDK to create Cloud Functions and setup triggers.
const functions = require('firebase-functions');

// The Firebase Admin SDK to access the Firebase Realtime Database.
const admin = require('firebase-admin');
admin.initializeApp();

exports.getLocation = functions.https.onRequest((req, res) => {
    res.send("Hello!");
    let sa = JSON.parse(req.body);
    sa.CompanyLocation._latitude;
});