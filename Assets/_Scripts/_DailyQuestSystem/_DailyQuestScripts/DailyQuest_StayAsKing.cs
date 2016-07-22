using UnityEngine;
using System.Collections;

public class DailyQuest_StayAsKing : DailyQuestBase
{
    CombatCarScript _targetCar;

    IEnumerator _stayAsKingRoutine;

    

    public override void StartListeningEvents()
    {
        _targetCar = CombatCarManagerBase.BaseInstance.GetPlayerCarScript();

        _targetCar.OnBecameKing += OnBecameKing;
        _targetCar.OnLostKing += OnLostKing;
    }

    public override void FinishListeningEvents()
    {
        _targetCar.OnBecameKing -= OnBecameKing;
        _targetCar.OnLostKing -= OnLostKing;
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

    IEnumerator StayAsKingProgress()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            CurAmount++;
            CheckQuestCompleted();
        }
    }
}
