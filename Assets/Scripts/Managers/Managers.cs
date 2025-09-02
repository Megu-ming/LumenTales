using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Managers : MonoBehaviour
{
    static Managers instance;
    static bool init = false;

    #region Contents
    GameManager game = new GameManager();

    public static GameManager Game { get { return Instance?.game; } }
    #endregion

    #region Core
    DataManager data = new DataManager();
    ResourceManager resource = new ResourceManager();
    SceneManagerEx scene = new SceneManagerEx();
    SoundManager sound = new SoundManager();
    UIManager ui = new UIManager();
    public static DataManager Data { get { return Instance?.data; } }
    public static ResourceManager Resource { get { return Instance?.resource; } }
    public static SceneManagerEx Scene { get { return Instance?.scene; } }
    public static SoundManager Sound { get { return Instance?.sound; } }
    public static UIManager UI { get { return Instance?.ui; } }

    #endregion

    public static Managers Instance
    {
        get 
        {
            if(init == false)
            {
                init = true;

                GameObject go = GameObject.Find("@Managers");
                if(go == null)
                {
                    go = new GameObject { name = "@Managers" };
                    go.AddComponent<Managers>();
                }

                DontDestroyOnLoad(go);
                instance = go.GetComponent<Managers>();
            }
            return instance;
        }
    }
}
