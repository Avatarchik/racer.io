using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class DailyQuestManager : MonoBehaviour
{
    static DailyQuestManager _instance;

    public static DailyQuestManager Instance{ get { return _instance; } }

    List<DailyQuestInfo> _dailyQuestInfoList;

    List<DailyQuestBase> _dailyQuestScriptList;

    public DailyQuestBase CurDailyQuest;

    #region Events

    public static Action OnDailyQuestSet;

    void FireOnDailyQuestSet()
    {
        if (OnDailyQuestSet != null)
            OnDailyQuestSet();
    }

    public static Action OnQuestProgressUpdated;

    void FireOnQuestProgressUpdated()
    {
        if (OnQuestProgressUpdated != null)
            OnQuestProgressUpdated();
    }

    #endregion

    void Awake()
    {
        _instance = this;

        InitDailyQuestScriptList();
        LoadDailyQuests();
    }

    void OnEnable()
    {
        PlayerProfile.DailyQuestLoaded += InitCurDailyQuest;
    }

    void OnDisable()
    {
        PlayerProfile.DailyQuestLoaded -= InitCurDailyQuest;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void InitDailyQuestScriptList()
    {
        _dailyQuestScriptList = GetComponentsInChildren<DailyQuestBase>().ToList();
    }

    void LoadDailyQuests()
    {
        _dailyQuestInfoList = DailyQuestInfoSerializer.LoadDailyQuestList();
    }

    void InitCurDailyQuest(DateTime dt, int questID, int curAmount)
    {
        int dayDifference = (DateTime.Now - dt).Days;
        int nextQuestID;
        if (dayDifference > 0)
        {
            //TODO: aradan gün geçmiş sonraki quest için infoyu burada bulup sonra init edicez
            nextQuestID = questID + dayDifference;
            curAmount = 0;

        }
        else
        {
            //TODO : hala aynı gündeyiz curAmountdan devam
            nextQuestID = questID;

        }
            
        int maxQuestID = _dailyQuestInfoList.Max(val => val.QuestID);

        if (nextQuestID > maxQuestID)
            nextQuestID = nextQuestID % maxQuestID + 1;


        DailyQuestInfo questInfo = _dailyQuestInfoList.Single(val => val.QuestID == nextQuestID);

        SetCurDailyQuest(questInfo, curAmount);

        if (dayDifference == 0)
            CheckQuestAlreadyCompleted();
    }

    void SetCurDailyQuest(DailyQuestInfo questInfo, int curAmount)
    {
        CurDailyQuest = _dailyQuestScriptList.Single(val => val.QuestType == questInfo.QuestType);

        CurDailyQuest.InitDailyQuest(questInfo, curAmount);

        if (CurDailyQuest.QuestInfo.QuestDuration == DailyQuestDuration.Session
            && CurDailyQuest.CurAmount < CurDailyQuest.QuestInfo.RequiredAmount)
            CurDailyQuest.CurAmount = 0;

        FireOnDailyQuestSet();
    }


    void CheckQuestAlreadyCompleted()
    {

        if (CurDailyQuest.CurAmount >= CurDailyQuest.QuestInfo.RequiredAmount)
        {  
            CurDailyQuest.IsCompleted = true;
        }

        FireOnQuestProgressUpdated();
    }

    public void InitNewGame()
    {
        if (CurDailyQuest.IsCompleted)
            return;

        CurDailyQuest.ActivateDailyQuest();
    }

    public void GameOver()
    {
        if (CurDailyQuest.IsCompleted)
            return;

        if (CurDailyQuest.IsCompleted)
            ExperienceManager.Instance.IncreaseExperience(ExperienceManager.ExperienceSource.QuestCompletion, 1);

        CurDailyQuest.DeactivateDailyQuest();

        FireOnQuestProgressUpdated();
    }

    public void QuestUpdated()
    {
        MessagingSystem.Instance.WriteQuestMessage();

        FireOnQuestProgressUpdated();
    }
}
