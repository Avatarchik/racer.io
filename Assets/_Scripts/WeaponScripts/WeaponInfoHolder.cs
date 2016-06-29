using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WeaponInfo
{
    public WeaponTypeEnum WeaponType;
    public GameObject BulletPrefab;
    public Color CarTrailColor;
    //Rate of fire : RPS(Rounds/s)
    public int RateOfFire, BulletSpeed, FireDistance, WeaponDamage;
}

public class WeaponInfoHolder : MonoBehaviour
{
    static WeaponInfoHolder _instance;

    public static WeaponInfoHolder Instance { get { return _instance; } }

    public List<WeaponInfo> WeaponInfoList;

    void Awake()
    {
        _instance = this;
    }

    public WeaponInfo GetWeaponInfo(WeaponTypeEnum weaponType)
    {
        var targetWeaponInfo = WeaponInfoList.Find(w => w.WeaponType == weaponType);

        if (targetWeaponInfo == null)
            return GetWeaponInfo(WeaponTypeEnum.Standard);

        return targetWeaponInfo;
    }
}
