using UnityEngine;
using System.Collections;

public class CoinCar : SpawnableBase 
{
    public int InitHealth;

    public CoinCarMovementController MovementController;
    public CoinCarAI CarAI;
    public CoinCarAnimationController AnimationController;
    public CoinCarParticleController ParticleController;

    public AudioSource HitSound;
    public AudioSource KilledSound;

    int _curHealth;
    public int CurHealth { get { return _curHealth; } }

    bool _canGetHit;

    public bool CanGetHit { get { return _canGetHit; } }

    public override void Activate(Vector3 spawnPos)
    {
        MovementController.ActivateMovement();

        _canGetHit = true;
        SetInitHealth();

        base.Activate(spawnPos);
    }

    void SetInitHealth()
    {
        _curHealth = InitHealth;
    }

    public void GetHit(BulletScript bullet)
    {
        if (!_canGetHit)
            return;

        HitSound.Play();
        ParticleController.PlayHitParticle();

        DecreaseHealth(bullet.Damage);
    }

    public void DecreaseHealth(int amount)
    {
        _curHealth -= amount;

        if (_curHealth < 0)
            _curHealth = 0;

        if (_curHealth == 0)
            GetKilled();
    }

    public void GetKilled()
    {
        _canGetHit = false;

        SetRendererActive(false);

        KilledSound.Play();
        ParticleController.PlayExplodeParticle();

        StartCoroutine(Utilities.WaitForSoundFinish(KilledSound, Deactivate));
    }
}
