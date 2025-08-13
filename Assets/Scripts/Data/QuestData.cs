using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Data/Quest")]
public class QuestData : ScriptableObject
{
    public string questName;
    public string description;
    public QuestGoalType goalType; // Kill, Gather, Talk
    public string targetName;
    public int targetAmount;
    public ItemData rewardItem;
    public int rewardGold;
}