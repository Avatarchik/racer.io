using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoinBarMainMenu : MonoBehaviour
{
    static CoinBarMainMenu _instance;

    public static CoinBarMainMenu Instance{ get { return _instance; } }

    public Text CoinText;

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void UpdateCoinBar()
    {
        CoinText.text = CoinCurrencyManager.Instance.CurCoinAmount.ToString();
    }

}
