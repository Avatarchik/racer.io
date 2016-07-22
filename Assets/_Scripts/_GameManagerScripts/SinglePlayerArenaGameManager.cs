using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;

public class SinglePlayerArenaGameManager : GameManagerBase
{
    static SinglePlayerArenaGameManager _instance;

    public static SinglePlayerArenaGameManager Instance { get { return _instance; } }


    List<string> _usernameList = new List<string>();
    List<string> _curUsedUsernameList = new List<string>();

    IEnumerator _gameOverRoutine;

    const string USERNAME_FILE_NAME = "usernames";

    protected override void Awake()
    {
        base.Awake();
        
        _instance = this;

        _gameModeType = GameModeType.SinglePlayerArena;

        InitUsernames();
    }

    void InitUsernames()
    {
        TextAsset usernameTextAsset = Resources.Load(USERNAME_FILE_NAME) as TextAsset;

        _usernameList = usernameTextAsset.text.Split('\n').ToList();
    }

    public override void EnterGame()
    {
        CombatCarManagerBase.BaseInstance.ActivateMyCar();

        base.EnterGame();
    }

    public string GetRandomName()
    {
        int randomIndex;
        do
        {
            randomIndex = Utilities.NextInt(0, _usernameList.Count - 1);
        } while (_curUsedUsernameList.Contains(_usernameList[randomIndex]));

        return _usernameList[randomIndex];
    }

    protected override void FixedUpdate()
    {
        GeneralCarManager.Instance.FixedUpdateFrame();
        SinglePlayerArenaCarManager.Instance.FixedUpdateFrame();
        
        base.FixedUpdate();


    }
}
