using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class WeaponInfoBase
{
    [SerializeField]
    public WeaponTypeEnum WeaponType;
    public int WeaponDamage;
    public float WeaponCooldown;
    public int InitAmmoCount;
}

[System.Serializable]
public class Weapon_AmmoBasedInfo : WeaponInfoBase
{
    public float AmmoSpeed;
    public float WeaponRange;
}

public class WeaponInfoContoller : MonoBehaviour 
{
    static WeaponInfoContoller _instance;

    public static WeaponInfoContoller Instance { get { return _instance; } }

    public List<Weapon_AmmoBasedInfo> Weapon_AmmoBasedInfoList;

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public Weapon_AmmoBasedInfo GetWeaponAmmoBasedInfo(WeaponTypeEnum weaponType)
    {
        return Weapon_AmmoBasedInfoList.Single(val => val.WeaponType == weaponType);
    }
}
