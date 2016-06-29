using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour 
{
    static AnalyticsManager _instance;

    public static AnalyticsManager Instance{get{return _instance;}}

    public GoogleAnalyticsV4 googleAnalytics;

    int _sessionCount;
    float _totalSessionLength;
    float _sessionLengthAvarage;

    float _sessionStartTime;

    void Awake()
    {
        _instance = this;
        googleAnalytics.LogScreen("In Game");
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void SessionStarted()
    {
        _sessionCount++;
        _sessionStartTime = Time.realtimeSinceStartup;
    }

    public void SessionFinished()
    {
        _totalSessionLength += Time.realtimeSinceStartup - _sessionStartTime;
    }

    void OnApplicationQuit()
    {
        SendAnalyticEvent();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SendAnalyticEvent();
        }
    }



    void SendAnalyticEvent()
    {
        SetAvgSessionLength();
        
        Analytics.CustomEvent("SessionInfo", new Dictionary<string, object>
            {
                { "sessionCount", _sessionCount },
                { "sessionAvgLength", _sessionLengthAvarage }
            });
        googleAnalytics.LogTiming("SessionInfo", (long)_sessionLengthAvarage, "In Game", "Average Session Length");
        googleAnalytics.LogTiming("SessionInfo", (long)_sessionCount, "In Game", "Session Count");
    }

    void SetAvgSessionLength()
    {
        _sessionLengthAvarage = _totalSessionLength / (float)_sessionCount;
    }



}
