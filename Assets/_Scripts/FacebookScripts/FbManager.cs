using UnityEngine;
using System.Collections.Generic;
using Facebook.Unity;
using System.Collections;
using System;

public enum ActionType
{
    NA,
    Login,
    GetPlayerInfo,
    GetPlayerPicture,
    GetInvitableFriends,
    GetFriends,
    RecommendGameToFriends,
    RequestChallenge,
    GetScore,
    PostScore,
    ShareLink,
    ShareFeed,
    None
}

public class FbManager : MonoBehaviour
{
    #region Singleton

    static FbManager _instance;

    public static FbManager Instance { get { return _instance; } }

    #endregion

    public GoogleAnalyticsV4 googleAnalytics;

    ActionType nextAction;
    bool nextActionComplete = false;
    bool _processCompleted;

    #region Unity Methods

    void Awake()
    {
        _instance = this;
        if (FB.IsInitialized)
        {   //you can observe how frequently users activate your app through Facebook Analytics for Apps.
            FB.ActivateApp();
        }
        else
        {
            //Handle FB.Init
            FB.Init(() =>
                {
                    FB.ActivateApp();
                    FB.Mobile.FetchDeferredAppLinkData((IAppLinkResult result) =>
                        {
                            if (!String.IsNullOrEmpty(result.Url))
                            {
                                Debug.Log(result.Url);
                            }
                        });
                    FB.LogAppEvent(AppEventName.ActivatedApp);
                }, OnHideUnity);
        }
    }

    void OnHideUnity(bool isGameShown)
    {
        Debug.Log("onhideunity");
        if (!isGameShown)
            Time.timeScale = 0;
        else
        {
            Time.timeScale = 1;
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && FB.IsInitialized)
        {
            FB.ActivateApp();
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void OnLevelWasLoaded()
    {
        if (_instance == null)
            _instance = this;
    }

    void OnApplicationQuit()
    {
        FB.LogOut();
    }

    #endregion

    #region Actions

    IEnumerator ActionsProcess(List<ActionType> actionList)
    {
        _processCompleted = false;
        foreach (ActionType nextAct in actionList)
        {
            if (nextAction == ActionType.None)
                break;
            nextActionComplete = false;
            nextAction = nextAct;
            OnNextAction();
            while (!nextActionComplete)
                yield return null;
        }
        _processCompleted = true;
    }

    public void ShareFeed()
    {
        ResetNextAction();
        List<ActionType> actionList = new List<ActionType>();

        if (FB.IsLoggedIn)
        {
            actionList.Add(ActionType.ShareFeed);
            StartCoroutine(ActionsProcess(actionList));
        }
        else
        {
            actionList.Add(ActionType.Login);
            actionList.Add(ActionType.ShareFeed);
            StartCoroutine(ActionsProcess(actionList));
        }
    }

    public void ShareLink()
    {
        ResetNextAction();
        List<ActionType> actionList = new List<ActionType>();

        if (FB.IsLoggedIn)
        {
            actionList.Add(ActionType.ShareLink);
            StartCoroutine(ActionsProcess(actionList));
        }
        else
        {
            actionList.Add(ActionType.Login);
            actionList.Add(ActionType.ShareLink);
            StartCoroutine(ActionsProcess(actionList));
        }
    }

    void OnNextAction()
    {
        if (nextAction == ActionType.Login)
        {
            FBLogin.PromptForLogin(() =>
                {
                    nextActionComplete = true;
                });

        }
        else if (nextAction == ActionType.ShareFeed)
        {
            FB.FeedShare(
                string.Empty,
                new Uri("https://play.google.com/store/apps/details?id=com.lastchance.planesio"),
                "Just scored " + MainMenuScoreController.Instance.PlayerScore + " on planes.io. Can you beat me?",
                "Sharpen your wings and clash against other planes! Become the best war pilot!",
                "Fight against other air crafts, flex your wings and get into incredible dog-fights, " +
                "try to survive and kill others and have fun!.. Avoid crashing to others! You start as a beginner airplane " +
                "pilot and fight your way through the best while others are trying to do the same!.. Become the greatest air plane pilot ever!" +
                " How far can you fly? Are you up for a challenge?",
                new Uri("http://i.imgur.com/p0rTmRT.png"),
                string.Empty,
                this.HandleResult);


        }
        else if (nextAction == ActionType.ShareLink)
        {
            FB.ShareLink(
                new Uri("https://developers.facebook.com/"),
                "Link Share",
                "Look I'm sharing a link",
                new Uri("http://i.imgur.com/p0rTmRT.png"),
                callback: this.HandleResult
            );
        }

        // reset next action
        ResetNextAction();
    }

    void ResetNextAction()
    {
        nextAction = ActionType.NA;
    }

    #endregion

    public IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        var width = Screen.width;
        var height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        byte[] screenshot = tex.EncodeToPNG();

        var wwwForm = new WWWForm();
        wwwForm.AddBinaryData("image", screenshot, "planes_ss.png");

        FB.API("me/photos", HttpMethod.POST, ScreenShotComplete, wwwForm);
    }

    public void ScreenShotComplete(IGraphResult result)
    {
        if (result.Error != null)
        {
            Debug.Log("Cannot share screenshot");
        }
        nextActionComplete = true;
    }

    protected void HandleResult(IResult result)
    {
        nextActionComplete = true;
        if (result == null)
        {
            return;
        }
        googleAnalytics.LogSocial(new SocialHitBuilder()
            .SetSocialNetwork("Facebook")
            .SetSocialAction("Share on Feed"));
        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            PlayerProfile.Instance.UnlockCar(CarTypeEnum.Bulky);
        }
        else if (result.Cancelled)
        {
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
        }
        else
        {
        }
    }
		
}