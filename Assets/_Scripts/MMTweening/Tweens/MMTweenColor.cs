using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
/// <summary>
/// Does NOT work with MMTweenAlpha.
/// </summary>
public class MMTweenColor : MMUITweener
{
    public Color From, To;

    Image myImage;

    protected override void Wake()
    {
        myImage = gameObject.GetComponent<Image>();

        myImage.color = From;
    }

    protected override void PlayAnim(float clampedValue)
    {
        float r = CalculateR(clampedValue);
        float g = CalculateG(clampedValue);
        float b = CalculateB(clampedValue);
        float a = CalculateA(clampedValue);

        myImage.color = From + new Color(r, g, b, a);
    }

    float CalculateR(float clampedValue)
    {    
        float diff = To.r - From.r;
        return diff * clampedValue;
    }

    float CalculateG(float clampedValue)
    {
        float diff = To.g - From.g;
        return diff * clampedValue;
    }

    float CalculateB(float clampedValue)
    {
        float diff = To.b - From.b;
        return diff * clampedValue;
    }

    float CalculateA(float clampedValue)
    {
        float diff = To.a - From.a;
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
        From = GetComponent<Image>().color;
    }

    [ContextMenu("Set TO")]
    void SetTo()
    {
        To = GetComponent<Image>().color;
    }

    [ContextMenu("Assume FROM")]
    void AssumeFrom()
    {
        GetComponent<Image>().color = From;
    }

    [ContextMenu("Assume TO")]
    void AssumeTo()
    {
        GetComponent<Image>().color = To;
    }
    #endregion
}
