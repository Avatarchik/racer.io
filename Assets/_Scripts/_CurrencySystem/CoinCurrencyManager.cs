using UnityEngine;
using System.Collections;
using System;

public class CoinCurrencyManager : MonoBehaviour
{
    static CoinCurrencyManager _instance;

    public static CoinCurrencyManager Instance { get { return _instance; } }

    int _curCoinAmount;

    public int CurCoinAmount { get { return _curCoinAmount; } }

    int _cachedCoinAmount;

    public int CachedCoinAmount { get { return _cachedCoinAmount; } }

    public const string COIN = "coin";

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void LoadCoins()
    {
        if (!PlayerPrefs.HasKey(COIN))
        {
            _curCoinAmount = 20;
            SaveCoins();
        }
        else
        {
            _curCoinAmount = PlayerPrefs.GetInt(COIN);


            if (_curCoinAmount < 0)
                _curCoinAmount = 50;

            SaveCoins();
        }

        _cachedCoinAmount = _curCoinAmount;
    }

    void SaveCoins()
    {
        if (_curCoinAmount < 0)
            _curCoinAmount = 0;

        PlayerPrefs.SetInt(COIN, _curCoinAmount);
    }

    public bool CanAffordCoinCost(int coinCost)
    {
        if (_cachedCoinAmount >= coinCost)
            return true;

        return false;
    }

    public bool UseCoin(int coinCost)
    {
        if (CanAffordCoinCost(coinCost))
        {
            _curCoinAmount -= coinCost;
            _cachedCoinAmount = _curCoinAmount;

            SaveCoins();

            return true;
        }
        else
        {
            return false;
        }
    }

    public void GainCoin(int amount)
    {
        _curCoinAmount += amount;
        _cachedCoinAmount = _curCoinAmount;

        SaveCoins();
    }

    public bool CacheCoinUsage(int amount)
    {
        if (!CanAffordCoinCost(amount))
            return false;

        _cachedCoinAmount -= amount;

        return true;
    }

    public void UncacheCoinUsage(int amount)
    {
        _cachedCoinAmount += amount;
    }

    public void CheckoutCachedCoin()
    {
        _curCoinAmount = _cachedCoinAmount;

        SaveCoins();
    }
}
