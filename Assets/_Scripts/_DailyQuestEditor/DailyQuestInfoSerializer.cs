using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;
using System.IO;

public static class DailyQuestInfoSerializer
{
    const string QUEST_INFO_MAIN_FOLDER = "Assets/Resources";

    const string FOLDER_PATH = "DailyQuestInfos/";
    const string FILE_NAME = "DailyQuestInfos";

    public const string QUESTS = "Quests";
    public const string QUEST_TAG = "Quest";
    public const string QUEST_ID = "QuestId";
    public const string QUEST_TYPE = "QuestType";
    public const string QUEST_DURATION = "QuestDuration";
    public const string QUEST_GAME_MODE = "GameMode";
    public const string QUEST_REQUIRED_AMOUNT = "RequiredAmount";
    public const string ADDITIONAL_PARAMS = "AddParams";
    public const string PARAM = "Param";

    public static void SaveQuestInfoList(List<DailyQuestInfo> questInfoList)
    {
        string directoryPath = Path.Combine(QUEST_INFO_MAIN_FOLDER, FOLDER_PATH);
        Directory.CreateDirectory(directoryPath);

        string fileFullName = directoryPath + FILE_NAME + ".txt";

        string serializedQuestInfos = SerializeQuestInfoList(questInfoList);


        File.WriteAllText(fileFullName, serializedQuestInfos);
    }

    static string SerializeQuestInfoList(List<DailyQuestInfo> questInfoList)
    {
        JSONObject infosObj = new JSONObject();
        
        JSONArray infoArr = new JSONArray();

        foreach (DailyQuestInfo questInfo in questInfoList)
        {
            infoArr.Add(SerializeQuest(questInfo));
        }

        infosObj.Add(QUESTS, infoArr);

        Debug.Log(infosObj.ToString());

        return infosObj.ToString();
    }

    static JSONObject SerializeQuest(DailyQuestInfo questInfo)
    {
        JSONObject questInfoObj = new JSONObject();

        questInfoObj.Add(QUEST_ID, questInfo.QuestID);
        questInfoObj.Add(QUEST_TYPE, questInfo.QuestType.ToString());
        questInfoObj.Add(QUEST_DURATION, questInfo.QuestDuration.ToString());
        questInfoObj.Add(QUEST_GAME_MODE, questInfo.GameModeType.ToString());
        questInfoObj.Add(QUEST_REQUIRED_AMOUNT, questInfo.RequiredAmount);

        JSONArray paramObjArr = new JSONArray();

        foreach (string addParam in questInfo.AdditionalParams)
        {
            JSONObject addParamObj = new JSONObject();

            addParamObj.Add(PARAM, addParam);
            
            paramObjArr.Add(addParamObj);
        }

        questInfoObj.Add(ADDITIONAL_PARAMS, paramObjArr);

        return questInfoObj;
    }

    public static List<DailyQuestInfo> LoadDailyQuestList()
    {
        List<DailyQuestInfo> questInfoList = new List<DailyQuestInfo>();

        string fileFullName = FOLDER_PATH + FILE_NAME;

        UnityEngine.Object obj = Resources.Load(fileFullName);

        if (obj == null)
            return questInfoList;

        questInfoList = DeserializeQuestInfoList(((TextAsset)obj).ToString());

        return questInfoList;
    }

    static List<DailyQuestInfo> DeserializeQuestInfoList(string jsonText)
    {
        List<DailyQuestInfo> questInfoList = new List<DailyQuestInfo>();
        
        JSONObject infosObj = JSONObject.Parse(jsonText);

        JSONArray infoArr = infosObj.GetArray(QUESTS);

        foreach (var infoObj in infoArr)
        {
            JSONObject questObj = infoObj.Obj;

            questInfoList.Add(DeserializeQuestInfo(questObj));
        }

        return questInfoList;

    }

    static DailyQuestInfo DeserializeQuestInfo(JSONObject questInfoObj)
    {
        int questID = (int)questInfoObj.GetNumber(QUEST_ID);
        DailyQuestType questType = Utilities.IdentifyObjectEnum(questInfoObj.GetString(QUEST_TYPE), DailyQuestType.CollectHealthPack);
        DailyQuestDuration durationType = Utilities.IdentifyObjectEnum(questInfoObj.GetString(QUEST_DURATION), DailyQuestDuration.Daily);
        GameModeType gameMode = Utilities.IdentifyObjectEnum(questInfoObj.GetString(QUEST_GAME_MODE), GameModeType.All);
        int requiredAmount = (int)questInfoObj.GetNumber(QUEST_REQUIRED_AMOUNT);

        List<string> paramList = new List<string>();

        JSONArray paramObjArr = questInfoObj.GetArray(ADDITIONAL_PARAMS);

        foreach (var paramObj in paramObjArr)
        {
            JSONObject paramJSonObj = paramObj.Obj;

            paramList.Add(paramJSonObj.GetString(PARAM));
        }

        return new DailyQuestInfo(questID, questType, durationType, gameMode, requiredAmount, paramList.ToArray());

    }

}
