using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WeaponController : MonoBehaviour
{
    public WeaponTypeEnum WeaponType;
    public CarScript Car;
    public Transform AmmoPoolParent;
    public List<Transform> AmmoSlot;

    [HideInInspector]
    public int AmmoCount;
    [HideInInspector]
    public WeaponInfo WeaponInfo;

    List<BulletScript> _deactiveBulletList = new List<BulletScript>(), _activeBulletList = new List<BulletScript>();

    bool _isFiring;

    public bool IsFiring { get { return _isFiring; } }

    IEnumerator _fireRoutine;

    /// <summary>
    /// This method is ONLY called from WeaponSystemController at the beginning of the game, DONT CALL it from anywhere else.
    /// </summary>
    public void Init()
    {
        _fireRoutine = null;

        WeaponInfo = WeaponInfoHolder.Instance.GetWeaponInfo(WeaponType);

        float rps = WeaponInfo.RateOfFire / 60f;
        int maxAmmoOnScreen = Mathf.CeilToInt((float)WeaponInfo.FireDistance / WeaponInfo.BulletSpeed * rps) * AmmoSlot.Count;

        for (int i = 0; i < maxAmmoOnScreen; i++)
        {
            GameObject bullet = GameObject.Instantiate(WeaponInfo.BulletPrefab, Vector2.zero, Quaternion.identity) as GameObject;
            BulletScript bulletScript = bullet.GetComponent<BulletScript>();

            AddBulletToDeactiveList(bulletScript);

            bulletScript.Deactivate();
            bulletScript.transform.parent = AmmoPoolParent;
            bulletScript.transform.localPosition = Vector2.zero;
        }
    }

    public void AddBulletToDeactiveList(BulletScript bulletScript)
    {
        _activeBulletList.Remove(bulletScript);
        _deactiveBulletList.Add(bulletScript);
    }

    public void AddBulletToActiveList(BulletScript bulletScript)
    {
        _deactiveBulletList.Remove(bulletScript);
        _activeBulletList.Add(bulletScript);
    }

    public void Activate(int ammoCount)
    {
        gameObject.SetActive(true);

        Car.ChangeTrailColor(WeaponInfo.CarTrailColor);

        AmmoCount = ammoCount;

        if (PlayerCarFireController.Instance != null && PlayerCarFireController.Instance.IsPressed)
            PlayerCarFireController.Instance.FireCarWeapon();

        if (!gameObject.activeInHierarchy)
            return;

        if (_fireRoutine == null)
        {
            _fireRoutine = FireRoutine();
            StartCoroutine(_fireRoutine);
        }
    }

    public void Deactivate()
    {
        if (_fireRoutine != null)
        {
            StopCoroutine(_fireRoutine);
            _fireRoutine = null;
        }

        gameObject.SetActive(false);
    }

    public void Fire()
    {
        _isFiring = true;

        if (_fireRoutine == null)
        {
            _fireRoutine = FireRoutine();
            StartCoroutine(_fireRoutine);
        }
    }

    public void CeaseFire()
    {
        _isFiring = false;

        if (_fireRoutine != null)
        {
            StopCoroutine(_fireRoutine);
            _fireRoutine = null;
        }
    }

    IEnumerator FireRoutine()
    {
        while (true)
        {
            if (_isFiring)
            {
                foreach (var ammoSlot in AmmoSlot)
                {
                    if (_deactiveBulletList.Count == 0)
                        break;

                    _deactiveBulletList[0].Activate(this, ammoSlot, Mathf.CeilToInt(Car.MovementController.Velocity.magnitude) + WeaponInfo.BulletSpeed, WeaponInfo.WeaponDamage);
                    AddBulletToActiveList(_deactiveBulletList[0]);

                    AmmoCount--;

                    if (AmmoCount < 0)
                        AmmoCount = 0;
                }

                yield return new WaitForSeconds(60f / WeaponInfo.RateOfFire);
            }
            else
                yield return new WaitForFixedUpdate();
        }
    }
}
