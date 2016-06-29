﻿using UnityEngine;
using System.Collections;
using System;

public class PlayerInGameStatController : MonoBehaviour
{
    static PlayerInGameStatController _instance;

    public static PlayerInGameStatController Instance { get { return _instance; } }

    CarScript _targetCar;

    int _destroyedCarCount;
    int _collectedHealthPackCount;
    int _collectedNewWeaponCount;
    int _stayedAsKingDuration;
    int _totalStrikeCount;
    int _initialXP;

    IEnumerator _stayAsKingRoutine;

    public GoogleAnalyticsV4 GoogleAnalytics;

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void OnEnable()
    {
        ExperienceManager.LevelIncreased += OnLevelIncreased;
    }

    void OnDisable()
    {
        ExperienceManager.LevelIncreased -= OnLevelIncreased;
    }


    public void InitNewGame()
    {
        ResetStats();

        _targetCar = CarManagerBase.BaseInstance.GetPlayerCarScript();

        _targetCar.OnDestroyedCar += OnDestroyedCar;
        _targetCar.OnCollectedHealthPack += OnCollectedHealthPack;
        _targetCar.OnCollectedNewWeapon += OnCollectedNewWeapon;
        _targetCar.OnDidStrike += OnDidStrike;
        _targetCar.OnBecameKing += OnBecameKing;
        _targetCar.OnLostKing += OnLostKing;

        _initialXP = ExperienceManager.Instance.CurXP;
    }


    public void GameOver()
    {
        _targetCar.OnDestroyedCar -= OnDestroyedCar;
        _targetCar.OnCollectedHealthPack -= OnCollectedHealthPack;
        _targetCar.OnCollectedNewWeapon -= OnCollectedNewWeapon;
        _targetCar.OnDidStrike -= OnDidStrike;
        _targetCar.OnBecameKing -= OnBecameKing;
        _targetCar.OnLostKing -= OnLostKing;

        ExperienceManager.Instance.IncreaseExperience(ExperienceManager.ExperienceSource.CarKill, _destroyedCarCount);
        ExperienceManager.Instance.IncreaseExperience(ExperienceManager.ExperienceSource.CollectedhealthPack, _collectedHealthPackCount);
        ExperienceManager.Instance.IncreaseExperience(ExperienceManager.ExperienceSource.CollectedNewWeapon, _collectedNewWeaponCount);
        ExperienceManager.Instance.IncreaseExperience(ExperienceManager.ExperienceSource.StayedAsKing, _stayedAsKingDuration);
        ExperienceManager.Instance.IncreaseExperience(ExperienceManager.ExperienceSource.Strike, _totalStrikeCount);

        GoogleAnalytics.LogEvent(new EventHitBuilder()
            .SetEventCategory("PlayerStat")
            .SetEventAction("GainedExperienceInOneGame")
            .SetEventValue(ExperienceManager.Instance.CurXP - _initialXP));
    }

    void ResetStats()
    {
        _destroyedCarCount = 0;
        _collectedHealthPackCount = 0;
        _collectedNewWeaponCount = 0;
        _stayedAsKingDuration = 0;
        _totalStrikeCount = 0;
        _initialXP = 0;
    }

    void OnLevelIncreased(int levelDiff)
    {
        GoogleAnalytics.LogEvent(new EventHitBuilder()
            .SetEventCategory("PlayerStat")
            .SetEventAction("UserLevelIncreased")
            .SetEventValue(ExperienceManager.Instance.CurLevel));
    }

    void OnDestroyedCar(CarScript p, DestroyReasonType r)
    {
        _destroyedCarCount++;
    }

    void OnCollectedHealthPack()
    {
        _collectedHealthPackCount++;
    }

    void OnCollectedNewWeapon(WeaponTypeEnum w)
    {
        _collectedNewWeaponCount++;
    }

    void OnDidStrike()
    {
        _totalStrikeCount++;
    }

    void OnBecameKing()
    {
        if (_stayAsKingRoutine != null)
            StopCoroutine(_stayAsKingRoutine);

        _stayAsKingRoutine = StayAsKingProgress();
        StartCoroutine(_stayAsKingRoutine);
    }

    void OnLostKing()
    {
        if (_stayAsKingRoutine != null)
            StopCoroutine(_stayAsKingRoutine);
    }

    public void DailyQuestCompleted(int questID)
    {
        GoogleAnalytics.LogEvent(new EventHitBuilder()
            .SetEventCategory("PlayerStat")
            .SetEventAction("CompletedDailyQuest")
            .SetEventValue(questID));
    }

    IEnumerator StayAsKingProgress()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            _stayedAsKingDuration++;
        }
    }

}
