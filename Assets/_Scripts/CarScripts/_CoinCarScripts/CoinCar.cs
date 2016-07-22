using UnityEngine;
using System.Collections;

public class CoinCar : CarBase
{
    public override void GetKilled(DestroyReasonType reason, CarBase otherCar)
    {
        base.GetKilled(reason, otherCar);

        CoinCarManager.Instance.CoinCarKilled();
    }
}
