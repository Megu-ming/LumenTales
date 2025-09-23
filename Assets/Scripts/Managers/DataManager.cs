using System;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    static GameObject container;

    static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if(!instance)
            {
                container = new GameObject();
                container.name = "DataManager";
                instance = container.AddComponent<DataManager>();

                DontDestroyOnLoad(container);
            }
            return instance;
        }
    }

    string gameDataFileName = "LumenTalesData.json";

    // 저장용 클래스
    public GameData data = new GameData();

    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + gameDataFileName;

        if (File.Exists(filePath))
        {
            string fromJsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<GameData>(fromJsonData);
            Debug.Log("불러오기 성공");
        }
    }

    public void SaveGameData() 
    {
        string toJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + gameDataFileName;

        File.WriteAllText(filePath, toJsonData);
    }
}

[Serializable]
public class GameData
{
    [Header("Player Status")]
    public int currentExp;
    public int maxExp;
    public float spAddedStr;
    public float spAddedAgi;
    public float spAddedLuk;
}