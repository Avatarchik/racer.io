using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HealthManager : MonoBehaviour 
{
    static HealthManager _instance;

    public static HealthManager Instance { get { return _instance; } }

    public int BonusScore;
    public int MaxHealthPackInGameArea;
    public float HealthPackSpawnInterval;
    public GameObject HealthPackPrefab;

    List<HealthPack> _deactiveHealthPackList = new List<HealthPack>(), _activeHealthPackList = new List<HealthPack>();

    float lastSpawnTime;

    void Awake()
    {
        _instance = this;

        lastSpawnTime = Time.realtimeSinceStartup;

        Init();
    }

    void Start()
    {
        FillArea();
    }

    public void FixedUpdateFrame()
    {
        foreach(var activePack in _activeHealthPackList.ToList())
        {
            if (activePack.IsTakenByPlayer)
                DeactivateHealthPack(activePack);
        }

        if (Time.realtimeSinceStartup - lastSpawnTime < HealthPackSpawnInterval)
            return;

        if(_activeHealthPackList.Count < MaxHealthPackInGameArea && _deactiveHealthPackList.Count > 0)
        {
            lastSpawnTime = Time.realtimeSinceStartup;

            ActivateHealthPack();
        }
    }

    public void FillArea()
    {
        for (int i = 0; i < MaxHealthPackInGameArea; i++)
        {
            if (_deactiveHealthPackList.Count == 0)
                break;

            ActivateHealthPack();
        }
    }

    void ActivateHealthPack()
    {
        HealthPack targetPack = _deactiveHealthPackList[0];

        Vector2 spawnPos = GameArea.Instance.GetRandomPosInGameArea();

        targetPack.Activate(spawnPos);

        AddHealthPackToActiveList(targetPack);
    }

    void DeactivateHealthPack(HealthPack pack)
    {
        pack.Deactivate();

        AddHealthPackToDeactiveList(pack);
    }

    void Init()
    {
        for(int i = 0; i < MaxHealthPackInGameArea; i++)
        {
            GameObject healthPack = GameObject.Instantiate(HealthPackPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            HealthPack script = healthPack.GetComponent<HealthPack>();

            script.Deactivate();

            healthPack.transform.parent = transform;

            AddHealthPackToDeactiveList(script);
        }
    }

    public void AddHealthPackToDeactiveList(HealthPack healthPack)
    {
        _activeHealthPackList.Remove(healthPack);
        _deactiveHealthPackList.Add(healthPack);
    }

    public void AddHealthPackToActiveList(HealthPack healthPack)
    {
        _deactiveHealthPackList.Remove(healthPack);
        _activeHealthPackList.Add(healthPack);
    }

    public HealthPack GetClosestHealthPack(Vector2 position)
    {
        List<HealthPack> packList = _activeHealthPackList;

        if (packList.Count == 0)
            return null;

        packList = packList.OrderByDescending(val => Vector2.Distance(position, val.transform.position)).ToList();

        return packList[packList.Count - 1];
    }


    public void Reset()
    {
        foreach (var activePack in _activeHealthPackList.ToList())
            DeactivateHealthPack(activePack);
    }
}
