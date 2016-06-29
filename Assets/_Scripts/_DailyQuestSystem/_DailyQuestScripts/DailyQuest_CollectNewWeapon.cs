using UnityEngine;
using System.Collections;

public class DailyQuest_CollectNewWeapon : DailyQuestBase
{
    CarScript _targetCar;

    public override void StartListeningEvents()
    {
        _targetCar = CarManagerBase.BaseInstance.GetPlayerCarScript();

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
