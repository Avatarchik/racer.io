using UnityEngine;
using System.Collections;

public class DailyQuest_GetRevenge : DailyQuestBase
{
    CarScript _targetCar;

    public override void StartListeningEvents()
    {
        _targetCar = CarManagerBase.BaseInstance.GetPlayerCarScript();

        _targetCar.OnGetRevenge += OnGetRevenge;
    }

    public override void FinishListeningEvents()
    {
        _targetCar.OnGetRevenge -= OnGetRevenge;
    }

    void OnGetRevenge()
    {
        CurAmount++;
        CheckQuestCompleted();
    }
}
