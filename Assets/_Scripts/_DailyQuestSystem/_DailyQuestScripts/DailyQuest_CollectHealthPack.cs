using UnityEngine;
using System.Collections;

public class DailyQuest_CollectHealthPack : DailyQuestBase
{
    CombatCarScript _targetCar;

    public override void StartListeningEvents()
    {
        _targetCar = CombatCarManagerBase.BaseInstance.GetPlayerCarScript();

        _targetCar.OnCollectedHealthPack += OnCollectedhealthPack;
    }

    public override void FinishListeningEvents()
    {
        _targetCar.OnCollectedHealthPack -= OnCollectedhealthPack;
    }

    void OnCollectedhealthPack()
    {
        CurAmount++;
        CheckQuestCompleted();
    }
}
