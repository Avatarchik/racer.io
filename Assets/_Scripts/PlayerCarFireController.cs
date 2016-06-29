using UnityEngine;
using System.Collections;

public class PlayerCarFireController : MonoBehaviour
{
    static PlayerCarFireController _instance;

    public static PlayerCarFireController Instance { get { return _instance; } }

    WeaponSystemController _weaponSystemController;

    [HideInInspector]
    public bool IsPressed;

    void Awake()
    {
        _instance = this;

        IsPressed = false;
    }

    public void SetPlayerMovementScript(WeaponSystemController weaponSystemController)
    {
        _weaponSystemController = weaponSystemController;
    }

    public void FireCarWeapon()
    {
        IsPressed = true;

        if (_weaponSystemController == null || _weaponSystemController.CarScript.IsInGhostMode)
            return;

        _weaponSystemController.ActiveWeaponSystem.Fire();
    }

    public void CeaseCarWeapon()
    {
        IsPressed = false;

        if (_weaponSystemController == null)
            return;

        _weaponSystemController.ActiveWeaponSystem.CeaseFire();
    }
}
