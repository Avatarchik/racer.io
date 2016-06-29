using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActiveCarUI : MonoBehaviour
{
    static ActiveCarUI _instance;

    public static ActiveCarUI Instance { get { return _instance; } }

    public Text ActiveCarName;
    public Image ActiveCarImage;

    void Awake()
    {
        _instance = this;

        CarSelectionWindowController.OnCarSelectionUpdate += ChangeActiveCar;
    }

    void OnDestroy()
    {
        _instance = null;

        CarSelectionWindowController.OnCarSelectionUpdate -= ChangeActiveCar;
    }

    public void ChangeActiveCar(CarSlot activeCar)
    {
        ActiveCarImage.sprite = activeCar.CarSprite.sprite;
        ActiveCarImage.SetNativeSize();

        ActiveCarName.text = CarSelectionWindowController.Instance.GetCarName(activeCar.CarType);
    }
}
