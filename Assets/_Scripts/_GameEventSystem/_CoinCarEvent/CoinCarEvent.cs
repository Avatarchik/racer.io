using UnityEngine;
using System.Collections;

public class CoinCarEvent : GameEventBase
{
    protected override void StartEvent()
    {
        base.StartEvent();

        CoinCarManager.Instance.StartSpawnProgress();
    }

    protected override void EndEvent()
    {
        base.EndEvent();

        CoinCarManager.Instance.StopSpawnProgress();
    }
}
