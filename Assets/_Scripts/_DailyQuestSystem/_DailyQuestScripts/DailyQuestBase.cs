using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DailyQuestType
{
    DestroyPlane,
    CollectHealthPack,
    CollectWeapon,
    GetRevenge,
    StayAsKing,
    DoStrike,
}

public enum DailyQuestDuration
{
    Daily,
    Session,
}

public class DailyQuestInfo
{
    public int QuestID;
    public DailyQuestType QuestType;
    public DailyQuestDuration QuestDuration;
    public GameModeType GameModeType;
    public int RequiredAmount;
    public string[] AdditionalParams;

    public DailyQuestInfo(int questID, DailyQuestType questType, DailyQuestDuration questDuration, GameModeType gameModeType, int requiredAmount, params string[] list)
    {
        QuestID = questID;
        QuestType = questType;
        QuestDuration = questDuration;
        GameModeType = gameModeType;
        RequiredAmount = requiredAmount;
        AdditionalParams = list;
    }
}

public abstract class DailyQuestBase : MonoBehaviour
{
    public DailyQuestType QuestType;
    public DailyQuestInfo QuestInfo;

    public string QuestDesc;

    protected bool _isActivated;

    protected bool _isCompleted;

    public bool IsCompleted { get { return _isCompleted; } set { _isCompleted = value; } }

    [HideInInspector]
    public int CurAmount;


    protected const string QUEST_COMPLETED_DESC = "Mission Completed! Check Again Tomorrow";

    protected const string AMOUNT_TAG = "{amount}";

    protected const string IN_ONE_GAME_DESC = " in one game!";

    public virtual void InitDailyQuest(DailyQuestInfo questInfo, int curAmount)
    {
        QuestInfo = questInfo;

        CurAmount = curAmount;
    }

    public virtual void ActivateDailyQuest()
    {
        if (QuestInfo.GameModeType == GameModeType.All
            || QuestInfo.GameModeType == GameManagerBase.BaseInstance.GameModeType)
        {
            if (QuestInfo.QuestDuration == DailyQuestDuration.Session)
                CurAmount = 0;
            
            StartListeningEvents();
            _isActivated = true;

            //Debug.Log("quest activated: " + QuestType);
            //Debug.Log("target amount: " + QuestInfo.RequiredAmount + " cur amount: " + CurAmount);
        }
    }

    public virtual void DeactivateDailyQuest()
    {
        if (!_isActivated)
            return;

        FinishListeningEvents();

        _isActivated = false;

        if (QuestInfo.QuestDuration == DailyQuestDuration.Session
            && !_isCompleted)
            CurAmount = 0;
    }

    protected virtual void CheckQuestCompleted()
    {
        DailyQuestManager.Instance.QuestUpdated();

        if (CurAmount >= QuestInfo.RequiredAmount)
            QuestCompleted();
    }

    public virtual void QuestCompleted()
    {
        //Debug.Log("quest completed");
        PlayerInGameStatController.Instance.DailyQuestCompleted(QuestInfo.QuestID);

        _isCompleted = true;

        DeactivateDailyQuest();
    }

    public abstract void StartListeningEvents();

    public abstract void FinishListeningEvents();

    public virtual string GetQuestDescription()
    {
        string curQuestDesc = QuestDesc;

        curQuestDesc = curQuestDesc.Replace(AMOUNT_TAG, QuestInfo.RequiredAmount.ToString());

        if (QuestInfo.QuestDuration == DailyQuestDuration.Session)
        {
            curQuestDesc = curQuestDesc.Replace("!", "");
            curQuestDesc += IN_ONE_GAME_DESC;
        }

        if (_isCompleted)
            curQuestDesc = QUEST_COMPLETED_DESC;

        return curQuestDesc;
    }

    public virtual string GetCurrentStatus()
    {
        string status = CurAmount.ToString() + "/" + QuestInfo.RequiredAmount.ToString();

        if (_isCompleted)
            status = "";

        return status;
    }
}
