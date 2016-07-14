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

    public static RandomGiftMachineManager Instance { get { return _instance; } }

    static RandomGiftMachineManager _instance;

    static int[] _randomColors;

    static List<GiftPackage> _packages;


    void Awake()
    {
        _packages = new List<GiftPackage>();
        _randomColors = new int[Constants.RANDOM_GIFT_AMOUNT];
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void OnEnable()
    {
        InitPackages();
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

    static void SelectRandomPackage(int amount)
    {
        _randomColors = Utilities.RandomIntinAmount(amount, 0, 5);
    }

    void InitPackages()
    {
        _packages.AddRange(GetComponentsInChildren<GiftPackage>(true));
        /*for (int i = 0; i < amount; i++)
        {
            GameObject giftObject = Object.Instantiate(GiftPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            Vector3 localScale = giftObject.transform.localScale;
            giftObject.transform.parent = transform;
            giftObject.transform.localScale = localScale;//Vector3.one;
            _packages.Add(giftObject.GetComponent<GiftPackage>());
        }*/

    }

    public void OpenSelection()
    {
        SelectRandomPackage(Constants.RANDOM_GIFT_AMOUNT);
        for (int i = 0, _packagesCount = _packages.Count; i < _packagesCount; i++)
        {
            GiftPackage package = _packages[i];
            package.gameObject.SetActive(true);
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
