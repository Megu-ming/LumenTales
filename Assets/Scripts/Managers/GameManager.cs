public class GameManager
{
    public static GameManager Instance { get; } = new GameManager();

    PlayerStatus playerStatus { get; set; }
    public PlayerStatus GetStatus() => playerStatus;
    public PlayerStatus SetStatus(PlayerStatus status) => playerStatus = status;

    int statusPoint = 0;
    public int GetStatusPoint() => statusPoint;
    public bool UseStatusPoint() 
    {
        if (statusPoint > 0) { --statusPoint; return true; }
        else return false;
    }

    public void AddExp(int exp)
    {
        if(playerStatus == null) return;

        playerStatus.CurrentExp += exp;
        if(playerStatus.CurrentExp >= playerStatus.MaxExp)
        {
            playerStatus.Level++;
            UnityEngine.Debug.Log($"Player Level :{playerStatus.Level}");
            statusPoint += 5;
            playerStatus.CurrentExp -= playerStatus.MaxExp;
            playerStatus.MaxExp *= 2;
            CharacterEvents.infoUIRefresh?.Invoke();
        }
    }
}
