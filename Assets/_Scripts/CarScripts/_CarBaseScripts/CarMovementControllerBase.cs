using UnityEngine;
using System.Collections;
using System;

public class CarMovementControllerBase : MonoBehaviour
{
    #region Event

    public Action<bool> OnDriftActive;

    void FireOnDriftActive(bool isActive)
    {
        if (OnDriftActive != null)
            OnDriftActive(isActive);
    }


    #endregion

    public CarBase MyCar;

    public Rigidbody2D Rigidbody;
    public CircleCollider2D WanderZone;
    float _wanderAngle = 0;

    Vector2 _velocity;
    float _torque;

    public Vector2 Velocity { get { return _velocity; } }

    Vector2 _totalInput;

    bool _clampToMaxSpeed;

    IEnumerator _dashRoutine;

    bool _isDrifting;

    public void ActivatePlayerMovement()
    {
        _velocity = Vector2.zero;

        _clampToMaxSpeed = true;

        SeekRandomPos();
    }

    void SeekRandomPos()
    {
        Vector2 targetPos = GameArea.Instance.GetRandomPosInEffectiveGameArea();

        Vector2 dir = targetPos - Rigidbody.position;
        dir.Normalize();

        targetPos = Rigidbody.position + dir * CarSettings.Instance.MaxSpeed;

        Seek(targetPos);
    }

    #region Seek

    public void SeekByInput(Vector2 inputDir)
    {
        _totalInput += inputDir;
    }

    public void Seek(Vector2 targetPos, float slowingRadius = CarSettings.SlowingRadius)
    { 
        _totalInput += DoSeek(targetPos, slowingRadius);
    }

    Vector2 DoSeek(Vector2 targetPos, float slowingRadius = CarSettings.SlowingRadius)
    {
        Vector2 inputDir;

        Vector2 desiredDir = targetPos - Rigidbody.position;

        desiredDir.Normalize();

        float angleBetUp = transform.right.AngleBetween(desiredDir);

        if (Mathf.Abs(angleBetUp) < 5)
        {
            inputDir = Vector2.zero;
            return inputDir;
        }

        if (angleBetUp > 0)
            inputDir = new Vector2(1, 0);
        else
            inputDir = new Vector2(-1, 0);

        return inputDir;
    }

    #endregion

    #region Flee

    public void Flee(Vector2 targetPos)
    {
        _totalInput += DoFlee(targetPos);
    }

    Vector2 DoFlee(Vector2 target)
    {
        return -DoSeek(target);
    }

    #endregion

    #region Wander

    public void Wander()
    {
        _totalInput += DoWander();
    }

    Vector2 DoWander()
    {
        Vector2 circleCenter = (Vector2)WanderZone.transform.position - Rigidbody.position;

        Vector2 displacement = new Vector2(0, WanderZone.radius * transform.localScale.x);

        displacement = GetDisplacement(displacement, _wanderAngle);

        Vector2 targetPos = (Vector2)WanderZone.transform.position + displacement;

        _wanderAngle += Utilities.NextFloat(0, 1) * CarSettings.Instance.WanderAngleChange - CarSettings.Instance.WanderAngleChange * 0.5f;

        return DoSeek(targetPos);
    }

    #endregion

    #region Evade

    public void Evade(CombatCarScript other)
    {
        _totalInput += DoEvade(other);
    }

    Vector2 DoEvade(CombatCarScript other)
    {
        float distance = Vector2.Distance(other.MovementController.Rigidbody.position, Rigidbody.position);

        int stepAhead = (int)(distance / CarSettings.Instance.MaxSpeed);

        Vector2 futurePos = (Vector2)other.MovementController.Rigidbody.position + other.MovementController.Velocity * Time.deltaTime * stepAhead;

        return DoFlee(futurePos);
    }

    #endregion

    #region Pursuit

    public void Pursuit(CarBase other)
    {
        _totalInput += DoPursuit(other);
    }

    Vector2 DoPursuit(CarBase other)
    {
        float distance = Vector2.Distance(other.MovementController.Rigidbody.position, Rigidbody.position);

        int stepAhead = (int)(distance / CarSettings.Instance.MaxSpeed);

        Vector2 futurePos = (Vector2)other.MovementController.Rigidbody.position + other.MovementController.Velocity * Time.deltaTime * stepAhead;

        return DoSeek(futurePos);
    }

    #endregion

    public void FixedUpdateFrame()
    {
        if (MyCar.ControllerType == CarControllerType.Opponent)
            return;

        if (MyCar.ControllerType == CarControllerType.NPC)
            CheckGameAreaBounds();

        CalculateFinalVelocity();
        KillOrthogonalVelocity();
        ClampVelocity();
        ApplyVelocity();
        ApplyTorque();
        ResetTotalInput();
    }

    public void ResetTotalInput()
    {
        _totalInput = Vector2.zero;
    }

    public void ResetVelocity()
    {
        _velocity = Vector2.zero;
    }

    public void SetVelocity(Vector2 vel)
    {
        _velocity = vel;
    }

    void CalculateFinalVelocity()
    {
        float accAmount = CarSettings.Instance.EasyAIAcc;

        if (MyCar.ControllerType == CarControllerType.NPC)
        {
            switch (MyCar.CarAI.AIDiff)
            {
                case AIDifficulty.Normal:
                    accAmount = CarSettings.Instance.NormalAIAcc;
                    break;
                case AIDifficulty.Hard:
                    accAmount = CarSettings.Instance.HardAIAcc;
                    break;
            }
        }
        else
        {
            accAmount = CarSettings.Instance.PlayerAcc;
        }

        _velocity += (Vector2)transform.right * accAmount * Time.fixedDeltaTime;
    }

    public void SetPosition(float x, float y)
    {
        Rigidbody.MovePosition(new Vector2(x, y));
    }

    public void SetRotation(float rotZ)
    {
        Rigidbody.MoveRotation(rotZ);
    }

    float GetCurTorque()
    {
        float torqueAmount = CarSettings.Instance.EasyAITorque;
        float speed = _velocity.magnitude;

        if (MyCar.ControllerType == CarControllerType.NPC)
        {
            switch (MyCar.CarAI.AIDiff)
            {
                case AIDifficulty.Normal:
                    torqueAmount = CarSettings.Instance.NormalAITorque;
                    break;
                case AIDifficulty.Hard:
                    torqueAmount = CarSettings.Instance.HardAITorque;
                    break;
            }
        }
        else
        {
            torqueAmount = CarSettings.Instance.PlayerTorque;
        }

        if (speed < 2.0f)
            return torqueAmount * (speed / 4.0f);

        return torqueAmount;
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.right * Vector2.Dot(_velocity, transform.right);
        Vector2 rightVelocity = transform.up * -1.0f * Vector2.Dot(_velocity, transform.up * -1.0f);
        _velocity = forwardVelocity + rightVelocity * CarSettings.Instance.DriftAmount;

        Debug.DrawRay(Rigidbody.position, forwardVelocity, Color.green);
    }

    void ClampVelocity()
    {
        _velocity = Vector2.ClampMagnitude(_velocity, CarSettings.Instance.MaxSpeed);
    }

    void ApplyVelocity()
    {
        Vector2 newPos = Rigidbody.position;
        newPos += _velocity * Time.fixedDeltaTime;

        Rigidbody.MovePosition(newPos);
    }

    void ApplyTorque()
    {
        if (Mathf.Abs(_totalInput.x) > 0)
            _torque += GetCurTorque() * -_totalInput.x;
        else if (_torque != 0)
            _torque += -1.0f * Mathf.Sign(_torque) * GetCurTorque();

        _torque = Mathf.Clamp(_torque, -CarSettings.Instance.MaxTorque, CarSettings.Instance.MaxTorque);

        float newRot = Rigidbody.rotation;

        newRot += _torque * Time.fixedDeltaTime;

        if (Mathf.Abs(_torque) > 30.0f)
            StartDrifting();
        else
            FinishDrifting();

        Rigidbody.MoveRotation(newRot);
    }

    void StartDrifting()
    {
        if (_isDrifting)
            return;

        SkidMarkManager.Instance.StartedDrifting(MyCar);

        MyCar.AnimationController.PlayAnim(CarAnimEnum.Rotate);

        _isDrifting = true;

        FireOnDriftActive(true);
    }

    void FinishDrifting()
    { 
        if (!_isDrifting)
            return;

        MyCar.AnimationController.PlayAnim(CarAnimEnum.Idle);

        _isDrifting = false;

        FireOnDriftActive(false);
    }

    void CheckGameAreaBounds()
    {
        if (GameArea.Instance.IsInEffectiveArea(Rigidbody.position))
            return;

        Vector2 posInGameArea = GameArea.Instance.GetRandomPosInEffectiveGameArea();

        Seek(posInGameArea);
    }

    #region Helper Methods

    Vector2 GetDisplacement(Vector2 displacement, float angle)
    {
        float len = displacement.magnitude;

        displacement.x = Mathf.Cos(Mathf.Deg2Rad * angle) * len;
        displacement.y = Mathf.Sin(Mathf.Deg2Rad * angle) * len;

        return displacement;
    }

    #endregion

}
