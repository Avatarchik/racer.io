using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ExperienceManager : MonoBehaviour
{
    public enum ExperienceSource
    {
        CarKill,
        QuestCompletion,
        CollectedhealthPack,
        CollectedNewWeapon,
        StayedAsKing,
        Strike,
    }

    static ExperienceManager _instance;

    public static ExperienceManager Instance { get { return _instance; } }

    public int CurXP { get; set; }

    public int PrevXP { get; set; }

    public int NextLevelXP { get; set; }

    public int CurLevel { get; set; }

    public int PrevLevel { get; set; }

    public float CurPercent{ get; set; }

    public float PrevPercent{ get; set; }

    public static event Action<int> LevelIncreased;
    public static event Action<int> XPIncreased;

    float _curXpCoef;

    // Use this for initialization
    void Awake()
    {
        _instance = this;

        InitXPCoef();
    }

    void InitXPCoef()
    {
        _curXpCoef = 1.0f;
    }

    void OnEnable()
    {
        PlayerProfile.ExperienceLoaded += InitLevelAndXP;
    }

    void OnDisable()
    {
        PlayerProfile.ExperienceLoaded -= InitLevelAndXP;
    }

    public void InitLevelAndXP(int level, int xp)
    {
        PrevXP = CurXP = xp;
        PrevLevel = CurLevel = level;

        CalculateCurPercent();
        PrevPercent = CurPercent;

        NextLevelXP = CalculateXPForLevel(CurLevel + 1);
    }

    void CalculateCurPercent()
    {
        int curLevelStartXP = CalculateXPForLevel(CurLevel);
        int curLevelFinishXP = CalculateXPForLevel(CurLevel + 1);

        CurPercent = (100.0f * (CurXP - curLevelStartXP) / (curLevelFinishXP - curLevelStartXP));
    }

    public int CalculateXPForLevel(int level)
    {
        level--;

        return (int)(1000.0f * (level + Utilities.Combination(level, 2)));
    }

    public void IncreaseExperience(ExperienceSource expSource, int amount)
    {
        switch (expSource)
        {
            case ExperienceSource.CarKill:
                IncreaseLevelAndXP(amount * Constants.CAR_KILL_EXP);
                break;
            case ExperienceSource.QuestCompletion:
                IncreaseLevelAndXP(amount * Constants.QUEST_COMPLETE_EXP);
                break;
            case ExperienceSource.CollectedhealthPack:
                IncreaseLevelAndXP(amount * Constants.COLLECTED_HEALTHPACK_EXP);
                break;
            case ExperienceSource.CollectedNewWeapon:
                IncreaseLevelAndXP(amount * Constants.COLLECTED_NEW_WEAPON_EXP);
                break;
            case ExperienceSource.StayedAsKing:
                IncreaseLevelAndXP(amount * Constants.STAYED_AS_KING_EXP);
                break;
            case ExperienceSource.Strike:
                IncreaseLevelAndXP(amount * Constants.STRIKE_EXP);
                break;
        }
    }

    void IncreaseLevelAndXP(int XPAmount)
    {
        PrevXP = CurXP;
        PrevLevel = CurLevel;
        PrevPercent = CurPercent;

        Debug.Log("xp coef: " + _curXpCoef);

        int nextXP = (int)((float)CurXP + ((float)XPAmount * _curXpCoef));

        int nextLevel = (int)Mathf.Floor((1.0f + Mathf.Sqrt((float)(nextXP) / 125.0f + 1.0f)) / 2.0f);

        int levelDifference = nextLevel - CurLevel;


        if (levelDifference > 0 && LevelIncreased != null)
            LevelIncreased(levelDifference);
        else if (levelDifference <= 0 && XPIncreased != null)
            XPIncreased(XPAmount);


        CurXP = nextXP;
        CurLevel = nextLevel;
        NextLevelXP = CalculateXPForLevel(CurLevel + 1);
        CheckUnlockForLevel();
        CalculateCurPercent();
    }


    void CheckUnlockForLevel()
    {
        if (CurLevel >= Constants.Phantom_Unlock_Level && !PlayerProfile.Instance.CheckIfCarUnlocked(CarTypeEnum.Buggy))
        {
            PlayerProfile.Instance.UnlockCar(CarTypeEnum.Buggy);
        }

        if (CurLevel >= Constants.Curtiss_Unlock_Level && !PlayerProfile.Instance.CheckIfCarUnlocked(CarTypeEnum.Speedy))
        {
            PlayerProfile.Instance.UnlockCar(CarTypeEnum.Speedy);
        }

        if (CurLevel >= Constants.Baron_Unlock_Level && !PlayerProfile.Instance.CheckIfCarUnlocked(CarTypeEnum.Spark))
        {
            PlayerProfile.Instance.UnlockCar(CarTypeEnum.Spark);
        }
    }

    public void SetXPCoef(float coef)
    {
        _curXpCoef *= coef;
    }

}
