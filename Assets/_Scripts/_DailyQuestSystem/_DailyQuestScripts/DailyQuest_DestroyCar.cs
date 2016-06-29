using UnityEngine;
using System.Collections;

public class DailyQuest_DestroyCar : DailyQuestBase
{
    DestroyReasonType _destroyReason;
    WeaponTypeEnum _weaponType;

    CarScript _targetCar;

    const string BY_CRASHING = " by crashing";
    const string BY_WEAPON = " by using {weapon}";
    const string WEAPON_TAG = "{weapon}";

    #region implemented abstract members of DailyQuestBase

    public override void InitDailyQuest(DailyQuestInfo questInfo, int curAmount)
    {
        base.InitDailyQuest(questInfo, curAmount);

        _destroyReason = Utilities.IdentifyObjectEnum(questInfo.AdditionalParams[0], DestroyReasonType.None);
        _weaponType = Utilities.IdentifyObjectEnum(questInfo.AdditionalParams[1], WeaponTypeEnum.None);
    }

    public override void StartListeningEvents()
    { 
        _targetCar = CarManagerBase.BaseInstance.GetPlayerCarScript();

        if (_weaponType == WeaponTypeEnum.None)
            _targetCar.OnDestroyedCar += OnDestroyedCar;
        else
            _targetCar.OnDestroyedCarWithWeapon += OnDestroyedCarWithWeapon;
    }

    public override void FinishListeningEvents()
    {
        if (_weaponType == WeaponTypeEnum.None)
            _targetCar.OnDestroyedCar -= OnDestroyedCar;
        else
            _targetCar.OnDestroyedCarWithWeapon -= OnDestroyedCarWithWeapon;
    }

    #endregion

    void OnDestroyedCar(CarScript destroyedCar, DestroyReasonType reasonType)
    {
        if (_destroyReason == reasonType
            || _destroyReason == DestroyReasonType.None)
        {
            CurAmount++;
            CheckQuestCompleted();
        }
    }

    void OnDestroyedCarWithWeapon(CarScript destroyedCar, WeaponTypeEnum weaponType)
    {
        if (_weaponType == weaponType)
        {
            CurAmount++;
            CheckQuestCompleted();
        }
    }

    public override string GetQuestDescription()
    {
        string curQuestDesc = QuestDesc;

        curQuestDesc = curQuestDesc.Replace(AMOUNT_TAG, QuestInfo.RequiredAmount.ToString());

        if (_destroyReason == DestroyReasonType.CarCrash)
            curQuestDesc += BY_CRASHING;
        else if (_destroyReason == DestroyReasonType.Weapon)
        {
            curQuestDesc += BY_WEAPON;
            curQuestDesc = curQuestDesc.Replace(WEAPON_TAG, _weaponType.ToString());
        }

        if (QuestInfo.QuestDuration == DailyQuestDuration.Session)
        {
            curQuestDesc = curQuestDesc.Replace("!", "");
            curQuestDesc += IN_ONE_GAME_DESC;
        }

        if (_isCompleted)
            curQuestDesc = QUEST_COMPLETED_DESC;

        return curQuestDesc;
    }
}
