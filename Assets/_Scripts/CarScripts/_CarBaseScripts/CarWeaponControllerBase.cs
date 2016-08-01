using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CarWeaponControllerBase : MonoBehaviour
{
    public CarBase MyCar;

    public List<WeaponBase> WeaponList;

    WeaponBase _curWeapon;

    public WeaponBase CurWeapon{ get { return _curWeapon; } }

    public void SetNewWeapon(WeaponTypeEnum weaponType, int ammoCount)
    {
        WeaponBase targetWeapon = WeaponList.Single(val => val.WeaponType == weaponType);

        if (_curWeapon != null)
            _curWeapon.DeactivateWeapon();

        _curWeapon = targetWeapon;

        _curWeapon.ActivateWeapon(ammoCount);

        InGameUIAmmoContainer.Instance.ChangeWeaponIcon(_curWeapon.WeaponType);
    }

    public void Fire()
    {
        if (_curWeapon == null)
            return;

        _curWeapon.FireWeapon();
    }
}
