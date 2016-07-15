using UnityEngine;
using System.Collections;
using System;

public enum GameEventType
{
    X2XP = 1,
}

public class GameEventBase : MonoBehaviour
{
    public GameEventType EventType;

    public string Title;

    public string SoonNotificationDesc;
    public string StartedNotificationDesc;

    public string EventDescription;

    int _soonNotificationID;
    int _startedNotificationID;

    public int EventStartHour;
    public int EventStartMin;
    public int EventDurationInMins;

    DateTime _eventStartTime;
    DateTime _eventEndTime;

    double _remTimeToStart;
    double _remTimeToFinish;

    bool _isActive;

    public bool IsActive { get { return _isActive; } }

    IEnumerator _updateRemTimeRoutine;

    public void InitGameEvent(int soonNotificationID, int startedNotificationID)
    {
        _soonNotificationID = soonNotificationID;
        _startedNotificationID = startedNotificationID;

        #if UNITY_ANDROID && !UNITY_EDITOR
        ClearEventNotification();
        #endif

        SetEventStartAndEndTime();
        CheckIfEventStarted();
    }

    void SetEventStartAndEndTime()
    {
        DateTime dateNow = DateTime.Now;

        _eventStartTime = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, EventStartHour, EventStartMin, 0);

        _eventEndTime = _eventStartTime.AddMinutes(EventDurationInMins);
    }

    void CheckIfEventStarted()
    {
        DateTime dateNow = DateTime.Now;

        if (dateNow > _eventStartTime)
        {
            if (dateNow < _eventEndTime)
                StartEvent();
            else
                EndEvent();
        }
        else
        {
            if (_updateRemTimeRoutine != null)
                StopCoroutine(_updateRemTimeRoutine);

            _updateRemTimeRoutine = UpdateRemTime(false);
            StartCoroutine(_updateRemTimeRoutine);
        }
    }

    IEnumerator UpdateRemTime(bool isActive)
    {
        while (true)
        {
            if (isActive)
            {
                SetRemTimeToFinish();

                if (_remTimeToFinish <= 0)
                {
                    EndEvent();
                    yield break;
                }
            }
            else
            {
                SetRemTimeToStart();

                if (_remTimeToStart <= 0)
                {
                    StartEvent();
                    yield break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }


    void SetEventActive(bool isActive)
    {
        _isActive = isActive;
    }

    protected virtual void StartEvent()
    {
        SetEventActive(true);

        if (_updateRemTimeRoutine != null)
            StopCoroutine(_updateRemTimeRoutine);

        _updateRemTimeRoutine = UpdateRemTime(true);
        StartCoroutine(_updateRemTimeRoutine);
    }

    protected virtual void EndEvent()
    {
        SetEventActive(false);

        SetNextEventTime();

        if (_updateRemTimeRoutine != null)
            StopCoroutine(_updateRemTimeRoutine);

        _updateRemTimeRoutine = UpdateRemTime(false);
        StartCoroutine(_updateRemTimeRoutine);
    }

    void SetNextEventTime()
    {
        _eventStartTime = _eventStartTime.AddDays(1);
        _eventEndTime = _eventEndTime.AddMinutes(EventDurationInMins);

        //Debug.Log("next event time: " + _eventStartTime);
    }

    void SetRemTimeToStart()
    {
        _remTimeToStart = _eventStartTime.Subtract(DateTime.Now).TotalSeconds;

        //Debug.Log("rem time to start: " + _remTimeToStart);

    }

    void SetRemTimeToFinish()
    {
        _remTimeToFinish = _eventEndTime.Subtract(DateTime.Now).TotalSeconds;

        //Debug.Log("rem time to finish: " + _remTimeToFinish);

    }

    void OnApplicationPause()
    {
        OnApplicationQuit();
    }

    void OnApplicationQuit()
    {
        SetEventNotifications();
    }

    void ClearEventNotification()
    {
        GameEventManager.Instance.ClearEventNotification(_soonNotificationID);
        GameEventManager.Instance.ClearEventNotification(_startedNotificationID);
    }

    void SetEventNotifications()
    {
        DateTime nextEventStartTime;
        DateTime nextEventSoonTime;

        nextEventStartTime = _eventStartTime;
        nextEventSoonTime = nextEventStartTime.AddMinutes(-GameEventManager.Instance.ReminderNotificationInMins);

        if (nextEventStartTime < DateTime.Now)
            nextEventStartTime = nextEventStartTime.AddDays(1);

        if (nextEventSoonTime < DateTime.Now)
            nextEventSoonTime = nextEventSoonTime.AddDays(1);

        TimeSpan delay;

        delay = (nextEventSoonTime - DateTime.Now);
        SetEventNotification(_soonNotificationID, SoonNotificationDesc, delay);

        delay = (nextEventStartTime - DateTime.Now);
        SetEventNotification(_startedNotificationID, StartedNotificationDesc, delay);
    }

    void SetEventNotification(int id, string desc, TimeSpan delay)
    {
        GameEventManager.Instance.ClearEventNotification(id);

        #if UNITY_ANDROID && !UNITY_EDITOR
        GameEventManager.Instance.SendEventNotification(id, Title, desc, delay, (new TimeSpan(1, 0, 0, 0)));
        #endif
    }

    public double GetRemainingTimeToFinish()
    {
        return _remTimeToFinish;
    }

    public double GetRemainingTimeToStart()
    {
        return _remTimeToStart;
    }
}
