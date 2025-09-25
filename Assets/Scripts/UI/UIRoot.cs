using UnityEngine;

public class UIRoot : MonoBehaviour
{
    public static UIRoot instance;

    private void Awake()
    {
        InitSingleton();
    }

    void InitSingleton()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
