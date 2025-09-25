using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] string nextScene;
    
    public void OnInteraction()
    {
        SceneManager.LoadScene(nextScene);
    }
}
