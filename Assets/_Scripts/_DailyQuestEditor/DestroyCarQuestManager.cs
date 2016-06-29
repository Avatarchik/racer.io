using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class DestroyCarQuestManager : QuestCreationUIBase
{
    public Dropdown DestroyReasonSelectionDD;
    public Dropdown WeaponSelectionDD;

    DestroyReasonType _selectedReason;
    WeaponTypeEnum _selectedWeapon;

    public override void ActivateUI()
    {
        InitDestroyReasonSelectionDD();
        InitWeaponSelectionDD();

        gameObject.SetActive(true);
    }

    void InitDestroyReasonSelectionDD()
    {
        List<string> destroyReasonStringList = new List<string>();

        foreach (DestroyReasonType reasonType in Enum.GetValues(typeof(DestroyReasonType)))
        {
            destroyReasonStringList.Add(reasonType.ToString());
        }

        DestroyReasonSelectionDD.ClearOptions();
        DestroyReasonSelectionDD.AddOptions(destroyReasonStringList);

        _selectedReason = (DestroyReasonType)DestroyReasonSelectionDD.value;
    }

    void InitWeaponSelectionDD()
    {
        List<string> weaponStringList = new List<string>();

        foreach (WeaponTypeEnum reasonType in Enum.GetValues(typeof(WeaponTypeEnum)))
        {
            weaponStringList.Add(reasonType.ToString());
        }

        WeaponSelectionDD.ClearOptions();
        WeaponSelectionDD.AddOptions(weaponStringList);

        _selectedWeapon = (WeaponTypeEnum)WeaponSelectionDD.value;
    }

    public void DestroyTypeChanged()
    {
        _selectedReason = (DestroyReasonType)DestroyReasonSelectionDD.value;
    }

    public void WeaponTypeChanged()
    {
        _selectedWeapon = (WeaponTypeEnum)WeaponSelectionDD.value;
    }

    public override DailyQuestInfo GetQuestInfo(int questID)
    {
        DailyQuestInfo questInfo = new DailyQuestInfo(
                                       questID,
                                       DailyQuestType.DestroyPlane,
                                       QuestSelectionManager.Instance.SelectedDurationType,
                                       QuestSelectionManager.Instance.SelectedGameMode,
                                       QuestSelectionManager.Instance.RequiredAmount,
                                       _selectedReason.ToString(),
                                       _selectedWeapon.ToString());

        return questInfo;

    }
}
