using UnityEngine;
using System.Collections;

public abstract class WeaponBase : MonoBehaviour
{
    public CarWeaponControllerBase WeaponController;
    
    public WeaponTypeEnum WeaponType;

    public int WeaponDamage;

    public float WeaponCooldown;
    protected bool _isInCooldown;

    protected int _ammoCount;

    public int AmmoCount{ get { return _ammoCount; } }

    public virtual void InitWeapon()
    {
        
    }

    public virtual void ActivateWeapon(int ammoCount)
    {
        _ammoCount = ammoCount;
        
        _isInCooldown = false;
        
        gameObject.SetActive(true);
    }

    public virtual void DeactivateWeapon()
    {
        gameObject.SetActive(false);
    }

    public void FireWeapon()
    {
        if (_isInCooldown)
            return;
        
        Fire();

        StartCoroutine(CooldownProgress());
    }

    protected abstract void Fire();

    protected virtual void CheckRemAmmo()
    {
        if (_ammoCount <= 0)
            WeaponController.SetNewWeapon(WeaponTypeEnum.Standard, 0);
    }

    IEnumerator CooldownProgress()
    {
        _isInCooldown = true;
        
        yield return new WaitForSeconds(WeaponCooldown);

        _isInCooldown = false;
    }
}
