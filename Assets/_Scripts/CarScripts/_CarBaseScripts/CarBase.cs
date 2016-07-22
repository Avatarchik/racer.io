using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum CarBaseType
{
    CombatCar,
    CoinCar,
}

public class CarBase : MonoBehaviour
{
    public int InitHealth;
    public float GhostModeDuration;

    [HideInInspector]
    public bool IsPlayerCar, IsInGhostMode, EnteredWarningArea, EnteredAbandonArea;

    public CarBaseType CarBaseType;

    [HideInInspector]
    public CarTypeEnum CarType;
    [HideInInspector]
    public CarColorEnum CarColor;

    public CarControllerType ControllerType;

    public CarMovementControllerBase MovementController;
    public CarAnimationControllerBase AnimationController;
    public CarParticleControllerBase ParticleController;
    public CarCollectibleControllerBase CollectibleController;
    public CarSoundControllerBase SoundController;
    public CarAIBase CarAI;

    public CarDangerZone DangerZone;

    protected bool _canGetHit;

    public bool CanGetHit{ get { return _canGetHit; } }

    protected int _curHealth;

    public int CurHealth { get { return _curHealth; } }

    public Transform TrailCarrier;

    #region Car Events

    public Action<CarBase, DestroyReasonType> OnDestroyedCar;

    protected void FireOnDestroyedCarEvent(CarBase car, DestroyReasonType destroyReason)
    {
        if (OnDestroyedCar != null)
            OnDestroyedCar(car, destroyReason);
    }

    public Action<CarBase, WeaponTypeEnum> OnDestroyedCarWithWeapon;

    protected void FireOnDestroyedCarWithWeaponEvent(CarBase car, WeaponTypeEnum weaponType)
    {
        if (OnDestroyedCarWithWeapon != null)
            OnDestroyedCarWithWeapon(car, weaponType);
    }

    public Action<CarBase, DestroyReasonType> OnGetKilled;

    protected void FireOnGetKilled(CarBase car, DestroyReasonType destroyReason)
    {
        if (OnGetKilled != null)
            OnGetKilled(car, destroyReason);
    }

    public Action<CarBase, WeaponTypeEnum> OnGetKilledByWeapon;

    protected void FireOnGetKilledByWeapon(CarBase car, WeaponTypeEnum weaponType)
    {
        if (OnGetKilledByWeapon != null)
            OnGetKilledByWeapon(car, weaponType);
    }

    #endregion

    public virtual void SetCarController(CarControllerType controllerType)
    {
        ControllerType = controllerType;
        
        if (controllerType == CarControllerType.Player)
            IsPlayerCar = true;
        else
            IsPlayerCar = false;
    }

    public void SetCarTypeInfo(CarColorEnum color, CarTypeEnum type)
    { 
        CarColor = color;
        CarType = type;
    }

    public void SetCarTransform(float rotZ, float posX, float posY)
    {
        MovementController.SetPosition(posX, posY);

        MovementController.SetRotation(rotZ);
    }

    public void SetCarVelocity(float velX, float velY)
    {
        MovementController.SetVelocity(new Vector2(velX, velY));
    }


    public void SetCarHealth(int health)
    {
        _curHealth = health;
    }

    public void SetInitHealth(bool isRandomHealth)
    {
        if (!isRandomHealth)
            _curHealth = InitHealth;
        else
            _curHealth = Utilities.NextInt(10, InitHealth);
    }

    public void SetInitPosition(Vector2 initPos)
    {
        transform.position = initPos;
    }

    public void GetHit(WeaponBase weapon)
    {
        if (!_canGetHit)
            return;

        if (IsPlayerCar
            || weapon.WeaponController.MyCar.IsPlayerCar)
            SoundController.PlayHitSound();

        DecreaseHealth(weapon.WeaponDamage);

        ParticleController.PlayHitParticle();

        if (_curHealth == 0)
        {
            GetKilled(DestroyReasonType.Weapon, weapon.WeaponController.MyCar);

            if (IsPlayerCar)
                FireOnGetKilledByWeapon(weapon.WeaponController.MyCar, weapon.WeaponType);
        }
    }

    public virtual void CollectedHealthPack(int healthAmount)
    {
        AddHealth(healthAmount);
    }

    public virtual void AddHealth(int amount)
    {
        _curHealth += amount;

        if (_curHealth > InitHealth)
            _curHealth = InitHealth;
        else if (_curHealth > (InitHealth * 2))
            _curHealth = InitHealth * 2;
    }

    public void DecreaseHealth(int amount)
    {
        _curHealth -= amount;

        if (_curHealth < 0)
            _curHealth = 0;
    }

    public virtual void Activate(bool startInGhostMode)
    {
        _canGetHit = true;

        AnimationController.InitAnimationController();
        MovementController.ActivatePlayerMovement();
        SoundController.Activate();

        EnteredWarningArea = false;
        EnteredAbandonArea = false;

        IsInGhostMode = startInGhostMode;

        gameObject.SetActive(true);

        if (IsInGhostMode)
            StartCoroutine(ActivateRoutine());
    }

    protected virtual IEnumerator ActivateRoutine()
    {
        float _curDur = 0f;

        while (_curDur <= GhostModeDuration)
        {
            if (AnimationController.CarSprite.color.a == 1f)
                AnimationController.CarSprite.color = new Color(1f, 1f, 1f, 0.3f);
            else
                AnimationController.CarSprite.color = new Color(1f, 1f, 1f, 1f);

            _curDur += 0.1f;

            yield return new WaitForSeconds(0.1f);
        }

        AnimationController.CarSprite.color = new Color(1f, 1f, 1f, 1f);

        IsInGhostMode = false;

    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == (int)LayerEnum.Car)
        {
            CarBase otherCar = other.GetComponent<CarBase>();

            if (IsInGhostMode
                || otherCar.IsInGhostMode
                || !_canGetHit
                || !otherCar.CanGetHit)
                return;

            if (otherCar.CurHealth >= _curHealth)
                DecreaseHealth(_curHealth);
            else
                DecreaseHealth(SinglePlayerArenaGameManager.Instance.CrashDamageAmount);

            if (_curHealth == 0)
            {
                GetKilled(DestroyReasonType.CarCrash, otherCar);
            }
        }
        else if (other.gameObject.layer == (int)LayerEnum.CoinCar)
        {
            if (IsInGhostMode
                || !_canGetHit)
                return;

            DecreaseHealth(_curHealth);

            if (_curHealth == 0)
                GetKilled(DestroyReasonType.CarCrash, null);
        }
    }


    public virtual void GetKilled(DestroyReasonType reason, CarBase otherCar)
    {
        _canGetHit = false;

        AnimationController.CarRenderer.enabled = false;

        ParticleController.PlayExplodeParticle();

        SoundController.PlayExplodeSound();

        FireOnGetKilled(otherCar, reason);
    }

    public virtual void DestroyedCar(DestroyReasonType reason, CarBase otherCar)
    {

    }

    public void CarDestroyed(CarBase destroyedCar)
    {
        if (CameraFrustum.IsInCameraFrustum(destroyedCar.transform.position))
            CameraShakeScript.Instance.ShakeCamera();
    }


    public virtual void FixedUpdateFrame()
    {
        if (ControllerType == CarControllerType.NPC)
            CarAI.FixedUpdateFrame();

        MovementController.FixedUpdateFrame();
    }
}
