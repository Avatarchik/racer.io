using System.Collections.Generic;
using UnityEngine;

namespace Area730.Notifications
{

    /// <summary>
    /// Class that holds data set up in Notifications editor window
    /// </summary>
    [System.Serializable]
    public class DataHolder : ScriptableObject
    {

        public List<NotificationInstance> notifications = new List<NotificationInstance>();
        public string unityClass                        = "com.unity3d.player.UnityPlayerNativeActivity";

        /// <summary>
        /// For debug purposes
        /// </summary>
        public void print()
        {
            foreach (var item in notifications)
            {
                item.Print();
            }
        }

    }

}


