using UnityEngine;
using System.Collections;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    static UIController _instance;

    public static UIController Instance { get { return _instance; } }

    public Transform NewGameUI, PlaneSelectionUI, EventsUI, InGameUI, WatchModeUI, JoystickUI, StrikeControllerUI, FadeLayerUI;

    public Transform EventsUIInactiveTransform, PlaneSelectUIInactiveTransform;

    public int PunchVibrato;
    public float PunchElasticity, PunchDuration;
    public Vector3 Punch;

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void ShowWatchModeUI()
    {
        NewGameUI.gameObject.SetActive(false);
        JoystickUI.gameObject.SetActive(false);
        StrikeControllerUI.gameObject.SetActive(false);
        PlaneSelectionUI.gameObject.SetActive(false);
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

        Debug.Log("buraya geldi");

        NewGameUI.gameObject.SetActive(true);
        NewGameUI.DOPunchScale(Punch, PunchDuration, PunchVibrato, PunchElasticity);

        PlaneSelectionUI.gameObject.SetActive(true);
        PlaneSelectionUI.DOPunchScale(Punch, PunchDuration, PunchVibrato, PunchElasticity);

        EventsUI.gameObject.SetActive(true);
        EventsUI.DOPunchScale(Punch, PunchDuration, PunchVibrato, PunchElasticity);
    }

    public void ShowInGameUI()
    {
        NewGameUI.gameObject.SetActive(false);
        WatchModeUI.gameObject.SetActive(false);
        PlaneSelectionUI.gameObject.SetActive(false);
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
        PlaneSelectionUI.DOLocalMove(Vector3.zero, 0.2f, true).SetEase(Ease.InCubic);

        EventsUI.DOLocalMove(EventsUIInactiveTransform.localPosition, 0.2f, true).SetEase(Ease.InCubic);
    }

    public void ShowEventsUI()
    {
        EventsUI.DOLocalMove(Vector3.zero, 0.2f, true).SetEase(Ease.InCubic);

        PlaneSelectionUI.DOLocalMove(PlaneSelectUIInactiveTransform.localPosition, 0.2f, true).SetEase(Ease.InCubic);
    }
}
