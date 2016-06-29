using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class UIDailyQuest : MonoBehaviour
{
    public Text DescriptionText, StatusText;

    static UIDailyQuest _instance;

    public static UIDailyQuest Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;

        DailyQuestManager.OnDailyQuestSet += OnDailyQuestSet;
        DailyQuestManager.OnQuestProgressUpdated += UpdateDailyQuestInfo;
    }

    void OnDestroy()
    {
        _instance = null;

        DailyQuestManager.OnQuestProgressUpdated -= UpdateDailyQuestInfo;
    }

    void OnEnable()
    {
        transform.DOPunchScale(new Vector3(0.05f, 0.05f, 1f), 0.5f, 5, 0.5f);
    }

    void OnDailyQuestSet()
    {
        DailyQuestManager.OnDailyQuestSet -= OnDailyQuestSet;

        UpdateDailyQuestInfo();
    }

    public void UpdateDailyQuestInfo()
    {
        DescriptionText.text = DailyQuestManager.Instance.CurDailyQuest.GetQuestDescription();
        StatusText.text = DailyQuestManager.Instance.CurDailyQuest.GetCurrentStatus();
    }
}
