using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SmartAds
{
    public class CampaignListAdapter : BaseAdapter<Campaign>, IListAdapter
    {
        Context context;
        List<Campaign> campaigns;

        public CampaignListAdapter(Context context, List<Campaign> campaigns)
        {
            this.context = context;
            this.campaigns = campaigns;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            CampaignListAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as CampaignListAdapterViewHolder;

            if (holder == null)
            {
                holder = new CampaignListAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                view = inflater.Inflate(Resource.Layout.listview_single, parent, false);
                holder.CampaignName = view.FindViewById<TextView>(Resource.Id.campaign_name); 
                holder.CampaignDescription = view.FindViewById<TextView>(Resource.Id.campaign_description);
                holder.Distance = view.FindViewById<TextView>(Resource.Id.campaign_distance);
                holder.Duration = view.FindViewById<TextView>(Resource.Id.campaign_duration);
                view.Tag = holder;
            }

            //fill in your items
            holder.CampaignName.Text = campaigns[position].CampaignName + " (" + campaigns[position].CampaignCompanyName + ")";
            holder.CampaignDescription.Text = campaigns[position].CampaignDescription;
            DateTime dt = campaigns[position].CampaignDeadlineDT;
            double remainingDays = dt.Subtract(DateTime.Today).TotalDays;
            holder.Duration.Text = (remainingDays > 1) ? 
                                        $"{remainingDays:F0} days left" : 
                                        (remainingDays <= 1 && remainingDays >= 0) ?
                                            "Less than a day left" :
                                            "Expired";
            double distance = campaigns[position].CampaignDistance;
            holder.Distance.Text = (distance > 1) ? 
                                        $"{distance:F1} km" :
                                        "Nearby";
            return view;
        }

        public override int Count
        {
            get
            {
                return campaigns.Count;
            }
        }

        public override Campaign this[int position]
        {
            get { return campaigns[position]; }
        }
    }

    class CampaignListAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        public TextView CampaignName { get; set; }
        public TextView CampaignDescription { get; set; }
        public TextView Distance { get; set; }
        public TextView Duration { get; set; }
    }
}