using UnityEngine;

public class SceneBase : MonoBehaviour
{
    public SceneType sceneType;

    int statusPoint;
    public int StatusPoint
    {
        get => statusPoint;
    }

    public bool UseStatusPoint()
    {
        if (statusPoint > 0) { --statusPoint; return true; }
        else return false;
    }

    public void AddExp(int exp)
    {
        GameManager.Instance.Player.TryGetComponent<PlayerStatus>(out var status);
        if(status != null)
        {
            status.CurrentExp += exp;
            if(status.CurrentExp>=status.MaxExp)
            {
                status.Level++;
                Debug.Log($"Player Level Up! : {status.Level}");
                statusPoint += 5;
                status.CurrentExp -= status.MaxExp;
                status.MaxExp *= 2;
                CharacterEvents.infoUIRefresh?.Invoke();
            }
        }
    }
}
