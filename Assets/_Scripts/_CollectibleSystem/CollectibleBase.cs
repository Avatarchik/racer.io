using UnityEngine;
using System.Collections;
using System;

public enum CollectibleTypeEnum
{
    Coin,
}

public abstract class CollectibleBase : SpawnableBase
{
    public CollectibleTypeEnum CollectibleType;

    public AudioSource CollectedSound;

    protected bool _canBeCollected;

    public void InitCollectible(SpawnableManagerBase parentManager)
    {
        _parentManager = parentManager;

        Deactivate();
    }

    public override void Activate(Vector3 spawnPos)
    {
        _canBeCollected = true;
        
        base.Activate(spawnPos);
    }

    public virtual void Collected(CarScript car)
    {
        if (!_canBeCollected)
            return;

        SetRendererActive(false);

        Use(car);

        _canBeCollected = false;

        CollectedSound.Play();

        StartCoroutine(Utilities.WaitForSoundFinish(CollectedSound, Deactivate));
    }

    public abstract void Use(CarScript car);

    public CollectibleTypeEnum GetCollectibleType()
    {
        return CollectibleType;
    }
}
