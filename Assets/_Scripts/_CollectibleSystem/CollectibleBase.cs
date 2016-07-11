using UnityEngine;
using System.Collections;
using System;

public enum CollectibleTypeEnum
{
    Coin,
}

public class CollectibleBase : MonoBehaviour 
{
    public CollectibleTypeEnum CollectibleType;

    protected bool _canBeCollected;

    protected CollectibleManagerBase _parentManager;

    public void InitCollectible(CollectibleManagerBase parentManager)
    {
        _parentManager = parentManager;
    }

    public virtual void Activate(Vector3 spawnPos)
    {
        transform.position = spawnPos;

        transform.parent = null;

        gameObject.SetActive(true);
    }

    public virtual void Deactivate()
    {
        transform.parent = _parentManager.transform;

        _parentManager.AddToDeactiveList(this);

        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)LayerEnum.Car)
        {
            if (_canBeCollected)
                Collected();
        }
    }

    public virtual void Collected()
    {
        _canBeCollected = false;

        //_parentManager.CollectedCollectible(this);
    }

    public CollectibleTypeEnum GetCollectibleType()
    {
        return CollectibleType;
    }
}
