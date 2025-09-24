using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    SceneBase currentScene;
    public SceneBase CurrentScene
    {
        get
        {
            if(!currentScene)
            {
                currentScene = FindAnyObjectByType<SceneBase>();
            }
            return currentScene;
        }
        set 
        { }
    }

    private void Awake()
    {
        InitSingleton();
    }

    private void OnApplicationQuit()
    {
        DataManager.instance.SaveGameData();
    }

    public void StartGame(int slot)
    {
        // 해당 슬롯의 데이터 가져오기 (있건 없건)
        var data = DataManager.instance.GetSlotData(slot);

        DataManager.instance.CurrentSlot = slot; // 현재 슬롯 저장
        DataManager.instance.Current = data;
        // 있으면 그냥 데이터 넣어서 시작
        // ex) 플레이어 생성, 저장 데이터 주입, 

        
        // 없으면 튜토리얼 시작
        if(data == null)
            SceneManager.LoadScene("TownScene");
    }

    private void InitSingleton()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
