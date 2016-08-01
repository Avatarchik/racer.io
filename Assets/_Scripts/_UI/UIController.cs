using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    static UIController _instance;

    public static UIController Instance { get { return _instance; } }

    public Transform NewGameUI, CarSelectionUI, EventsUI, InGameUI, WatchModeUI, JoystickUI, StrikeControllerUI, FadeLayerUI, EveryPlayRecUI;
    public MMTweenPosition EventsUITween, CarSelectUITween, GiftScreenUITween, CameraButtonsUITween;

    public MMTweenAlpha FadeTween;

    public int PunchVibrato;
    public float PunchElasticity, PunchDuration;
    public Vector3 Punch;

    bool _isEveryPlayButtonsActive, _isGiftScreenActive;

    void Awake()
    {
        _instance = this;

        _isEveryPlayButtonsActive = false;
        _isGiftScreenActive = false;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.G) && !_isGiftScreenActive)
            ShowGiftScreenUI();

        if (Input.GetKeyUp(KeyCode.H) && _isGiftScreenActive)
            HideGiftScreenUI();
    }

    public void ShowWatchModeUI()
    {
        NewGameUI.gameObject.SetActive(false);
        JoystickUI.gameObject.SetActive(false);
        StrikeControllerUI.gameObject.SetActive(false);
        CarSelectionUI.gameObject.SetActive(false);
        EventsUI.gameObject.SetActive(false);
        FadeLayerUI.gameObject.SetActive(false);

        InGameUI.gameObject.SetActive(true);
        InGameUI.DOPunchScale(Punch, PunchDuration, PunchVibrato, PunchElasticity);

        WatchModeUI.gameObject.SetActive(true);
        WatchModeUI.DOPunchScale(Punch, PunchDuration, PunchVibrato, PunchElasticity);
    }

    public void ShowNewGameUI()
    {
        InGameUI.gameObject.SetActive(false);
        WatchModeUI.gameObject.SetActive(false);
        JoystickUI.gameObject.SetActive(false);
        StrikeControllerUI.gameObject.SetActive(false);

        FadeLayerUI.gameObject.SetActive(true);

        NewGameUI.gameObject.SetActive(true);
        NewGameUI.DOPunchScale(Punch, PunchDuration, PunchVibrato, PunchElasticity);

        CarSelectionUI.gameObject.SetActive(true);
        CarSelectionUI.DOPunchScale(Punch, PunchDuration, PunchVibrato, PunchElasticity);

        EventsUI.gameObject.SetActive(true);
        EventsUI.DOPunchScale(Punch, PunchDuration, PunchVibrato, PunchElasticity);

        CoinBarMainMenu.Instance.UpdateCoinBar();
    }

    public void ShowInGameUI()
    {
        NewGameUI.gameObject.SetActive(false);
        WatchModeUI.gameObject.SetActive(false);
        CarSelectionUI.gameObject.SetActive(false);
        EventsUI.gameObject.SetActive(false);
        FadeLayerUI.gameObject.SetActive(false);

        JoystickUI.gameObject.SetActive(true);
        JoystickUI.DOPunchScale(Punch, PunchDuration, PunchVibrato, PunchElasticity);

        StrikeControllerUI.gameObject.SetActive(true);
        StrikeControllerUI.DOPunchScale(Punch, PunchDuration, PunchVibrato, PunchElasticity);

        InGameUI.gameObject.SetActive(true);
        InGameUI.DOPunchScale(Punch, PunchDuration, PunchVibrato, PunchElasticity);
    }

    public void ShowPlaneSelectionUI()
    {
        EventsUITween.PlayForward();
        CarSelectUITween.PlayForward();
    }

    public void ShowEventsUI()
    {
        CarSelectUITween.PlayReverse();
        EventsUITween.PlayReverse();

        CoinBarMainMenu.Instance.UpdateCoinBar();
    }

    public void ShowGiftScreenUI()
    {
        _isGiftScreenActive = true;

        ActivateFadeObj();

        FadeInScreen();

        FadeTween.AddOnFinish(ShowGiftScreen);

        GiftScreenUITween.AddOnFinish(FadeOutScreen);
    }

    public void HideGiftScreenUI()
    {
        _isGiftScreenActive = false;

        ActivateFadeObj();

        FadeInScreen();

        FadeTween.AddOnFinish(HideGiftScreen);

        GiftScreenUITween.AddOnFinish(FadeOutScreen);
    }

    void FadeInScreen()
    {
        FadeTween.PlayForward();
    }

    void FadeOutScreen()
    {
        GiftScreenUITween.RemoveOnFinish(FadeOutScreen);

        FadeTween.PlayReverse();
        FadeTween.AddOnFinish(DeactivateFadeObj);
    }

    void ShowGiftScreen()
    {
        FadeTween.RemoveOnFinish(ShowGiftScreen);

        GiftScreenUITween.PlayForward();
    }

    void HideGiftScreen()
    {
        FadeTween.RemoveOnFinish(HideGiftScreen);

        GiftScreenUITween.PlayReverse();
    }

    void DeactivateFadeObj()
    {
        FadeTween.RemoveOnFinish(DeactivateFadeObj);

        FadeTween.gameObject.SetActive(false);
    }
    void ActivateFadeObj()
    {
        FadeTween.RemoveOnFinish(ActivateFadeObj);

        FadeTween.gameObject.SetActive(true);
    }


    public void SetCameraButtons()
    {
        if (!_isEveryPlayButtonsActive)
            CameraButtonsUITween.PlayForward();
        else
            CameraButtonsUITween.PlayReverse();

        _isEveryPlayButtonsActive = !_isEveryPlayButtonsActive;
    }

    public void OnPlayButtonPressed()
    {
        GameManagerBase.BaseInstance.EnterGame();
    }

    public void OnWatchButtonPressed()
    {
        GameManagerBase.BaseInstance.WatchGame();
    }

    public void OnWatchExitPressed()
    {
        GameManagerBase.BaseInstance.ExitWatch();
    }

    public void OnNoAdsPressed()
    {
        IAPManager.Instance.PurchaseProduct(Constants.NoAds_Product_ID);
    }

    public void OnGameServicesPressed()
    {
        PlayGameConnector.Instance.ShowLeaderboard();
    }

    public void OnSharePressed()
    {
        FbManager.Instance.ShareFeed();
    }

    public void OnCarBuyPressed(Image carImage)
    {
        CarTypeEnum carType = CarSelectionWindowController.Instance.CarSlotList.Find(p => p.CarSprite == carImage).CarType;

        switch (carType)
        {
            case CarTypeEnum.MrGrim:
                IAPManager.Instance.PurchaseProduct(Constants.XWing_Product_ID);
                break;
        }
    }

    public void OnEveryPlayPressed()
    {
        Everyplay.Show();
    }
}
