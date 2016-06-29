using UnityEngine;
using System.Collections;

public class SkidMarkManager : MonoBehaviour
{
    static SkidMarkManager _instance;

    public static SkidMarkManager Instance{ get { return _instance; } }

    public GameObject SkidMarkPrefab;

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }
}
