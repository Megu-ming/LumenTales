using UnityEngine;
using UnityEngine.SceneManagement;

public class UIRoot : MonoBehaviour
{
    public static UIRoot instance;

    public IngameMenu ingameMenu;

    private void Awake()
    {
        InitSingleton();

        if(ingameMenu == null)
            ingameMenu = GetComponentInChildren<IngameMenu>();
    }

    void InitSingleton()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
