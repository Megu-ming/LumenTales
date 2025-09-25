using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] SceneType nextScene;
    
    public void OnInteraction()
    {
        GameManager.instance.LoadScene(nextScene);
    }
}
