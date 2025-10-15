using UnityEngine;

[CreateAssetMenu(fileName = "CollectQuestData", menuName = "Quest/CollectQuestData")]
public class CollectQuestData : QuestData
{
    public ItemData targetItem;     // 목표 아이템
    public int requiredItemCount;   // 목표 아이템 수량
}
