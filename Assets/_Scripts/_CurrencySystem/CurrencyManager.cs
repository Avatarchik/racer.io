using UnityEngine;
using System.Collections;

public enum CurrencyType
{
    Coin,
}

public class CurrencyManager : MonoBehaviour
{
    static CurrencyManager _instance;

    public static CurrencyManager Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public bool CanAffordPrice(CurrencyType currencyType, int currencyAmount)
    {
        switch (currencyType)
        {
            case CurrencyType.Coin:
                return (CoinCurrencyManager.Instance.CanAffordCoinCost(currencyAmount));
        }

        return false;
    }

    public void CacheCurrency(CurrencyType currencyType, int currencyAmount)
    {
        if (!CanAffordPrice(currencyType, currencyAmount))
            return;

        switch (currencyType)
        {
            case CurrencyType.Coin:
                CoinCurrencyManager.Instance.CacheCoinUsage(currencyAmount);
                break;
        }
    }

    public void UncacheCurrency(CurrencyType currencyType, int currencyAmount)
    {
        switch (currencyType)
        {
            case CurrencyType.Coin:
                CoinCurrencyManager.Instance.UncacheCoinUsage(currencyAmount);
                break;
        }
    }

    public void CheckoutCachedCurrency(CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.Coin:
                CoinCurrencyManager.Instance.CheckoutCachedCoin();
                break;
        }
    }

    public void UseCurrency(CurrencyType currencyType, int currencyAmount)
    {
        if (!CanAffordPrice(currencyType, currencyAmount))
            return;

        switch (currencyType)
        {
            case CurrencyType.Coin:
                CoinCurrencyManager.Instance.UseCoin(currencyAmount);
                break;
        }
    }

    public void GainCurrency(CurrencyType currencyType, int currencyAmount)
    {
        switch (currencyType)
        {
            case CurrencyType.Coin:
                CoinCurrencyManager.Instance.GainCoin(currencyAmount);
                break;
        }

    }
}
