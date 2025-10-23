using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] SceneType nextScene;
    [SerializeField] PlayerSpawnPoint spawnPoint;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        if(DataManager.instance is not null)
        {
            DataManager.instance.BackupCurrentSlot();
            DataManager.instance.SaveAll();
        }
        if(GameManager.instance is not null)
        {
            GameManager.instance.SetSpawnPoint(spawnPoint);
            GameManager.instance.LoadScene(nextScene);
        }
    }
}
