using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public enum DestroyReasonType
{
    None,
    Weapon,
    CarCrash,
    AbandonArea,
}

public enum CarControllerType
{
    Player,
    Opponent,
    NPC,
}

public class CombatCarScript : CarBase
{
    [HideInInspector]
    public bool IsKing, IsRevengeActive, IsInCombat;
    [HideInInspector]
    public int Score, StrikeCount;

    string _playerID;

    public string PlayerID{ get { return _playerID; } }

    public GameObject PlayerArrow;
    public GameObject CrownIcon, RevengeIcon;
    public TextMesh Username;
    public PlayerInputController InputController;
    public CarWeaponControllerBase WeaponController;

    float _curCombatDur;

    public float CurCombatDur { get { return _curCombatDur; } }

    #region Car Events;

    public Action OnBecameKing;

    void FireOnBecameKingEvent()
    {
        if (OnBecameKing != null)
            OnBecameKing();
    }

    public Action OnLostKing;

    void FireOnLostKingEvent()
    {
        if (OnLostKing != null)
            OnLostKing();
    }

    public Action OnGetRevenge;

    void FireOnGetRevengeEvent()
    {
        if (OnGetRevenge != null)
            OnGetRevenge();
    }

    public Action OnCollectedHealthPack;

    void FireOnCollectedHealthPack()
    {
        if (OnCollectedHealthPack != null)
            OnCollectedHealthPack();
    }

    public Action<WeaponTypeEnum> OnCollectedNewWeapon;

    public void FireOnCollectedWeapon(WeaponTypeEnum weaponType)
    {
        if (OnCollectedNewWeapon != null)
            OnCollectedNewWeapon(weaponType);
    }

    public Action OnDidStrike;

    void FireOnDidStrikeEvent()
    {
        if (OnDidStrike != null)
            OnDidStrike();
    }

    #endregion

    void Awake()
    {
        SetRevenge(false);
    }

    public override void SetCarController(CarControllerType controllerType)
    {
        base.SetCarController(controllerType);

        if (IsPlayerCar)
            PlayerArrow.SetActive(true);
        else
            PlayerArrow.SetActive(false);
    }

    public void SetPlayerID(string id)
    {
        _playerID = id;
    }

    public void AddScore(int score, bool coefWithStrikeCount)
    {
        if (coefWithStrikeCount)
            Score += StrikeCount * SinglePlayerArenaGameManager.Instance.ScoreOnKill;
        else
            Score += score;
    }

    public void AddStrikeCount(int amount = 1)
    {
        StrikeCount += amount;

        if (StrikeCount < 0)
            StrikeCount = 0;

        if (StrikeCount >= 2)
            FireOnDidStrikeEvent();
    }

    public override void Deactivate()
    {
        SetKing(false);
        SetInCombat(false);

        AddScore(-1 * (Score / 2), false);

        if (IsPlayerCar)
            InputController.DeactivateInputController();

        base.Deactivate();
    }

    public void Activate(bool startInGhostMode)
    {
        if (IsPlayerCar)
        {
            InputController.ActivateInputController();
            AnalyticsManager.Instance.SessionStarted();
        }

        WeaponController.SetNewWeapon(WeaponTypeEnum.Standard, 0);
        
        base.Activate(startInGhostMode);
    }

    public override void CollectedHealthPack(int healthAmount)
    {
        base.CollectedHealthPack(healthAmount);

        AddScore(HealthManager.Instance.BonusScore, false);

        if (IsPlayerCar)
        {
            SoundController.PlayCollectHealthPackSound();
            FireOnCollectedHealthPack();
        }
    }

    public override void AddHealth(int amount)
    {
        _curHealth += amount;

        if (_curHealth > InitHealth && !IsKing)
            _curHealth = InitHealth;
        else if (_curHealth > (InitHealth * 2) && IsKing)
            _curHealth = InitHealth * 2;
    }

    public override void GetKilled(DestroyReasonType reason, CarBase otherCar)
    {
        base.GetKilled(reason, otherCar);

        if (IsPlayerCar)
        {
            FireOnLostKingEvent();

            AnalyticsManager.Instance.SessionFinished();
        }

        CombatCarManagerBase.BaseInstance.CarDestroyed(this, otherCar, reason);
    }

    public override void DestroyedCar(DestroyReasonType reason, CarBase otherCar)
    {
        AddStrikeCount();
        SetInCombat(true);

        AddScore(SinglePlayerArenaGameManager.Instance.ScoreOnKill, true);
        AddHealth(InitHealth / 4);

        if (IsPlayerCar)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            if (PlayGameConnector.Instance.IsAuthenticated)
                PlayGameConnector.Instance.ReportKillAchievement(1);
            #endif

            FireOnDestroyedCarEvent(otherCar, reason);

            if (reason == DestroyReasonType.Weapon)
                FireOnDestroyedCarWithWeaponEvent(otherCar, WeaponController.CurWeapon.WeaponType);

            if (otherCar == CombatCarManagerBase.BaseInstance.PlayerRevengeCar)
                FireOnGetRevengeEvent();

            if (otherCar.CarBaseType == CarBaseType.CoinCar)
            {
                CurrencyManager.Instance.GainCurrency(CurrencyType.Coin, CoinCarManager.Instance.CoinRewardAmount);
            }
        }
    }

    public void SetKing(bool state)
    {
        CrownIcon.SetActive(state);

        if (state == true && !IsKing)
        {
            IsKing = state;

            AddHealth((InitHealth * 2) - _curHealth);

            if (IsPlayerCar)
                FireOnBecameKingEvent();
        }

        if (state == false && IsKing)
        {
            IsKing = state;

            float coef = _curHealth / (InitHealth * 2);

            AddHealth(Mathf.CeilToInt(InitHealth * coef) - _curHealth);

            if (IsPlayerCar)
                FireOnLostKingEvent();
        }
    }

    public void SetRevenge(bool state)
    {
        IsRevengeActive = state;

        RevengeIcon.SetActive(state);

        if (IsRevengeActive && IsKing)
        {
            RevengeIcon.transform.localPosition = new Vector3(-0.8f, RevengeIcon.transform.localPosition.y, 0f);
            CrownIcon.transform.localPosition = new Vector3(0.8f, CrownIcon.transform.localPosition.y, 0f);
        }
        else
        {
            RevengeIcon.transform.localPosition = new Vector3(0, RevengeIcon.transform.localPosition.y, 0f);
            CrownIcon.transform.localPosition = new Vector3(0, CrownIcon.transform.localPosition.y, 0f);
        }
    }

    public void SetInCombat(bool state)
    {
        IsInCombat = state;

        _curCombatDur = 0f;

        if (!IsInCombat)
            StrikeCount = 0;
    }

    public override void FixedUpdateFrame()
    {
        base.FixedUpdateFrame();

        InputController.FixedUpdateFrame();

        if (IsInCombat)
            _curCombatDur += Time.fixedDeltaTime;

        if (_curCombatDur >= SinglePlayerArenaGameManager.Instance.CombatDuration)
        {
            SetInCombat(false);
        }
    }
}