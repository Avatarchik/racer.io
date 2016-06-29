using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CarManagerBase : MonoBehaviour
{
    static CarManagerBase _baseInstance;

    public static CarManagerBase BaseInstance{ get { return _baseInstance; } }

    public GameObject CarPrefab;

    public int MaxCarInGame;

    protected List<CarScript> _availableCarList = new List<CarScript>();

    protected Dictionary<string, CarScript> _deactiveCarDict = new Dictionary<string, CarScript>();

    public Dictionary<string, CarScript> DeactiveCarDict{ get { return _deactiveCarDict; } }

    protected Dictionary<string, CarScript> _activeCarDict = new Dictionary<string, CarScript>();

    public Dictionary<string, CarScript> ActiveCarDict{ get { return _activeCarDict; } }

    protected KeyValuePair<string, CarScript> _myCar;

    public KeyValuePair<string, CarScript> MyCar{ get { return _myCar; } }

    [HideInInspector]
    public CarScript PlayerRevengeCar;

    protected virtual void Awake()
    {
        _baseInstance = this;
        
        InitCars();
    }

    public virtual void FixedUpdateFrame()
    {
        UpdateActiveCars();
    }

    protected virtual void UpdateActiveCars()
    {
        string[] keys = _activeCarDict.Keys.ToArray();

        foreach (string playerID in keys)
        {
            CarScript car = _activeCarDict[playerID];
            
            car.FixedUpdateFrame();

            CheckBordersOfCar(car);

            CheckCarLeaderboard(car);
        }
    }

    void InitCars()
    {
        for (int i = 0; i < MaxCarInGame; i++)
        {
            GameObject car = GameObject.Instantiate(CarPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            CarScript carScript = car.GetComponent<CarScript>();

            carScript.transform.parent = transform;

            AddToAvailableCarList(carScript);

            carScript.gameObject.SetActive(false);
        }
    }

    public CarScript AssignCarToPlayer(string participantID, CarControllerType controllerType)
    {
        if (_availableCarList.Count == 0)
            return null;

        CarScript car = _availableCarList[0];

        _availableCarList.Remove(car);

        _deactiveCarDict.Add(participantID, car);

        _deactiveCarDict[participantID].SetPlayerID(participantID);

        _deactiveCarDict[participantID].SetCarController(controllerType);

        if (controllerType == CarControllerType.Player)
            _myCar = new KeyValuePair<string, CarScript>(participantID, car);

        return _deactiveCarDict[participantID];
    }

    public void SetCarCarInfo(string participantID, 
                              CarColorEnum color, 
                              CarTypeEnum type, 
                              float rotZ, 
                              float posX, 
                              float posY, 
                              float velX,
                              float velY,
                              int health)
    {


        _deactiveCarDict[participantID].SetCarTypeInfo(color, type);

        _deactiveCarDict[participantID].SetCarTransform(rotZ, posX, posY);

        _deactiveCarDict[participantID].SetCarVelocity(velX, velY);

        _deactiveCarDict[participantID].SetCarHealth(health);
    }


    public void ActivateMyCar()
    {
        Vector2 spawnPos = GameArea.Instance.GetRandomPosInGameArea();

        _myCar.Value.SetInitPosition(spawnPos);

        _myCar.Value.SetInitHealth(false);

        _myCar.Value.Username.text = CarSelectionWindowController.Instance.Username;

        _myCar.Value.SetCarTypeInfo(CarSelectionWindowController.Instance.SelectedCarColor, CarSelectionWindowController.Instance.SelectedCarType);

        _myCar.Value.WeaponSystemController.ActivateWeaponSystemInput();

        ActivateCar(_myCar.Key, true);
    }

    protected void ActivateCar(string participantID, bool isInGhostMode)
    {
        _deactiveCarDict[participantID].Activate(isInGhostMode);

        AddCarToActiveCarList(participantID, _deactiveCarDict[participantID]);
    }

    void DeactivateCar(CarScript car)
    {
        if (car.IsPlayerCar)
            MainMenuScoreController.Instance.SetScore(car.Score);

        car.Deactivate();

        AddCarToDeactiveCarList(car);

        if (car.IsPlayerCar)
            SinglePlayerArenaGameManager.Instance.GameOver();
    }


    public void AddToAvailableCarList(CarScript car)
    {
        _availableCarList.Add(car);
    }

    public void AddCarToDeactiveCarList(CarScript car)
    {
        KeyValuePair<string, CarScript> item = new KeyValuePair<string,CarScript>();

        if (_activeCarDict.Any(kvp => kvp.Value == car))
        {
            item = _activeCarDict.First(kvp => kvp.Value == car);
            _activeCarDict.Remove(item.Key);
        }
            
        _deactiveCarDict.Add(item.Key, item.Value);
    }

    public void AddCarToActiveCarList(string carID, CarScript car)
    {
        KeyValuePair<string, CarScript> item = new KeyValuePair<string,CarScript>();

        if (_deactiveCarDict.ContainsKey(carID))
            _deactiveCarDict.Remove(carID);

        _activeCarDict.Add(carID, car);
    }

    void CheckBordersOfCar(CarScript car)
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

    public bool CheckEnteredAbondonArea(CarScript car)
    {
        return (!GameArea.Instance.IsInAbandonArea(car.transform.position)
        && !car.EnteredAbandonArea);
    }

    public bool CheckEnteredWarningArea(CarScript car)
    {
        return (!GameArea.Instance.IsInWarningArea(car.transform.position)
        && !car.EnteredWarningArea);
    }

    void CheckCarLeaderboard(CarScript car)
    {
        int leaderboardRank = LeaderboardController.Instance.GetLeaderboardRank(car);

        if (leaderboardRank == 1)
            car.SetKing(true);
        else
            car.SetKing(false);
    }

    public void CarDestroyed(CarScript destroyedCar, CarScript destroyerCar, DestroyReasonType destroyReason)
    {
        CheckForCamShake(destroyedCar, destroyerCar);

        DeactivateCar(destroyedCar);

        if (destroyerCar == null)
            return;

        CheckStrikeUI(destroyerCar);

        MessagingSystem.Instance.WriteKillMessage(destroyedCar, destroyerCar);

        destroyerCar.DestroyedCar(destroyReason, destroyedCar);

        SetRevenge(destroyedCar, destroyerCar);

        if (destroyedCar.IsPlayerCar)
            StartCoroutine(WaitForCheckAd());

    }

    protected void CheckForCamShake(CarScript destroyedCar, CarScript destroyerCar)
    {
        if (destroyedCar.IsPlayerCar)
            destroyedCar.CarDestroyed(destroyedCar);
        else if (destroyerCar != null && destroyerCar.IsPlayerCar)
            destroyedCar.CarDestroyed(destroyedCar);
        else
        {
            CarScript playerCar = GetPlayerCarScript();

            if (playerCar != null)
                playerCar.CarDestroyed(destroyedCar);
        }
    }

    protected void SetRevenge(CarScript destroyedCar, CarScript destroyerCar)
    {
        if (destroyedCar.IsPlayerCar)
            SetPlayerRevengeOn(destroyerCar);

        if (destroyerCar != null && destroyerCar.IsPlayerCar && destroyedCar.IsRevengeActive)
            SetPlayerRevengeOn(null);
    }

    protected void SetPlayerRevengeOn(CarScript targetCar)
    {
        if (PlayerRevengeCar != null)
            PlayerRevengeCar.SetRevenge(false);

        PlayerRevengeCar = targetCar;

        if (PlayerRevengeCar != null)
            PlayerRevengeCar.SetRevenge(true);
    }

    void CheckForWeaponDrop(CarScript destroyedCar)
    {
        if (destroyedCar.WeaponSystemController.ActiveWeaponSystem.WeaponType != WeaponTypeEnum.Standard)
            WeaponDropManager.Instance.SpawnWeapon(destroyedCar.WeaponSystemController.ActiveWeaponSystem.WeaponType, destroyedCar.transform.parent.transform.position);
    }

    protected void CheckStrikeUI(CarScript destroyerCar)
    {
        if (destroyerCar.IsPlayerCar && destroyerCar.StrikeCount > 1)
            StrikeController.Instance.ActivateStrikeUI(destroyerCar.StrikeCount);
    }

    public Transform GetPlayerCar()
    {
        CarScript car = GetPlayerCarScript();

        if (car == null)
            return null;

        return car.transform;
    }

    public CarScript GetPlayerCarScript()
    {
        if (!_activeCarDict.Values.Any(val => val.IsPlayerCar))
            return null;
        else
            return _activeCarDict.Values.Single(val => val.IsPlayerCar);
    }


    public Transform GetRandomActiveCar()
    {
        int randomIndex = Utilities.NextInt(0, _activeCarDict.Count - 1);

        return _activeCarDict.ElementAt(randomIndex).Value.transform;
    }

    public CarScript GetRandomActiveCarScript()
    {
        int randomIndex = Utilities.NextInt(0, _activeCarDict.Count - 1);

        return _activeCarDict.ElementAt(randomIndex).Value;
    }

    IEnumerator WaitForCheckAd()
    {
        yield return new WaitForSeconds(1.0f);

        HeyzapAdManager.Instance.CheckTimerToShowNextAd();
    }

}