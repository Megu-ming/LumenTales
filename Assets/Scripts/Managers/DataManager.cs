using System;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    string gameDataFileName = "LumenTalesData.json";

    // 저장용 클래스
    public GameData data = new GameData();

    private void Awake()
    {
        InitSingleton();
    }

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

    private void InitSingleton()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
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