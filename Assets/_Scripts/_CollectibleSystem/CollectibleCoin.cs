using UnityEngine;
using System.Collections;

public class CollectibleCoin : CollectibleBase
{
    public override void Use(CarScript car)
    {
        if (car.ControllerType != CarControllerType.Player)
            return;

        CurrencyManager.Instance.GainCurrency(CurrencyType.Coin, 1);
    }
}
