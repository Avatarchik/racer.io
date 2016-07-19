using UnityEngine;
using System.Collections;
using System;

public class CoinCarMovementController : MonoBehaviour 
{
    public CoinCar Car;

    public Rigidbody2D Rigidbody;
    public CircleCollider2D WanderZone;
    float _wanderAngle = 0;

    Vector2 _velocity;
    float _torque;

    public Vector2 Velocity { get { return _velocity; } }

    Vector2 _totalInput;

    bool _clampToMaxSpeed;

    IEnumerator _dashRoutine;

    bool _isDrifting;

    #region Event

    public Action<bool> OnDriftActive;

    void FireOnDriftActive(bool isActive)
    {
        if (OnDriftActive != null)
            OnDriftActive(isActive);
    }


    #endregion

    public void ActivateMovement()
    {
        _velocity = Vector2.zero;

        _clampToMaxSpeed = true;

        SeekRandomPos();
    }

    void SeekRandomPos()
    {
        Vector2 targetPos = GameArea.Instance.GetRandomPosInEffectiveGameArea();

        Vector2 dir = targetPos - Rigidbody.position;
        dir.Normalize();

        targetPos = Rigidbody.position + dir * CarSettings.Instance.MaxSpeed;

        Seek(targetPos);
    }

    #region Seek

    public void SeekByInput(Vector2 inputDir)
    {
        _totalInput += inputDir;
    }

    public void Seek(Vector2 targetPos, float slowingRadius = CarSettings.SlowingRadius)
    {
        _totalInput += DoSeek(targetPos, slowingRadius);
    }

    Vector2 DoSeek(Vector2 targetPos, float slowingRadius = CarSettings.SlowingRadius)
    {
        Vector2 inputDir;

        Vector2 desiredDir = targetPos - Rigidbody.position;

        desiredDir.Normalize();

        float angleBetUp = transform.right.AngleBetween(desiredDir);

        if (Mathf.Abs(angleBetUp) < 5)
        {
            inputDir = Vector2.zero;
            return inputDir;
        }

        if (angleBetUp > 0)
            inputDir = new Vector2(1, 0);
        else
            inputDir = new Vector2(-1, 0);

        return inputDir;
    }

    #endregion
}
