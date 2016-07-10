using UnityEngine;
using System.Collections;

public class CoinManager : MonoBehaviour 
{
    static CoinManager _instance;

    public static CoinManager Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }

}
