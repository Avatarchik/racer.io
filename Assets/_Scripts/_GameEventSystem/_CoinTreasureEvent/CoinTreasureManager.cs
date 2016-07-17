using UnityEngine;
using System.Collections;

public class CoinTreasureManager : SpawnableManagerBase
{
    static CoinTreasureManager _instance;

    public static CoinTreasureManager Instance{ get { return _instance; } }

    protected override void Awake()
    {
        base.Awake();

        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }
}
