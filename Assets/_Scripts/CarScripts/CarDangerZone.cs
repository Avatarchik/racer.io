using UnityEngine;
using System.Collections;
using System;

public class CarDangerZone : MonoBehaviour
{
    public event Action<CarScript, bool> OnAttackDetectedEvent;

    void FireOnAttackDetectedEvent(CarScript plane, bool isEntered)
    {
        if (OnAttackDetectedEvent != null)
            OnAttackDetectedEvent(plane, isEntered);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ProcessCollision(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        ProcessCollision(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == (int)LayerEnum.Bullet)
        {
            BulletScript bullet = other.GetComponent<BulletScript>();

            FireOnAttackDetectedEvent(bullet.WeaponController.Car, false);
        }
        else if (other.gameObject.layer == (int)LayerEnum.CarDangerZone)
        {
            CarScript plane = other.transform.parent.GetComponent<CarScript>();

            FireOnAttackDetectedEvent(plane, false);
        }
    }

    void ProcessCollision(Collider2D other)
    {
        if (other.gameObject.layer == (int)LayerEnum.Bullet)
        {
            BulletScript bullet = other.GetComponent<BulletScript>();

            FireOnAttackDetectedEvent(bullet.WeaponController.Car, true);
        }
        else if (other.gameObject.layer == (int)LayerEnum.CarDangerZone)
        {
            CarScript plane = other.transform.parent.GetComponent<CarScript>();

            FireOnAttackDetectedEvent(plane, true);
        }
    }
}
