using UnityEngine;
using System.Collections;

public class CoinTreasureEvent : GameEventBase
{
    protected override void StartEvent()
    {
        base.StartEvent();

        CoinTreasureManager.Instance.StartSpawnProgress();
    }

    protected override void EndEvent()
    {
        base.EndEvent();

        CoinTreasureManager.Instance.StopSpawnProgress();
    }
}
