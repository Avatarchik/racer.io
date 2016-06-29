using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MissileBulletScript : BulletScript
{
    public float MissileSearchRange;
    public float MaxSpeed;
    public float MaxRotation;

    CarScript _targetCar;

    bool _isLockedToTarget;


    Vector2 _velocity;
    Vector2 _steering;


    
    protected override IEnumerator MoveRoutine(int BulletSpeed)
    {
        float distanceTaken = 0;

        Vector2 prevPos = (Vector2)transform.position;

        _targetCar = null;
        _isLockedToTarget = false;

        _velocity = transform.right * MaxSpeed;

        while (true)
        {
            if (_targetCar != null
                && !CarManagerBase.BaseInstance.ActiveCarDict.ContainsValue(_targetCar))
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

            if (distanceTaken > _weaponController.WeaponInfo.FireDistance
                || IsCarHit)
            {
                _weaponController.AddBulletToDeactiveList(this);

                Deactivate();
            }

            yield return new WaitForFixedUpdate();
        }
    }

    Vector3 GetTargetPos()
    {
        Vector3 targetPos = transform.position;
        
        if (_targetCar == null)
            targetPos += (Vector3)_velocity.normalized * MaxSpeed;
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
            desired = desired.normalized * MaxSpeed * distance / slowingRadius;
        }
        else
            desired = desired.normalized * MaxSpeed;

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

        _velocity = Vector2.ClampMagnitude(_velocity, MaxSpeed);

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

        List<CarScript> activeCarList = CarManagerBase.BaseInstance.ActiveCarDict.Values.ToList();

        foreach (CarScript car in activeCarList)
        {
            if (car == _weaponController.Car
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
