using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CarAIBase : MonoBehaviour
{
    protected AIDifficulty _aiDiff;

    public AIDifficulty AIDiff{ get { return _aiDiff; } }

    public CarBase MyCar;

    protected List<CarBase> _carsInSight;

    protected List<CarBase> _threatCarList;

    protected bool _isCriticalHealth;

    protected IEnumerator _moveRoutine;

    protected const float CHECK_OTHER_CARS_INTERVAL = 0.5f;

    public virtual void ActivateAI()
    {
        SetPilotDifficulty();

        StartListeningEvents();

        InitLists();

        StartCoroutine(CheckOtherCarsProgress());
        StartCoroutine(MakeDecisionProgress());
    }

    void SetPilotDifficulty()
    {
        int rand = Utilities.NextInt(1, (int)AIDifficulty.Hard + 1);

        _aiDiff = (AIDifficulty)rand;
    }

    public void DeactivateAI()
    {
        FinishListeningEvents();
    }

    void InitLists()
    {
        if (_carsInSight == null)
            _carsInSight = new List<CarBase>();
        else
            _carsInSight.Clear();

        if (_threatCarList == null)
            _threatCarList = new List<CarBase>();
        else
            _threatCarList.Clear();
    }

    void StartListeningEvents()
    {
        MyCar.DangerZone.OnAttackDetectedEvent += OnAttackDetected;
    }

    void FinishListeningEvents()
    {
        MyCar.DangerZone.OnAttackDetectedEvent -= OnAttackDetected;
    }

    public virtual void FixedUpdateFrame()
    {
        CheckHealthLimit();
    }

    IEnumerator CheckOtherCarsProgress()
    {
        while (true)
        {
            UpdateCarsInSight();
            UpdateThreatCars();

            yield return new WaitForSeconds(CHECK_OTHER_CARS_INTERVAL);
        }
    }

    void UpdateCarsInSight()
    {
        _carsInSight.Clear();

        List<CarBase> carList = GeneralCarManager.Instance.ActiveCarBaseList;

        foreach (CarBase car in carList)
        {
            if (car == MyCar)
                continue;

            if (Vector2.Distance(transform.position, car.transform.position) < CarSettings.Instance.SightRange)
                _carsInSight.Add(car);
        }
    }

    void UpdateThreatCars()
    {
        _threatCarList.Clear();

        List<CarBase> carList = GeneralCarManager.Instance.ActiveCarBaseList;

        foreach (CarBase car in carList)
        {
            if (car == MyCar)
                continue;

            if (!car.IsPlayerCar && car.CarBaseType == CarBaseType.CombatCar)
            {
                if (((CombatCarAI)car.CarAI).TargetCar == MyCar)
                    _threatCarList.Add(car);
            }
        }
    }

    void CheckHealthLimit()
    {
        if (MyCar.CurHealth < MyCar.InitHealth * CarSettings.Instance.HealthLimitPerc / 100.0f)
            _isCriticalHealth = true;
        else
            _isCriticalHealth = false;
    }

    protected void SortCarsInSightAccToDistane()
    {
        _carsInSight.OrderByDescending(val => Vector2.Distance(val.transform.position, transform.position));
    }

    protected void SortCarsInSightAccToHealth()
    {
        _carsInSight.OrderByDescending(val => val.CurHealth);
    }

    void OnAttackDetected(CarBase other, bool isEntered)
    {
        if (isEntered)
        {
            if (!_threatCarList.Contains(other))
                _threatCarList.Add(other);
        }
        else
        {
            _threatCarList.Remove(other);
        }
    }

    #region Decision Making

    protected virtual IEnumerator MakeDecisionProgress()
    {
        while (true)
        {
            if (_threatCarList.Count > 0)
            {
                EvadeThreat();
            }

            if (_isCriticalHealth)
            {
                CollectHealthPack();
            }

            if (_threatCarList.Count == 0
                && !_isCriticalHealth)
                Wander();

            yield return null;
        }
    }

    #endregion

    #region Flee Action

    protected void EvadeThreat()
    {
        foreach (CombatCarScript car in _threatCarList)
            MyCar.MovementController.Evade(car);
    }

    #endregion

    #region Wander Action

    protected void Wander()
    {
        MyCar.MovementController.Wander();
    }

    #endregion

    #region Collect HealthPack

    protected void CollectHealthPack()
    {
        HealthPack targetPack = HealthManager.Instance.GetClosestHealthPack(transform.position);

        if (targetPack == null)
            return;

        MyCar.MovementController.Seek(targetPack.transform.position, 0);
    }

    #endregion
}
