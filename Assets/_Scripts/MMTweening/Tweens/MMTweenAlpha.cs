using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
/// <summary>
/// Does NOT work with MMTweenColor.
/// </summary>
public class MMTweenAlpha : MMUITweener
{
    [HideInInspector]
    public float From, To;

    Image myImage;

    protected override void Wake()
    {
        myImage = gameObject.GetComponent<Image>();

        Color color = myImage.color;
        color.a = From;

        myImage.color = color;
    }

    protected override void PlayAnim(float clampedValue)
    {
        float a = CalculateA(clampedValue);

        Color newColor = myImage.color;
        newColor.a = From + a;

        myImage.color = newColor;
    }

    float CalculateA(float clampedValue)
    {
        float diff = To - From;
        return diff * clampedValue;
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
        From = GetComponent<Image>().color.a;
    }

    [ContextMenu("Set TO")]
    void SetTo()
    {
        To = GetComponent<Image>().color.a;
    }

    [ContextMenu("Assume FROM")]
    void AssumeFrom()
    {
        Color color = GetComponent<Image>().color;
        color.a = From;

        GetComponent<Image>().color = color;
    }

    [ContextMenu("Assume TO")]
    void AssumeTo()
    {
        Color color = GetComponent<Image>().color;
        color.a = To;

        GetComponent<Image>().color = color;
    }
    #endregion
}
