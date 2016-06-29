using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponSystemController : MonoBehaviour
{
    public CarScript CarScript;

    public List<WeaponController> WeaponControllerList = new List<WeaponController>();

    [HideInInspector]
    public WeaponController ActiveWeaponSystem;

    public void Awake()
    {
        WeaponControllerList.ForEach(w => w.Init());
    }

    public void Update()
    {
        if (CarScript.IsInGhostMode)
            return;

        if (ActiveWeaponSystem != null && ActiveWeaponSystem.WeaponType != WeaponTypeEnum.Standard && ActiveWeaponSystem.AmmoCount == 0)
            ActivateWeaponSystem(WeaponTypeEnum.Standard, 0);
    }

    public void ActivateWeaponSystemInput()
    {
        if (CarScript.IsPlayerCar)
            PlayerCarFireController.Instance.SetPlayerMovementScript(this);
    }

    public void DeactivateWeaponSystemInput()
    {
        if (ActiveWeaponSystem == null)
            return;
        
        if (CarScript.IsPlayerCar)
        {
            PlayerCarFireController.Instance.CeaseCarWeapon();

            PlayerCarFireController.Instance.SetPlayerMovementScript(null);
        }
        else
            ActiveWeaponSystem.CeaseFire();
    }

    public void ActivateWeaponSystem(WeaponTypeEnum weaponType, int ammoCount)
    {
        WeaponController targetSystem = WeaponControllerList.Find(w => w.WeaponType == weaponType);

        if (ActiveWeaponSystem != null)
            ActiveWeaponSystem.Deactivate();

        if (targetSystem != null)
            ActiveWeaponSystem = targetSystem;

        ActiveWeaponSystem.Activate(ammoCount);

        if (CarScript.IsPlayerCar)
        {
            InGameUIAmmoContainer.Instance.ChangeWeaponIcon(ActiveWeaponSystem.WeaponType);
            CarScript.SoundController.PlayCollectWeaponSound();
        }

        if (SinglePlayerArenaGameManager.Instance.IsInWatchMode && CameraFollowScript.Instance.TargetCar == CarScript)
        {
            InGameUIAmmoContainer.Instance.ChangeWeaponIcon(ActiveWeaponSystem.WeaponType);
        }
    }
}
