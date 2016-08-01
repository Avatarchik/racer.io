using UnityEngine;
using System.Collections;
using System;

public enum MMTweeningEaseEnum
{
    Curve,
    Linear,
    InCubic,
    OutCubic,
    InOutCubic,
    InSine,
    OutSine,
    InOutSine,
    InExpo,
    OutExpo,
    InOutExpo,
    //InBack,
    //OutBack,
    InOutBack
}

public enum MMTweeningLoopTypeEnum
{
    Once,
    Loop,
    PingPong,
} 

public abstract class MMUITweener : MonoBehaviour
{
    #region CommonVariables
    [HideInInspector]
    public MMTweeningEaseEnum Ease;
    [HideInInspector]
    public MMTweeningLoopTypeEnum LoopType;
    [HideInInspector]
    public AnimationCurve AnimationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

    [HideInInspector]
    public bool PlayAutomatically;
    [HideInInspector]
    public bool Delay;
    [HideInInspector]
    public bool IsPlayingForward;
    [HideInInspector]
    public bool IsPlayingReverse;
    [HideInInspector]
    public bool IsPaused;
    [HideInInspector]
    public float DelayDuration;
    [HideInInspector]
    public float Duration;
    [HideInInspector]
    public bool IgnoreTimeScale = true;

    float _clampedValue, _curDuration, _enableTime;
    bool _firstEnable, _enabled;
    #endregion

    #region Events
    public delegate void TweenCallback();

    TweenCallback onKill, onFinish, onStart, onUpdate, onPause, onResume;

    #region Callback Functions
    public MMUITweener AddOnKill(TweenCallback callback)
    {
        onKill += callback;

        return this;
    }

    public MMUITweener AddOnFinish(TweenCallback callback)
    {
        onFinish += callback;

        return this;
    }

    public MMUITweener AddOnStart(TweenCallback callback)
    {
        onStart += callback;

        return this;
    }

    public MMUITweener AddOnUpdate(TweenCallback callback)
    {
        onUpdate += callback;

        return this;
    }

    public MMUITweener AddOnPause(TweenCallback callback)
    {
        onPause += callback;

        return this;
    }

    public MMUITweener AddOnResume(TweenCallback callback)
    {
        onResume += callback;

        return this;
    }
    public MMUITweener RemoveOnKill(TweenCallback callback)
    {
        onKill -= callback;

        return this;
    }

    public MMUITweener RemoveOnFinish(TweenCallback callback)
    {
        onFinish -= callback;

        return this;
    }

    public MMUITweener RemoveOnStart(TweenCallback callback)
    {
        onStart -= callback;

        return this;
    }

    public MMUITweener RemoveOnUpdate(TweenCallback callback)
    {
        onUpdate -= callback;

        return this;
    }

    public MMUITweener RemoveOnPause(TweenCallback callback)
    {
        onPause -= callback;

        return this;
    }

    public MMUITweener RemoveOnResume(TweenCallback callback)
    {
        onResume -= callback;

        return this;
    } 
    #endregion

    #region Callback Fire Functions
    protected void FireOnKill()
    {
        if (onKill == null)
            return;

        onKill();
    }

    protected void FireOnFinish()
    {
        if (onFinish == null)
            return;

        onFinish();
    }

    protected void FireOnStart()
    {
        if (onStart == null)
            return;

        onStart();
    }

    protected void FireOnUpdate()
    {
        if (onUpdate == null)
            return;

        onUpdate();
    }

    protected void FireOnPause()
    {
        if (onPause == null)
            return;

        onPause();
    }

    protected void FireOnResume()
    {
        if (onResume == null)
            return;

        onResume();
    }
    #endregion

    #endregion

    void Awake()
    {
        if (Duration < 0f)
        {
            Debug.LogWarning("Tweener duration of " + gameObject.name + "is below 0, setting it to 0.");
            Duration = 0f;
        }

        InitEventDelegates();

        KillTween();

        Wake();

        if (PlayAutomatically)
            PlayForward();
    }

    void InitEventDelegates()
    {
        onKill = null;
        onFinish = null;
        onStart = null;
        onUpdate = null;
        onPause = null;
        onResume = null;
    }

    public virtual bool IsPlaying()
    {
        if (IsPlayingForward || IsPlayingReverse)
            return true;

        return false;
    }

    public void PlayForward()
    {
        if (IsPlayingForward)
            return;

        if (IsPaused)
            ResumeTween();

        Play(true);
    }

    public void PlayReverse()
    {
        if (IsPlayingReverse)
            return;

        if (IsPaused)
            ResumeTween();

        Play(false);
    }

    public void PauseTween()
    {
        IsPaused = true;

        FireOnPause();
    }

    public void ResumeTween()
    {
        IsPaused = false;

        FireOnResume();
    }

    void Play(bool forward)
    {
        FireOnStart();

        SetPlayingDirection(forward);

        InitClampedValue(forward);

        _enabled = true;
        _enableTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        if(_enabled && !IsPaused)
        {
            if(Delay)
            {
                if (_firstEnable && Time.realtimeSinceStartup - _enableTime < DelayDuration)
                    return;
                else
                    _firstEnable = false;
            }

            _clampedValue = GetSample();

            CheckIfClampValueExceeds();

            PlayAnim(_clampedValue);

            FireOnUpdate();

            CheckIfAnimShouldFinish();

            _curDuration += IgnoreTimeScale ? Time.fixedDeltaTime : Time.fixedDeltaTime;
        }
    }

    float GetSample()
    {
        float curClampedValue = _clampedValue;

        float startValue, endValue;

        if(IsPlayingForward)
        {
            startValue = 0f;
            endValue = 1f;
        }
        else
        {
            startValue = 1f;
            endValue = 0f;
        }

        switch(Ease)
        {
            case MMTweeningEaseEnum.Linear:
                curClampedValue = MMTweeningUtilities.Linear(startValue, endValue, _curDuration, Duration);
                break;
            case MMTweeningEaseEnum.InCubic:
                curClampedValue = MMTweeningUtilities.EaseInCubic(startValue, endValue, _curDuration, Duration);
                break;
            case MMTweeningEaseEnum.OutCubic:
                curClampedValue = MMTweeningUtilities.EaseOutCubic(startValue, endValue, _curDuration, Duration);
                break;
            case MMTweeningEaseEnum.InOutCubic:
                curClampedValue = MMTweeningUtilities.EaseInOutCubic(startValue, endValue, _curDuration, Duration);
                break;
            case MMTweeningEaseEnum.InSine:
                curClampedValue = MMTweeningUtilities.EaseInSine(startValue, endValue, _curDuration, Duration);
                break;
            case MMTweeningEaseEnum.OutSine:
                curClampedValue = MMTweeningUtilities.EaseOutSine(startValue, endValue, _curDuration, Duration);
                break;
            case MMTweeningEaseEnum.InOutSine:
                curClampedValue = MMTweeningUtilities.EaseInOutSine(startValue, endValue, _curDuration, Duration);
                break;
            case MMTweeningEaseEnum.InExpo:
                curClampedValue = MMTweeningUtilities.EaseInExpo(startValue, endValue, _curDuration, Duration);
                break;
            case MMTweeningEaseEnum.OutExpo:
                curClampedValue = MMTweeningUtilities.EaseOutExpo(startValue, endValue, _curDuration, Duration);
                break;
            case MMTweeningEaseEnum.InOutExpo:
                curClampedValue = MMTweeningUtilities.EaseInOutExpo(startValue, endValue, _curDuration, Duration);
                break;
            case MMTweeningEaseEnum.InOutBack:
                curClampedValue = MMTweeningUtilities.EaseInOutBack(startValue, endValue, _curDuration, Duration);
                break;
        }

        return curClampedValue;
    }

    void CheckIfClampValueExceeds()
    {
        switch (LoopType)
        {
            case MMTweeningLoopTypeEnum.Once:
                if (_clampedValue > 1f)
                    _clampedValue = 1f;
                else if (_clampedValue < 0f)
                    _clampedValue = 0f;
                break;
            case MMTweeningLoopTypeEnum.PingPong:
                if (_clampedValue > 1f)
                {
                    _clampedValue = 1f;

                    SetPlayingDirection(false);
                }
                else if (_clampedValue < 0f)
                {
                    _clampedValue = 0f;

                    SetPlayingDirection(true);
                }
                break;
            case MMTweeningLoopTypeEnum.Loop:
                if (_clampedValue > 1f)
                {
                    _clampedValue = 0f;

                    SetPlayingDirection(IsPlayingForward);
                }
                else if (_clampedValue < 0f)
                {
                    _clampedValue = 1f;

                    SetPlayingDirection(IsPlayingForward);
                }
                break;
        }
    }

    void SetPlayingDirection(bool forward)
    {
        IsPlayingForward = forward;
        IsPlayingReverse = !forward;

        _curDuration = 0f;
    }

    void CheckIfAnimShouldFinish()
    {
        if (LoopType == MMTweeningLoopTypeEnum.Once)
        {
            if ((_clampedValue == 0f && IsPlayingReverse) || (_clampedValue == 1f && IsPlayingForward))
                FinishTween();
        }
    }

    void InitClampedValue(bool forward)
    {
        if (IsPaused)
            return;

        if (forward)
            _clampedValue = 0f;
        else
            _clampedValue = 1f;
    }

    /// <summary>
    /// DIRECTLY KILLS the tween.
    /// </summary>
    public void KillTween()
    {
        IsPaused = false;
        IsPlayingForward = false;
        IsPlayingReverse = false;

        _enableTime = 0f;
        _curDuration = 0f;
        _enabled = false;
        _firstEnable = true;

        Kill();

        FireOnKill();
    }

    /// <summary>
    /// Sets the tweening object to from/to value DIRECTLY, depending on the forward/reverse. Then KILLS the tween.
    /// </summary>
    public void FinishTween()
    {
        if (IsPlayingForward)
            _clampedValue = 1f;
        else if (IsPlayingReverse)
            _clampedValue = 0f;

        PlayAnim(_clampedValue);

        Finish();

        FireOnFinish();

        KillTween();
    }

    #region Abstract Methods
    protected abstract void Wake();
    protected abstract void PlayAnim(float clampedValue);
    protected abstract void Finish();
    protected abstract void Kill();
    #endregion
}
