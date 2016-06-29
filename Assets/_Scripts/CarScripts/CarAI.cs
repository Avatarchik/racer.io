using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public enum AIDifficulty
{
    Easy = 1,
    Normal,
    Hard
}

public class CarAI : MonoBehaviour
{
    public bool IsAggresivePilot;

    AIDifficulty _aiDiff;

    public AIDifficulty AIDiff{ get { return _aiDiff; } }

    public CarScript MyCar;

    List<CarScript> _carsInSight;

    CarScript _targetCar;

    public CarScript TargetCar { get { return _targetCar; } }

    List<CarScript> _threatCarList;

    bool _isCriticalHealth;

    IEnumerator _flyRoutine;

    const float CHECK_OTHER_PLANES_INTERVAL = 0.5f;

    public void ActivateAI()
    {
        _targetCar = null;

        SetIfPilotAggressive();
        SetPilotDifficulty();

        StartListeningEvents();

        InitLists();

        StartCoroutine(CheckOtherCarsProgress());
        StartCoroutine(MakeDecisionProgress());
    }

    void SetIfPilotAggressive()
    {
        int rand = Utilities.NextInt(0, 2);

        if (rand == 1)
            IsAggresivePilot = true;
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
            _carsInSight = new List<CarScript>();
        else
            _carsInSight.Clear();

        if (_threatCarList == null)
            _threatCarList = new List<CarScript>();
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

    public void FixedUpdateFrame()
    {
        CheckHealthLimit();
        SelectTargetCar();
    }

    IEnumerator CheckOtherCarsProgress()
    {
        while (true)
        {
            UpdateCarsInSight();
            UpdateThreatCars();

            yield return new WaitForSeconds(CHECK_OTHER_PLANES_INTERVAL);
        }
    }

    void UpdateCarsInSight()
    {
        _carsInSight.Clear();

        List<CarScript> carList = CarManagerBase.BaseInstance.ActiveCarDict.Values.ToList();

        foreach (CarScript car in carList)
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

        List<CarScript> carList = CarManagerBase.BaseInstance.ActiveCarDict.Values.ToList();

        foreach (CarScript car in carList)
        {
            if (car == MyCar)
                continue;

            if ((!car.IsPlayerCar && car.CarAI.TargetCar == MyCar))
                _threatCarList.Add(car);
        }
    }

    void CheckHealthLimit()
    {
        if (MyCar.CurHealth < MyCar.InitHealth * CarSettings.Instance.HealthLimitPerc / 100.0f)
            _isCriticalHealth = true;
        else
            _isCriticalHealth = false;
    }

    void SelectTargetCar()
    {
        _targetCar = null;

        if (_carsInSight.Count == 0)
        {
            _targetCar = null;
            return;
        }

        if (IsAggresivePilot)
            SortCarsInSightAccToDistane();
        else
            SortCarsInSightAccToHealth();

        _targetCar = _carsInSight.FirstOrDefault(val => !val.IsInGhostMode);

    }

    void SortCarsInSightAccToDistane()
    {
        _carsInSight.OrderByDescending(val => Vector2.Distance(val.transform.position, transform.position));
    }

    void SortCarsInSightAccToHealth()
    {
        _carsInSight.OrderByDescending(val => val.CurHealth);
    }

    void OnAttackDetected(CarScript other, bool isEntered)
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

    IEnumerator MakeDecisionProgress()
    {
        while (true)
        {
            if (_targetCar != null)
                AttackTarget();

            if (_threatCarList.Count > 0)
            {
                EvadeThreat();
            }

            if (_isCriticalHealth)
            {
                if (!IsAggresivePilot || _targetCar == null)
                    CollectHealthPack();
            }

            if (_targetCar == null
                && _threatCarList.Count == 0
                && !_isCriticalHealth)
                Wander();

            yield return null;
        }
    }

    #endregion


    #region Attack Action

    void AttackTarget()
    {
        MyCar.MovementController.Pursuit(_targetCar);

        if (!MyCar.IsInGhostMode)
            MyCar.WeaponSystemController.ActiveWeaponSystem.Fire();
    }

    #endregion

    #region Flee Action

    void EvadeThreat()
    {
        foreach (CarScript car in _threatCarList)
            MyCar.MovementController.Evade(car);
    }

    #endregion

    #region Wander Action

    void Wander()
    {
        MyCar.MovementController.Wander();
    }

    #endregion

    #region Collect HealthPack

    void CollectHealthPack()
    {
        HealthPack targetPack = HealthManager.Instance.GetClosestHealthPack(transform.position);

        if (targetPack == null)
            return;

        MyCar.MovementController.Seek(targetPack.transform.position, 0);
    }

    #endregion
}