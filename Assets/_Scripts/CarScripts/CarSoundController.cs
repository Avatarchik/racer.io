using UnityEngine;
using System.Collections;

public class CarSoundController : MonoBehaviour
{
    public AudioSource HitSoundSource;
    public AudioSource ExplodeSoundSource;
    public AudioSource CollectWeaponSoundSource;
    public AudioSource CollectHealthPackSoundSource;

    Transform _explodeParentTransform;

    void Awake()
    {
        _explodeParentTransform = ExplodeSoundSource.transform.parent;
    }

    public void Activate()
    {
        ExplodeSoundSource.transform.parent = _explodeParentTransform;
    }

    public void PlayHitSound()
    {
        HitSoundSource.Play();
    }

    public void PlayExplodeSound()
    {
        ExplodeSoundSource.transform.parent = null;
        
        ExplodeSoundSource.Play();
    }

    public void PlayCollectWeaponSound()
    {
        CollectWeaponSoundSource.Play();
    }

    public void PlayCollectHealthPackSound()
    {
        CollectHealthPackSoundSource.Play();
    }
}
