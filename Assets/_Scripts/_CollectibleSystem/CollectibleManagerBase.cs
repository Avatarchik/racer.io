using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CollectibleManagerBase : MonoBehaviour 
{
    public GameObject CollectiblePrefab;

    public int MaxCollectibleInGame;

    protected List<CollectibleBase> _deactiveCollectibleList;
    protected List<CollectibleBase> _activeCollectibleList;

    #region Events

    public static Action<CollectibleBase> OnCollectibleCollected;

    void FireOnCollectibleCollected(CollectibleBase collectible)
    {
        if (OnCollectibleCollected != null)
            OnCollectibleCollected(collectible);
    }

    #endregion

    void Awake()
    {

    }

    void InitEnterableLists()
    {
        _deactiveCollectibleList = new List<CollectibleBase>();
        _activeCollectibleList = new List<CollectibleBase>();
    }

    void GenerateEnterables()
    {
        for(int i = 0; i < MaxCollectibleInGame; i++)
        {
            GameObject tempCol = GameObject.Instantiate(CollectiblePrefab, Vector3.zero, Quaternion.identity) as GameObject;

            tempCol.transform.parent = transform;

            CollectibleBase colBase = tempCol.GetComponent<CollectibleBase>();

            colBase.InitCollectible(this);
        }
    }

    public void CollectibleCollected(CollectibleBase collectible)
    {

    }

    void AddToActiveList(CollectibleBase collectible)
    {
        _deactiveCollectibleList.Remove(collectible);

        _activeCollectibleList.Add(collectible);
    }

    public void AddToDeactiveList(CollectibleBase collectible)
    {
        _activeCollectibleList.Remove(collectible);

        _deactiveCollectibleList.Add(collectible);
    }
}
