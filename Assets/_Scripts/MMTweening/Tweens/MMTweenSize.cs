using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class MMTweenSize : MMUITweener
{
    public Vector2 From, To;

    RectTransform myTransform;

    protected override void Wake()
    {
        myTransform = gameObject.GetComponent<RectTransform>();

        myTransform.sizeDelta = From;
    }

    protected override void PlayAnim(float clampedValue)
    {
        Vector2 diff = To - From;
        Vector2 delta = diff * clampedValue;

        myTransform.sizeDelta = From + delta;
    }

    protected override void Finish()
    {
    }

    protected override void Kill()
    {
    }

    #region ContextMenu
    [ContextMenu("Set FROM")]
    void SetFrom()
    {
        From = GetComponent<RectTransform>().sizeDelta;
    }

    [ContextMenu("Set TO")]
    void SetTo()
    {
        To = GetComponent<RectTransform>().sizeDelta;
    }

    [ContextMenu("Assume FROM")]
    void AssumeFrom()
    {
        GetComponent<RectTransform>().sizeDelta = From;
    }

    [ContextMenu("Assume TO")]
    void AssumeTo()
    {
        GetComponent<RectTransform>().sizeDelta = To;
    }
    #endregion
}
