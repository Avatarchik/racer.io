using UnityEngine;
using System.Collections;

public class CollectibleWeaponBox : CollectibleBase
{
    public tk2dSprite BoxSprite;

    WeaponTypeEnum _weaponBoxType;

    int _ammoCount;

    public void SetWeaponBox(WeaponTypeEnum weaponBoxType, int spriteID, int ammoCount)
    {
        _weaponBoxType = weaponBoxType;
        BoxSprite.spriteId = spriteID;
        _ammoCount = ammoCount;
    }

    public override void Use(CarBase car)
    {
        if (car.CarBaseType != CarBaseType.CombatCar)
            return;

        ((CombatCarScript)car).WeaponController.SetNewWeapon(_weaponBoxType, _ammoCount);
    }
}
