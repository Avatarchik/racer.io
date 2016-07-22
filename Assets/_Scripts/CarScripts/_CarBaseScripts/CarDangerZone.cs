using UnityEngine;
using System.Collections;
using System;

public class CarDangerZone : MonoBehaviour
{
    public event Action<CarBase, bool> OnAttackDetectedEvent;

    void FireOnAttackDetectedEvent(CarBase car, bool isEntered)
    {
        if (OnAttackDetectedEvent != null)
            OnAttackDetectedEvent(car, isEntered);
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
            AmmoBase ammo = other.GetComponent<AmmoBase>();

            FireOnAttackDetectedEvent(ammo.ParentWeapon.WeaponController.MyCar, false);
        }
        else if (other.gameObject.layer == (int)LayerEnum.CarDangerZone)
        {
            CombatCarScript plane = other.transform.parent.GetComponent<CombatCarScript>();

            FireOnAttackDetectedEvent(plane, false);
        }
    }

    void ProcessCollision(Collider2D other)
    {
        if (other.gameObject.layer == (int)LayerEnum.Bullet)
        {
            AmmoBase ammo = other.GetComponent<AmmoBase>();

            FireOnAttackDetectedEvent(ammo.ParentWeapon.WeaponController.MyCar, true);
        }
        else if (other.gameObject.layer == (int)LayerEnum.CarDangerZone)
        {
            CombatCarScript plane = other.transform.parent.GetComponent<CombatCarScript>();

            FireOnAttackDetectedEvent(plane, true);
        }
    }
}
