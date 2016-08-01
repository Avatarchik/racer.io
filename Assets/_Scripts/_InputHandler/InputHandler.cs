using UnityEngine;
using System.Collections;
using System;

public class InputHandler : MonoBehaviour
{
    #region Events

    public static Action OnFireButtonPressed;

    static void FireOnFireButtonPressed()
    {
        if (OnFireButtonPressed != null)
            OnFireButtonPressed();
    }

    public static Action OnFireButtonReleased;

    static void FireOnFireButtonReleased()
    {
        if (OnFireButtonReleased != null)
            OnFireButtonReleased();
    }

    public static Action OnLeftButtonPressed;

    static void FireOnLeftButtonPressed()
    {
        if (OnLeftButtonPressed != null)
            OnLeftButtonPressed();
    }

    public static Action OnLeftButtonReleased;

    static void FireOnLeftButtonReleased()
    {
        if (OnLeftButtonReleased != null)
            OnLeftButtonReleased();
    }

    public static Action OnRightButtonPressed;

    static void FireOnRightButtonPressed()
    {
        if (OnRightButtonPressed != null)
            OnRightButtonPressed();
    }

    public static Action OnRightButtonReleased;

    static void FireOnRightButtonReleased()
    {
        if (OnRightButtonReleased != null)
            OnRightButtonReleased();
    }

    #endregion

    public void FireButtonPressed()
    {
        FireOnFireButtonPressed();
    }

    public void FireButtonReleased()
    {
        FireOnFireButtonReleased();
    }

    public void LeftButtonPressed()
    {
        FireOnLeftButtonPressed();
    }

    public void LeftButtonReleased()
    {
        FireOnLeftButtonReleased();
    }

    public void RightButtonPressed()
    {
        FireOnRightButtonPressed();
    }

    public void RightButtonReleased()
    {
        FireOnRightButtonReleased();
    }
}
