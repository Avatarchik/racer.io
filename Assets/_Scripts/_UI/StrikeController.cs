using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

[System.Serializable]
public class UIStrikeInfo
{
    public int ActivationStrikeCount;
    public Transform Parent;
    public Text StrikeText, StrikeCount;
}

public class StrikeController : MonoBehaviour
{
    static StrikeController _instance;
    public static StrikeController Instance { get { return _instance; } }

    public int PunchVibrato;
    public float PunchElasticity, PunchDuration;
    public Vector3 Punch;

    public float StrikeWaitDuration;

    [HideInInspector]
    public Tweener PunchTweener;

    IEnumerator _strikeUIRoutine;

    public List<UIStrikeInfo> UIStrikeInfoList;

    void Awake()
    {
        _instance = this;

        PunchTweener = null;
        _strikeUIRoutine = null;

        UIStrikeInfoList = UIStrikeInfoList.OrderBy(s => s.ActivationStrikeCount).ToList();
    }

    public void ActivateStrikeUI(int strikeCount)
    {
        if (_strikeUIRoutine != null)
            StopCoroutine(_strikeUIRoutine);

        _strikeUIRoutine = ActivateStrikeUIRoutine(strikeCount);
        StartCoroutine(_strikeUIRoutine);
    }

    IEnumerator ActivateStrikeUIRoutine(int strikeCount)
    {
        UIStrikeInfo targetContainer = UIStrikeInfoList.Find(c => c.ActivationStrikeCount >= strikeCount);

        if (targetContainer == null)
            targetContainer = UIStrikeInfoList[UIStrikeInfoList.Count - 1];

        if (PunchTweener != null && !PunchTweener.IsComplete())
            PunchTweener.Complete();

        UIStrikeInfoList.ForEach(c => c.Parent.gameObject.SetActive(false));
        UIStrikeInfoList.ForEach(c => c.Parent.localScale = new Vector3(0.3f, 0.3f, 1f));

        targetContainer.StrikeCount.text = strikeCount.ToString();

        targetContainer.Parent.gameObject.SetActive(true);

        PunchTweener = targetContainer.Parent.DOPunchScale(Punch, PunchDuration, PunchVibrato, PunchElasticity);

        yield return new WaitForSeconds(StrikeWaitDuration + PunchTweener.Duration());

        targetContainer.Parent.DOScale(0f, 0.2f);
    }
}
