using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class PlayGameConnector : MonoBehaviour
{

    public static PlayGameConnector Instance { get { return _instance; } }

    static PlayGameConnector _instance;
    public bool IsAuthenticated;
    public GoogleAnalyticsV4 googleAnalytics;

    // Use this for initialization
    void Awake()
    {
        _instance = this;
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);

        //PlayGamesPlatform.DebugLogEnabled = true;

        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate((bool success) =>
            {
                IsAuthenticated = true;
                MainMenuScoreController.Instance.EnableGameServicesButton();
                if (PlayGamesPlatform.Instance.GetAchievement(GPGSIds.achievement_mega_killer) != null &&
                    PlayGamesPlatform.Instance.GetAchievement(GPGSIds.achievement_mega_killer).IsUnlocked)
                {
                    PlayerProfile.Instance.UnlockCar(CarTypeEnum.Fury);
                }
                StartCoroutine(SetUsernameWhenAvailable());

            });


    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void ShowLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }

    public void ShowAchievements()
    {
        Social.ShowAchievementsUI();
    }

    public void PostScoreToLeaderboard(int score)
    {
        if (IsAuthenticated)
        {
            Social.ReportScore(score, GPGSIds.leaderboard_kings_of_wings, null);
        }
    }

    IEnumerator SetUsernameWhenAvailable()
    {
        while (CarSelectionWindowController.Instance == null)
        {
            yield return new WaitForEndOfFrame();
        }
        if (string.IsNullOrEmpty(CarSelectionWindowController.Instance.Username))
        {
            CarSelectionWindowController.Instance.Username = Social.localUser.userName;
        }
    }

    public void ReportKillAchievement(int score)
    {
        if (!PlayGamesPlatform.Instance.GetAchievement(GPGSIds.achievement_apprentice_pilot).IsUnlocked)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(
                GPGSIds.achievement_apprentice_pilot, score, null
            );
            googleAnalytics.LogEvent("Achievement", "Unlocked", "Apprentice Pilot", 1);
        }
        else if (!PlayGamesPlatform.Instance.GetAchievement(GPGSIds.achievement_dogfight_veteran).IsUnlocked)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(
                GPGSIds.achievement_dogfight_veteran, score, null
            );
            
        }
        else if (!PlayGamesPlatform.Instance.GetAchievement(GPGSIds.achievement_mega_killer).IsUnlocked)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(
                GPGSIds.achievement_mega_killer, score, null
            );
        }
        else if (!PlayGamesPlatform.Instance.GetAchievement(GPGSIds.achievement_dominator).IsUnlocked)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(
                GPGSIds.achievement_dominator, score, null
            );
        }
        else if (!PlayGamesPlatform.Instance.GetAchievement(GPGSIds.achievement_i_am_the_king).IsUnlocked)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(
                GPGSIds.achievement_i_am_the_king, score, null
            );
            googleAnalytics.LogEvent("Achievement", "Unlocked", "I am the king", 1);
        }

        if (PlayGamesPlatform.Instance.GetAchievement(GPGSIds.achievement_mega_killer).IsUnlocked)
        {
            PlayerProfile.Instance.UnlockCar(CarTypeEnum.Fury);
        }
    }
}
