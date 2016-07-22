using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon_AmmoBased : WeaponBase
{
    public Transform AmmoCarrier;

    public List<Transform> AmmoSlotList;

    public GameObject AmmoPrefab;
    public int AmmoCount;
    public int MaxAmmoInGame;

    public float AmmoSpeed;

    public float WeaponRange;

    protected List<AmmoBase> _deactiveAmmoList;
    protected List<AmmoBase> _activeAmmoList;

    public override void InitWeapon()
    {
        base.InitWeapon();

        InitLists();
        GenerateAmmos();
    }

    void InitLists()
    {
        _deactiveAmmoList = new List<AmmoBase>();
        _activeAmmoList = new List<AmmoBase>();
    }

    void GenerateAmmos()
    {
        for (int i = 0; i < MaxAmmoInGame; i++)
        {
            GameObject tempAmmo = GameObject.Instantiate(AmmoPrefab, Vector3.zero, Quaternion.identity) as GameObject;

            tempAmmo.transform.parent = AmmoCarrier;

            AmmoBase ammo = tempAmmo.GetComponent<AmmoBase>();

            ammo.InitAmmo(this);
        }
    }

    protected override void Fire()
    {
        foreach (Transform ammoSlot in AmmoSlotList)
            SpawnAmmo(ammoSlot);
    }

    protected override void CheckRemAmmo()
    {
        if (_ammoCount <= 0
            || WeaponType != WeaponTypeEnum.Standard)
            WeaponController.SetNewWeapon(WeaponTypeEnum.Standard, 0);
    }

    protected virtual void SpawnAmmo(Transform ammoSlot)
    {
        if (_deactiveAmmoList.Count == 0)
            return;

        _ammoCount--;

        AmmoBase targetAmmo = _deactiveAmmoList[0];

        AddToActiveList(targetAmmo);

        targetAmmo.Activate(ammoSlot);
    }

    void AddToActiveList(AmmoBase ammo)
    {
        _deactiveAmmoList.Remove(ammo);

        _activeAmmoList.Add(ammo);
    }

    public void AddToDeactiveList(AmmoBase ammo)
    {
        _activeAmmoList.Remove(ammo);

        _deactiveAmmoList.Add(ammo);
    }
}
