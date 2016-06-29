using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class WeaponDropInfo
{
    public WeaponTypeEnum WeaponType;
    public int MinDropChancePoint;
    public GameObject WeaponDropPrefab;
    public int MinAmmoCount, MaxAmmoCount;
}

public class WeaponDropManager : MonoBehaviour
{
    static WeaponDropManager _instance;
    public static WeaponDropManager Instance { get { return _instance; } }

    public float DropTransitionDuration, WeaponSpawnInterval, YOffset;
    public int WeaponPoolAmount, WeaponDropBonusPoint;
    public List<WeaponDropInfo> AcquirableWeaponPrefabList = new List<WeaponDropInfo>();

    List<WeaponAcquirable> _activeAcquirableWeaponList, _deactiveAcquirableWeaponList;
    float lastWeaponSpawnTime = 0f;

    void Awake()
    {
        _instance = this;

        InitWeaponManager();
    }

    void Start()
    {
        FillArea();
    }

    void FillArea()
    {
        float randomPassedTime = Utilities.NextFloat(10, 100);

        int spawnCount = Mathf.CeilToInt(randomPassedTime / WeaponSpawnInterval);

        if (spawnCount > _deactiveAcquirableWeaponList.Count)
            spawnCount = _deactiveAcquirableWeaponList.Count;

        for(int i = 0; i < spawnCount; i++)
            SpawnWeapon(false, true);
    }

    public void FixedUpdateFrame()
    {
        if(Time.realtimeSinceStartup - lastWeaponSpawnTime >= WeaponSpawnInterval)
        {
            lastWeaponSpawnTime = Time.realtimeSinceStartup;

            SpawnWeapon();
        }

        _activeAcquirableWeaponList.ForEach(w => w.Move());

        foreach (var weapon in _activeAcquirableWeaponList.ToList())
        {
            if (weapon.transform.position.y <= GameArea.Instance.GetBorderPos(GameAreaSideEnum.MinY) || weapon.IsAcquired)
            {
                weapon.Deactivate();

                AddWeaponToDeactivateAcquirableWeapon(weapon);
            }
        }
    }

    void SpawnWeapon(bool spawnAtTopBorderOfArea = true, bool isInstantSpawn = false)
    {
        List<WeaponDropInfo> possibilities = GetWeaponPossibilities();

        WeaponDropInfo targetWeapon = possibilities[0];

        if(possibilities.Count > 1)
        {
            int randomIndex = Random.Range(0, possibilities.Count);

            targetWeapon = possibilities[randomIndex];
        }

        WeaponAcquirable weapon = _deactiveAcquirableWeaponList.Find(w => w.WeaponType == targetWeapon.WeaponType);

        if(weapon != null)
        {
            Vector2 targetPos = GameArea.Instance.GetRandomPosInGameArea();

            if (spawnAtTopBorderOfArea)
                targetPos.y = GameArea.Instance.GetBorderPos(GameAreaSideEnum.MaxY);

            targetPos.y += YOffset;

            int ammoCount = Utilities.NextInt(targetWeapon.MinAmmoCount, targetWeapon.MaxAmmoCount);

            weapon.Activate(isInstantSpawn, targetPos, ammoCount);

            AddWeaponToActivateAcquirableWeapon(weapon);
        }
    }

    public void SpawnWeapon(WeaponTypeEnum weaponType, Vector2 spawnPos)
    {
        WeaponDropInfo weaponInfo = AcquirableWeaponPrefabList.Find(w => w.WeaponType == weaponType);
        WeaponAcquirable weapon = _deactiveAcquirableWeaponList.Find(w => w.WeaponType == weaponType);

        if(weapon != null)
        {
            int ammoCount = Utilities.NextInt(weaponInfo.MinAmmoCount, weaponInfo.MaxAmmoCount);

            weapon.Activate(true, spawnPos, ammoCount);

            AddWeaponToActivateAcquirableWeapon(weapon);
        }
    }

    List<WeaponDropInfo> GetWeaponPossibilities()
    {
        float random = Random.Range(0f, 100f);
        float targetProbability = 0f;

        foreach (var info in AcquirableWeaponPrefabList)
        {
            if (info.MinDropChancePoint <= random)
            {
                targetProbability = info.MinDropChancePoint;
                break;
            }
        }

        return AcquirableWeaponPrefabList.FindAll(p => p.MinDropChancePoint == targetProbability);
    }

    void InitWeaponManager()
    {
        _deactiveAcquirableWeaponList = new List<WeaponAcquirable>();
        _activeAcquirableWeaponList = new List<WeaponAcquirable>();

        AcquirableWeaponPrefabList = AcquirableWeaponPrefabList.OrderByDescending(p => p.MinDropChancePoint).ToList();

        foreach(var info in AcquirableWeaponPrefabList)
        {
            for(int i = 0; i < WeaponPoolAmount; i++)
            {
                GameObject weapon = GameObject.Instantiate(info.WeaponDropPrefab, Vector2.zero, Quaternion.identity) as GameObject;
                WeaponAcquirable weaponScript = weapon.GetComponent<WeaponAcquirable>();

                weaponScript.Init(transform);

                AddWeaponToDeactivateAcquirableWeapon(weaponScript);
            }
        }
    }

    public void AddWeaponToDeactivateAcquirableWeapon(WeaponAcquirable weapon)
    {
        _activeAcquirableWeaponList.Remove(weapon);
        _deactiveAcquirableWeaponList.Add(weapon);
    }

    public void AddWeaponToActivateAcquirableWeapon(WeaponAcquirable weapon)
    {
        _deactiveAcquirableWeaponList.Remove(weapon);
        _activeAcquirableWeaponList.Add(weapon);
    }

    public void Reset()
    {
        foreach (var weapon in _activeAcquirableWeaponList.ToList())
        {
            weapon.Deactivate();
            AddWeaponToDeactivateAcquirableWeapon(weapon);
        }
    }
}
