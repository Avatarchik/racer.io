using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public enum GameModeType
{
    All,
    SinglePlayerArena,
}


public class GameManagerBase : MonoBehaviour
{
    static GameManagerBase _baseInstance;

    public static GameManagerBase BaseInstance { get { return _baseInstance; } }

    protected GameModeType _gameModeType;

    public GameModeType GameModeType{ get { return _gameModeType; } }

    [HideInInspector]
    public bool IsInWatchMode;

    public int GameOverTransDuration, CrashDamageAmount, ScoreOnKill;
    public float CombatDuration;

    IEnumerator _gameOverRoutine;

    protected virtual void Awake()
    {
        _baseInstance = this;

        Utilities.Initialize();

        UIController.Instance.ShowNewGameUI();
    }

    void OnDestroy()
    {
        _baseInstance = null;
    }

    public virtual void EnterGame()
    {
        UIController.Instance.ShowInGameUI();

        GameArea.Instance.ResetAbondonAreaControllerList();

        MessagingSystem.Instance.ResetPlayerMessage();

        DailyQuestManager.Instance.InitNewGame();
        PlayerInGameStatController.Instance.InitNewGame();

        EveryplayController.StartRecording();
    }

    public void WatchGame()
    {
        IsInWatchMode = true;

        UIController.Instance.ShowWatchModeUI();
    }

    public void ExitWatch()
    {
        IsInWatchMode = false;

        CameraFollowScript.Instance.TargetCar = null;

        UIController.Instance.ShowNewGameUI();
    }

    protected virtual void FixedUpdate()
    {
        WeaponDropManager.Instance.FixedUpdateFrame();
        HealthManager.Instance.FixedUpdateFrame();
        CameraFollowScript.Instance.FixedUpdateFrame();
        LeaderboardController.Instance.FixedUpdateFrame();
        GameArea.Instance.FixedUpdateFrame();
    }

    public void GameOver()
    {

        if (_gameOverRoutine != null)
            StopCoroutine(_gameOverRoutine);

        _gameOverRoutine = GameOverRoutine();
        StartCoroutine(_gameOverRoutine);
    }

    IEnumerator GameOverRoutine()
    {

        DailyQuestManager.Instance.GameOver();
        PlayerInGameStatController.Instance.GameOver();
        
        yield return new WaitForSeconds(GameOverTransDuration);

        EveryplayController.StopRecording();
        EveryplayController.Instance.SetActive(false);


        UIController.Instance.ShowNewGameUI();

    }
}