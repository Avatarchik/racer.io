using UnityEngine;

namespace Area730.Notifications
{
    /// <summary>
    /// Simple tracker of the scheduled notifications so you can cancel them all at once
    /// without need to track their IDs by yourself
    /// </summary>
    public class NotificationTracker 
    {

        private const string IDS_KEY = "area730_notification_ids";

        /// <summary>
        /// Track the id of the scheduled notification. Tracked IDs can be cancelled with NotificationTracker.CancelAll() method
        /// </summary>
        /// <param name="id">Id of the notification to track</param>
        public static void TrackId(int id)
        {
            string keys = PlayerPrefs.GetString(IDS_KEY);
            keys += id + ";";
            PlayerPrefs.SetString(IDS_KEY, keys);
        }
        /// <summary>
        /// Cancels all schedules notifications tracked with NotificationTracker.TrackId(int id) method. By default all notifications are tracked
        /// </summary>
        public static void CancelAll()
        {
            string keys = PlayerPrefs.GetString(IDS_KEY);
            string[] ids = keys.Split(';');

            for (int i = 0; i < ids.Length - 1; ++i)
            {
                int id = System.Convert.ToInt32(ids[i]);
                AndroidNotifications.cancelNotification(id);
            }

            PlayerPrefs.DeleteKey(IDS_KEY);
        }


      
    }

}

