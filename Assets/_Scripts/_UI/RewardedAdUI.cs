using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class RewardedAdUI : MonoBehaviour
{
    public Transform Button;
    public Image ButtonImage;
    public Text RemTimeText;

    bool _canPress;

    static RewardedAdUI _instance;

    public static RewardedAdUI Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;

        _canPress = false;
    }

    void OnEnable()
    {
        HeyzapAdManager.FetchRewardedFinished += OnFetchRewardedFinished;

        if (!HeyzapAdManager.Instance.CheckIfRewardedVideoFetched())
            Button.gameObject.SetActive(false);
        else
            Init();
    }

    void OnFetchRewardedFinished()
    {
        HeyzapAdManager.FetchRewardedFinished -= OnFetchRewardedFinished;

        Button.gameObject.SetActive(true);

        SetCanWatch();
    }

    IEnumerator CountdownRemDuration()
    {
        while (true)
        {
            double remTime = RewardedVideoManager.Instance.GetRemainingRewardDurationInSecs();
            TimeSpan span = TimeSpan.FromSeconds(remTime);

            RemTimeText.text = string.Format("{0:00}:{1:00}", (int)span.Minutes, span.Seconds);

            yield return new WaitForFixedUpdate();

            if (SetCanWatch(false))
                yield break;
        }
    }

    void Init()
    {
        SetCanWatch();
    }

    public bool SetCanWatch(bool forceStop = true)
    {
        bool state = true;

        if (!RewardedVideoManager.Instance.CheckCanWatchVideo())
            state = false;
        
        _canPress = state;

        ButtonImage.gameObject.SetActive(state);
        RemTimeText.gameObject.SetActive(!state);

        if (forceStop)
        {
            StopCoroutine(CountdownRemDuration());

            if (!state)
                StartCoroutine(CountdownRemDuration());
        }
        return state;
    }

    public void ShowRewardedAd()
    {
        if (_canPress)
            RewardedVideoManager.Instance.WatchVideoPressed();
    }
}
