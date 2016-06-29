using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SinglePlayerArenaCarManager : CarManagerBase
{
    static SinglePlayerArenaCarManager _instance;

    public static SinglePlayerArenaCarManager Instance { get { return _instance; } }

    public int MaxAICarInGame;

    public float MinWaitTimeForSpawn, MaxWaitTimeForSpawn;

    float _lastSpawnTime, _nextWaitTimeForSpawn;

    protected const string MY_PLAYER_ID = "MyPlayer";


    protected override void Awake()
    {
        base.Awake();
        
        _instance = this;

        PlayerRevengeCar = null;

        InitVariables();

        AssignCarToMyPlayer();
    }

    void Start()
    {
        FillArea();
    }

    void InitVariables()
    {
        _lastSpawnTime = Time.realtimeSinceStartup;

        _nextWaitTimeForSpawn = Utilities.NextFloat(MinWaitTimeForSpawn, MaxWaitTimeForSpawn);
    }

    void AssignCarToMyPlayer()
    {
        CarScript myCar = AssignCarToPlayer(MY_PLAYER_ID, CarControllerType.Player);

        if (myCar == null)
            return;

        myCar.Username.text = CarSelectionWindowController.Instance.Username;
    }

    void FillArea()
    {
        float randomPassedTime = Utilities.NextFloat(20, 100);

        int spawnCount = Mathf.CeilToInt(randomPassedTime / _nextWaitTimeForSpawn);

        if (spawnCount > MaxAICarInGame)
            spawnCount = MaxAICarInGame;

        for (int i = 0; i < spawnCount; i++)
        {
            string playerID = i.ToString() + "_AI";
            
            CarScript plane = AssignCarToPlayer(playerID, CarControllerType.NPC);
           
            AssignRandomNameToAICar(plane);

            AssignRandomScoreToAICar(plane);

            ActivateAICar(plane, false, true);
        }
    }

    void AssignRandomCarTypeToAICar(CarScript plane)
    {
        var result = CarSelectionWindowController.Instance.GetRandomCarAndColor();

        plane.SetCarTypeInfo(result.Key, result.Value);
    }

    void AssignRandomNameToAICar(CarScript plane)
    {
        plane.Username.text = SinglePlayerArenaGameManager.Instance.GetRandomName();
    }

    void AssignRandomScoreToAICar(CarScript plane)
    {
        int randomScore = Utilities.NextInt(0, 500);
        plane.Score = randomScore;
    }

    public override void FixedUpdateFrame()
    {
        base.FixedUpdateFrame();

        CheckAICarSpawn();
    }

    void CheckAICarSpawn()
    {
        int aiCount = _activeCarDict.Values.Where(p => !p.IsPlayerCar).Count();


        if (aiCount < MaxAICarInGame && Time.realtimeSinceStartup - _lastSpawnTime >= _nextWaitTimeForSpawn)
        { 

            if (!_deactiveCarDict.Any(val => val.Value.ControllerType == CarControllerType.NPC))
                return;

            _lastSpawnTime = Time.realtimeSinceStartup;

            _nextWaitTimeForSpawn = Utilities.NextFloat(MinWaitTimeForSpawn, MaxWaitTimeForSpawn);

            CarScript targetCar = _deactiveCarDict.First(val => val.Value.ControllerType == CarControllerType.NPC).Value;

            ActivateAICar(targetCar, true, false);
        }
    }

    public void ActivateAICar(CarScript plane, bool isInGhostMode, bool isRandomHealth)
    {
        AssignRandomCarTypeToAICar(plane);
        
        Vector2 spawnPos = GameArea.Instance.GetRandomPosInGameArea();

        plane.SetInitPosition(spawnPos);

        plane.SetInitHealth(isRandomHealth);

        ActivateCar(plane.PlayerID, isInGhostMode);

        plane.CarAI.ActivateAI();

    }
}
