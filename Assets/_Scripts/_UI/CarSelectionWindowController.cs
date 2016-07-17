using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public enum CarColorEnum
{
    Red,
    Blue,
    Green,
    Purple,
    Cyan,
}

public enum CarTypeEnum
{
    BigFoot,
    Buggy,
    Bulky,
    Fury,
    MrGrim,
    Spark,
    Speedy,
    Talon,
}

[System.Serializable]
public class CarSlot
{
    public Image CarSprite;
    public Text CarNameText;
    public CarTypeEnum CarType;
    public GameObject LockImage;
}

[System.Serializable]
public class ColorInfo
{
    public CarColorEnum ColorType;
    public Color Color;
}

public class CarSelectionWindowController : MonoBehaviour
{
    public List<Image> ColorSlotList;
    public List<CarSlot> CarSlotList;
    public List<ColorInfo> ColorInfoList;

    public List<Sprite> CarSpriteList;

    [HideInInspector]
    public string Username;
    [HideInInspector]
    public CarColorEnum SelectedCarColor;
    [HideInInspector]
    public CarTypeEnum SelectedCarType;

    static CarSelectionWindowController _instance;

    public static CarSelectionWindowController Instance { get { return _instance; } }

    public bool InitSuccessful;

    #region Events

    public static Action<CarSlot> OnCarSelectionUpdate;

    void FireOnCarSelectionUpdated(CarSlot planeSlot)
    {
        if (OnCarSelectionUpdate != null)
            OnCarSelectionUpdate.Invoke(planeSlot);
    }

    #endregion

    void Awake()
    {
        _instance = this;

        Username = "";

        Init();
    }

    void Init()
    {
        ShowColors();

        SetRandomCarAndColor();

        ShowCars();

        HighlightSelections();

        InitSuccessful = true;

    }

    void ShowColors()
    {
        for (int i = 0; i < ColorInfoList.Count; i++)
            ColorSlotList[i].color = ColorInfoList[i].Color;
    }

    void SetRandomCarAndColor()
    {
        var result = GetRandomCarAndColor();

        SelectedCarColor = result.Key;
        SelectedCarType = result.Value;
    }

    public KeyValuePair<CarColorEnum, CarTypeEnum> GetRandomCarAndColor()
    {
        int randomColorIndex = Utilities.NextInt(0, Enum.GetNames(typeof(CarColorEnum)).Length - 1);

        int randomCarIndex = Utilities.NextInt(0, Enum.GetNames(typeof(CarTypeEnum)).Length - 1);

        return new KeyValuePair<CarColorEnum, CarTypeEnum>((CarColorEnum)randomColorIndex, (CarTypeEnum)randomCarIndex);
    }

    public void ShowCars()
    {
        for (int i = 0; i < CarSlotList.Count; i++)
        {
            CarSlot slot = CarSlotList[i];

            string planeName = GetCarSpriteName(SelectedCarColor, CarSlotList[i].CarType);
            slot.CarSprite.sprite = CarSpriteList.Find(s => s.name == planeName);

            if (PlayerProfile.Instance.CheckIfCarUnlocked(slot.CarType))
                UnlockCar(slot);
            else
                LockCar(slot);

            CarSlotList[i].CarSprite.SetNativeSize();

            CarSlotList[i].CarNameText.text = GetCarName(slot.CarType);
        }
    }

    string GetCarSpriteName(CarColorEnum selectedCarColor, CarTypeEnum selectedCarType)
    {
        return selectedCarColor.ToString() + "_" + selectedCarType.ToString();
    }

    public string GetCarName(CarTypeEnum planeTypeEnum)
    {
        switch (planeTypeEnum)
        {
            case CarTypeEnum.BigFoot:
                return "Big Foot";
            case CarTypeEnum.Buggy:
                return "Buggy";
            case CarTypeEnum.Bulky:
                return "Bulky";
            case CarTypeEnum.Fury:
                return "Fury";
            case CarTypeEnum.MrGrim:
                return "Mr. Grim";
            case CarTypeEnum.Speedy:
                return "Speedy";
            case CarTypeEnum.Spark:
                return "Spark";
            case CarTypeEnum.Talon:
                return "Talon";
            default:
                return "";
        }
    }

    public void UnlockCarAfterInit(CarTypeEnum pte)
    {
        if (!InitSuccessful)
        {
            StartCoroutine(UnlockAfterInitReady(pte));
            return;
        }

        var slot = UnlockCar(pte);

        slot.CarSprite.SetNativeSize();
    }

    CarSlot UnlockCar(CarTypeEnum pte)
    {
        CarSlot slot = CarSlotList.Find(x => x.CarType == pte);

        slot.CarSprite.color = new Color(255f, 255f, 255f);

        slot.LockImage.SetActive(false);

        return slot;
    }

    void UnlockCar(CarSlot slot)
    {
        slot.CarSprite.color = new Color(255f, 255f, 255f);

        slot.LockImage.SetActive(false);
    }

    CarSlot LockCar(CarTypeEnum pte)
    {
        CarSlot slot = CarSlotList.Find(x => x.CarType == pte);

        slot.CarSprite.color = new Color(0f, 0f, 0f);

        slot.LockImage.SetActive(true);

        return slot;
    }

    void LockCar(CarSlot slot)
    {
        slot.CarSprite.color = new Color(0f, 0f, 0f);

        slot.LockImage.SetActive(true);
    }

    public IEnumerator UnlockAfterInitReady(CarTypeEnum pte)
    {
        while (!InitSuccessful)
        {
            yield return new WaitForEndOfFrame();
        }
        UnlockCarAfterInit(pte);
    }

    public void SetCarSelection(Image selectedCarImage)
    {
        CarSlot selectedSlot = CarSlotList.Find(x => x.CarSprite == selectedCarImage);

        if (selectedSlot != null && !PlayerProfile.Instance.CheckIfCarUnlocked(selectedSlot.CarType))
            return;

        SelectedCarType = selectedSlot.CarType;

        HighlightSelections();
    }

    public void SetCarColorSelection(Image colorSelection)
    {
        SelectedCarColor = ColorInfoList.Find(c => c.Color == colorSelection.color).ColorType;

        ShowCars();

        HighlightSelections();
    }

    void HighlightSelections()
    {
        //Color highlight
        Image colorSelection = ColorSlotList.Find(s => s.color == ColorInfoList.Find(t => t.ColorType == SelectedCarColor).Color);
        ColorSlotList.ForEach(s => s.transform.GetChild(0).gameObject.SetActive(false));
        ColorSlotList.Find(s => s == colorSelection).transform.GetChild(0).gameObject.SetActive(true);

        string planeName = GetCarSpriteName(SelectedCarColor, SelectedCarType);
        Sprite selectedCar = CarSpriteList.Find(s => s.name == planeName);

        //Car highlight
        Image selectedCarImage = CarSlotList.Find(s => s.CarSprite.sprite == selectedCar).CarSprite;
        CarSlotList.ForEach(s => s.CarSprite.transform.GetChild(0).gameObject.SetActive(false));
        CarSlotList.Find(s => s.CarSprite == selectedCarImage).CarSprite.transform.GetChild(0).gameObject.SetActive(true);

        FireOnCarSelectionUpdated(CarSlotList.Find(s => s.CarSprite == selectedCarImage));
    }

    public void SetUsername(InputField inputField)
    {
        Username = inputField.text;
    }
}
