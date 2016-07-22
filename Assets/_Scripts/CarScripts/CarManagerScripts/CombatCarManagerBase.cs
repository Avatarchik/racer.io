using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CombatCarManagerBase : MonoBehaviour
{
    static CombatCarManagerBase _baseInstance;

    public static CombatCarManagerBase BaseInstance{ get { return _baseInstance; } }

    public GameObject CarPrefab;

    public int MaxCarInGame;

    protected List<CombatCarScript> _availableCarList = new List<CombatCarScript>();

    protected Dictionary<string, CombatCarScript> _deactiveCarDict = new Dictionary<string, CombatCarScript>();

    public Dictionary<string, CombatCarScript> DeactiveCarDict{ get { return _deactiveCarDict; } }

    protected Dictionary<string, CombatCarScript> _activeCarDict = new Dictionary<string, CombatCarScript>();

    public Dictionary<string, CombatCarScript> ActiveCarDict{ get { return _activeCarDict; } }

    protected KeyValuePair<string, CombatCarScript> _myCar;

    public KeyValuePair<string, CombatCarScript> MyCar{ get { return _myCar; } }

    [HideInInspector]
    public CombatCarScript PlayerRevengeCar;

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
            CombatCarScript car = _activeCarDict[playerID];

            CheckCarLeaderboard(car);
        }
    }

    void InitCars()
    {
        for (int i = 0; i < MaxCarInGame; i++)
        {
            GameObject car = GameObject.Instantiate(CarPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            CombatCarScript carScript = car.GetComponent<CombatCarScript>();

            carScript.transform.parent = transform;

            AddToAvailableCarList(carScript);

            carScript.gameObject.SetActive(false);
        }
    }

    public CombatCarScript AssignCarToPlayer(string participantID, CarControllerType controllerType)
    {
        if (_availableCarList.Count == 0)
            return null;

        CombatCarScript car = _availableCarList[0];

        _availableCarList.Remove(car);

        _deactiveCarDict.Add(participantID, car);

        _deactiveCarDict[participantID].SetPlayerID(participantID);

        _deactiveCarDict[participantID].SetCarController(controllerType);

        if (controllerType == CarControllerType.Player)
            _myCar = new KeyValuePair<string, CombatCarScript>(participantID, car);

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

        ActivateCar(_myCar.Key, true);
    }

    protected void ActivateCar(string participantID, bool isInGhostMode)
    {
        _deactiveCarDict[participantID].Activate(isInGhostMode);

        AddCarToActiveCarList(participantID, _deactiveCarDict[participantID]);
    }

    void DeactivateCar(CarBase car)
    {
        if (car.IsPlayerCar)
            MainMenuScoreController.Instance.SetScore(((CombatCarScript)car).Score);

        car.Deactivate();

        AddCarToDeactiveCarList(car);

        if (car.IsPlayerCar)
            SinglePlayerArenaGameManager.Instance.GameOver();
    }


    public void AddToAvailableCarList(CarBase car)
    {
        _availableCarList.Add((CombatCarScript)car);
    }

    public void AddCarToDeactiveCarList(CarBase car)
    {
        KeyValuePair<string, CombatCarScript> item = new KeyValuePair<string,CombatCarScript>();

        if (_activeCarDict.Any(kvp => kvp.Value == (CombatCarScript)car))
        {
            item = _activeCarDict.First(kvp => kvp.Value == (CombatCarScript)car);
            _activeCarDict.Remove(item.Key);
        }
            
        _deactiveCarDict.Add(item.Key, item.Value);

        GeneralCarManager.Instance.UnregisterFromActiveCarList(car);
    }

    public void AddCarToActiveCarList(string carID, CombatCarScript car)
    {
        KeyValuePair<string, CombatCarScript> item = new KeyValuePair<string,CombatCarScript>();

        if (_deactiveCarDict.ContainsKey(carID))
            _deactiveCarDict.Remove(carID);

        _activeCarDict.Add(carID, (CombatCarScript)car);

        GeneralCarManager.Instance.RegisterToActiveCarList(car);

    }

    void CheckCarLeaderboard(CombatCarScript car)
    {
        int leaderboardRank = LeaderboardController.Instance.GetLeaderboardRank(car);

        if (leaderboardRank == 1)
            car.SetKing(true);
        else
            car.SetKing(false);
    }

    public void CarDestroyed(CarBase destroyedCar, CarBase destroyerCar, DestroyReasonType destroyReason)
    {
        CheckForCamShake(destroyedCar, destroyerCar);

        DeactivateCar(destroyedCar);

        if (destroyerCar == null)
            return;

        if (destroyerCar.CarBaseType == CarBaseType.CombatCar)
            CheckStrikeUI((CombatCarScript)destroyerCar);

        if (destroyerCar.CarBaseType == CarBaseType.CombatCar
            && destroyedCar.CarBaseType == CarBaseType.CombatCar)
        {
            MessagingSystem.Instance.WriteKillMessage((CombatCarScript)destroyedCar, (CombatCarScript)destroyerCar);
        }

        destroyerCar.DestroyedCar(destroyReason, destroyedCar);

        SetRevenge(destroyedCar, destroyerCar);

        if (destroyedCar.IsPlayerCar)
            StartCoroutine(WaitForCheckAd());

    }

    protected void CheckForCamShake(CarBase destroyedCar, CarBase destroyerCar)
    {
        if (destroyedCar.IsPlayerCar)
            destroyedCar.CarDestroyed(destroyedCar);
        else if (destroyerCar != null && destroyerCar.IsPlayerCar)
            destroyedCar.CarDestroyed(destroyedCar);
        else
        {
            CombatCarScript playerCar = GetPlayerCarScript();

            if (playerCar != null)
                playerCar.CarDestroyed(destroyedCar);
        }
    }

    protected void SetRevenge(CarBase destroyedCar, CarBase destroyerCar)
    {
        if (destroyerCar == null
            || destroyedCar.CarBaseType != CarBaseType.CombatCar)
            return;
        
        if (destroyedCar.IsPlayerCar)
            SetPlayerRevengeOn((CombatCarScript)destroyerCar);

        if (destroyerCar.IsPlayerCar
            && ((CombatCarScript)destroyedCar).IsRevengeActive)
            SetPlayerRevengeOn(null);
    }

    protected void SetPlayerRevengeOn(CombatCarScript targetCar)
    {
        if (PlayerRevengeCar != null)
            PlayerRevengeCar.SetRevenge(false);

        PlayerRevengeCar = targetCar;

        if (PlayerRevengeCar != null)
            PlayerRevengeCar.SetRevenge(true);
    }

    void CheckForWeaponDrop(CombatCarScript destroyedCar)
    {
        //TODO: Drop Cur weapon if not standart
    }

    protected void CheckStrikeUI(CombatCarScript destroyerCar)
    {
        if (destroyerCar.IsPlayerCar && destroyerCar.StrikeCount > 1)
            StrikeController.Instance.ActivateStrikeUI(destroyerCar.StrikeCount);
    }

    public Transform GetPlayerCar()
    {
        CombatCarScript car = GetPlayerCarScript();

        if (car == null)
            return null;

        return car.transform;
    }

    public CombatCarScript GetPlayerCarScript()
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

    public CombatCarScript GetRandomActiveCarScript()
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