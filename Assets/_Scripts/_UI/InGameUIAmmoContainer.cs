using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class WeaponIconInfo
{
    public Sprite Sprite;
    public WeaponTypeEnum WeaponType;
}

public class InGameUIAmmoContainer : MonoBehaviour
{
    static InGameUIAmmoContainer _instance;

    public static InGameUIAmmoContainer Instance { get { return _instance; } }

    public Image WeaponIconImage;
    public Text AmmoCountLabel;

    public List<WeaponIconInfo> WeaponIconList;

    void Awake()
    {
        _instance = this;
    }

    void LateUpdate()
    {
        CarScript targetCar = CarManagerBase.BaseInstance.MyCar.Value;

        if (GameManagerBase.BaseInstance.IsInWatchMode)
        {
            targetCar = CameraFollowScript.Instance.TargetCar;

            if (targetCar != null)
                ChangeWeaponIcon(targetCar.WeaponSystemController.ActiveWeaponSystem.WeaponType);
        }


        if (targetCar != null && targetCar.WeaponSystemController.ActiveWeaponSystem != null)
        {
            AmmoCountLabel.text = targetCar.WeaponSystemController.ActiveWeaponSystem.AmmoCount.ToString();

            if (targetCar.WeaponSystemController.ActiveWeaponSystem.WeaponType == WeaponTypeEnum.Standard)
                AmmoCountLabel.text = "N/A";
        }
    }

    public void ChangeWeaponIcon(WeaponTypeEnum weaponType)
    {
        WeaponIconInfo iconInfo = WeaponIconList.Find(w => w.WeaponType == weaponType);

        if (iconInfo != null)
            WeaponIconImage.sprite = iconInfo.Sprite;

        WeaponIconImage.SetNativeSize();
    }
}
