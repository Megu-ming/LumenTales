using System;
using System.IO;
using UnityEngine;

[Serializable]
public class GameData
{
    // 플레이어 정보들
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    string gameDataFileName = "LumenTalesData_";

    public int CurrentSlot = -1;
    public GameData Current;

    // 저장용 클래스
    GameData data = new GameData();

    private void Awake()
    {
        InitSingleton();
    }

    public bool LoadGameData(int slot)
    {
        string filePath = Application.persistentDataPath + "/" + gameDataFileName + slot.ToString() + ".json";

        if (File.Exists(filePath))
        {
            string fromJsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<GameData>(fromJsonData);
            Debug.Log(gameDataFileName + $"{slot} 불러오기 성공");

            return true;
        }
        return false;
    }

    public void SaveGameData() 
    {
        string toJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + gameDataFileName + CurrentSlot.ToString() + ".json";

        //File.WriteAllText(filePath, toJsonData);
    }

    public GameData GetSlotData(int slot)
    {
        if (LoadGameData(slot))
            return data;
        else return null;
    }

    private void InitSingleton()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
    }
}