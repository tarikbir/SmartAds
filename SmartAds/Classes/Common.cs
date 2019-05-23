using Android.Content;
using Android.Widget;

namespace SmartAds
{
    public static class Common
    {
        public static void ShowToast(Context context, string text, ToastLength length)
        {
            Toast toast = Toast.MakeText(context, text, length);
            toast.Show();
        }
    }
}