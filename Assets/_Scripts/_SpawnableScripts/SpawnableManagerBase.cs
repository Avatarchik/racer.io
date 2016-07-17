using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpawnableManagerBase : MonoBehaviour
{
    public GameObject SpawnablePrefab;

    public int MaxSpawnableInGame;

    public float MinSpawnInterval, MaxSpawnInterval;

    public bool StartAutomatically;

    protected List<SpawnableBase> _deactiveSpawnableList;
    protected List<SpawnableBase> _activeSpawnableList;

    protected bool _canSpawnSpawnable;

    protected virtual void Awake()
    {
        InitSpawnableLists();
        GenerateSpawnables();

        if (StartAutomatically)
        {
            FillArea();

            StartSpawnProgress();
        }
    }

    protected void InitSpawnableLists()
    {
        _deactiveSpawnableList = new List<SpawnableBase>();
        _activeSpawnableList = new List<SpawnableBase>();
    }

    protected void GenerateSpawnables()
    {
        for (int i = 0; i < MaxSpawnableInGame; i++)
        {
            GameObject tempCol = GameObject.Instantiate(SpawnablePrefab, Vector3.zero, Quaternion.identity) as GameObject;

            tempCol.transform.parent = transform;

            SpawnableBase colBase = tempCol.GetComponent<SpawnableBase>();

            colBase.InitSpawnable(this);
        }
    }

    protected void FillArea()
    {
        int fillAmount = Utilities.NextInt(0, MaxSpawnableInGame);
        
        for (int i = 0; i < fillAmount; i++)
            SpawnSpawnable();
    }

    public void StartSpawnProgress()
    {
        StartCoroutine(CheckSpawnProgress());
    }

    public void StopSpawnProgress()
    {
        StopCoroutine(CheckSpawnProgress());
    }

    protected virtual IEnumerator CheckSpawnProgress()
    {
        while (_canSpawnSpawnable)
        {
            SpawnSpawnable();
            
            float spawnInterval = Utilities.NextFloat(MinSpawnInterval, MaxSpawnInterval);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    protected void SpawnSpawnable()
    {
        if (_deactiveSpawnableList.Count == 0)
            return;

        SpawnableBase targetCol = _deactiveSpawnableList[0];

        Vector3 spawnPos = GetSpawnPos();

        AddToActiveList(targetCol);

        targetCol.Activate(spawnPos);

    }

    Vector3 GetSpawnPos()
    {
        return GameArea.Instance.GetRandomPosInGameArea();
    }

    void AddToActiveList(SpawnableBase spawnable)
    {
        _deactiveSpawnableList.Remove(spawnable);

        _activeSpawnableList.Add(spawnable);
    }

    public void AddToDeactiveList(SpawnableBase spawnable)
    {
        _activeSpawnableList.Remove(spawnable);

        _deactiveSpawnableList.Add(spawnable);
    }
}
