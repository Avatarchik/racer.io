using UnityEngine;
using System.Collections;

public class DailyQuest_CollectNewWeapon : DailyQuestBase
{
    CombatCarScript _targetCar;

    public override void StartListeningEvents()
    {
        _targetCar = CombatCarManagerBase.BaseInstance.GetPlayerCarScript();

        _targetCar.OnCollectedNewWeapon += OnCollectedNewWeapon;
    }

    public override void FinishListeningEvents()
    {
        _targetCar.OnCollectedNewWeapon -= OnCollectedNewWeapon;
    }

    void OnCollectedNewWeapon(WeaponTypeEnum weaponType)
    {
        CurAmount++;
        CheckQuestCompleted();
    }
}
