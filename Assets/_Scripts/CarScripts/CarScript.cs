using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public enum DestroyReasonType
{
    None,
    Weapon,
    CarCrash,
    AbandonArea,
}

public enum CarControllerType
{
    Player,
    Opponent,
    NPC,
}

public class CarScript : MonoBehaviour
{
    public int InitHealth;
    public float GhostModeDuration;

    [HideInInspector]
    public bool IsActiveInGame, IsPlayerCar, IsInGhostMode, IsKing, IsRevengeActive, IsInCombat, EnteredWarningArea, EnteredAbandonArea;
    [HideInInspector]
    public int Score, StrikeCount;
    [HideInInspector]
    public CarColorEnum CarColor;
    [HideInInspector]
    public CarTypeEnum CarType;

    public CarControllerType ControllerType;

    string _playerID;

    public string PlayerID{ get { return _playerID; } }

    public GameObject PlayerArrow;
    public GameObject CrownIcon, RevengeIcon;
    public TextMesh Username;
    public CarMovementController MovementController;
    public PlayerInputController InputController;
    public CarAnimationController AnimationController;
    public CarParticleController ParticleController;
    public CarCollectibleController CollectibleController;
    public WeaponSystemController WeaponSystemController;
    public CarSoundController SoundController;
    public CarAI CarAI;

    public Transform TrailCarrier;

    public CarDangerZone DangerZone;

    bool _canGetHit;

    public bool CanGetHit{ get { return _canGetHit; } }

    int _curHealth;

    public int CurHealth { get { return _curHealth; } }

    float _curCombatDur;

    public float CurCombatDur { get { return _curCombatDur; } }

    #region Car Events

    public Action<CarScript, DestroyReasonType> OnDestroyedCar;

    void FireOnDestroyedCarEvent(CarScript car, DestroyReasonType destroyReason)
    {
        if (OnDestroyedCar != null)
            OnDestroyedCar(car, destroyReason);
    }

    public Action<CarScript, WeaponTypeEnum> OnDestroyedCarWithWeapon;

    void FireOnDestroyedCarWithWeaponEvent(CarScript car, WeaponTypeEnum weaponType)
    {
        if (OnDestroyedCarWithWeapon != null)
            OnDestroyedCarWithWeapon(car, weaponType);
    }

    public Action<CarScript, DestroyReasonType> OnGetKilled;

    void FireOnGetKilled(CarScript car, DestroyReasonType destroyReason)
    {
        if (OnGetKilled != null)
            OnGetKilled(car, destroyReason);
    }

    public Action<CarScript, WeaponTypeEnum> OnGetKilledByWeapon;

    void FireOnGetKilledByWeapon(CarScript car, WeaponTypeEnum weaponType)
    {
        if (OnGetKilledByWeapon != null)
            OnGetKilledByWeapon(car, weaponType);
    }

    public Action OnBecameKing;

    void FireOnBecameKingEvent()
    {
        if (OnBecameKing != null)
            OnBecameKing();
    }

    public Action OnLostKing;

    void FireOnLostKingEvent()
    {
        if (OnLostKing != null)
            OnLostKing();
    }

    public Action OnGetRevenge;

    void FireOnGetRevengeEvent()
    {
        if (OnGetRevenge != null)
            OnGetRevenge();
    }

    public Action OnCollectedHealthPack;

    void FireOnCollectedHealthPack()
    {
        if (OnCollectedHealthPack != null)
            OnCollectedHealthPack();
    }

    public Action<WeaponTypeEnum> OnCollectedNewWeapon;

    public void FireOnCollectedWeapon(WeaponTypeEnum weaponType)
    {
        if (OnCollectedNewWeapon != null)
            OnCollectedNewWeapon(weaponType);
    }

    public Action OnDidStrike;

    void FireOnDidStrikeEvent()
    {
        if (OnDidStrike != null)
            OnDidStrike();
    }

    #endregion

    void Awake()
    {
        SetRevenge(false);
    }

    public void SetCarController(CarControllerType controllerType)
    {
        ControllerType = controllerType;

        if (controllerType == CarControllerType.Player)
            IsPlayerCar = true;
        else
            IsPlayerCar = false;

        if (IsPlayerCar)
            PlayerArrow.SetActive(true);
        else
            PlayerArrow.SetActive(false);
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

    public void SetPlayerID(string id)
    {
        _playerID = id;
    }


    public void GetHit(BulletScript bullet)
    {
        if (!_canGetHit)
            return;
        
        if (IsPlayerCar
            || bullet.WeaponController.Car.IsPlayerCar)
            SoundController.PlayHitSound();

        DecreaseHealth(bullet.Damage);

        ParticleController.PlayHitParticle();

        if (_curHealth == 0)
        {
            GetKilled(DestroyReasonType.Weapon, bullet.WeaponController.Car);

            if (IsPlayerCar)
                FireOnGetKilledByWeapon(bullet.WeaponController.Car, bullet.WeaponController.WeaponType);
        }
    }

    public void AddScore(int score, bool coefWithStrikeCount)
    {
        if (coefWithStrikeCount)
            Score += StrikeCount * SinglePlayerArenaGameManager.Instance.ScoreOnKill;
        else
            Score += score;
    }

    public void AddStrikeCount(int amount = 1)
    {
        StrikeCount += amount;

        if (StrikeCount < 0)
            StrikeCount = 0;

        if (StrikeCount >= 2)
            FireOnDidStrikeEvent();
    }

    public void CollectedHealthPack(int healthAmount)
    {
        AddHealth(healthAmount);
        AddScore(HealthManager.Instance.BonusScore, false);

        if (IsPlayerCar)
        {
            SoundController.PlayCollectHealthPackSound();
            FireOnCollectedHealthPack();
        }
    }

    public void AddHealth(int amount)
    {
        _curHealth += amount;

        if (_curHealth > InitHealth && !IsKing)
            _curHealth = InitHealth;
        else if (_curHealth > (InitHealth * 2) && IsKing)
            _curHealth = InitHealth * 2;
    }

    public void DecreaseHealth(int amount)
    {
        _curHealth -= amount;

        if (_curHealth < 0)
            _curHealth = 0;
    }

    public void Deactivate()
    {
        SetKing(false);
        SetInCombat(false);

        AddScore(-1 * (Score / 2), false);

        WeaponSystemController.DeactivateWeaponSystemInput();

        gameObject.SetActive(false);
    }

    public void Activate(bool startInGhostMode)
    {
        _canGetHit = true;

        AnimationController.InitAnimationController();
        MovementController.ActivatePlayerMovement();
        SoundController.Activate();

        if (IsPlayerCar)
        {
            InputController.ActivateInputController();
            AnalyticsManager.Instance.SessionStarted();
        }

        EnteredWarningArea = false;
        EnteredAbandonArea = false;

        IsInGhostMode = startInGhostMode;

        gameObject.SetActive(true);

        WeaponSystemController.ActivateWeaponSystem(WeaponTypeEnum.Standard, 0);

        if (IsInGhostMode)
            StartCoroutine(ActivateRoutine());
    }

    IEnumerator ActivateRoutine()
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

        if (PlayerCarFireController.Instance.IsPressed)
            PlayerCarFireController.Instance.FireCarWeapon();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == (int)LayerEnum.Car)
        {
            CarScript otherCar = other.GetComponent<CarScript>();

            if (IsInGhostMode
                || otherCar.IsInGhostMode
                || !_canGetHit
                || !otherCar.CanGetHit)
                return;

            if (otherCar._curHealth >= _curHealth)
                DecreaseHealth(_curHealth);
            else
                DecreaseHealth(SinglePlayerArenaGameManager.Instance.CrashDamageAmount);

            if (_curHealth == 0)
            {
                GetKilled(DestroyReasonType.CarCrash, otherCar);
            }
        }
        else if(other.gameObject.layer == (int)LayerEnum.CoinCar)
        {
            if (IsInGhostMode
                || !_canGetHit)
                return;

            DecreaseHealth(_curHealth);

            if (_curHealth == 0)
                GetKilled(DestroyReasonType.CarCrash, null);
        }
    }


    public void GetKilled(DestroyReasonType reason, CarScript otherCar)
    {
        _canGetHit = false;

        AnimationController.CarRenderer.enabled = false;

        ParticleController.PlayExplodeParticle();

        SoundController.PlayExplodeSound();

        FireOnGetKilled(otherCar, reason);

        if (IsPlayerCar)
        {

            FireOnLostKingEvent();

            AnalyticsManager.Instance.SessionFinished();
        }

        CarManagerBase.BaseInstance.CarDestroyed(this, otherCar, reason);

    }

    public void DestroyedCar(DestroyReasonType reason, CarScript otherCar)
    {
        AddStrikeCount();
        SetInCombat(true);

        AddScore(SinglePlayerArenaGameManager.Instance.ScoreOnKill, true);
        AddHealth(InitHealth / 4);

        if (IsPlayerCar)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            if (PlayGameConnector.Instance.IsAuthenticated)
                PlayGameConnector.Instance.ReportKillAchievement(1);
            #endif

            FireOnDestroyedCarEvent(otherCar, reason);

            if (reason == DestroyReasonType.Weapon)
                FireOnDestroyedCarWithWeaponEvent(otherCar, WeaponSystemController.ActiveWeaponSystem.WeaponType);

            if (otherCar == CarManagerBase.BaseInstance.PlayerRevengeCar)
                FireOnGetRevengeEvent();
        }
    }

    public void SetKing(bool state)
    {
        CrownIcon.SetActive(state);

        if (state == true && !IsKing)
        {
            IsKing = state;

            AddHealth((InitHealth * 2) - _curHealth);

            if (IsPlayerCar)
                FireOnBecameKingEvent();
        }

        if (state == false && IsKing)
        {
            IsKing = state;

            float coef = _curHealth / (InitHealth * 2);

            AddHealth(Mathf.CeilToInt(InitHealth * coef) - _curHealth);

            if (IsPlayerCar)
                FireOnLostKingEvent();
        }
    }

    public void ChangeTrailColor(Color color)
    {
        //TrailRenderer.material.DOColor(color, 0.3f);
    }

    public void CarDestroyed(CarScript destroyedCar)
    {
        if (CameraFrustum.IsInCameraFrustum(destroyedCar.transform.position))
            CameraShakeScript.Instance.ShakeCamera();
    }

    public void SetRevenge(bool state)
    {
        IsRevengeActive = state;

        RevengeIcon.SetActive(state);

        if (IsRevengeActive && IsKing)
        {
            RevengeIcon.transform.localPosition = new Vector3(-0.8f, RevengeIcon.transform.localPosition.y, 0f);
            CrownIcon.transform.localPosition = new Vector3(0.8f, CrownIcon.transform.localPosition.y, 0f);
        }
        else
        {
            RevengeIcon.transform.localPosition = new Vector3(0, RevengeIcon.transform.localPosition.y, 0f);
            CrownIcon.transform.localPosition = new Vector3(0, CrownIcon.transform.localPosition.y, 0f);
        }
    }

    public void SetInCombat(bool state)
    {
        IsInCombat = state;

        _curCombatDur = 0f;

        if (!IsInCombat)
            StrikeCount = 0;
    }

    public void FixedUpdateFrame()
    {
        if (ControllerType == CarControllerType.NPC)
            CarAI.FixedUpdateFrame();
        
        InputController.FixedUpdateFrame();
        MovementController.FixedUpdateFrame();

        if (IsInCombat)
            _curCombatDur += Time.fixedDeltaTime;

        if (_curCombatDur >= SinglePlayerArenaGameManager.Instance.CombatDuration)
        {
            SetInCombat(false);
        }
    }
}