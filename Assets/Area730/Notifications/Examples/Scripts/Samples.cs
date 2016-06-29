using UnityEngine;
using System;

namespace Area730.Notifications
{

    public class Samples : MonoBehaviour
    {

        /// <summary>
        /// The most simple notification you can schedule
        /// </summary>
        public void SimpleNotificationSample()
        {
            int id          = 1;
            string title    = "Notification titile";
            string body     = "Notification body";

            NotificationBuilder builder = new NotificationBuilder(id, title, body);
            AndroidNotifications.scheduleNotification(builder.build());
        }

        /// <summary>
        /// Notification with some customizations (group, color, ticker and other)
        /// </summary>
        public void CustomizedNotificationSample()
        {
            int id          = 1;
            string title    = "New notification";
            string body     = "You have some unfinished business!";

            // Show notification in one hour
            TimeSpan delay  = new TimeSpan(1, 0, 0); 

            NotificationBuilder builder = new NotificationBuilder(id, title, body);
            builder
                .setTicker("New notification from your app!")
                .setDefaults(NotificationBuilder.DEFAULT_ALL)
                .setAlertOnlyOnce(true)
                .setDelay(delay)
                .setAutoCancel(true)
                .setGroup("Group 1")
                .setColor("#B30000");

            AndroidNotifications.scheduleNotification(builder.build());
        }

        /// <summary>
        /// Repetitive notification
        /// </summary>
        public void RepeatingNotificationSample()
        {
            int id              = 1;
            string title        = "New repeating notification";
            string body         = "You have some unfinished business!";

            // Show notification in 5 minutes
            TimeSpan delay      = new TimeSpan(0, 5, 0);

            // Show notification with 10 minute interval
            TimeSpan interval   = new TimeSpan(0, 10, 0);

            NotificationBuilder builder = new NotificationBuilder(id, title, body);
            builder
                .setDelay(delay)
                .setRepeating(true)
                .setInterval(interval);

            AndroidNotifications.scheduleNotification(builder.build());
        }

        /// <summary>
        /// Notification with custom icons and sounds
        /// </summary>
        public void CustomIconsAndSoundSample()
        {
            int     id      = 1;
            string  title   = "Custom icon and sound";
            string  body    = "You have some unfinished business!";

            // Show notification in 5 minutes
            TimeSpan delay = new TimeSpan(0, 5, 0);

            // WARNING: in order to this sample to work place the icons with the corresponding names res/drawable folder
            // and sound with corresponding name in res/raw folder
            NotificationBuilder builder = new NotificationBuilder(id, title, body);
            builder
                .setDelay(delay)
                .setSmallIcon("mySmallIcon") 
                .setLargeIcon("myLargeIcon")
                .setSound("mySound");

            AndroidNotifications.scheduleNotification(builder.build());
        }

        /// <summary>
        /// Schedule created in editor notification
        /// </summary>
        public void ScheduleCreatedInEditorSample()
        {
            string notificationName = "FirstNotif";

            // Method returns builder so you can config your notification afterwards if you want
            NotificationBuilder builder = AndroidNotifications.GetNotificationBuilderByName(notificationName);
            
            // If notification with specified name doesn't exist builder will be null
            if (builder != null)
            {
                Notification notif = builder.build();
                AndroidNotifications.scheduleNotification(notif);
            }
            else
            {
                Debug.LogError("Notification with name " + notificationName + " wasn't found");
            }
        }

        /// <summary>
        /// Cancell all scheduled notifications
        /// </summary>
        public void CancelAllSample()
        {
            AndroidNotifications.cancelAll();
        }

        /// <summary>
        /// Clear all shown notifications
        /// </summary>
        public void ClearAllSample()
        {
            AndroidNotifications.clearAll();
        }

    }

}
