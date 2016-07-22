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

public class CombatCarAI : CarAIBase
{
    public bool IsAggresivePilot;

    CarBase _targetCar;

    public CarBase TargetCar { get { return _targetCar; } }

    public override void ActivateAI()
    {
        _targetCar = null;

        SetIfPilotAggressive();

        base.ActivateAI();
    }

    void SetIfPilotAggressive()
    {
        int rand = Utilities.NextInt(0, 2);

        if (rand == 1)
            IsAggresivePilot = true;
    }

    public override void FixedUpdateFrame()
    {
        base.FixedUpdateFrame();
        SelectTargetCar();
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

    #region Decision Making

    protected override IEnumerator MakeDecisionProgress()
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
            ((CombatCarScript)MyCar).WeaponController.Fire();
    }

    #endregion


}