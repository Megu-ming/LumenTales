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

        if(GameManager.Instance.GetDataManager() is not null)
        {
            GameManager.Instance.GetDataManager().BackupCurrentSlot();
            GameManager.Instance.GetDataManager().SaveAll();
        }
        if(GameManager.Instance is not null)
        {
            GameManager.Instance.SetSpawnPoint(nextSpawnPoint);
            GameManager.Instance.LoadScene(nextScene);
        }
    }
}
