using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissileAmmo : AmmoBase
{
    public float MissileSearchRange;
    public float MaxRotation;

    CarBase _targetCar;

    bool _isLockedToTarget;

    Vector2 _velocity;
    Vector2 _steering;

    protected override IEnumerator MoveProgress()
    {
        AmmoSound.Play();

        float trailTime = TrailRenderer.time;
        TrailRenderer.time = 0f;

        yield return new WaitForFixedUpdate();

        TrailRenderer.time = trailTime;

        float distanceTaken = 0;
        Vector2 prevPos = (Vector2)transform.position;

        while (distanceTaken < _parentWeapon.WeaponRange && !_hitCar)
        {
            if (_targetCar != null
                && !GeneralCarManager.Instance.ActiveCarBaseList.Contains(_targetCar))
            {
                _isLockedToTarget = false;
                _targetCar = null;
            }

            if (_targetCar == null)
                CheckTargetCar();


            Seek(GetTargetPos(), 0);
            CalculateFinalVelocity();
            UpdatePosition();
            UpdateRotation();

            distanceTaken += Vector2.Distance(transform.position, prevPos);

            prevPos = (Vector2)transform.position;

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(DeactivateProgress());
    }

    Vector3 GetTargetPos()
    {
        Vector3 targetPos = transform.position;

        if (_targetCar == null)
            targetPos += (Vector3)_velocity.normalized * _parentWeapon.AmmoSpeed;
        else
            targetPos = _targetCar.transform.position;

        return targetPos;
    }

    public void Seek(Vector2 targetPos, float slowingRadius = CarSettings.SlowingRadius)
    {
        _steering += DoSeek(targetPos, slowingRadius);
    }

    Vector2 DoSeek(Vector2 targetPos, float slowingRadius = CarSettings.SlowingRadius)
    {
        Vector2 force;
        float distance;

        Vector2 desired = targetPos - (Vector2)transform.position;
        distance = desired.magnitude;

        if (distance <= slowingRadius)
        {
            desired = desired.normalized * _parentWeapon.AmmoSpeed * distance / slowingRadius;
        }
        else
            desired = desired.normalized * _parentWeapon.AmmoSpeed;

        force = desired - _velocity;

        return force;
    }

    void CalculateFinalVelocity()
    {
        _steering = Vector2.ClampMagnitude(_steering, CarSettings.Instance.PlayerAcc);

        if (_velocity.magnitude != 0)
        {
            float angDiff = _velocity.AngleBetween(_steering);

            if (Mathf.Abs(angDiff) > MaxRotation)
            {
                Vector2 newDir = _velocity.normalized.Rotate(MaxRotation * -Mathf.Sign(angDiff));

                _steering = newDir * _steering.magnitude;
            }
        }

        _velocity = _velocity + _steering;

        _velocity = Vector2.ClampMagnitude(_velocity, _parentWeapon.AmmoSpeed);

        _steering = Vector2.zero;
    }

    void UpdatePosition()
    {
        Rigidbody.position += _velocity * Time.deltaTime;
    }

    void UpdateRotation()
    {
        float rot_z = Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg;

        Rigidbody.rotation = rot_z;
    }

    void CheckTargetCar()
    {
        if (_isLockedToTarget)
            return;

        List<CarBase> activeCarList = GeneralCarManager.Instance.ActiveCarBaseList;

        foreach (CombatCarScript car in activeCarList)
        {
            if (car == _parentWeapon.WeaponController.MyCar
                || car.IsInGhostMode)
                continue;

            if (Vector2.Distance(car.transform.position, transform.position) < MissileSearchRange)
            { 
                _isLockedToTarget = true;
                _targetCar = car;
                break;
            }
        }
    }
}
