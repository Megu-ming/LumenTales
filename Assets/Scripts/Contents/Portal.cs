using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : InteractiveObj
{
    [SerializeField] SceneType nextScene;
    
    public override void OnInteraction()
    {
        DataManager.instance.BackupCurrentSlot();
        DataManager.instance.SaveAll();

        GameManager.instance.LoadScene(nextScene);
    }
}
