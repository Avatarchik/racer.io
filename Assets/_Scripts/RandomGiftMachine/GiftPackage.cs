using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;


public class GiftPackage : EventTrigger
{
    public event Action<GiftPackage> GiftSelected;

    Button _button;

    void Awake()
    {
        gameObject.SetActive(false);
        _button = GetComponent<Button>();
        _button.interactable = true;
    }

    public void SetColorAndScale(RandomGiftMachineManager.RandomGiftColor color)
    {
        gameObject.SetActive(true);
        //TODO:Set color
    }

    void OnEnable()
    {
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.5f, 5, 0.6f);
    }

    public override void OnSelect(BaseEventData data)
    {
        SelectGift();
    }

    public void SelectGift()
    {
        if (GiftSelected != null)
            GiftSelected(this);

        transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.5f)
            .SetEase(Ease.OutCubic);

        StartCoroutine(UnlockRandomCar());
        Debug.Log("Animasyonlar felan");
    }

    public void OtherGiftSelected()
    {
        _button.interactable = false;

        transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.5f)
            .SetEase(Ease.OutCubic);
    }

    IEnumerator UnlockRandomCar()
    {
        yield return new WaitForSeconds(1);
        PlayerProfile.Instance.UnlockCar(Utilities.GetRandomEnum<CarTypeEnum>());
    }
}
