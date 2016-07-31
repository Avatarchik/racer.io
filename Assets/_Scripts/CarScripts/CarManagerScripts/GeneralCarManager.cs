using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GeneralCarManager : MonoBehaviour
{
    static GeneralCarManager _instance;

    public static GeneralCarManager Instance{ get { return _instance; } }

    List<CarBase> _activeCarBaseList;

    public List<CarBase> ActiveCarBaseList{ get { return _activeCarBaseList; } }

    void Awake()
    {
        _instance = this;

        InitActiveCarList();
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void InitActiveCarList()
    {
        _activeCarBaseList = new List<CarBase>();
    }

    public void RegisterToActiveCarList(CarBase car)
    {
        _activeCarBaseList.Add(car);
    }

    public void UnregisterFromActiveCarList(CarBase car)
    {
        _activeCarBaseList.Remove(car);
    }

    public void FixedUpdateFrame()
    {
        UpdateActiveCars();
    }

    void UpdateActiveCars()
    {
        List<CarBase> tempActiveCarList = _activeCarBaseList.ToList();
        
        foreach (CarBase car in tempActiveCarList)
        {
            car.FixedUpdateFrame();

            CheckBordersOfCar(car);
        }
    }

    void CheckBordersOfCar(CarBase car)
    {
        car.EnteredAbandonArea = CheckEnteredAbondonArea(car);
        car.EnteredWarningArea = CheckEnteredWarningArea(car);

        if (car.EnteredAbandonArea)
        {
            car.DecreaseHealth(car.CurHealth);

            car.GetKilled(DestroyReasonType.AbandonArea, null);

            if (car.IsPlayerCar)
                MessagingSystem.Instance.WriteLeaveMessage(car);
        }
        else if (car.EnteredWarningArea)
        {
            if (car.IsPlayerCar)
                MessagingSystem.Instance.WriteLeaveMessage(car);
        }
    }


    public CarBase GetRandomActiveCar()
    {
        return _activeCarBaseList[Utilities.NextInt(0, _activeCarBaseList.Count)];
    }

    public bool CheckEnteredAbondonArea(CarBase car)
    {
        return (!GameArea.Instance.IsInAbandonArea(car.transform.position)
        && !car.EnteredAbandonArea);
    }

    public bool CheckEnteredWarningArea(CarBase car)
    {
        return (!GameArea.Instance.IsInWarningArea(car.transform.position)
        && !car.EnteredWarningArea);
    }
}
