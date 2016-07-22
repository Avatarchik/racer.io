using UnityEngine;
using System.Collections;

public class DailyQuest_GetRevenge : DailyQuestBase
{
    CombatCarScript _targetCar;

    public override void StartListeningEvents()
    {
        _targetCar = CombatCarManagerBase.BaseInstance.GetPlayerCarScript();

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
