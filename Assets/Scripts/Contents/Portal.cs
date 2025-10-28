using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] SceneType nextScene;
    [SerializeField] PlayerSpawnPoint nextSpawnPoint;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        if(GameManager.instance.GetDataManager() is not null)
        {
            GameManager.instance.GetDataManager().BackupCurrentSlot();
            GameManager.instance.GetDataManager().SaveAll();
        }
        if(GameManager.instance is not null)
        {
            GameManager.instance.SetSpawnPoint(nextSpawnPoint);
            GameManager.instance.LoadScene(nextScene);
        }
    }
}
