using System;
using System.Collections.Generic;

namespace SmartAds
{
    [Serializable]
    public class Campaign
    {
        public string CampaignName { get; set; }
        public Dictionary<string, long> CampaignDeadline
        {
            set
            {
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                long l = value["_seconds"];
                dt = dt.AddSeconds(l);
                CampaignDeadlineDT = dt;
            }
        }
        public DateTime CampaignDeadlineDT { get; set; }
        public string CampaignDescription { get; set; }
        public string CampaignCompanyName { get; set; }
        public double CampaignDistance { get; set; }

        public override string ToString()
        {
            return CampaignName + CampaignDescription + CampaignDistance;
        }

        /* Example rest json response:
            {
                "CampaignName": "Yarı yarıya kumpir!",
                "CampaignDeadline": {
                    "_seconds": 1561842000,
                    "_nanoseconds": 0
                },
                "CampaignDescription": "Kumpirde inanılmaz indirim! Bir kumpir alana öteki %50 indirimli!",
                "CampaignCompanyName": "Aşiyan Tesisleri",
                "CampaignDistance": 15.387466050325067
            }
         */
    }
}