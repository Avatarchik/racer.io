using UnityEngine;

namespace Area730.Notifications
{

    /// <summary>
    /// Class that handles requests to android java lib
    /// </summary>
    public class AndroidNotifications
    {

        //public static string UNITY_CLASS = "com.unity3d.player.UnityPlayerNativeActivity";

        /// <summary>
        /// Instance of java notification handler class
        /// </summary>
#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaClass notifHandlerClass;
#endif
        /// <summary>
        /// Holds serialized notifications created in editor window (Window -> Android local notifications)
        /// </summary>
        private static DataHolder _dataHolder;

        /// <summary>
        /// Default constructor
        /// </summary>
        static AndroidNotifications()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // Find the java class that handles notifications
            notifHandlerClass = new AndroidJavaClass("com.area730.localnotif.NotificationHandler");
            if (notifHandlerClass == null)
            {
                Debug.LogError("Class com.area730.localnotif.NotificationHandler not found");
                return;
            }

            Debug.Log("Android notifications plugin loaded. Version: " + getVersion());
#else
            Debug.LogWarning("Android notifications can work only on android devices");
#endif

            _dataHolder = (DataHolder)Resources.Load("NotificationData");
            
        }

        /// <summary>
        /// Returns builder for notification created in editor by index(position in list in editor window)
        /// </summary>
        /// <param name="pos">index of notification in the editor list</param>
        /// <returns>Notification builder</returns>
        public static NotificationBuilder GetNotificationBuilderByIndex(int pos)
        {
            NotificationInstance item   = _dataHolder.notifications[pos];
            NotificationBuilder builder = NotificationBuilder.FromInstance(item);

            return builder;
        }

        /// <summary>
        /// Returns builder for notification created in editor window by name
        /// </summary>
        /// <param name="name">Name of the notification to get</param>
        /// <returns>Notification builder or null if the notification is not found</returns>
        public static NotificationBuilder GetNotificationBuilderByName(string name)
        {
            foreach(NotificationInstance item in _dataHolder.notifications)
            {
                if (item.name.Equals(name))
                {
                    NotificationBuilder builder = NotificationBuilder.FromInstance(item);
                    return builder;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the version of the plugin
        /// </summary>
        /// <returns>Vertion of the plugin</returns>
        public static float getVersion()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            float version = notifHandlerClass.CallStatic<float>("getVersion");
            return version;
#else
            return -1;            
#endif
        }

        /// <summary>
        /// Schedule the notification
        /// </summary>
        /// <param name="notif">Notification to be scheduled</param>
        public static void scheduleNotification(Notification notif)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            NotificationTracker.TrackId(notif.ID);

            notifHandlerClass.CallStatic("scheduleNotification",
                    notif.Delay,
                    notif.ID,
                    notif.Title,
                    notif.Body,
                    notif.Ticker,
                    notif.SmallIcon,
                    notif.LargeIcon,
                    notif.Defaults,
                    notif.AutoCancel,
                    notif.Sound,
                    notif.VibratePattern,
                    notif.When,
                    notif.IsRepeating,
                    notif.Interval,
                    notif.Number,
                    notif.AlertOnce,
                    notif.Color,
                    _dataHolder.unityClass,
                    notif.Group,
                    notif.SortKey);
#endif

        }


        /// <summary>
        /// Cancels the notification. Both repeating and non-repeating.
        /// </summary>
        /// <param name="notif">Notification to be cancelled</param>
        public static void cancelNotification(Notification notif)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            cancelNotification(notif.ID);
#endif
        }

        /// <summary>
        /// Cancel all tracked notifications. 
        /// By defauld all notifications scheduled with AndroidNotifications.scheduleNotification() are tracked
        /// </summary>
        public static void cancelAll()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            NotificationTracker.CancelAll();
#endif
        }

        /// <summary>
        /// Cancels the notification. Both repeating and non-repeating.
        /// </summary>
        /// <param name="id">Id of the notification to be scheduled</param>
        public static void cancelNotification(int id)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            notifHandlerClass.CallStatic("cancelNotifications", id);
#endif
        }

        /// <summary>
        /// Clear shown notification with specified if
        /// </summary>
        /// <param name="id">Id of the notification to be cleated</param>
        public static void clear(int id)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            notifHandlerClass.CallStatic("clear", id);
#endif
        }

        /// <summary>
        /// Cleared all shown notifications
        /// </summary>
        public static void clearAll()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            notifHandlerClass.CallStatic("clearAll");
#endif
        }

        /// <summary>
        /// Shows native android toast notification
        /// </summary>
        /// <param name="text">Text of the toast</param>
        public static void showToast(string text)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            notifHandlerClass.CallStatic("showToast", text);
#endif
        }

    }
}
