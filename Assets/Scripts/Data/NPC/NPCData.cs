using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "NPC/NPCData")]
public class NPCData : ScriptableObject
{
    public string Name;
    public int id;

    public string[] conversations;
    public QuestData quest;
}
