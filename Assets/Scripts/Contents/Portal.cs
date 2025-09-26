using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] SceneType nextScene;
    
    public void OnInteraction()
    {
        DataManager.instance.BackupCurrentSlot();
        DataManager.instance.SaveAll();

        GameManager.instance.LoadScene(nextScene);
    }
}
