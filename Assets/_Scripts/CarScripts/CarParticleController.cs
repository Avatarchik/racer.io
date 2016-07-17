using UnityEngine;
using System.Collections;

public class CarParticleController : MonoBehaviour
{
    public CircleCollider2D HitParticleRange;
    
    public tk2dSpriteAnimator HitParticle;
    public tk2dSpriteAnimator ExplodeParticle;

    Vector2 _explodeInitLocalPos;
    Transform _explodeInitParent;

    void Awake()
    {
        _explodeInitParent = ExplodeParticle.transform.parent;
        _explodeInitLocalPos = ExplodeParticle.transform.localPosition;
    }

    public void Activate()
    {
        
    }

    public void PlayHitParticle()
    {
        return;
        
        Vector3 particleNewPos = GetRandomHitPos();
        particleNewPos.z = HitParticle.transform.position.z;
        HitParticle.transform.position = particleNewPos;

        HitParticle.Play();

    }

    Vector2 GetRandomHitPos()
    {
        Vector2 particlePos = Random.insideUnitCircle * HitParticleRange.radius + (Vector2)transform.position;

        return particlePos;
    }

    public void PlayExplodeParticle()
    {
        ExplodeParticle.transform.parent = _explodeInitParent;
        ExplodeParticle.transform.localPosition = _explodeInitLocalPos;

        ExplodeParticle.transform.parent = null;

        ExplodeParticle.Play();

    }
}
