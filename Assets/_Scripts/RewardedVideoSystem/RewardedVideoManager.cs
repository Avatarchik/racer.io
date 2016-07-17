using UnityEngine;
using System;
using System.Collections;

public class RewardedVideoManager : MonoBehaviour
{
    static RewardedVideoManager _instance;

    public static RewardedVideoManager Instance{ get { return _instance; } }

    public int WatchVideoIntervalInSecs;

    public int WatchVideoRewardDurationInSecs;

    const string LAST_WATCHED_VIDEO_TIME = "lastWatchedVideoTime";

    DateTime _lastWatchedTime;

    bool _isRewardActive;

    IEnumerator _updateRewardRoutine;

    DateTime _rewardEndTime;
    double _remTimeToFinish;

    void Awake()
    {
        _instance = this;

        LoadLastWatchedTime();
        CheckIfRewardActive();
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public bool CheckCanWatchVideo()
    {
        if ((DateTime.Now - _lastWatchedTime).TotalSeconds > WatchVideoIntervalInSecs)
            return true;

        return false;
    }

    void LoadLastWatchedTime()
    {        
        if (!PlayerPrefs.HasKey(LAST_WATCHED_VIDEO_TIME))
        {
            _lastWatchedTime = DateTime.Now;
            _lastWatchedTime = _lastWatchedTime.AddDays(-1);
            SaveWatchVideoTime();
        }

        Debug.Log("last watch time: " + PlayerPrefs.GetString(LAST_WATCHED_VIDEO_TIME));

        _lastWatchedTime = DateTime.Parse(PlayerPrefs.GetString(LAST_WATCHED_VIDEO_TIME));

        Debug.Log("last watch time: " + _lastWatchedTime);
    }

    void CheckIfRewardActive()
    {
        if ((DateTime.Now - _lastWatchedTime).TotalSeconds < WatchVideoRewardDurationInSecs)
            ActivateReward();
    }

    public void SaveWatchVideoTime()
    {
        PlayerPrefs.SetString(LAST_WATCHED_VIDEO_TIME, _lastWatchedTime.ToString());
    }

    void OnApplicationPause()
    {
        OnApplicationQuit();
    }

    void OnApplicationQuit()
    {
        SaveWatchVideoTime();
    }

    public void WatchVideoPressed()
    {
        HeyzapAdManager.OnIncentivizedFinished += OnIncentivizedFinished;
        Debug.Log("incentivized listening");
        HeyzapAdManager.Instance.ShowRewardedAd();
    }

    void OnIncentivizedFinished(bool isFinished)
    {
        Debug.Log("adui onincentivizedfinished " + isFinished);
        HeyzapAdManager.OnIncentivizedFinished -= OnIncentivizedFinished;

        if (isFinished)
        {
            _lastWatchedTime = DateTime.Now;

            SaveWatchVideoTime();

            RewardedAdUI.Instance.SetCanWatch();

            ActivateReward();
        }
    }

    void ActivateReward()
    {
        _isRewardActive = true;

        if (_updateRewardRoutine != null)
            StopCoroutine(_updateRewardRoutine);

        _updateRewardRoutine = UpdateRewardProgress();
        StartCoroutine(_updateRewardRoutine);
    }

    void DeactivateReward()
    {
        _isRewardActive = false;

    }

    IEnumerator UpdateRewardProgress()
    {
        _rewardEndTime = _lastWatchedTime.AddSeconds(WatchVideoRewardDurationInSecs);
        Debug.Log("reward end time: " + _rewardEndTime);
        while (true)
        {
            SetRemTimeToFinish();

            if (_remTimeToFinish <= 0)
            {
                DeactivateReward();
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    void SetRemTimeToFinish()
    {
        _remTimeToFinish = _rewardEndTime.Subtract(DateTime.Now).TotalSeconds;

    }

    public double GetRemainingRewardDurationInSecs()
    {
        SetRemTimeToFinish();

        return _remTimeToFinish;
    }
}
