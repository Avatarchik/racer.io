using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Area730.Notifications;
using System;


public class GameEventManager : MonoBehaviour
{
    static GameEventManager _instance;

    public static GameEventManager Instance { get { return _instance; } }

    public List<GameEventBase> AllEventList;

    GameEventBase _curSelectedEvent;

    public int ReminderNotificationInMins;

    const string LARGE_ICON_NAME = "notification_big_icon";
    const string SMALL_ICON_NAME = "notification_small_icon";


    void Awake()
    {
        _instance = this;

        InitGameEvents();
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void InitGameEvents()
    {
        int notificationID = 1;

        foreach (GameEventBase gameEvent in AllEventList)
        {
            gameEvent.InitGameEvent(notificationID, notificationID + 1);

            notificationID += 2;
        }

        SetSelectedEvent(AllEventList[0]);
    }

    public void ClearEventNotification(int notificationID)
    {
        AndroidNotifications.cancelNotification(notificationID);
    }

    public void SendEventNotification(int id, string title, string message, TimeSpan delay, TimeSpan repeatingInterval)
    {
        NotificationBuilder nb = new NotificationBuilder(id, title, message)
            .setDelay(delay)
            .setRepeating(true)
            .setInterval(repeatingInterval)
            .setLargeIcon(LARGE_ICON_NAME)
            .setSmallIcon(SMALL_ICON_NAME);
        
        AndroidNotifications.scheduleNotification(nb.build());
    }

    void SetSelectedEvent(GameEventBase eventBase)
    {
        _curSelectedEvent = eventBase;
    }

    public bool HasActiveEvent()
    {
        return _curSelectedEvent.IsActive;
    }

    public double GetActiveEventRemainingTimeToFinish()
    {
        return _curSelectedEvent.GetRemainingTimeToFinish();
    }

    public double GetActiveEventRemainingTimeToStart()
    {
        return _curSelectedEvent.GetRemainingTimeToStart();
    }
}
