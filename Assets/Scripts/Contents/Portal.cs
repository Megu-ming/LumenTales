using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : InteractiveObj
{
    [SerializeField] SceneType nextScene;
    [SerializeField] PlayerSpawnPoint spawnPoint;
    
    public override void OnInteraction()
    {
        DataManager.instance.BackupCurrentSlot();
        DataManager.instance.SaveAll();

        GameManager.instance.SetSpawnPoint(spawnPoint);
        GameManager.instance.LoadScene(nextScene);
    }
}
