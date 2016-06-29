using System.Collections.Generic;
using UnityEngine;


namespace Area730.Notifications
{

    /// <summary>
    /// This is the internal representation of notification used for serializing notifications
    /// from editor window
    /// </summary>
    [System.Serializable]
    public class NotificationInstance
    {
        public string name;
        public Texture2D smallIcon;
        public Texture2D largeIcon;
        public string title;
        public string body;
        public string ticker;
        public bool autoCancel;
        public bool isRepeating;
        public int intervalHours;
        public int intervalMinutes;
        public int intervalSeconds;
        public bool alertOnce;
        public int number;
        public int delayHours;
        public int delayMinutes;
        public int delaySeconds;
        public bool defaultSound;
        public bool defaultVibrate;
        public AudioClip soundFile;
        public List<long> vibroPattern = new List<long>();
        public string group;
        public string sortKey;
        public bool hasColor;
        public Color color;
        public int id;

        /// <summary>
        /// For debug purposes
        /// </summary>
        public void Print()
        {
            string res = "Notification: ";
            res += "title: " + title;
            res += ", body: " + body;
            res += ", ticker: " + ticker;
            res += ", autoCancel: " + autoCancel;
            res += ", isRepeating: " + isRepeating;
            res += ", intervalHours: " + intervalHours;
            res += ", intervalMinutes: " + intervalMinutes;
            res += ", intervalSeconds: " + intervalSeconds;
            res += ", alertOnce: " + alertOnce;
            res += ", number: " + number;
            res += ", delayHours: " + delayHours;
            res += ", delayMinutes: " + delayMinutes;
            res += ", delaySeconds: " + delaySeconds;
            res += ", defaultSound: " + defaultSound;
            res += ", defaultVibrate: " + defaultVibrate;
            res += ", group: " + group;
            res += ", sortKey: " + sortKey;
            res += ", hasColor: " + hasColor;
            res += ", color: " + ColorUtils.ToHtmlStringRGB(color);
            res += ", body: " + body;
            Debug.Log(res);
        }

    }

}


