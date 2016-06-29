using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using Boomlagoon.JSON;

public class QuestSelectionManager : MonoBehaviour
{
    static QuestSelectionManager _instance;

    public static QuestSelectionManager Instance{ get { return _instance; } }

    public Dropdown QuestTypeSelectionDropDown;
    public Dropdown QuestDurationSelectionDD;
    public Dropdown GameModeSelectionDD;

    public InputField AmountRequiredInputField;

    DailyQuestType _selectedQuestType;

    DailyQuestDuration _selectedDurationType;

    public DailyQuestDuration SelectedDurationType{ get { return _selectedDurationType; } }

    GameModeType _selectedGameMode;

    public GameModeType SelectedGameMode{ get { return _selectedGameMode; } }

    int _requiredAmount;

    public int RequiredAmount{ get { return _requiredAmount; } }

    public List<QuestCreationUIBase> UIList;
    QuestCreationUIBase _curUI;

    int _curQuestID;

    List<DailyQuestInfo> _questInfoList;

    void Awake()
    {
        _instance = this;

        LoadQuestInfoList();

        InitMissionTypeSelectionDropdown();
        InitDurationTypeSelectionDD();
        InitGameModeSelectionDD();
        InitAmountInputField();
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void InitAmountInputField()
    {
        AmountRequiredInputField.text = "0";

        AmountTextChanged();
    }

    void InitMissionTypeSelectionDropdown()
    {
        List<string> questTypeStringList = new List<string>();

        foreach (DailyQuestType questType in Enum.GetValues(typeof(DailyQuestType)))
        {
            questTypeStringList.Add(questType.ToString());
        }

        QuestTypeSelectionDropDown.ClearOptions();
        QuestTypeSelectionDropDown.AddOptions(questTypeStringList);

        _selectedQuestType = (DailyQuestType)QuestTypeSelectionDropDown.value;

        MissionTypeChanged();
    }

    void InitDurationTypeSelectionDD()
    {
        List<string> durationTypeStringList = new List<string>();

        foreach (DailyQuestDuration durationType in Enum.GetValues(typeof(DailyQuestDuration)))
        {
            durationTypeStringList.Add(durationType.ToString());
        }

        QuestDurationSelectionDD.ClearOptions();
        QuestDurationSelectionDD.AddOptions(durationTypeStringList);

        _selectedDurationType = (DailyQuestDuration)QuestDurationSelectionDD.value;

        DurationTypeChanged();
    }

    void InitGameModeSelectionDD()
    {
        List<string> gameModeStringList = new List<string>();

        foreach (GameModeType questType in Enum.GetValues(typeof(GameModeType)))
        {
            gameModeStringList.Add(questType.ToString());
        }

        GameModeSelectionDD.ClearOptions();
        GameModeSelectionDD.AddOptions(gameModeStringList);

        _selectedGameMode = (GameModeType)GameModeSelectionDD.value;

        GameModeChanged();
    }

    public void MissionTypeChanged()
    {
        _selectedQuestType = (DailyQuestType)QuestTypeSelectionDropDown.value;

        ActivateSelectedQuestUI();
    }

    public void DurationTypeChanged()
    {
        _selectedDurationType = (DailyQuestDuration)QuestDurationSelectionDD.value;
    }

    public void GameModeChanged()
    {
        _selectedGameMode = (GameModeType)GameModeSelectionDD.value;
    }

    public void AmountTextChanged()
    {
        try
        {
            _requiredAmount = int.Parse(AmountRequiredInputField.text);
        }
        catch
        {
            Debug.Log("Please enter a valid number");
        }
    }

    void ActivateSelectedQuestUI()
    {
        if (_curUI != null)
            _curUI.DeactivateUI();

        _curUI = null;

        if (UIList.Any(val => val.QuestType == _selectedQuestType))
        {
            _curUI = UIList.Single(val => val.QuestType == _selectedQuestType);

            _curUI.ActivateUI();
        }
    }

    public void SaveQuest()
    {
        DailyQuestInfo questInfo;

        if (_curUI != null)
            questInfo = _curUI.GetQuestInfo(_curQuestID);
        else
            questInfo = GetDefaultQuestInfo();

        _questInfoList.Add(questInfo);

        DailyQuestInfoSerializer.SaveQuestInfoList(_questInfoList);

        _curQuestID++;
    }

    void LoadQuestInfoList()
    {
        _questInfoList = DailyQuestInfoSerializer.LoadDailyQuestList();

        if (_questInfoList.Count == 0)
            _curQuestID = 0;
        else
            _curQuestID = _questInfoList.Max(val => val.QuestID);
        
        _curQuestID++;
    }

    public DailyQuestInfo GetDefaultQuestInfo()
    {
        DailyQuestInfo questInfo = new DailyQuestInfo(
                                       _curQuestID,
                                       _selectedQuestType,
                                       _selectedDurationType,
                                       _selectedGameMode,
                                       _requiredAmount);

        return questInfo;

    }
}
