using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] SceneType nextScene;
    [SerializeField] PlayerSpawnPoint spawnPoint;
    
    private void OnTriggerEnter2D()
    {
        if(DataManager.instance is not null)
        {
            DataManager.instance.BackupCurrentSlot();
            DataManager.instance.SaveAll();
        }

        GameManager.instance?.SetSpawnPoint(spawnPoint);
        GameManager.instance?.LoadScene(nextScene);
    }
}
