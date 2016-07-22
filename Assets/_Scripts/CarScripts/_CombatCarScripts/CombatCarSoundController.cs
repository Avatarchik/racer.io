using UnityEngine;
using System.Collections;

public class CombatCarSoundController : CarSoundControllerBase
{
    public AudioSource CollectWeaponSoundSource;

    public void PlayCollectWeaponSound()
    {
        CollectWeaponSoundSource.Play();
    }
}
