using UnityEngine;
using System.Collections;
using System.Collections.Generic; 


public class CrossPlatformPushNotificationController : MonoBehaviour {
	
	public string _IOSOneSignalId;		//for ex. c86358cf-8b7c-44bc-800a-fbae799c4f32

	//Android part
	public string _androidOneSignalId; 	//for ex. c86358cf-8b7c-44bc-800a-fbae799c4f32
	public string _gmsAppId;			//for ex. 701666001854

	// Use this for initialization
	void Start () 
	{
		// Enable line below to enable logging if you are having issues setting up OneSignal. (logLevel, visualLogLevel)
		//OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.INFO, OneSignal.LOG_LEVEL.INFO);
#if UNITY_ANDROID && !UNITY_EDITOR
		OneSignal.Init(_androidOneSignalId, _gmsAppId, HandleNotification);

#elif UNITY_IOS && !UNITY_EDITOR
		OneSignal.Init(_IOSOneSignalId, null, HandleNotification);
#endif
	}

	// Gets called when the player opens the notification.
	private static void HandleNotification(string message, Dictionary<string, object> additionalData, bool isActive) {
		Debug.Log("Handle push notification from One Signal");
	}
}
