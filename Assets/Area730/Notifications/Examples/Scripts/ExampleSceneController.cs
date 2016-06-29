using UnityEngine;
using UnityEngine.UI;
using System;
using Area730.Notifications;

namespace Area730.Examples
{
    public class ExampleSceneController : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField]
        private Text        versionLabel;

        [SerializeField]
        private InputField  titlefield;

        [SerializeField]
        private InputField  bodyField;

        [SerializeField]
        private InputField  tickerField;

        [SerializeField]
        private InputField  delayField;

        [SerializeField]
        private InputField  idField;

        [SerializeField]
        private InputField  intervalField;

        [SerializeField]
        private Toggle      alertOnce;

        [SerializeField]
        private Toggle      sound;

        [SerializeField]
        private Toggle      vibro;

        [SerializeField]
        private Toggle      autoCancel;

        [SerializeField]
        private Toggle      repeating;

        [SerializeField]
        private InputField  colorField;


        #endregion

        #region Vars

        private string  title;
        private string  body;
        private string  ticker;
        private int     delay;
        private int     id;
        private int     interval;
        private int     flags;
        private string  color;


        #endregion


        void Start()
        {
            versionLabel.text = "Version: " + AndroidNotifications.getVersion().ToString();

            
        }

        
        public void scheduleAction()
        {
            updateValues();

            NotificationBuilder builder = new NotificationBuilder(id, title, body);
            builder
                .setTicker          (ticker)
                .setDefaults        (flags)
                .setAlertOnlyOnce   (alertOnce.isOn)
                .setDelay           (delay * 1000)
                .setRepeating       (repeating.isOn)
                .setAutoCancel      (autoCancel.isOn)
                .setColor           (color)
                .setInterval        (interval * 1000);


            if (repeating.isOn && interval == 0)
            {
                AndroidNotifications.showToast("Enter interval");
            } else
            {
                Debug.Log(builder.build());
                AndroidNotifications.scheduleNotification(builder.build());

                AndroidNotifications.showToast("Notification scheduled");
            }

           
        }

        public void ScheduleFromList()
        {
            string notificationName = "FirstNotif";
            NotificationBuilder builder = AndroidNotifications.GetNotificationBuilderByName(notificationName);

            if (builder != null)
            {
                Notification notif = builder.build();
                AndroidNotifications.scheduleNotification(notif);

                Debug.Log(notif);
            } else
            {
                Debug.LogError("Notification with name " + notificationName + " wasn't found");
            }
            
        }

        /// <summary>
        /// Cancels notification with current id
        /// </summary>
        public void cancelAction()
        {
            updateValues();

            AndroidNotifications.cancelNotification(id);

            AndroidNotifications.showToast("Notification cancelled (" + id + ")");
        }

        /// <summary>
        /// Clear current notification button id
        /// </summary>
        public void clearCurrent()
        {
            AndroidNotifications.clear(id);
            AndroidNotifications.showToast("Cleared id " + id);
        }

        /// <summary>
        /// Clear all notifications buttin action
        /// </summary>
        public void clearAll()
        {
            AndroidNotifications.clearAll();
            AndroidNotifications.showToast("All cleared");
        }


        /// <summary>
        /// Retreive values from ui fields. If they are empty - set some defaluts
        /// </summary>
        private void updateValues()
        {
            if (!String.IsNullOrEmpty(titlefield.text))
            {
                title = titlefield.text;
            }
            else
            {
                title = "New notification!";
            }

            if (!String.IsNullOrEmpty(bodyField.text))
            {
                body = bodyField.text;
            }
            else
            {
                body = "Looks like its working!";
            }


            ticker  = tickerField.text;
            color   = colorField.text;
          
            if (!String.IsNullOrEmpty(delayField.text))
            {
                delay = Convert.ToInt32(delayField.text);
            }
            else
            {
                delay = 5;
            }

            if (!String.IsNullOrEmpty(idField.text))
            {
                id = Convert.ToInt32(idField.text);
            }
            else
            {
                id = 1;
            }

            if (!String.IsNullOrEmpty(intervalField.text))
            {
                interval = Convert.ToInt32(intervalField.text);
            } else
            {
                interval = 0;
            }

            flags = 0;
            if (sound.isOn)
            {
                flags |= NotificationBuilder.DEFAULT_SOUND;
            }

            if (vibro.isOn)
            {
                flags |= NotificationBuilder.DEFAULT_VIBRATE;
            }
        }


    }
}