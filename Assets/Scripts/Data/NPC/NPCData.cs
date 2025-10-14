using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "NPC/NPCData")]
public class NPCData : ScriptableObject
{
    public string Name;

    public string[] conversations;
    public QuestData[] quests;
}
