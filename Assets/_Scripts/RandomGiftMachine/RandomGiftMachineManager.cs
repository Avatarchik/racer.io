using UnityEngine;
using System.Collections.Generic;

public class RandomGiftMachineManager : MonoBehaviour
{
    public enum RandomGiftColor
    {
        red,
        blue,
        green,
        cyan,
        purple
    }

    int[] _randomColors;

    List<GiftPackage> _packages;

    public GameObject GiftPrefab;

    void Awake()
    {
        _packages = new List<GiftPackage>();
        _randomColors = new int[Constants.RANDOM_GIFT_AMOUNT];
        InitPackages(Constants.RANDOM_GIFT_AMOUNT);
        OpenSelection();

    }

    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnSubscribeEvents();
    }

    void SubscribeEvents()
    {
        foreach (GiftPackage gift in _packages)
        {
            gift.GiftSelected += OnGiftSelected;
        }
    }

    void UnSubscribeEvents()
    {
        foreach (GiftPackage gift in _packages)
        {
            gift.GiftSelected -= OnGiftSelected;
        }
    }

    void SelectRandomPackage(int amount)
    {
        _randomColors = Utilities.RandomIntinAmount(amount, 0, 5);
    }

    void InitPackages(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject giftObject = Object.Instantiate(GiftPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            Vector3 localScale = giftObject.transform.localScale;
            giftObject.transform.parent = transform;
            giftObject.transform.localScale = localScale;//Vector3.one;
            _packages.Add(giftObject.GetComponent<GiftPackage>());
        }
    }

    public void OpenSelection()
    {
        SelectRandomPackage(Constants.RANDOM_GIFT_AMOUNT);
        for (int i = 0, _packagesCount = _packages.Count; i < _packagesCount; i++)
        {
            GiftPackage package = _packages[i];
            package.SetColor((RandomGiftColor)_randomColors[i]);
        }
    }

    void OnGiftSelected(GiftPackage sender)
    {
        foreach (GiftPackage gift in _packages)
        {
            if (gift != sender)
                gift.OtherGiftSelected();
        }
    }
        
}
