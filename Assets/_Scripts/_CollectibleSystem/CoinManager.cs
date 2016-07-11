using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour 
{
    static CoinManager _instance;

    public static CoinManager Instance { get { return _instance; } }

    public GameObject CoinPrefab;

    public int MaxCoinInGame;


    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }

}
