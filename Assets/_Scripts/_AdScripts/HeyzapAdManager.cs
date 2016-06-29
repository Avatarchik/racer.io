using UnityEngine;
using System.Collections;
using Heyzap;
using System;

public class HeyzapAdManager : MonoBehaviour
{

    public enum AdType
    {
        Interstitial,
        Video,
        Incentivized,
        Banner
    }

    public int AD_INTERVAL_IN_SECONDS = -1;

    public static HeyzapAdManager Instance { get { return _instance; } }

    static HeyzapAdManager _instance;
    float _lastVideoShownTime;

    public static event Action<bool> OnIncentivizedFinished;
    public static event Action FetchRewardedFinished;

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void Start()
    {

        StartCoroutine(FetchAdInterval());
        HeyzapAds.Start("96f999087dee7ba29c03838181efa13e", HeyzapAds.FLAG_NO_OPTIONS);
        HandleAdDisplayEvents();
        TryFetchAds();
        TryFetchIncentivized();
    }

    IEnumerator FetchAdInterval()
    {
        while (true)
        {
            if (PlayerProfile.Instance.CheckIfAdsAreRemoved())
                yield break;
            if (InternetChecker.IsconnectedToInternet)
            {
                yield return StartCoroutine(InternetChecker.GetAdInterval(value => AD_INTERVAL_IN_SECONDS = value));
                yield break;
            }
            else if (AD_INTERVAL_IN_SECONDS < 0)
            {
                AD_INTERVAL_IN_SECONDS = InternetChecker.GetIntervalFromResources();
            }
            yield return new WaitForSeconds(150);
            //CheckTimerToShowNextAd();
        }
    }

    private void HandleAdDisplayEvents()
    {
        HZInterstitialAd.SetDisplayListener(delegate(string adState, string adTag)
            {
                if (adState.Equals("show"))
                {
                    // Sent when an ad has been displayed.
                    // This is a good place to pause your app, if applicable.
                }
                if (adState.Equals("hide"))
                {
                    // Sent when an ad has been removed from view.
                    // This is a good place to unpause your app, if applicable.
                    _lastVideoShownTime = Time.timeSinceLevelLoad;
                    TryFetchAds();
                }
                if (adState.Equals("click"))
                {
                    // ad'e tıklandiginda bisey yapacaksak
                }
                if (adState.Equals("failed"))
                {
                    // Sent when you call `show`, but there isn't an ad to be shown.
                    TryFetchAds();
                }
                if (adState.Equals("available"))
                {
                    // Sent when an ad has been loaded and is ready to be displayed
                }
                if (adState.Equals("fetch_failed"))
                {
                    // Sent when an ad has failed to load.

                }
                if (adState.Equals("audio_starting"))
                {
                    // The ad about to be shown will need audio.
                    // Mute any background music.
                }
                if (adState.Equals("audio_finished"))
                {
                    // The ad being shown no longer needs audio.
                    // Any background music can be resumed.
                }
            });

        HZIncentivizedAd.SetDisplayListener(delegate(string adState, string adTag)
            {
                Debug.Log("display listener" + adState);
                if (adState.Equals("hide"))
                {
                    // Sent when an ad has been removed from view.
                    // This is a good place to unpause your app, if applicable.
                    TryFetchIncentivized();
                }
                if (adState.Equals("incentivized_result_complete"))
                {
                    // The user has watched the entire video and should be given a reward.
                    Debug.Log("onincentivizedeventtriggered true");
                    if (OnIncentivizedFinished != null)
                        OnIncentivizedFinished(true);

                }
                if (adState.Equals("incentivized_result_incomplete"))
                {
                    // The user did not watch the entire video and should not be given a reward.
                    Debug.Log("onincentivizedeventtriggered false");
                    if (OnIncentivizedFinished != null)
                        OnIncentivizedFinished(false);
                }
                if (adState.Equals("available"))
                {
                    Debug.Log("fetch complete");
                    if (FetchRewardedFinished != null)
                        FetchRewardedFinished();
                    // Sent when an ad has been loaded and is ready to be displayed
                }
                if (adState.Equals("fetch_failed"))
                {
                    Debug.Log("fetch failed");
                    TryFetchIncentivized();
                    // Sent when an ad has failed to load.

                }
            });
    }

    void TryFetchAds()
    {

        if (PlayerProfile.Instance.CheckIfAdsAreRemoved())
            return;
        if (!HZInterstitialAd.IsAvailable())
            HZInterstitialAd.Fetch();
        //if (!HZVideoAd.IsAvailable())
        //    HZVideoAd.Fetch();
    }

    void TryFetchIncentivized()
    {
        if (!HZIncentivizedAd.IsAvailable())
            HZIncentivizedAd.Fetch();
    }

    void ShowInterstitial()
    {
        if (PlayerProfile.Instance.CheckIfAdsAreRemoved())
            return;
        if (HZInterstitialAd.IsAvailable())
        {
            HZInterstitialAd.Show();
        }
    }

    void ShowVideoAd()
    {
        if (PlayerProfile.Instance.CheckIfAdsAreRemoved())
            return;
        if (HZVideoAd.IsAvailable())
        {
            HZVideoAd.Show();
        }
    }

    public void ShowRewardedAd()
    {
        if (HZIncentivizedAd.IsAvailable())
        {
            HZIncentivizedAd.Show();
        }
    }

    public void CheckTimerToShowNextAd()
    {
        //Debug.Log(AD_INTERVAL_IN_SECONDS + " " + InternetChecker.IsconnectedToInternet);
        if (Time.timeSinceLevelLoad - _lastVideoShownTime >= AD_INTERVAL_IN_SECONDS)
        {
            ShowInterstitial();
        }
    }

    public bool CheckIfRewardedVideoFetched()
    {
        return HZIncentivizedAd.IsAvailable();
    }
	
}
