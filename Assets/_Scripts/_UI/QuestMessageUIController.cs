using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class UIItemAlphaInfo
{
    public Graphic UIItem;
    public float MinAlpha, MaxAlpha;
}

public class QuestMessageUIController : MonoBehaviour
{
    public Text QuestStatusText;
    public float TransitionDuration;
    public float MessageWaitDuration;

    public List<UIItemAlphaInfo> AlphaEffectedItemsList;

    IEnumerator _questStatusRoutine;

    public void StartQuestStatusRoutine()
    {
        if (_questStatusRoutine != null)
            StopCoroutine(_questStatusRoutine);

        _questStatusRoutine = WriteQuestStatusRoutine();
        StartCoroutine(_questStatusRoutine);
    }

    void OnEnable()
    {
        AlphaEffectedItemsList.ForEach(o => o.UIItem.color = new Color(o.UIItem.color.r, o.UIItem.color.g, o.UIItem.color.b, 0f));
    }

    IEnumerator WriteQuestStatusRoutine()
    {
        QuestStatusText.text = DailyQuestManager.Instance.CurDailyQuest.GetCurrentStatus();

        //Starts to fade out
        float curTime = 0;
        while (curTime <= TransitionDuration)
        {
            foreach (var item in AlphaEffectedItemsList)
            {
                float startingAlpha = item.MinAlpha;
                if (item.UIItem.color.a > item.MinAlpha)
                    startingAlpha = item.UIItem.color.a;

                float endAlpha = item.MaxAlpha;

                float newAlpha = Mathf.Lerp(startingAlpha, endAlpha, curTime / TransitionDuration);

                item.UIItem.color = new Color(item.UIItem.color.r, item.UIItem.color.g, item.UIItem.color.b, newAlpha);
            }

            curTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(MessageWaitDuration);

        //Starts to fade in
        curTime = 0;
        while (curTime <= TransitionDuration)
        {
            foreach (var item in AlphaEffectedItemsList)
            {
                float startingAlpha = item.MaxAlpha;
                float endAlpha = item.MinAlpha;

                float newAlpha = Mathf.Lerp(startingAlpha, endAlpha, curTime / TransitionDuration);

                item.UIItem.color = new Color(item.UIItem.color.r, item.UIItem.color.g, item.UIItem.color.b, newAlpha);
            }

            curTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        _questStatusRoutine = null;
    }
}
