using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIDailyEvent : MonoBehaviour
{
    public Text DescriptionText, StatusText, RemTimeText;

    static UIDailyEvent _instance;

    public static UIDailyEvent Instance { get { return _instance; } }

    IEnumerator _updateRoutine;
    
    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;

        if (_updateRoutine != null)
            StopCoroutine(_updateRoutine);
    }

    void OnEnable()
    {
        if (_updateRoutine != null)
            StopCoroutine(_updateRoutine);

        _updateRoutine = UpdateDailyEventUI();
        StartCoroutine(_updateRoutine);
    }

    IEnumerator UpdateDailyEventUI()
    {
        while(true)
        {
            double remTime = 0;

            if (GameEventManager.Instance.HasActiveEvent())
            {
                remTime = GameEventManager.Instance.GetActiveEventRemainingTimeToFinish();
                StatusText.text = "Time left to finish";
            }
            else
            {
                remTime = GameEventManager.Instance.GetActiveEventRemainingTimeToStart();
                StatusText.text = "Time left to start";
            }

            TimeSpan span = TimeSpan.FromSeconds(remTime);

            RemTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", (int)span.Hours, (int)span.Minutes, span.Seconds);

            yield return new WaitForSeconds(1f);
        }
    }
}
