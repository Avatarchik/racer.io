using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AvoEx;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Boomlagoon.JSON;
using System;

public class PlayerProfile : MonoBehaviour
{

    private static PlayerProfile _instance;

    public static PlayerProfile Instance { get { return _instance; } }

    public Dictionary<CarTypeEnum, bool> UnlockedCars;

    private KeyValuePair<string, bool> _adRemove;

    public static event Action XWingUnlocked;

    public static event Action<int, int> ExperienceLoaded;

    public static event Action<DateTime, int, int> DailyQuestLoaded;

    const string _unlockedCarsKey = "UnlockedCars";

    const string _XPKey = "Experience";

    const string _DQKey = "DailyQuest";

    void Awake()
    {
        _instance = this;
        InitUnlockedCars();
        InitAdRemoval();
        InitExperience();
        InitDailyQuest();
        LoadFile();
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void OnApplicationQuit()
    {
        SaveFile();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveFile();
        }
    }

    void InitUnlockedCars()
    {
        UnlockedCars = new Dictionary<CarTypeEnum, bool>();
        foreach (int carNumber in Enum.GetValues(typeof(CarTypeEnum)))
        {
            UnlockedCars.Add((CarTypeEnum)carNumber, false);
        }
        UnlockCar(CarTypeEnum.BigFoot);
    }

    void InitAdRemoval()
    {
        _adRemove = new KeyValuePair<string, bool>(Constants.NoAds_Product_ID, false);
    }

    void InitExperience()
    {
        ExperienceManager.Instance.InitLevelAndXP(1, 0);
    }

    DateTime _dailyQuestGivenDate;
    int _dailyQuestID, _dailyQuestCurAmount;

    void InitDailyQuest()
    {
        _dailyQuestGivenDate = DateTime.Now;
        _dailyQuestID = 1;
        _dailyQuestCurAmount = 0;
    }

    void LoadFile()
    {
        if (!File.Exists(Application.persistentDataPath + "/playerprofile.sf"))
        {
            Debug.Log("nothing to load");
            CallEventsIfNoSaveExists();
            return;
        }

        try
        {
            var jsonString = DecryptLoadFile();
            JSONObject saveObject = JSONObject.Parse(jsonString);
            if (!saveObject.ContainsKey("Version"))
            {
                V1LoadMethod(saveObject);
                CallEventsIfNoSaveExists();
            }
            else
            {
                if (CheckVersionToLoad(saveObject))
                {
                    LoadUnlockedCars(saveObject[_unlockedCarsKey]);
                    LoadAdRemoval(saveObject[Constants.NoAds_Product_ID]);
                    LoadExperience(saveObject[_XPKey]);
                    LoadDailyQuest(saveObject[_DQKey]);
                }
                else
                {
                    Debug.Log("Versions do not match, future work");
                }
            }
        }
        catch
        {
        }
    }

    private void CallEventsIfNoSaveExists()
    {
        if (DailyQuestLoaded != null)
            DailyQuestLoaded(_dailyQuestGivenDate, _dailyQuestID, _dailyQuestCurAmount);
        if (ExperienceLoaded != null)
            ExperienceLoaded(1, 0);
    }

    static bool CheckVersionToLoad(JSONObject saveObject)
    {
        return Math.Abs(saveObject["Version"].Number - Constants.Version) < Mathf.Epsilon;
    }

    void LoadUnlockedCars(JSONValue jSONValue)
    {
        foreach (KeyValuePair<string, JSONValue> kvp in jSONValue.Obj)
        {
            if (kvp.Value.Boolean)
            {
                CarTypeEnum pte = (CarTypeEnum)Enum.Parse(typeof(CarTypeEnum), kvp.Key);
                UnlockCar(pte);
            }
        }
    }

    void LoadAdRemoval(JSONValue jSONValue)
    {
        if (jSONValue.Boolean)
        {
            RemoveAds();
        }
    }

    void LoadExperience(JSONValue jSONValue)
    {
        int CurXP = (int)jSONValue.Obj["CurXP"].Number;
        int CurLevel = (int)jSONValue.Obj["CurLevel"].Number;

        if (ExperienceLoaded != null)
            ExperienceLoaded(CurLevel, CurXP);
    }

    void LoadDailyQuest(JSONValue jSONValue)
    {
        _dailyQuestGivenDate = Utilities.ConvertStringToDate(jSONValue.Obj["DailyQuestGivenDate"].Str);
        _dailyQuestID = (int)jSONValue.Obj["DailyQuestID"].Number;
        _dailyQuestCurAmount = (int)jSONValue.Obj["DailyQuestAmount"].Number;
        if (DailyQuestLoaded != null)
            DailyQuestLoaded(_dailyQuestGivenDate, _dailyQuestID, _dailyQuestCurAmount);
    }

    void V1LoadMethod(JSONObject saveObject)
    {
        foreach (KeyValuePair<string, JSONValue> kvp in saveObject)
        {
            if (kvp.Value.Boolean)
            {
                CarTypeEnum pte = (CarTypeEnum)Enum.Parse(typeof(CarTypeEnum), kvp.Key);
                UnlockCar(pte);
            }
        }
        //ClearSavefile();
    }


    static string DecryptLoadFile()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/playerprofile.sf", FileMode.Open);
        var encryptedValue = (string)bf.Deserialize(file);
        string jsonString = AesEncryptor.DecryptString(encryptedValue);
        return jsonString;
        file.Close();
    }

    void SaveFile()
    {
        try
        {
            JSONObject saveObject = new JSONObject();
            SaveCurrentVersion(ref saveObject);
            SaveUnlockedCars(ref saveObject);
            SaveAdRemoval(ref saveObject);
            SaveExperience(ref saveObject);
            SaveDailyQuest(ref saveObject);
            EncryptSaveFile(saveObject);
        }
        catch
        {
        }
    }

    static void SaveCurrentVersion(ref JSONObject saveObject)
    {
        saveObject.Add("Version", Constants.Version);
    }

    void SaveUnlockedCars(ref JSONObject saveObject)
    {
        JSONObject unlockedCars = new JSONObject();
        foreach (KeyValuePair<CarTypeEnum, bool> kvp in UnlockedCars)
        {
            KeyValuePair<string, JSONValue> newkvp = new KeyValuePair<string, JSONValue>(kvp.Key.ToString(), kvp.Value);
            unlockedCars.Add(newkvp);
        }
        saveObject.Add(_unlockedCarsKey, unlockedCars);
    }

    void SaveAdRemoval(ref JSONObject saveObject)
    {
        saveObject.Add(new KeyValuePair<string, JSONValue>(_adRemove.Key, _adRemove.Value));
    }

    void SaveExperience(ref JSONObject saveObject)
    {
        JSONObject experienceObject = new JSONObject
        {
            { "CurXP", ExperienceManager.Instance.CurXP },
            { "CurLevel", ExperienceManager.Instance.CurLevel }
        };

        saveObject.Add(_XPKey, experienceObject);
    }

    void SaveDailyQuest(ref JSONObject saveObject)
    {
        JSONObject dailyQuestObject = new JSONObject
        {
            { "DailyQuestGivenDate", Utilities.ConvertDateToString(DateTime.Now) },
            { "DailyQuestID", DailyQuestManager.Instance.CurDailyQuest.QuestInfo.QuestID },
            { "DailyQuestAmount", DailyQuestManager.Instance.CurDailyQuest.CurAmount }
        };

        saveObject.Add(_DQKey, dailyQuestObject);
    }

    static void ClearSavefile()
    {
        File.WriteAllText(Application.persistentDataPath + "/playerprofile.sf", string.Empty);
    }

    static void EncryptSaveFile(JSONObject saveObject)
    {
        string encryptedValue = AesEncryptor.Encrypt(saveObject.ToString());
        FileStream file = File.Create(Application.persistentDataPath + "/playerprofile.sf");
        BinaryFormatter vf = new BinaryFormatter();
        vf.Serialize(file, encryptedValue);
        file.Close();
    }

    public void UnlockCar(CarTypeEnum pte)
    {
        bool unlockedFirstTime = false;

        if (UnlockedCars.ContainsKey(pte))
        {
            if (!UnlockedCars[pte])
            {
                unlockedFirstTime = true;
                if (CarSelectionWindowController.Instance != null)
                    CarSelectionWindowController.Instance.UnlockCarAfterInit(pte);
            }
            UnlockedCars[pte] = true;
        }
        else
            UnlockedCars.Add(pte, true);

        if (pte == CarTypeEnum.MrGrim && XWingUnlocked != null && unlockedFirstTime)
        {
            XWingUnlocked();
        }
    }

    public bool CheckIfCarUnlocked(CarTypeEnum pte)
    {
        if (UnlockedCars.ContainsKey(pte))
        {
            return UnlockedCars[pte];
        }
        return false;
    }

    public void RemoveAds()
    {
        _adRemove = new KeyValuePair<string, bool>(Constants.NoAds_Product_ID, true);
        NoAdsButtonController.Instance.LockButton();
    }

    public bool CheckIfAdsAreRemoved()
    {
        return _adRemove.Value;
    }
}
