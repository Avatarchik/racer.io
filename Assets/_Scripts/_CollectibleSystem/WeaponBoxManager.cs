using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class WeaponBoxInfo
{
    public WeaponTypeEnum WeaponBoxType;
    public int MinAmmoCount;
    public int MaxAmmoCount;
}

public class WeaponBoxManager : SpawnableManagerBase
{
    public List<WeaponBoxInfo> WeaponBoxInfoList;

    public tk2dSpriteCollectionData WeaponBoxAtlasData;

    int _maxWeaponIndex;

    const string SPRITE_NAME_POSTFIX = "_WeaponBox";

    Dictionary<WeaponTypeEnum, int> _boxSpriteIDDict;

    protected override void Awake()
    {
        InitMaxWeaponIndex();
        InitWeaponBoxSpriteDict();

        base.Awake();
    }

    void InitMaxWeaponIndex()
    {
        _maxWeaponIndex = Enum.GetValues(typeof(WeaponTypeEnum)).Cast<int>().Max();
    }

    void InitWeaponBoxSpriteDict()
    {
        _boxSpriteIDDict = new Dictionary<WeaponTypeEnum, int>();

        for (int i = 0; i <= _maxWeaponIndex; i++)
        {
            string spriteName = ((WeaponTypeEnum)i).ToString() + SPRITE_NAME_POSTFIX;

            _boxSpriteIDDict.Add(((WeaponTypeEnum)i), WeaponBoxAtlasData.GetSpriteIdByName(spriteName));
        }
    }

    protected override void SpawnSpawnable()
    {
        if (_deactiveSpawnableList.Count == 0)
            return;

        SpawnableBase targetCol = _deactiveSpawnableList[0];

        Vector3 spawnPos = GetSpawnPos();

        WeaponTypeEnum weaponBoxType = GetRandWeaponType();

        AddToActiveList(targetCol);

        ((CollectibleWeaponBox)targetCol).SetWeaponBox(weaponBoxType, _boxSpriteIDDict[weaponBoxType], GetAmmoCount(weaponBoxType));

        targetCol.Activate(spawnPos);
    }

    WeaponTypeEnum GetRandWeaponType()
    {
        return (WeaponTypeEnum)Utilities.NextInt(1, _maxWeaponIndex + 1);
    }

    int GetAmmoCount(WeaponTypeEnum weaponBoxType)
    {
        WeaponBoxInfo wpi = WeaponBoxInfoList.Single(val => val.WeaponBoxType == weaponBoxType);

        return Utilities.NextInt(wpi.MinAmmoCount, wpi.MaxAmmoCount);
    }
}
