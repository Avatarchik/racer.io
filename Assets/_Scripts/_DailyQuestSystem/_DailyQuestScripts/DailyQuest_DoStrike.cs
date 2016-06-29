using UnityEngine;
using System.Collections;

public class DailyQuest_DoStrike : DailyQuestBase
{
    CarScript _targetCar;

    public override void StartListeningEvents()
    {
        _targetCar = CarManagerBase.BaseInstance.GetPlayerCarScript();

        _targetCar.OnDidStrike += OnDidStrike;
    }

    public override void FinishListeningEvents()
    {
        _targetCar.OnDidStrike -= OnDidStrike;
    }

    void OnDidStrike()
    {
        CurAmount++;
        CheckQuestCompleted();
    }
}
